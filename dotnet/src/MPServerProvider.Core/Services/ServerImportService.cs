using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Providers;

namespace MPServerProvider.Core.Services;

/// <summary>What <see cref="ServerImportService.Detect"/> found in a folder.</summary>
public sealed class ServerImportDetection
{
    /// <summary>Set when the folder can't be imported for this game; everything else is then unset.</summary>
    public string? Problem { get; init; }

    public bool IsValid => Problem is null;

    /// <summary>Server file relative to the folder (jar, exe, or Forge's <c>win_args.txt</c>).</summary>
    public string ServerFile { get; init; } = string.Empty;

    /// <summary>Minecraft variant (<c>paper</c>, <c>purpur</c>, <c>fabric</c>, …; empty = vanilla).</summary>
    public string ServerType { get; init; } = string.Empty;

    /// <summary>Version string in the same shape the installer records (e.g. <c>1.21.11 (build 2568)</c>).</summary>
    public string Version { get; init; } = string.Empty;

    /// <summary>Port from the server's own config, when it has one.</summary>
    public int? Port { get; init; }

    public string? ModsFolder { get; init; }

    public int JavaMajor { get; init; }

    /// <summary>Launch arguments when the default template doesn't fit (Forge/NeoForge argument files).</summary>
    public string? StartCommand { get; init; }

    public int RamMinMb { get; init; }

    public int RamMaxMb { get; init; }

    /// <summary>Things worth telling the user (EULA not accepted yet, version unknown, …).</summary>
    public List<string> Notes { get; init; } = new();

    /// <summary>One line for the dialog, e.g. <c>Purpur · Minecraft 1.21.11 (build 2568) · purpur.jar</c>.</summary>
    public string Summary { get; init; } = string.Empty;

    public static ServerImportDetection Fail(string problem) => new() { Problem = problem };
}

/// <summary>
/// Imports an existing server folder (made by hand, another panel, or an older install) as a
/// server of a given game: detects the server file, variant, version and port from the folder,
/// then either copies it into the app's servers folder or registers it where it is. The source
/// folder is never modified.
/// </summary>
public sealed partial class ServerImportService
{
    private readonly ServerRepository _servers;

    public ServerImportService(ServerRepository servers)
    {
        _servers = servers;
    }

    // ---- detection -----------------------------------------------------------------------

