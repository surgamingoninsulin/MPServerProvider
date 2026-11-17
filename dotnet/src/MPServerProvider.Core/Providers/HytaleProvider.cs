using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

public sealed partial class HytaleProvider : IInstallProvider
{
    internal const string DownloaderUrl = "https://downloader.hytale.com/hytale-downloader.zip";
    private const string PackageName = "hytale-game.zip";

    private static readonly TimeSpan RunTimeout = TimeSpan.FromMinutes(45);

    private readonly string _toolsDir;

    public HytaleProvider(string? toolsDir = null)
    {
        _toolsDir = toolsDir ?? Path.Combine(DownloadHelper.ToolsDir(), "hytale-downloader");
    }

    public string Id => "hytale";

    public Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct)
        => Task.FromResult(new List<string> { "latest" });

    public async Task<InstallResult> InstallAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var downloader = await EnsureDownloaderAsync(progress, ct).ConfigureAwait(false);
        var package = Path.Combine(request.TargetDir, PackageName);
        TryDelete(package);

        progress.Report(InstallProgress.Indeterminate("Hytale Downloader: starting…"));
        string? version = null;
        void OnLine(string line)
        {
            if (SignInLink(line) is { } link)
            {
                var code = DeviceCode(line);
                progress.Report(new InstallProgress
                {
                    Status = code is not null
                        ? $"Sign in with your Hytale account: open the sign-in page and enter code {code}."
                        : "Sign in with your Hytale account on the page that just opened.",
                    Link = link,
                    Code = code,
                });
            }
            else if (DeviceCode(line) is { } code)
            {
                progress.Report(new InstallProgress
                {
                    Status = $"Sign in with your Hytale account and enter code {code}.",
                    Code = code,
                });
            }
            else if (Percent(line) is { } percent)
            {
                progress.Report(InstallProgress.At("Hytale Downloader: downloading the server…", percent));
            }

            version ??= Version(line);
        }

        var run = await ExternalProcess.RunAsync(
            downloader,
            new[] { "-download-path", package },
            _toolsDir,
            OnLine,
            RunTimeout,
            ct).ConfigureAwait(false);

        if (!File.Exists(package))
        {
            var detail = run.Output.Split('\n').Select(l => l.Trim()).LastOrDefault(l => l.Length > 0);
            throw new InvalidOperationException(
                $"The Hytale Downloader didn't download the server (exit code {run.ExitCode})" + (detail is null ? "." : $": {detail}"));
        }

        progress.Report(InstallProgress.Indeterminate("Unpacking the Hytale server…"));
        try
        {
            await Task.Run(() => DownloadHelper.ExtractZip(package, request.TargetDir), ct).ConfigureAwait(false);
        }
        finally
        {
            TryDelete(package);
        }

        var jar = Path.Combine(request.TargetDir, "Server", "HytaleServer.jar");
        if (!File.Exists(jar) || !File.Exists(Path.Combine(request.TargetDir, "Assets.zip")))
        {
            throw new InvalidOperationException("The Hytale package didn't contain Server\\HytaleServer.jar and Assets.zip.");
        }

        return new InstallResult
        {
            ServerFile = jar,
            Version = version ?? "latest",
            StartCommand = null,
            JavaMajor = 25,
        };
    }

    private async Task<string> EnsureDownloaderAsync(IProgress<InstallProgress> progress, CancellationToken ct)
    {
        if (FindDownloaderExe(_toolsDir) is { } existing)
        {
            return existing;
        }

        progress.Report(InstallProgress.Indeterminate("Downloading the Hytale Downloader…"));
        Directory.CreateDirectory(_toolsDir);
        var zip = Path.Combine(_toolsDir, "hytale-downloader.zip");
        await DownloadHelper.DownloadFileAsync(DownloaderUrl, zip, ct).ConfigureAwait(false);
        try
        {
            DownloadHelper.ExtractZip(zip, _toolsDir);
        }
        finally
        {
            TryDelete(zip);
        }

        return FindDownloaderExe(_toolsDir)
            ?? throw new InvalidOperationException("The Hytale Downloader zip didn't contain a Windows program.");
    }

    internal static string? FindDownloaderExe(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return null;
        }

        var exes = Directory.GetFiles(dir, "*.exe", SearchOption.AllDirectories)
            .Where(f => Path.GetFileName(f).Contains("hytale-downloader", StringComparison.OrdinalIgnoreCase))
            .ToList();
        return exes.FirstOrDefault(f => Path.GetFileName(f).Contains("windows", StringComparison.OrdinalIgnoreCase))
            ?? exes.FirstOrDefault();
    }

    internal static string? SignInLink(string line)
    {
        foreach (Match match in UrlRegex().Matches(line))
        {
            if (Uri.TryCreate(match.Value.TrimEnd('.', ',', ')'), UriKind.Absolute, out var uri)
                && uri.Scheme == Uri.UriSchemeHttps
                && (uri.Host.Equals("hytale.com", StringComparison.OrdinalIgnoreCase)
                    || uri.Host.EndsWith(".hytale.com", StringComparison.OrdinalIgnoreCase))
                && !uri.Host.StartsWith("downloader.", StringComparison.OrdinalIgnoreCase))
            {
                return uri.AbsoluteUri;
            }
        }

        return null;
    }

    internal static string? DeviceCode(string line)
    {
        var match = DeviceCodeRegex().Match(line);
        return match.Success ? match.Groups["code"].Value : null;
    }

    internal static double? Percent(string line)
    {
        var match = PercentRegex().Match(line);
        return match.Success
               && double.TryParse(match.Groups["pct"].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var pct)
               && pct is >= 0 and <= 100
            ? pct
            : null;
    }

    internal static string? Version(string line)
    {
        var match = VersionRegex().Match(line);
        return match.Success ? match.Groups["v"].Value : null;
    }

    private static void TryDelete(string path)
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
            // Leftover temp file; harmless.
        }
    }

    [GeneratedRegex(@"https://[^\s""'<>]+", RegexOptions.IgnoreCase)]
    private static partial Regex UrlRegex();

    [GeneratedRegex(@"(?:user_code=|code[:\s]+)(?<code>[A-Z0-9]{4,}(?:-[A-Z0-9]{4,})?)", RegexOptions.IgnoreCase)]
    private static partial Regex DeviceCodeRegex();

    [GeneratedRegex(@"(?<pct>\d{1,3}(?:\.\d+)?)\s*%")]
    private static partial Regex PercentRegex();

    [GeneratedRegex(@"version[:\s]+(?<v>\d{4}\.\d{2}\.\d{2}[\w.\-]*)", RegexOptions.IgnoreCase)]
    private static partial Regex VersionRegex();
}
