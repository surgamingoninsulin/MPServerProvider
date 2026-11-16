using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Providers;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Tells whether a newer version of an installed server is available (the Library's purple
/// "update" dot). Only providers that publish a real version list can answer: Minecraft Java
/// (every variant: newest Minecraft version vs. the installed one) and Cfx.re FiveM/RedM
/// (installed build older than the recommended build). SteamCMD, Bedrock, BeamMP and direct
/// downloads always install "latest", so they report no update. Version lists are cached per
/// game + variant for <see cref="CacheFor"/> so opening the Library doesn't hammer the APIs.
/// </summary>
public static partial class ServerUpdateCheck
{
    public static readonly TimeSpan CacheFor = TimeSpan.FromMinutes(30);

    private static readonly ConcurrentDictionary<string, (DateTime At, Task<List<string>> Versions)> Cache = new();

    /// <summary>The newer version the server could be updated to, or null when it is current
    /// (or the provider can't tell). Never throws.</summary>
    public static async Task<string?> NewerVersionAsync(InstallService installer, ServerInstance server, GameDefinition game, CancellationToken ct = default)
    {
        try
        {
            var provider = game.Install.Provider;
            if (provider is not ("minecraft-java" or "cfx"))
            {
                return null;
            }

            var variant = string.IsNullOrWhiteSpace(server.ServerType) ? null : server.ServerType;
            var key = $"{game.Id}|{variant}";
            var entry = Cache.AddOrUpdate(
                key,
                _ => (DateTime.UtcNow, installer.GetVersionsAsync(game, variant, CancellationToken.None)),
                (_, old) => DateTime.UtcNow - old.At < CacheFor && !old.Versions.IsFaulted
                    ? old
                    : (DateTime.UtcNow, installer.GetVersionsAsync(game, variant, CancellationToken.None)));
            var versions = await entry.Versions.WaitAsync(ct).ConfigureAwait(false);

            return provider == "cfx"
                ? NewerCfxBuild(server.ServerVersion, versions)
                : NewerMinecraftVersion(server.ServerVersion, versions);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>"26.2 (build 120)", "1.21.10", "26.3-66.1.8" (Forge), "4.3.1-SNAPSHOT" → the
    /// leading version, compared with the first (newest) entry of the provider's list.</summary>
    internal static string? NewerMinecraftVersion(string? installed, IReadOnlyList<string> versions)
    {
        var newest = versions.FirstOrDefault(v => !v.Equals("latest", StringComparison.OrdinalIgnoreCase));
        var current = LeadingVersion(installed);
        if (newest is null || current is null)
        {
            return null;
        }

        var newestCore = LeadingVersion(newest) ?? newest;
        return MinecraftJavaProvider.CompareVersions(newestCore, current) > 0 ? newest : null;
    }

    /// <summary>"build 38435" installed vs "build 38621 (recommended)" → the recommended build
    /// when it is newer (the recommended channel, not "latest", is what servers should run).</summary>
    internal static string? NewerCfxBuild(string? installed, IReadOnlyList<string> versions)
    {
        var current = FirstNumber(installed);
        var recommended = versions.FirstOrDefault(v => v.Contains("recommended", StringComparison.OrdinalIgnoreCase));
        var target = FirstNumber(recommended);
        return current is { } c && target is { } t && t > c ? $"build {t}" : null;
    }

    private static string? LeadingVersion(string? text)
    {
        var match = LeadingVersionRegex().Match(text ?? string.Empty);
        return match.Success ? match.Value : null;
    }

    private static long? FirstNumber(string? text)
    {
        var match = Regex.Match(text ?? string.Empty, @"\d+");
        return match.Success && long.TryParse(match.Value, out var n) ? n : null;
    }

    [GeneratedRegex(@"^\s*\d+(\.\d+)+")]
    private static partial Regex LeadingVersionRegex();
}
