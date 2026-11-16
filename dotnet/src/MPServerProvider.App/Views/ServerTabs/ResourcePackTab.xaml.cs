using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using MPServerProvider.App.ViewModels;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Views.ServerTabs;

/// <summary>
/// Resource Pack tab: mcpacks.dev in a WebView2 (left) and the server.properties form (right,
/// <see cref="ResourcePackTabViewModel"/>).
/// <para>
/// How the upload result is captured: mcpacks.dev is a Laravel + Inertia.js (Vue) app. The
/// upload form posts the zip to <c>/upload</c> with an <c>X-Inertia</c> XHR; the redirect lands on
/// an Inertia page <c>{"component":"UploadSuccess","props":{"pack":{"download_url":…,
/// "sha1_hash":…}}}</c> (the same <c>pack</c> shape is used by the <c>PackDetails</c> page).
/// Three independent readers feed <see cref="ResourcePackTabViewModel.OnPackCaptured"/>, each
/// validated in Core before use:
/// </para>
/// <list type="number">
/// <item><c>WebResourceResponseReceived</c>: the JSON body of the <c>X-Inertia</c> response;</item>
/// <item>an injected script polling <c>history.state.page</c> (Inertia keeps the current page
/// object there) and the server-rendered <c>script[data-page]</c> JSON, posted back with
/// <c>chrome.webview.postMessage</c>;</item>
/// <item>a DOM fallback in the same script: a 40-hex <c>&lt;code&gt;</c> next to a
/// <c>…/download</c> link, in case the site changes its data layer.</item>
/// </list>
/// </summary>
public partial class ResourcePackTab : UserControl
{
    private const string McPacksHost = "mcpacks.dev";

    /// <summary>Runs in every mcpacks.dev document; reports the shown pack (if any) once per change.</summary>
    private const string CaptureScript = """
        (() => {
          if (window.__mpspPacks) return; window.__mpspPacks = true;
          let last = '';
          const post = (o) => { try { const s = JSON.stringify(o); if (s !== last) { last = s; window.chrome.webview.postMessage(s); } } catch (e) {} };
          const hasPack = (p) => p && p.props && p.props.pack && p.props.pack.sha1_hash && p.props.pack.download_url;
          const scan = () => {
            try { const p = history.state && history.state.page; if (hasPack(p)) { post({ kind: 'page', page: JSON.stringify(p) }); return; } } catch (e) {}
            try { const el = document.querySelector('script[data-page]'); if (el) { const p = JSON.parse(el.textContent); if (hasPack(p)) { post({ kind: 'page', page: JSON.stringify(p) }); return; } } } catch (e) {}
            try {
              let sha = '', url = '';
              document.querySelectorAll('code').forEach(c => {
                const t = (c.textContent || '').trim();
                if (!sha && /^[0-9a-fA-F]{40}$/.test(t)) sha = t;
                if (!url && /^https?:\/\/\S+$/.test(t) && /\/download$|\/pack\//i.test(t)) url = t;
              });
              if (sha && url) post({ kind: 'dom', url: url, sha1: sha });
            } catch (e) {}
          };
          setInterval(scan, 1000);
        })();
        """;

    private bool _initStarted;

    public ResourcePackTab()
    {
        InitializeComponent();
        // Lazy: the browser only starts the first time the tab is actually shown.
        IsVisibleChanged += (_, e) =>
        {
            if (e.NewValue is true && !_initStarted)
            {
                _initStarted = true;
                _ = InitializeBrowserAsync();
            }
        };
    }

    private ResourcePackTabViewModel? ViewModel => DataContext as ResourcePackTabViewModel;

    private void ReportError(string what, Exception ex)
    {
        if (ViewModel is { } vm)
        {
            vm.ReportError(what, ex);
        }
        else if (Window.GetWindow(this) is MainWindow { Toasts: { } toasts })
        {
            toasts.Error($"{what}: {ex.Message}");
        }
    }

