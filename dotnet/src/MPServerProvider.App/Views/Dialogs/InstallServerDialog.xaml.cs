using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.Views.Dialogs;

/// <summary>
/// Port of <c>InstallServerDialog</c>/<c>OnVariantChanged</c>/<c>OnInstallClick</c>
/// (0x0200000d): pick a variant -&gt; <see cref="Services.InstallService.GetVersionsAsync"/>
/// repopulates the version list -&gt; Install runs <see cref="Services.InstallService.InstallAsync"/>
/// on a background task, reporting <see cref="InstallProgress"/> back into
/// <see cref="ProgressPanel"/> -&gt; success closes the dialog and shows a toast (Library's card
/// grid refreshes itself right after <c>ShowDialog()</c> returns, same as the proven Rust
/// reference's <c>pages::library::refresh</c> call after a successful install).
/// </summary>
public partial class InstallServerDialog : DialogWindow
{
    private readonly AppServices _services;
    private readonly ToastService? _toasts;
    private readonly GameDefinition _game;
    private CancellationTokenSource? _cts;
    private bool _closeRequestedDuringInstall;

    public InstallServerDialog(AppServices services, ToastService? toasts, GameDefinition game)
    {
        _services = services;
        _toasts = toasts;
        _game = game;

        InitializeComponent();

        Title = "Install Server";
        TitleText.Text = $"Install {game.Name}";
        SubtitleText.Text = game.Description;
        SubtitleText.Visibility = string.IsNullOrWhiteSpace(game.Description) ? Visibility.Collapsed : Visibility.Visible;
        try
        {
            NameBox.Text = DefaultServerName(game.Name, _services.Servers.NameTaken);
        }
        catch
        {
            NameBox.Text = DefaultServerName(game.Name, _ => false);
        }
        // The game's default port unless another server (or another program, e.g. a Docker
        // container) already uses it; then the next free one, so a fresh install just starts.
        var defaultPort = game.Network.DefaultPort > 0 ? game.Network.DefaultPort : 25565;
        try
        {
            PortBox.Text = Core.Services.ServerProcessService.SuggestFreePort(
                defaultPort, game.Network.Protocol, _services.Servers.Servers.Select(s => s.ServerPort)).ToString();
        }
        catch
        {
            PortBox.Text = defaultPort.ToString();
        }

        var variants = game.Install.Variants ?? new List<string>();
        if (variants.Count > 0)
        {
            VariantCombo.ItemsSource = variants;
            VariantCombo.SelectedIndex = 0;
        }
        else
        {
            // Most games have an EMPTY Install.Variants list: there's nothing to pick, so hide
            // the Variant picker entirely and let Version span the row. VariantCombo's
            // SelectionChanged never fires in that case, so load the versions explicitly.
            VariantPanel.Visibility = Visibility.Collapsed;
            Grid.SetColumn(VersionPanel, 0);
            Grid.SetColumnSpan(VersionPanel, 3);
            _ = LoadVersionsAsync();
        }

        var isMinecraft = game.Id == "minecraft-java";
        EulaPanel.Visibility = isMinecraft ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Default server name: the game's own name when it already says "Server" (e.g. "7 Days to
    /// Die Dedicated Server" stays as is instead of "... Dedicated Server Server"), otherwise
    /// "{game} Server"; de-duplicated with " (2)", " (3)", ... against existing servers.
    /// </summary>
    internal static string DefaultServerName(string gameName, Func<string, bool> nameTaken)
    {
        var trimmed = (gameName ?? string.Empty).Trim();
        if (trimmed.Length == 0)
        {
            trimmed = "Game";
        }
        var baseName = Regex.IsMatch(trimmed, @"\bserver\b", RegexOptions.IgnoreCase)
            ? trimmed
            : $"{trimmed} Server";

        var name = baseName;
        var i = 2;
        while (nameTaken(name) && i < 1000)
        {
            name = $"{baseName} ({i++})";
        }
        return name;
    }

    private async void VariantCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            await LoadVersionsAsync();
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Could not load versions: {ex.Message}");
        }
    }

    private async Task LoadVersionsAsync()
    {
        var variant = VariantCombo.SelectedItem as string;
        try
        {
            var versions = await _services.Installer.GetVersionsAsync(_game, variant);
            VersionCombo.ItemsSource = versions;
            if (versions.Count > 0)
            {
                VersionCombo.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            VersionCombo.ItemsSource = null;
            _toasts?.Warning($"Could not load versions for {_game.Name}: {ex.Message}");
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (_cts is { } cts)
        {
            cts.Cancel();
            ProgressStatusText.Text = "Cancelling…";
            return;
        }
        Close();
    }

    /// <summary>
    /// Closing (X / Escape) while an install is running cancels it and keeps the window open
    /// until the install task has actually unwound, then closes — so a late success never tries
    /// to set <see cref="Window.DialogResult"/> on a closed window.
    /// </summary>
    protected override void OnClosing(CancelEventArgs e)
    {
        if (_cts is { } cts)
        {
            _closeRequestedDuringInstall = true;
            try { cts.Cancel(); } catch (ObjectDisposedException) { }
            ProgressStatusText.Text = "Cancelling…";
            e.Cancel = true;
        }
        base.OnClosing(e);
    }

    private async void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        if (_cts is not null)
        {
            return;
        }

        try
        {
            var name = NameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                _toasts?.Error("Enter a server name.");
                return;
            }

            if (!int.TryParse(PortBox.Text.Trim(), out var port) || port is <= 0 or > 65535)
            {
                _toasts?.Error("Enter a valid port number.");
                return;
            }

            _ = int.TryParse(RamMinBox.Text.Trim(), out var ramMin);
            _ = int.TryParse(RamMaxBox.Text.Trim(), out var ramMax);

            var eulaAccepted = EulaCheck.IsChecked == true;
            if (EulaPanel.Visibility == Visibility.Visible && !eulaAccepted)
            {
                _toasts?.Error("You must accept the Minecraft EULA to continue.");
                return;
            }

            var variant = VariantCombo.SelectedItem as string;
            var version = VersionCombo.SelectedItem as string;
            var description = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text.Trim();

            await RunInstallAsync(name, port, ramMin, ramMax, eulaAccepted, variant, version, description);
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Install failed: {ex.Message}");
        }
    }

    private async Task RunInstallAsync(string name, int port, int ramMin, int ramMax, bool eulaAccepted,
        string? variant, string? version, string? description)
    {
        InstallButton.IsEnabled = false;
        ProgressPanel.Visibility = Visibility.Visible;
        ProgressStatusText.Text = "Preparing…";
        ProgressBarControl.IsIndeterminate = true;
        ProgressBarControl.Value = 0;

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

        var succeeded = false;
        try
        {
            var instance = await _services.Installer.InstallAsync(
                _game, variant, version, name, port, ramMin, ramMax, description,
                eulaAccepted, progress, cts.Token);

            succeeded = true;
            _toasts?.Success($"Installed '{instance.ServerName}'.");
        }
        catch (OperationCanceledException)
        {
            _toasts?.Warning("Install cancelled.");
            ProgressStatusText.Text = "Install cancelled.";
            ProgressBarControl.IsIndeterminate = false;
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Install failed: {ex.Message}");
            ProgressStatusText.Text = ex.Message;
            ProgressBarControl.IsIndeterminate = false;
        }
        finally
        {
            _cts = null;
            cts.Dispose();
            InstallButton.IsEnabled = true;
        }

        if (!IsLoaded)
        {
            return;
        }
        if (succeeded)
        {
            DialogResult = true;
        }
        else if (_closeRequestedDuringInstall)
        {
            Close();
        }
    }
}
