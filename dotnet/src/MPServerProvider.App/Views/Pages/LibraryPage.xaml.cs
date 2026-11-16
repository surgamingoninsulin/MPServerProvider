using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MPServerProvider.App.Controls;
using MPServerProvider.App.Views.Dialogs;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Views.Pages;

/// <summary>
/// Port of <c>views_librarypage.xaml</c>/<c>LibraryPage</c> (decompile token 0x0200000e) +
/// <c>GameCardVm</c> (0x0200000a): search box + genre filter pills over
/// <see cref="GameLibraryService.Games"/>, a real card grid with async cover-art loading, and
/// "Install"/"+ Add Custom Game" wiring the three install-flow dialogs. Genre pills and cards are
/// built programmatically since their content is fully catalog-driven (see
/// <c>crates/mpsp-app/src/pages/library.rs</c>'s <c>apply_filter</c>/<c>to_card</c>, the proven
/// reference for this page's data flow).
/// <para>
/// Performance: the catalog is NOT reloaded from disk on every visit (AppServices.Initialize and
/// AddCustomGameDialog already reload it); cards are built once per <see cref="GameDefinition"/>
/// instance and cached, filtering only toggles their visibility, the search box is debounced,
/// and covers are decoded once, off the UI thread, at card size.
/// </para>
/// <para>
/// Each card shows a 2-line trimmed description; clicking the card body (anything but the
/// Install button) opens <see cref="GameDetailPage"/> via <see cref="MainWindow.ShowGameDetail"/>.
/// </para>
/// </summary>
public partial class LibraryPage : UserControl, INavigationAware
{
    private const double CardWidth = 280;

    private readonly List<ToggleButton> _genreButtons = new();
    private readonly Dictionary<GameDefinition, GameCard> _cards = new(ReferenceEqualityComparer.Instance);
    private readonly DispatcherTimer _searchDebounce;
    private List<GameDefinition> _snapshot = new();
    private string _activeGenre = string.Empty;

    public LibraryPage()
    {
        InitializeComponent();
        _searchDebounce = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
        _searchDebounce.Tick += (_, _) =>
        {
            _searchDebounce.Stop();
            ApplyFilter();
        };
    }

    public void OnNavigatedTo() => Refresh();

    private AppServices? Services => (Application.Current as App)?.Services;

    private MainWindow? Main => Window.GetWindow(this) as MainWindow;

    private static Brush Res(string key) => (Brush)Application.Current.Resources[key];

    /// <summary>
    /// Syncs the cached cards with the service's current catalog. Cheap when nothing changed
    /// (same <see cref="GameDefinition"/> instances): only the filter is re-applied.
    /// </summary>
    private void Refresh()
    {
        try
        {
            var services = Services;
            if (services is null)
            {
                return;
            }

            var games = services.Games.Games;
            var unchanged = games.Count == _snapshot.Count
                && games.Zip(_snapshot).All(pair => ReferenceEquals(pair.First, pair.Second));
            if (!unchanged)
            {
                RebuildCards(games, services);
                BuildGenrePanel(services);
            }

            ApplyFilter();
            HookStatusUpdates(services);
            RefreshStatusBadges(services, checkUpdates: true);
        }
        catch (Exception ex)
        {
            Main?.Toasts.Error($"Couldn't load the game library: {ex.Message}");
        }
    }

    private void RebuildCards(List<GameDefinition> games, AppServices services)
    {
        _snapshot = games;
        var keep = new HashSet<GameDefinition>(games, ReferenceEqualityComparer.Instance);
        foreach (var stale in _cards.Keys.Where(g => !keep.Contains(g)).ToList())
        {
            _cards.Remove(stale);
        }

        CardsPanel.Items.Clear();
        foreach (var game in games)
        {
            if (!_cards.TryGetValue(game, out var card))
            {
                card = BuildCard(game);
                _cards[game] = card;
                _ = LoadCoverAsync(card, services);
            }

            CardsPanel.Items.Add(card.Root);
        }
    }

