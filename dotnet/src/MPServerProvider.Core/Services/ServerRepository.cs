using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.ServerRepository</c> (decompile token
/// 0x02000015). Persists/loads <see cref="ServerInstance"/> records to/from
/// <c>{ServersDir}/&lt;name&gt;/server.json</c>.
/// <para>
/// HARDENING: the repository is the single owner of <see cref="ServerInstance"/> objects.
/// <see cref="Reload"/> updates the existing objects in place (matched by name) instead of
/// handing out freshly-deserialized copies on every poll, so every holder — the server list, the
/// detail panel, and the <see cref="ServerProcessService"/> entry captured at Start — sees the
/// same, latest values. Previously a stop/exit saved the stale instance captured at Start and
/// silently reverted settings saved while the server was running. All writes are atomic
/// (temp file + rename) and serialized under a lock.
/// </para>
/// </summary>
public class ServerRepository
{
    /// <summary>Longest server name accepted by <see cref="ValidateServerName"/>.</summary>
    public const int MaxServerNameLength = 64;

    private readonly AppPaths _paths;
    private readonly object _lock = new();
    private readonly object _ioLock = new();
    private List<ServerInstance> _servers = new();

    /// <summary>Port of <c>add_Changed(Action)</c>'s backing event.</summary>
    public event Action? Changed;

    /// <summary>Port of <c>add_Deleted(Action&lt;ServerInstance&gt;)</c>'s backing event.</summary>
    public event Action<ServerInstance>? Deleted;

    /// <summary>Port of <c>.ctor(AppPaths)</c>.</summary>
    public ServerRepository(AppPaths paths)
    {
        _paths = paths;
    }

    /// <summary>
    /// Optional "is this server's process alive?" hook (wired by the UI layer to
    /// <see cref="ServerProcessService.StateOf"/>). When it returns <c>true</c>,
    /// <see cref="Delete"/> refuses instead of deleting files out from under a live process.
    /// </summary>
    public Func<ServerInstance, bool>? InUseCheck { get; set; }

    /// <summary>Port of <c>get_Servers()</c>.</summary>
    public List<ServerInstance> Servers
    {
        get { lock (_lock) { return new List<ServerInstance>(_servers); } }
    }

    /// <summary>
    /// Port of <c>Reload()</c>: (re)loads every <c>*/server.json</c> under every known servers
    /// folder (<see cref="AppPaths.AllServersDirs"/> — the current install location plus every
    /// previous one, so changing the install drive never hides existing servers). Existing
    /// instances are updated in place; a <c>server.json</c> that exists but can't be read right
    /// now (locked, mid-copy) keeps its last known instance rather than vanishing from the list.
    /// </summary>
    public void Reload()
    {
        var loaded = new List<ServerInstance>();
        var unreadable = new List<string>();

        foreach (var serversDir in _paths.AllServersDirs)
        {
            string[] dirs;
            try
            {
                if (!Directory.Exists(serversDir))
                {
                    continue;
                }

                dirs = Directory.GetDirectories(serversDir);
            }
            catch
            {
                // Drive removed / access denied: skip this location, keep scanning the others.
                continue;
            }

            foreach (var dir in dirs)
            {
                var serverJson = Path.Combine(dir, "server.json");
                ServerInstance? instance;
                try
                {
                    if (!File.Exists(serverJson))
                    {
                        continue;
                    }

                    instance = Json.Json.Deserialize<ServerInstance>(File.ReadAllText(serverJson));
                }
                catch
                {
                    unreadable.Add(serverJson);
                    continue;
                }

                if (instance is null || string.IsNullOrWhiteSpace(instance.ServerName))
                {
                    continue;
                }

                if (loaded.Any(s => string.Equals(s.ServerName, instance.ServerName, StringComparison.OrdinalIgnoreCase)))
                {
                    // Same name in two install locations: first (current location) wins.
                    continue;
                }

                instance.SourceFile = serverJson;
                RollMonthCounter(instance);
                var migrated = MigrateCfxToTxAdmin(instance);
                migrated |= MigrateDuplicatedServerFileStartCommand(instance);
                migrated |= MigrateMinecraftJavaMajor(instance);
                migrated |= MigrateStaleStartCommand(instance);
                if (migrated)
                {
                    try
                    {
                        WriteFile(serverJson, instance);
                    }
                    catch
                    {
                        // Best-effort migration write.
                    }
                }

                loaded.Add(instance);
            }
        }

        lock (_lock)
        {
            var merged = new List<ServerInstance>(loaded.Count);
            foreach (var fresh in loaded)
            {
                var existing = _servers.FirstOrDefault(s => s.ServerName == fresh.ServerName);
                if (existing is null)
                {
                    merged.Add(fresh);
                }
                else
                {
                    CopyInto(fresh, existing);
                    merged.Add(existing);
                }
            }

            // Keep (unchanged) any instance whose file exists but couldn't be read this time.
            foreach (var path in unreadable)
            {
                var kept = _servers.FirstOrDefault(s =>
                    s.SourceFile is not null && AppPaths.SamePath(s.SourceFile, path));
                if (kept is not null && !merged.Contains(kept))
                {
                    merged.Add(kept);
                }
            }

            _servers = merged;
        }

        Changed?.Invoke();
    }

    /// <summary>Port of <c>Find(string name) -&gt; ServerInstance?</c>.</summary>
    public ServerInstance? Find(string name)
    {
        lock (_lock)
        {
            return _servers.FirstOrDefault(s => s.ServerName == name);
        }
    }

    /// <summary>
    /// Port of <c>RootFor(ServerInstance) -&gt; string</c>.
    /// INFERRED: the decompiled body calls <c>AppPaths.get_DataRoot()</c> then
    /// <c>ServerInstance.ResolveRoot(...)</c>, but <see cref="ServerInstance.ResolveRoot"/> (built
    /// by a different task) resolves a <i>path</i> against <c>ServerRoot</c>, not <c>ServerRoot</c>
    /// against a base — the two don't compose the same way as the original's exact overload.
    /// Since every real <c>server.json</c> sample stores an absolute <c>ServerRoot</c> already,
    /// the safe, always-correct choice (matching the already-verified Rust port) is to just
    /// return it directly.
    /// </summary>
    public string RootFor(ServerInstance server) => server.ServerRoot;

    /// <summary>Port of <c>NewServerDir(string name) -&gt; string</c>.</summary>
    public string NewServerDir(string name) => Path.Combine(_paths.ServersDir, Sanitize(name));

    /// <summary>
    /// Port of <c>NameTaken(string name) -&gt; bool</c>. Case-insensitive, matching both the
    /// Windows file system and <see cref="ServerProcessService"/>'s running-server table.
    /// </summary>
    public bool NameTaken(string name)
    {
        lock (_lock)
        {
            if (_servers.Any(s => string.Equals(s.ServerName, name, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        return Directory.Exists(NewServerDir(name));
    }

    /// <summary>
    /// <c>true</c> when <paramref name="path"/> is strictly inside one of the app's servers
    /// folders (<see cref="AppPaths.AllServersDirs"/>).
    /// </summary>
    public bool IsInsideServersDirs(string path)
        => _paths.AllServersDirs.Any(dir => AppPaths.IsStrictlyInside(path, dir));

    /// <summary>
    /// Validates a user-supplied server name. Returns <c>null</c> when acceptable, otherwise a
    /// user-facing reason. Server names are substituted into launch arguments
    /// (<c>{server_name}</c>) and config files, so quotes and control characters (which could
    /// inject extra arguments or config keys) are rejected outright.
    /// </summary>
    public static string? ValidateServerName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Server name can't be empty.";
        }

        if (name.Length > MaxServerNameLength)
        {
            return $"Server name must be {MaxServerNameLength} characters or fewer.";
        }

        if (name.Any(c => c == '"' || char.IsControl(c)))
        {
            return "Server name can't contain quotes (\") or control characters.";
        }

        if (name.Trim().Trim('.').Length == 0)
        {
            return "That server name isn't allowed.";
        }

        return null;
    }

    /// <summary>
    /// Port of <c>Save(ServerInstance)</c>: writes <c>server.json</c>, assigning a fresh
    /// <c>SourceFile</c>/<c>ServerRoot</c> the first time a given instance is saved.
    /// <para>
    /// If a different object with the same name is already owned by the repository, its values
    /// are copied into the owned instance (which is then what gets persisted and kept), so
    /// references held elsewhere never go stale.
    /// </para>
    /// </summary>
    public void Save(ServerInstance server)
    {
        ServerInstance target;
        lock (_lock)
        {
            var existing = _servers.FirstOrDefault(s => s.ServerName == server.ServerName);
            if (existing is not null && !ReferenceEquals(existing, server))
            {
                if (string.IsNullOrEmpty(server.SourceFile))
                {
                    server.SourceFile = existing.SourceFile;
                }

                CopyInto(server, existing);
                target = existing;
            }
            else
            {
                target = server;
            }
        }

        if (string.IsNullOrEmpty(target.SourceFile))
        {
            var dir = NewServerDir(target.ServerName);
            var candidate = Path.Combine(dir, "server.json");
            if (File.Exists(candidate))
            {
                // Never silently overwrite another server's record (e.g. an import whose
                // de-duplicated name still maps onto an existing folder).
                throw new InvalidOperationException($"A server folder for '{target.ServerName}' already exists.");
            }

            Directory.CreateDirectory(dir);
            target.SourceFile = candidate;
            if (string.IsNullOrEmpty(target.ServerRoot))
            {
                target.ServerRoot = dir;
            }
        }

        WriteFile(target.SourceFile!, target);

        lock (_lock)
        {
            if (!_servers.Contains(target))
            {
                _servers.RemoveAll(s => s.ServerName == target.ServerName);
                _servers.Add(target);
            }
        }

        Changed?.Invoke();
    }

    /// <summary>
    /// Changes only the statistics of the owned instance named <paramref name="name"/> and
    /// persists it (atomically, under the repository's write lock). Returns <c>false</c> when no
    /// such server is known. Use this from background threads (process exit handlers) instead of
    /// <see cref="Save"/>-ing an instance captured earlier.
    /// </summary>
    public bool UpdateStats(string name, Action<ServerStats> mutate)
    {
        ServerInstance? owned;
        lock (_lock)
        {
            owned = _servers.FirstOrDefault(s => s.ServerName == name);
            if (owned is null)
            {
                return false;
            }

            mutate(owned.Stats);
        }

        if (!string.IsNullOrEmpty(owned.SourceFile))
        {
            WriteFile(owned.SourceFile!, owned);
        }

        Changed?.Invoke();
        return true;
    }

    /// <summary>
    /// Port of <c>Delete(ServerInstance, bool deleteFiles)</c>.
    /// <para>
    /// HARDENING: refuses while the server's process is alive (<see cref="InUseCheck"/>), refuses
    /// to delete any folder that isn't strictly inside one of the app's servers folders (an
    /// imported/tampered <c>server.json</c> could otherwise point <c>server_root</c> at e.g.
    /// <c>Documents</c>), surfaces a failed deletion as an exception, and only forgets the
    /// server once its files are actually gone.
    /// </para>
    /// </summary>
    public void Delete(ServerInstance server, bool deleteFiles)
    {
        if (InUseCheck?.Invoke(server) == true)
        {
            throw new InvalidOperationException($"'{server.ServerName}' is still running. Stop it before deleting it.");
        }

        if (deleteFiles)
        {
            var root = RootFor(server);
            var registration = string.IsNullOrEmpty(server.SourceFile) ? null : Path.GetDirectoryName(server.SourceFile);
            if (!string.IsNullOrWhiteSpace(root) && !IsInsideServersDirs(root)
                && registration is not null && IsInsideServersDirs(registration) && !AppPaths.SamePath(registration, root))
            {
                // Imported "where it is": the server's files belong to the user and stay; only
                // the registration folder the app created is removed.
                ForceDeleteDirectory(registration);
            }
            else if (!string.IsNullOrWhiteSpace(root) && Directory.Exists(root))
            {
                if (!IsInsideServersDirs(root))
                {
                    throw new InvalidOperationException(
                        $"Refusing to delete '{root}': it is outside the servers folder. Remove those files manually if you really want them gone.");
                }

                ForceDeleteDirectory(root);
            }
        }
        else if (!string.IsNullOrEmpty(server.SourceFile) && File.Exists(server.SourceFile))
        {
            try
            {
                File.Delete(server.SourceFile);
            }
            catch
            {
                // Best-effort cleanup.
            }
        }

        lock (_lock)
        {
            _servers.RemoveAll(s => s.ServerName == server.ServerName);
        }

        Deleted?.Invoke(server);
        Changed?.Invoke();
    }

    private void WriteFile(string path, ServerInstance server)
    {
        lock (_ioLock)
        {
            string json;
            lock (_lock)
            {
                json = Json.Json.Serialize(server);
            }

            Json.Json.WriteAllTextAtomic(path, json);
        }
    }

    /// <summary>Copies every persisted field of <paramref name="source"/> onto <paramref name="target"/>.</summary>
    private static void CopyInto(ServerInstance source, ServerInstance target)
    {
        target.ManifestVersion = source.ManifestVersion;
        target.ServerGame = source.ServerGame;
        target.ServerGenres = source.ServerGenres;
        target.ServerVersion = source.ServerVersion;
        target.ServerType = source.ServerType;
        target.ServerName = source.ServerName;
        target.ServerDescription = source.ServerDescription;
        target.ServerRoot = source.ServerRoot;
        target.ServerFile = source.ServerFile;
        target.ServerIp = source.ServerIp;
        target.ServerPort = source.ServerPort;
        target.RamMinMb = source.RamMinMb;
        target.RamMaxMb = source.RamMaxMb;
        target.JavaMajor = source.JavaMajor;
        target.StartCommand = source.StartCommand;
        target.StopCommand = source.StopCommand;
        target.ModsFolder = source.ModsFolder;
        target.AutoRestartOnCrash = source.AutoRestartOnCrash;
        target.CreateFirewallRule = source.CreateFirewallRule;
        target.Tunnel = source.Tunnel;
        target.Stats = source.Stats;
        target.SourceFile = source.SourceFile;
    }

    /// <summary>
    /// Port of <c>MigrateCfxToTxAdmin(ServerInstance)</c>. Returns <c>true</c> if the instance was
    /// modified (caller is responsible for persisting it).
    /// </summary>
    private static bool MigrateCfxToTxAdmin(ServerInstance server)
    {
        if (server.ServerGame != "fivem" && server.ServerGame != "redm")
        {
            return false;
        }

        if (!server.StartCommand.Contains("+exec server.cfg"))
        {
            return false;
        }

        server.StartCommand = server.StartCommand.Replace("+exec server.cfg", string.Empty).TrimEnd();
        return true;
    }

    /// <summary>
    /// One-time repair for servers installed before a fix to <c>InstallService.BuildInstance</c>'s
    /// default <c>StartCommand</c> construction: it used to prepend
    /// <c>"{resolved server file}"</c> in front of <c>game.Runtime.Arguments</c> unconditionally,
    /// which for any template that ALSO references <c>{server_file}</c> itself (e.g. Minecraft
    /// Java's <c>-jar "{server_file}"</c>) produced a garbled, unlaunchable command like
    /// <c>"server.jar" -Xms{ram_min_mb}M -Xmx{ram_max_mb}M -jar "{server_file}" nogui</c>. Detects
    /// that exact shape (a leading quoted literal with no placeholder braces in it, followed later
    /// by a real <c>{server_file}</c> reference) and strips the bogus leading quoted token so
    /// already-installed servers don't need to be reinstalled to pick up the real fix. Returns
    /// <c>true</c> if the instance was modified (caller is responsible for persisting it).
    /// </summary>
    private static bool MigrateDuplicatedServerFileStartCommand(ServerInstance server)
    {
        var command = server.StartCommand;
        if (string.IsNullOrEmpty(command) || !command.StartsWith('"'))
        {
            return false;
        }

        var closingQuote = command.IndexOf('"', 1);
        if (closingQuote < 0)
        {
            return false;
        }

        var leadingToken = command[1..closingQuote];
        var rest = command[(closingQuote + 1)..].TrimStart();

        // The leading quoted token must be a plain literal (no template placeholder) and the
        // remainder must contain its own real {server_file} reference — that combination only
        // happens from the old buggy default construction, never from a legitimately
        // provider-supplied start command.
        if (leadingToken.Contains('{') || !rest.Contains("{server_file}"))
        {
            return false;
        }

        server.StartCommand = rest;
        return true;
    }

    /// <summary>
    /// Repair for Minecraft Java servers installed while the provider read Mojang's
    /// <c>javaVersion</c> from the wrong JSON path: every install fell back to the version-number
    /// heuristic, which mapped the year-numbered 26.x releases to Java 8, so those servers could
    /// never start (UnsupportedClassVersionError). Only ever RAISES a recorded
    /// <see cref="ServerInstance.JavaMajor"/> to what the server's version needs at minimum, so a
    /// deliberately older runtime (e.g. Java 8 for Forge 1.12) is never touched. Returns
    /// <c>true</c> if the instance was modified.
    /// </summary>
    internal static bool MigrateMinecraftJavaMajor(ServerInstance server)
    {
        if (server.ServerGame != "minecraft-java" || server.JavaMajor <= 0 || string.IsNullOrWhiteSpace(server.ServerVersion)
            || server.ServerType is "velocity" or "bungeecord")
        {
            return false;
        }

        var needed = Providers.MinecraftJavaProvider.GuessJavaMajor(server.ServerVersion);
        if (needed <= server.JavaMajor)
        {
            return false;
        }

        server.JavaMajor = needed;
        return true;
    }

    /// <summary>
    /// Installed servers keep a copy of the definition's launch arguments in
    /// <see cref="ServerInstance.StartCommand"/>. When that copy is still EXACTLY a known-broken
    /// built-in default (<see cref="GameLibraryService.StaleLaunchDefinitions"/>, e.g. PixARK
    /// without its CubePort), replace it with the current built-in arguments; anything the user
    /// edited is left alone. Returns <c>true</c> if the instance was modified.
    /// </summary>
    internal static bool MigrateStaleStartCommand(ServerInstance server)
    {
        if (string.IsNullOrEmpty(server.ServerGame)
            || !GameLibraryService.StaleLaunchDefinitions.TryGetValue(server.ServerGame, out var stale)
            || !stale.Any(s => s.Arguments == server.StartCommand))
        {
            return false;
        }

        var current = BuiltInGames.All().FirstOrDefault(g => g.Id == server.ServerGame)?.Runtime.Arguments;
        if (string.IsNullOrEmpty(current) || current == server.StartCommand)
        {
            return false;
        }

        server.StartCommand = current;
        return true;
    }

    /// <summary>
    /// Port of <c>ForceDeleteDirectory(string)</c>: clears read-only attributes then retries
    /// <c>Directory.Delete(recursive: true)</c> a few times (handles transient <c>AccessDenied</c>
    /// from an antivirus/indexer holding a handle). HARDENING: the final failure is surfaced as an
    /// <see cref="IOException"/> with a clear message instead of being swallowed (which used to
    /// report "deleted" for a half-deleted folder).
    /// </summary>
    private static void ForceDeleteDirectory(string dir)
    {
        ClearReadOnlyAttributes(dir);
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                Directory.Delete(dir, recursive: true);
                return;
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            catch (Exception) when (attempt < 4)
            {
                Thread.Sleep(100 * (attempt + 1));
            }
            catch (Exception ex)
            {
                throw new IOException(
                    $"Could not delete '{dir}' completely ({ex.Message}). A file is probably still in use — close any program using it and try again.",
                    ex);
            }
        }
    }

    /// <summary>
    /// Port of <c>ClearReadOnlyAttributes(string)</c>. Never follows junctions/symlinks, so it
    /// can't touch attributes of files outside the folder being deleted.
    /// </summary>
    private static void ClearReadOnlyAttributes(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }

        try
        {
            foreach (var entry in Directory.GetFileSystemEntries(dir))
            {
                FileAttributes attrs;
                try
                {
                    attrs = File.GetAttributes(entry);
                    if (attrs.HasFlag(FileAttributes.ReadOnly))
                    {
                        File.SetAttributes(entry, attrs & ~FileAttributes.ReadOnly);
                    }
                }
                catch
                {
                    // Best-effort.
                    continue;
                }

                if (attrs.HasFlag(FileAttributes.Directory) && !attrs.HasFlag(FileAttributes.ReparsePoint))
                {
                    ClearReadOnlyAttributes(entry);
                }
            }
        }
        catch
        {
            // Best-effort.
        }
    }

    /// <summary>
    /// Port of <c>Sanitize(string) -&gt; string</c>: replaces every
    /// <c>Path.GetInvalidFileNameChars()</c> character with <c>_</c>, trims, and falls back to
    /// <c>"server"</c> if the result is blank.
    /// </summary>
    private static string Sanitize(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new char[name.Length];
        for (var i = 0; i < name.Length; i++)
        {
            cleaned[i] = invalid.Contains(name[i]) ? '_' : name[i];
        }

        var trimmed = new string(cleaned).Trim().TrimEnd('.');
        return trimmed.Length == 0 || trimmed == "." || trimmed == ".." ? "server" : trimmed;
    }

    /// <summary>
    /// Port of <c>RollMonthCounter(ServerInstance)</c>: resets <c>MonthUptimeSeconds</c> whenever
    /// the current <c>yyyy-MM</c> doesn't match the stored <c>MonthKey</c>.
    /// </summary>
    private static void RollMonthCounter(ServerInstance server)
    {
        var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");
        if (server.Stats.MonthKey != currentMonth)
        {
            server.Stats.MonthKey = currentMonth;
            server.Stats.MonthUptimeSeconds = 0;
        }
    }
}
