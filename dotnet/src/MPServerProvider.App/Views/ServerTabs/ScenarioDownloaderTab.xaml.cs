using System.Windows.Controls;
using MPServerProvider.App.ViewModels;

namespace MPServerProvider.App.Views.ServerTabs;

public partial class ScenarioDownloaderTab : UserControl
{
    public ScenarioDownloaderTab()
    {
        InitializeComponent();
        // Lazy: the scenario list only loads the first time the tab is actually shown.
        IsVisibleChanged += (_, e) =>
        {
            if (e.NewValue is true && DataContext is ScenarioDownloaderTabViewModel vm)
            {
                vm.EnsureLoaded();
            }
        };
    }
}
