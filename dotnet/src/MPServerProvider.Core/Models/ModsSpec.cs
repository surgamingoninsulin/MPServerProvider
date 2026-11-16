namespace MPServerProvider.Core.Models;

public class ModsSpec
{
    public bool Supported { get; set; }

    public string Folder { get; set; } = "mods";

    public List<string> Extensions { get; set; } = new();

    public string Kind { get; set; } = "mod";
}
