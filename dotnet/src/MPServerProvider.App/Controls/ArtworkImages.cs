using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Controls;

internal static class ArtworkImages
{
    public static string StableKey(string prefix, string url)
        => $"{prefix}_{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(url)))[..16]}";

    public static int PixelWidth(Visual visual, double dipWidth)
    {
        double scale = 1.0;
        try
        {
            scale = VisualTreeHelper.GetDpi(visual).DpiScaleX;
        }
        catch
        {
            // Not yet connected to a presentation source; assume 96 DPI.
        }

        return Math.Max(1, (int)Math.Ceiling(dipWidth * Math.Max(1.0, scale)));
    }

    public const string AssetScheme = "asset:";

    public static string? BundledAssetPath(string? url)
    {
        if (url is null || !url.StartsWith(AssetScheme, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var name = url[AssetScheme.Length..].Trim();
        if (name.Length == 0 || name != Path.GetFileName(name))
        {
            return null;
        }

        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "Artwork", name);
        return File.Exists(path) ? path : null;
    }

    public static async Task<BitmapSource?> LoadAsync(AppServices services, string cacheKey, string? url, int decodePixelWidth, CancellationToken ct = default)
    {
        if (BundledAssetPath(url) is { } assetPath)
        {
            try
            {
                return await Task.Run(() => Decode(assetPath, decodePixelWidth, deleteOnFailure: false), ct).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        if (!TryGetWebUri(url, out _))
        {
            return null;
        }

        try
        {
            var adapter = new CachePaths(services.Paths);
            var path = await services.Artwork.GetLocalPathAsync(adapter, cacheKey, url!, ct).ConfigureAwait(false);
            return await Task.Run(() => Decode(path, decodePixelWidth), ct).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }

    private static BitmapSource? Decode(string path, int decodePixelWidth, bool deleteOnFailure = true)
    {
        try
        {
            using var stream = File.OpenRead(path);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            if (decodePixelWidth > 0)
            {
                bitmap.DecodePixelWidth = decodePixelWidth;
            }
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        catch
        {
            // A cached file that won't decode (e.g. an HTML error page saved as .jpg) would
            // otherwise poison the cache forever; drop it so the next attempt re-downloads.
            // (Never for bundled assets, which aren't cache files.)
            try
            {
                if (deleteOnFailure)
                {
                    File.Delete(path);
                }
            }
            catch
            {
                // Best-effort.
            }

            return null;
        }
    }

    public static bool TryGetWebUri(string? url, out Uri uri)
    {
        if (!string.IsNullOrWhiteSpace(url)
            && Uri.TryCreate(url.Trim(), UriKind.Absolute, out var parsed)
            && (parsed.Scheme == Uri.UriSchemeHttps || parsed.Scheme == Uri.UriSchemeHttp))
        {
            uri = parsed;
            return true;
        }

        uri = null!;
        return false;
    }

    public static bool OpenInBrowser(string? url)
    {
        if (!TryGetWebUri(url, out var uri))
        {
            return false;
        }

        Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
        return true;
    }

    public static string ThumbnailUrl(string url)
        => url.Contains("steamstatic.com", StringComparison.OrdinalIgnoreCase)
            ? url.Replace(".1920x1080.jpg", ".600x338.jpg", StringComparison.OrdinalIgnoreCase)
            : url;

    public static Brush AccentBrush(string? hex)
    {
        if (!string.IsNullOrWhiteSpace(hex))
        {
            try
            {
                if (ColorConverter.ConvertFromString(hex) is Color color)
                {
                    var brush = new SolidColorBrush(color);
                    brush.Freeze();
                    return brush;
                }
            }
            catch
            {
                // Fall through.
            }
        }

        var fallback = new SolidColorBrush(Color.FromRgb(0x4F, 0x6B, 0xFF));
        fallback.Freeze();
        return fallback;
    }

    private sealed class CachePaths(AppPaths paths) : ArtworkCacheService.ICachePaths
    {
        public string ArtworkCacheDir => paths.ArtworkCacheDir;
    }
}
