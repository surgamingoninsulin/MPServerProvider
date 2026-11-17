using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

public sealed partial class ScenarioDownloaderTabViewModel : ObservableObject
{
    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private readonly RctgoScenarioService _rctgo = new();

    public ScenarioDownloaderTabViewModel(AppServices services, ToastService toasts)
    {
        _services = services;
        _toasts = toasts;
    }

    public ServerInstance? Current { get; private set; }

    [ObservableProperty]
    private bool _isAvailable;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    private int _page = 1;

    public ObservableCollection<ScenarioCardViewModel> Scenarios { get; } = new();

    private bool _loadedOnce;

    public void SetServer(ServerInstance? server)
    {
        var isSame = server is not null && Current?.ServerName == server.ServerName;
        Current = server;
        IsAvailable = server?.ServerGame == OpenRct2Setup.GameId;
        if (IsAvailable && !isSame)
        {
            _loadedOnce = false;
        }
    }

    public void EnsureLoaded()
    {
        if (!IsAvailable || _loadedOnce)
        {
            return;
        }

        _loadedOnce = true;
        _ = LoadAsync();
    }

    [RelayCommand]
    private Task Refresh() => LoadAsync();

    private bool CanGoToPreviousPage() => Page > 1 && !IsLoading;

    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private Task PreviousPage()
    {
        Page = Math.Max(1, Page - 1);
        return LoadAsync();
    }

    [RelayCommand]
    private Task NextPage()
    {
        Page++;
        return LoadAsync();
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        StatusText = "Loading scenarios from rctgo.com…";
        try
        {
            var page = Page;
            var results = await _rctgo.SearchAsync(page);
            Scenarios.Clear();
            foreach (var scenario in results)
            {
                Scenarios.Add(new ScenarioCardViewModel(scenario));
            }

            StatusText = results.Count == 0
                ? "No scenarios on this page."
                : $"Page {page} — RCT2 & RCT Classic scenarios from rctgo.com.";
        }
        catch (Exception ex)
        {
            StatusText = $"Couldn't load scenarios: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Download(ScenarioCardViewModel? card)
    {
        if (card is null || card.IsDownloading || Current is not { } server)
        {
            return;
        }

        var scenario = card.Listing;
        var root = _services.Servers.RootFor(server);
        var parkFile = Path.Combine(root, "park.park");
        if (File.Exists(parkFile))
        {
            var confirmed = Views.Dialogs.ConfirmDialog.Show(
                Application.Current.MainWindow,
                "Replace the active park?",
                $"'{scenario.Title}' will replace this server's current park.park. The server picks it up next time it starts.",
                confirmText: "Replace",
                destructive: true);
            if (!confirmed)
            {
                return;
            }
        }

        card.IsDownloading = true;
        card.IsDownloaded = false;
        StatusText = $"Downloading '{scenario.Title}'…";
        try
        {
            var (data, _) = await _rctgo.DownloadAsync(scenario.Id);
            await File.WriteAllBytesAsync(parkFile, data);
            card.IsDownloaded = true;
            StatusText = $"'{scenario.Title}' downloaded and set as the server's park.";
            _toasts.Success($"'{scenario.Title}' is set as the server's park. Restart the server to load it.");
        }
        catch (Exception ex)
        {
            StatusText = $"Couldn't download '{scenario.Title}': {ex.Message}";
            _toasts.Error($"Couldn't download '{scenario.Title}': {ex.Message}");
        }
        finally
        {
            card.IsDownloading = false;
        }
    }

    [RelayCommand]
    private void ImportGameData()
    {
        if (Current is not { } server)
        {
            return;
        }

        Views.Dialogs.ImportRctAssetsDialog.Show(Application.Current.MainWindow, _services.Servers.RootFor(server));
    }

    [RelayCommand]
    private void OpenOnWebsite()
    {
        Controls.ArtworkImages.OpenInBrowser("https://rctgo.com/downloads/scenarios/rct2");
    }
}
