using System.Text.Json;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

public sealed partial class CfxProvider : IInstallProvider
{
    private const string ChangelogApi = "https://changelogs-live.fivem.net/api/changelog/versions/win32/server";

    private const string ArtifactBase = "https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/";

    private const string DownloadPage = "https://docs.fivem.net/docs/server-download/?platform=legacy&os=windows";

    internal const string ArtifactFolder = "artifact";

    private static readonly string[] Channels = { "recommended", "latest", "optional", "critical" };

    public string Id => "cfx";

    private static string ArtifactLink() => ArtifactBase;

    internal static SortedDictionary<string, string> ParseArtifactIndex(string html)
    {
        var map = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (Match match in ArtifactIndexRegex().Matches(html))
        {
            var full = match.Groups[1].Value;
            var build = match.Groups[2].Value;
            map.TryAdd(build, full);
        }

        return map;
    }

    [GeneratedRegex("(?:href=\"|build_server_windows/master/)((\\d+)-[0-9a-fA-F]+)/")]
    private static partial Regex ArtifactIndexRegex();

    internal sealed record Channel(string Name, string Build, string? ArtifactDir);

    internal static List<Channel> ParseChannels(string json)
    {
        var channels = new List<Channel>();
        using var doc = JsonDocument.Parse(json);
        foreach (var name in Channels)
        {
            if (!doc.RootElement.TryGetProperty(name, out var buildEl) || buildEl.GetString() is not { Length: > 0 } build)
            {
                continue;
            }

            string? dir = null;
            if (doc.RootElement.TryGetProperty($"{name}_download", out var urlEl) && urlEl.GetString() is { } url)
            {
                var index = ParseArtifactIndex(url);
                index.TryGetValue(build, out dir);
            }

            channels.Add(new Channel(name, build, dir));
        }

        return channels;
    }

    private static async Task<List<Channel>> GetChannelsAsync(CancellationToken ct)
    {
        try
        {
            return ParseChannels(await DownloadHelper.GetStringAsync(ChangelogApi, ct).ConfigureAwait(false));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new List<Channel>();
        }
    }

    private static async Task<(SortedDictionary<string, string> Index, List<Channel> Channels)> GetIndexAsync(CancellationToken ct)
    {
        var channelsTask = GetChannelsAsync(ct);

        SortedDictionary<string, string> index;
        try
        {
            index = ParseArtifactIndex(await DownloadHelper.GetStringAsync(DownloadPage, ct).ConfigureAwait(false));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            index = new SortedDictionary<string, string>(StringComparer.Ordinal);
        }

        var channels = await channelsTask.ConfigureAwait(false);
        foreach (var channel in channels)
        {
            if (channel.ArtifactDir is not null)
            {
                index.TryAdd(channel.Build, channel.ArtifactDir);
            }
        }

        return (index, channels);
    }

    internal static List<string> BuildVersionList(SortedDictionary<string, string> index, IReadOnlyList<Channel> channels)
    {
        var output = new List<string>();
        var seen = new HashSet<string>();

        foreach (var group in channels.Where(c => index.ContainsKey(c.Build)).GroupBy(c => c.Build))
        {
            if (seen.Add(group.Key))
            {
                output.Add($"build {group.Key} ({string.Join(", ", group.Select(c => c.Name))})");
            }
        }

        var builds = index.Keys
            .Select(k => long.TryParse(k, out var v) ? v : (long?)null)
            .Where(v => v is not null)
            .Select(v => v!.Value)
            .OrderByDescending(v => v);

        foreach (var build in builds)
        {
            var key = build.ToString();
            if (seen.Add(key))
            {
                output.Add($"build {key}");
            }
        }

        return output;
    }

    public async Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct)
    {
        var (index, channels) = await GetIndexAsync(ct).ConfigureAwait(false);
        return BuildVersionList(index, channels);
    }

    public async Task<InstallResult> InstallAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        progress.Report(InstallProgress.Indeterminate("Resolving Cfx.re server artifact…"));

        // "build 1234 (recommended)" / "build 1234" / "1234" -> "1234"
        // (first run of ASCII digits in request.Version: split on non-digits, take the first
        // non-empty segment)
        var requestedBuild = ExtractFirstDigitRun(request.Version ?? string.Empty);

        var (index, channels) = await GetIndexAsync(ct).ConfigureAwait(false);
        if (index.Count == 0)
        {
            throw new InvalidOperationException(
                "Could not reach Cfx.re to look up FXServer builds. Check your internet connection and try again.");
        }

        string buildNumber;
        string artifactDir;

        if (!string.IsNullOrEmpty(requestedBuild) && index.TryGetValue(requestedBuild, out var requestedDir))
        {
            buildNumber = requestedBuild;
            artifactDir = requestedDir;
        }
        else
        {
            var recommended = channels.FirstOrDefault(c => c.Name == "recommended")?.Build ?? string.Empty;
            if (index.TryGetValue(recommended, out var recommendedDir))
            {
                if (!string.IsNullOrEmpty(requestedBuild))
                {
                    progress.Report(InstallProgress.Indeterminate(
                        $"Build {requestedBuild} is no longer available — using recommended build {recommended} instead."));
                }

                buildNumber = recommended;
                artifactDir = recommendedDir;
            }
            else
            {
                var newest = index.Keys
                    .Select(k => long.TryParse(k, out var v) ? v : (long?)null)
                    .Where(v => v is not null)
                    .Select(v => v!.Value)
                    .Max()
                    .ToString();
                buildNumber = newest;
                artifactDir = index[newest];
            }
        }

        var zipUrl = $"{ArtifactLink()}{artifactDir}/server.zip";
        var zipPath = Path.Combine(request.TargetDir, "server.zip");

        progress.Report(InstallProgress.Indeterminate($"Downloading FXServer build {buildNumber}"));
        await DownloadHelper.DownloadFileAsync(zipUrl, zipPath, ct).ConfigureAwait(false);

        // The artifact's server.zip has FXServer.exe at its root. Extract it into an "artifact"
        // sub-folder (Cfx.re's recommended layout: binaries separate from server data/txData),
        // which is where both built-in definitions' Runtime.Executable
        // ("{server_root}\artifact\FXServer.exe") expect it.
        progress.Report(InstallProgress.Indeterminate("Extracting FXServer…"));
        DownloadHelper.ExtractZip(zipPath, Path.Combine(request.TargetDir, ArtifactFolder));
        TryDelete(zipPath);

        return new InstallResult
        {
            ServerFile = Path.Combine(ArtifactFolder, "FXServer.exe"),
            Version = $"build {buildNumber}",
            StartCommand = null,
            JavaMajor = 0,
        };
    }

    private static string ExtractFirstDigitRun(string value)
    {
        var start = -1;
        for (var i = 0; i < value.Length; i++)
        {
            if (char.IsAsciiDigit(value[i]))
            {
                start = i;
                break;
            }
        }

        if (start < 0)
        {
            return string.Empty;
        }

        var end = start;
        while (end < value.Length && char.IsAsciiDigit(value[end]))
        {
            end++;
        }

        return value[start..end];
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch
        {
            // Best-effort, fire-and-forget cleanup.
        }
    }
}
