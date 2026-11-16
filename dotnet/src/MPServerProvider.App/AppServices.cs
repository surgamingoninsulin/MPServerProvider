using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App;

/// <summary>
/// Port of <c>MPServerProvider.App.AppServices</c> (decompile token 0x02000005): a composition
/// root holding one instance of every Core service, constructed once at startup and shared for
/// the lifetime of the application. The original is a static service-locator (static
/// get/set-backed fields written once from <c>Initialize()</c>); this port uses plain instance
/// properties on a singleton instance (<see cref="Instance"/>) instead of statics, which is more
/// testable while preserving the same "construct once, hand to everything" shape.
/// <para>
/// Construction order mirrors the decompiled <c>Initialize()</c> call sequence: AppPaths -&gt;
/// SettingsService -&gt; GameLibraryService -&gt; ServerRepository -&gt; InstallService -&gt;
/// ServerProcessService -&gt; QueryService (stateless, nothing to construct) -&gt; ModLibraryService
/// -&gt; WorkshopService -&gt; RconService -&gt; FirewallService -&gt; SystemMetricsService -&gt;
/// ArtworkCacheService -&gt; TunnelService -&gt; NewsService, followed by
/// <c>GameLibraryService.Reload()</c>. This is the same order the already-proven Rust port
/// (<c>crates/mpsp-app/src/app_services.rs</c>) uses, cross-referenced against the decompile's
/// call-edge list in <c>0x02000005_AppServices.md</c>.
/// </para>
/// </summary>
public sealed class AppServices
{
    public AppPaths Paths { get; }
    public SettingsService Settings { get; }
    public GameLibraryService Games { get; }
    public ServerRepository Servers { get; }
    public InstallService Installer { get; }
    public ServerProcessService Processes { get; }
    public ModLibraryService Mods { get; }
    public WorkshopService Workshop { get; }
    public RconService Rcon { get; }
    public FirewallService Firewall { get; }
    public SystemMetricsService Metrics { get; }
    public ArtworkCacheService Artwork { get; }
    public TunnelService Tunnel { get; }
    public NewsService News { get; }

    /// <summary>Remembers which servers were running, for the restore-on-launch setting.</summary>
    public RunningServerTracker RunningTracker { get; }

    private AppServices(
        AppPaths paths,
        SettingsService settings,
        GameLibraryService games,
        ServerRepository servers,
        InstallService installer,
        ServerProcessService processes,
        ModLibraryService mods,
        WorkshopService workshop,
        RconService rcon,
        FirewallService firewall,
        SystemMetricsService metrics,
        ArtworkCacheService artwork,
        TunnelService tunnel,
        NewsService news,
        RunningServerTracker runningTracker)
    {
        Paths = paths;
        Settings = settings;
        Games = games;
        Servers = servers;
        Installer = installer;
        Processes = processes;
        Mods = mods;
        Workshop = workshop;
        Rcon = rcon;
        Firewall = firewall;
        Metrics = metrics;
        Artwork = artwork;
        Tunnel = tunnel;
        News = news;
        RunningTracker = runningTracker;
    }

    /// <summary>
    /// Port of <c>AppServices.Initialize()</c>. Builds every Core service instance in the
    /// decompiled construction order and reloads the game library at the end, exactly like the
    /// original and the proven Rust port.
    /// </summary>
    public static AppServices Initialize()
    {
        var paths = new AppPaths();
        var settings = new SettingsService(paths);
        var games = new GameLibraryService(paths);
        var servers = new ServerRepository(paths);
        var installer = new InstallService(paths, servers);
        var processes = new ServerProcessService(
            new ServerRepositoryPersistence(servers),
            new GameLibraryLookup(games));
        // Deleting a server whose process is alive would delete files under it: the repository
        // refuses while this says the server is busy. Wired here (not by a page) so it holds
        // from startup, whichever screen deletes.
        servers.InUseCheck = server =>
            processes.StateOf(server) is ServerState.Running or ServerState.Starting or ServerState.Stopping
            || processes.IsAutoRestartPending(server);
        var runningTracker = new RunningServerTracker(settings, processes, servers);
        var mods = new ModLibraryService(paths, servers, games);
        var workshop = new WorkshopService(paths);
        var rcon = new RconService();
        var firewall = new FirewallService();
        var metrics = new SystemMetricsService();
        var artwork = new ArtworkCacheService();
        var tunnel = new TunnelService(System.IO.Path.Combine(paths.DataRoot, "tunnel"), paths.ToolsDir);
        // Tunnel "when needed": servers with TunnelSpec.AutoStart get UPnP/playit enabled on
        // start and disabled on stop/delete. Event subscription only; never blocks a start.
        tunnel.AttachAutomation(processes, server => games.Find(server.ServerGame)?.Network?.Protocol, servers);
        var news = new NewsService();

        // Port of the trailing `GameLibraryService.Reload()` call at the end of Initialize():
        // seeds built-in games on first run and merges any on-disk manifests.
        games.Reload();

        return new AppServices(
            paths, settings, games, servers, installer, processes,
            mods, workshop, rcon, firewall, metrics, artwork, tunnel, news, runningTracker);
    }

    /// <summary>
    /// Adapter: implements <see cref="IServerPersistence"/> on top of the real
    /// <see cref="ServerRepository"/> so <see cref="ServerProcessService"/> can persist
    /// uptime/crash stats without Core needing a direct dependency on the process-management
    /// layer's trait. Same trick the Rust port uses (<c>ServerRepositoryPersistence</c> in
    /// <c>app_services.rs</c>), since Core itself doesn't implement these small App-side
    /// interfaces for its repository/catalog types.
    /// </summary>
    private sealed class ServerRepositoryPersistence(ServerRepository repository) : IServerPersistence
    {
        public void Save(ServerInstance server) => repository.Save(server);

        public bool UpdateStats(string serverName, Action<ServerStats> mutate) => repository.UpdateStats(serverName, mutate);
    }

    /// <summary>
    /// Adapter: implements <see cref="IGameLookup"/> on top of the real
    /// <see cref="GameLibraryService"/> so <see cref="ServerProcessService"/> can resolve a
    /// server's <see cref="GameDefinition"/> without a direct Core trait implementation.
    /// </summary>
    private sealed class GameLibraryLookup(GameLibraryService games) : IGameLookup
    {
        public GameDefinition? Find(string gameId) => games.Find(gameId);
    }
}
