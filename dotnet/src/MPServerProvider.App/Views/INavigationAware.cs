namespace MPServerProvider.App.Views;

public interface INavigationAware
{
    void OnNavigatedTo();

    void OnNavigatedFrom()
    {
    }
}
