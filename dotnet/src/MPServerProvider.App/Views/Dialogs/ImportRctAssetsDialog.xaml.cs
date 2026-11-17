using System.IO;
using System.Windows;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Views.Dialogs;

public partial class ImportRctAssetsDialog : DialogWindow
{
    private readonly string _serverRoot;

    private ImportRctAssetsDialog(string serverRoot)
    {
        _serverRoot = serverRoot;
        InitializeComponent();

        var (rct2, rct1) = OpenRct2Setup.ReadGameDataPaths(serverRoot);
        Rct2Box.Text = rct2 ?? string.Empty;
        Rct1Box.Text = rct1 ?? string.Empty;
    }

    /// <summary>Shows the dialog; returns true if the player saved a RCT2 path.</summary>
    public static bool Show(Window? owner, string serverRoot)
    {
        var dialog = new ImportRctAssetsDialog(serverRoot);
        if (owner is not null)
        {
            dialog.Owner = owner;
        }

        return dialog.ShowDialog() == true;
    }

    private void BrowseRct2_Click(object sender, RoutedEventArgs e)
    {
        if (FolderBrowserDialog.Pick(this, "Select your RollerCoaster Tycoon 2 / RCT Classic folder", Rct2Box.Text) is { } folder)
        {
            Rct2Box.Text = folder;
        }
    }

    private void BrowseRct1_Click(object sender, RoutedEventArgs e)
    {
        if (FolderBrowserDialog.Pick(this, "Select your RollerCoaster Tycoon 1 / Deluxe folder", Rct1Box.Text) is { } folder)
        {
            Rct1Box.Text = folder;
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var rct2 = Rct2Box.Text.Trim();
        if (rct2.Length == 0 || !Directory.Exists(rct2))
        {
            ErrorText.Text = "Pick a valid RCT2 / RCT Classic install folder.";
            ErrorText.Visibility = Visibility.Visible;
            return;
        }

        var rct1 = Rct1Box.Text.Trim();
        if (rct1.Length > 0 && !Directory.Exists(rct1))
        {
            ErrorText.Text = "That RCT1 folder doesn't exist.";
            ErrorText.Visibility = Visibility.Visible;
            return;
        }

        OpenRct2Setup.SetGameDataPaths(_serverRoot, rct2, rct1.Length > 0 ? rct1 : null);
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}
