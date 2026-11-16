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

    public void OnNavigatedFrom() => _viewModel?.Stop();

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
