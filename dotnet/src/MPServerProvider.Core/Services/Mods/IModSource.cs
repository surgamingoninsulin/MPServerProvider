using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services.Mods;

/// <summary>
/// One external mod marketplace (CurseForge, Thunderstore, mod.io, ...). Implementations only
/// search and fetch file metadata/bytes; placing the downloaded file(s) into a server's mods
/// folder is the caller's job (see <see cref="ModInstallService"/>), so each source stays a thin,
/// independently testable HTTP client.
/// </summary>
public interface IModSource
{
    ModSourceKind Kind { get; }

    /// <summary>Whether this source has anything wired up for <paramref name="gameId"/> right now.</summary>
    bool SupportsGame(string gameId);

    /// <summary>Searches, returning one page of up to ~25 results. <paramref name="page"/> is
    /// 0-based; the caller (<see cref="ModInstallService"/>/the Install Mods tab) keeps asking for
    /// the next page as the user scrolls, stopping once a page comes back smaller than it asked for.</summary>
    Task<IReadOnlyList<ModListing>> SearchAsync(string gameId, string query, int page, CancellationToken ct);

    Task<IReadOnlyList<ModFileOption>> GetFilesAsync(string gameId, string modId, CancellationToken ct);

    /// <summary>Downloads the chosen file to <paramref name="targetDir"/> and returns the path(s) written there.</summary>
    Task<IReadOnlyList<string>> DownloadAsync(string gameId, string modId, string fileId, string targetDir, CancellationToken ct);
}
