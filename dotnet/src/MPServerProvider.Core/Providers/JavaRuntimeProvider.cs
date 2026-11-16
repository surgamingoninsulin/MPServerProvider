using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

public sealed partial class JavaRuntimeProvider
{
    private readonly Dictionary<int, string> _resolved = new();
    private readonly object _lock = new();

    public async Task<string> ResolveAsync(int majorVersion, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        lock (_lock)
        {
            if (_resolved.TryGetValue(majorVersion, out var cached))
            {
                return cached;
            }
        }

        var path = FindProvisioned(majorVersion);
        path ??= await SystemJavaMatchesAsync(majorVersion).ConfigureAwait(false);
        path ??= await ProvisionTemurinAsync(majorVersion, progress, ct).ConfigureAwait(false);

        lock (_lock)
        {
            _resolved[majorVersion] = path;
        }

        return path;
    }

    private static string? FindProvisioned(int major)
    {
        var dir = Path.Combine(DownloadHelper.ToolsDir(), "java", major.ToString());
        return !Directory.Exists(dir) ? null : WalkForJavaExe(dir);
    }

    private static async Task<string?> SystemJavaMatchesAsync(int major)
    {
        string banner;
        try
        {
            var psi = new ProcessStartInfo("java", "-version")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var process = Process.Start(psi);
            if (process is null)
            {
                return null;
            }

            var stdout = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            var stderr = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
            await process.WaitForExitAsync().ConfigureAwait(false);
            banner = stdout + stderr;
        }
        catch
        {
            return null;
        }

        var match = JavaVersionRegex().Match(banner);
        if (!match.Success)
        {
            return null;
        }

        if (!int.TryParse(match.Groups[1].Value, out var first))
        {
            return null;
        }

        int detectedMajor;
        if (first == 1)
        {
            // Legacy "1.8.0_xxx" scheme: the real major is the second component.
            if (!match.Groups[2].Success || !int.TryParse(match.Groups[2].Value, out detectedMajor))
            {
                return null;
            }
        }
        else
        {
            detectedMajor = first;
        }

        return detectedMajor == major ? "java" : null;
    }

    [GeneratedRegex("version \"(\\d+)(?:\\.(\\d+))?")]
    private static partial Regex JavaVersionRegex();

    private static async Task<string> ProvisionTemurinAsync(int major, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        progress.Report(InstallProgress.Indeterminate($"Resolving Temurin JRE {major} from Adoptium…"));

        // Exact major first (JRE, then JDK: Adoptium doesn't publish a JRE image for every
        // release). If Temurin doesn't publish that major for Windows x64 at all, fall back to
        // the nearest NEWER release it does publish (Java runs older class files, so a newer
        // runtime is the safe direction), preferring an LTS.
        var picked = await FindTemurinPackageAsync(major, ct).ConfigureAwait(false);
        if (picked is null)
        {
            var fallback = await PickFallbackFromAdoptiumAsync(major, ct).ConfigureAwait(false);
            if (fallback is { } newer)
            {
                progress.Report(InstallProgress.Indeterminate($"Temurin {major} isn't published for Windows x64; using Temurin {newer} instead…"));
                picked = await FindTemurinPackageAsync(newer, ct).ConfigureAwait(false);
            }
        }

        if (picked is not { } package)
        {
            throw new InvalidOperationException(
                $"Adoptium (Eclipse Temurin) does not publish a Java {major} runtime for Windows x64, and no newer one could be found. " +
                $"Install Java {major} or newer yourself (for example from adoptium.net) and make sure 'java' on PATH reports version {major}.");
        }

        var (link, checksum, actualMajor) = package;
        var existing = FindProvisioned(actualMajor);
        if (existing is not null)
        {
            return existing;
        }

        var dir = Path.Combine(DownloadHelper.ToolsDir(), "java", actualMajor.ToString());
        var zipPath = Path.Combine(dir, "temurin.zip");

        progress.Report(InstallProgress.Indeterminate($"Downloading Temurin Java {actualMajor}"));
        // Adoptium publishes the package's SHA-256; a mismatch discards the download.
        await DownloadHelper.DownloadFileAsync(link, zipPath, ct, expectedSha256: checksum).ConfigureAwait(false);

        progress.Report(InstallProgress.Indeterminate("Extracting Java runtime…"));
        try
        {
            DownloadHelper.ExtractZip(zipPath, dir);
        }
        finally
        {
            DownloadHelper.TryDeleteFile(zipPath);
        }

        return FindProvisioned(actualMajor)
            ?? throw new InvalidOperationException($"Temurin Java {actualMajor} was downloaded but java.exe was not found.");
    }

