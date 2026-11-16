namespace MPServerProvider.Core.Models;

/// <summary>
/// A persisted, installed server instance (<c>server.json</c> in a server's folder).
/// Ported from <c>MPServerProvider.Core.Models.ServerInstance</c> (decompile token 0x02000037).
/// This type extends plain <c>System.Object</c> in the decompile (no
/// <c>CommunityToolkit.Mvvm.ObservableObject</c> base, no observable-property evidence anywhere
/// in <c>MPServerProvider.Core.Models</c>), so it is a plain DTO rather than a bindable
/// view-model-adjacent type — any live-updating binding happens in a ViewModel that wraps this,
/// not here.
/// </summary>
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

    /// <summary>
    /// Not present in the on-disk <c>server.json</c>; set by the loader to the path the
    /// instance was read from.
    /// </summary>
    public string? SourceFile { get; set; }

    /// <summary>
    /// Ported from the decompiled <c>ServerInstance.ResolveRoot(string)</c> helper: resolves a
    /// (possibly relative/templated) path against this server's root directory.
    /// </summary>
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
