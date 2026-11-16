namespace MPServerProvider.Core.Models;

public class GameDetails
{
    public string? About { get; set; }

    public string? Developer { get; set; }

    public string? Publisher { get; set; }

    public string? ReleaseDate { get; set; }

    public string? Website { get; set; }

    public string? StoreUrl { get; set; }

    public string? DocsUrl { get; set; }

    public string? MaxPlayers { get; set; }

    public List<string> Features { get; set; } = new();

    public ServerRequirementsSpec? ServerRequirements { get; set; }

    public List<GamePortInfo> Ports { get; set; } = new();

    public List<string> SetupTips { get; set; } = new();

    public List<string> Screenshots { get; set; } = new();
}

public class ServerRequirementsSpec
{
    public int? MinRamMb { get; set; }

    public int? RecommendedRamMb { get; set; }

    public int? DiskGb { get; set; }

    public string? Notes { get; set; }
}

public class GamePortInfo
{
    public string Port { get; set; } = string.Empty;

    public string Protocol { get; set; } = string.Empty;

    public string? Purpose { get; set; }
}
