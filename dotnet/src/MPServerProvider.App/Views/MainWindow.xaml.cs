using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.App.Views.Pages;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.Views;

/// <summary>
/// Port of <c>MPServerProvider.App.Views.MainWindow</c> (decompile token 0x02000010): the
/// borderless-chrome shell. Navigation mechanism: each sidebar <c>RadioButton</c>'s
/// <c>Tag</c> names a page key; <see cref="NavButton_Checked"/> swaps <see cref="PageHost"/>'s
/// content to a cached page instance and calls <see cref="INavigationAware.OnNavigatedTo"/> on
/// it if it implements that interface, mirroring the decompiled nav-aware page lifecycle.
/// </summary>
public partial class MainWindow : Window
{
    private readonly Dictionary<string, UserControl> _pages = new();
    public ToastService Toasts { get; }

    public MainWindow() : this(new ToastService())
    {
    }

    public MainWindow(ToastService toasts)
    {
        Toasts = toasts;
        InitializeComponent();
        DataContext = this;

        _pages["home"] = new HomePage();
        _pages["library"] = new LibraryPage();
        _pages["mods"] = new ModsPage();
        _pages["servers"] = new MyServersPage();
        _pages["settings"] = new SettingsPage();

        // Clicking the already-checked pill doesn't raise Checked, so without this a user on a
        // Game Detail page (hosted outside the pill set) couldn't get back by clicking the pill
        // of the page they came from.
        foreach (var pill in Nav.Children.OfType<RadioButton>())
        {
            pill.Click += NavButton_Click;
        }

        NavigateTo("home");
    }

