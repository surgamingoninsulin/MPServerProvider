using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;

namespace MPServerProvider.Core.Providers;

/// <summary>
/// Minimal download/extract helpers used by the install providers.
/// The decompiled providers call into <c>MPServerProvider.Core.Services.DownloadService</c>
/// (<c>GetStringAsync</c>, <c>DownloadFileAsync</c>, <c>ExtractZip</c>,
/// <c>FlattenSingleRootFolder</c>) for every network/archive operation.
/// <c>MPServerProvider.Core.Services</c> is out of scope for this task (a separate task owns
/// <c>Services/</c>), so these are small, provider-local equivalents built directly on
/// <see cref="HttpClient"/>/<see cref="ZipFile"/>/<see cref="System.IO"/> rather than a port of
/// that service — mirroring the proven Rust port's <c>providers/http.rs</c> approach exactly.
/// <para>
/// HARDENING: downloads are https-only, streamed to a <c>.part</c> file that is only moved into
/// place once complete (and, when the upstream API publishes one, hash-verified), and the
/// partial file is always removed on failure/cancellation.
/// </para>
/// </summary>
internal static class DownloadHelper
{
    /// <summary>Hard cap for a single download (a server artifact is never anywhere near this).</summary>
    internal const long MaxDownloadBytes = 16L * 1024 * 1024 * 1024;

    private static readonly HttpClient Client = new()
    {
        Timeout = TimeSpan.FromMinutes(30),
    };

    /// <summary>
    /// Port-equivalent of <c>DownloadService.GetStringAsync(string url)</c>: a plain HTTP GET
    /// returning the response body as text. Used for every JSON/HTML manifest fetch (Mojang's
    /// version manifest, Cfx.re's artifact index, GitHub release metadata, etc).
    /// </summary>
    public static async Task<string> GetStringAsync(string url, CancellationToken ct = default)
    {
        var uri = RequireHttps(url);
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.UserAgent.ParseAdd("MPServerProvider/1.0");
        using var response = await Client.SendAsync(request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Port-equivalent of <c>DownloadService.DownloadFileAsync(string url, string destPath)</c>:
    /// streams the response body to <paramref name="destPath"/>, creating parent directories as
    /// needed. The body goes to <c>destPath + ".part"</c> first and is moved into place only once
    /// complete and (when <paramref name="expectedSha256"/>/<paramref name="expectedSha1"/> is
    /// given) verified; the partial file is deleted on any failure or cancellation.
    /// </summary>
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

    /// <summary>Rejects anything but an absolute <c>https://</c> URL.</summary>
    internal static Uri RequireHttps(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException($"Refusing to download from '{url}': only https:// URLs are allowed.");
        }

        return uri;
    }

    /// <summary>
    /// Turns a remote-supplied name (URL path segment, API <c>download.name</c>) into a plain
    /// file name that can't escape the target folder: takes the last path segment (both
    /// separators), rejects invalid characters, <c>.</c>/<c>..</c> and empty results, falling
    /// back to <paramref name="fallback"/>.
    /// </summary>
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

    /// <summary>Port-equivalent of <c>DownloadService.ExtractZip(string zipPath, string destDir)</c>.</summary>
    public static void ExtractZip(string zipPath, string destDir)
    {
        Directory.CreateDirectory(destDir);
        ZipFile.ExtractToDirectory(zipPath, destDir, overwriteFiles: true);
    }

    /// <summary>Extracts a gzip-compressed tar (<c>.tar.gz</c>/<c>.tgz</c>) into <paramref name="destDir"/>.</summary>
    public static async Task ExtractTarGzAsync(string archivePath, string destDir, CancellationToken ct = default)
    {
        Directory.CreateDirectory(destDir);
        await using var file = File.OpenRead(archivePath);
        await using var gzip = new GZipStream(file, CompressionMode.Decompress);
        await TarFile.ExtractToDirectoryAsync(gzip, destDir, overwriteFiles: true, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Port-equivalent of <c>DownloadService.FlattenSingleRootFolder(string dir)</c>: if
    /// <paramref name="dir"/> contains exactly one entry and that entry is itself a directory
    /// (the common shape of a zip that wraps everything in a single top-level folder, e.g.
    /// <c>project-v1.2.3/...</c>), move that folder's contents up into <paramref name="dir"/> and
    /// remove the now-empty wrapper.
    /// <para>
    /// INFERRED: the decompile evidence for this method's own body wasn't captured (only its call
    /// sites, e.g. <c>ManualProvider.InstallAsync</c>, <c>UrlProvider.InstallAsync</c>), so this
    /// reconstructs the documented *purpose* from the name and call sites rather than from a
    /// disassembled body.
    /// </para>
    /// </summary>
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

    /// <summary>
    /// Port-equivalent of <c>AppPaths.ToolsDir</c>: <c>{DataRoot}/tools</c>. Resolves the data
    /// root through <see cref="Services.AppPaths.DefaultDataRoot"/> — the exact same rules the
    /// app's own <see cref="Services.AppPaths"/> uses — so SteamCMD/Java provisioned by the
    /// providers and by <see cref="Services.WorkshopService"/>/<see cref="Services.TunnelService"/>
    /// land in one shared folder instead of being downloaded twice.
    /// </summary>
    public static string ToolsDir() => Path.Combine(Services.AppPaths.DefaultDataRoot, "tools");
}
