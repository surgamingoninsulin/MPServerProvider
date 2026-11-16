namespace MPServerProvider.Core.Models;

public class ModItem
{
    public string FileName { get; set; } = string.Empty;

    public string GameId { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public DateTime Imported { get; set; }

    public string? LibraryPath { get; set; }

    public List<string> AssignedServers { get; set; } = new();

    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsDirectory { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime ModifiedUtc { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public MinecraftModInfo? Minecraft { get; set; }
}