    private async Task InitializeBrowserAsync()
    {
        try
        {
            await EmbeddedBrowser.InitializeAsync(Web);
            var core = Web.CoreWebView2;
            await core.AddScriptToExecuteOnDocumentCreatedAsync(CaptureScript);
            core.WebMessageReceived += Core_WebMessageReceived;
            core.WebResourceResponseReceived += Core_WebResourceResponseReceived;
            core.NewWindowRequested += Core_NewWindowRequested;
            core.SourceChanged += (_, _) => AddressText.Text = core.Source;
            core.NavigationCompleted += (_, e) =>
            {
                if (!e.IsSuccess && Web.Visibility != Visibility.Visible)
                {
                    PlaceholderText.Text = $"Couldn't load mcpacks.dev ({e.WebErrorStatus}). Check your internet connection and press Reload.";
                }
                else
                {
                    Web.Visibility = Visibility.Visible;
                    Placeholder.Visibility = Visibility.Collapsed;
                }
            };
            core.Navigate(ResourcePackTabViewModel.SiteUrl);
        }
        catch (Exception ex)
        {
            _initStarted = false; // allow a retry next time the tab is shown
            Web.Visibility = Visibility.Collapsed;
            Placeholder.Visibility = Visibility.Visible;
            PlaceholderText.Text = EmbeddedBrowser.DescribeFailure(ex);
        }
    }

    private static bool IsMcPacks(string? url)
        => Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == "https" &&
           (uri.Host == McPacksHost || uri.Host.EndsWith("." + McPacksHost, StringComparison.OrdinalIgnoreCase));

    private void Core_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            if (!IsMcPacks(e.Source) || ViewModel is not { } vm)
            {
                return;
            }

            using var doc = JsonDocument.Parse(e.TryGetWebMessageAsString());
            var root = doc.RootElement;
            var kind = root.TryGetProperty("kind", out var k) ? k.GetString() : null;
            HostedPackInfo? pack = kind switch
            {
                "page" => ResourcePackProperties.TryParseMcPacksPage(root.GetProperty("page").GetString()),
                "dom" => ResourcePackProperties.TryCreateHostedPack(
                    root.GetProperty("url").GetString(), root.GetProperty("sha1").GetString(), null),
                _ => null,
            };
            if (pack is not null)
            {
                vm.OnPackCaptured(pack);
            }
        }
        catch
        {
            // Page messages are best-effort; malformed ones are ignored.
        }
    }

    /// <summary>Reads the Inertia JSON response of the upload (and any page visit) directly.</summary>
    private async void Core_WebResourceResponseReceived(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
    {
        try
        {
            if (!IsMcPacks(e.Request.Uri) || !e.Request.Headers.Contains("X-Inertia") || e.Response.StatusCode != 200)
            {
                return;
            }

            using var stream = await e.Response.GetContentAsync();
            if (stream is null || ViewModel is not { } vm)
            {
                return;
            }

            using var reader = new StreamReader(stream);
            var buffer = new char[1 << 20];
            var read = await reader.ReadBlockAsync(buffer, 0, buffer.Length);
            if (ResourcePackProperties.TryParseMcPacksPage(new string(buffer, 0, read)) is { } pack)
            {
                vm.OnPackCaptured(pack);
            }
        }
        catch
        {
            // The body may be unavailable (redirects, aborted requests); the page script covers it.
        }
    }

    /// <summary>mcpacks.dev pages open in place; anything else (docs, socials) in the real browser.</summary>
    private void Core_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        try
        {
            e.Handled = true;
            if (IsMcPacks(e.Uri))
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
            ReportError("Could not open the link", ex);
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
            ReportError("Could not go back", ex);
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
            ReportError("Could not reload", ex);
        }
    }

    private void Home_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Web.CoreWebView2?.Navigate(ResourcePackTabViewModel.SiteUrl);
        }
        catch (Exception ex)
        {
            ReportError("Could not open mcpacks.dev", ex);
        }
    }

    private void OpenExternal_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var current = Web.CoreWebView2?.Source;
            EmbeddedBrowser.OpenExternal(IsMcPacks(current) ? current! : ResourcePackTabViewModel.SiteUrl);
        }
        catch (Exception ex)
        {
            ReportError("Could not open the browser", ex);
        }
    }
}
