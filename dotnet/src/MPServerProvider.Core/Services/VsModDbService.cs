using System.Net.Http.Json;
using System.Text.Json;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Browses and downloads Vintage Story mods from mods.vintagestory.at's public API (no login
/// needed). https://mods.vintagestory.at/api/mods and .../api/mod/{modidstr}.
/// </summary>
public sealed class VsModDbService
{
    private const string BaseUrl = "https://mods.vintagestory.at/api";

    private static readonly HttpClient Http = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MPServerProvider/1.0 (+https://github.com/surgamingoninsulin/MPServerProvider)");
        return client;
    }

    public async Task<List<VsModDbProject>> SearchAsync(string? query, int limit = 30, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/mods?text={Uri.EscapeDataString(query ?? string.Empty)}";
        using var doc = await Http.GetFromJsonAsync<JsonDocument>(url, ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("mods.vintagestory.at returned an empty response.");

        return ParseSearchResults(doc.RootElement, limit);
    }

    internal static List<VsModDbProject> ParseSearchResults(JsonElement root, int limit)
    {
        var results = new List<VsModDbProject>();
        if (!root.TryGetProperty("mods", out var mods) || mods.ValueKind != JsonValueKind.Array)
        {
            return results;
        }

        foreach (var mod in mods.EnumerateArray())
        {
            // Client-only mods (textures, HUDs, shaders) have nothing to install on a server.
            var side = mod.TryGetProperty("side", out var sideEl) ? sideEl.GetString() : null;
            if (string.Equals(side, "client", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var modIdStr = mod.TryGetProperty("modidstrs", out var idsEl) && idsEl.ValueKind == JsonValueKind.Array
                ? idsEl.EnumerateArray().Select(e => e.GetString()).FirstOrDefault(s => !string.IsNullOrEmpty(s))
                : null;
            if (modIdStr is null)
            {
                continue;
            }

            results.Add(new VsModDbProject(
                ModIdStr: modIdStr,
                Title: mod.TryGetProperty("name", out var nameEl) ? nameEl.GetString() ?? modIdStr : modIdStr,
                Summary: mod.TryGetProperty("summary", out var summaryEl) ? summaryEl.GetString() ?? string.Empty : string.Empty,
                LogoUrl: mod.TryGetProperty("logo", out var logoEl) ? logoEl.GetString() : null,
                Downloads: mod.TryGetProperty("downloads", out var dlEl) && dlEl.TryGetInt64(out var dl) ? dl : 0));

            if (results.Count >= limit)
            {
                break;
            }
        }

        return results;
    }

    /// <summary>The newest release whose tags list this game version, or the newest release at all if gameVersion is unknown.</summary>
    public async Task<VsModDbRelease?> GetBestReleaseAsync(string modIdStr, string? gameVersion, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/mod/{Uri.EscapeDataString(modIdStr)}";
        using var doc = await Http.GetFromJsonAsync<JsonDocument>(url, ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("mods.vintagestory.at returned an empty response.");

        return PickRelease(doc.RootElement, gameVersion);
    }

    internal static VsModDbRelease? PickRelease(JsonElement root, string? gameVersion)
    {
        if (!root.TryGetProperty("mod", out var mod) || !mod.TryGetProperty("releases", out var releases)
            || releases.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        VsModDbRelease? fallback = null;
        foreach (var release in releases.EnumerateArray())
        {
            var gameVersions = release.TryGetProperty("tags", out var tagsEl) && tagsEl.ValueKind == JsonValueKind.Array
                ? tagsEl.EnumerateArray().Select(t => t.GetString() ?? string.Empty).Where(t => t.Length > 0).ToList()
                : new List<string>();
            var parsed = new VsModDbRelease(
                Filename: release.TryGetProperty("filename", out var fnEl) ? fnEl.GetString() ?? "mod.zip" : "mod.zip",
                Url: release.TryGetProperty("mainfile", out var urlEl) ? urlEl.GetString() ?? throw new InvalidOperationException("Release has no mainfile url.") : throw new InvalidOperationException("Release has no mainfile url."),
                ModVersion: release.TryGetProperty("modversion", out var vEl) ? vEl.GetString() ?? string.Empty : string.Empty,
                GameVersions: gameVersions);

            fallback ??= parsed; // releases are newest first
            if (string.IsNullOrWhiteSpace(gameVersion) || gameVersions.Contains(gameVersion, StringComparer.OrdinalIgnoreCase))
            {
                return parsed;
            }
        }

        return fallback;
    }

    public async Task<byte[]> DownloadAsync(VsModDbRelease release, CancellationToken ct = default)
        => await Http.GetByteArrayAsync(release.Url, ct).ConfigureAwait(false);
}
