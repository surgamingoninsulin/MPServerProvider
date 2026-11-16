namespace MPServerProvider.Core.Models;

/// <summary>
/// Optional rich "Game Detail" page content for a <see cref="GameDefinition"/> (JSON key
/// <c>details</c>). Not part of the original app's manifest shape — added so the Library's game
/// detail view can show researched, hardcoded information (about text, developer/publisher,
/// ports, requirements, setup tips, screenshots) without any runtime web lookups. Every member is
/// optional: older manifests without a <c>details</c> object (or with only some fields) still
/// load, and the UI hides whatever is missing.
/// </summary>
public class GameDetails
{
    /// <summary>Multi-paragraph description; paragraphs are separated by a blank line (<c>\n\n</c>).</summary>
    public string? About { get; set; }

    public string? Developer { get; set; }

    public string? Publisher { get; set; }

    /// <summary>Free-form display date, e.g. <c>"Feb 8, 2018"</c>.</summary>
    public string? ReleaseDate { get; set; }

    /// <summary>Official game website (http/https only; anything else is ignored by the UI).</summary>
    public string? Website { get; set; }

    /// <summary>Store page (Steam store page or the official purchase page).</summary>
    public string? StoreUrl { get; set; }

    /// <summary>Official (or de-facto official) dedicated-server documentation.</summary>
    public string? DocsUrl { get; set; }

    /// <summary>Display string such as <c>"Up to 10 players"</c>.</summary>
    public string? MaxPlayers { get; set; }

    public List<string> Features { get; set; } = new();

    public ServerRequirementsSpec? ServerRequirements { get; set; }

    public List<GamePortInfo> Ports { get; set; } = new();

    public List<string> SetupTips { get; set; } = new();

    /// <summary>Full-size screenshot image URLs.</summary>
    public List<string> Screenshots { get; set; } = new();
}

/// <summary>Dedicated-server hardware guidance (JSON key <c>server_requirements</c>).</summary>
public class ServerRequirementsSpec
{
    public int? MinRamMb { get; set; }

    public int? RecommendedRamMb { get; set; }

    public int? DiskGb { get; set; }

    public string? Notes { get; set; }
}

/// <summary>One documented network port a server uses (JSON key <c>ports[]</c>).</summary>
public class GamePortInfo
{
    /// <summary>A single port (<c>"28015"</c>) or an inclusive range (<c>"26900-26902"</c>).</summary>
    public string Port { get; set; } = string.Empty;

    /// <summary><c>"TCP"</c>, <c>"UDP"</c> or <c>"TCP+UDP"</c>.</summary>
    public string Protocol { get; set; } = string.Empty;

    public string? Purpose { get; set; }
}
