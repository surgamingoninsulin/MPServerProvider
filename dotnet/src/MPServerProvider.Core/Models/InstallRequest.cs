namespace MPServerProvider.Core.Models;

public class InstallRequest
{
    public required GameDefinition Game { get; init; }

    public string ServerName { get; init; } = string.Empty;

    public string? Variant { get; init; }

    public string? Version { get; init; }

    public string TargetDir { get; init; } = string.Empty;

    public int Port { get; init; }

    public int RamMinMb { get; init; }

    public int RamMaxMb { get; init; }

    public string? Description { get; init; }

    public bool EulaAccepted { get; init; }

    public SteamLogin? SteamLogin { get; init; }
}

public sealed record SteamLogin(string Username, string? Password = null, string? GuardCode = null)
{
    public override string ToString() => $"SteamLogin {{ Username = {Username} }}";
}
