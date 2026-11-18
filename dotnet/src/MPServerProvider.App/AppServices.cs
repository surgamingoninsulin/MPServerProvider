using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using MPServerProvider.Core.Services.Mods;

namespace MPServerProvider.App;

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
    public HytaleAuthAutomation HytaleAuth { get; }
    public ModInstallService ModInstaller { get; }

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
        RunningServerTracker runningTracker,
        HytaleAuthAutomation hytaleAuth,
        ModInstallService modInstaller)
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
        HytaleAuth = hytaleAuth;
        ModInstaller = modInstaller;
    }

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
        // Keeps a Hytale server signed in across restarts: starts the device-code login when the
        // server reports it has no token, and saves it once signed in so it isn't asked again.
        var hytaleAuth = new HytaleAuthAutomation(processes);
        var modHttp = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        modHttp.DefaultRequestHeaders.UserAgent.ParseAdd("MPServerProvider/1.0 (+https://github.com/surgamingoninsulin/MPServerProvider)");
        var modInstaller = new ModInstallService(
            new IModSource[]
            {
                new CurseForgeModSource(modHttp, () => settings.Settings.CurseForgeApiKey),
                new ThunderstoreModSource(modHttp),
                new ModIoModSource(modHttp, () => settings.Settings.ModIoApiKey, () => settings.Settings.ModIoApiHost),
            },
            mods,
            servers);

        // Seeds built-in games on first run and merges any on-disk manifests.
        games.Reload();

        return new AppServices(
            paths, settings, games, servers, installer, processes,
            mods, workshop, rcon, firewall, metrics, artwork, tunnel, news, runningTracker, hytaleAuth, modInstaller);
    }

    private sealed class ServerRepositoryPersistence(ServerRepository repository) : IServerPersistence
    {
        public void Save(ServerInstance server) => repository.Save(server);

        public bool UpdateStats(string serverName, Action<ServerStats> mutate) => repository.UpdateStats(serverName, mutate);
    }

    private sealed class GameLibraryLookup(GameLibraryService games) : IGameLookup
    {
        public GameDefinition? Find(string gameId) => games.Find(gameId);
    }
}
