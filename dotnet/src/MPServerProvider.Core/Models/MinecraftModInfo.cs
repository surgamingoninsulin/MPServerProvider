namespace MPServerProvider.Core.Models;

[Flags]
public enum MinecraftPlatform
{
    None = 0,

    Fabric = 1,

    Quilt = 2,

    Forge = 4,

    NeoForge = 8,

    Bukkit = 16,

    Paper = 32,

    BungeeCord = 64,

    Velocity = 128,

    AnyMod = Fabric | Quilt | Forge | NeoForge,

    AnyPlugin = Bukkit | Paper | BungeeCord | Velocity,
}

public sealed class MinecraftModInfo
{
    public static readonly MinecraftModInfo NotAJar = new() { IsReadable = false, Error = "Not a .jar file" };

    public bool IsReadable { get; init; } = true;

    public string? Error { get; init; }

    public MinecraftPlatform Platforms { get; init; }

    public string? Id { get; init; }

    public string? Name { get; init; }

    public string? Version { get; init; }

    public IReadOnlyDictionary<MinecraftPlatform, string> MinecraftRanges { get; init; }
        = new Dictionary<MinecraftPlatform, string>();

    public string? ApiVersion { get; init; }

    public IReadOnlyList<string> RequiredDependencies { get; init; } = Array.Empty<string>();

    public bool IsUnknown => IsReadable && Platforms == MinecraftPlatform.None;

    public bool IsMod => (Platforms & MinecraftPlatform.AnyMod) != 0;

    public bool IsPlugin => (Platforms & MinecraftPlatform.AnyPlugin) != 0;

    public bool NeedsFabricApi => RequiredDependencies.Any(d =>
        d is "fabric-api" or "fabric" or "quilted_fabric_api" or "qsl");

    public string KindLabel
    {
        get
        {
            if (!IsReadable)
            {
                return "Unreadable jar";
            }

            var parts = new List<string>();
            if (Platforms.HasFlag(MinecraftPlatform.Fabric)) parts.Add("Fabric");
            if (Platforms.HasFlag(MinecraftPlatform.Quilt)) parts.Add("Quilt");
            if (Platforms.HasFlag(MinecraftPlatform.Forge)) parts.Add("Forge");
            if (Platforms.HasFlag(MinecraftPlatform.NeoForge)) parts.Add("NeoForge");
            if (Platforms.HasFlag(MinecraftPlatform.Paper)) parts.Add("Paper plugin");
            else if (Platforms.HasFlag(MinecraftPlatform.Bukkit)) parts.Add("Bukkit plugin");
            if (Platforms.HasFlag(MinecraftPlatform.BungeeCord)) parts.Add("BungeeCord plugin");
            if (Platforms.HasFlag(MinecraftPlatform.Velocity)) parts.Add("Velocity plugin");
            return parts.Count == 0 ? "Unknown jar" : string.Join(" / ", parts);
        }
    }

    public string? PrimaryMinecraftRange
    {
        get
        {
            foreach (var platform in new[] { MinecraftPlatform.Fabric, MinecraftPlatform.Quilt, MinecraftPlatform.NeoForge, MinecraftPlatform.Forge })
            {
                if (MinecraftRanges.TryGetValue(platform, out var range))
                {
                    return range;
                }
            }

            return null;
        }
    }
}
