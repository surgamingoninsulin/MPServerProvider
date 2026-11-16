namespace MPServerProvider.Core.Providers;

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
