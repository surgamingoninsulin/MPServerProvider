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
}
