using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services.Mods;

/// <summary>
/// Searches and downloads mods from the mod.io v1 API (https://api.mod.io/v1). Requires a free
/// read-only API key from https://mod.io/me/access, supplied by the caller (Settings ->
/// ModIoApiKey) since this service holds no credentials of its own.
///
/// For most games mapped to mod.io here (see <see cref="GameModSupport.LaunchArgFlagFor"/>),
/// "installing" never calls <see cref="DownloadAsync"/> at all - <see cref="ModInstallService"/>
/// adds the mod's ID straight to the server's launch arguments instead, and the dedicated server
/// downloads the mod itself on its next start. Search/file listing still goes through here either
/// way, since both styles of game need to let the user find the mod in the first place.
/// </summary>
public sealed class ModIoModSource : IModSource
{
    // mod.io's old shared domain is heavily restricted since January 2025; every account now has
    // its own host (shown as "API path" on https://mod.io/me/access, e.g.
    // https://u-26716762.modapi.io/v1). This is only a last-resort fallback for when that hasn't
    // been set in Settings yet.
    private const string FallbackBaseUrl = "https://api.mod.io/v1";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _http;
    private readonly Func<string?> _apiKey;
    private readonly Func<string?> _apiHost;
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, int> _gameIdCache = new(StringComparer.OrdinalIgnoreCase);

    public ModIoModSource(HttpClient http, Func<string?> apiKey, Func<string?> apiHost)
    {
        _http = http;
        _apiKey = apiKey;
        _apiHost = apiHost;
    }

    private string BaseUrl
    {
        get
        {
            var host = _apiHost()?.Trim().TrimEnd('/');
            return string.IsNullOrEmpty(host) ? FallbackBaseUrl : host;
        }
    }

    public ModSourceKind Kind => ModSourceKind.ModIo;

    public bool SupportsGame(string gameId) => GameModSupport.ModIoNamesFor(gameId) is not null;

    private const int SearchPageSize = 25;

    public async Task<IReadOnlyList<ModListing>> SearchAsync(string gameId, string query, int page, CancellationToken ct)
    {
        var modIoGameId = await ResolveGameIdAsync(gameId, ct).ConfigureAwait(false);
        if (modIoGameId is null)
        {
            return Array.Empty<ModListing>();
        }

        var qs = query.Length > 0 ? $"&_q={Uri.EscapeDataString(query)}" : string.Empty;
        var payload = await GetJsonAsync<ModsResponse>(
            $"/games/{modIoGameId}/mods?_limit={SearchPageSize}&_offset={page * SearchPageSize}&_sort=-downloads_total{qs}", ct).ConfigureAwait(false);
        return payload?.Data?.Select(ToListing).ToList() ?? new List<ModListing>();
    }

