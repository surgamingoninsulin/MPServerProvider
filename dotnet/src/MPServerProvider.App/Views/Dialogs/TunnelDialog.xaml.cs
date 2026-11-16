using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.App.ViewModels;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.Views.Dialogs;

/// <summary>
/// Port of <c>MPServerProvider.App.Views.TunnelDialog</c> (decompile token 0x0200001d): a
/// borderless, draggable modal in the app's dark chrome. The view model does the work; this
/// code-behind only handles tabs, dragging, clipboard and closing.
/// </summary>
public partial class TunnelDialog : Window
{
    private readonly ToastService _toasts;

    public TunnelDialog(AppServices services, ToastService toasts, ServerInstance? preselected)
    {
        InitializeComponent();
        _toasts = toasts;
        DataContext = new TunnelDialogViewModel(services, toasts, preselected);
        Closed += (_, _) => (DataContext as TunnelDialogViewModel)?.Dispose();
    }

    private TunnelDialogViewModel? ViewModel => DataContext as TunnelDialogViewModel;

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
            // DragMove throws if the button was released before it started; nothing to do.
        }
    }

    private void Tab_Checked(object sender, RoutedEventArgs e)
    {
        if (UpnpPanel is null || PlayitPanel is null)
        {
            return;
        }

        UpnpPanel.Visibility = ReferenceEquals(sender, TabUpnp) ? Visibility.Visible : Visibility.Collapsed;
        PlayitPanel.Visibility = ReferenceEquals(sender, TabPlayit) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void CopyExternalAddress_Click(object sender, RoutedEventArgs e) => Copy(ViewModel?.ExternalAddress);

    private void CopyClaimUrl_Click(object sender, RoutedEventArgs e) => Copy(ViewModel?.ClaimUrl);

    private void CopyTunnelAddress_Click(object sender, RoutedEventArgs e) => Copy((sender as FrameworkElement)?.Tag as string);

    private void Copy(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }
        try
        {
            Clipboard.SetText(text);
            _toasts.Info("Copied to clipboard.");
        }
        catch (Exception ex)
        {
            // The clipboard can be locked by another app (COMException/ExternalException).
            _toasts.Warning($"Couldn't copy to the clipboard: {ex.Message}");
        }
    }

    private void PlayitOutput_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox box)
        {
            box.ScrollToEnd();
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
