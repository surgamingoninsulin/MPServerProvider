using System.Windows;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Providers;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Views.Dialogs;

public partial class SteamLoginDialog : DialogWindow
{
    private SteamLogin? _login;

    public SteamLoginDialog(GameDefinition game, string? rememberedUsername, string? error)
    {
        InitializeComponent();
        IntroText.Text = $"The {game.Name.Replace(" Dedicated Server", string.Empty).Replace(" Server", string.Empty)} server only downloads for a Steam account that owns the game.";
        UsernameBox.Text = rememberedUsername ?? string.Empty;
        CachedHint.Visibility = string.IsNullOrWhiteSpace(rememberedUsername) ? Visibility.Collapsed : Visibility.Visible;
        if (!string.IsNullOrWhiteSpace(error))
        {
            ShowError(error);
        }

        Loaded += (_, _) =>
        {
            if (UsernameBox.Text.Length == 0)
            {
                UsernameBox.Focus();
            }
            else
            {
                PasswordBox.Focus();
            }
        };
    }

    public static SteamLogin? Ask(Window? owner, GameDefinition game, SettingsService settings, out bool cancelled, string? error = null)
    {
        cancelled = false;
        if (!SteamCmdProvider.NeedsSteamLogin(game))
        {
            return null;
        }

        var dialog = new SteamLoginDialog(game, settings.Settings.SteamUsername, error);
        if (owner is not null && owner.IsLoaded)
        {
            dialog.Owner = owner;
        }

        if (dialog.ShowDialog() != true || dialog._login is null)
        {
            cancelled = true;
            return null;
        }

        var username = dialog._login.Username;
        try
        {
            settings.Update(s => s.SteamUsername = username);
        }
        catch
        {
            // Remembering the name is a convenience only.
        }

        return dialog._login;
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ContinueButton_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameBox.Text.Trim();
        var password = PasswordBox.Password;
        var guard = GuardBox.Text.Trim();

        if (username.Length == 0)
        {
            ShowError("Enter your Steam username.");
            return;
        }

        if (new[] { username, password, guard }.Any(v => v.Any(c => c == '"' || char.IsControl(c))))
        {
            ShowError("SteamCMD can't take quote characters in a login. Please type it without them.");
            return;
        }

        if (password.Length == 0 && guard.Length > 0)
        {
            ShowError("Enter your password together with the Steam Guard code.");
            return;
        }

        _login = new SteamLogin(username, password.Length == 0 ? null : password, guard.Length == 0 ? null : guard);
        DialogResult = true;
        Close();
    }
}
