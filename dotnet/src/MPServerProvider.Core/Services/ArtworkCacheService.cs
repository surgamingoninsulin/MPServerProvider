using System.Text.Json;

namespace MPServerProvider.Core.Services;

public sealed class ArtworkCacheService
{
    public const string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0 Safari/537.36";

    public static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(15);

    public const int MaxConcurrentDownloads = 4;

    private readonly HttpClient _http;
    private readonly SemaphoreSlim _gate;

    public ArtworkCacheService()
    {
        _http = CreateArtworkClient();
        _gate = new SemaphoreSlim(MaxConcurrentDownloads);
    }

    private static HttpClient CreateArtworkClient()
    {
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
            AutomaticDecompression = System.Net.DecompressionMethods.All,
        };
        var client = new HttpClient(handler) { Timeout = HttpTimeout };
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        return client;
    }

    public interface ICachePaths
    {
        string ArtworkCacheDir { get; }
    }

    public async Task<string> GetLocalPathAsync(ICachePaths paths, string key, string url, CancellationToken ct = default)
    {
        var dir = paths.ArtworkCacheDir;
        Directory.CreateDirectory(dir);

        var extension = GuessExtension(url);
        var localPath = Path.Combine(dir, $"{SanitizeCacheKey(key)}.{extension}");

        if (File.Exists(localPath))
        {
            return localPath;
        }

        await _gate.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            // Re-check after acquiring the gate in case a concurrent caller just finished the
            // same download.
            if (File.Exists(localPath))
            {
                return localPath;
            }

            using var response = await _http.GetAsync(url, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            // Only ever cache real images: an HTML error/captcha page served with 200 used to be
            // written to disk and then "cached" forever.
            if (!IsImageContentType(response.Content.Headers.ContentType?.MediaType))
            {
                throw new InvalidDataException(
                    $"Artwork URL returned '{response.Content.Headers.ContentType?.MediaType ?? "no content type"}', not an image.");
            }

            if (response.Content.Headers.ContentLength is > MaxImageBytes)
            {
                throw new InvalidDataException("Artwork image is unexpectedly large.");
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
            if (bytes.Length == 0 || bytes.Length > MaxImageBytes)
            {
                throw new InvalidDataException("Artwork image is empty or unexpectedly large.");
            }

            // Write-then-rename so a cancelled/failed write never leaves a truncated image that
            // File.Exists would treat as cached.
            var tmp = localPath + ".part";
            await File.WriteAllBytesAsync(tmp, bytes, ct).ConfigureAwait(false);
            File.Move(tmp, localPath, overwrite: true);
            return localPath;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task PrefetchArtworkAsync(ICachePaths paths, IEnumerable<int> steamAppIds, CancellationToken ct = default)
    {
        foreach (var appId in steamAppIds)
        {
            try
            {
                var url = await FetchSteamHeaderUrlAsync(appId, ct).ConfigureAwait(false);
                if (url is not null)
                {
                    await GetLocalPathAsync(paths, appId.ToString(), url, ct).ConfigureAwait(false);
                }
            }
            catch
            {
                // Best-effort: one app id's failure must not abort the whole prefetch batch.
            }
        }
    }

    private async Task<string?> FetchSteamHeaderUrlAsync(int appId, CancellationToken ct)
    {
        var url = $"https://store.steampowered.com/api/appdetails?appids={appId}";
        using var response = await _http.GetAsync(url, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct).ConfigureAwait(false);

        if (!document.RootElement.TryGetProperty(appId.ToString(), out var entry))
        {
            return null;
        }
        if (!entry.TryGetProperty("data", out var data))
        {
            return null;
        }
        if (!data.TryGetProperty("header_image", out var headerImage))
        {
            return null;
        }
        return headerImage.GetString();
    }

    public async Task<string?> TryFillFromStoreAsync(ICachePaths paths, int appId, CancellationToken ct = default)
    {
        var url = await FetchSteamHeaderUrlAsync(appId, ct).ConfigureAwait(false);
        if (url is null)
        {
            return null;
        }
        return await GetLocalPathAsync(paths, appId.ToString(), url, ct).ConfigureAwait(false);
    }

    public static string SanitizeCacheKey(string key)
    {
        var chars = key.Select(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_').ToArray();
        return new string(chars);
    }

    public const int MaxImageBytes = 20 * 1024 * 1024;

    internal static bool IsImageContentType(string? mediaType)
        => mediaType is not null && mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

    private static string GuessExtension(string url)
    {
        var lower = url.ToLowerInvariant();
        if (lower.EndsWith(".png", StringComparison.Ordinal))
        {
            return "png";
        }
        if (lower.EndsWith(".webp", StringComparison.Ordinal))
        {
            return "webp";
        }
        return "jpg";
    }
}
