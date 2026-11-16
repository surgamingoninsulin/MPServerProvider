namespace MPServerProvider.Core.Models;

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

    public GameDetails? Details { get; set; }

    public bool IsBuiltIn { get; set; }

    public string? SourceFile { get; set; }
}
