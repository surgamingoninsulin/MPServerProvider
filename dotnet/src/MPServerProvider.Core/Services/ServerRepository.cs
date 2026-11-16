using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public class ServerRepository
{
    public const int MaxServerNameLength = 64;

    private readonly AppPaths _paths;
    private readonly object _lock = new();
    private readonly object _ioLock = new();
    private List<ServerInstance> _servers = new();

    public event Action? Changed;

    public event Action<ServerInstance>? Deleted;

    public ServerRepository(AppPaths paths)
    {
        _paths = paths;
    }

    public Func<ServerInstance, bool>? InUseCheck { get; set; }

    public List<ServerInstance> Servers
    {
        get { lock (_lock) { return new List<ServerInstance>(_servers); } }
    }

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

    public ServerInstance? Find(string name)
    {
        lock (_lock)
        {
            return _servers.FirstOrDefault(s => s.ServerName == name);
        }
    }

    public string RootFor(ServerInstance server) => server.ServerRoot;

    public string NewServerDir(string name) => Path.Combine(_paths.ServersDir, Sanitize(name));

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

    public bool IsInsideServersDirs(string path)
        => _paths.AllServersDirs.Any(dir => AppPaths.IsStrictlyInside(path, dir));

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
