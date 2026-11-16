using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MPServerProvider.App.ViewModels;
using MPServerProvider.App.Views.Dialogs;

namespace MPServerProvider.App.Views.Pages;

/// <summary>
/// Port of <c>MPServerProvider.App.Views.SettingsPage</c> (decompile token 0x0200001b). Storage
/// Management / Startup &amp; Behavior / System Resources / System Specifications / Firewall
/// Rules live in <see cref="SettingsPageViewModel"/>; this code-behind only owns the live-metrics
/// polling timer (a <see cref="DispatcherTimer"/> ticking every 2 seconds, per the task brief —
/// WPF's dispatcher makes this trivial compared to the proven Rust port's
/// <c>std::thread::spawn</c> + <c>invoke_from_event_loop</c> dance) and opening
/// <see cref="AddFirewallRuleDialog"/>, which isn't a plain command (it's a modal child window).
/// </summary>
public partial class SettingsPage : UserControl, INavigationAware
{
    private readonly DispatcherTimer _liveTimer;

    private SettingsPageViewModel? _viewModel;

    // Never cast DataContext: it's inherited from MainWindow (DataContext = itself) until our own
    // view model is assigned, so the old `(SettingsPageViewModel)DataContext` cast threw
    // InvalidCastException every time the Settings page opened.
    private SettingsPageViewModel? ViewModel => _viewModel;

    public SettingsPage()
    {
        InitializeComponent();

        _liveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _liveTimer.Tick += (_, _) => ViewModel?.RefreshLive();

        // Only sample CPU/RAM while the page is actually on screen (the shell swaps pages in and
        // out of its ContentControl, which raises Loaded/Unloaded each time).
        Loaded += (_, _) => { if (_viewModel is not null) _liveTimer.Start(); };
        Unloaded += (_, _) => _liveTimer.Stop();
    }

    /// <summary>
    /// Lazily builds the view model on first navigation: <see cref="MainWindow"/> constructs every
    /// page from its own constructor, before the window (and its <see cref="MainWindow.Toasts"/>)
    /// is reachable, so the view model can only be built once the shell has navigated here.
    /// </summary>
    public void OnNavigatedTo()
    {
        try
        {
            if (_viewModel is null && Application.Current is App { Services: { } services } &&
                Window.GetWindow(this) is MainWindow mainWindow)
            {
                _viewModel = new SettingsPageViewModel(services, mainWindow.Toasts);
                DataContext = _viewModel;
                _viewModel.Initialize();
                _liveTimer.Start();
                return;
            }

            _viewModel?.OnNavigatedTo();
        }
        catch (Exception ex)
        {
            ReportError("Could not load settings", ex);
        }
    }

    /// <summary>Port of <c>SettingsPage.OnAddRule</c>: opens <see cref="AddFirewallRuleDialog"/>,
    /// which performs the actual <c>FirewallService.AddRuleAsync</c> + settings persistence
    /// itself, then refreshes the rules list on a successful add.</summary>
    private void AddRuleButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (Application.Current is not App { Services: { } services })
            {
                return;
            }
            var dialog = new AddFirewallRuleDialog(services.Firewall, services.Settings)
            {
                Owner = Window.GetWindow(this),
            };

            if (dialog.ShowDialog() == true)
            {
                ViewModel?.RefreshRules();
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not add the firewall rule", ex);
        }
    }

    private void ReportError(string what, Exception ex)
    {
        if (Window.GetWindow(this) is MainWindow { Toasts: { } toasts })
        {
            toasts.Error($"{what}: {ex.Message}");
        }
    }
}
