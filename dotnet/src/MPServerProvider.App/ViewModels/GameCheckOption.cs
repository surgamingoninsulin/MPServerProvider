using CommunityToolkit.Mvvm.ComponentModel;

namespace MPServerProvider.App.ViewModels;

/// <summary>
/// One entry in the Mods page's "Filters" sidebar checkbox list (one per moddable game), ported
/// from <c>ModsPage</c>'s <c>GameChecks</c> <c>StackPanel</c> (<c>views_modspage.xaml</c>) and the
/// decompiled <c>_uncheckedGames</c> "games unchecked so far" exclusion set. Checking/unchecking
/// triggers a re-filter of the mod list, wired by <see cref="ViewModels.ModsPageViewModel"/>.
/// </summary>
public sealed partial class GameCheckOption : ObservableObject
{
    public string Id { get; }

    public string Name { get; }

    [ObservableProperty]
    private bool _isChecked = true;

    public GameCheckOption(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
