using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.Core.Services.Mods;

/// <summary>Result of <see cref="ModInstallService.InstallAsync"/>, covering both install styles
/// (a file dropped into the mods folder, or a mod ID added to the launch arguments).</summary>
public sealed record ModInstallOutcome(bool Success, string Message);

/// <summary>
/// The "Install Mods" tab's single entry point. Most games get a mod by downloading the chosen
/// file and handing it to <see cref="ModLibraryService"/>, so placement, compatibility checks
/// (Minecraft loader/version, etc.) and duplicate detection all go through the same path as a
/// manual mod drop. Games in <see cref="GameModSupport.LaunchArgFlagFor"/> instead get the mod's
/// ID merged into their launch arguments - no file to download, since the dedicated server fetches
/// the mod itself on its next start.
/// </summary>
public sealed class ModInstallService
{
    private readonly IReadOnlyList<IModSource> _sources;
    private readonly ModLibraryService _mods;
    private readonly ServerRepository _servers;

    public ModInstallService(IReadOnlyList<IModSource> sources, ModLibraryService mods, ServerRepository servers)
    {
        _sources = sources;
        _mods = mods;
        _servers = servers;
    }

    public IReadOnlyList<ModSourceKind> SourcesFor(string gameId) =>
        _sources.Where(s => s.SupportsGame(gameId)).Select(s => s.Kind).ToList();

    /// <summary>True if installing for this game means adding its mod ID to the launch arguments
    /// rather than downloading and placing a file - see <see cref="GameModSupport.LaunchArgFlagFor"/>.</summary>
    public bool IsLaunchArgInstall(string gameId) => GameModSupport.LaunchArgFlagFor(gameId) is not null;

    /// <summary>Mod IDs already in <paramref name="server"/>'s launch arguments for a launch-arg
    /// install game - there's no file on disk for these, so the regular Mods tab/File Manager
    /// can't show them; this is how the Install Mods tab marks a result "already installed".</summary>
    public IReadOnlyList<string> GetLaunchArgModIds(ServerInstance server)
    {
        var flag = GameModSupport.LaunchArgFlagFor(server.ServerGame);
        return flag is null ? Array.Empty<string>() : GameModSupport.ParseLaunchArgModIds(server.StartCommand, flag);
    }

    public Task<IReadOnlyList<ModListing>> SearchAsync(ModSourceKind source, string gameId, string query, int page, CancellationToken ct) =>
        Resolve(source).SearchAsync(gameId, query, page, ct);

    public Task<IReadOnlyList<ModFileOption>> GetFilesAsync(ModSourceKind source, string gameId, string modId, CancellationToken ct) =>
        Resolve(source).GetFilesAsync(gameId, modId, ct);

    /// <summary>Launch-arg install: adds <paramref name="modId"/> to the server's launch
    /// arguments and saves it. No source/network call needed - the server fetches it itself.</summary>
    public ModInstallOutcome InstallByLaunchArg(ServerInstance server, string modId)
    {
        var flag = GameModSupport.LaunchArgFlagFor(server.ServerGame)
            ?? throw new InvalidOperationException($"{server.ServerGame} doesn't install mods through launch arguments.");

        var before = server.StartCommand ?? string.Empty;
        var after = GameModSupport.MergeModIdIntoLaunchArgs(before, flag, modId);
        if (after == before && before.Contains(modId, StringComparison.OrdinalIgnoreCase))
        {
            return new ModInstallOutcome(true, "That mod is already in this server's launch arguments.");
        }

        server.StartCommand = after;
        _servers.Save(server);
        return new ModInstallOutcome(true,
            $"Added to {server.ServerName}'s launch arguments ({flag.TrimEnd('=')}). Restart the server to download and load it.");
    }

    public async Task<ModInstallOutcome> InstallAsync(ModSourceKind source, ServerInstance server, string modId, string fileId, CancellationToken ct)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"mpsp-mod-install-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        try
        {
            var paths = await Resolve(source).DownloadAsync(server.ServerGame, modId, fileId, tempDir, ct).ConfigureAwait(false);
            var result = _mods.InstallFilesToServer(paths, server);
            if (result.Added.Count > 0 || result.Replaced.Count > 0)
            {
                return new ModInstallOutcome(true, $"Installed to {server.ServerName}. Restart the server to load it.");
            }
            if (result.Skipped.Count > 0)
            {
                return new ModInstallOutcome(false, result.Skipped[0].Reason);
            }
            return new ModInstallOutcome(false, "Nothing to install.");
        }
        finally
        {
            try
            {
                Directory.Delete(tempDir, recursive: true);
            }
            catch (IOException)
            {
                // Best-effort temp cleanup.
            }
            catch (UnauthorizedAccessException)
            {
                // Best-effort temp cleanup.
            }
        }
    }

    private IModSource Resolve(ModSourceKind kind) =>
        _sources.FirstOrDefault(s => s.Kind == kind)
        ?? throw new InvalidOperationException($"No mod source is registered for {kind}.");
}
