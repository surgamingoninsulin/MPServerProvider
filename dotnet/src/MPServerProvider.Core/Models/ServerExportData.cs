namespace MPServerProvider.Core.Models;

/// <summary>
/// Portable export of a server's configuration (sans runtime state like stats/tunnel) for
/// sharing or backup.
/// Ported from <c>MPServerProvider.Core.Models.ServerExportData</c> (decompile token 0x02000033).
/// No standalone export sample file was found under <c>data/</c>, so field names follow the same
/// <c>snake_case</c> convention confirmed by real <c>server.json</c> shape, whose fields this
/// type is a subset of.
/// <para>
/// HARDENING: an export never contains absolute paths (<c>server_root</c>, <c>source_file</c>,
/// a rooted <c>server_file</c>/<c>mods_folder</c>), so sharing one leaks nothing about the
/// exporting machine and importing one can't point at someone else's folders. Because the field
/// names are a subset of <c>server.json</c>'s, <see cref="Json.Json"/> reads both an export and an
/// old-style full <c>server.json</c> into this type (unknown fields are ignored) — import accepts
/// either shape.
/// </para>
/// </summary>
public class ServerExportData
{
    public int ManifestVersion { get; set; } = 1;

    public string ServerName { get; set; } = string.Empty;

    public string ServerDescription { get; set; } = string.Empty;

    public string ServerGame { get; set; } = string.Empty;

    public string ServerVersion { get; set; } = string.Empty;

    public string ServerType { get; set; } = string.Empty;

    /// <summary>The server's main file relative to its folder (e.g. <c>server.jar</c>); never rooted.</summary>
    public string? ServerFile { get; set; }

    public int ServerPort { get; set; }

    public int RamMinMb { get; set; }

    public int RamMaxMb { get; set; }

    public int JavaMajor { get; set; }

    public string StartCommand { get; set; } = string.Empty;

    public string? StopCommand { get; set; }

    public string? ModsFolder { get; set; }

    public bool AutoRestartOnCrash { get; set; }

    public bool CreateFirewallRule { get; set; }

    /// <summary>Builds the portable export of <paramref name="server"/>.</summary>
    public static ServerExportData FromServer(ServerInstance server) => new()
    {
        ManifestVersion = server.ManifestVersion,
        ServerName = server.ServerName,
        ServerDescription = server.ServerDescription,
        ServerGame = server.ServerGame,
        ServerVersion = server.ServerVersion,
        ServerType = server.ServerType,
        ServerFile = PortableRelative(server.ServerFile, server.ServerRoot),
        ServerPort = server.ServerPort,
        RamMinMb = server.RamMinMb,
        RamMaxMb = server.RamMaxMb,
        JavaMajor = server.JavaMajor,
        StartCommand = server.StartCommand,
        StopCommand = server.StopCommand,
        ModsFolder = PortableRelative(server.ModsFolder, server.ServerRoot),
        AutoRestartOnCrash = server.AutoRestartOnCrash,
        CreateFirewallRule = server.CreateFirewallRule,
    };

    /// <summary>
    /// A new, unsaved <see cref="ServerInstance"/> carrying this export's settings. Paths
    /// (<c>ServerRoot</c>, <c>SourceFile</c>) are left empty for the importer to assign, stats
    /// start fresh, and a rooted or <c>..</c>-escaping <see cref="ServerFile"/> is dropped.
    /// </summary>
    public ServerInstance ToServerInstance() => new()
    {
        ManifestVersion = ManifestVersion,
        ServerName = ServerName,
        ServerDescription = ServerDescription,
        ServerGame = ServerGame,
        ServerVersion = ServerVersion,
        ServerType = ServerType,
        ServerFile = IsSafeRelative(ServerFile) ? ServerFile ?? string.Empty : string.Empty,
        ServerPort = ServerPort,
        RamMinMb = RamMinMb,
        RamMaxMb = RamMaxMb,
        JavaMajor = JavaMajor,
        StartCommand = StartCommand ?? string.Empty,
        StopCommand = StopCommand ?? string.Empty,
        ModsFolder = IsSafeRelative(ModsFolder) ? ModsFolder : null,
        AutoRestartOnCrash = AutoRestartOnCrash,
        CreateFirewallRule = CreateFirewallRule,
    };

    /// <summary>A plain relative path with no drive, UNC prefix or <c>..</c> segments.</summary>
    public static bool IsSafeRelative(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return true;
        }

        if (Path.IsPathRooted(path) || path.Contains(':') || path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            return false;
        }

        return path.Split('/', '\\').All(s => s.Length > 0 && s != "." && s != "..");
    }

    /// <summary><paramref name="path"/> made relative to <paramref name="root"/> when it lies
    /// inside it; any other absolute path is dropped (<c>null</c>).</summary>
    private static string? PortableRelative(string? path, string root)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return path;
        }

        if (!Path.IsPathRooted(path))
        {
            return IsSafeRelative(path) ? path : null;
        }

        if (string.IsNullOrWhiteSpace(root))
        {
            return null;
        }

        try
        {
            var relative = Path.GetRelativePath(Path.GetFullPath(root), Path.GetFullPath(path));
            return IsSafeRelative(relative) && !Path.IsPathRooted(relative) ? relative : null;
        }
        catch
        {
            return null;
        }
    }
}
