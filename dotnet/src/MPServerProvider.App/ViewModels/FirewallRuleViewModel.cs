using MPServerProvider.Core.Models;

namespace MPServerProvider.App.ViewModels;

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
