using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Views.ServerTabs;

/// <summary>
/// Embeds the real mods.vintagestory.at site so browsing/searching looks exactly like the
/// website, instead of a cut-down in-app card list. Downloads started from it (the site's own
/// "Download" button) are redirected into the current server's mods folder instead of the
/// system Downloads folder.
/// </summary>
public partial class VsModWebBrowser : UserControl
{
    private const string AllModsUrl = "https://mods.vintagestory.at/list/mod";
    private const string Host = "mods.vintagestory.at";

    private AppServices? _services;
    private ToastService? _toasts;
    private ServerInstance? _server;
    private Action? _onInstalled;
    private bool _initStarted;

    public VsModWebBrowser()
    {
        InitializeComponent();
        IsVisibleChanged += (_, e) =>
        {
            if (e.NewValue is true && !_initStarted)
            {
                _initStarted = true;
                _ = InitializeBrowserAsync();
            }
        };
    }

    public void SetContext(AppServices services, ToastService toasts, ServerInstance? server, Action onInstalled)
    {
        _services = services;
        _toasts = toasts;
        _server = server;
        _onInstalled = onInstalled;
        ServerVersionText.Text = !string.IsNullOrWhiteSpace(server?.ServerVersion)
            ? $"Your server: Vintage Story {server!.ServerVersion}"
            : string.Empty;
    }

    private async Task InitializeBrowserAsync()
    {
        try
        {
            await EmbeddedBrowser.InitializeAsync(Web);
            var core = Web.CoreWebView2;
            core.DownloadStarting += Core_DownloadStarting;
            core.NewWindowRequested += Core_NewWindowRequested;
            core.SourceChanged += (_, _) => AddressText.Text = core.Source;
            core.NavigationCompleted += (_, e) =>
            {
                if (!e.IsSuccess && Web.Visibility != Visibility.Visible)
                {
                    PlaceholderText.Text = $"Couldn't load mods.vintagestory.at ({e.WebErrorStatus}). Check your internet connection and press Reload.";
                }
                else
                {
                    Web.Visibility = Visibility.Visible;
                    Placeholder.Visibility = Visibility.Collapsed;
                }
            };
            core.Navigate(AllModsUrl);
        }
        catch (Exception ex)
        {
            _initStarted = false; // allow a retry next time the tab is shown
            Web.Visibility = Visibility.Collapsed;
            Placeholder.Visibility = Visibility.Visible;
            PlaceholderText.Text = EmbeddedBrowser.DescribeFailure(ex);
        }
    }

    private static bool IsModsVintageStory(string? url)
        => Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == "https" &&
           (uri.Host == Host || uri.Host.EndsWith("." + Host, StringComparison.OrdinalIgnoreCase));

    private void Core_DownloadStarting(object? sender, CoreWebView2DownloadStartingEventArgs e)
    {
        try
        {
            if (_services is null || _server is not { } server)
            {
                return;
            }

            var destDir = _services.Mods.ServerModsDir(server);
            if (destDir is null)
            {
                _toasts?.Error("This server has no mods folder configured.");
                e.Cancel = true;
                return;
            }

            Directory.CreateDirectory(destDir);
            var fileName = Path.GetFileName(e.ResultFilePath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = Path.GetFileName(e.DownloadOperation.Uri);
            }

            e.ResultFilePath = Path.Combine(destDir, fileName);
            e.Handled = true;

            var name = fileName;
            e.DownloadOperation.StateChanged += (opSender, _) =>
            {
                if (opSender is not CoreWebView2DownloadOperation op)
                {
                    return;
                }

                Dispatcher.Invoke(() =>
                {
                    switch (op.State)
                    {
                        case CoreWebView2DownloadState.Completed:
                            _toasts?.Success($"Installed '{name}'.");
                            _onInstalled?.Invoke();
                            break;
                        case CoreWebView2DownloadState.Interrupted:
                            _toasts?.Error($"Couldn't download '{name}': {op.InterruptReason}");
                            break;
                    }
                });
            };
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Couldn't install that mod: {ex.Message}");
        }
    }

    private void Core_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        try
        {
            e.Handled = true;
            if (IsModsVintageStory(e.Uri))
            {
                Web.CoreWebView2?.Navigate(e.Uri);
            }
            else
            {
                EmbeddedBrowser.OpenExternal(e.Uri);
            }
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Could not open the link: {ex.Message}");
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (Web.CoreWebView2 is { CanGoBack: true } core)
            {
                core.GoBack();
            }
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Could not go back: {ex.Message}");
        }
    }

    private void Reload_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (Web.CoreWebView2 is { } core)
            {
                core.Reload();
            }
            else if (!_initStarted)
            {
                _initStarted = true;
                _ = InitializeBrowserAsync();
            }
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Could not reload: {ex.Message}");
        }
    }

    private void Home_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Web.CoreWebView2?.Navigate(AllModsUrl);
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Could not open mods.vintagestory.at: {ex.Message}");
        }
    }

    private void OpenExternal_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var current = Web.CoreWebView2?.Source;
            EmbeddedBrowser.OpenExternal(IsModsVintageStory(current) ? current! : AllModsUrl);
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Could not open the browser: {ex.Message}");
        }
    }
}
