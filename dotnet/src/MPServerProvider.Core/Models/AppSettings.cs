namespace MPServerProvider.Core.Models;

public class AppSettings
{
    public string? InstallRoot { get; set; }

    public bool StartOnBoot { get; set; }

    public bool StartMinimizedToTray { get; set; }

    public bool RestoreRunningServers { get; set; }

    public List<string> LastRunningServers { get; set; } = new();

    public bool NewsAutoScroll { get; set; } = true;

    public List<FirewallRuleInfo> FirewallRules { get; set; } = new();

    public double AssumedHostingPricePerHour { get; set; } = 0.02;

    public string? SteamUsername { get; set; }

    public string? SteamId64 { get; set; }

    /// <summary>CurseForge Core API key (from https://console.curseforge.com/) used by the "Install Mods" tab.</summary>
    public string? CurseForgeApiKey { get; set; }

    /// <summary>mod.io read-only API key (from https://mod.io/me/access) used by the "Install Mods" tab.</summary>
    public string? ModIoApiKey { get; set; }

    /// <summary>
    /// mod.io now issues each account its own API host (shown as "API path" on the mod.io API
    /// access page, e.g. https://u-26716762.modapi.io/v1) instead of the old shared api.mod.io
    /// domain, which is heavily restricted since January 2025. Falls back to that old domain if
    /// not set, since it still answers requests, just unreliably.
    /// </summary>
    public string? ModIoApiHost { get; set; }
}
