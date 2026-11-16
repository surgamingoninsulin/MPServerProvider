using System.Text.Json;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Providers.CfxProvider</c> (decompile token 0x0200001e).
/// Installs the Cfx.re "FXServer" dedicated server (FiveM/RedM).
/// <para>
/// Cfx.re retired the browsable artifact index the original scraped
/// (<c>runtime.fivem.net/artifacts/.../master/</c> now 301-redirects to
/// <c>docs.fivem.net/docs/server-download/</c>), which left the version list empty. Builds are
/// now collected from two sources: the changelog API (the recommended/latest/optional/critical
/// channels, each with its own download link) and the download page, which embeds direct links
/// to every build Cfx.re still offers. The artifact files themselves still live at the same
/// <c>runtime.fivem.net</c> paths, so the download step is unchanged.
/// </para>
/// </summary>
public sealed partial class CfxProvider : IInstallProvider
{
    /// <summary><c>ChangelogApi</c> field.</summary>
    private const string ChangelogApi = "https://changelogs-live.fivem.net/api/changelog/versions/win32/server";

    /// <summary><c>ArtifactBase</c> field.</summary>
    private const string ArtifactBase = "https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/";

    /// <summary>Cfx.re's server download page, which embeds links to every offered Windows build.</summary>
    private const string DownloadPage = "https://docs.fivem.net/docs/server-download/?platform=legacy&os=windows";

    /// <summary>Sub-folder of the server root the FXServer artifact is extracted into.</summary>
    internal const string ArtifactFolder = "artifact";

    /// <summary>Channel names reported by the changelog API, in the order they are listed.</summary>
    private static readonly string[] Channels = { "recommended", "latest", "optional", "critical" };

    public string Id => "cfx";

    /// <summary>Port of <c>CfxProvider.ArtifactLink()</c>.</summary>
    private static string ArtifactLink() => ArtifactBase;

    /// <summary>
    /// Port of <c>CfxProvider.ParseArtifactIndex(string html) -&gt; Dictionary&lt;string, string&gt;</c>:
    /// maps each build number to its artifact directory name (<c>1234-&lt;commit sha&gt;</c>, the
    /// part needed to build the <c>.../&lt;dir&gt;/server.zip</c> download URL). Accepts both the
    /// old Apache-style listing (<c>&lt;a href="16000-abcdef/"&gt;</c>) and the full
    /// <c>.../build_server_windows/master/16000-abcdef/server.7z</c> links on the current
    /// download page.
    /// </summary>
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

    /// <summary>One channel from the changelog API: its build number and artifact directory.</summary>
    internal sealed record Channel(string Name, string Build, string? ArtifactDir);

    /// <summary>
    /// Port of <c>CfxProvider.GetChannelsAsync</c>, extended to every channel the changelog API
    /// reports. Each <c>&lt;channel&gt;_download</c> URL carries the build's artifact directory,
    /// so these builds stay installable even when the download page can't be read.
    /// </summary>
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

    /// <summary>
    /// Port of <c>CfxProvider.GetIndexAsync(CancellationToken)</c>: build number -&gt; artifact
    /// directory for every build the download page links, plus the changelog channels.
    /// </summary>
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

    /// <summary>
    /// Builds the version list: channel builds first ("build 38435 (recommended)"), then every
    /// other build newest first. A build that is several channels at once is labelled with all
    /// of them.
    /// </summary>
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
        // (first run of ASCII digits in request.Version, matching the proven Rust port's
        // split-on-non-digit + first-non-empty-segment semantics)
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
            // Best-effort cleanup, matching the original's fire-and-forget delete.
        }
    }
}
