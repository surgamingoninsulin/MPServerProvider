using System.ComponentModel;
using System.Windows;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.Views.Dialogs;

/// <summary>
/// Updates an installed (and already stopped) server in place through
/// <see cref="Core.Services.InstallService.UpdateAsync"/>: lists the provider's versions, then
/// re-runs its installer into the server folder while keeping worlds and config files.
/// <see cref="Window.DialogResult"/> is <c>true</c> after a successful update.
/// </summary>
public partial class UpdateServerDialog : DialogWindow
{
    private readonly AppServices _services;
    private readonly ToastService? _toasts;
    private readonly ServerInstance _server;
    private readonly GameDefinition _game;
    private CancellationTokenSource? _cts;

    public UpdateServerDialog(AppServices services, ToastService? toasts, ServerInstance server, GameDefinition game)
    {
        _services = services;
        _toasts = toasts;
        _server = server;
        _game = game;

        InitializeComponent();

        TitleText.Text = $"Update {server.ServerName}";
        var type = string.IsNullOrWhiteSpace(server.ServerType) ? string.Empty : $" ({server.ServerType})";
        CurrentText.Text = $"{game.Name}{type} — currently {(string.IsNullOrWhiteSpace(server.ServerVersion) ? "unknown version" : server.ServerVersion)}";

        Loaded += async (_, _) => await LoadVersionsAsync();
        Closing += OnClosing;
    }

    private async Task LoadVersionsAsync()
    {
        VersionCombo.IsEnabled = false;
        VersionCombo.ItemsSource = new[] { "Loading versions…" };
        VersionCombo.SelectedIndex = 0;
        try
        {
            var variant = string.IsNullOrWhiteSpace(_server.ServerType) ? null : _server.ServerType;
            var versions = await _services.Installer.GetVersionsAsync(_game, variant);
            if (versions.Count == 0)
            {
                // Steam and other "always latest" providers have nothing to pick.
                versions = new List<string> { "latest" };
            }

            VersionCombo.ItemsSource = versions;
            VersionCombo.SelectedIndex = 0;
            VersionCombo.IsEnabled = versions.Count > 1;
            UpdateButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            VersionCombo.ItemsSource = new[] { "Couldn't load versions" };
            VersionCombo.SelectedIndex = 0;
            ProgressPanel.Visibility = Visibility.Visible;
            ProgressStatusText.Text = $"Couldn't load the version list: {ex.Message}";
            ProgressBarControl.Visibility = Visibility.Collapsed;
        }
    }

    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        if (_cts is not null)
        {
            return;
        }

        var version = VersionCombo.SelectedItem as string;
        if (string.Equals(version, "latest", StringComparison.OrdinalIgnoreCase))
        {
            version = null;
        }

        UpdateButton.IsEnabled = false;
        VersionCombo.IsEnabled = false;
        ProgressPanel.Visibility = Visibility.Visible;
        ProgressBarControl.Visibility = Visibility.Visible;
        ProgressStatusText.Text = "Preparing…";
        ProgressBarControl.IsIndeterminate = true;

        var cts = new CancellationTokenSource();
        _cts = cts;
        var progress = new Progress<InstallProgress>(p =>
        {
            if (cts.IsCancellationRequested)
            {
                return;
            }

            ProgressStatusText.Text = p.Status;
            if (p.Percent > 0)
            {
                ProgressBarControl.IsIndeterminate = false;
                ProgressBarControl.Value = p.Percent;
            }
        });

        try
        {
            await _services.Installer.UpdateAsync(_server, _game, version, progress, cts.Token);
            _toasts?.Success($"Updated {_server.ServerName} to {_server.ServerVersion}.");
            _cts = null;
            DialogResult = true;
            Close();
            return;
        }
        catch (OperationCanceledException)
        {
            ProgressStatusText.Text = "Update cancelled. Your settings were kept.";
        }
        catch (Exception ex)
        {
            ProgressStatusText.Text = $"Update failed: {ex.Message}";
            _toasts?.Error($"Update failed: {ex.Message}");
        }
        finally
        {
            _cts = null;
            cts.Dispose();
        }

        ProgressBarControl.IsIndeterminate = false;
        ProgressBarControl.Value = 0;
        UpdateButton.IsEnabled = true;
        VersionCombo.IsEnabled = (VersionCombo.ItemsSource as IList<string>)?.Count > 1;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (_cts is { } cts)
        {
            ProgressStatusText.Text = "Cancelling…";
            cts.Cancel();
            return;
        }

        Close();
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        // Closing mid-update would leave the folder half-written; cancel first instead.
        if (_cts is { } cts)
        {
            e.Cancel = true;
            ProgressStatusText.Text = "Cancelling…";
            cts.Cancel();
        }
    }
}
