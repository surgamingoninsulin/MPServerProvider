using System.Windows;
using System.Windows.Input;

namespace MPServerProvider.App.Views.Dialogs;

public class DialogWindow : Window
{
    public DialogWindow()
    {
        if (Application.Current?.TryFindResource("DialogWindowStyle") is Style style)
        {
            Style = style;
        }
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (_, _) => Close()));
        KeyDown += OnDialogKeyDown;
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        try
        {
            if (Owner is null &&
                Application.Current?.MainWindow is { } main &&
                !ReferenceEquals(main, this) &&
                main.IsLoaded)
            {
                Owner = main;
            }
        }
        catch (InvalidOperationException)
        {
            // Owner can't be set in some edge states (e.g. main window closing) — not fatal.
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (GetTemplateChild("PART_TitleBar") is UIElement titleBar)
        {
            titleBar.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
        }
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState != MouseButtonState.Pressed)
        {
            return;
        }
        try
        {
            DragMove();
        }
        catch (InvalidOperationException)
        {
            // DragMove throws if the mouse button was released before it started.
        }
    }

    private void OnDialogKeyDown(object sender, KeyEventArgs e)
    {
        // Bubbling KeyDown: an open ComboBox dropdown handles Escape itself first.
        if (e.Key == Key.Escape && !e.Handled)
        {
            e.Handled = true;
            Close();
        }
    }

    protected static void ToastError(string message)
    {
        try
        {
            (Application.Current?.MainWindow as MainWindow)?.Toasts.Error(message);
        }
        catch
        {
            // Never let error reporting itself throw out of an event handler.
        }
    }
}
