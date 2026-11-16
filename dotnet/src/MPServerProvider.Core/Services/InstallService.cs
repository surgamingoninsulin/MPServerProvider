using MPServerProvider.Core.Models;
using MPServerProvider.Core.Providers;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.InstallService</c> (decompile token
/// 0x0200000e). Orchestrates: pick the right <see cref="IInstallProvider"/> by the game's
/// <c>Install.Provider</c> field, drive the provider's install, deploy any configured Steam
/// Workshop items, then build and persist the resulting <see cref="ServerInstance"/> via
/// <see cref="ServerRepository"/>.
/// </summary>
public class InstallService
{
    private readonly Dictionary<string, IInstallProvider> _providers;
    private readonly ServerRepository _servers;
    private readonly JavaRuntimeProvider _javaRuntime;
    private readonly WorkshopService _workshop;

    /// <summary>
    /// Port of <c>.ctor(AppPaths, ServerRepository, DownloadService)</c>. Builds every concrete
    /// <see cref="IInstallProvider"/> and indexes them by <see cref="IInstallProvider.Id"/>,
    /// mirroring the decompiled ctor's <c>.ToDictionary(p =&gt; p.Id)</c> over a fixed array of 7
    /// provider instances.
    /// </summary>
    public InstallService(AppPaths paths, ServerRepository servers)
    {
        _servers = servers;

        IInstallProvider[] providers =
        {
            new MinecraftJavaProvider(),
            new BedrockProvider(),
            new CfxProvider(),
            new BeamMpProvider(),
            new SteamCmdProvider(),
            new UrlProvider(),
            new ManualProvider(),
        };
        _providers = providers.ToDictionary(p => p.Id);

        _workshop = new WorkshopService(paths);
        _javaRuntime = new JavaRuntimeProvider();
    }

    /// <summary>Port of <c>get_JavaRuntime()</c>.</summary>
    public JavaRuntimeProvider JavaRuntime => _javaRuntime;

    /// <summary>Port of <c>get_Workshop()</c>.</summary>
    public WorkshopService Workshop => _workshop;

    /// <summary>
    /// Port of <c>ProviderFor(GameDefinition) -&gt; IInstallProvider</c>: looks up
    /// <c>game.Install.Provider</c> in the provider map, falling back to the <c>"url"</c>
    /// provider (matching the decompiled body's <c>TryGetValue(...) ?? _providers["url"]</c>-shaped
    /// fallback) when the field is unset or names an unknown provider.
    /// </summary>
    public IInstallProvider ProviderFor(GameDefinition game)
    {
        var id = game.Install.Provider ?? "url";
        if (_providers.TryGetValue(id, out var provider))
        {
            return provider;
        }

        return _providers["url"];
    }

    /// <summary>Port of <c>GetVersionsAsync(GameDefinition, string?, CancellationToken)</c>.</summary>
    public Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct = default)
        => ProviderFor(game).GetVersionsAsync(game, variant, ct);

    /// <summary>
    /// Port of <c>InstallAsync(GameDefinition, string variant, string version, string serverName,
    /// int port, int ramMin, int ramMax, string description, bool eulaAccepted,
    /// IProgress&lt;InstallProgress&gt;, CancellationToken) -&gt; Task&lt;ServerInstance&gt;</c>.
    /// <para>
    /// Resolves a fresh target directory via <see cref="ServerRepository.NewServerDir"/>, builds
    /// an <see cref="InstallRequest"/>, hands it to <see cref="ProviderFor"/>'s provider, deploys
    /// any configured Steam Workshop items, then <see cref="BuildInstance"/>s and
    /// <see cref="ServerRepository.Save"/>s the result.
    /// </para>
    /// </summary>
    public async Task<ServerInstance> InstallAsync(
        GameDefinition game,
        string? variant,
        string? version,
        string serverName,
        int port,
        int ramMinMb,
        int ramMaxMb,
        string? description,
        bool eulaAccepted,
        IProgress<InstallProgress> progress,
        CancellationToken ct = default)
    {
        if (ServerRepository.ValidateServerName(serverName) is { } nameError)
        {
            throw new InvalidOperationException(nameError);
        }

        if (_servers.NameTaken(serverName))
        {
            throw new InvalidOperationException($"A server named '{serverName}' already exists.");
        }

        var targetDir = _servers.NewServerDir(serverName);
        Directory.CreateDirectory(targetDir);

        try
        {
            return await InstallIntoAsync(game, variant, version, serverName, port, ramMinMb, ramMaxMb, description, eulaAccepted, targetDir, progress, ct)
                .ConfigureAwait(false);
        }
        catch
        {
            // The folder was created fresh above (NameTaken guarantees it didn't exist), so it is
            // safe to remove. Leaving it behind used to waste gigabytes and permanently reserve
            // the name (NameTaken checks Directory.Exists).
            TryDeleteDirectory(targetDir);
            throw;
        }
    }

    private async Task<ServerInstance> InstallIntoAsync(
        GameDefinition game,
        string? variant,
        string? version,
        string serverName,
        int port,
        int ramMinMb,
        int ramMaxMb,
        string? description,
        bool eulaAccepted,
        string targetDir,
        IProgress<InstallProgress> progress,
        CancellationToken ct)
    {
        var request = new InstallRequest
        {
            Game = game,
            ServerName = serverName,
            Variant = variant,
            Version = version,
            TargetDir = targetDir,
            Port = port,
            RamMinMb = ramMinMb,
            RamMaxMb = ramMaxMb,
            Description = description,
            EulaAccepted = eulaAccepted,
        };

        var provider = ProviderFor(game);
        var result = await provider.InstallAsync(request, progress, ct).ConfigureAwait(false);

        if (game.Install.WorkshopIds.Count > 0)
        {
            _workshop.DeployToServer(game, request.TargetDir);
        }

        ct.ThrowIfCancellationRequested();
        var instance = BuildInstance(game, request, result);
        _servers.Save(instance);
        return instance;
    }

    /// <summary>
    /// Server config files an update must never replace: game archives/depots ship default
    /// copies of these (Bedrock's zip contains server.properties/permissions.json/allowlist.json,
    /// 7 Days to Die's depot contains serverconfig.xml), and extracting over the folder would
    /// silently reset the user's settings. They are restored byte-for-byte after the update.
    /// </summary>
    internal static readonly string[] PreservedOnUpdate =
    {
        "server.properties", "eula.txt", "ops.json", "whitelist.json", "allowlist.json", "permissions.json",
        "banned-players.json", "banned-ips.json", "user_jvm_args.txt", "bukkit.yml", "spigot.yml",
        "paper-global.yml", "purpur.yml", "velocity.toml", "config.yml",
        "server.cfg", "ServerConfig.toml", "serverconfig.xml", "serveradmin.xml",
    };

    /// <summary>
    /// Updates an installed server in place to <paramref name="version"/> (the provider's newest
    /// when null): runs the same provider install into the server's own folder, keeps worlds,
    /// mods and every file in <see cref="PreservedOnUpdate"/>, then records the new version /
    /// server file / start command. The caller must stop the server first.
    /// </summary>
    public async Task<ServerInstance> UpdateAsync(ServerInstance server, GameDefinition game, string? version, IProgress<InstallProgress> progress, CancellationToken ct = default)
    {
        if (game.Install.Provider == "manual")
        {
            throw new InvalidOperationException($"{game.Name} servers are installed manually, so they can't be updated from here.");
        }

        var root = _servers.RootFor(server);
        if (!Directory.Exists(root))
        {
            throw new InvalidOperationException($"The server folder '{root}' no longer exists.");
        }

        var backup = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in PreservedOnUpdate)
        {
            var path = Path.Combine(root, name);
            try
            {
                if (File.Exists(path) && new FileInfo(path).Length <= 8 * 1024 * 1024)
                {
                    backup[name] = await File.ReadAllBytesAsync(path, ct).ConfigureAwait(false);
                }
            }
            catch (IOException)
            {
                // Unreadable right now: leave it to the provider (it most likely won't touch it).
            }
        }

        var request = new InstallRequest
        {
            Game = game,
            ServerName = server.ServerName,
            Variant = string.IsNullOrWhiteSpace(server.ServerType) ? null : server.ServerType,
            Version = version,
            TargetDir = root,
            Port = server.ServerPort,
            RamMinMb = server.RamMinMb,
            RamMaxMb = server.RamMaxMb,
            Description = server.ServerDescription,
            EulaAccepted = true,
        };

        InstallResult result;
        try
        {
            result = await ProviderFor(game).InstallAsync(request, progress, ct).ConfigureAwait(false);
            if (game.Install.WorkshopIds.Count > 0)
            {
                _workshop.DeployToServer(game, root);
            }
        }
        finally
        {
            // Restore even when the update failed half-way, so a failed update never costs the
            // user their settings.
            foreach (var (name, bytes) in backup)
            {
                try
                {
                    await File.WriteAllBytesAsync(Path.Combine(root, name), bytes, CancellationToken.None).ConfigureAwait(false);
                }
                catch
                {
                    // Best-effort.
                }
            }
        }

        // A version-stamped jar (paper-26.2-120.jar) replaced by a new one is just clutter now.
        var oldFile = server.ServerFile;
        if (!string.IsNullOrWhiteSpace(oldFile)
            && oldFile.EndsWith(".jar", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(oldFile, result.ServerFile, StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var oldPath = Path.GetFullPath(Path.Combine(root, oldFile));
                if (oldPath.StartsWith(Path.GetFullPath(root) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(oldPath);
                }
            }
            catch
            {
                // Best-effort.
            }
        }

        server.ServerVersion = result.Version ?? server.ServerVersion;
        server.ServerFile = result.ServerFile;
        if (result.JavaMajor > 0)
        {
            server.JavaMajor = result.JavaMajor;
        }

        // Provider-built commands (e.g. Forge's versioned win_args.txt path) must follow the new
        // version; otherwise the user's own start command is kept.
        if (!string.IsNullOrWhiteSpace(result.StartCommand))
        {
            server.StartCommand = result.StartCommand;
        }

        _servers.Save(server);
        return server;
    }

    /// <summary>
    /// Removes a half-created server folder after a failed/cancelled install. Retries briefly
    /// (a just-killed installer process may still hold handles for a moment); best-effort.
    /// </summary>
    private static void TryDeleteDirectory(string dir)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    return;
                }

                foreach (var file in Directory.EnumerateFiles(dir, "*", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true, AttributesToSkip = FileAttributes.ReparsePoint }))
                {
                    try
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                    }
                    catch
                    {
                        // Best-effort.
                    }
                }

                Directory.Delete(dir, recursive: true);
                return;
            }
            catch
            {
                Thread.Sleep(200 * (attempt + 1));
            }
        }
    }

    /// <summary>
    /// Port of <c>BuildInstance(GameDefinition, InstallRequest, InstallResult, string) -&gt;
    /// ServerInstance</c>.
    /// </summary>
    internal static ServerInstance BuildInstance(GameDefinition game, InstallRequest request, InstallResult result)
    {
        // FIX: this used to default to $"\"{result.ServerFile}\" {game.Runtime.Arguments}" when a
        // provider didn't supply an explicit start command — which, for any game whose Runtime
        // .Arguments template already references {server_file} itself (e.g. Minecraft Java's
        // `-jar "{server_file}"`), produced a garbled double-reference like
        // `"server.jar" -Xms{ram_min_mb}M -Xmx{ram_max_mb}M -jar "{server_file}" nogui`. Leaving
        // this empty instead defers correctly to ServerProcessService.BuildCommandAsync's own
        // existing fallback (substitute game.Runtime.Arguments directly when StartCommand is
        // empty), which is the single source of truth for placeholder substitution.
        var startCommand = result.StartCommand ?? game.Runtime.Arguments;

        var description = !string.IsNullOrWhiteSpace(request.Description)
            ? request.Description!
            : game.Name;

        return new ServerInstance
        {
            ManifestVersion = 1,
            ServerGame = game.Id,
            ServerGenres = game.Genres,
            ServerVersion = result.Version ?? string.Empty,
            ServerType = request.Variant ?? string.Empty,
            ServerName = request.ServerName,
            ServerDescription = description,
            ServerRoot = request.TargetDir,
            ServerFile = result.ServerFile,
            ServerIp = "0.0.0.0",
            ServerPort = request.Port,
            RamMinMb = request.RamMinMb,
            RamMaxMb = request.RamMaxMb,
            JavaMajor = result.JavaMajor,
            StartCommand = startCommand,
            StopCommand = game.Runtime.StopCommand ?? string.Empty,
            ModsFolder = ModsFolderFor(game, request.Variant),
            AutoRestartOnCrash = false,
            CreateFirewallRule = false,
            Tunnel = new TunnelSpec(),
            Stats = new ServerStats(),
            SourceFile = null,
        };
    }

    /// <summary>
    /// Port of <c>ModsFolderFor(GameDefinition, string? variant) -&gt; string?</c>.
    /// <c>minecraft-java</c>'s proxy/plugin variants (<c>paper</c>, <c>purpur</c>,
    /// <c>velocity</c>, <c>bungeecord</c>) use a <c>plugins</c> folder instead of the manifest's
    /// default <c>mods</c> folder; <c>fabric</c>/<c>forge</c> (and an unset variant, i.e.
    /// vanilla) keep <c>mods</c>.
    /// </summary>
    public static string? ModsFolderFor(GameDefinition game, string? variant)
    {
        if (!game.Mods.Supported)
        {
            return null;
        }

        if (game.Id == "minecraft-java")
        {
            return variant is "paper" or "purpur" or "velocity" or "bungeecord" ? "plugins" : "mods";
        }

        return game.Mods.Folder;
    }
}
