namespace MPServerProvider.Core.Models;

public class ServerInstance
{
    public int ManifestVersion { get; set; } = 1;

    public string ServerGame { get; set; } = string.Empty;

    public List<string> ServerGenres { get; set; } = new();

    public string ServerVersion { get; set; } = string.Empty;

    public string ServerType { get; set; } = string.Empty;

    public string ServerName { get; set; } = string.Empty;

    public string ServerDescription { get; set; } = string.Empty;

    public string ServerRoot { get; set; } = string.Empty;

    public string ServerFile { get; set; } = string.Empty;

    public string ServerIp { get; set; } = "0.0.0.0";

    public int ServerPort { get; set; }

    public int RamMinMb { get; set; }

    public int RamMaxMb { get; set; }

    public int JavaMajor { get; set; }

    public string StartCommand { get; set; } = string.Empty;

    public string StopCommand { get; set; } = string.Empty;

    public string? ModsFolder { get; set; }

    public bool AutoRestartOnCrash { get; set; }

    public bool CreateFirewallRule { get; set; }

    public TunnelSpec Tunnel { get; set; } = new();

    public ServerStats Stats { get; set; } = new();

    public string? SourceFile { get; set; }

    public string ResolveRoot(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return ServerRoot;
        }

        if (Path.IsPathRooted(path))
        {
            return path;
        }

        var trimmed = path.TrimStart('\\', '/');
        return Path.Combine(ServerRoot, trimmed);
    }
}
