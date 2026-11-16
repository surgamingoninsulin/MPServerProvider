using System.Windows;

namespace MPServerProvider.App.Views.Dialogs;

/// <summary>
/// Reusable themed yes/no confirmation dialog. Replaces the native
/// <see cref="MessageBox"/> previously used for destructive confirmations ("Delete server?",
/// "Delete file?"), which looked out of place against this app's dark custom chrome.
/// </summary>
public partial class ConfirmDialog : DialogWindow
{
    private ConfirmDialog(string title, string message, string confirmText, bool destructive, string cancelText)
    {
        InitializeComponent();
        Title = title;
        TitleText.Text = title;
        MessageText.Text = message;
        ConfirmButton.Content = confirmText;
        CancelButton.Content = cancelText;
        ConfirmButton.Style = (Style)FindResource(destructive ? "DangerButtonStyle" : "AccentButtonStyle");
    }

    /// <summary>Shows the dialog modally and returns <c>true</c> if the user confirmed.
    /// Closing it any other way (Cancel, Escape, the close button) returns <c>false</c>.</summary>
    public static bool Show(Window? owner, string title, string message, string confirmText = "Delete", bool destructive = true, string cancelText = "Cancel")
    {
        var dialog = new ConfirmDialog(title, message, confirmText, destructive, cancelText);
        if (owner is not null && !ReferenceEquals(owner, dialog) && owner.IsLoaded)
        {
            dialog.Owner = owner;
        }
        return dialog.ShowDialog() == true;
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