    /// <summary>Looks at <paramref name="folder"/> as a server for <paramref name="game"/>.</summary>
    public static ServerImportDetection Detect(GameDefinition game, string folder)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                return ServerImportDetection.Fail("That folder doesn't exist.");
            }

            return ModCompatibility.IsMinecraftJava(game.Id) ? DetectMinecraftJava(folder) : DetectGeneric(game, folder);
        }
        catch (UnauthorizedAccessException)
        {
            return ServerImportDetection.Fail("Windows denied access to that folder.");
        }
        catch (IOException ex)
        {
            return ServerImportDetection.Fail($"The folder couldn't be read: {ex.Message}");
        }
    }

    private static readonly string[] PluginVariants = { "paper", "purpur", "folia", "pufferfish", "spigot", "velocity", "bungeecord" };

    private static ServerImportDetection DetectMinecraftJava(string folder)
    {
        var notes = new List<string>();
        string? type = null;
        string? mcVersion = null;
        string? build = null;
        string? fullVersion = null;
        string? serverFile = null;
        string? startCommand = null;

        // Forge / NeoForge 1.17+ launch through argument files under libraries/.
        foreach (var (variant, rel) in new[]
                 {
                     ("neoforge", Path.Combine("libraries", "net", "neoforged", "neoforge")),
                     ("forge", Path.Combine("libraries", "net", "minecraftforge", "forge")),
                 })
        {
            var dir = Path.Combine(folder, rel);
            if (!Directory.Exists(dir))
            {
                continue;
            }

            var newest = Directory.EnumerateDirectories(dir)
                .Select(Path.GetFileName)
                .Where(n => n is not null && File.Exists(Path.Combine(dir, n, "win_args.txt")))
                .Select(n => n!)
                .OrderByDescending(n => n, Comparer<string>.Create(MinecraftJavaProvider.CompareVersions))
                .FirstOrDefault();
            if (newest is null)
            {
                continue;
            }

            type = variant;
            var argsRel = Path.Combine(rel, newest, "win_args.txt");
            serverFile = argsRel;
            var jvmArgs = File.Exists(Path.Combine(folder, "user_jvm_args.txt")) ? "@user_jvm_args.txt " : string.Empty;
            startCommand = $"-Xms{{ram_min_mb}}M -Xmx{{ram_max_mb}}M {jvmArgs}@{argsRel.Replace('\\', '/')} nogui";
            if (variant == "forge")
            {
                fullVersion = newest; // 1.20.1-47.2.0
                mcVersion = newest.Split('-')[0];
            }
            else
            {
                mcVersion = NeoForgeMinecraftVersion(newest);
                fullVersion = mcVersion is null ? newest : $"{mcVersion} (NeoForge {newest})";
            }

            break;
        }

        if (serverFile is null)
        {
            var jars = Directory.EnumerateFiles(folder, "*.jar")
                .Select(p => (Path: p, Info: InspectJar(p)))
                .Where(j => !j.Info.IsInstaller && j.Info.HasMainClass)
                .ToList();
            if (jars.Count == 0)
            {
                return ServerImportDetection.Fail(
                    "No Minecraft server jar was found in this folder. Pick the folder that contains the server's .jar (and server.properties).");
            }

            var best = jars.OrderByDescending(j => j.Info.Kind is not null).ThenByDescending(j => new FileInfo(j.Path).Length).First();
            serverFile = Path.GetFileName(best.Path);
            type = best.Info.Kind ?? string.Empty;
            mcVersion = best.Info.MinecraftVersion;
            build = best.Info.Build;
            if (type == "velocity" || type == "bungeecord")
            {
                fullVersion = best.Info.ProxyVersion;
            }
        }

        // Paper/Purpur write the exact version after the first start.
        var history = Path.Combine(folder, "version_history.json");
        if (File.Exists(history) && type is "paper" or "purpur" or "folia" or "pufferfish" or "")
        {
            try
            {
                using var doc = JsonDocument.Parse(File.ReadAllText(history));
                if (doc.RootElement.TryGetProperty("currentVersion", out var current) && current.GetString() is { } text)
                {
                    var m = VersionHistoryRegex().Match(text);
                    if (m.Success)
                    {
                        mcVersion = m.Groups["mc"].Value;
                        build ??= m.Groups["build"].Success ? m.Groups["build"].Value : null;
                    }
                }
            }
            catch (Exception ex) when (ex is IOException or JsonException)
            {
                // Optional source.
            }
        }

        if (mcVersion is null && Directory.Exists(Path.Combine(folder, "versions")))
        {
            mcVersion = Directory.EnumerateDirectories(Path.Combine(folder, "versions"))
                .Select(Path.GetFileName)
                .Where(n => n is not null && MinecraftVersionRange.ServerMinecraftVersion(n) is not null)
                .Select(n => n!)
                .OrderByDescending(n => n, Comparer<string>.Create(MinecraftJavaProvider.CompareVersions))
                .FirstOrDefault();
        }

        type ??= string.Empty;
        var version = fullVersion
            ?? (mcVersion is null ? string.Empty : build is null ? mcVersion : $"{mcVersion} (build {build})");
        if (version.Length == 0)
        {
            notes.Add("The Minecraft version couldn't be detected; updates and mod version checks won't know it.");
        }

        var props = ReadProperties(Path.Combine(folder, "server.properties"));
        int? port = props?.Get("server-port") is { } p && int.TryParse(p, out var parsed) && parsed is > 0 and <= 65535 ? parsed : null;
        if (type == "velocity")
        {
            port = VelocityPort(folder) ?? port;
        }

        if (type is not ("velocity" or "bungeecord"))
        {
            var eula = Path.Combine(folder, "eula.txt");
            if (!File.Exists(eula) || !File.ReadAllText(eula).Contains("eula=true", StringComparison.OrdinalIgnoreCase))
            {
                notes.Add("The Minecraft EULA isn't accepted in this folder yet (eula.txt).");
            }
        }

        var javaMajor = type switch
        {
            "velocity" => fullVersion?.StartsWith("3.", StringComparison.Ordinal) == true ? 17 : 21,
            "bungeecord" => 0,
            _ => mcVersion is null ? 21 : MinecraftJavaProvider.GuessJavaMajor(mcVersion),
        };

        var modsFolder = PluginVariants.Contains(type) ? "plugins" : "mods";
        var label = type.Length == 0 ? "Vanilla" : ModCompatibility.VariantLabel(new ServerInstance { ServerType = type });
        var summary = string.Join(" · ", new[]
        {
            label,
            version.Length == 0 ? null : type is "velocity" or "bungeecord" ? $"version {version}" : $"Minecraft {version}",
            Path.GetFileName(serverFile),
        }.Where(s => !string.IsNullOrEmpty(s)));

        return new ServerImportDetection
        {
            ServerFile = serverFile,
            ServerType = type,
            Version = version,
            Port = port,
            ModsFolder = modsFolder,
            JavaMajor = javaMajor,
            StartCommand = startCommand,
            RamMinMb = 1024,
            RamMaxMb = 4096,
            Notes = notes,
            Summary = summary,
        };
    }

    /// <summary>NeoForge <c>21.1.77</c> → Minecraft <c>1.21.1</c>; <c>20.4.237</c> → <c>1.20.4</c>;
    /// <c>21.0.x</c> → <c>1.21</c>; year-numbered releases (<c>26.1.0.5</c>) → <c>26.1</c>.</summary>
    internal static string? NeoForgeMinecraftVersion(string neoVersion)
    {
        var parts = neoVersion.Split('.', '-');
        if (parts.Length < 2 || !int.TryParse(parts[0], out var major) || !int.TryParse(parts[1], out var minor))
        {
            return null;
        }

        if (major >= 25)
        {
            return $"{major}.{minor}";
        }

        return minor == 0 ? $"1.{major}" : $"1.{major}.{minor}";
    }

    private static int? VelocityPort(string folder)
    {
        var toml = Path.Combine(folder, "velocity.toml");
        if (!File.Exists(toml))
        {
            return null;
        }

        var m = VelocityBindRegex().Match(File.ReadAllText(toml));
        return m.Success && int.TryParse(m.Groups[1].Value, out var port) ? port : null;
    }

    /// <summary>What a jar's contents say about it.</summary>
    internal sealed record JarFacts(bool HasMainClass, bool IsInstaller, string? Kind, string? MinecraftVersion, string? Build, string? ProxyVersion);

    /// <summary>Reads a jar's manifest and launcher metadata: Paperclip's <c>versions.list</c>
    /// (Paper/Purpur/Folia + Minecraft version), Fabric's <c>install.properties</c>, Mojang's
    /// <c>version.json</c>, and the main classes of Velocity/BungeeCord/Spigot/Forge.</summary>
    internal static JarFacts InspectJar(string path)
    {
        var name = Path.GetFileName(path).ToLowerInvariant();
        try
        {
            using var zip = ZipFile.OpenRead(path);
            var manifest = ReadEntry(zip, "META-INF/MANIFEST.MF") ?? string.Empty;
            var mainClass = ManifestValue(manifest, "Main-Class");
            var implVersion = ManifestValue(manifest, "Implementation-Version");
            var isInstaller = name.Contains("installer")
                || (mainClass?.Contains("installer", StringComparison.OrdinalIgnoreCase) ?? false);

            string? kind = null;
            string? mc = null;
            string? build = null;
            string? proxyVersion = null;

            if (ReadEntry(zip, "META-INF/versions.list") is { } versionsList)
            {
                // "<sha256>\t<mc version>\t<mc>/<purpur|paper|folia>-<mc>.jar"
                var line = versionsList.Split('\n').Select(l => l.Trim()).FirstOrDefault(l => l.Length > 0);
                var cols = line?.Split('\t') ?? Array.Empty<string>();
                if (cols.Length >= 3)
                {
                    mc = cols[1];
                    var file = Path.GetFileName(cols[2]).ToLowerInvariant();
                    kind = new[] { "purpur", "folia", "pufferfish", "paper" }.FirstOrDefault(k => file.StartsWith(k, StringComparison.Ordinal)) ?? "paper";
                }

                var buildMatch = BuildNumberRegex().Match(name);
                build = buildMatch.Success ? buildMatch.Groups[1].Value : null;
            }
            else if (ReadEntry(zip, "install.properties") is { } fabric && fabric.Contains("fabric-loader-version", StringComparison.Ordinal))
            {
                kind = "fabric";
                mc = PropertyValue(fabric, "game-version");
            }
            else if (zip.GetEntry("com/velocitypowered/proxy/Velocity.class") is not null)
            {
                kind = "velocity";
                proxyVersion = implVersion;
            }
            else if (zip.GetEntry("net/md_5/bungee/BungeeCord.class") is not null)
            {
                kind = "bungeecord";
                proxyVersion = implVersion;
            }
            else if (zip.GetEntry("org/bukkit/craftbukkit/Main.class") is not null || name.StartsWith("spigot", StringComparison.Ordinal))
            {
                kind = "spigot";
            }
            else if (name.StartsWith("forge-", StringComparison.Ordinal))
            {
                kind = "forge";
                var m = Regex.Match(name, @"^forge-(\d+(?:\.\d+)+)-");
                mc = m.Success ? m.Groups[1].Value : null;
            }
            else if (zip.GetEntry("net/fabricmc/loader/launch/server/FabricServerLauncher.class") is not null
                     || name.StartsWith("fabric", StringComparison.Ordinal))
            {
                kind = "fabric";
            }

            if (ReadEntry(zip, "version.json") is { } versionJson)
            {
                try
                {
                    using var doc = JsonDocument.Parse(versionJson);
                    if (doc.RootElement.TryGetProperty("id", out var id) && id.GetString() is { } idText)
                    {
                        mc ??= idText;
                        kind ??= string.Empty; // vanilla
                    }
                }
                catch (JsonException)
                {
                    // Not Mojang's version.json.
                }
            }

            if (kind is null && (name is "server.jar" || name.StartsWith("minecraft_server", StringComparison.Ordinal)))
            {
                kind = string.Empty;
                var m = Regex.Match(name, @"minecraft_server\.(\d+(?:\.\d+)+)\.jar");
                mc ??= m.Success ? m.Groups[1].Value : null;
            }

            return new JarFacts(mainClass is not null, isInstaller, kind, mc, build, proxyVersion);
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            return new JarFacts(false, false, null, null, null, null);
        }
    }

    private static string? ReadEntry(ZipArchive zip, string name)
    {
        var entry = zip.GetEntry(name);
        if (entry is null || entry.Length > 1024 * 1024)
        {
            return null;
        }

        using var reader = new StreamReader(entry.Open());
        return reader.ReadToEnd();
    }

    private static string? ManifestValue(string manifest, string key)
    {
        foreach (var raw in manifest.Split('\n'))
        {
            var line = raw.TrimEnd('\r');
            if (line.StartsWith(key + ":", StringComparison.OrdinalIgnoreCase))
            {
                var value = line[(key.Length + 1)..].Trim();
                return value.Length == 0 ? null : value;
            }
        }

        return null;
    }

    private static string? PropertyValue(string properties, string key)
    {
        foreach (var raw in properties.Split('\n'))
        {
            var line = raw.Trim();
            if (line.StartsWith(key + "=", StringComparison.Ordinal))
            {
                return line[(key.Length + 1)..].Trim();
            }
        }

        return null;
    }

    private static ServerImportDetection DetectGeneric(GameDefinition game, string folder)
    {
        var serverFile = FindServerExecutable(game, folder);
        if (serverFile is null)
        {
            var expected = ExpectedExecutable(game);
            return ServerImportDetection.Fail(expected is null
                ? $"This folder doesn't look like a {game.Name} server: no server program (.exe) was found in it."
                : $"This folder doesn't look like a {game.Name} server: {expected} wasn't found in it.");
        }

        int? port = null;
        var props = ReadProperties(Path.Combine(folder, "server.properties"));
        if (props?.Get("server-port") is { } p && int.TryParse(p, out var parsed) && parsed is > 0 and <= 65535)
        {
            port = parsed;
        }

        return new ServerImportDetection
        {
            ServerFile = serverFile,
            Version = "imported",
            Port = port,
            ModsFolder = InstallService.ModsFolderFor(game, null),
            Summary = $"{game.Name} · {serverFile}",
        };
    }

    /// <summary>The executable the game's installer would produce, when it's a fixed name.</summary>
    private static string? ExpectedExecutable(GameDefinition game)
    {
        var fromDefinition = DefinitionExecutable(game);
        if (fromDefinition is not null)
        {
            return fromDefinition;
        }

        return game.Install.Provider switch
        {
            "bedrock" => "bedrock_server.exe",
            "cfx" => "FXServer.exe",
            "beammp" => "BeamMP-Server.exe",
            _ => null,
        };
    }

    /// <summary>The definition's own executable (relative), when it names one instead of <c>{server_file}</c>.</summary>
    private static string? DefinitionExecutable(GameDefinition game)
    {
        var executable = game.Runtime.Executable;
        if (string.IsNullOrWhiteSpace(executable) || SteamCmdProvider.DefinitionNamesExecutable(executable))
        {
            return null;
        }

        var relative = executable.Replace("{server_root}\\", string.Empty).Replace("{server_root}/", string.Empty)
            .Replace("{server_root}", string.Empty).Trim().Trim('"');
        return relative.Length == 0 || relative.Contains('{') || Path.IsPathRooted(relative) ? null : relative;
    }

    /// <summary>Server executable relative to <paramref name="folder"/>, or <c>null</c>.</summary>
    internal static string? FindServerExecutable(GameDefinition game, string folder)
    {
        if (DefinitionExecutable(game) is { } named)
        {
            return File.Exists(Path.Combine(folder, named)) ? named : null;
        }

        if (ExpectedExecutable(game) is { } fixedName)
        {
            return FindFile(folder, fixedName, maxDepth: 3);
        }

        if (game.Install.Provider == "manual" && ManualProvider.FindKnownServerExe(folder) is { } known)
        {
            return known;
        }

        return SteamCmdProvider.AutoDetectServerExe(folder) is { } exe ? Relative(folder, exe) : null;
    }

    private static JavaPropertiesFile? ReadProperties(string path)
    {
        try
        {
            return File.Exists(path) ? JavaPropertiesFile.Load(path) : null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static string Relative(string folder, string path)
        => Path.IsPathRooted(path) ? Path.GetRelativePath(folder, path) : path;

    private static string? FindFile(string folder, string fileName, int maxDepth)
    {
        var queue = new Queue<(string Dir, int Depth)>();
        queue.Enqueue((folder, 0));
        while (queue.Count > 0)
        {
            var (dir, depth) = queue.Dequeue();
            var candidate = Path.Combine(dir, fileName);
            if (File.Exists(candidate))
            {
                return Path.GetRelativePath(folder, candidate);
            }

            if (depth >= maxDepth)
            {
                continue;
            }

            try
            {
                foreach (var sub in new DirectoryInfo(dir).EnumerateDirectories())
                {
                    if (!sub.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    {
                        queue.Enqueue((sub.FullName, depth + 1));
                    }
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Skip unreadable folders.
            }
        }

        return null;
    }

    // ---- import --------------------------------------------------------------------------

    /// <summary>
    /// Why <paramref name="folder"/> can't be imported (it is, contains, or sits inside an
    /// existing server's folder, or contains the app's servers folder), or <c>null</c>.
    /// </summary>
    public string? CheckFolder(string folder)
    {
        var full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder));
        foreach (var server in _servers.Servers)
        {
            var root = _servers.RootFor(server);
            if (string.IsNullOrWhiteSpace(root))
            {
                continue;
            }

            if (AppPaths.SamePath(root, full) || AppPaths.IsStrictlyInside(full, root) || AppPaths.IsStrictlyInside(root, full))
            {
                return $"That folder is already used by the server '{server.ServerName}'.";
            }
        }

        var target = _servers.NewServerDir("x");
        var serversDir = Path.GetDirectoryName(target)!;
        if (AppPaths.SamePath(serversDir, full) || AppPaths.IsStrictlyInside(serversDir, full))
        {
            return "That folder contains the app's own servers folder. Pick the server's folder itself.";
        }

        return null;
    }

    /// <summary>
    /// Imports <paramref name="folder"/> as server <paramref name="name"/>. With
    /// <paramref name="copy"/> the files are copied into a new folder in the servers folder
    /// (junctions/symlinks are not followed); otherwise the server keeps running from
    /// <paramref name="folder"/> and only its registration lives in the servers folder.
    /// </summary>
    public async Task<ServerInstance> ImportAsync(
        GameDefinition game,
        string folder,
        string name,
        int port,
        bool copy,
        IProgress<InstallProgress> progress,
        CancellationToken ct = default)
    {
        name = name.Trim();
        if (ServerRepository.ValidateServerName(name) is { } nameError)
        {
            throw new InvalidOperationException(nameError);
        }

        if (_servers.NameTaken(name))
        {
            throw new InvalidOperationException($"A server named '{name}' already exists.");
        }

        if (port is <= 0 or > 65535)
        {
            throw new InvalidOperationException("Enter a port between 1 and 65535.");
        }

        folder = Path.TrimEndingDirectorySeparator(Path.GetFullPath(folder));
        if (CheckFolder(folder) is { } folderError)
        {
            throw new InvalidOperationException(folderError);
        }

        var detection = Detect(game, folder);
        if (detection.Problem is { } problem)
        {
            throw new InvalidOperationException(problem);
        }

        var root = folder;
        string? created = null;
        if (copy)
        {
            if (File.Exists(Path.Combine(folder, "server.json")) && !IsOurManifest(Path.Combine(folder, "server.json")))
            {
                throw new InvalidOperationException(
                    "This server has its own server.json, which would clash with the app's server file. Choose \"Use the folder where it is\" instead.");
            }

            root = _servers.NewServerDir(name);
            if (Directory.Exists(root))
            {
                throw new InvalidOperationException($"A server folder for '{name}' already exists.");
            }

            created = root;
            try
            {
                await Task.Run(() => CopyTree(folder, root, progress, ct), ct).ConfigureAwait(false);
            }
            catch
            {
                TryDelete(created);
                throw;
            }
        }

        try
        {
            progress.Report(InstallProgress.Indeterminate("Registering the server…"));
            if (ModCompatibility.IsMinecraftJava(game.Id))
            {
                SyncMinecraftPort(root, port);
            }

            var instance = new ServerInstance
            {
                ManifestVersion = 1,
                ServerGame = game.Id,
                ServerGenres = game.Genres,
                ServerVersion = detection.Version,
                ServerType = detection.ServerType,
                ServerName = name,
                ServerDescription = $"Imported {game.Name} server",
                ServerRoot = root,
                ServerFile = detection.ServerFile,
                ServerIp = "0.0.0.0",
                ServerPort = port,
                RamMinMb = detection.RamMinMb,
                RamMaxMb = detection.RamMaxMb,
                JavaMajor = detection.JavaMajor,
                StartCommand = detection.StartCommand ?? game.Runtime.Arguments,
                StopCommand = game.Runtime.StopCommand ?? string.Empty,
                ModsFolder = game.Mods.Supported ? detection.ModsFolder : null,
                AutoRestartOnCrash = false,
                CreateFirewallRule = false,
                Tunnel = new TunnelSpec(),
                Stats = new ServerStats(),
            };

            // A copied folder that held one of this app's own server.json files: the
            // repository writes the fresh one.
            if (copy)
            {
                TryDeleteFile(Path.Combine(root, "server.json"));
            }

            _servers.Save(instance);
            return instance;
        }
        catch
        {
            if (created is not null)
            {
                TryDelete(created);
            }

            throw;
        }
    }

    /// <summary>Copied servers get the port the user chose in server.properties (in-place imports
    /// are left untouched; the app syncs the port when the server starts).</summary>
    private static void SyncMinecraftPort(string root, int port)
    {
        var path = Path.Combine(root, "server.properties");
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            var file = JavaPropertiesFile.Load(path);
            if (file.Get("server-port") != port.ToString())
            {
                file.Set("server-port", port.ToString());
                file.Save(path);
            }
        }
        catch (IOException)
        {
            // The app re-syncs server-port on start.
        }
    }

    private static bool IsOurManifest(string path)
    {
        try
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            return doc.RootElement.ValueKind == JsonValueKind.Object
                && doc.RootElement.TryGetProperty("server_game", out _)
                && doc.RootElement.TryGetProperty("server_name", out _);
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>Copies <paramref name="source"/> into <paramref name="dest"/>, reporting byte
    /// progress. Junctions/symlinks are skipped (never followed), so a link loop or a link to
    /// another drive can't blow up the copy.</summary>
    internal static void CopyTree(string source, string dest, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            AttributesToSkip = FileAttributes.ReparsePoint,
            IgnoreInaccessible = false,
        };

        progress.Report(InstallProgress.Indeterminate("Counting files…"));
        var files = new DirectoryInfo(source).EnumerateFiles("*", options).ToList();
        var total = Math.Max(1L, files.Sum(f => f.Length));
        long done = 0;

        Directory.CreateDirectory(dest);
        foreach (var dir in new DirectoryInfo(source).EnumerateDirectories("*", options))
        {
            Directory.CreateDirectory(Path.Combine(dest, Path.GetRelativePath(source, dir.FullName)));
        }

        var lastReport = Environment.TickCount64;
        foreach (var file in files)
        {
            ct.ThrowIfCancellationRequested();
            var target = Path.Combine(dest, Path.GetRelativePath(source, file.FullName));
            try
            {
                file.CopyTo(target, overwrite: false);
            }
            catch (IOException ex) when (ex.HResult == unchecked((int)0x80070020))
            {
                throw new IOException(
                    $"'{Path.GetRelativePath(source, file.FullName)}' is in use by another program. Stop that server first, then import again.", ex);
            }

            done += file.Length;
            if (Environment.TickCount64 - lastReport > 100)
            {
                lastReport = Environment.TickCount64;
                progress.Report(new InstallProgress
                {
                    Status = $"Copying server files… {FormatBytes(done)} of {FormatBytes(total)}",
                    Percent = done * 100.0 / total,
                    BytesReceived = done,
                    TotalBytes = total,
                });
            }
        }

        progress.Report(InstallProgress.At("Copied all server files.", 100));
    }

    private static string FormatBytes(long bytes) => bytes switch
    {
        >= 1L << 30 => $"{bytes / (double)(1L << 30):0.0} GB",
        >= 1L << 20 => $"{bytes / (double)(1L << 20):0.0} MB",
        >= 1L << 10 => $"{bytes / 1024.0:0} KB",
        _ => $"{bytes} B",
    };

    private static void TryDelete(string dir)
    {
        try
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, recursive: true);
            }
        }
        catch
        {
            // Best-effort cleanup of a half-copied folder.
        }
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Save() then refuses with a clear message.
        }
    }

    // "1.21.11-2568-f57bd86 (MC: 1.21.11)" (Paper/Purpur), "git-Paper-196 (MC: 1.20.1)" (older Paper).
    [GeneratedRegex(@"^(?:(?:\d+(?:\.\d+)+)-(?<build>\d+)-\w+|git-\w+-(?<build>\d+))?.*\(MC:\s*(?<mc>[^)\s]+)\)")]
    private static partial Regex VersionHistoryRegex();

    [GeneratedRegex(@"(?m)^\s*bind\s*=\s*""[^""]*:(\d+)""")]
    private static partial Regex VelocityBindRegex();

    // purpur-1.21.11-2568.jar, paper-1.21.1-133.jar
    [GeneratedRegex(@"-(\d+)\.jar$")]
    private static partial Regex BuildNumberRegex();
}
