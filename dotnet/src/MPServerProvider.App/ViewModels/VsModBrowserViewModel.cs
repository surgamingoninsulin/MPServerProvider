using CommunityToolkit.Mvvm.ComponentModel;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.ViewModels;

/// <summary>
/// Backs the embedded mods.vintagestory.at browser (<see cref="Views.ServerTabs.VsModWebBrowser"/>)
/// shown inside the Mods &amp; Plugins tab for Vintage Story servers: tracks which server is
/// selected and exposes what that control needs (the mods folder via <see cref="Services"/>, a
/// toast sink, and whether a Vintage Story server is even selected).
/// </summary>
public sealed partial class VsModBrowserViewModel : ObservableObject
{
    private readonly Action _onInstalled;

    public VsModBrowserViewModel(AppServices services, ToastService toasts, Action onInstalled)
    {
        Services = services;
        Toasts = toasts;
        _onInstalled = onInstalled;
    }

    public AppServices Services { get; }

    public ToastService Toasts { get; }

    public ServerInstance? Current { get; private set; }

    [ObservableProperty]
    private bool _isAvailable;

    [ObservableProperty]
    private string? _serverVersionLabel;

    public void SetServer(ServerInstance? server)
    {
        Current = server;
        IsAvailable = server?.ServerGame == "vintage-story";
        ServerVersionLabel = IsAvailable && !string.IsNullOrWhiteSpace(server?.ServerVersion)
            ? $"Your server is on Vintage Story {server!.ServerVersion}"
            : null;
    }

    public void NotifyInstalled() => _onInstalled();
}
