namespace MPServerProvider.Core.Models;

/// <summary>
/// The Minecraft loaders/server platforms a jar declares itself for. A flags enum because
/// multi-platform jars are common (e.g. a permissions plugin shipping <c>plugin.yml</c>,
/// <c>bungee.yml</c> and <c>velocity-plugin.json</c> in one file).
/// </summary>
[Flags]
public enum MinecraftPlatform
{
    None = 0,

    /// <summary><c>fabric.mod.json</c>.</summary>
    Fabric = 1,

    /// <summary><c>quilt.mod.json</c>.</summary>
    Quilt = 2,

    /// <summary><c>META-INF/mods.toml</c> (modern Forge) or <c>mcmod.info</c> (legacy Forge).</summary>
    Forge = 4,

    /// <summary><c>META-INF/neoforge.mods.toml</c>, or a <c>mods.toml</c> that depends on <c>neoforge</c>.</summary>
    NeoForge = 8,

    /// <summary><c>plugin.yml</c>: Bukkit/Spigot API plugin (runs on Spigot, Paper and Purpur).</summary>
    Bukkit = 16,

    /// <summary><c>paper-plugin.yml</c>: Paper-only plugin (Paper and its forks such as Purpur).</summary>
    Paper = 32,

    /// <summary><c>bungee.yml</c>: BungeeCord/Waterfall proxy plugin.</summary>
    BungeeCord = 64,

    /// <summary><c>velocity-plugin.json</c>: Velocity proxy plugin.</summary>
    Velocity = 128,

    /// <summary>Every loader-side (mod) platform.</summary>
    AnyMod = Fabric | Quilt | Forge | NeoForge,

    /// <summary>Every server-side (plugin) platform, including proxies.</summary>
    AnyPlugin = Bukkit | Paper | BungeeCord | Velocity,
}

/// <summary>
/// What could be learned about a Minecraft mod/plugin jar by reading the descriptor files inside
/// it (see <see cref="Services.MinecraftJarReader"/>). Immutable and cached by the mod library
/// keyed on path + size + last-write time, so a jar is opened at most once per change.
/// </summary>
public sealed class MinecraftModInfo
{
    /// <summary>Shared instance for a file that isn't a jar at all (not read).</summary>
    public static readonly MinecraftModInfo NotAJar = new() { IsReadable = false, Error = "Not a .jar file" };

    /// <summary><c>false</c> when the file couldn't be opened as a zip (corrupt, truncated, locked).</summary>
    public bool IsReadable { get; init; } = true;

    /// <summary>Why <see cref="IsReadable"/> is <c>false</c>.</summary>
    public string? Error { get; init; }

    public MinecraftPlatform Platforms { get; init; }

    public string? Id { get; init; }

    public string? Name { get; init; }

    public string? Version { get; init; }

    /// <summary>
    /// The Minecraft version requirement each platform's descriptor declares, in that
    /// descriptor's own syntax (Fabric/Quilt semver predicates, Forge Maven ranges, a legacy
    /// <c>mcmod.info</c> <c>mcversion</c>). Absent when the descriptor doesn't say.
    /// </summary>
    public IReadOnlyDictionary<MinecraftPlatform, string> MinecraftRanges { get; init; }
        = new Dictionary<MinecraftPlatform, string>();

    /// <summary><c>api-version</c> from <c>plugin.yml</c>/<c>paper-plugin.yml</c> (e.g. <c>1.21</c>).</summary>
    public string? ApiVersion { get; init; }

    /// <summary>Required (non-optional) dependency ids other than the game/loader itself.</summary>
    public IReadOnlyList<string> RequiredDependencies { get; init; } = Array.Empty<string>();

    /// <summary>A readable jar with no descriptor this reader knows (a library, a client-only
    /// tweak, or something that isn't a Minecraft mod at all).</summary>
    public bool IsUnknown => IsReadable && Platforms == MinecraftPlatform.None;

    public bool IsMod => (Platforms & MinecraftPlatform.AnyMod) != 0;

    public bool IsPlugin => (Platforms & MinecraftPlatform.AnyPlugin) != 0;

    /// <summary>Whether this jar needs Fabric API (<c>fabric-api</c>, the legacy <c>fabric</c> id, or
    /// Quilt's <c>quilted_fabric_api</c>/<c>qsl</c>).</summary>
    public bool NeedsFabricApi => RequiredDependencies.Any(d =>
        d is "fabric-api" or "fabric" or "quilted_fabric_api" or "qsl");

    /// <summary>Human label for the declared platforms, e.g. <c>Fabric</c> or <c>Paper plugin</c>.</summary>
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

    /// <summary>The first declared Minecraft range (for display when the server isn't known).</summary>
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
