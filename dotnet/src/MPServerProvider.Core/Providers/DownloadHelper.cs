using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;

namespace MPServerProvider.Core.Providers;

internal static class DownloadHelper
{
    internal const long MaxDownloadBytes = 16L * 1024 * 1024 * 1024;

    private static readonly HttpClient Client = new()
    {
        Timeout = TimeSpan.FromMinutes(30),
    };

    public static async Task<string> GetStringAsync(string url, CancellationToken ct = default)
    {
        var uri = RequireHttps(url);
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.UserAgent.ParseAdd("MPServerProvider/1.0");
        using var response = await Client.SendAsync(request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
    }

    public static async Task DownloadFileAsync(
        string url,
        string destPath,
        CancellationToken ct = default,
        string? expectedSha256 = null,
        string? expectedSha1 = null)
    {
        var uri = RequireHttps(url);
        var dir = Path.GetDirectoryName(destPath);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var partPath = destPath + ".part";
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.UserAgent.ParseAdd("MPServerProvider/1.0");
            using var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength is > MaxDownloadBytes)
            {
                throw new InvalidOperationException($"Download from {uri.Host} is unexpectedly large; refusing it.");
            }

            using var sha256 = expectedSha256 is null ? null : IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            using var sha1 = expectedSha1 is null ? null : IncrementalHash.CreateHash(HashAlgorithmName.SHA1);

            await using (var source = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false))
            await using (var dest = new FileStream(partPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                var buffer = new byte[81920];
                long total = 0;
                int read;
                while ((read = await source.ReadAsync(buffer, ct).ConfigureAwait(false)) > 0)
                {
                    total += read;
                    if (total > MaxDownloadBytes)
                    {
                        throw new InvalidOperationException($"Download from {uri.Host} exceeded the size limit; aborted.");
                    }

                    sha256?.AppendData(buffer, 0, read);
                    sha1?.AppendData(buffer, 0, read);
                    await dest.WriteAsync(buffer.AsMemory(0, read), ct).ConfigureAwait(false);
                }
            }

            VerifyHash(sha256, expectedSha256, "SHA-256", uri);
            VerifyHash(sha1, expectedSha1, "SHA-1", uri);

            File.Move(partPath, destPath, overwrite: true);
        }
        catch
        {
            TryDeleteFile(partPath);
            throw;
        }
    }

    private static void VerifyHash(IncrementalHash? hash, string? expectedHex, string name, Uri uri)
    {
        if (hash is null || string.IsNullOrWhiteSpace(expectedHex))
        {
            return;
        }

        var actual = Convert.ToHexString(hash.GetHashAndReset());
        if (!string.Equals(actual, expectedHex.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"The file downloaded from {uri.Host} failed its {name} integrity check (expected {expectedHex.Trim()}, got {actual}). It was discarded.");
        }
    }

    internal static Uri RequireHttps(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException($"Refusing to download from '{url}': only https:// URLs are allowed.");
        }

        return uri;
    }

    internal static string SafeFileName(string? name, string fallback)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return fallback;
        }

        var lastSep = name.LastIndexOfAny(new[] { '/', '\\' });
        var candidate = (lastSep >= 0 ? name[(lastSep + 1)..] : name).Trim();
        if (candidate.Length == 0 || candidate is "." or ".."
            || candidate.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || candidate.Contains(':'))
        {
            return fallback;
        }

        return candidate;
    }

    public static void ExtractZip(string zipPath, string destDir)
    {
        Directory.CreateDirectory(destDir);
        ZipFile.ExtractToDirectory(zipPath, destDir, overwriteFiles: true);
    }

    public static async Task ExtractTarGzAsync(string archivePath, string destDir, CancellationToken ct = default)
    {
        Directory.CreateDirectory(destDir);
        await using var file = File.OpenRead(archivePath);
        await using var gzip = new GZipStream(file, CompressionMode.Decompress);
        await TarFile.ExtractToDirectoryAsync(gzip, destDir, overwriteFiles: true, ct).ConfigureAwait(false);
    }

    public static void FlattenSingleRootFolder(string dir)
    {
        var entries = Directory.GetFileSystemEntries(dir);
        if (entries.Length != 1)
        {
            return;
        }

        var only = entries[0];
        if (!Directory.Exists(only))
        {
            return;
        }

        var wrapperName = Path.GetFileName(only);
        var children = Directory.GetFileSystemEntries(only);
        if (children.Any(c => string.Equals(Path.GetFileName(c), wrapperName, StringComparison.OrdinalIgnoreCase)))
        {
            // A child named like its wrapper (e.g. "server/server") can't be moved up while the
            // wrapper still exists; leave the layout as-is rather than failing half-way.
            return;
        }

        foreach (var child in children)
        {
            var target = Path.Combine(dir, Path.GetFileName(child));
            if (Directory.Exists(child))
            {
                Directory.Move(child, target);
            }
            else
            {
                File.Move(child, target);
            }
        }

        Directory.Delete(only);
    }

    internal static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Best-effort cleanup.
        }
    }

    public static string ToolsDir() => Path.Combine(Services.AppPaths.DefaultDataRoot, "tools");
}
