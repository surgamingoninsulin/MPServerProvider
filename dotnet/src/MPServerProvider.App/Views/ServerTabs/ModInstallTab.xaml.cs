using System.Windows.Controls;
using System.Windows.Input;
using MPServerProvider.App.ViewModels;

namespace MPServerProvider.App.Views.ServerTabs;

public partial class ModInstallTab : UserControl
{
    public ModInstallTab()
    {
        InitializeComponent();
        // Lazy: the mod list only loads the first time the tab is actually shown.
        IsVisibleChanged += (_, e) =>
        {
            if (e.NewValue is true && DataContext is ModInstallTabViewModel vm)
            {
                vm.EnsureLoaded();
            }
        };
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is ModInstallTabViewModel vm && vm.SearchCommand.CanExecute(null))
        {
            vm.SearchCommand.Execute(null);
        }
    }

    // "Infinite" window: once the user has scrolled within one card's height of the bottom,
    // ask the view model for the next page instead of waiting for them to hit the end exactly.
    private const double LoadMoreThreshold = 260;

    private void ResultsScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (DataContext is not ModInstallTabViewModel vm || sender is not ScrollViewer scroll)
        {
            return;
        }

        var remaining = scroll.ScrollableHeight - scroll.VerticalOffset;
        if (remaining <= LoadMoreThreshold)
        {
            vm.LoadMore();
        }
    }
}
