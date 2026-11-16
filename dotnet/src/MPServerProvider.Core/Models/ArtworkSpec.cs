namespace MPServerProvider.Core.Models;

/// <summary>
/// Artwork URLs/colors for a game's catalog card.
/// Ported from <c>MPServerProvider.Core.Models.ArtworkSpec</c> (decompile token 0x02000029).
/// </summary>
public class ArtworkSpec
{
    public string? Cover { get; set; }

    public string? Hero { get; set; }

    public string? Accent { get; set; }
}
