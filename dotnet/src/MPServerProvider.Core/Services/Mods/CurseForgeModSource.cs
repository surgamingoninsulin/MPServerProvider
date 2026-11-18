using System.Net;
using System.Text.Json;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services.Mods;

/// <summary>
/// Searches and downloads mods from the CurseForge Core API (https://api.curseforge.com).
/// Requires a free developer API key from https://console.curseforge.com/, supplied by the
/// caller (Settings -> CurseForgeApiKey) since this service holds no credentials of its own.
/// </summary>
public sealed class CurseForgeModSource : IModSource
{
    private const string BaseUrl = "https://api.curseforge.com";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _http;
    private readonly Func<string?> _apiKey;
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, int> _gameIdCache = new(StringComparer.OrdinalIgnoreCase);

    public CurseForgeModSource(HttpClient http, Func<string?> apiKey)
    {
        _http = http;
        _apiKey = apiKey;
    }

    public ModSourceKind Kind => ModSourceKind.CurseForge;

    public bool SupportsGame(string gameId) => GameModSupport.CurseForgeNamesFor(gameId) is not null;

    private const int SearchPageSize = 25;

    public async Task<IReadOnlyList<ModListing>> SearchAsync(string gameId, string query, int page, CancellationToken ct)
    {
        var curseForgeGameId = await ResolveGameIdAsync(gameId, ct).ConfigureAwait(false);
        if (curseForgeGameId is null)
        {
            return Array.Empty<ModListing>();
        }

        var payload = await GetJsonAsync<SearchResponse>(
            $"/v1/mods/search?gameId={curseForgeGameId}&searchFilter={Uri.EscapeDataString(query)}&index={page * SearchPageSize}&pageSize={SearchPageSize}&sortField=2&sortOrder=desc",
            ct).ConfigureAwait(false);
        return payload?.Data?.Select(ToListing).ToList() ?? new List<ModListing>();
    }

    public async Task<IReadOnlyList<ModFileOption>> GetFilesAsync(string gameId, string modId, CancellationToken ct)
    {
        var payload = await GetJsonAsync<FilesResponse>($"/v1/mods/{modId}/files?pageSize=50", ct).ConfigureAwait(false);
        return payload?.Data?
            .OrderByDescending(f => f.FileDate)
            .Select(f => new ModFileOption(
                f.Id.ToString(),
                f.DisplayName ?? f.FileName ?? $"File {f.Id}",
                f.GameVersions?.FirstOrDefault(),
                f.FileDate,
                f.FileLength))
            .ToList() ?? new List<ModFileOption>();
    }

    public async Task<IReadOnlyList<string>> DownloadAsync(string gameId, string modId, string fileId, string targetDir, CancellationToken ct)
    {
        var payload = await GetJsonAsync<FileResponse>($"/v1/mods/{modId}/files/{fileId}", ct).ConfigureAwait(false);
        var file = payload?.Data ?? throw new InvalidOperationException("CurseForge didn't return that file.");
        if (string.IsNullOrEmpty(file.DownloadUrl))
        {
            throw new InvalidOperationException(
                $"'{file.DisplayName ?? file.FileName}' can't be downloaded through the API - the mod's author has disabled third-party distribution. Download it from the CurseForge website instead.");
        }

        Directory.CreateDirectory(targetDir);
        var fileName = SanitizeFileName(file.FileName ?? $"{modId}-{fileId}.jar");
        var destination = Path.Combine(targetDir, fileName);
        using var download = await _http.GetAsync(file.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        EnsureSuccess(download);
        await using var stream = await download.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        await using var target = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(target, ct).ConfigureAwait(false);
        return new[] { destination };
    }

    private async Task<int?> ResolveGameIdAsync(string gameId, CancellationToken ct)
    {
        if (_gameIdCache.TryGetValue(gameId, out var cached))
        {
            return cached;
        }

        var candidates = GameModSupport.CurseForgeNamesFor(gameId);
        if (candidates is null)
        {
            return null;
        }

        // CurseForge caps pageSize at 50 regardless of what's asked for, and there are now
        // thousands of games, so a newer addition (e.g. Hytale) can sit well past the first page.
        // Page through until a match turns up or the list is exhausted, instead of only ever
        // looking at the first page and silently finding nothing.
        const int pageSize = 50;
        const int maxPages = 40; // 2000 games - generous headroom over CurseForge's current count.
        for (var page = 0; page < maxPages; page++)
        {
            var payload = await GetJsonAsync<GamesResponse>($"/v1/games?index={page * pageSize}&pageSize={pageSize}", ct).ConfigureAwait(false);
            var games = payload?.Data ?? new List<GameSummary>();
            var match = games.FirstOrDefault(g =>
                candidates.Any(c => string.Equals(g.Name, c, StringComparison.OrdinalIgnoreCase)
                                     || string.Equals(g.Slug, c, StringComparison.OrdinalIgnoreCase)));
            if (match is not null)
            {
                _gameIdCache[gameId] = match.Id;
                return match.Id;
            }

            if (games.Count < pageSize)
            {
                break; // Last page.
            }
        }

        return null;
    }

    private async Task<T?> GetJsonAsync<T>(string path, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + path);
        var key = _apiKey();
        if (!string.IsNullOrWhiteSpace(key))
        {
            request.Headers.TryAddWithoutValidation("x-api-key", key);
        }

        using var response = await _http.SendAsync(request, ct).ConfigureAwait(false);
        EnsureSuccess(response);
        var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    private static void EnsureSuccess(HttpResponseMessage response)
    {
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            throw new InvalidOperationException("CurseForge rejected the request - check the API key in Settings.");
        }
        response.EnsureSuccessStatusCode();
    }

    private static ModListing ToListing(ModSummary m) => new(
        ModSourceKind.CurseForge,
        m.Id.ToString(),
        m.Name ?? $"Mod {m.Id}",
        m.Authors?.FirstOrDefault()?.Name,
        m.Summary,
        m.Logo?.ThumbnailUrl,
        m.Links?.WebsiteUrl,
        m.DownloadCount);

    private static string SanitizeFileName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }

    // -- Wire format (only the fields this service uses) --------------------------------------

    private sealed class GamesResponse { public List<GameSummary>? Data { get; set; } }

    private sealed class GameSummary
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
    }

    private sealed class SearchResponse { public List<ModSummary>? Data { get; set; } }

    private sealed class ModSummary
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Summary { get; set; }
        public long DownloadCount { get; set; }
        public ModLogo? Logo { get; set; }
        public ModLinks? Links { get; set; }
        public List<ModAuthor>? Authors { get; set; }
    }

    private sealed class ModLogo { public string? ThumbnailUrl { get; set; } }

    private sealed class ModLinks { public string? WebsiteUrl { get; set; } }

    private sealed class ModAuthor { public string? Name { get; set; } }

    private sealed class FilesResponse { public List<ModFile>? Data { get; set; } }

    private sealed class FileResponse { public ModFile? Data { get; set; } }

    private sealed class ModFile
    {
        public int Id { get; set; }
        public string? DisplayName { get; set; }
        public string? FileName { get; set; }
        public DateTimeOffset? FileDate { get; set; }
        public long FileLength { get; set; }
        public string? DownloadUrl { get; set; }
        public List<string>? GameVersions { get; set; }
    }
}
