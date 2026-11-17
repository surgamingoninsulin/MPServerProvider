using System.Net.Http.Json;
using System.Text.Json;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Browses and downloads Minecraft mods/plugins from Modrinth's public API (no login needed).
/// https://docs.modrinth.com/api/operations/searchprojects/ and .../getversions/
/// </summary>
public sealed class ModrinthService
{
    private const string BaseUrl = "https://api.modrinth.com/v2";

    private static readonly HttpClient Http = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MPServerProvider/1.0 (+https://github.com/surgamingoninsulin/MPServerProvider)");
        return client;
    }

    internal static string BuildFacets(IReadOnlyList<string> loaders, string? gameVersion)
    {
        var groups = new List<string> { "[\"project_type:mod\"]" };
        if (loaders.Count > 0)
        {
            groups.Add("[" + string.Join(",", loaders.Select(l => JsonSerializer.Serialize($"categories:{l}"))) + "]");
        }
        if (!string.IsNullOrWhiteSpace(gameVersion))
        {
            groups.Add("[" + JsonSerializer.Serialize($"versions:{gameVersion}") + "]");
        }

        return "[" + string.Join(",", groups) + "]";
    }

    public async Task<List<ModrinthProject>> SearchAsync(
        string? query, IReadOnlyList<string> loaders, string? gameVersion, int offset = 0, int limit = 20, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/search?query={Uri.EscapeDataString(query ?? string.Empty)}" +
                   $"&limit={limit}&offset={offset}&index=relevance&facets={Uri.EscapeDataString(BuildFacets(loaders, gameVersion))}";

        using var doc = await Http.GetFromJsonAsync<JsonDocument>(url, ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Modrinth returned an empty response.");

        return ParseSearchResults(doc.RootElement);
    }

    internal static List<ModrinthProject> ParseSearchResults(JsonElement root)
    {
        var results = new List<ModrinthProject>();
        if (!root.TryGetProperty("hits", out var hits))
        {
            return results;
        }

        foreach (var hit in hits.EnumerateArray())
        {
            results.Add(new ModrinthProject(
                ProjectId: hit.GetProperty("project_id").GetString() ?? string.Empty,
                Slug: hit.TryGetProperty("slug", out var slugEl) ? slugEl.GetString() ?? string.Empty : string.Empty,
                Title: hit.TryGetProperty("title", out var titleEl) ? titleEl.GetString() ?? string.Empty : string.Empty,
                Description: hit.TryGetProperty("description", out var descEl) ? descEl.GetString() ?? string.Empty : string.Empty,
                IconUrl: hit.TryGetProperty("icon_url", out var iconEl) ? iconEl.GetString() : null,
                Downloads: hit.TryGetProperty("downloads", out var dlEl) && dlEl.TryGetInt64(out var dl) ? dl : 0));
        }

        return results;
    }

    /// <summary>The newest version file matching this server's loaders (and game version, if known).</summary>
    public async Task<ModrinthVersionFile?> GetBestVersionAsync(
        string projectIdOrSlug, IReadOnlyList<string> loaders, string? gameVersion, CancellationToken ct = default)
    {
        var loadersJson = "[" + string.Join(",", loaders.Select(l => JsonSerializer.Serialize(l))) + "]";
        var url = $"{BaseUrl}/project/{Uri.EscapeDataString(projectIdOrSlug)}/version?loaders={Uri.EscapeDataString(loadersJson)}";
        if (!string.IsNullOrWhiteSpace(gameVersion))
        {
            url += $"&game_versions={Uri.EscapeDataString("[" + JsonSerializer.Serialize(gameVersion) + "]")}";
        }

        using var doc = await Http.GetFromJsonAsync<JsonDocument>(url, ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Modrinth returned an empty response.");

        return PickVersionFile(doc.RootElement);
    }

    internal static ModrinthVersionFile? PickVersionFile(JsonElement versions)
    {
        foreach (var version in versions.EnumerateArray())
        {
            if (!version.TryGetProperty("files", out var files) || files.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            var file = files.EnumerateArray().FirstOrDefault(f => f.TryGetProperty("primary", out var p) && p.ValueKind == JsonValueKind.True)
                is { ValueKind: JsonValueKind.Object } primary
                ? primary
                : files.EnumerateArray().FirstOrDefault();
            if (file.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            return new ModrinthVersionFile(
                VersionId: version.TryGetProperty("id", out var idEl) ? idEl.GetString() ?? string.Empty : string.Empty,
                VersionNumber: version.TryGetProperty("version_number", out var vnEl) ? vnEl.GetString() ?? string.Empty : string.Empty,
                Filename: file.TryGetProperty("filename", out var fnEl) ? fnEl.GetString() ?? "mod.jar" : "mod.jar",
                Url: file.GetProperty("url").GetString() ?? throw new InvalidOperationException("Modrinth file has no url."),
                Sha1: file.TryGetProperty("hashes", out var hashesEl) && hashesEl.TryGetProperty("sha1", out var sha1El) ? sha1El.GetString() : null);
        }

        return null;
    }

    public async Task<byte[]> DownloadAsync(ModrinthVersionFile file, CancellationToken ct = default)
        => await Http.GetByteArrayAsync(file.Url, ct).ConfigureAwait(false);
}
