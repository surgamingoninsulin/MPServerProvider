namespace MPServerProvider.Core.Models;

public class ServerExportData
{
    public int ManifestVersion { get; set; } = 1;

    public string ServerName { get; set; } = string.Empty;

    public string ServerDescription { get; set; } = string.Empty;

    public string ServerGame { get; set; } = string.Empty;

    public string ServerVersion { get; set; } = string.Empty;

    public string ServerType { get; set; } = string.Empty;

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
