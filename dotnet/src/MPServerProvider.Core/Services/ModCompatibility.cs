using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public sealed record ModCompatibilityResult(bool IsCompatible, string? Reason)
{
    public static readonly ModCompatibilityResult Ok = new(true, null);

    public static ModCompatibilityResult No(string reason) => new(false, reason);
}

public static class ModCompatibility
{
    public const string MinecraftJavaId = "minecraft-java";

    public static bool IsMinecraftJava(string? gameId) => string.Equals(gameId, MinecraftJavaId, StringComparison.OrdinalIgnoreCase);

    public static bool IsJarFile(string fileName) => fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase);

    public static MinecraftPlatform AcceptedPlatforms(ServerInstance server)
    {
        var type = (server.ServerType ?? string.Empty).Trim().ToLowerInvariant();
        return type switch
        {
            "paper" or "purpur" or "folia" or "pufferfish" or "leaf" => MinecraftPlatform.Bukkit | MinecraftPlatform.Paper,
            "spigot" or "bukkit" or "craftbukkit" => MinecraftPlatform.Bukkit,
            "fabric" => MinecraftPlatform.Fabric,
            "quilt" => MinecraftPlatform.Fabric | MinecraftPlatform.Quilt,
            "forge" => MinecraftPlatform.Forge,
            // NeoForge for 1.20.1 was still a drop-in Forge fork and loads Forge mods; later ones don't.
            "neoforge" => MinecraftVersionRange.ServerMinecraftVersion(server.ServerVersion) == "1.20.1"
                ? MinecraftPlatform.NeoForge | MinecraftPlatform.Forge
                : MinecraftPlatform.NeoForge,
            "velocity" => MinecraftPlatform.Velocity,
            "bungeecord" or "waterfall" => MinecraftPlatform.BungeeCord,
            // Unset variant means vanilla (that's what the installer does) — unless the server was
            // clearly set up with a plugins folder, in which case stay lenient.
            "" => string.Equals(server.ModsFolder, "plugins", StringComparison.OrdinalIgnoreCase)
                ? MinecraftPlatform.AnyPlugin
                : MinecraftPlatform.None,
            "vanilla" => MinecraftPlatform.None,
            _ => MinecraftPlatform.AnyMod | MinecraftPlatform.AnyPlugin,
        };
    }

    public static string VariantLabel(ServerInstance server)
    {
        var type = (server.ServerType ?? string.Empty).Trim();
        return type.Length == 0 ? "Vanilla"
            : type.Equals("bungeecord", StringComparison.OrdinalIgnoreCase) ? "BungeeCord"
            : type.Equals("neoforge", StringComparison.OrdinalIgnoreCase) ? "NeoForge"
            : char.ToUpperInvariant(type[0]) + type[1..].ToLowerInvariant();
    }

    public static string AcceptedDescription(ServerInstance server)
    {
        var accepted = AcceptedPlatforms(server);
        if (accepted == MinecraftPlatform.None)
        {
            return "nothing — vanilla servers can't load mods or plugins";
        }

        return accepted switch
        {
            MinecraftPlatform.Bukkit | MinecraftPlatform.Paper => "Bukkit/Spigot/Paper plugins (.jar)",
            MinecraftPlatform.Bukkit => "Bukkit/Spigot plugins (.jar)",
            MinecraftPlatform.Fabric => "Fabric mods (.jar)",
            MinecraftPlatform.Fabric | MinecraftPlatform.Quilt => "Fabric and Quilt mods (.jar)",
            MinecraftPlatform.Forge => "Forge mods (.jar)",
            MinecraftPlatform.NeoForge => "NeoForge mods (.jar)",
            MinecraftPlatform.NeoForge | MinecraftPlatform.Forge => "NeoForge and Forge mods (.jar)",
            MinecraftPlatform.Velocity => "Velocity proxy plugins (.jar)",
            MinecraftPlatform.BungeeCord => "BungeeCord proxy plugins (.jar)",
            _ => "Minecraft mods and plugins (.jar)",
        };
    }

    private static bool IsProxy(MinecraftPlatform accepted)
        => accepted is MinecraftPlatform.Velocity or MinecraftPlatform.BungeeCord;

    public static ModCompatibilityResult Check(ModItem item, ServerInstance server, GameDefinition? serverGame)
    {
        var isMcServer = IsMinecraftJava(server.ServerGame);
        var isJar = !item.IsDirectory && IsJarFile(item.FileName);
        var info = item.Minecraft;
        var looksLikeMinecraft = isJar && info is { IsReadable: true, Platforms: not MinecraftPlatform.None };

        if (!isMcServer)
        {
            if (looksLikeMinecraft || IsMinecraftJava(item.GameId))
            {
                return ModCompatibilityResult.No("Minecraft mod/plugin — not for this game");
            }

            if (!string.Equals(item.GameId, server.ServerGame, StringComparison.OrdinalIgnoreCase))
            {
                return ModCompatibilityResult.No("Made for a different game");
            }

            var extensions = serverGame?.Mods.Extensions ?? new List<string>();
            if (!item.IsDirectory && extensions.Count > 0
                && !extensions.Any(e => string.Equals(e.TrimStart('.'), Path.GetExtension(item.FileName).TrimStart('.'), StringComparison.OrdinalIgnoreCase)))
            {
                return ModCompatibilityResult.No($"Not a {string.Join("/", extensions)} file");
            }

            return ModCompatibilityResult.Ok;
        }

        // Minecraft Java server from here on.
        if (!IsMinecraftJava(item.GameId))
        {
            return ModCompatibilityResult.No("Not a Minecraft mod");
        }

        if (!isJar)
        {
            return ModCompatibilityResult.No(item.IsDirectory
                ? "Folder — Minecraft only loads .jar files"
                : "Not a .jar — Minecraft only loads .jar files");
        }

        if (info is { IsReadable: false })
        {
            return ModCompatibilityResult.No("Not a valid jar (can't be opened)");
        }

        var accepted = AcceptedPlatforms(server);
        if (accepted == MinecraftPlatform.None)
        {
            return ModCompatibilityResult.No("Vanilla server — can't load mods or plugins");
        }

        if (info is null || info.IsUnknown)
        {
            // Lenient: no descriptor we know (a library jar, say). Allowed, but tagged in the UI.
            return ModCompatibilityResult.Ok;
        }

        var matched = info.Platforms & accepted;
        if (matched == MinecraftPlatform.None)
        {
            var variant = VariantLabel(server);
            if (info.Platforms == MinecraftPlatform.Quilt && accepted.HasFlag(MinecraftPlatform.Fabric))
            {
                return ModCompatibilityResult.No("Quilt-only mod — Fabric can't load it");
            }

            if (info.IsMod && !info.IsPlugin && (accepted & MinecraftPlatform.AnyPlugin) != 0)
            {
                return ModCompatibilityResult.No($"{info.KindLabel} mod — {variant} takes plugins");
            }

            if (info.IsPlugin && !info.IsMod && (accepted & MinecraftPlatform.AnyMod) != 0)
            {
                return ModCompatibilityResult.No($"{info.KindLabel} — {variant} takes mods");
            }

            return ModCompatibilityResult.No($"{info.KindLabel} — not for {variant}");
        }

        if (IsProxy(accepted))
        {
            return ModCompatibilityResult.Ok; // proxies run any backend version
        }

        var serverMc = MinecraftVersionRange.ServerMinecraftVersion(server.ServerVersion);
        if (serverMc is null)
        {
            return ModCompatibilityResult.Ok; // "latest", snapshot: can't tell
        }

        string? firstMismatch = null;
        foreach (var platform in new[]
                 {
                     MinecraftPlatform.Fabric, MinecraftPlatform.Quilt, MinecraftPlatform.Forge,
                     MinecraftPlatform.NeoForge, MinecraftPlatform.Paper, MinecraftPlatform.Bukkit,
                 })
        {
            if (!matched.HasFlag(platform))
            {
                continue;
            }

            bool? fits;
            string needs;
            if (platform is MinecraftPlatform.Bukkit or MinecraftPlatform.Paper)
            {
                fits = MinecraftVersionRange.SatisfiesApiVersion(info.ApiVersion, serverMc);
                needs = $"Needs MC {info.ApiVersion}+";
            }
            else
            {
                info.MinecraftRanges.TryGetValue(platform, out var range);
                var maven = platform is MinecraftPlatform.Forge or MinecraftPlatform.NeoForge;
                fits = range is null ? null : MinecraftVersionRange.Satisfies(range, serverMc, maven);
                needs = $"Needs MC {MinecraftVersionRange.Describe(range, maven)}";
            }

            if (fits != false)
            {
                return ModCompatibilityResult.Ok; // matches, or unknown → lenient
            }

            firstMismatch ??= $"{needs}, server is {serverMc}";
        }

        return firstMismatch is null ? ModCompatibilityResult.Ok : ModCompatibilityResult.No(firstMismatch);
    }

    public static List<string> Tags(ModItem item)
    {
        var tags = new List<string>();
        if (!IsMinecraftJava(item.GameId) && item.Minecraft is not { IsReadable: true, Platforms: not MinecraftPlatform.None })
        {
            return tags;
        }

        if (item.IsDirectory || !IsJarFile(item.FileName))
        {
            // A folder inside a server's plugins/mods folder is a plugin's data/config folder.
            tags.Add(item.IsDirectory ? "Data folder" : "Not a jar");
            return tags;
        }

        var info = item.Minecraft;
        if (info is null)
        {
            return tags;
        }

        if (!info.IsReadable)
        {
            tags.Add("Unreadable jar");
            return tags;
        }

        if (info.IsUnknown)
        {
            tags.Add("Unknown jar");
            return tags;
        }

        var main = new List<string>();
        if (info.PrimaryMinecraftRange is { } range)
        {
            var maven = !info.MinecraftRanges.ContainsKey(MinecraftPlatform.Fabric) && !info.MinecraftRanges.ContainsKey(MinecraftPlatform.Quilt);
            var described = MinecraftVersionRange.Describe(range, maven);
            if (described != "any")
            {
                main.Add($"MC {described}");
            }
        }

        main.Add(info.KindLabel);
        if (info.IsPlugin && info.ApiVersion is { } api)
        {
            main.Add($"API {api}");
        }

        tags.Add(string.Join(" · ", main));
        if (info.NeedsFabricApi)
        {
            tags.Add("Needs Fabric API");
        }

        return tags;
    }
}
