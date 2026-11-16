using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using MPServerProvider.App.ViewModels;

namespace MPServerProvider.App.Views.ServerTabs;

public partial class LuckPermsTab : UserControl
{
    private const string WatchScript = """
        (() => {
          if (window.__mpspLp) return; window.__mpspLp = true;
          const seen = {};
          const post = (o) => { try { window.chrome.webview.postMessage(JSON.stringify(o)); } catch (e) {} };
          const scanText = (t) => {
            if (!t) return;
            const re = /\/?\b(lp[bv]?)\s+(applyedits|trusteditor)\s+([A-Za-z0-9_-]{1,64})/gi;
            let m;
            while ((m = re.exec(t))) {
              const key = m[2].toLowerCase() + ':' + m[3];
              if (!seen[key]) { seen[key] = 1; post({ kind: m[2].toLowerCase(), alias: m[1], code: m[3] }); }
            }
          };
          try {
            const cb = navigator.clipboard;
            if (cb && cb.writeText) {
              const orig = cb.writeText.bind(cb);
              cb.writeText = (t) => { scanText(String(t)); return orig(t); };
            }
          } catch (e) {}
          const scan = () => { try { document.querySelectorAll('code').forEach(c => scanText(c.textContent)); } catch (e) {} };
          let timer = 0;
          const start = () => {
            scan();
            new MutationObserver(() => { clearTimeout(timer); timer = setTimeout(scan, 150); })
              .observe(document.documentElement, { subtree: true, childList: true, characterData: true });
          };
          if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', start); else start();
        })();
        """;

    private LuckPermsTabViewModel? _vm;
    private bool _initialized;
    private Task? _initTask;
    private string? _editorHost;
    private string? _loadedUrl;

    public LuckPermsTab()
    {
        InitializeComponent();
        DataContextChanged += (_, e) =>
        {
            if (_vm is not null)
            {
                _vm.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _vm = e.NewValue as LuckPermsTabViewModel;
            if (_vm is not null)
            {
                _vm.PropertyChanged += ViewModel_PropertyChanged;
            }

            _ = ShowEditorAsync();
        };
        IsVisibleChanged += (_, e) =>
        {
            if (e.NewValue is true)
            {
                _ = ShowEditorAsync();
            }
        };
    }

    private void ReportError(string what, Exception ex)
    {
        if (Window.GetWindow(this) is MainWindow { Toasts: { } toasts })
        {
            toasts.Error($"{what}: {ex.Message}");
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LuckPermsTabViewModel.EditorUrl))
        {
            _ = ShowEditorAsync();
        }
    }

    private Task EnsureBrowserAsync()
    {
        if (_initialized)
        {
            return Task.CompletedTask;
        }

        return _initTask ??= InitializeBrowserAsync();
    }

    private async Task InitializeBrowserAsync()
    {
        try
        {
            await EmbeddedBrowser.InitializeAsync(Web);
            var core = Web.CoreWebView2;
            await core.AddScriptToExecuteOnDocumentCreatedAsync(WatchScript);
            core.WebMessageReceived += Core_WebMessageReceived;
            core.NewWindowRequested += Core_NewWindowRequested;
            _initialized = true;
        }
        finally
        {
            _initTask = null;
        }
    }

    private async Task ShowEditorAsync()
    {
        try
        {
            var url = _vm?.EditorUrl ?? string.Empty;
            if (url.Length == 0)
            {
                Web.Visibility = Visibility.Collapsed;
                Placeholder.Visibility = Visibility.Visible;
                PlaceholderTitle.Text = "LuckPerms web editor";
                if (_loadedUrl is not null && Web.CoreWebView2 is { } idle)
                {
                    _loadedUrl = null;
                    _editorHost = null;
                    idle.Navigate("about:blank"); // drop the old session's page
                }

                return;
            }

            if (!IsVisible)
            {
                return;
            }

            await EnsureBrowserAsync();
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
            {
                return;
            }

            // Compare with what we last loaded, not Source: the editor rewrites its own URL.
            if (!string.Equals(_loadedUrl, uri.AbsoluteUri, StringComparison.Ordinal))
            {
                _loadedUrl = uri.AbsoluteUri;
                _editorHost = uri.Host;
                Web.CoreWebView2.Navigate(uri.AbsoluteUri);
            }

            Web.Visibility = Visibility.Visible;
            Placeholder.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            Web.Visibility = Visibility.Collapsed;
            Placeholder.Visibility = Visibility.Visible;
            PlaceholderTitle.Text = "Can't show the editor here";
            PlaceholderText.Text = EmbeddedBrowser.DescribeFailure(ex) + " Copy link also works.";
        }
    }

    private void Core_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            // Only the editor page we opened may arm the console buttons.
            if (_vm is null || _editorHost is null ||
                !Uri.TryCreate(e.Source, UriKind.Absolute, out var source) ||
                !string.Equals(source.Host, _editorHost, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            using var doc = JsonDocument.Parse(e.TryGetWebMessageAsString());
            var root = doc.RootElement;
            _vm.OnEditorPageCommand(
                root.GetProperty("kind").GetString() ?? string.Empty,
                root.GetProperty("alias").GetString() ?? string.Empty,
                root.GetProperty("code").GetString() ?? string.Empty);
        }
        catch
        {
            // Malformed page messages are ignored.
        }
    }

    private void Core_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        try
        {
            e.Handled = true;
            EmbeddedBrowser.OpenExternal(e.Uri);
        }
        catch (Exception ex)
        {
            ReportError("Could not open the link", ex);
        }
    }
}
