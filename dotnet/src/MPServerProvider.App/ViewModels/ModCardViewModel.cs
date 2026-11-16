using CommunityToolkit.Mvvm.ComponentModel;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.ViewModels;

public sealed record ModServerTarget(string ServerName, bool IsCompatible, string? Reason)
{
    public string Header => IsCompatible ? ServerName : $"{ServerName} — {Reason}";
}

public sealed partial class ModCardViewModel : ObservableObject
{
    public ModCardViewModel(string key, ModItem item)
    {
        Key = key;
        _item = item;
    }

    public string Key { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FileName))]
    private ModItem _item;

    [ObservableProperty]
    private string _gameName = string.Empty;

    [ObservableProperty]
    private string _detailText = string.Empty;

    [ObservableProperty]
    private IReadOnlyList<string> _tags = Array.Empty<string>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasWarning))]
    private string? _warning;

    public bool HasWarning => !string.IsNullOrEmpty(Warning);

    [ObservableProperty]
    private IReadOnlyList<string> _assignedServers = Array.Empty<string>();

    public IReadOnlyList<ModServerTarget> ServerTargets { get; private set; } = Array.Empty<ModServerTarget>();

    public string SearchText { get; private set; } = string.Empty;

    public string FileName => Item.FileName;

    public void Update(ModItem item, string gameName, string detail, IReadOnlyList<string> tags, string? warning,
        IReadOnlyList<ModServerTarget> targets)
    {
        Item = item;
        GameName = gameName;
        DetailText = detail;
        Warning = warning;
        if (!Tags.SequenceEqual(tags))
        {
            Tags = tags;
        }

        if (!AssignedServers.SequenceEqual(item.AssignedServers))
        {
            AssignedServers = item.AssignedServers.ToList();
        }

        ServerTargets = targets;
        SearchText = string.Join("\n", new[] { item.FileName, item.Minecraft?.Name, item.Minecraft?.Id, gameName }
            .Concat(tags)
            .Where(s => !string.IsNullOrEmpty(s)))
            .ToLowerInvariant();
    }
}
