using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

public sealed class DotNetRuntimeProvider
{
    private const string ReleaseMetadataUrl = "https://builds.dotnet.microsoft.com/dotnet/release-metadata/{0}.0/releases.json";
    private const string RuntimeFileName = "dotnet-runtime-win-x64.zip";

    private readonly Dictionary<int, string> _resolved = new();
    private readonly object _lock = new();
    private readonly string _toolsDir;

    public DotNetRuntimeProvider(string? toolsDir = null)
    {
        _toolsDir = toolsDir ?? Path.Combine(DownloadHelper.ToolsDir(), "dotnet");
    }

    public async Task<string> ResolveAsync(int major, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        lock (_lock)
        {
            if (_resolved.TryGetValue(major, out var cached))
            {
                return cached;
            }
        }

        var path = FindProvisioned(major)
            ?? await SystemDotNetWithRuntimeAsync(major).ConfigureAwait(false)
            ?? await ProvisionAsync(major, progress, ct).ConfigureAwait(false);

        lock (_lock)
        {
            _resolved[major] = path;
        }

        return path;
    }

    private string? FindProvisioned(int major)
    {
        var dir = Path.Combine(_toolsDir, major.ToString());
        var exe = Path.Combine(dir, "dotnet.exe");
        return File.Exists(exe) && HasRuntime(dir, major) ? exe : null;
    }

    internal static bool HasRuntime(string dotnetRoot, int major)
    {
        var shared = Path.Combine(dotnetRoot, "shared", "Microsoft.NETCore.App");
        return Directory.Exists(shared)
               && Directory.GetDirectories(shared).Any(d => Path.GetFileName(d).StartsWith($"{major}.", StringComparison.Ordinal));
    }

    private static async Task<string?> SystemDotNetWithRuntimeAsync(int major)
    {
        try
        {
            var psi = new ProcessStartInfo("dotnet", "--list-runtimes")
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

            var output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            await process.WaitForExitAsync().ConfigureAwait(false);
            if (!ListsRuntime(output, major))
            {
                return null;
            }

            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var exe = Path.Combine(programFiles, "dotnet", "dotnet.exe");
            return File.Exists(exe) ? exe : "dotnet";
        }
        catch
        {
            return null;
        }
    }

    internal static bool ListsRuntime(string listRuntimesOutput, int major)
        => listRuntimesOutput.Split('\n')
            .Any(l => l.TrimStart().StartsWith($"Microsoft.NETCore.App {major}.", StringComparison.Ordinal));

    private async Task<string> ProvisionAsync(int major, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        progress.Report(InstallProgress.Indeterminate($".NET {major} isn't installed: looking up Microsoft's latest .NET {major} runtime…"));
        var metadata = await DownloadHelper.GetStringAsync(string.Format(ReleaseMetadataUrl, major), ct).ConfigureAwait(false);
        var (version, url, sha512) = ParseLatestRuntime(metadata)
            ?? throw new InvalidOperationException($"Microsoft's release list has no Windows x64 .NET {major} runtime download.");

        var target = Path.Combine(_toolsDir, major.ToString());
        var staging = target + ".staging";
        var zip = Path.Combine(_toolsDir, $"dotnet-runtime-{version}-win-x64.zip");
        try
        {
            progress.Report(InstallProgress.Indeterminate($"Downloading the .NET {version} runtime (about 30 MB)…"));
            await DownloadHelper.DownloadFileAsync(url, zip, ct).ConfigureAwait(false);

            await using (var stream = File.OpenRead(zip))
            {
                var actual = Convert.ToHexString(await SHA512.HashDataAsync(stream, ct).ConfigureAwait(false));
                if (!actual.Equals(sha512, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"The .NET {version} download didn't match Microsoft's checksum, so it wasn't used.");
                }
            }

            progress.Report(InstallProgress.Indeterminate($"Unpacking the .NET {version} runtime…"));
            if (Directory.Exists(staging))
            {
                Directory.Delete(staging, recursive: true);
            }

            await Task.Run(() => DownloadHelper.ExtractZip(zip, staging), ct).ConfigureAwait(false);
            if (Directory.Exists(target))
            {
                Directory.Delete(target, recursive: true);
            }

            Directory.Move(staging, target);
        }
        finally
        {
            try
            {
                File.Delete(zip);
                if (Directory.Exists(staging))
                {
                    Directory.Delete(staging, recursive: true);
                }
            }
            catch
            {
                // Leftovers are cleaned up on the next attempt.
            }
        }

        return FindProvisioned(major)
            ?? throw new InvalidOperationException($"The .NET {major} runtime was downloaded but dotnet.exe wasn't found in it.");
    }

    internal static (string Version, string Url, string Sha512)? ParseLatestRuntime(string releasesJson)
    {
        using var doc = JsonDocument.Parse(releasesJson);
        var root = doc.RootElement;
        if (!root.TryGetProperty("latest-runtime", out var latestEl) || latestEl.GetString() is not { Length: > 0 } latest
            || !root.TryGetProperty("releases", out var releases))
        {
            return null;
        }

        foreach (var release in releases.EnumerateArray())
        {
            if (!release.TryGetProperty("runtime", out var runtime) || runtime.ValueKind != JsonValueKind.Object
                || runtime.GetProperty("version").GetString() != latest
                || !runtime.TryGetProperty("files", out var files))
            {
                continue;
            }

            foreach (var file in files.EnumerateArray())
            {
                if (file.GetProperty("name").GetString() != RuntimeFileName)
                {
                    continue;
                }

                var url = file.GetProperty("url").GetString();
                var hash = file.GetProperty("hash").GetString();
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps
                    && (uri.Host.EndsWith(".microsoft.com", StringComparison.OrdinalIgnoreCase)
                        || uri.Host.EndsWith(".dot.net", StringComparison.OrdinalIgnoreCase))
                    && !string.IsNullOrWhiteSpace(hash))
                {
                    return (latest, uri.AbsoluteUri, hash);
                }
            }
        }

        return null;
    }
}
