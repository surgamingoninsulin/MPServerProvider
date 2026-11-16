namespace MPServerProvider.Core.Models;

/// <summary>
/// A Windows Firewall rule the app created/tracks for a server's port.
/// Ported from <c>MPServerProvider.Core.Models.FirewallRuleInfo</c> (decompile token 0x02000030).
/// Field names confirmed against ground truth in <c>data/settings.json</c> (<c>firewall_rules[]</c>).
/// </summary>
public class FirewallRuleInfo
{
    public string Name { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Protocol { get; set; } = "both";

    public bool Active { get; set; }

    public DateTime Created { get; set; }
}
