using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

/// <summary>
/// One-click Minecraft mod/plugin install from Modrinth, shown inside the Mods &amp; Plugins tab
/// for Minecraft Java servers whose variant accepts mods or plugins.
/// </summary>
public sealed partial class ModrinthBrowserViewModel : ObservableObject
{
    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private readonly ModrinthService _modrinth = new();
    private readonly Action _onInstalled;

    public ModrinthBrowserViewModel(AppServices services, ToastService toasts, Action onInstalled)
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

    public ObservableCollection<ModrinthResultViewModel> Results { get; } = new();

    private List<string> _loaders = new();
    private string? _mcVersion;

    public void SetServer(ServerInstance? server)
    {
        Current = server;
        Results.Clear();
        StatusText = string.Empty;

        if (server is null || !ModCompatibility.IsMinecraftJava(server.ServerGame))
        {
            IsAvailable = false;
            return;
        }

        var accepted = ModCompatibility.AcceptedPlatforms(server);
        _loaders = ModCompatibility.ModrinthLoaders(accepted);
        _mcVersion = MinecraftVersionRange.ServerMinecraftVersion(server.ServerVersion);
        IsAvailable = _loaders.Count > 0;
    }

    [RelayCommand]
    private async Task Search()
    {
        if (Current is not { } server || !IsAvailable)
        {
            return;
        }

        IsSearching = true;
        StatusText = "Searching Modrinth…";
        try
        {
            var hits = await _modrinth.SearchAsync(Query, _loaders, _mcVersion);
            Results.Clear();
            foreach (var hit in hits)
            {
                Results.Add(new ModrinthResultViewModel(hit));
            }

            StatusText = hits.Count == 0
                ? $"No results for {ModCompatibility.VariantLabel(server)}{(_mcVersion is null ? string.Empty : $" / MC {_mcVersion}")}."
                : $"{hits.Count} result(s) for {ModCompatibility.VariantLabel(server)}{(_mcVersion is null ? string.Empty : $" / MC {_mcVersion}")}.";
        }
        catch (Exception ex)
        {
            StatusText = $"Couldn't search Modrinth: {ex.Message}";
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private async Task Install(ModrinthResultViewModel? result)
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
            var file = await _modrinth.GetBestVersionAsync(result.Project.ProjectId, _loaders, _mcVersion);
            if (file is null)
            {
                _toasts.Warning($"'{result.Title}' has no version that fits {ModCompatibility.VariantLabel(server)}{(_mcVersion is null ? string.Empty : $" / MC {_mcVersion}")}.");
                return;
            }

            var data = await _modrinth.DownloadAsync(file);
            Directory.CreateDirectory(destDir);
            await File.WriteAllBytesAsync(Path.Combine(destDir, file.Filename), data);

            result.IsInstalled = true;
            _toasts.Success($"Installed '{result.Title}' ({file.VersionNumber}).");
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
