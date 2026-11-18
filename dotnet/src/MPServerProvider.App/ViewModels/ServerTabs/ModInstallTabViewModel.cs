using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Controls;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using MPServerProvider.Core.Services.Mods;

namespace MPServerProvider.App.ViewModels;

public sealed partial class ModListingCardViewModel : ObservableObject
{
    // Same fixed, cached decode width the Home page uses for its "last installed" server art -
    // the card is deliberately smaller than a game cover, so a smaller width keeps it crisp
    // without decoding (and caching) a full-size image just to shrink it on screen.
    private const int ThumbnailPixelWidth = 96;

    public ModListingCardViewModel(ModListing listing, AppServices services)
    {
        Listing = listing;
        _ = LoadThumbnailAsync(services);
    }

    public ModListing Listing { get; }

    public string Name => Listing.Name;

    public string? Author => Listing.Author;

    public string? Summary => Listing.Summary;

    public string? WebsiteUrl => Listing.WebsiteUrl;

    public string DownloadsText => Listing.Downloads is { } n ? $"{n:N0} downloads" : string.Empty;

    [ObservableProperty]
    private ImageSource? _thumbnail;

    private async Task LoadThumbnailAsync(AppServices services)
    {
        if (Listing.ThumbnailUrl is not { Length: > 0 } url)
        {
            return;
        }
        try
        {
            Thumbnail = await ArtworkImages.LoadAsync(
                services, ArtworkImages.StableKey("mod_thumb", url), url, ThumbnailPixelWidth);
        }
        catch
        {
            // A missing/broken thumbnail just means the card shows its placeholder background.
        }
    }

    [ObservableProperty]
    private bool _isInstalling;

    [ObservableProperty]
    private bool _isInstalled;

    [ObservableProperty]
    private string _statusText = string.Empty;
}

/// <summary>
/// "Install Mods" tab: search a server's available marketplace(s) (CurseForge, Thunderstore - see
/// <see cref="GameModSupport"/>) and drop a mod straight into the server's mods folder, reusing
/// the same compatibility checks and duplicate handling as a manual drag-and-drop.
/// </summary>
public sealed partial class ModInstallTabViewModel : ObservableObject
{
    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private CancellationTokenSource? _searchCts;
    private int _nextPage;
    private bool _hasMore;
    private bool _isLoadingMore;
    private HashSet<string> _activeLaunchArgModIds = new(StringComparer.OrdinalIgnoreCase);

    public ModInstallTabViewModel(AppServices services, ToastService toasts)
    {
        _services = services;
        _toasts = toasts;
    }

    public ServerInstance? Current { get; private set; }

    [ObservableProperty]
    private bool _isAvailable;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private bool _isSearching;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private bool _isLaunchArgInstall;

    /// <summary>Explains, for launch-arg games (Abiotic Factor, ...), why an installed mod won't
    /// show up in the Mods tab or File Manager: the dedicated server downloads it itself into its
    /// own cache outside this app's managed folders, not into anything this app placed there.</summary>
    [ObservableProperty]
    private string _launchArgNoticeText = string.Empty;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private ModSourceKind? _selectedSource;

    public ObservableCollection<ModSourceKind> AvailableSources { get; } = new();

    public ObservableCollection<ModListingCardViewModel> Results { get; } = new();

    private bool _loadedOnce;

