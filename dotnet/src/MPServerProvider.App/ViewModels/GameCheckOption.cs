using CommunityToolkit.Mvvm.ComponentModel;

namespace MPServerProvider.App.ViewModels;

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