    private void BuildGenrePanel(AppServices services)
    {
        _genreButtons.Clear();
        GenrePanel.Items.Clear();

        var genres = services.Games.AllGenres();
        if (!string.IsNullOrEmpty(_activeGenre) && !genres.Contains(_activeGenre))
        {
            _activeGenre = string.Empty;
        }

        var allButton = MakeGenrePill("All Games", string.Empty);
        GenrePanel.Items.Add(allButton);
        _genreButtons.Add(allButton);

        foreach (var genre in genres)
        {
            var button = MakeGenrePill(genre, genre);
            GenrePanel.Items.Add(button);
            _genreButtons.Add(button);
        }

        var active = _genreButtons.FirstOrDefault(b => (b.Tag as string) == _activeGenre) ?? allButton;
        active.IsChecked = true;
    }

    private ToggleButton MakeGenrePill(string label, string genreValue)
    {
        var button = new ToggleButton
        {
            Content = label,
            Tag = genreValue,
            Margin = new Thickness(0, 0, 8, 8),
            Padding = new Thickness(14, 7, 14, 7),
            Background = Res("PanelBackgroundBrush"),
            Foreground = Res("TextSecondaryBrush"),
            BorderThickness = new Thickness(0),
            Cursor = Cursors.Hand,
        };
        button.Template = BuildPillTemplate();
        button.Checked += GenrePill_Checked;
        return button;
    }

    private static ControlTemplate BuildPillTemplate()
    {
        var template = new ControlTemplate(typeof(ToggleButton));
        var border = new FrameworkElementFactory(typeof(Border));
        border.Name = "PillBg";
        border.SetValue(Border.BackgroundProperty, new System.Windows.Data.Binding("Background") { RelativeSource = System.Windows.Data.RelativeSource.TemplatedParent });
        border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
        border.SetValue(Border.PaddingProperty, new System.Windows.Data.Binding("Padding") { RelativeSource = System.Windows.Data.RelativeSource.TemplatedParent });
        var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
        presenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        presenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        border.AppendChild(presenter);
        template.VisualTree = border;

        var checkedTrigger = new Trigger { Property = ToggleButton.IsCheckedProperty, Value = true };
        checkedTrigger.Setters.Add(new Setter(Border.BackgroundProperty, Application.Current.Resources["BrandAccentBrush"], "PillBg"));
        checkedTrigger.Setters.Add(new Setter(ToggleButton.ForegroundProperty, Brushes.White));
        template.Triggers.Add(checkedTrigger);

        return template;
    }

