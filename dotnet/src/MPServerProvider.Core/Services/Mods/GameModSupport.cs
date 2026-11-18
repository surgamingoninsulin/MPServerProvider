using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services.Mods;

/// <summary>
/// Which marketplace(s) a game can install from, and what that marketplace calls the game.
///
/// A mod source is only listed here once the whole install path actually works end to end for
/// that game - not just "the marketplace has a category for it". Several games with real
/// CurseForge/Thunderstore/mod.io listings are deliberately left out for now, or wired to the
/// launch-argument path below instead of a file drop, because this app's side of the install is
/// genuinely different per game:
///   - ARK: Survival Evolved/Ascended, Palworld and Abiotic Factor all take mod.io/CurseForge
///     mods through a launch-argument mod-ID list (e.g. `-Mods=123;456`) that the game itself
///     downloads on the next start - there is no mods folder to drop a file into. See
///     <see cref="LaunchArgFlagFor"/> and <see cref="MergeModIdIntoLaunchArgs"/>.
///   - Valheim and Aska mods on Thunderstore assume BepInEx is already installed
///     (`BepInEx/plugins/...`), and this app doesn't bootstrap BepInEx yet - its ModsSpec for
///     both games is still "Supported = false".
///   - Terraria here is the vanilla dedicated server, not tModLoader, so CurseForge's Terraria
///     mods don't apply to it.
/// </summary>
public static class GameModSupport
{
    private static readonly Dictionary<string, string[]> CurseForgeGameNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["minecraft-java"] = new[] { "Minecraft" },
        ["hytale"] = new[] { "Hytale" },
    };

    private static readonly Dictionary<string, string[]> ModIoGameNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["abiotic-factor"] = new[] { "Abiotic Factor" },
    };

    private static readonly Dictionary<string, string> ThunderstoreCommunities = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Games where "installing" a mod means adding its numeric ID to this launch-argument flag
    /// (semicolon-separated), not downloading a file - the dedicated server fetches the mod
    /// itself from the marketplace on its next start using that ID.
    /// </summary>
    private static readonly Dictionary<string, string> LaunchArgFlags = new(StringComparer.OrdinalIgnoreCase)
    {
        ["abiotic-factor"] = "-Mods=",
    };

    public static IReadOnlyList<string>? CurseForgeNamesFor(string gameId) =>
        CurseForgeGameNames.TryGetValue(gameId, out var names) ? names : null;

    public static IReadOnlyList<string>? ModIoNamesFor(string gameId) =>
        ModIoGameNames.TryGetValue(gameId, out var names) ? names : null;

    public static string? ThunderstoreCommunityFor(string gameId) =>
        ThunderstoreCommunities.TryGetValue(gameId, out var slug) ? slug : null;

    public static string? LaunchArgFlagFor(string gameId) =>
        LaunchArgFlags.TryGetValue(gameId, out var flag) ? flag : null;

    public static IReadOnlyList<ModSourceKind> SourcesFor(string gameId)
    {
        var list = new List<ModSourceKind>();
        if (CurseForgeNamesFor(gameId) is not null)
        {
            list.Add(ModSourceKind.CurseForge);
        }
        if (ModIoNamesFor(gameId) is not null)
        {
            list.Add(ModSourceKind.ModIo);
        }
        if (ThunderstoreCommunityFor(gameId) is not null)
        {
            list.Add(ModSourceKind.Thunderstore);
        }
        return list;
    }

    /// <summary>
    /// Adds <paramref name="modId"/> to an existing "<paramref name="flag"/>id;id;id" token in
    /// <paramref name="currentArgs"/> (case-insensitive, semicolon or comma separated, either
    /// separator preserved), or appends a new one if the flag isn't present yet. A no-op if the
    /// ID is already listed.
    /// </summary>
    public static string MergeModIdIntoLaunchArgs(string currentArgs, string flag, string modId)
    {
        currentArgs ??= string.Empty;
        var flagStart = currentArgs.IndexOf(flag, StringComparison.OrdinalIgnoreCase);
        if (flagStart < 0)
        {
            var separator = currentArgs.Length > 0 && !currentArgs.EndsWith(' ') ? " " : string.Empty;
            return $"{currentArgs}{separator}{flag}{modId}";
        }

        var valueStart = flagStart + flag.Length;
        var valueEnd = currentArgs.IndexOf(' ', valueStart);
        if (valueEnd < 0)
        {
            valueEnd = currentArgs.Length;
        }
        var value = currentArgs[valueStart..valueEnd];
        var separatorChar = value.Contains(',') && !value.Contains(';') ? ',' : ';';
        var ids = value.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        if (ids.Contains(modId, StringComparer.OrdinalIgnoreCase))
        {
            return currentArgs;
        }

        ids.Add(modId);
        var newValue = string.Join(separatorChar, ids);
        return currentArgs[..valueStart] + newValue + currentArgs[valueEnd..];
    }

    /// <summary>The mod IDs currently listed in <paramref name="currentArgs"/>'s <paramref
    /// name="flag"/> token, if any - so the UI can show what's already configured even though
    /// there's no file on disk to find (see the type doc comment).</summary>
    public static IReadOnlyList<string> ParseLaunchArgModIds(string? currentArgs, string flag)
    {
        if (string.IsNullOrEmpty(currentArgs))
        {
            return Array.Empty<string>();
        }

        var flagStart = currentArgs.IndexOf(flag, StringComparison.OrdinalIgnoreCase);
        if (flagStart < 0)
        {
            return Array.Empty<string>();
        }

        var valueStart = flagStart + flag.Length;
        var valueEnd = currentArgs.IndexOf(' ', valueStart);
        if (valueEnd < 0)
        {
            valueEnd = currentArgs.Length;
        }

        return currentArgs[valueStart..valueEnd]
            .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }
}
