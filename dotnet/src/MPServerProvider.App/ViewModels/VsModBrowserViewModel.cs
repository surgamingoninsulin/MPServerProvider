using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

/// <summary>
/// One-click Vintage Story mod install from mods.vintagestory.at, shown inside the Mods &amp;
/// Plugins tab for Vintage Story servers.
/// </summary>
public sealed partial class VsModBrowserViewModel : ObservableObject
{
    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private readonly VsModDbService _modDb = new();
    private readonly Action _onInstalled;

    public VsModBrowserViewModel(AppServices services, ToastService toasts, Action onInstalled)
    {
        _services = services;
        _toasts = toasts;
        _onInstalled = onInstalled;
    }

    public ServerInstance? Current { get; private set; }

    [ObservableProperty]
    private bool _isAvailable;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private string _statusText = string.Empty;

    public ObservableCollection<VsModCardViewModel> Results { get; } = new();

    public void SetServer(ServerInstance? server)
    {
        Current = server;
        Results.Clear();
        StatusText = string.Empty;
        IsAvailable = server?.ServerGame == "vintage-story";
    }

    [RelayCommand]
    private async Task Search()
    {
        if (Current is null || !IsAvailable)
        {
            return;
        }

        IsSearching = true;
        StatusText = "Searching mods.vintagestory.at…";
        try
        {
            var hits = await _modDb.SearchAsync(Query);
            Results.Clear();
            foreach (var hit in hits)
            {
                Results.Add(new VsModCardViewModel(hit));
            }

            StatusText = hits.Count == 0 ? "No results." : $"{hits.Count} result(s).";
        }
        catch (Exception ex)
        {
            StatusText = $"Couldn't search mods.vintagestory.at: {ex.Message}";
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private async Task Install(VsModCardViewModel? result)
    {
        if (result is null || result.IsInstalling || Current is not { } server)
        {
            return;
        }

        var destDir = _services.Mods.ServerModsDir(server);
        if (destDir is null)
        {
            _toasts.Error("This server has no mods folder configured.");
            return;
        }

        result.IsInstalling = true;
        result.IsInstalled = false;
        try
        {
            var release = await _modDb.GetBestReleaseAsync(result.Project.ModIdStr, server.ServerVersion);
            if (release is null)
            {
                _toasts.Warning($"'{result.Title}' has no downloadable release.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(server.ServerVersion) && !release.GameVersions.Contains(server.ServerVersion, StringComparer.OrdinalIgnoreCase))
            {
                _toasts.Warning($"'{result.Title}' has no release confirmed for Vintage Story {server.ServerVersion}; installing the newest release anyway.");
            }

            var data = await _modDb.DownloadAsync(release);
            Directory.CreateDirectory(destDir);
            await File.WriteAllBytesAsync(Path.Combine(destDir, release.Filename), data);

            result.IsInstalled = true;
            _toasts.Success($"Installed '{result.Title}' ({release.ModVersion}).");
            _onInstalled();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't install '{result.Title}': {ex.Message}");
        }
        finally
        {
            result.IsInstalling = false;
        }
    }
}
