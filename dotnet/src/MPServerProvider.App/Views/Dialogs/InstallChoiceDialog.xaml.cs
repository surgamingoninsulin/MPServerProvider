using System.Windows;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.Views.Dialogs;

public partial class InstallChoiceDialog : DialogWindow
{
    private enum Choice
    {
        None,
        New,
        Import,
    }

    private Choice _choice;

    private InstallChoiceDialog(GameDefinition game)
    {
        InitializeComponent();
        Title = $"Add {game.Name} Server";
        TitleText.Text = Title;
        NewText.Text = game.Install.Provider == "manual"
            ? $"Set up a {game.Name} server from server files you've downloaded."
            : $"Download and set up a fresh {game.Name} server with this app.";
        ImportText.Text = $"Pick the folder of a {game.Name} server you already have (made by hand or another panel). Worlds, configs and mods come along.";
        Loaded += (_, _) => NewCard.Focus();
    }

    public static void Run(AppServices services, ToastService? toasts, GameDefinition game, Window? owner)
    {
        var choice = new InstallChoiceDialog(game) { Owner = owner };
        choice.ShowDialog();
        switch (choice._choice)
        {
            case Choice.New when game.Install.Provider == "manual":
                new ManualInstallDialog(services, toasts, game) { Owner = owner }.ShowDialog();
                break;
            case Choice.New:
                new InstallServerDialog(services, toasts, game) { Owner = owner }.ShowDialog();
                break;
            case Choice.Import:
                var folder = FolderBrowserDialog.Pick(owner, $"Select the {game.Name} server folder to import");
                if (folder is not null)
                {
                    new ImportServerDialog(services, toasts, game, folder) { Owner = owner }.ShowDialog();
                }

                break;
        }
    }

    private void NewCard_Click(object sender, RoutedEventArgs e)
    {
        _choice = Choice.New;
        Close();
    }

    private void ImportCard_Click(object sender, RoutedEventArgs e)
    {
        _choice = Choice.Import;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}
