using System.IO;
using System.Windows;
using System.Windows.Threading;
using MPServerProvider.App.Toasts;
using MPServerProvider.App.Views;

namespace MPServerProvider.App;

/// <summary>
/// Port of <c>MPServerProvider.App.App</c> (decompile token 0x02000004): shows the splash,
/// builds the <see cref="AppServices"/> composition root off the UI thread, then constructs and
/// shows <see cref="MainWindow"/> and closes the splash. Progress sequencing mirrors
/// <c>OnAppStartup</c>/the proven Rust port's <c>main.rs</c>:
/// "Initializing services…" -&gt; "Loading game library…" -&gt; "Fetching official artwork…" -&gt;
/// "You are up to date. Finalizing...".
/// <para>
/// Unlike the Rust/Slint port (which needed a manual background thread + mpsc channel + polling
/// timer because Slint's event loop isn't tokio-aware), WPF's <see cref="Dispatcher"/> lets a
/// background <see cref="System.Threading.Tasks.Task"/> marshal splash updates straight back to
/// the UI thread via <see cref="Dispatcher.InvokeAsync(Action)"/> — no channel/poll-timer needed.
/// </para>
/// </summary>
public partial class App : Application
{
    public AppServices? Services { get; private set; }

    /// <summary>
    /// Global safety net: the Mods/Settings pages (and likely others) were crashing the entire
    /// application on real-world filesystem/WMI edge cases that no single targeted fix could
    /// fully rule out in advance — e.g. <c>ModLibraryService.Scan()</c> enumerating a locked or
    /// permission-denied mods subfolder, or a WMI/perf-counter hiccup. Rather than keep
    /// whack-a-moling individual call sites, this catches ANY otherwise-unhandled exception on
    /// the UI thread, writes the full details to <c>%LocalAppData%\MPServerProvider\crash.log</c>
    /// (so the exact cause is always recoverable after the fact), shows the message instead of
    /// silently dying, and marks the exception handled so the app keeps running. A page that hit
    /// this will be left in a partially-refreshed state, which is strictly better than the whole
    /// application disappearing.
    /// </summary>
    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        System.Threading.Tasks.TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogAndReportCrash(e.Exception, "UI thread", fatal: false);
        e.Handled = true;
    }

    private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            LogAndReportCrash(ex, "background thread (unrecoverable)", fatal: e.IsTerminating);
        }
    }

    private void OnUnobservedTaskException(object? sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
    {
        LogAndReportCrash(e.Exception, "unobserved Task", fatal: false);
        e.SetObserved();
    }

    // -- Crash dialog rate limiting (audit #33) ---------------------------------------------------

    private static readonly TimeSpan SameCrashDialogInterval = TimeSpan.FromMinutes(1);
    private static readonly object CrashDialogLock = new();
    private static readonly Dictionary<string, DateTime> LastCrashDialogAt = new();
    private static bool _crashDialogOpen;

    /// <summary>Appends <paramref name="ex"/> to <c>%LocalAppData%\MPServerProvider\crash.log</c>. Never throws.</summary>
    internal static void LogCrash(Exception ex, string origin)
    {
        try
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MPServerProvider");
            Directory.CreateDirectory(dir);
            var logPath = Path.Combine(dir, "crash.log");
            File.AppendAllText(logPath,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ({origin}){Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}");
        }
        catch
        {
            // If we can't even write the crash log, there's nothing more to do about it.
        }
    }

    /// <summary>
    /// Always logs; shows at most one crash dialog at a time and the same message at most once a
    /// minute — a timer tick that throws every time used to stack a new modal per tick. Non-fatal
    /// reports from other threads are marshalled to the UI thread (never shown on the finalizer
    /// thread, which an unobserved-Task report runs on).
    /// </summary>
    private static void LogAndReportCrash(Exception ex, string origin, bool fatal)
    {
        LogCrash(ex, origin);

        var key = $"{ex.GetType().FullName}: {ex.Message}";
        lock (CrashDialogLock)
        {
            var now = DateTime.UtcNow;
            if (_crashDialogOpen ||
                (LastCrashDialogAt.TryGetValue(key, out var last) && now - last < SameCrashDialogInterval))
            {
                return;
            }

            _crashDialogOpen = true;
            LastCrashDialogAt[key] = now;
        }

        void ShowDialog()
        {
            try
            {
                MessageBox.Show(
                    fatal
                        ? $"MP Server Provider hit an unrecoverable error and has to close.\n\n{ex.Message}\n\n" +
                          "Full details were written to %LocalAppData%\\MPServerProvider\\crash.log."
                        : $"Something went wrong, but MP Server Provider will keep running.\n\n{ex.Message}\n\n" +
                          "Full details were written to %LocalAppData%\\MPServerProvider\\crash.log.",
                    "MP Server Provider",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch
            {
                // Showing the dialog itself failed (e.g. no UI thread left) — the log is the fallback.
            }
            finally
            {
                lock (CrashDialogLock)
                {
                    _crashDialogOpen = false;
                }
            }
        }

        try
        {
            var dispatcher = Current?.Dispatcher;
            if (fatal || dispatcher is null || dispatcher.CheckAccess() || dispatcher.HasShutdownStarted)
            {
                ShowDialog();
            }
            else
            {
                dispatcher.BeginInvoke(ShowDialog);
            }
        }
        catch
        {
            lock (CrashDialogLock)
            {
                _crashDialogOpen = false;
            }
        }
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);
            await StartAsync(e);
        }
        catch (Exception ex)
        {
            // async void: nothing may escape. A failure after the splash (e.g. the main window's
            // constructor) used to leave only the splash on screen, with no way to exit.
            LogCrash(ex, "startup");
            try
            {
                MessageBox.Show($"Startup failed: {ex.Message}", "MP Server Provider", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                // Logged above.
            }

            Shutdown(-1);
        }
    }

    private async Task StartAsync(StartupEventArgs e)
    {
        var splash = new SplashWindow();
        splash.Show();

        AppServices services;
        try
        {
            services = await System.Threading.Tasks.Task.Run(async () =>
            {
                await ReportAsync(splash, "Initializing services…", 0.1);
                var built = AppServices.Initialize();

                await ReportAsync(splash, "Loading game library…", 0.4);
                // AppServices.Initialize() already reloaded the library synchronously; this is
                // a cheap yield so the message above is actually visible rather than flashing by,
                // matching the proven Rust port's same 150ms pacing delay.
                await System.Threading.Tasks.Task.Delay(150);

                await ReportAsync(splash, "Fetching official artwork…", 0.7);
                try
                {
                    await built.Artwork.PrefetchArtworkAsync(new CachePathsAdapter(built.Paths), Array.Empty<int>());
                }
                catch
                {
                    // Best-effort prefetch; startup must not fail if artwork fetch fails (e.g. no
                    // network), matching the proven Rust port's same tolerance.
                }

                // Load the installed servers now (still off the UI thread): Home's "servers
                // installed" tile and the Library's status badges read this list, and it used to
                // stay empty until the My Servers page was first opened.
                try
                {
                    built.Servers.Reload();
                }
                catch
                {
                    // My Servers reloads again when opened.
                }

                await ReportAsync(splash, "You are up to date. Finalizing...", 1.0);
                await System.Threading.Tasks.Task.Delay(200);

                return built;
            });
        }
        catch (Exception ex)
        {
            LogCrash(ex, "startup (services)");
            splash.Close();
            MessageBox.Show($"Startup failed: {ex.Message}", "MP Server Provider", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
            return;
        }

        try
        {
            Services = services;

            var toasts = new ToastService();
            services.Tunnel.Notice += notice =>
            {
                try
                {
                    switch (notice.Level)
                    {
                        case Core.Services.TunnelNoticeLevel.Success: toasts.Success(notice.Message); break;
                        case Core.Services.TunnelNoticeLevel.Warning: toasts.Warning(notice.Message); break;
                        case Core.Services.TunnelNoticeLevel.Error: toasts.Error(notice.Message); break;
                        default: toasts.Info(notice.Message); break;
                    }
                }
                catch
                {
                    // A toast must never break the tunnel's notice path.
                }
            };
            // Remove UPnP forwards a previous (crashed) run left on the router. Background, never throws.
            _ = services.Tunnel.CleanupStaleMappingsAsync();
            var main = new MainWindow(toasts);
            MainWindow = main;

            // "--minimized" is what the Windows sign-in Run entry passes when "Start minimized"
            // is on (SettingsService.ApplyStartOnBoot).
            if (e.Args.Any(a => string.Equals(a, "--minimized", StringComparison.OrdinalIgnoreCase)))
            {
                main.WindowState = WindowState.Minimized;
            }

            main.Show();
            toasts.Info("MP Server Provider is ready.");

            _ = RestoreRunningServersAsync(services, toasts);
        }
        finally
        {
            splash.Close();
        }
    }

    /// <summary>
    /// "Restart servers that were running on last shutdown": only when that setting is on.
    /// Runs in the background after the window is up; each failure is a toast. Never throws.
    /// </summary>
    private static async Task RestoreRunningServersAsync(AppServices services, ToastService toasts)
    {
        try
        {
            if (!services.Settings.Settings.RestoreRunningServers || services.RunningTracker.Remembered.Count == 0)
            {
                return;
            }

            await System.Threading.Tasks.Task.Run(() => services.Servers.Reload());
            var results = await services.RunningTracker.RestoreAsync(services.Servers.Find, services.Processes);
            var started = results.Where(r => r.Error is null).Select(r => r.Name).ToList();
            if (started.Count > 0)
            {
                toasts.Success($"Restarted {string.Join(", ", started)} (running when MP Server Provider last closed).");
            }

            foreach (var (name, error) in results.Where(r => r.Error is not null))
            {
                toasts.Error($"Could not restart {name}: {error}");
            }
        }
        catch (Exception ex)
        {
            LogCrash(ex, "restoring running servers");
            toasts.Error($"Could not restart the servers from last time: {ex.Message}");
        }
    }
    private static Task ReportAsync(SplashWindow splash, string status, double fraction)
        => splash.Dispatcher.InvokeAsync(() => splash.Report(status, fraction)).Task;

    /// <summary>Adapter: <see cref="Core.Services.AppPaths"/> already exposes
    /// <c>ArtworkCacheDir</c>, it just doesn't implement
    /// <see cref="Core.Services.ArtworkCacheService.ICachePaths"/> directly — same small
    /// App-side adapter pattern as <see cref="AppServices"/>'s persistence/lookup adapters.</summary>
    private sealed class CachePathsAdapter(Core.Services.AppPaths paths) : Core.Services.ArtworkCacheService.ICachePaths
    {
        public string ArtworkCacheDir => paths.ArtworkCacheDir;
    }

    private bool _gracefulShutdownDone;

    /// <summary>Called by <see cref="MainWindow"/> once it has stopped every server and removed
    /// the tunnel forwards, so <see cref="OnExit"/> has nothing slow left to do.</summary>
    internal void MarkGracefulShutdownDone() => _gracefulShutdownDone = true;

    protected override void OnExit(ExitEventArgs e)
    {
        // Port of AppServices.Shutdown(). Normally the main window already stopped every server
        // gracefully (with an overlay, off the UI thread). This path only has work left when the
        // app exits without that (e.g. Windows signing out): give servers a short, bounded
        // chance, then kill whatever is left (Dispose; the kill-on-close jobs back that up).
        if (Services is { } services)
        {
            try
            {
                services.RunningTracker.Freeze();
                if (!_gracefulShutdownDone)
                {
                    if (services.Processes.ActiveServers.Count > 0)
                    {
                        System.Threading.Tasks.Task.Run(() => services.Processes.StopAllAsync(TimeSpan.FromSeconds(4)))
                            .Wait(TimeSpan.FromSeconds(5));
                    }

                    // Removes the UPnP forwards this app created (bounded wait) and stops playit.
                    services.Tunnel.Shutdown(TimeSpan.FromSeconds(3));
                }
            }
            catch (Exception ex)
            {
                LogCrash(ex, "shutdown");
            }

            try
            {
                services.Processes.Dispose();
                services.Rcon.Dispose();
            }
            catch (Exception ex)
            {
                LogCrash(ex, "shutdown (dispose)");
            }
        }

        base.OnExit(e);
    }
}