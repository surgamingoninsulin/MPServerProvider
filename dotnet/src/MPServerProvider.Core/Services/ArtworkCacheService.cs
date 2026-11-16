using System.Text.Json;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.ArtworkCacheService</c> (decompile token
/// 0x02000005, see <c>docs/decompile-report/report/MPServerProvider.Core.dll/
/// MPServerProvider.Core.Services/0x02000005_ArtworkCacheService.md</c>).
///
/// <para>
/// Decompile evidence: a static <c>HttpClient</c> (<c>ArtworkHttp</c>) built via
/// <c>CreateArtworkClient()</c> with a browser-spoofing User-Agent string (recovered verbatim,
/// see <see cref="UserAgent"/>), auto-redirect enabled, automatic decompression, and a timeout
/// set via <c>TimeSpan.FromSeconds(...)</c> (exact literal not recoverable from metadata alone —
/// INFERRED 15s, a conventional value for a small image download). A gate caps concurrent
/// downloads at <see cref="MaxConcurrentDownloads"/> (confirmed by the <c>.ctor</c>'s IL:
/// <c>ldc.i4.4</c> immediately before the <c>SemaphoreSlim..ctor</c> call).
/// </para>
/// <para>
/// <c>LoadCoverAsync</c>/<c>PrefetchArtworkAsync</c>/<c>TryFillFromStoreAsync</c>/
/// <c>OnFetchSteamInfo</c> aren't directly evidenced as members of this type in the decompile
/// report (only <c>.ctor</c>, <c>CreateArtworkClient</c>, <c>GetLocalPathAsync</c> are), matching
/// the already-verified Rust port's same finding — they likely live on a cooperating type not
/// covered by this report's token range, or were inlined into <c>GetLocalPathAsync</c>'s state
/// machine. This port implements the one confirmed public surface
/// (<see cref="GetLocalPathAsync"/>) plus the task-evidenced convenience wrappers backed by the
/// public, unauthenticated Steam store <c>appdetails</c> API for resolving a Steam App ID's
/// header/cover image — the obvious artwork source given this app's SteamCMD-provider-heavy
/// game catalog.
/// </para>
/// </summary>
public sealed class ArtworkCacheService
{
    /// <summary>Browser-spoofing User-Agent, matching the decompiled literal exactly.</summary>
    public const string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0 Safari/537.36";

    /// <summary>
    /// HTTP timeout for artwork fetches. INFERRED: the original's <c>.cctor</c> sets this via
    /// <c>TimeSpan.FromSeconds(&lt;literal not recoverable from metadata&gt;)</c>; 15s is a
    /// conventional value for a small image download.
    /// </summary>
    public static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Max concurrent downloads, matching the <c>SemaphoreSlim(4)</c> constructor argument
    /// evidenced in the <c>.ctor</c>'s IL.
    /// </summary>
    public const int MaxConcurrentDownloads = 4;

    private readonly HttpClient _http;
    private readonly SemaphoreSlim _gate;

    public ArtworkCacheService()
    {
        _http = CreateArtworkClient();
        _gate = new SemaphoreSlim(MaxConcurrentDownloads);
    }

    /// <summary>Port of <c>ArtworkCacheService.CreateArtworkClient()</c>.</summary>
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

    /// <summary>
    /// Resolves the on-disk cache directory for artwork files (e.g. <c>data/cache/artwork</c>).
    /// // TODO: reconcile with Services.AppPaths once that lands — this trait-shaped interface
    /// exists only so this file isn't blocked on that file's commit landing; once it does,
    /// callers should pass <c>AppPaths.ArtworkCacheDir</c>-equivalent directly instead.
    /// </summary>
    public interface ICachePaths
    {
        string ArtworkCacheDir { get; }
    }

    /// <summary>
    /// Port of <c>ArtworkCacheService.GetLocalPathAsync(string key, CancellationToken)</c>.
    /// <paramref name="key"/> is a cache-safe identifier (e.g. a Steam App ID or game definition
    /// id); <paramref name="url"/> is the remote image to fetch if the cache is cold. Returns the
    /// local file path once the image is present on disk (downloading it first if necessary),
    /// gated by <see cref="MaxConcurrentDownloads"/> concurrent fetches.
    /// </summary>
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

    /// <summary>
    /// Port of the task-evidenced <c>PrefetchArtworkAsync</c>: resolves each Steam App ID's
    /// header image via the Steam store API and warms the disk cache for it, ignoring (but not
    /// throwing on) failures for any individual app id — a best-effort prefetch.
    /// </summary>
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

    /// <summary>
    /// Port of the task-evidenced <c>OnFetchSteamInfo</c>: queries the public, unauthenticated
    /// Steam store <c>appdetails</c> endpoint for <paramref name="appId"/> and extracts its
    /// <c>header_image</c> URL.
    /// </summary>
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

    /// <summary>
    /// Port of the task-evidenced <c>TryFillFromStoreAsync</c>: like
    /// <see cref="FetchSteamHeaderUrlAsync"/> + <see cref="GetLocalPathAsync"/> combined, but
    /// returns <c>null</c> instead of throwing when the store simply has no artwork for this app
    /// id (distinct from a hard network/IO failure, which still throws).
    /// </summary>
    public async Task<string?> TryFillFromStoreAsync(ICachePaths paths, int appId, CancellationToken ct = default)
    {
        var url = await FetchSteamHeaderUrlAsync(appId, ct).ConfigureAwait(false);
        if (url is null)
        {
            return null;
        }
        return await GetLocalPathAsync(paths, appId.ToString(), url, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Port of the cache-key sanitization <c>GetLocalPathAsync</c> applies before composing the
    /// on-disk file name (<c>{SanitizeCacheKey(key)}.{ext}</c>). Public so callers that pre-seed
    /// the cache can compute the exact file name a given key resolves to.
    /// </summary>
    public static string SanitizeCacheKey(string key)
    {
        var chars = key.Select(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_').ToArray();
        return new string(chars);
    }

    /// <summary>Upper bound for one cached artwork image.</summary>
    public const int MaxImageBytes = 20 * 1024 * 1024;

    /// <summary><c>true</c> only for <c>image/*</c> media types.</summary>
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
