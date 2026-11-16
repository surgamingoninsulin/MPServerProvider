using MPServerProvider.Core.Models;

namespace MPServerProvider.App.ViewModels;

/// <summary>
/// Display row for one firewall rule on the Settings page, ported from the decompiled
/// <c>RuleVm.get_DetailText()</c> (<c>0x0200001a_RuleVm.md</c>).
/// </summary>
public sealed class FirewallRuleViewModel
{
    public required FirewallRuleInfo Rule { get; init; }

    public string Name => Rule.Name;

    public string DetailText
    {
        get
        {
            var proto = Rule.Protocol.Equals("both", StringComparison.OrdinalIgnoreCase)
                ? "TCP/UDP"
                : Rule.Protocol.ToUpperInvariant();
            return $"Port: {Rule.Port}   •   {proto}";
        }
    }
}
