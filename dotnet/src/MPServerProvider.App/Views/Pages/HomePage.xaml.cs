using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using MPServerProvider.App.Controls;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Views.Pages;

/// <summary>
/// Port of <c>views_homepage.xaml</c>'s real data wiring: hero banner CTAs navigate the shell,
/// stat tiles pull from the real Core services (<see cref="ServerProcessService"/> for
/// uptime/running counts, <see cref="ModLibraryService"/> for installed mod count, a best-effort
/// live query pass for total players), and the news carousel rides on the real
/// <see cref="NewsService.Load"/> feed instead of the wave-1 placeholder rows.
/// </summary>
public partial class HomePage : UserControl, INavigationAware
{
    public HomePage()
    {
        InitializeComponent();
    }

    public void OnNavigatedTo() => Refresh();

    private AppServices? Services => (Application.Current as App)?.Services;

    private void Refresh()
    {
        var services = Services;
        if (services is null)
        {
            return;
        }

        var newsCards = services.News.Load()
            .Select(item => new NewsCardViewModel(item, item.GameId is { Length: > 0 } id ? services.Games.Find(id) : null))
            .ToList();
        NewsScroller.ItemsSource = newsCards;
        foreach (var card in newsCards)
        {
            _ = LoadNewsCoverAsync(card, services);
        }

        var servers = services.Servers.Servers;
        var running = servers.Where(s => services.Processes.StateOf(s) is ServerState.Running or ServerState.Starting).ToList();

        var totalUptime = TimeSpan.Zero;
        foreach (var server in running)
        {
            totalUptime += services.Processes.UptimeOf(server);
        }
        UptimeStat.Text = $"{(int)totalUptime.TotalHours}h {totalUptime.Minutes}m";

        ServersShortcutText.Text = $"{running.Count} running";
        RefreshInstalledServers(services, servers);

        var modsCount = 0;
        try
        {
            modsCount = services.Mods.Scan().Count;
        }
        catch
        {
            // Best-effort; mod scan can fail if a server's mods folder is missing/locked.
        }
        ModsShortcutText.Text = $"{modsCount} installed";

        PlayersStat.Text = "…";
        _ = RefreshPlayerCountAsync(services, running);
    }

    private ServerInstance? _lastInstalled;

    /// <summary>
    /// "Servers installed" count (with a per-game breakdown) and the most recently installed
    /// server. Servers carry no install timestamp, so the creation time of their
    /// <c>server.json</c> (written once, at install) is used.
    /// </summary>
    private void RefreshInstalledServers(AppServices services, IReadOnlyList<ServerInstance> servers)
    {
        InstalledCountText.Text = servers.Count.ToString();
        InstalledBreakdownText.Text = servers.Count == 0
            ? "Install one from the Library"
            : string.Join(" · ", servers
                .GroupBy(s => services.Games.Find(s.ServerGame)?.Name ?? s.ServerGame)
                .OrderByDescending(g => g.Count())
                .Select(g => $"{g.Count()} {g.Key}"));

        _lastInstalled = servers
            .Select(s => (Server: s, Installed: InstalledAt(s)))
            .OrderByDescending(x => x.Installed)
            .Select(x => x.Server)
            .FirstOrDefault();

        if (_lastInstalled is not { } last)
        {
            LastInstalledName.Text = "None yet";
            LastInstalledDetail.Text = "Pick a game in the Library to install your first server.";
            LastInstalledArt.Background = (Brush)FindResource("PanelBackgroundBrush");
            return;
        }

        var game = services.Games.Find(last.ServerGame);
        LastInstalledName.Text = last.ServerName;
        var version = string.IsNullOrWhiteSpace(last.ServerVersion) ? string.Empty : $" {last.ServerVersion}";
        LastInstalledDetail.Text = $"{game?.Name ?? last.ServerGame}{version} · port {last.ServerPort} · installed {Ago(InstalledAt(last))}";
        LastInstalledArt.Background = ArtworkImages.AccentBrush(game?.Artwork.Accent);
        if (game is not null)
        {
            _ = LoadLastInstalledArtAsync(services, game);
        }
    }

    private async Task LoadLastInstalledArtAsync(AppServices services, GameDefinition game)
    {
        try
        {
            var bitmap = await ArtworkImages.LoadAsync(services, $"{game.Id}_header", game.Artwork.Cover, ArtworkImages.PixelWidth(this, 96)).ConfigureAwait(true);
            if (bitmap is not null)
            {
                var brush = new ImageBrush(bitmap) { Stretch = Stretch.UniformToFill };
                brush.Freeze();
                LastInstalledArt.Background = brush;
            }
        }
        catch
        {
            // Accent tile stays.
        }
    }

    private DateTime InstalledAt(ServerInstance server)
    {
        try
        {
            var root = Services?.Servers.RootFor(server) ?? server.ServerRoot;
            var manifest = Path.Combine(root, "server.json");
            if (File.Exists(manifest))
            {
                return File.GetCreationTime(manifest);
            }

            return Directory.Exists(root) ? Directory.GetCreationTime(root) : DateTime.MinValue;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    private static string Ago(DateTime when)
    {
        if (when == DateTime.MinValue)
        {
            return "a while ago";
        }

        var span = DateTime.Now - when;
        return span.TotalMinutes < 1 ? "just now"
            : span.TotalHours < 1 ? $"{(int)span.TotalMinutes} min ago"
            : span.TotalDays < 1 ? $"{(int)span.TotalHours} h ago"
            : span.TotalDays < 2 ? "yesterday"
            : span.TotalDays < 30 ? $"{(int)span.TotalDays} days ago"
            : when.ToString("d MMM yyyy");
    }

    private void LastInstalledCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (_lastInstalled is null)
            {
                NavigateShell("library");
                return;
            }

            NavigateShell("servers");
            (Window.GetWindow(this) as MainWindow)?.SelectServer(_lastInstalled.ServerName);
        }
        catch (Exception ex)
        {
            (Window.GetWindow(this) as MainWindow)?.Toasts.Error($"Couldn't open that server: {ex.Message}");
        }
    }

