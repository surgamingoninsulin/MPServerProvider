namespace MPServerProvider.Core.Models;

public class FirewallRuleInfo
{
    public string Name { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Protocol { get; set; } = "both";

    public bool Active { get; set; }

    public DateTime Created { get; set; }
}
