using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.Views.Dialogs;

/// <summary>
/// Port of <c>ManualInstallDialog</c>'s "Create Server" flow (0x02000012): calls
/// <see cref="Services.InstallService.InstallAsync"/> against a <c>"manual"</c>-provider
/// <see cref="GameDefinition"/>, carrying the user-picked source folder in the version slot
/// (mirrors <c>ManualProvider.InstallAsync</c>, which reads <c>request.Version</c> as the folder
/// to copy from — see the proven Rust reference's <c>dialogs/manual_install.rs</c>).
/// </summary>
public partial class ManualInstallDialog : DialogWindow
{
    private readonly AppServices _services;
    private readonly ToastService? _toasts;
    private readonly GameDefinition _game;
    private bool _busy;

    public ManualInstallDialog(AppServices services, ToastService? toasts, GameDefinition game)
    {
        _services = services;
        _toasts = toasts;
        _game = game;

        InitializeComponent();

        Title = $"Create {game.Name} Server";
        TitleText.Text = Title;
        NameBox.Text = game.Name;
        PortBox.Text = (game.Network.DefaultPort > 0 ? game.Network.DefaultPort : 25565).ToString();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var dialog = new OpenFolderDialog { Title = "Select the folder containing the server files" };
            if (dialog.ShowDialog(this) == true)
            {
                FolderBox.Text = dialog.FolderName;
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Could not open the folder picker: {ex.Message}";
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    /// <summary>The manual copy can't be cancelled midway, so block closing while it runs.</summary>
    protected override void OnClosing(CancelEventArgs e)
    {
        if (_busy)
        {
            e.Cancel = true;
            StatusText.Text = "Please wait for the server files to finish copying…";
        }
        base.OnClosing(e);
    }

    private async void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var name = NameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                StatusText.Text = "Enter a server name.";
                return;
            }

            if (!int.TryParse(PortBox.Text.Trim(), out var port) || port is <= 0 or > 65535)
            {
                StatusText.Text = "Enter a valid port number.";
                return;
            }

            var folder = FolderBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(folder) || !System.IO.Directory.Exists(folder))
            {
                StatusText.Text = "Enter a folder that already contains the server files.";
                return;
            }

            StatusText.Text = string.Empty;
            InstallButton.IsEnabled = false;
            _busy = true;

            try
            {
                var noOpProgress = new Progress<InstallProgress>(_ => { });
                var instance = await _services.Installer.InstallAsync(
                    _game, null, folder, name, port, 0, 0, null, true, noOpProgress);

                _toasts?.Success($"Created server '{instance.ServerName}'.");
                _busy = false;
                if (IsLoaded)
                {
                    DialogResult = true;
                }
            }
            finally
            {
                _busy = false;
                InstallButton.IsEnabled = true;
            }
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Install failed: {ex.Message}");
            StatusText.Text = ex.Message;
        }
    }
}