    /// <summary>
    /// Best-effort live player-count query across every running server, matching the proven
    /// Rust port's judgment that a per-server A2S/Minecraft/RakNet query (via
    /// <see cref="QueryService.QueryAsync"/>) is the only real source of truth for this stat
    /// (nothing in <see cref="ServerStats"/> tracks concurrent players). Failures for any one
    /// server are swallowed so a single unreachable server can't blank out the whole tile.
    /// </summary>
    private async Task RefreshPlayerCountAsync(AppServices services, List<ServerInstance> running)
    {
        var total = 0;
        foreach (var server in running)
        {
            var game = services.Games.Find(server.ServerGame);
            var kind = game?.Network.Query ?? "none";
            if (kind is "none" or "")
            {
                continue;
            }

            try
            {
                var host = server.ServerIp is "0.0.0.0" or "" ? "127.0.0.1" : server.ServerIp;
                var result = await QueryService.QueryAsync(kind, host, server.ServerPort).ConfigureAwait(true);
                if (result is { } r)
                {
                    total += r.Players;
                }
            }
            catch
            {
                // Best-effort; an unreachable/timed-out server just doesn't contribute.
            }
        }

        PlayersStat.Text = total.ToString();
    }

    /// <summary>
    /// Port of <c>NewsCardVm.LoadCoverAsync</c>: resolves the news item's associated game's
    /// cover art the same way <c>LibraryPage.LoadCoverAsync</c> does (same <c>{id}_header</c>
    /// cache key, so the file is shared), so the sliding news carousel shows real game artwork
    /// per card — the real app's own news row (see <c>docs/reference-screenshots/1.png</c>)
    /// always shows the associated game's banner. Falls back to <see cref="NewsItem.Image"/>
    /// (cached under a stable SHA-256-derived key, not the per-process-randomized
    /// <see cref="string.GetHashCode"/>) when there's no matching game. Decoded off the UI
    /// thread at card width; on any failure the card keeps its accent tile.
    /// </summary>
    private async Task LoadNewsCoverAsync(NewsCardViewModel card, AppServices services)
    {
        try
        {
            var game = card.Game;
            var url = game?.Artwork.Cover ?? card.Item.Image;
            var key = game is not null ? $"{game.Id}_header" : ArtworkImages.StableKey("news", url ?? string.Empty);
            var bitmap = await ArtworkImages.LoadAsync(services, key, url, ArtworkImages.PixelWidth(this, NewsCardWidth)).ConfigureAwait(true);
            if (bitmap is null && game is not null && !string.IsNullOrWhiteSpace(card.Item.Image) && card.Item.Image != url)
            {
                bitmap = await ArtworkImages.LoadAsync(services, ArtworkImages.StableKey("news", card.Item.Image), card.Item.Image, ArtworkImages.PixelWidth(this, NewsCardWidth)).ConfigureAwait(true);
            }

            card.CoverImage = bitmap;
        }
        catch
        {
            // Best-effort; the card keeps its accent tile.
        }
    }

    private const double NewsCardWidth = 340;

    private object? _pressedNewsCard;

    private void NewsCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        => _pressedNewsCard = (sender as FrameworkElement)?.DataContext;

    /// <summary>Clicking a news card opens the Game Detail page of the game it announces.</summary>
    private void NewsCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        try
        {
            var card = (sender as FrameworkElement)?.DataContext as NewsCardViewModel;
            var pressed = ReferenceEquals(_pressedNewsCard, card);
            _pressedNewsCard = null;
            if (!pressed || card?.Game is not { } game)
            {
                return;
            }

            e.Handled = true;
            (Window.GetWindow(this) as MainWindow)?.ShowGameDetail(game, "home");
        }
        catch (Exception ex)
        {
            (Window.GetWindow(this) as MainWindow)?.Toasts.Error($"Couldn't open that game: {ex.Message}");
        }
    }

    private void ManageServersButton_Click(object sender, RoutedEventArgs e) => NavigateShell("servers");

    private void BrowseGamesButton_Click(object sender, RoutedEventArgs e) => NavigateShell("library");

    private void NavigateShell(string pageKey)
    {
        if (Window.GetWindow(this) is not MainWindow main)
        {
            return;
        }

        switch (pageKey)
        {
            case "servers":
                main.NavServers.IsChecked = true;
                break;
            case "library":
                main.NavLibrary.IsChecked = true;
                break;
        }
    }

    /// <summary>Bindable wrapper exposing <see cref="NewsItem"/>'s title/summary plus a
    /// lazily-resolved <see cref="CoverImage"/> for the carousel's <c>Image</c> binding.</summary>
    private sealed partial class NewsCardViewModel : ObservableObject
    {
        public NewsCardViewModel(NewsItem item, GameDefinition? game)
        {
            Item = item;
            Game = game;
            AccentBrush = ArtworkImages.AccentBrush(game?.Artwork.Accent);
        }

        public NewsItem Item { get; }

        /// <summary>The catalog game this item announces, if <see cref="NewsItem.GameId"/> resolves.</summary>
        public GameDefinition? Game { get; }

        public bool CanOpen => Game is not null;

        public Brush AccentBrush { get; }

        public string Tag => string.IsNullOrWhiteSpace(Item.Tag) ? "News" : Item.Tag;

        public string Title => Item.Title;

        public string Summary => Item.Summary;

        [ObservableProperty]
        private BitmapSource? _coverImage;
    }
}
