namespace MPServerProvider.Core.Models;

public class ServerStats
{
    public long TotalUptimeSeconds { get; set; }

    public long MonthUptimeSeconds { get; set; }

    public string? MonthKey { get; set; }

    public int CrashCount { get; set; }

    public DateTime? LastStarted { get; set; }
}
