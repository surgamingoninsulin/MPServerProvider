using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MPServerProvider.App.ViewModels;
using MPServerProvider.App.Views.Dialogs;

namespace MPServerProvider.App.Views.Pages;

public partial class SettingsPage : UserControl, INavigationAware
{
    private readonly DispatcherTimer _liveTimer;

    private SettingsPageViewModel? _viewModel;
    private bool _settingCurseForgeKeyFromViewModel;
    private bool _settingModIoKeyFromViewModel;

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

                // PasswordBox.Password can't be data-bound (WPF deliberately blocks that for
                // secrets), so it's synced by hand: seed it once from the loaded setting here,
                // then push edits back on PasswordChanged below.
                _settingCurseForgeKeyFromViewModel = true;
                try
                {
                    CurseForgeApiKeyBox.Password = _viewModel.CurseForgeApiKey;
                }
                finally
                {
                    _settingCurseForgeKeyFromViewModel = false;
                }
                _settingModIoKeyFromViewModel = true;
                try
                {
                    ModIoApiKeyBox.Password = _viewModel.ModIoApiKey;
                }
                finally
                {
                    _settingModIoKeyFromViewModel = false;
                }
                return;
            }

            _viewModel?.OnNavigatedTo();
        }
        catch (Exception ex)
        {
            ReportError("Could not load settings", ex);
        }
    }

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

    private void CurseForgeApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_settingCurseForgeKeyFromViewModel || ViewModel is not { } vm)
        {
            return;
        }
        vm.CurseForgeApiKey = CurseForgeApiKeyBox.Password;
    }

    private void ModIoApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_settingModIoKeyFromViewModel || ViewModel is not { } vm)
        {
            return;
        }
        vm.ModIoApiKey = ModIoApiKeyBox.Password;
    }

    private void ReportError(string what, Exception ex)
    {
        if (Window.GetWindow(this) is MainWindow { Toasts: { } toasts })
        {
            toasts.Error($"{what}: {ex.Message}");
        }
    }
}
