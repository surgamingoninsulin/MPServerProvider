using System.Diagnostics;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.FirewallService</c> (decompile token 0x0200000b,
/// see <c>docs/decompile-report/report/MPServerProvider.Core.dll/MPServerProvider.Core.Services/
/// 0x0200000b_FirewallService.md</c>). Manages Windows Firewall rules for server ports by
/// shelling out to <c>netsh.exe</c> rather than the COM Windows Firewall API
/// (<c>HNetCfg.FwPolicy2</c>/<c>INetFwPolicy2</c>) — the decompile's recovered string literals
/// (<c>"tcp"</c>, <c>"udp"</c>, <c>"TCP"</c>, <c>"UDP"</c> in <c>ProtocolsFor</c>, and the
/// <c>"MPSM - "</c> rule-name prefix) are unambiguous evidence of a <c>netsh</c>-based
/// implementation, already confirmed by the proven Rust port
/// (<c>crates/mpsp-core/src/services/firewall.rs</c>) which recovered the exact command strings
/// this restores verbatim:
/// <c>netsh advfirewall firewall add rule name="MPSM - {name}" dir=in action=allow protocol={proto} localport={port}</c>.
///
/// <para>
/// This port keeps that exact rule-naming/command shape (so existing installs' firewall rules
/// remain recognized/manageable), but uses a cleaner C#-idiomatic elevation mechanism than the
/// Rust port could: <see cref="ProcessStartInfo"/> with <c>UseShellExecute = true</c> and
/// <c>Verb = "runas"</c> launches <c>netsh.exe</c> directly, elevated, with no intermediate
/// <c>cmd.exe /c</c> wrapper (the Rust port needed that wrapper since it had no straightforward
/// <c>ShellExecuteEx</c> equivalent without extra Win32 interop; .NET's <see cref="Process"/>
/// exposes the <c>runas</c> verb natively).
/// </para>
/// </summary>
public sealed class FirewallService
{
    /// <summary>
    /// Prefix applied to every rule name this app creates/manages. Matches the decompiled
    /// <c>Prefix</c> static field (confirmed by the <c>"MPSM - "</c> string literal recovered in
    /// <c>AddRuleAsync</c>'s state machine by the Rust port's decompile cross-reference).
    /// </summary>
    public const string RulePrefix = "MPSM - ";

    /// <summary>
    /// Port of <c>FirewallService.ProtocolsFor(string)</c>: normalizes a protocol spec string
    /// into the concrete <c>netsh</c> protocol tokens to create rules for. Matches the
    /// decompiled string literals exactly ("tcp", "udp", "TCP", "UDP"): anything that
    /// lowercases to "tcp" or "udp" maps to that single protocol; anything else (including
    /// "both", empty, or unrecognized) maps to both TCP and UDP.
    /// </summary>
    public static IReadOnlyList<string> ProtocolsFor(string protocol)
    {
        return protocol.ToLowerInvariant() switch
        {
            "tcp" => new[] { "TCP" },
            "udp" => new[] { "UDP" },
            _ => new[] { "TCP", "UDP" },
        };
    }

    /// <summary>
    /// Port of <c>FirewallService.AddRuleAsync(string name, int port, string protocol)</c>.
    /// Builds one <c>netsh advfirewall firewall add rule ...</c> invocation per protocol
    /// returned by <see cref="ProtocolsFor"/> and runs each elevated. Returns the
    /// <see cref="FirewallRuleInfo"/> record the caller should persist (the original calls
    /// <c>SettingsService.Save()</c> after this — persistence is owned by the settings service
    /// landing elsewhere in this port, not duplicated here).
    /// </summary>
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

    /// <summary>
    /// Firewall profiles new rules apply to. Public networks are deliberately excluded: opening
    /// a game port on café/airport Wi-Fi is never what a home server operator wants.
    /// </summary>
    public const string DefaultProfiles = "private,domain";

    private static readonly System.Text.RegularExpressions.Regex RuleNamePattern =
        new(@"^[\w .()\-]{1,64}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    /// <summary>
    /// SECURITY: the name is embedded in an elevated <c>netsh</c> command line, so anything
    /// outside <c>^[\w .()\-]{1,64}$</c> (notably <c>"</c>, which could append extra netsh
    /// parameters such as <c>program=</c>) is rejected.
    /// </summary>
    public static bool IsValidRuleName(string? name) => name is not null && RuleNamePattern.IsMatch(name);

    /// <summary>The <c>netsh</c> arguments to add one inbound allow rule.</summary>
    internal static string BuildAddRuleArguments(string ruleName, string protocol, int port)
        => $"advfirewall firewall add rule name=\"{ruleName}\" dir=in action=allow protocol={protocol} localport={port} profile={DefaultProfiles}";

    /// <summary>
    /// The <c>netsh</c> arguments to delete <paramref name="ruleName"/>, or <c>null</c> when the
    /// name isn't one this app created (missing <see cref="RulePrefix"/>) or fails validation —
    /// a tampered <c>settings.json</c> entry like <c>"all"</c> must never reach netsh.
    /// </summary>
    internal static string? BuildDeleteRuleArguments(string? ruleName)
    {
        if (ruleName is null || !ruleName.StartsWith(RulePrefix, StringComparison.Ordinal)
            || !IsValidRuleName(ruleName[RulePrefix.Length..]))
        {
            return null;
        }

        return $"advfirewall firewall delete rule name=\"{ruleName}\"";
    }

    /// <summary>Port of <c>FirewallService.DeleteRuleAsync(FirewallRuleInfo rule)</c>.</summary>
    public async Task DeleteRuleAsync(FirewallRuleInfo rule, CancellationToken ct = default)
    {
        var args = BuildDeleteRuleArguments(rule.Name)
            ?? throw new InvalidOperationException($"'{rule.Name}' is not a firewall rule created by this app, so it won't be deleted.");
        await RunElevatedAsync(args, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Port of <c>FirewallService.RuleExistsAsync(FirewallRuleInfo rule)</c>. Unlike add/delete,
    /// this runs <c>netsh advfirewall firewall show rule name="..."</c> *without* elevation
    /// (a read-only query needs no admin rights), capturing stdout and treating a successful
    /// run whose output doesn't contain "No rules match the specified criteria" as "exists".
    /// </summary>
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

    /// <summary>
    /// Port of <c>FirewallService.RunElevatedAsync(string commandLine)</c>. Launches
    /// <c>netsh.exe {arguments}</c> directly via <see cref="ProcessStartInfo"/> with
    /// <c>UseShellExecute = true</c> and <c>Verb = "runas"</c> (triggers a UAC elevation prompt
    /// if the current process isn't already elevated), waits for exit, and throws on a non-zero
    /// exit code. See the type doc comment for why this C# idiom replaces the Rust port's
    /// <c>cmd.exe /c</c> wrapper while keeping the exact same underlying <c>netsh</c> command
    /// strings.
    /// </summary>
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
