namespace MPServerProvider.Core.Models;

/// <summary>
/// A single imported mod/addon file tracked by the mod library.
/// Ported from <c>MPServerProvider.Core.Models.ModItem</c> (decompile token 0x0200002f).
/// No real on-disk sample of this type was available in <c>data/</c> (the repo ships no
/// mod-library index file), so field names are reconstructed by applying the same
/// <c>snake_case</c> convention confirmed everywhere else by
/// <c>MPServerProvider.Core.Json.Json</c>'s global <see cref="System.Text.Json.JsonNamingPolicy.SnakeCaseLower"/>
/// policy. See the already-verified Rust port (<c>crates/mpsp-core/src/models/mod_item.rs</c>)
/// for the same judgment call.
/// </summary>
public class ModItem
{
    public string FileName { get; set; } = string.Empty;

    public string GameId { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public DateTime Imported { get; set; }

    public string? LibraryPath { get; set; }

    public List<string> AssignedServers { get; set; } = new();

    /// <summary>Whether <see cref="LibraryPath"/> is a folder (folder mods, Workshop items).</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsDirectory { get; set; }

    /// <summary>Last-write time of the library file (with <see cref="SizeBytes"/>, the change key).</summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime ModifiedUtc { get; set; }

    /// <summary>
    /// What the jar's own descriptors say (platform, Minecraft versions, dependencies), for
    /// <c>.jar</c> files when the scan was asked to read metadata; otherwise <c>null</c>.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public MinecraftModInfo? Minecraft { get; set; }
}
