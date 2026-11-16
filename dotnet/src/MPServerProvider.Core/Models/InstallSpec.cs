namespace MPServerProvider.Core.Models;

/// <summary>
/// Install provider configuration (SteamCMD app id, cfx product, direct download URL, etc).
/// Ported from <c>MPServerProvider.Core.Models.InstallSpec</c> (decompile token 0x0200002a).
/// </summary>
public class InstallSpec
{
    public string? Provider { get; set; }

    public List<string> Variants { get; set; } = new();

    public string? Url { get; set; }

    public string? Archive { get; set; }

    /// <summary>
    /// <c>url</c> provider only: <c>owner/repo</c> of a GitHub project whose LATEST release holds
    /// the download. Used instead of <see cref="Url"/> when release asset names carry the version
    /// (so no stable <c>releases/latest/download/&lt;name&gt;</c> URL exists); the asset is picked
    /// with <see cref="AssetPattern"/>.
    /// </summary>
    public string? GithubRepo { get; set; }

    /// <summary>
    /// Regex (case-insensitive) matched against the asset names of <see cref="GithubRepo"/>'s
    /// latest release, e.g. <c>^OpenRCT2-.*-windows-portable-x64\.zip$</c>.
    /// </summary>
    public string? AssetPattern { get; set; }

    public int? AppId { get; set; }

    public bool Anonymous { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Product { get; set; }

    public List<long> WorkshopIds { get; set; } = new();
}