    public async Task<IReadOnlyList<ModFileOption>> GetFilesAsync(string gameId, string modId, CancellationToken ct)
    {
        var modIoGameId = await ResolveGameIdAsync(gameId, ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("mod.io isn't available for this game.");
        var payload = await GetJsonAsync<FilesResponse>($"/games/{modIoGameId}/mods/{modId}/files?_limit=50&_sort=-date_added", ct).ConfigureAwait(false);
        return payload?.Data?
            .Select(f => new ModFileOption(f.Id.ToString(), f.Filename ?? $"File {f.Id}", null, UnixSeconds(f.DateAdded), f.Filesize))
            .ToList() ?? new List<ModFileOption>();
    }

    public async Task<IReadOnlyList<string>> DownloadAsync(string gameId, string modId, string fileId, string targetDir, CancellationToken ct)
    {
        var modIoGameId = await ResolveGameIdAsync(gameId, ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("mod.io isn't available for this game.");
        var payload = await GetJsonAsync<FileResponse>($"/games/{modIoGameId}/mods/{modId}/files/{fileId}", ct).ConfigureAwait(false);
        var file = payload ?? throw new InvalidOperationException("mod.io didn't return that file.");
        var url = file.Download?.BinaryUrl;
        if (string.IsNullOrEmpty(url))
        {
            throw new InvalidOperationException($"mod.io has no download link for '{file.Filename}'.");
        }

        Directory.CreateDirectory(targetDir);
        var fileName = SanitizeFileName(file.Filename ?? $"{modId}-{fileId}.zip");
        var destination = Path.Combine(targetDir, fileName);
        using var download = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        download.EnsureSuccessStatusCode();
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

        var candidates = GameModSupport.ModIoNamesFor(gameId);
        if (candidates is null)
        {
            return null;
        }

        const int pageSize = 100;
        const int maxPages = 20; // 2000 games.
        for (var page = 0; page < maxPages; page++)
        {
            var payload = await GetJsonAsync<GamesResponse>($"/games?_limit={pageSize}&_offset={page * pageSize}", ct).ConfigureAwait(false);
            var games = payload?.Data ?? new List<GameSummary>();
            var match = games.FirstOrDefault(g =>
                candidates.Any(c => string.Equals(g.Name, c, StringComparison.OrdinalIgnoreCase)
                                     || string.Equals(g.NameId, c, StringComparison.OrdinalIgnoreCase)));
            if (match is not null)
            {
                _gameIdCache[gameId] = match.Id;
                return match.Id;
            }

            if (games.Count < pageSize)
            {
                break;
            }
        }

        return null;
    }

    private async Task<T?> GetJsonAsync<T>(string path, CancellationToken ct)
    {
        var key = _apiKey();
        var separator = path.Contains('?') ? "&" : "?";
        var url = BaseUrl + path + (string.IsNullOrWhiteSpace(key) ? string.Empty : $"{separator}api_key={Uri.EscapeDataString(key)}");

        using var response = await _http.GetAsync(url, ct).ConfigureAwait(false);
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            throw new InvalidOperationException("mod.io rejected the request - check the API key in Settings.");
        }
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    private static ModListing ToListing(ModSummary m) => new(
        ModSourceKind.ModIo,
        m.Id.ToString(),
        m.Name ?? $"Mod {m.Id}",
        m.SubmittedBy?.Username,
        m.Summary,
        m.Logo?.Thumb320X180,
        m.ProfileUrl,
        m.Stats?.DownloadsTotal);

    private static DateTimeOffset? UnixSeconds(long? seconds) =>
        seconds is { } s and > 0 ? DateTimeOffset.FromUnixTimeSeconds(s) : null;

    private static string SanitizeFileName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }

    // -- Wire format (only the fields this service uses; mod.io uses snake_case) --------------

    private sealed class GamesResponse { public List<GameSummary>? Data { get; set; } }

    private sealed class GameSummary
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NameId { get; set; }
    }

    private sealed class ModsResponse { public List<ModSummary>? Data { get; set; } }

    private sealed class ModSummary
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Summary { get; set; }
        public string? ProfileUrl { get; set; }
        public ModLogo? Logo { get; set; }
        public ModSubmitter? SubmittedBy { get; set; }
        public ModStats? Stats { get; set; }
    }

    private sealed class ModLogo
    {
        // mod.io's actual field is "thumb_320x180" - the auto snake_case policy mangles the
        // embedded "320X180" (digits next to an uppercase letter) into something else entirely,
        // so this needs to be spelled out explicitly rather than left to convention.
        [JsonPropertyName("thumb_320x180")]
        public string? Thumb320X180 { get; set; }
    }

    private sealed class ModSubmitter { public string? Username { get; set; } }

    private sealed class ModStats { public long DownloadsTotal { get; set; } }

    private sealed class FilesResponse { public List<ModFile>? Data { get; set; } }

    private sealed class FileResponse : ModFile;

    private class ModFile
    {
        public int Id { get; set; }
        public string? Filename { get; set; }
        public long? DateAdded { get; set; }
        public long Filesize { get; set; }
        public ModFileDownload? Download { get; set; }
    }

    private sealed class ModFileDownload { public string? BinaryUrl { get; set; } }
}