    private const string AdoptiumApi = "https://api.adoptium.net/v3";

    private static async Task<(string Link, string? Sha256, int Major)?> FindTemurinPackageAsync(int major, CancellationToken ct)
    {
        foreach (var imageType in new[] { "jre", "jdk" })
        {
            string body;
            try
            {
                body = await DownloadHelper.GetStringAsync(
                    $"{AdoptiumApi}/assets/latest/{major}/hotspot?os=windows&architecture=x64&image_type={imageType}", ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                continue;
            }

            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
            {
                continue;
            }

            var (link, checksum) = ParseAdoptiumPackage(doc.RootElement[0]);
            if (link is not null)
            {
                return (link, checksum, major);
            }
        }

        return null;
    }

    private static async Task<int?> PickFallbackFromAdoptiumAsync(int major, CancellationToken ct)
    {
        try
        {
            using var doc = JsonDocument.Parse(await DownloadHelper.GetStringAsync($"{AdoptiumApi}/info/available_releases", ct).ConfigureAwait(false));
            return PickFallbackMajor(major, ReadIntArray(doc.RootElement, "available_releases"), ReadIntArray(doc.RootElement, "available_lts_releases"));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    private static List<int> ReadIntArray(JsonElement root, string name)
    {
        var result = new List<int>();
        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty(name, out var arr) && arr.ValueKind == JsonValueKind.Array)
        {
            foreach (var v in arr.EnumerateArray())
            {
                if (v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var n))
                {
                    result.Add(n);
                }
            }
        }

        return result;
    }

    internal static int? PickFallbackMajor(int requested, IEnumerable<int> available, IEnumerable<int> lts)
    {
        var newerLts = lts.Where(v => v > requested).OrderBy(v => v).ToList();
        if (newerLts.Count > 0)
        {
            return newerLts[0];
        }

        var newer = available.Where(v => v > requested).OrderBy(v => v).ToList();
        return newer.Count > 0 ? newer[0] : null;
    }

    internal static (string? Link, string? Sha256) ParseAdoptiumPackage(JsonElement asset)
    {
        if (asset.ValueKind != JsonValueKind.Object
            || !asset.TryGetProperty("binary", out var binary)
            || !binary.TryGetProperty("package", out var package))
        {
            return (null, null);
        }

        var link = package.TryGetProperty("link", out var linkEl) && linkEl.ValueKind == JsonValueKind.String ? linkEl.GetString() : null;
        var checksum = package.TryGetProperty("checksum", out var sumEl) && sumEl.ValueKind == JsonValueKind.String ? sumEl.GetString() : null;
        return (link, string.IsNullOrWhiteSpace(checksum) ? null : checksum);
    }

    internal static string? WalkForJavaExe(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return null;
        }

        var subdirs = new List<string>();
        foreach (var entry in Directory.EnumerateFileSystemEntries(dir))
        {
            if (Directory.Exists(entry))
            {
                subdirs.Add(entry);
            }
            else if (string.Equals(Path.GetFileName(entry), "java.exe", StringComparison.OrdinalIgnoreCase))
            {
                return entry;
            }
        }

        foreach (var sub in subdirs)
        {
            var found = WalkForJavaExe(sub);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }
}
