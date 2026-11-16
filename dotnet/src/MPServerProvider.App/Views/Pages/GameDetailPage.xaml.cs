using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MPServerProvider.App.Controls;
using MPServerProvider.App.Views.Dialogs;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.Views.Pages;

public partial class GameDetailPage : UserControl
{
    private const double ThumbnailWidth = 220;

    private readonly GameDefinition _game;
    private readonly string _returnToPageKey;
    private readonly CancellationTokenSource _cts = new();

    public GameDetailPage(GameDefinition game, string returnToPageKey)
    {
        _game = game;
        _returnToPageKey = returnToPageKey;
        InitializeComponent();

        BackText.Text = returnToPageKey switch
        {
            "home" => "Back to Home",
            "library" => "Back to Library",
            _ => "Back",
        };

        Populate();

        Loaded += OnLoaded;
        Unloaded += (_, _) => _cts.Cancel();
        PreviewMouseUp += OnPreviewMouseUp;
        PreviewKeyDown += OnPreviewKeyDown;
    }

    private AppServices? Services => (Application.Current as App)?.Services;

    private MainWindow? Main => Window.GetWindow(this) as MainWindow;

    private static Brush Res(string key) => (Brush)Application.Current.Resources[key];

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Focus();
        if (Services is { } services)
        {
            _ = LoadHeroAsync(services);
            _ = LoadScreenshotsAsync(services);
        }
    }

    // ---- content -----------------------------------------------------------

    private void Populate()
    {
        var details = _game.Details;
        var accent = ArtworkImages.AccentBrush(_game.Artwork.Accent);

        HeroBanner.Background = accent;
        NameText.Text = _game.Name;
        TaglineText.Text = _game.Description;
        TaglineText.Visibility = string.IsNullOrWhiteSpace(_game.Description) ? Visibility.Collapsed : Visibility.Visible;
        GenreChips.ItemsSource = _game.Genres;
        InstallViaText.Text = DescribeProvider(_game.Install);

        // About: researched text, else the manifest description (custom games).
        var about = !string.IsNullOrWhiteSpace(details?.About) ? details!.About! : _game.Description;
        var paragraphs = (about ?? string.Empty)
            .Replace("\r\n", "\n")
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var paragraph in paragraphs)
        {
            AboutPanel.Children.Add(new TextBlock
            {
                Text = paragraph,
                Foreground = Res("TextSecondaryBrush"),
                FontSize = 13,
                LineHeight = 21,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 10),
            });
        }
        AboutCard.Visibility = paragraphs.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        // Features
        foreach (var feature in details?.Features ?? new List<string>())
        {
            FeaturesPanel.Children.Add(MakeBullet("", feature, accent));
        }
        FeaturesCard.Visibility = FeaturesPanel.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        // Setup tips (numbered)
        var tipIndex = 0;
        foreach (var tip in details?.SetupTips ?? new List<string>())
        {
            TipsPanel.Children.Add(MakeNumberedTip(++tipIndex, tip, accent));
        }
        TipsCard.Visibility = tipIndex > 0 ? Visibility.Visible : Visibility.Collapsed;

        // Screenshots: placeholders now, images stream in from LoadScreenshotsAsync.
        foreach (var url in (details?.Screenshots ?? new List<string>()).Where(u => ArtworkImages.TryGetWebUri(u, out _)))
        {
            ScreenshotsPanel.Children.Add(MakeScreenshotTile(url, accent));
        }
        ScreenshotsCard.Visibility = ScreenshotsPanel.Children.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        // Info sidebar: researched facts plus what the manifest itself knows.
        AddInfo("Developer", details?.Developer);
        AddInfo("Publisher", details?.Publisher);
        AddInfo("Release date", details?.ReleaseDate);
        AddInfo("Players", details?.MaxPlayers);
        AddInfo("Default port", _game.Network.DefaultPort > 0
            ? $"{_game.Network.DefaultPort} ({DescribeProtocol(_game.Network.Protocol)})"
            : null);
        AddInfo("Mods", _game.Mods.Supported
            ? $"Supported ({(_game.Mods.Folder ?? "mods").Replace('\\', '/')})"
            : "Not supported in this app");

        // Requirements
        var req = details?.ServerRequirements;
        var hasReq = false;
        if (req is not null)
        {
            hasReq |= AddRequirement("Minimum RAM", FormatRam(req.MinRamMb));
            hasReq |= AddRequirement("Recommended RAM", FormatRam(req.RecommendedRamMb));
            hasReq |= AddRequirement("Disk space", req.DiskGb is { } gb ? $"{gb} GB" : null);
            if (!string.IsNullOrWhiteSpace(req.Notes))
            {
                RequirementsPanel.Children.Add(new TextBlock
                {
                    Text = req.Notes,
                    Foreground = Res("TextSecondaryBrush"),
                    FontSize = 12,
                    LineHeight = 18,
                    TextWrapping = TextWrapping.Wrap,
                });
                hasReq = true;
            }
        }
        RequirementsCard.Visibility = hasReq ? Visibility.Visible : Visibility.Collapsed;

        // Ports table
        var ports = details?.Ports ?? new List<GamePortInfo>();
        for (var i = 0; i < ports.Count; i++)
        {
            AddPortRow(i, ports[i], accent);
        }
        PortsCard.Visibility = ports.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        // Links (http/https only)
        AddLink("", "Official website", details?.Website);
        AddLink("", "Store page", details?.StoreUrl);
        AddLink("", "Server documentation", details?.DocsUrl);
        LinksCard.Visibility = LinksPanel.Children.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
    }

    private static string DescribeProvider(InstallSpec install) => install.Provider switch
    {
        "steamcmd" => "Installs via SteamCMD",
        "minecraft-java" or "minecraft-bedrock" => "Official Mojang download",
        "cfx" => "Official Cfx.re artifacts",
        "beammp" => "Official BeamMP release",
        "url" => "Direct download",
        "manual" => "Manual install",
        _ => string.Empty,
    };

    private static string DescribeProtocol(string? protocol) => protocol?.ToLowerInvariant() switch
    {
        "both" => "TCP+UDP",
        "tcp" => "TCP",
        "udp" => "UDP",
        _ => protocol ?? string.Empty,
    };

    private static string? FormatRam(int? mb) => mb switch
    {
        null => null,
        >= 1024 when mb % 1024 == 0 => $"{mb / 1024} GB",
        >= 1024 => $"{mb / 1024.0:0.#} GB",
        _ => $"{mb} MB",
    };

    private void AddInfo(string label, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        InfoPanel.Children.Add(new TextBlock { Text = label.ToUpperInvariant(), Style = (Style)Resources["InfoLabel"] });
        InfoPanel.Children.Add(new TextBlock { Text = value, Style = (Style)Resources["InfoValue"] });
    }

    private bool AddRequirement(string label, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var row = new Grid { Margin = new Thickness(0, 0, 0, 10) };
        row.ColumnDefinitions.Add(new ColumnDefinition());
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        row.Children.Add(new TextBlock { Text = label, Foreground = Res("TextSecondaryBrush"), FontSize = 12 });
        var valueText = new TextBlock { Text = value, Foreground = Res("TextPrimaryBrush"), FontSize = 13, FontWeight = FontWeights.SemiBold };
        Grid.SetColumn(valueText, 1);
        row.Children.Add(valueText);
        RequirementsPanel.Children.Add(row);
        return true;
    }

    private void AddPortRow(int index, GamePortInfo port, Brush accent)
    {
        PortsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var portText = new TextBlock
        {
            Text = port.Port,
            Foreground = Res("TextPrimaryBrush"),
            FontFamily = new FontFamily("Consolas"),
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 10, 12),
            VerticalAlignment = VerticalAlignment.Top,
        };

        var protocolChip = new Border
        {
            Background = accent,
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(6, 1, 6, 2),
            Margin = new Thickness(0, 0, 10, 12),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Child = new TextBlock { Text = port.Protocol, Foreground = Brushes.White, FontSize = 10, FontWeight = FontWeights.SemiBold },
        };

        var purpose = new TextBlock
        {
            Text = port.Purpose ?? string.Empty,
            Foreground = Res("TextSecondaryBrush"),
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 12),
        };

        Grid.SetRow(portText, index);
        Grid.SetRow(protocolChip, index);
        Grid.SetColumn(protocolChip, 1);
        Grid.SetRow(purpose, index);
        Grid.SetColumn(purpose, 2);
        PortsGrid.Children.Add(portText);
        PortsGrid.Children.Add(protocolChip);
        PortsGrid.Children.Add(purpose);
    }

    private void AddLink(string glyph, string label, string? url)
    {
        if (!ArtworkImages.TryGetWebUri(url, out var uri))
        {
            return;
        }

        var content = new StackPanel { Orientation = Orientation.Horizontal };
        content.Children.Add(new TextBlock
        {
            Text = glyph,
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            Foreground = Res("BrandAccentBrush"),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 10, 0),
        });
        var texts = new StackPanel();
        texts.Children.Add(new TextBlock { Text = label, Foreground = Res("TextPrimaryBrush") });
        texts.Children.Add(new TextBlock { Text = uri.Host, Foreground = Res("TextMutedBrush"), FontSize = 10 });
        content.Children.Add(texts);

        var button = new Button
        {
            Style = (Style)Resources["LinkButton"],
            Content = content,
            ToolTip = uri.AbsoluteUri,
        };
        button.Click += (_, _) => OpenUrl(uri.AbsoluteUri);
        LinksPanel.Children.Add(button);
    }

    private static FrameworkElement MakeBullet(string glyph, string text, Brush accent)
    {
        var row = new DockPanel { Margin = new Thickness(0, 0, 14, 10) };
        var icon = new TextBlock
        {
            Text = glyph,
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            FontSize = 11,
            Foreground = accent,
            Margin = new Thickness(0, 3, 10, 0),
            VerticalAlignment = VerticalAlignment.Top,
        };
        DockPanel.SetDock(icon, Dock.Left);
        row.Children.Add(icon);
        row.Children.Add(new TextBlock
        {
            Text = text,
            Foreground = Res("TextSecondaryBrush"),
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap,
        });
        return row;
    }

    private static FrameworkElement MakeNumberedTip(int number, string text, Brush accent)
    {
        var row = new DockPanel { Margin = new Thickness(0, 0, 0, 12) };
        var badge = new Border
        {
            Width = 22,
            Height = 22,
            CornerRadius = new CornerRadius(11),
            Background = accent,
            Margin = new Thickness(0, 0, 12, 0),
            VerticalAlignment = VerticalAlignment.Top,
            Child = new TextBlock
            {
                Text = number.ToString(),
                Foreground = Brushes.White,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            },
        };
        DockPanel.SetDock(badge, Dock.Left);
        row.Children.Add(badge);
        row.Children.Add(new TextBlock
        {
            Text = text,
            Foreground = Res("TextSecondaryBrush"),
            FontSize = 13,
            LineHeight = 20,
            TextWrapping = TextWrapping.Wrap,
        });
        return row;
    }

    private FrameworkElement MakeScreenshotTile(string url, Brush accent)
    {
        var fallback = new Border
        {
            Background = accent,
            Opacity = 0.35,
            CornerRadius = new CornerRadius(8),
        };
        var image = new Image { Stretch = Stretch.UniformToFill, Visibility = Visibility.Collapsed };
        var host = new Grid { Height = 124, Margin = new Thickness(0, 0, 10, 10), Cursor = Cursors.Hand, Tag = url, ToolTip = "Open full size" };
        host.Children.Add(fallback);
        host.Children.Add(image);
        host.SizeChanged += (_, e) => image.Clip = new RectangleGeometry(new Rect(e.NewSize), 8, 8);
        host.MouseLeftButtonUp += (_, _) => OpenUrl(url);
        return host;
    }

    // ---- async images -----------------------------------------------------

    private async Task LoadHeroAsync(AppServices services)
    {
        try
        {
            // A real wide hero first; when the manifest only has the small Steam header (or no
            // hero at all), a 1080p screenshot looks far better stretched across the banner.
            var heroUrl = _game.Artwork.Hero;
            var firstShot = _game.Details?.Screenshots.FirstOrDefault(u => ArtworkImages.TryGetWebUri(u, out _));
            string? url;
            if (!string.IsNullOrWhiteSpace(heroUrl) && heroUrl != _game.Artwork.Cover)
            {
                url = heroUrl;
            }
            else
            {
                url = firstShot ?? _game.Artwork.Cover;
            }

            var width = ArtworkImages.PixelWidth(this, Math.Max(HeroBanner.ActualWidth, 1200));
            var heroTask = ArtworkImages.LoadAsync(services, ArtworkImages.StableKey($"{_game.Id}_hero", url ?? string.Empty), url, width, _cts.Token);

            // The small cover is almost always already cached by the Library page: show it right
            // away so the banner isn't an empty tile while a large hero image downloads.
            System.Windows.Media.Imaging.BitmapSource? cover = null;
            if (url != _game.Artwork.Cover)
            {
                cover = await ArtworkImages.LoadAsync(services, $"{_game.Id}_header", _game.Artwork.Cover, ArtworkImages.PixelWidth(this, 640), _cts.Token).ConfigureAwait(true);
                if (cover is not null && !heroTask.IsCompleted && !_cts.IsCancellationRequested)
                {
                    ShowHero(cover);
                }
            }

            var bitmap = await heroTask.ConfigureAwait(true);

            // A square/portrait "hero" (e.g. a logo) can't fill a wide banner; prefer a screenshot.
            if (bitmap is { PixelWidth: > 0 } && bitmap.PixelWidth < bitmap.PixelHeight * 1.5
                && firstShot is not null && firstShot != url)
            {
                bitmap = await ArtworkImages.LoadAsync(services, ArtworkImages.StableKey($"{_game.Id}_hero", firstShot), firstShot, width, _cts.Token).ConfigureAwait(true)
                    ?? bitmap;
            }

            bitmap ??= cover;
            if (bitmap is not null && !_cts.IsCancellationRequested)
            {
                ShowHero(bitmap);
            }
        }
        catch
        {
            // Accent fallback stays.
        }
    }

    private void ShowHero(ImageSource source)
    {
        var brush = new ImageBrush(source)
        {
            Stretch = Stretch.UniformToFill,
            AlignmentX = AlignmentX.Center,
            AlignmentY = AlignmentY.Center,
        };
        brush.Freeze();
        HeroBanner.Background = brush;
        HeroFallbackIcon.Visibility = Visibility.Collapsed;
    }

    private async Task LoadScreenshotsAsync(AppServices services)
    {
        var width = ArtworkImages.PixelWidth(this, ThumbnailWidth);
        var tasks = ScreenshotsPanel.Children.OfType<Grid>().Select(async host =>
        {
            try
            {
                if (host.Tag is not string url)
                {
                    return;
                }

                var thumbUrl = ArtworkImages.ThumbnailUrl(url);
                var bitmap = await ArtworkImages.LoadAsync(services, ArtworkImages.StableKey($"{_game.Id}_shot", thumbUrl), thumbUrl, width, _cts.Token).ConfigureAwait(true);
                if (bitmap is null && thumbUrl != url)
                {
                    bitmap = await ArtworkImages.LoadAsync(services, ArtworkImages.StableKey($"{_game.Id}_shot", url), url, width, _cts.Token).ConfigureAwait(true);
                }

                if (bitmap is not null && host.Children.OfType<Image>().FirstOrDefault() is { } image)
                {
                    image.Source = bitmap;
                    image.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                // Tile keeps its accent placeholder.
            }
        }).ToList();

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(true);
        }
        catch
        {
            // Individual tiles already handle their own failures.
        }
    }

    // ---- actions ----------------------------------------------------------

    private void OpenUrl(string url)
    {
        try
        {
            if (!ArtworkImages.OpenInBrowser(url))
            {
                Main?.Toasts.Warning("That link isn't a web address, so it wasn't opened.");
            }
        }
        catch (Exception ex)
        {
            Main?.Toasts.Error($"Couldn't open the browser: {ex.Message}");
        }
    }

    private void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        var main = Main;
        try
        {
            if (Services is not { } services)
            {
                return;
            }

            InstallChoiceDialog.Run(services, main?.Toasts, _game, main);
        }
        catch (Exception ex)
        {
            main?.Toasts.Error($"Couldn't start the install: {ex.Message}");
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => GoBack();

    private void GoBack()
    {
        try
        {
            Main?.NavigateToPage(_returnToPageKey);
        }
        catch (Exception ex)
        {
            Main?.Toasts.Error($"Navigation failed: {ex.Message}");
        }
    }

    private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.XButton1)
        {
            e.Handled = true;
            GoBack();
        }
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape || (e.Key == Key.Back && e.OriginalSource is not TextBox))
        {
            e.Handled = true;
            GoBack();
        }
    }
}
