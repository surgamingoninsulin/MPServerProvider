using System.Windows;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Views.Dialogs;

public partial class CreativerseWorldDialog : DialogWindow
{
    private readonly string _worldKey;
    private readonly int _port;

    public CreativerseWorldDialog(string worldKey, int port, string serverName, string? rememberedSteamId)
    {
        InitializeComponent();
        _worldKey = worldKey;
        _port = port;
        SteamIdBox.Text = rememberedSteamId ?? string.Empty;
        WorldNameBox.Text = serverName;

        var templates = CreativerseWorld.KnownTemplateKeys();
        TemplateCombo.ItemsSource = templates;
        if (templates.Count > 0)
        {
            TemplateCombo.SelectedIndex = 0;
            TemplateHint.Text = "Templates found on this PC. Keep the first one unless you want a different world type.";
        }
        else
        {
            TemplateHint.Text = "No templates found on this PC yet. Start Creativerse once and create any world, so its template is downloaded, or type a template key from mod.io.";
        }

        Loaded += (_, _) => (SteamIdBox.Text.Length == 0 ? SteamIdBox : (UIElement)WorldNameBox).Focus();
    }

    public static bool EnsureWorld(Window? owner, ServerInstance server, GameDefinition game, SettingsService settings)
    {
        if (game.Id != CreativerseWorld.GameId)
        {
            return true;
        }

        var key = CreativerseWorld.WorldKey(server, game);
        if (!CreativerseWorld.NeedsSetup(key))
        {
            return true;
        }

        var dialog = new CreativerseWorldDialog(key, server.ServerPort, server.ServerName, settings.Settings.SteamId64);
        if (owner is not null && owner.IsLoaded)
        {
            dialog.Owner = owner;
        }

        if (dialog.ShowDialog() != true)
        {
            return false;
        }

        var steamId = dialog.SteamIdBox.Text.Trim();
        try
        {
            settings.Update(s => s.SteamId64 = steamId);
        }
        catch
        {
            // Remembering the ID is a convenience only.
        }

        return true;
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }

    private void FindIdButton_Click(object sender, RoutedEventArgs e)
        => Controls.ArtworkImages.OpenInBrowser("https://steamid.io/");

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var steamId = SteamIdBox.Text.Trim();
        if (!CreativerseWorld.IsSteamId64(steamId))
        {
            ShowError("Enter your SteamID64: 17 digits starting with 7656119. \"Find my ID\" looks it up from your profile link.");
            return;
        }

        var template = TemplateCombo.Text.Trim();
        if (template.Length == 0)
        {
            ShowError("Pick or type a world template.");
            return;
        }

        try
        {
            CreativerseWorld.Write(_worldKey, steamId, _port, WorldNameBox.Text, template);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            ShowError($"Couldn't write config_world.json: {ex.Message}");
        }
    }
}
