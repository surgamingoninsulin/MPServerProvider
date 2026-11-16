namespace MPServerProvider.Core.Models;

/// <summary>
/// Global application settings persisted to <c>settings.json</c>.
/// Ported from <c>MPServerProvider.Core.Models.AppSettings</c> (decompile token 0x02000036).
/// Field names and shape confirmed against ground truth in <c>data/settings.json</c>.
/// Note <see cref="InstallRoot"/> is declared on the decompiled type but never appears in the
/// real <c>settings.json</c> sample (it's a reference-typed field, so it's omitted by
/// <see cref="System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull"/> whenever
/// null) — modeled as nullable accordingly.
/// </summary>
public class AppSettings
{
    public string? InstallRoot { get; set; }

    public bool StartOnBoot { get; set; }

    public bool StartMinimizedToTray { get; set; }

    public bool RestoreRunningServers { get; set; }

    /// <summary>
    /// Names of the servers that were running when the app last shut down (or crashed), kept
    /// up to date by <see cref="Services.RunningServerTracker"/>. Restarted on launch when
    /// <see cref="RestoreRunningServers"/> is on.
    /// </summary>
    public List<string> LastRunningServers { get; set; } = new();

    public bool NewsAutoScroll { get; set; } = true;

    public List<FirewallRuleInfo> FirewallRules { get; set; } = new();

    public double AssumedHostingPricePerHour { get; set; } = 0.02;
}
