namespace MPServerProvider.Core.Models;

public class InstallResult
{
    public string ServerFile { get; init; } = string.Empty;

    public string? Version { get; init; }

    public string? StartCommand { get; init; }

    public int JavaMajor { get; init; }
}
