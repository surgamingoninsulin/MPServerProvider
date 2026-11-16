using System.Windows;
using System.Windows.Controls;
using MPServerProvider.App.ViewModels;

namespace MPServerProvider.App.Views.Pages;

public partial class MyServersPage : UserControl, INavigationAware
{
    private MyServersViewModel? _viewModel;

    public MyServersPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Lazily builds the view model on first navigation rather than at construction time:
    /// <see cref="MainWindow"/> builds every page (including this one) from its own
    /// constructor, before <c>Application.Current.MainWindow</c> is assigned — so the live
    /// <see cref="MainWindow.Toasts"/> instance isn't reachable yet at construction time, only
    /// once the shell has actually navigated here.
    /// </summary>
    public void OnNavigatedTo()
    {
        if (_viewModel is null && Application.Current is App { Services: { } services } &&
            Window.GetWindow(this) is MainWindow mainWindow)
        {
            _viewModel = new MyServersViewModel(services, mainWindow.Toasts);
            DataContext = _viewModel;
        }

        _viewModel?.Start();
    }

    /// <summary>Stops the list's poll timer (and the detail panel's) while the page isn't shown,
    /// so nothing keeps hitting the disk/process table in the background.</summary>
    public void OnNavigatedFrom() => _viewModel?.Stop();

    /// <summary>Selects a server by name; call after navigating here.</summary>
    public void SelectServer(string serverName) => _viewModel?.Select(serverName);

    private void LibraryLink_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is { } window &&
            window.FindName("NavLibrary") is RadioButton libraryNav)
        {
            libraryNav.IsChecked = true;
        }
    }
}
