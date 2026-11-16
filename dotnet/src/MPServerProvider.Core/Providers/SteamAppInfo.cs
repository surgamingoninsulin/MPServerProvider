namespace MPServerProvider.Core.Providers;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Providers.SteamAppInfo</c> (decompile token 0x02000024):
/// the projection of a parsed SteamCMD <c>app_info_print</c> document used to auto-detect a Steam
/// dedicated server's install layout (install dir, launch executable/args).
/// </summary>
public sealed class SteamAppInfo
{
    public string Name { get; set; } = string.Empty;

    public int? ParentAppId { get; set; }

    public string? InstallDir { get; set; }

    public string? Executable { get; set; }

    public string? WorkingDir { get; set; }

    public string? Arguments { get; set; }

    public bool FreeToDownload { get; set; }
}
