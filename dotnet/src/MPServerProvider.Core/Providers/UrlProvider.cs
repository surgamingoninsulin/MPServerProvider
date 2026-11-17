using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

public sealed class UrlProvider : IInstallProvider
{
    public string Id => "url";

    public async Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(game.Install.VersionListUrl))
        {
            return new List<string> { "latest" };
        }

        if (string.IsNullOrWhiteSpace(game.Install.VersionListPattern))
        {
            throw new InvalidOperationException($"Game '{game.Id}' sets install.version_list_url but no install.version_list_pattern.");
        }

        var pageText = await DownloadHelper.GetStringAsync(game.Install.VersionListUrl, ct).ConfigureAwait(false);
        var versions = ParseVersionList(pageText, game.Install.VersionListPattern);
        return versions.Count > 0 ? versions : new List<string> { "latest" };
    }

    // The page lists versions newest-first, so this preserves that order for the Version
    // dropdown (which picks index 0 as the default) instead of re-sorting them.
    internal static List<string> ParseVersionList(string pageText, string pattern)
    {
        var regex = new System.Text.RegularExpressions.Regex(
            pattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(2));
        var versions = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (System.Text.RegularExpressions.Match match in regex.Matches(pageText))
        {
            var version = match.Groups["version"].Value;
            if (seen.Add(version))
            {
                versions.Add(version);
            }
        }

        return versions;
    }

    internal static string FileNameFor(string url)
    {
        var uri = DownloadHelper.RequireHttps(url);
        string segment;
        try
        {
            segment = Uri.UnescapeDataString(uri.AbsolutePath);
        }
        catch
        {
            segment = uri.AbsolutePath;
        }

        return DownloadHelper.SafeFileName(segment, "server-download");
    }

    internal static string LatestReleaseApiUrl(string repo)
    {
        var trimmed = (repo ?? string.Empty).Trim().Trim('/');
        var parts = trimmed.Split('/');
        if (parts.Length != 2 || parts.Any(p => p.Length == 0 || !p.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.')))
        {
            throw new InvalidOperationException($"install.github_repo '{repo}' is not an 'owner/repo' name.");
        }

        return $"https://api.github.com/repos/{parts[0]}/{parts[1]}/releases/latest";
    }

    internal static (string Url, string? Tag) PickReleaseAsset(System.Text.Json.JsonElement release, string assetPattern, string repo)
    {
        var regex = new System.Text.RegularExpressions.Regex(
            assetPattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(1));
        var tag = release.TryGetProperty("tag_name", out var tagEl) && tagEl.ValueKind == System.Text.Json.JsonValueKind.String ? tagEl.GetString() : null;

        if (release.TryGetProperty("assets", out var assets) && assets.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var asset in assets.EnumerateArray())
            {
                var name = asset.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : null;
                var url = asset.TryGetProperty("browser_download_url", out var urlEl) ? urlEl.GetString() : null;
                if (name is not null && url is not null && regex.IsMatch(name))
                {
                    DownloadHelper.RequireHttps(url);
                    return (url, tag);
                }
            }
        }

        throw new InvalidOperationException(
            $"The latest {repo} release{(tag is null ? string.Empty : $" ({tag})")} has no download matching '{assetPattern}'.");
    }

    private static async Task<(string Url, string? Version)> ResolveDownloadAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var game = request.Game;
        var install = game.Install;
        if (!string.IsNullOrWhiteSpace(install.GithubRepo))
        {
            if (string.IsNullOrWhiteSpace(install.AssetPattern))
            {
                throw new InvalidOperationException($"Game '{game.Id}' sets install.github_repo but no install.asset_pattern.");
            }

            progress.Report(InstallProgress.Indeterminate($"Finding the latest {install.GithubRepo} release…"));
            var body = await DownloadHelper.GetStringAsync(LatestReleaseApiUrl(install.GithubRepo), ct).ConfigureAwait(false);
            using var doc = System.Text.Json.JsonDocument.Parse(body);
            var (url, tag) = PickReleaseAsset(doc.RootElement, install.AssetPattern, install.GithubRepo);
            return (url, tag);
        }

        if (string.IsNullOrEmpty(install.Url))
        {
            throw new InvalidOperationException($"Custom game '{game.Id}' has no install.url.");
        }

        // A "{version}" placeholder is filled from the Version dropdown's pick (install.
        // version_list_url supplies that list), so the downloaded file always matches what the
        // player actually selected, not just whatever happens to be newest.
        if (install.Url.Contains("{version}", StringComparison.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(request.Version) || request.Version.Equals("latest", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Game '{game.Id}' needs a specific version to download, not 'latest'.");
            }

            var resolvedUrl = install.Url.Replace("{version}", request.Version);
            DownloadHelper.RequireHttps(resolvedUrl);
            return (resolvedUrl, request.Version);
        }

        return (install.Url, null);
    }

    public async Task<InstallResult> InstallAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var (url, version) = await ResolveDownloadAsync(request, progress, ct).ConfigureAwait(false);

        var fileName = FileNameFor(url);
        var downloaded = Path.Combine(request.TargetDir, fileName);

        progress.Report(InstallProgress.Indeterminate($"Downloading {request.Game.Name}…"));
        await DownloadHelper.DownloadFileAsync(url, downloaded, ct).ConfigureAwait(false);

        var archiveKind = (request.Game.Install.Archive ?? "none").ToLowerInvariant();

        switch (archiveKind)
        {
            case "zip":
                progress.Report(InstallProgress.Indeterminate("Extracting server files…"));
                DownloadHelper.ExtractZip(downloaded, request.TargetDir);
                DownloadHelper.TryDeleteFile(downloaded);
                DownloadHelper.FlattenSingleRootFolder(request.TargetDir);
                break;
            case "targz":
                progress.Report(InstallProgress.Indeterminate("Extracting server files…"));
                // `GZipStream` + `TarFile.ExtractToDirectoryAsync`:
                // the tar stream must be decompressed first (TarFile alone only reads plain tar,
                // which made every "targz" install fail with "Invalid tar format").
                await DownloadHelper.ExtractTarGzAsync(downloaded, request.TargetDir, ct).ConfigureAwait(false);
                DownloadHelper.TryDeleteFile(downloaded);
                DownloadHelper.FlattenSingleRootFolder(request.TargetDir);
                break;
            default:
                // "none": leave the downloaded file in place as-is.
                break;
        }

        // `RuntimeSpec.Executable` templates use `{server_root}` (the install target
        // dir) and `{server_file}` (the downloaded archive's own file name, for games whose
        // executable *is* the thing that was downloaded, e.g. a bare `.exe` with
        // `install.archive: "none"`), the same placeholder handling as
        // `SteamCmdProvider.ExeFromDefinition`.
        var serverFile = request.Game.Runtime.Executable
            .Replace("{server_root}", request.TargetDir)
            .Replace("{server_file}", fileName)
            .Trim();

        return new InstallResult
        {
            ServerFile = serverFile,
            Version = version,
            StartCommand = null,
            JavaMajor = 0,
        };
    }
}
