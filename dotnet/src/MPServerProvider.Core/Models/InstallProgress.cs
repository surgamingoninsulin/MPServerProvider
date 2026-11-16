namespace MPServerProvider.Core.Models;

public class InstallProgress
{
    public string Status { get; set; } = string.Empty;

    public double Percent { get; set; }

    public long BytesReceived { get; set; }

    public long? TotalBytes { get; set; }

    public string? Link { get; set; }

    public static InstallProgress Indeterminate(string status) => new()
    {
        Status = status,
    };

    public static InstallProgress At(string status, double percent) => new()
    {
        Status = status,
        Percent = percent,
    };
}
