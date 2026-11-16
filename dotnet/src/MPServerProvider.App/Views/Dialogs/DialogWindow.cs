using System.Windows;
using System.Windows.Input;

namespace MPServerProvider.App.Views.Dialogs;

/// <summary>
/// Base class for the app's modal dialogs (everything except TunnelDialog): applies the dark
/// borderless chrome from <c>Themes/Theme.xaml</c> (<c>DialogWindowStyle</c>: rounded card,
/// drop shadow, custom title + close button) instead of the native Windows title bar, makes
/// the title bar draggable, closes on Escape, and centers on the main window by default.
/// </summary>
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

    /// <summary>Defaults <see cref="Window.Owner"/> to the main window so the dialog is modal
    /// to it and centered over it; callers that set <c>Owner</c> themselves override this.</summary>
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

    /// <summary>Shows <paramref name="message"/> as an error toast through the main window's
    /// toast stack when available (dialogs that have their own error line still use it).</summary>
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