    public void SetServer(ServerInstance? server)
    {
        var isSame = server is not null && Current?.ServerName == server.ServerName;
        Current = server;

        var sources = server is null ? Array.Empty<ModSourceKind>() : _services.ModInstaller.SourcesFor(server.ServerGame);
        IsAvailable = sources.Count > 0;
        IsLaunchArgInstall = server is not null && _services.ModInstaller.IsLaunchArgInstall(server.ServerGame);
        _activeLaunchArgModIds = server is not null
            ? new HashSet<string>(_services.ModInstaller.GetLaunchArgModIds(server), StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        LaunchArgNoticeText = IsLaunchArgInstall
            ? (_activeLaunchArgModIds.Count == 0
                ? "This server fetches mods itself using the launch arguments below - installing adds the mod's ID there instead of placing a file, so it won't show up in the Mods tab or File Manager."
                : $"Currently configured: {string.Join(", ", _activeLaunchArgModIds)}. This server downloads these itself into its own cache, so they won't show up in the Mods tab or File Manager.")
            : string.Empty;

        if (!isSame)
        {
            Results.Clear();
            SearchText = string.Empty;
            StatusText = string.Empty;
            AvailableSources.Clear();
            foreach (var source in sources)
            {
                AvailableSources.Add(source);
            }
            SelectedSource = AvailableSources.FirstOrDefault();
            _loadedOnce = false;
        }
    }

    /// <summary>Loads a first page of results the moment the tab is actually shown, so there's
    /// something to scroll through right away instead of an empty list waiting on a manual search.</summary>
    public void EnsureLoaded()
    {
        if (_loadedOnce || !IsAvailable || !SearchCommand.CanExecute(null))
        {
            return;
        }
        _loadedOnce = true;
        SearchCommand.Execute(null);
    }

    private bool CanSearch() => !IsSearching && Current is not null && SelectedSource is not null;

    [RelayCommand(CanExecute = nameof(CanSearch))]
    private async Task Search()
    {
        if (Current is not { } server || SelectedSource is not { } source)
        {
            return;
        }

        _searchCts?.Cancel();
        var cts = _searchCts = new CancellationTokenSource();

        IsSearching = true;
        StatusText = $"Searching {SourceName(source)}…";
        Results.Clear();
        _nextPage = 0;
        _hasMore = true;
        try
        {
            var listings = await _services.ModInstaller.SearchAsync(source, server.ServerGame, SearchText, _nextPage, cts.Token);
            if (cts.IsCancellationRequested)
            {
                return;
            }

            AppendResults(listings);
            StatusText = listings.Count == 0
                ? $"No results on {SourceName(source)}."
                : $"{Results.Count} result(s) from {SourceName(source)}.";
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            StatusText = $"Search failed: {ex.Message}";
        }
        finally
        {
            if (!cts.IsCancellationRequested)
            {
                IsSearching = false;
            }
        }
    }

    /// <summary>Called when the results list scrolls near its bottom, so browsing a marketplace
    /// that has far more than one page's worth of mods (e.g. CurseForge's Minecraft/Hytale
    /// catalogs) keeps fetching further pages instead of stopping at the first ~25.</summary>
    public async void LoadMore()
    {
        if (_isLoadingMore || !_hasMore || IsSearching || Current is not { } server || SelectedSource is not { } source)
        {
            return;
        }

        _isLoadingMore = true;
        try
        {
            var page = _nextPage + 1;
            var listings = await _services.ModInstaller.SearchAsync(source, server.ServerGame, SearchText, page, CancellationToken.None);
            _nextPage = page;
            AppendResults(listings);
            if (listings.Count > 0)
            {
                StatusText = $"{Results.Count} result(s) from {SourceName(source)}.";
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Couldn't load more: {ex.Message}";
        }
        finally
        {
            _isLoadingMore = false;
        }
    }

    private void AppendResults(IReadOnlyList<ModListing> listings)
    {
        _hasMore = listings.Count > 0;
        foreach (var listing in listings)
        {
            var card = new ModListingCardViewModel(listing, _services);
            if (_activeLaunchArgModIds.Contains(listing.Id))
            {
                card.IsInstalled = true;
                card.StatusText = "Already in this server's launch arguments.";
            }
            Results.Add(card);
        }
    }

    [RelayCommand]
    private async Task Install(ModListingCardViewModel? card)
    {
        if (card is null || card.IsInstalling || Current is not { } server || SelectedSource is not { } source)
        {
            return;
        }

        card.IsInstalling = true;
        card.IsInstalled = false;
        try
        {
            ModInstallOutcome outcome;
            if (_services.ModInstaller.IsLaunchArgInstall(server.ServerGame))
            {
                // No file to fetch here - the dedicated server downloads the mod itself on its
                // next start once its ID is in the launch arguments.
                card.StatusText = "Adding to launch arguments…";
                outcome = _services.ModInstaller.InstallByLaunchArg(server, card.Listing.Id);
                if (outcome.Success)
                {
                    _activeLaunchArgModIds.Add(card.Listing.Id);
                    LaunchArgNoticeText = $"Currently configured: {string.Join(", ", _activeLaunchArgModIds)}. This server downloads these itself into its own cache, so they won't show up in the Mods tab or File Manager.";
                }
            }
            else
            {
                card.StatusText = "Finding the latest file…";
                var files = await _services.ModInstaller.GetFilesAsync(source, server.ServerGame, card.Listing.Id, CancellationToken.None);
                var file = files.FirstOrDefault();
                if (file is null)
                {
                    card.StatusText = "No downloadable files for this mod.";
                    return;
                }

                card.StatusText = $"Installing {file.DisplayName}…";
                outcome = await _services.ModInstaller.InstallAsync(source, server, card.Listing.Id, file.Id, CancellationToken.None);
            }

            card.StatusText = outcome.Message;
            if (outcome.Success)
            {
                card.IsInstalled = true;
                _toasts.Success($"'{card.Name}': {outcome.Message}");
            }
            else
            {
                _toasts.Warning($"'{card.Name}' wasn't installed: {outcome.Message}");
            }
        }
        catch (Exception ex)
        {
            card.StatusText = $"Install failed: {ex.Message}";
            _toasts.Error($"Couldn't install '{card.Name}': {ex.Message}");
        }
        finally
        {
            card.IsInstalling = false;
        }
    }

    private static string SourceName(ModSourceKind kind) => kind switch
    {
        ModSourceKind.CurseForge => "CurseForge",
        ModSourceKind.Thunderstore => "Thunderstore",
        ModSourceKind.ModIo => "mod.io",
        _ => kind.ToString(),
    };
}
