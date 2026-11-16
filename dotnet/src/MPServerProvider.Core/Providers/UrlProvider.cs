using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Providers.UrlProvider</c> (decompile token 0x02000027).
/// Installs a custom <see cref="GameDefinition"/> whose <c>install.url</c> points directly at a
/// downloadable archive (<c>.zip</c> or <c>.tar.gz</c>), per <c>install.archive</c>.
/// <para>
/// HARDENING: https only; the local file name is derived safely from the URL path (no query
/// string, no <c>..</c>/backslash traversal out of the server folder); <c>targz</c> archives are
/// actually gunzipped before untarring; the archive is deleted <i>before</i> flattening (so a
/// single wrapper folder is really detected); downloads go through a <c>.part</c> file.
/// </para>
/// </summary>
public sealed class UrlProvider : IInstallProvider
{
    public string Id => "url";

    public Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct)
        => Task.FromResult(new List<string> { "latest" });

    /// <summary>
    /// Derives the local file name for <paramref name="url"/>: the unescaped last segment of the
    /// URL <i>path</i> (query/fragment ignored), or <c>server-download</c> if that is empty or
    /// unsafe. Throws for anything that isn't an absolute https URL.
    /// </summary>
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

    /// <summary>GitHub REST endpoint for a repository's latest (non-prerelease) release.</summary>
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

    /// <summary>
    /// Picks the asset of a GitHub <c>releases/latest</c> response whose <c>name</c> matches
    /// <paramref name="assetPattern"/>. Returns the release tag and the asset's
    /// <c>browser_download_url</c> (https only).
    /// </summary>
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

    /// <summary>Resolves the download URL: <c>install.url</c>, or the latest GitHub release asset.</summary>
    private static async Task<(string Url, string? Version)> ResolveDownloadAsync(GameDefinition game, IProgress<InstallProgress> progress, CancellationToken ct)
    {
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

        return (install.Url, null);
    }

    public async Task<InstallResult> InstallAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var (url, version) = await ResolveDownloadAsync(request.Game, progress, ct).ConfigureAwait(false);

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
                // The decompiled body shows `GZipStream` + `TarFile.ExtractToDirectoryAsync`:
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

        // INFERRED: `RuntimeSpec.Executable` templates use `{server_root}` (the install target
        // dir) and `{server_file}` (the downloaded archive's own file name, for games whose
        // executable *is* the thing that was downloaded, e.g. a bare `.exe` with
        // `install.archive: "none"`) — this mirrors `SteamCmdProvider.ExeFromDefinition`'s
        // identical placeholder handling for the one call edge this method shares with it
        // (`RuntimeSpec.get_Executable` + `String.Replace` + `String.Trim`).
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
