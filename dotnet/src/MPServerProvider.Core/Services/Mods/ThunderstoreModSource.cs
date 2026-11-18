using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text.Json;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services.Mods;

/// <summary>
/// Searches and downloads mods from a Thunderstore community's package index
/// (https://thunderstore.io/c/{community}/api/v1/package/ - no API key needed). Packages are
/// BepInEx plugin zips, so a download is extracted into its own folder under the server's mods
/// directory rather than copied as a single file.
///
/// No game in <see cref="GameModSupport"/> maps to a community yet - this class exists so the
/// "Install Mods" tab's architecture doesn't need to change once one does (e.g. once Valheim/Aska
/// get BepInEx bootstrapping wired up).
/// </summary>
public sealed class ThunderstoreModSource : IModSource
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(10);

    private readonly HttpClient _http;
    private readonly ConcurrentDictionary<string, (DateTimeOffset At, List<Package> Packages)> _cache = new(StringComparer.OrdinalIgnoreCase);

    public ThunderstoreModSource(HttpClient http) => _http = http;

    public ModSourceKind Kind => ModSourceKind.Thunderstore;

    public bool SupportsGame(string gameId) => GameModSupport.ThunderstoreCommunityFor(gameId) is not null;

    private const int SearchPageSize = 25;

    public Task<IReadOnlyList<ModListing>> SearchAsync(string gameId, string query, int page, CancellationToken ct)
    {
        var community = GameModSupport.ThunderstoreCommunityFor(gameId);
        return community is null
            ? Task.FromResult<IReadOnlyList<ModListing>>(Array.Empty<ModListing>())
            : SearchCommunityAsync(community, query, page, ct);
    }

    /// <summary>The actual search, decoupled from <see cref="GameModSupport"/> so it's testable without a wired-up game.</summary>
    internal async Task<IReadOnlyList<ModListing>> SearchCommunityAsync(string community, string query, int page, CancellationToken ct)
    {
        var packages = await GetPackagesAsync(community, ct).ConfigureAwait(false);
        var q = query.Trim();
        return packages
            .Where(p => !p.IsDeprecated
                        && (q.Length == 0
                            || (p.Name?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)
                            || (p.Owner?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)))
            .OrderByDescending(p => p.Versions?.FirstOrDefault()?.Downloads ?? 0)
            .Skip(page * SearchPageSize)
            .Take(SearchPageSize)
            .Select(ToListing)
            .ToList();
    }

    public async Task<IReadOnlyList<ModFileOption>> GetFilesAsync(string gameId, string modId, CancellationToken ct)
    {
        var package = await FindPackageAsync(gameId, modId, ct).ConfigureAwait(false);
        return package.Versions?
            .Select(v => new ModFileOption(v.VersionNumber ?? "0.0.0", $"{package.Name} {v.VersionNumber}", null, v.DateCreated, v.FileSize))
            .ToList() ?? new List<ModFileOption>();
    }

    public async Task<IReadOnlyList<string>> DownloadAsync(string gameId, string modId, string fileId, string targetDir, CancellationToken ct)
    {
        var package = await FindPackageAsync(gameId, modId, ct).ConfigureAwait(false);
        return await DownloadPackageAsync(package, modId, fileId, targetDir, ct).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<string>> DownloadPackageAsync(Package package, string modId, string fileId, string targetDir, CancellationToken ct)
    {
        var version = package.Versions?.FirstOrDefault(v => v.VersionNumber == fileId)
            ?? throw new InvalidOperationException($"Thunderstore package '{modId}' has no version '{fileId}'.");
        if (string.IsNullOrEmpty(version.DownloadUrl))
        {
            throw new InvalidOperationException($"Thunderstore has no download URL for '{modId}' {fileId}.");
        }

        var zipPath = Path.Combine(Path.GetTempPath(), $"thunderstore-{Guid.NewGuid():N}.zip");
        try
        {
            using (var response = await _http.GetAsync(version.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
                await using var file = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await stream.CopyToAsync(file, ct).ConfigureAwait(false);
            }

            var extractDir = Path.Combine(targetDir, SanitizeFolderName(package.FullName ?? modId));
            if (Directory.Exists(extractDir))
            {
                Directory.Delete(extractDir, recursive: true);
            }
            Directory.CreateDirectory(extractDir);
            ZipFile.ExtractToDirectory(zipPath, extractDir, overwriteFiles: true);

            // Thunderstore's own metadata, not part of the mod itself.
            foreach (var extra in new[] { "manifest.json", "README.md", "icon.png" })
            {
                var path = Path.Combine(extractDir, extra);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            return new[] { extractDir };
        }
        finally
        {
            try
            {
                File.Delete(zipPath);
            }
            catch (IOException)
            {
                // Best-effort temp cleanup.
            }
        }
    }

    private async Task<Package> FindPackageAsync(string gameId, string modId, CancellationToken ct)
    {
        var community = GameModSupport.ThunderstoreCommunityFor(gameId)
            ?? throw new InvalidOperationException("Thunderstore isn't available for this game.");
        return await FindPackageInCommunityAsync(community, modId, ct).ConfigureAwait(false);
    }

    private async Task<Package> FindPackageInCommunityAsync(string community, string modId, CancellationToken ct)
    {
        var packages = await GetPackagesAsync(community, ct).ConfigureAwait(false);
        return packages.FirstOrDefault(p => string.Equals(p.FullName, modId, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Thunderstore package '{modId}' was not found.");
    }

    /// <summary>Test seam: exercises file-listing/download without a <see cref="GameModSupport"/> mapping.</summary>
    internal async Task<IReadOnlyList<ModFileOption>> GetFilesInCommunityAsync(string community, string modId, CancellationToken ct)
    {
        var package = await FindPackageInCommunityAsync(community, modId, ct).ConfigureAwait(false);
        return package.Versions?
            .Select(v => new ModFileOption(v.VersionNumber ?? "0.0.0", $"{package.Name} {v.VersionNumber}", null, v.DateCreated, v.FileSize))
            .ToList() ?? new List<ModFileOption>();
    }

    internal async Task<IReadOnlyList<string>> DownloadInCommunityAsync(string community, string modId, string fileId, string targetDir, CancellationToken ct)
    {
        var package = await FindPackageInCommunityAsync(community, modId, ct).ConfigureAwait(false);
        return await DownloadPackageAsync(package, modId, fileId, targetDir, ct).ConfigureAwait(false);
    }

    private async Task<List<Package>> GetPackagesAsync(string community, CancellationToken ct)
    {
        if (_cache.TryGetValue(community, out var cached) && DateTimeOffset.UtcNow - cached.At < CacheLifetime)
        {
            return cached.Packages;
        }

        var json = await _http.GetStringAsync($"https://thunderstore.io/c/{community}/api/v1/package/", ct).ConfigureAwait(false);
        var packages = JsonSerializer.Deserialize<List<Package>>(json, JsonOptions) ?? new List<Package>();
        _cache[community] = (DateTimeOffset.UtcNow, packages);
        return packages;
    }

    private static ModListing ToListing(Package p) => new(
        ModSourceKind.Thunderstore,
        p.FullName ?? p.Name ?? "unknown",
        p.Name ?? p.FullName ?? "Unknown",
        p.Owner,
        p.Versions?.FirstOrDefault()?.Description,
        p.Versions?.FirstOrDefault()?.Icon,
        p.PackageUrl,
        p.Versions?.FirstOrDefault()?.Downloads);

    private static string SanitizeFolderName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }

    // -- Wire format (Thunderstore's legacy package-index API; snake_case) --------------------

    private sealed class Package
    {
        public string? Name { get; set; }
        public string? FullName { get; set; }
        public string? Owner { get; set; }
        public string? PackageUrl { get; set; }
        public bool IsDeprecated { get; set; }
        public List<PackageVersion>? Versions { get; set; }
    }

    private sealed class PackageVersion
    {
        public string? VersionNumber { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? DownloadUrl { get; set; }
        public long Downloads { get; set; }
        public long FileSize { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
    }
}
