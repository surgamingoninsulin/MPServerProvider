namespace MPServerProvider.Core.Models;

/// <summary>
/// Accumulated uptime/crash statistics for a server.
/// Ported from <c>MPServerProvider.Core.Models.ServerStats</c> (decompile token 0x02000039).
/// This type extends plain <c>System.Object</c> in the decompile (no
/// <c>CommunityToolkit.Mvvm.ObservableObject</c> base, no observable-property evidence anywhere
/// in <c>MPServerProvider.Core.Models</c>), so it is a plain DTO rather than a bindable
/// view-model-adjacent type.
/// </summary>
public class ServerStats
{
    public long TotalUptimeSeconds { get; set; }

    public long MonthUptimeSeconds { get; set; }

    public string? MonthKey { get; set; }

    public int CrashCount { get; set; }

    public DateTime? LastStarted { get; set; }
}