    /// <summary>Selects <paramref name="serverName"/> on the My Servers page (navigate there first).</summary>
    public void SelectServer(string serverName)
    {
        if (_pages.TryGetValue("servers", out var page) && page is MyServersPage servers)
        {
            servers.SelectServer(serverName);
        }
    }

    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: string key } && _pages.TryGetValue(key, out var page)
            && !ReferenceEquals(PageHost.Content, page))
        {
            NavigateTo(key);
        }
    }

    /// <summary>
    /// Shows the Game Detail page for <paramref name="game"/> in the page host. The nav pill of
    /// <paramref name="returnToPageKey"/> (e.g. <c>"library"</c> or <c>"home"</c>) stays
    /// selected, since the detail page is a drill-down of that section; its Back button returns
    /// there via <see cref="NavigateToPage"/>.
    /// </summary>
    public void ShowGameDetail(GameDefinition game, string returnToPageKey)
    {
        try
        {
            if (!_pages.ContainsKey(returnToPageKey))
            {
                returnToPageKey = "library";
            }

            SetPage(new GameDetailPage(game, returnToPageKey));
        }
        catch (Exception ex)
        {
            Toasts.Error($"Couldn't open {game.Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Navigates to one of the cached top-level pages by key and keeps the nav pills in sync
    /// (used by the Game Detail page's Back button).
    /// </summary>
    public void NavigateToPage(string key)
    {
        var pill = Nav.Children.OfType<RadioButton>().FirstOrDefault(r => r.Tag as string == key);
        if (pill is { IsChecked: false })
        {
            pill.IsChecked = true; // raises Checked -> NavigateTo
            return;
        }

        NavigateTo(key);
    }

    private void NavButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: string key })
        {
            NavigateTo(key);
        }
    }

    private void NavigateTo(string key)
    {
        if (!_pages.TryGetValue(key, out var page))
        {
            return;
        }

        SetPage(page);
        if (page is INavigationAware aware)
        {
            aware.OnNavigatedTo();
        }
    }

    /// <summary>Swaps the hosted page, telling the page being left (so it can stop its poll
    /// timers) via <see cref="INavigationAware.OnNavigatedFrom"/>.</summary>
    private void SetPage(UserControl page)
    {
        var previous = PageHost.Content;
        if (!ReferenceEquals(previous, page) && previous is INavigationAware leaving)
        {
            try
            {
                leaving.OnNavigatedFrom();
            }
            catch (Exception ex)
            {
                Toasts.Error($"Couldn't leave the page cleanly: {ex.Message}");
            }
        }

        PageHost.Content = page;
    }

    // -- Closing: stop servers gracefully first -------------------------------------------------

    private bool _closeAllowed;
    private bool _stoppingServers;

    /// <summary>
    /// If servers are running, the close is postponed: a "Stopping servers…" overlay is shown
    /// while every server gets its graceful stop (in parallel, ~30 s each at most), the tunnel
    /// forwards are removed, and only then does the window really close. Nothing here blocks the
    /// UI thread.
    /// </summary>
    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        if (e.Cancel || _closeAllowed || Application.Current is not App { Services: { } services })
        {
            return;
        }

        try
        {
            if (services.Processes.ActiveServers.Count == 0)
            {
                services.RunningTracker.Freeze();
                return;
            }
        }
        catch
        {
            return;
        }

        e.Cancel = true;
        if (!_stoppingServers)
        {
            _stoppingServers = true;
            _ = StopServersThenCloseAsync(services);
        }
    }

    private async Task StopServersThenCloseAsync(AppServices services)
    {
        try
        {
            // Servers stopped by the app closing are still "running at last shutdown".
            services.RunningTracker.Freeze();
            var names = services.Processes.ActiveServers.Select(s => s.ServerName).ToList();
            ShutdownDetailText.Text = names.Count == 1
                ? $"Giving {names[0]} time to save its world before MP Server Provider closes."
                : $"Giving {names.Count} servers ({string.Join(", ", names)}) time to save their worlds before MP Server Provider closes.";
            ShutdownOverlay.Visibility = Visibility.Visible;

            await services.Processes.StopAllAsync(Core.Services.ServerProcessService.DefaultStopTimeout);
            ShutdownDetailText.Text = "Removing port forwards…";
            await Task.Run(() => services.Tunnel.Shutdown(TimeSpan.FromSeconds(4)));
            (Application.Current as App)?.MarkGracefulShutdownDone();
        }
        catch (Exception ex)
        {
            App.LogCrash(ex, "stopping servers on close");
        }
        finally
        {
            _closeAllowed = true;
            try
            {
                Close();
            }
            catch (InvalidOperationException)
            {
                // Already closing.
            }
        }
    }

    private void ForceQuit_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ForceQuitButton.IsEnabled = false;
            ShutdownDetailText.Text = "Stopping the servers immediately…";
            if (Application.Current is App { Services: { } services })
            {
                foreach (var server in services.Processes.ActiveServers)
                {
                    services.Processes.ProcessOf(server)?.Kill();
                }
            }
        }
        catch (Exception ex)
        {
            App.LogCrash(ex, "force quit");
        }
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ToggleMaximizeRestore();
            return;
        }
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void Fullscreen_Click(object sender, RoutedEventArgs e) => ToggleMaximizeRestore();

    private void MaxRestore_Click(object sender, RoutedEventArgs e) => ToggleMaximizeRestore();

    private void ToggleMaximizeRestore()
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    // ---- maximize to the work area --------------------------------------------
    // A WindowStyle=None window maximizes to the whole monitor, so the bottom of the app
    // (e.g. the console's command box) ended up behind the taskbar. Answering
    // WM_GETMINMAXINFO with the monitor's work area keeps the maximized window above the
    // taskbar on every monitor, whichever edge the taskbar is docked to.

    private const int WmGetMinMaxInfo = 0x0024;
    private const uint MonitorDefaultToNearest = 2;

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        if (PresentationSource.FromVisual(this) is System.Windows.Interop.HwndSource source)
        {
            source.AddHook(WndProc);
        }
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WmGetMinMaxInfo)
        {
            try
            {
                var monitor = MonitorFromWindow(hwnd, MonitorDefaultToNearest);
                var info = new MonitorInfo { Size = System.Runtime.InteropServices.Marshal.SizeOf<MonitorInfo>() };
                if (monitor != IntPtr.Zero && GetMonitorInfo(monitor, ref info))
                {
                    var mmi = System.Runtime.InteropServices.Marshal.PtrToStructure<MinMaxInfo>(lParam);
                    var work = info.Work;
                    var bounds = info.Monitor;
                    mmi.MaxPosition = new NativePoint { X = work.Left - bounds.Left, Y = work.Top - bounds.Top };
                    mmi.MaxSize = new NativePoint { X = work.Right - work.Left, Y = work.Bottom - work.Top };

                    // Keep MinWidth/MinHeight working: they arrive in DIPs, the message in pixels.
                    var dpi = System.Windows.Media.VisualTreeHelper.GetDpi(this);
                    mmi.MinTrackSize = new NativePoint
                    {
                        X = (int)Math.Ceiling(MinWidth * dpi.DpiScaleX),
                        Y = (int)Math.Ceiling(MinHeight * dpi.DpiScaleY),
                    };
                    System.Runtime.InteropServices.Marshal.StructureToPtr(mmi, lParam, true);
                    handled = true;
                }
            }
            catch
            {
                // Fall back to the default maximize behavior.
            }
        }

        return IntPtr.Zero;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct MinMaxInfo
    {
        public NativePoint Reserved;
        public NativePoint MaxSize;
        public NativePoint MaxPosition;
        public NativePoint MinTrackSize;
        public NativePoint MaxTrackSize;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public NativeRect Monitor;
        public NativeRect Work;
        public uint Flags;
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint flags);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
