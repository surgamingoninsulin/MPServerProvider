namespace MPServerProvider.Core.Models;

/// <summary>
/// Mod/addon support description for a game.
/// Ported from <c>MPServerProvider.Core.Models.ModsSpec</c> (decompile token 0x0200002e).
/// </summary>
public class ModsSpec
{
    public bool Supported { get; set; }

    public string Folder { get; set; } = "mods";

    public List<string> Extensions { get; set; } = new();

    public string Kind { get; set; } = "mod";
}