    private void GenrePill_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton checkedButton)
        {
            return;
        }

        foreach (var button in _genreButtons)
        {
            if (!ReferenceEquals(button, checkedButton))
            {
                button.IsChecked = false;
            }
        }

        _activeGenre = checkedButton.Tag as string ?? string.Empty;
        ApplyFilter();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Debounced: typing a word re-filters once, ~300 ms after the last keystroke.
        _searchDebounce.Stop();
        _searchDebounce.Start();
    }

    /// <summary>Filters the cached cards in place by toggling their visibility.</summary>
    private void ApplyFilter()
    {
        var search = SearchBox.Text?.Trim() ?? string.Empty;
        var genre = _activeGenre;
        var visible = 0;

        foreach (var game in _snapshot)
        {
            if (!_cards.TryGetValue(game, out var card))
            {
                continue;
            }

            var genreOk = string.IsNullOrEmpty(genre) || game.Genres.Contains(genre);
            var searchOk = search.Length == 0
                || game.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || game.Genres.Any(x => x.Contains(search, StringComparison.OrdinalIgnoreCase))
                || (game.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false);
            var show = genreOk && searchOk;
            card.Root.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            if (show)
            {
                visible++;
            }
        }

        CountText.Text = $"{visible} game{(visible == 1 ? "" : "s")} available";
    }

    private GameCard BuildCard(GameDefinition game)
    {
        var accent = ArtworkImages.AccentBrush(game.Artwork.Accent);
        var root = new InvokableCard
        {
            Style = (Style)Application.Current.Resources["CardStyle"],
            Width = CardWidth,
            Margin = new Thickness(0, 0, 16, 16),
            Padding = new Thickness(0),
            ClipToBounds = true,
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Transparent,
            Cursor = Cursors.Hand,
            ToolTip = $"View {game.Name} details",
        };
        System.Windows.Automation.AutomationProperties.SetName(root, $"View {game.Name} details");
        root.Invoked += (_, _) => OpenDetail(game);
        root.GotKeyboardFocus += (_, _) => root.BorderBrush = Res("BrandAccentBrush");
        root.LostKeyboardFocus += (_, _) => root.BorderBrush = Brushes.Transparent;

        var stack = new StackPanel();

        var artHost = new Grid { Height = 150 };
        var fallback = new Border { Background = accent };
        fallback.Child = new TextBlock
        {
            Text = "",
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            FontSize = 42,
            Foreground = Brushes.White,
            Opacity = 0.85,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        var image = new Image { Stretch = Stretch.UniformToFill, Visibility = Visibility.Collapsed };
        artHost.Children.Add(fallback);
        artHost.Children.Add(image);

        // Status badge (top-right): only shown once a server of this game is installed.
        var statusDot = new System.Windows.Shapes.Ellipse { Width = 9, Height = 9, VerticalAlignment = VerticalAlignment.Center };
        var statusText = new TextBlock { Foreground = Brushes.White, FontSize = 11, FontWeight = FontWeights.SemiBold, Margin = new Thickness(6, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
        var updateDot = new System.Windows.Shapes.Ellipse
        {
            Width = 9,
            Height = 9,
            Fill = UpdateBrush,
            Margin = new Thickness(8, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Visibility = Visibility.Collapsed,
        };
        var badgeRow = new StackPanel { Orientation = Orientation.Horizontal };
        badgeRow.Children.Add(statusDot);
        badgeRow.Children.Add(statusText);
        badgeRow.Children.Add(updateDot);
        var badge = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(0xCC, 0x05, 0x07, 0x0D)),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(8, 3, 9, 4),
            Margin = new Thickness(8),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Visibility = Visibility.Collapsed,
            Child = badgeRow,
        };
        artHost.Children.Add(badge);

        var infoPanel = new StackPanel { Margin = new Thickness(14, 10, 14, 12) };
        infoPanel.Children.Add(new TextBlock
        {
            Text = game.Name,
            Foreground = Res("TextPrimaryBrush"),
            FontWeight = FontWeights.SemiBold,
            FontSize = 14,
            TextTrimming = TextTrimming.CharacterEllipsis,
        });

        // Short description, trimmed with an ellipsis after two lines. Fixed height keeps every
        // card in the grid the same size whether the text is one line or ten.
        infoPanel.Children.Add(new TextBlock
        {
            Text = game.Description ?? string.Empty,
            Foreground = Res("TextSecondaryBrush"),
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            TextTrimming = TextTrimming.CharacterEllipsis,
            LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
            LineHeight = 17,
            Height = 34,
            Margin = new Thickness(0, 5, 0, 0),
            ToolTip = string.IsNullOrWhiteSpace(game.Description) ? null : game.Description,
        });

        var bottomRow = new Grid { Margin = new Thickness(0, 10, 0, 0) };
        bottomRow.ColumnDefinitions.Add(new ColumnDefinition());
        bottomRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var genreText = new TextBlock
        {
            Text = string.Join("  •  ", game.Genres),
            Foreground = Res("TextMutedBrush"),
            FontSize = 11,
            VerticalAlignment = VerticalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
        };

        var installButton = new Button
        {
            Content = "Install",
            Style = (Style)Application.Current.Resources["AccentButtonStyle"],
            Padding = new Thickness(14, 6, 14, 6),
            FontSize = 12,
        };
        installButton.Click += (_, e) =>
        {
            e.Handled = true;
            OnInstallClicked(game);
        };
        Grid.SetColumn(installButton, 1);

        bottomRow.Children.Add(genreText);
        bottomRow.Children.Add(installButton);
        infoPanel.Children.Add(bottomRow);

        stack.Children.Add(artHost);
        stack.Children.Add(infoPanel);
        root.Child = stack;

        // Card-body click -> Game Detail. The Install button handles its own mouse down, so a
        // press that started on it never sets this flag.
        var pressed = false;
        root.MouseLeftButtonDown += (_, _) => pressed = true;
        root.MouseLeave += (_, _) =>
        {
            pressed = false;
            root.BorderBrush = Brushes.Transparent;
        };
        root.MouseEnter += (_, _) => root.BorderBrush = Res("BrandAccentBrush");
        root.MouseLeftButtonUp += (_, e) =>
        {
            if (!pressed || IsInsideButton(e.OriginalSource as DependencyObject))
            {
                pressed = false;
                return;
            }

            pressed = false;
            OpenDetail(game);
        };

        return new GameCard(game, root, image, fallback, new CardBadge(badge, statusDot, statusText, updateDot));
    }

    private static bool IsInsideButton(DependencyObject? element)
    {
        while (element is not null)
        {
            if (element is ButtonBase)
            {
                return true;
            }

            element = element is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(element)
                : LogicalTreeHelper.GetParent(element);
        }

        return false;
    }

    private void OpenDetail(GameDefinition game)
    {
        try
        {
            Main?.ShowGameDetail(game, "library");
        }
        catch (Exception ex)
        {
            Main?.Toasts.Error($"Couldn't open {game.Name}: {ex.Message}");
        }
    }

    private void OnInstallClicked(GameDefinition game)
    {
        var main = Main;
        try
        {
            if (Services is not { } services)
            {
                return;
            }

            InstallChoiceDialog.Run(services, main?.Toasts, game, main);
        }
        catch (Exception ex)
        {
            main?.Toasts.Error($"Couldn't start the install: {ex.Message}");
        }
    }

    private void AddCustomGameButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (Services is not { } services)
            {
                return;
            }

            var main = Main;
            new AddCustomGameDialog(services, main?.Toasts) { Owner = main }.ShowDialog();
            Refresh(); // the dialog reloads the catalog itself when a game is added
        }
        catch (Exception ex)
        {
            Main?.Toasts.Error($"Couldn't add the game: {ex.Message}");
        }
    }

    /// <summary>
    /// Port of <c>GameCardVm.LoadCoverAsync</c>: resolves the game's cover art (cache-or-fetch
    /// via <see cref="ArtworkCacheService.GetLocalPathAsync"/>, decoded off the UI thread at card
    /// width) and swaps the card's fallback accent tile for the real image once it resolves.
    /// </summary>
    private async Task LoadCoverAsync(GameCard card, AppServices services)
    {
        try
        {
            var bitmap = await ArtworkImages.LoadAsync(
                services,
                $"{card.Game.Id}_header",
                card.Game.Artwork.Cover,
                ArtworkImages.PixelWidth(this, CardWidth)).ConfigureAwait(true);
            if (bitmap is not null)
            {
                card.Image.Source = bitmap;
                card.Image.Visibility = Visibility.Visible;
                card.Fallback.Visibility = Visibility.Collapsed;
            }
        }
        catch
        {
            // Best-effort; the flat accent tile stays as the fallback on any failure.
        }
    }

    /// <summary>A built card plus the image/fallback elements its cover load swaps.</summary>
    private sealed record GameCard(GameDefinition Game, Border Root, Image Image, Border Fallback, CardBadge Badge);

    private sealed record CardBadge(Border Root, System.Windows.Shapes.Ellipse StatusDot, TextBlock StatusText, System.Windows.Shapes.Ellipse UpdateDot);

    // ---- server status badges ------------------------------------------------------------
    // Per game: green = online, yellow = restarting/stopping (or waiting to auto-restart after
    // a crash), blinking yellow = starting, red = crashed, gray = installed but off. A purple
    // dot next to it means a newer server version is available (ServerUpdateCheck).

    private static readonly Brush OnlineBrush = Frozen(0x22, 0xC5, 0x5E);
    private static readonly Brush BusyBrush = Frozen(0xF5, 0x9E, 0x0B);
    private static readonly Brush CrashedBrush = Frozen(0xEF, 0x44, 0x44);
    private static readonly Brush OffBrush = Frozen(0x8B, 0x93, 0xA7);
    private static readonly Brush UpdateBrush = Frozen(0xA8, 0x55, 0xF7);

    private static Brush Frozen(byte r, byte g, byte b)
    {
        var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
        brush.Freeze();
        return brush;
    }

    private bool _statusHooked;

    private void HookStatusUpdates(AppServices services)
    {
        if (_statusHooked)
        {
            return;
        }

        _statusHooked = true;
        // StateChanged fires on process threads; marshal to the UI thread.
        services.Processes.StateChanged += (_, _) => Dispatcher.BeginInvoke(() =>
        {
            if (IsVisible && Services is { } s)
            {
                RefreshStatusBadges(s, checkUpdates: false);
            }
        });
    }

    private void RefreshStatusBadges(AppServices services, bool checkUpdates)
    {
        var servers = services.Servers.Servers;
        foreach (var card in _cards.Values)
        {
            try
            {
                var mine = servers.Where(s => s.ServerGame == card.Game.Id).ToList();
                var badge = card.Badge;
                if (mine.Count == 0)
                {
                    badge.Root.Visibility = Visibility.Collapsed;
                    continue;
                }

                var states = mine.Select(s => (State: services.Processes.StateOf(s), Pending: services.Processes.IsAutoRestartPending(s))).ToList();
                var (brush, label, blink) =
                    states.Any(x => x.State == ServerState.Crashed && !x.Pending) ? (CrashedBrush, "Crashed", false)
                    : states.Any(x => x.State is ServerState.Starting or ServerState.Installing) ? (BusyBrush, "Starting", true)
                    : states.Any(x => x.State == ServerState.Stopping || x.Pending) ? (BusyBrush, "Restarting", false)
                    : states.Any(x => x.State == ServerState.Running) ? (OnlineBrush, "Online", false)
                    : (OffBrush, "Off", false);
                var online = states.Count(x => x.State == ServerState.Running);
                if (label == "Online" && mine.Count > 1)
                {
                    label = $"{online}/{mine.Count} online";
                }
                else if (mine.Count > 1)
                {
                    label += $" · {mine.Count} servers";
                }

                badge.StatusDot.Fill = brush;
                badge.StatusText.Text = label;
                SetBlinking(badge.StatusDot, blink);
                badge.Root.Visibility = Visibility.Visible;
                badge.Root.ToolTip = string.Join(Environment.NewLine, mine.Select(s => $"{s.ServerName}: {services.Processes.StateOf(s)}"));

                if (checkUpdates)
                {
                    _ = RefreshUpdateDotAsync(services, card, mine);
                }
            }
            catch
            {
                // One bad server entry must not break the other cards.
            }
        }
    }

    private static void SetBlinking(UIElement dot, bool blink)
    {
        if (blink)
        {
            var animation = new System.Windows.Media.Animation.DoubleAnimation(1.0, 0.15, TimeSpan.FromMilliseconds(550))
            {
                AutoReverse = true,
                RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever,
            };
            dot.BeginAnimation(OpacityProperty, animation);
        }
        else
        {
            dot.BeginAnimation(OpacityProperty, null);
            dot.Opacity = 1;
        }
    }

    private async Task RefreshUpdateDotAsync(AppServices services, GameCard card, List<ServerInstance> servers)
    {
        var updates = new List<string>();
        foreach (var server in servers)
        {
            if (await ServerUpdateCheck.NewerVersionAsync(services.Installer, server, card.Game).ConfigureAwait(true) is { } newer)
            {
                updates.Add($"{server.ServerName}: {server.ServerVersion} → {newer}");
            }
        }

        card.Badge.UpdateDot.Visibility = updates.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        card.Badge.UpdateDot.ToolTip = updates.Count > 0
            ? "Update available (use Update in My Servers):" + Environment.NewLine + string.Join(Environment.NewLine, updates)
            : null;
    }
}
