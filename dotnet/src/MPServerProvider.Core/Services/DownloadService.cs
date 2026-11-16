using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;

using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public static class DownloadService
{
    private static readonly Lazy<HttpClient> LazyClient = new(CreateClient);

    private static HttpClient CreateClient()
    {
        var handler = new SocketsHttpHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.All,
            MaxAutomaticRedirections = 10,
        };
        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMinutes(10),
        };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MPServerProvider/1.0");
        return client;
    }

    private static HttpClient Client => LazyClient.Value;

    public static async Task<string> GetStringAsync(string url, CancellationToken ct = default)
    {
        using var response = await Client.GetAsync(url, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
    }

    public static async Task<T?> GetJsonAsync<T>(string url, CancellationToken ct = default)
    {
        var body = await GetStringAsync(url, ct).ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(body, Json.Json.Options);
    }

    public static async Task DownloadFileAsync(
        string url,
        string destPath,
        string status,
        IProgress<InstallProgress> progress,
        CancellationToken ct = default)
    {
        var dir = Path.GetDirectoryName(destPath);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException($"Refusing to download from '{url}': only https:// URLs are allowed.");
        }

        // Stream into a .part file and only move it into place once complete, so a failed or
        // cancelled download never leaves a truncated file that later looks "installed".
        var partPath = destPath + ".part";
        try
        {
            using var response = await Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var total = response.Content.Headers.ContentLength;

            progress.Report(InstallProgress.Indeterminate(status));

            await using (var source = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false))
            await using (var dest = File.Create(partPath))
            {
                var buffer = new byte[81920];
                long received = 0;
                int read;
                while ((read = await source.ReadAsync(buffer, ct).ConfigureAwait(false)) > 0)
                {
                    await dest.WriteAsync(buffer.AsMemory(0, read), ct).ConfigureAwait(false);
                    received += read;
                    if (total is > 0)
                    {
                        var percent = Math.Clamp((double)received / total.Value * 100.0, 0.0, 100.0);
                        progress.Report(InstallProgress.At(status, percent));
                    }
                }
            }

            File.Move(partPath, destPath, overwrite: true);
        }
        catch
        {
            try
            {
                if (File.Exists(partPath))
                {
                    File.Delete(partPath);
                }
            }
            catch
            {
                // Best-effort cleanup.
            }

            throw;
        }
    }

    public static void ExtractZip(string zipPath, string destDir, string status, IProgress<InstallProgress> progress)
    {
        progress.Report(InstallProgress.Indeterminate(status));
        Directory.CreateDirectory(destDir);
        ZipFile.ExtractToDirectory(zipPath, destDir, overwriteFiles: true);
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

        foreach (var child in Directory.GetFileSystemEntries(only))
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

    public static async Task<bool> VerifyHashAsync(string path, string expectedHex, CancellationToken ct = default)
    {
        await using var stream = File.OpenRead(path);
        var digest = await SHA256.HashDataAsync(stream, ct).ConfigureAwait(false);
        var actualHex = Convert.ToHexString(digest).ToLowerInvariant();
        return string.Equals(actualHex, expectedHex.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    public static string FormatBytes(long bytes)
    {
        const double Kb = 1024.0;
        const double Mb = Kb * 1024.0;
        const double Gb = Mb * 1024.0;
        double b = bytes;

        if (b >= Gb)
        {
            return $"{(b / Gb).ToString("F1", System.Globalization.CultureInfo.InvariantCulture)} GB";
        }

        if (b >= Mb)
        {
            return $"{(b / Mb).ToString("F1", System.Globalization.CultureInfo.InvariantCulture)} MB";
        }

        if (b >= Kb)
        {
            return $"{(b / Kb).ToString("F1", System.Globalization.CultureInfo.InvariantCulture)} KB";
        }

        return $"{bytes} B";
    }
}
