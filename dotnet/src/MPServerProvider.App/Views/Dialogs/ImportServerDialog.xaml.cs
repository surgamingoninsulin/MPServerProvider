using System.ComponentModel;
using System.IO;
using System.Windows;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Views.Dialogs;

/// <summary>
/// Imports an existing server folder as a server of <see cref="GameDefinition"/>: shows what
/// <see cref="ServerImportService.Detect"/> found, then copies the folder into the servers folder
/// (default) or registers it in place. Can't be closed while files are being copied.
/// </summary>
public partial class ImportServerDialog : DialogWindow
{
    private readonly AppServices _services;
    private readonly ToastService? _toasts;
    private readonly GameDefinition _game;
    private readonly ServerImportService _importer;
    private ServerImportDetection? _detection;
    private int _detectVersion;
    private bool _busy;

    public ImportServerDialog(AppServices services, ToastService? toasts, GameDefinition game, string folder)
    {
        _services = services;
        _toasts = toasts;
        _game = game;
        _importer = new ServerImportService(services.Servers);

        InitializeComponent();
        Title = $"Import {game.Name} Server";
        TitleText.Text = Title;
        Loaded += (_, _) => SetFolder(folder);
    }

    private async void SetFolder(string folder)
    {
        var version = ++_detectVersion;
        FolderBox.Text = folder;
        if (string.IsNullOrWhiteSpace(NameBox.Text) || NameBox.Tag as string == NameBox.Text)
        {
            var suggested = SuggestName(Path.GetFileName(Path.TrimEndingDirectorySeparator(folder)));
            NameBox.Text = suggested;
            NameBox.Tag = suggested; // still the suggestion: replace it when the folder changes
        }

        ShowDetection("", "Looking at the folder…", string.Empty, string.Empty, ok: true);
        ImportButton.IsEnabled = false;
        StatusText.Text = string.Empty;

        ServerImportDetection detection;
        string? folderProblem;
        try
        {
            (detection, folderProblem) = await Task.Run(() => (ServerImportService.Detect(_game, folder), _importer.CheckFolder(folder)));
        }
        catch (Exception ex)
        {
            detection = ServerImportDetection.Fail($"The folder couldn't be read: {ex.Message}");
            folderProblem = null;
        }

        if (version != _detectVersion || !IsLoaded)
        {
            return;
        }

        _detection = detection;
        if (folderProblem is not null || !detection.IsValid)
        {
            ShowDetection("", $"Can't import this folder", folderProblem ?? detection.Problem!, string.Empty, ok: false);
            return;
        }

        ShowDetection("", $"{_game.Name} server found", detection.Summary, string.Join("\n", detection.Notes), ok: true);
        if (detection.Port is { } port)
        {
            PortBox.Text = port.ToString();
        }
        else if (string.IsNullOrWhiteSpace(PortBox.Text))
        {
            PortBox.Text = (_game.Network.DefaultPort > 0 ? _game.Network.DefaultPort : 25565).ToString();
        }

        ImportButton.IsEnabled = true;
    }

    /// <summary>The folder name, made unique against existing servers ("Survival", "Survival 2", …).</summary>
    private string SuggestName(string folderName)
    {
        var baseName = string.IsNullOrWhiteSpace(folderName) || ServerRepository.ValidateServerName(folderName) is not null
            ? $"{_game.Name} Server"
            : folderName.Trim();
        if (baseName.Length > ServerRepository.MaxServerNameLength - 4)
        {
            baseName = baseName[..(ServerRepository.MaxServerNameLength - 4)].Trim();
        }

        var name = baseName;
        for (var i = 2; _services.Servers.NameTaken(name) && i < 100; i++)
        {
            name = $"{baseName} {i}";
        }

        return name;
    }

    private void ShowDetection(string icon, string title, string detail, string notes, bool ok)
    {
        DetectionIcon.Text = icon;
        DetectionIcon.Foreground = (System.Windows.Media.Brush)FindResource(ok ? "SuccessBrush" : "DangerBrush");
        DetectionTitle.Text = title;
        DetectionDetail.Text = detail;
        DetectionDetail.Visibility = detail.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
        DetectionNotes.Text = notes;
        DetectionNotes.Visibility = notes.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var start = Directory.Exists(FolderBox.Text) ? FolderBox.Text : null;
        if (FolderBrowserDialog.Pick(this, $"Select the {_game.Name} server folder to import", start) is { } folder)
        {
            SetFolder(folder);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    protected override void OnClosing(CancelEventArgs e)
    {
        if (_busy)
        {
            e.Cancel = true;
            ProgressStatusText.Text = "Please wait for the server files to finish copying…";
        }

        base.OnClosing(e);
    }

    private async void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        if (_detection is not { IsValid: true })
        {
            return;
        }

        var name = NameBox.Text.Trim();
        if (ServerRepository.ValidateServerName(name) is { } nameError)
        {
            StatusText.Text = nameError;
            return;
        }

        if (_services.Servers.NameTaken(name))
        {
            StatusText.Text = $"A server named '{name}' already exists.";
            return;
        }

        if (!int.TryParse(PortBox.Text.Trim(), out var port) || port is <= 0 or > 65535)
        {
            StatusText.Text = "Enter a port between 1 and 65535.";
            return;
        }

        StatusText.Text = string.Empty;
        _busy = true;
        ImportButton.IsEnabled = false;
        BrowseButton.IsEnabled = false;
        CancelButton.IsEnabled = false;
        ProgressPanel.Visibility = Visibility.Visible;
        ProgressBarControl.IsIndeterminate = true;
        var copy = CopyRadio.IsChecked == true;
        ProgressStatusText.Text = copy ? "Preparing to copy…" : "Registering the server…";

        var progress = new Progress<InstallProgress>(p =>
        {
            ProgressStatusText.Text = p.Status;
            ProgressBarControl.IsIndeterminate = p.TotalBytes is null && p.Percent <= 0;
            ProgressBarControl.Value = p.Percent;
        });

        var succeeded = false;
        try
        {
            var folder = FolderBox.Text;
            var instance = await _importer.ImportAsync(_game, folder, name, port, copy, progress);
            succeeded = true;
            _toasts?.Success($"Imported '{instance.ServerName}'. Find it in My Servers.");
        }
        catch (Exception ex)
        {
            StatusText.Text = ex.Message;
            ProgressPanel.Visibility = Visibility.Collapsed;
        }
        finally
        {
            _busy = false;
            ImportButton.IsEnabled = !succeeded;
            BrowseButton.IsEnabled = true;
            CancelButton.IsEnabled = true;
        }

        if (succeeded && IsLoaded)
        {
            DialogResult = true;
        }
    }
}
