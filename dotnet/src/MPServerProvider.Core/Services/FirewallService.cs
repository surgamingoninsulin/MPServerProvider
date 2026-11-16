using System.Diagnostics;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public sealed class FirewallService
{
    public const string RulePrefix = "MPSM - ";

    public static IReadOnlyList<string> ProtocolsFor(string protocol)
    {
        return protocol.ToLowerInvariant() switch
        {
            "tcp" => new[] { "TCP" },
            "udp" => new[] { "UDP" },
            _ => new[] { "TCP", "UDP" },
        };
    }

    public async Task<FirewallRuleInfo> AddRuleAsync(string name, int port, string protocol, CancellationToken ct = default)
    {
        name = (name ?? string.Empty).Trim();
        if (!IsValidRuleName(name))
        {
            throw new ArgumentException(
                "Rule names may only contain letters, digits, spaces, '.', '(', ')', '_' and '-' (1–64 characters).", nameof(name));
        }

        if (port is < 1 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 1 and 65535.");
        }

        var ruleName = RulePrefix + name;
        foreach (var proto in ProtocolsFor(protocol ?? string.Empty))
        {
            await RunElevatedAsync(BuildAddRuleArguments(ruleName, proto, port), ct).ConfigureAwait(false);
        }

        return new FirewallRuleInfo
        {
            Name = ruleName,
            Port = port,
            Protocol = (protocol ?? string.Empty).ToLowerInvariant(),
            Active = true,
            Created = DateTime.UtcNow,
        };
    }

    public const string DefaultProfiles = "private,domain";

    private static readonly System.Text.RegularExpressions.Regex RuleNamePattern =
        new(@"^[\w .()\-]{1,64}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    public static bool IsValidRuleName(string? name) => name is not null && RuleNamePattern.IsMatch(name);

    internal static string BuildAddRuleArguments(string ruleName, string protocol, int port)
        => $"advfirewall firewall add rule name=\"{ruleName}\" dir=in action=allow protocol={protocol} localport={port} profile={DefaultProfiles}";

    internal static string? BuildDeleteRuleArguments(string? ruleName)
    {
        if (ruleName is null || !ruleName.StartsWith(RulePrefix, StringComparison.Ordinal)
            || !IsValidRuleName(ruleName[RulePrefix.Length..]))
        {
            return null;
        }

        return $"advfirewall firewall delete rule name=\"{ruleName}\"";
    }

    public async Task DeleteRuleAsync(FirewallRuleInfo rule, CancellationToken ct = default)
    {
        var args = BuildDeleteRuleArguments(rule.Name)
            ?? throw new InvalidOperationException($"'{rule.Name}' is not a firewall rule created by this app, so it won't be deleted.");
        await RunElevatedAsync(args, ct).ConfigureAwait(false);
    }

    public async Task<bool> RuleExistsAsync(FirewallRuleInfo rule, CancellationToken ct = default)
    {
        if (BuildDeleteRuleArguments(rule.Name) is null)
        {
            return false;
        }

        var psi = new ProcessStartInfo("netsh")
        {
            ArgumentList = { "advfirewall", "firewall", "show", "rule", $"name=\"{rule.Name}\"" },
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        using var process = Process.Start(psi) ?? throw new InvalidOperationException("Could not start netsh.exe.");
        var stdout = await process.StandardOutput.ReadToEndAsync(ct).ConfigureAwait(false);
        await process.WaitForExitAsync(ct).ConfigureAwait(false);

        return process.ExitCode == 0 && !stdout.ToLowerInvariant().Contains("no rules match");
    }

    public async Task RunElevatedAsync(string netshArguments, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo("netsh")
        {
            Arguments = netshArguments,
            UseShellExecute = true,
            Verb = "runas",
            WindowStyle = ProcessWindowStyle.Hidden,
        };

        using var process = Process.Start(psi) ?? throw new InvalidOperationException("Could not start elevated netsh.exe.");
        await process.WaitForExitAsync(ct).ConfigureAwait(false);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"netsh command failed (exit code {process.ExitCode}): netsh {netshArguments}");
        }
    }
}
