namespace MPServerProvider.Core.Models;

/// <summary>
/// Top-level game/provider manifest (one per file under <c>data/providers/json/*.json</c>).
/// Ported from <c>MPServerProvider.Core.Models.GameDefinition</c> (decompile token 0x02000028).
/// JSON field names are <c>snake_case</c>, produced globally by
/// <see cref="Json.Json.Options"/> (<see cref="System.Text.Json.JsonNamingPolicy.SnakeCaseLower"/>),
/// confirmed against every file under <c>data/providers/json/*.json</c>.
/// </summary>
public class GameDefinition
{
    public int ManifestVersion { get; set; } = 1;

    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> Genres { get; set; } = new();

    public ArtworkSpec Artwork { get; set; } = new();

    public InstallSpec Install { get; set; } = new();

    public RuntimeSpec Runtime { get; set; } = new();

    public NetworkSpec Network { get; set; } = new();

    public ModsSpec Mods { get; set; } = new();

    /// <summary>
    /// Optional researched content for the Game Detail page (JSON key <c>details</c>). Null for
    /// user-added games and for manifests written before this field existed.
    /// </summary>
    public GameDetails? Details { get; set; }

    /// <summary>
    /// Not present in the on-disk manifest JSON; set by the loader to distinguish
    /// shipped/embedded definitions from user-added ones. Omitted from serialized JSON
    /// when <c>false</c> via <see cref="System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault"/>-style
    /// handling is not configured globally, so it round-trips as a plain bool.
    /// </summary>
    public bool IsBuiltIn { get; set; }

    /// <summary>
    /// Not present in the on-disk manifest JSON; set by the loader to the path the
    /// definition was read from.
    /// </summary>
    public string? SourceFile { get; set; }
}
