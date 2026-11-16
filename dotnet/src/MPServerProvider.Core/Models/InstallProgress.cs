namespace MPServerProvider.Core.Models;

/// <summary>
/// Progress update emitted while an install/download is running.
/// Ported from <c>MPServerProvider.Core.Models.InstallProgress</c> (decompile token 0x02000032).
/// This is an in-process/IPC payload rather than an on-disk manifest, so there is no
/// ground-truth JSON sample for it; field names follow the same <c>snake_case</c> convention
/// used by every type that *is* ground truth.
/// </summary>
public class InstallProgress
{
    public string Status { get; set; } = string.Empty;

    public double Percent { get; set; }

    public long BytesReceived { get; set; }

    public long? TotalBytes { get; set; }

    /// <summary>
    /// Mirrors the decompiled <c>InstallProgress.Indeterminate(string)</c> factory helper:
    /// a status update with no known percentage.
    /// </summary>
    public static InstallProgress Indeterminate(string status) => new()
    {
        Status = status,
    };

    /// <summary>
    /// Mirrors the decompiled <c>InstallProgress.At(string, double)</c> factory helper:
    /// a status update at a known percentage.
    /// </summary>
    public static InstallProgress At(string status, double percent) => new()
    {
        Status = status,
        Percent = percent,
    };
}
