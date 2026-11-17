using System.Windows;
using MPServerProvider.App.Toasts;
using MPServerProvider.App.Views.ServerTabs;

namespace MPServerProvider.App.Views.Dialogs;

public partial class HytaleAuthDialog : DialogWindow
{
    private readonly ToastService? _toasts;
    private string? _link;

    public HytaleAuthDialog(ToastService? toasts)
    {
        _toasts = toasts;
        InitializeComponent();
    }

    public async Task UpdateAsync(string? link, string? code)
    {
        if (code is not null)
        {
            CodeButtonText.Text = code;
            CodeButton.Visibility = Visibility.Visible;
        }

        if (link is null || link == _link)
        {
            return;
        }
        _link = link;

        try
        {
            await EmbeddedBrowser.InitializeAsync(Web);
            Web.CoreWebView2!.Navigate(link);
            Web.Visibility = Visibility.Visible;
            StatusText.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            Web.Visibility = Visibility.Collapsed;
            StatusText.Visibility = Visibility.Visible;
            StatusText.Text = EmbeddedBrowser.DescribeFailure(ex);
        }
    }

    private void CodeButton_Click(object sender, RoutedEventArgs e)
    {
        if (CodeButtonText.Text is not { Length: > 0 } code)
        {
            return;
        }

        try
        {
            Clipboard.SetText(code);
            _toasts?.Success("Code copied to clipboard.");
        }
        catch (Exception ex)
        {
            _toasts?.Error($"Couldn't copy the code: {ex.Message}");
        }
    }
}
