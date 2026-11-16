using System.Diagnostics;
using System.IO;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace MPServerProvider.App.Views.ServerTabs;

internal static class EmbeddedBrowser
{
    private static Task<CoreWebView2Environment>? _environment;

    public static readonly System.Drawing.Color Background = System.Drawing.Color.FromArgb(0x11, 0x15, 0x1F);

    public static string UserDataFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MPServerProvider", "WebView2");

    public static Task<CoreWebView2Environment> GetEnvironmentAsync()
    {
        if (_environment is { IsFaulted: false, IsCanceled: false } existing)
        {
            return existing;
        }

        Directory.CreateDirectory(UserDataFolder);
        _environment = CoreWebView2Environment.CreateAsync(browserExecutableFolder: null, userDataFolder: UserDataFolder);
        return _environment;
    }

    public static async Task InitializeAsync(WebView2 view)
    {
        if (view.CoreWebView2 is not null)
        {
            return;
        }

        view.DefaultBackgroundColor = Background;
        var environment = await GetEnvironmentAsync();
        await view.EnsureCoreWebView2Async(environment);
        var settings = view.CoreWebView2!.Settings;
        settings.AreDevToolsEnabled = false;
        settings.IsStatusBarEnabled = false;
        settings.AreDefaultContextMenusEnabled = true;

        // The app hides every scrollbar (mouse wheel / touch still scroll); do the same for the
        // embedded pages so mcpacks.dev and the LuckPerms editor don't show a native one.
        await view.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
            "(function(){var add=function(){var s=document.createElement('style');" +
            "s.textContent='html,body,*{scrollbar-width:none!important}::-webkit-scrollbar{display:none!important;width:0!important;height:0!important}';" +
            "(document.head||document.documentElement).appendChild(s);};" +
            "if(document.documentElement){add();}else{document.addEventListener('DOMContentLoaded',add);}})();");
    }

    public static string DescribeFailure(Exception ex) => ex switch
    {
        WebView2RuntimeNotFoundException =>
            "The Microsoft Edge WebView2 Runtime isn't installed, so the page can't be shown inside the app. " +
            "Install it from https://go.microsoft.com/fwlink/p/?LinkId=2124703 (free, from Microsoft) or use \"Open in browser\".",
        _ => $"The embedded browser couldn't start ({ex.Message}). Use \"Open in browser\" instead.",
    };

    public static void OpenExternal(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
        {
            throw new InvalidOperationException("Only web links can be opened.");
        }

        Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
    }
}
