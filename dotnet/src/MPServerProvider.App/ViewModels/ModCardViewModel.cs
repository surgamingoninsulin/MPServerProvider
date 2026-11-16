using CommunityToolkit.Mvvm.ComponentModel;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.ViewModels;

/// <summary>One server entry in a card's "Add to Server ▾" menu: disabled with a reason when
/// the mod can't go on that server.</summary>
public sealed record ModServerTarget(string ServerName, bool IsCompatible, string? Reason)
{
    public string Header => IsCompatible ? ServerName : $"{ServerName} — {Reason}";
}

/// <summary>
/// Display/row data for one mod card on the Mods page, ported from the decompiled
/// <c>ModCardVm</c> (<c>get_DetailText()</c>/<c>get_GameName()</c>).
/// <para>
/// Observable and keyed by <see cref="Key"/> (the library path) so a refresh can update an
/// existing card in place via <see cref="Update"/> instead of clearing and re-adding the whole
/// list — rebuilding every card on every scan is what reset the scroll position and made the
/// page "randomly update".
/// </para>
/// </summary>
public sealed partial class ModCardViewModel : ObservableObject
{
    public ModCardViewModel(string key, ModItem item)
    {
        Key = key;
        _item = item;
    }

    /// <summary>Stable identity across scans (library path, or game + file name).</summary>
    public string Key { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FileName))]
    private ModItem _item;

    [ObservableProperty]
    private string _gameName = string.Empty;

    [ObservableProperty]
    private string _detailText = string.Empty;

    /// <summary>Short metadata tags, e.g. "MC 1.21.1 · Fabric", "Needs Fabric API".</summary>
    [ObservableProperty]
    private IReadOnlyList<string> _tags = Array.Empty<string>();

    /// <summary>Set when no server of this game can take the mod (e.g. a folder in the Minecraft
    /// library); shown as a warning line on the card.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasWarning))]
    private string? _warning;

    public bool HasWarning => !string.IsNullOrEmpty(Warning);

    /// <summary>Servers this mod is currently assigned to (chips with a remove "x").</summary>
    [ObservableProperty]
    private IReadOnlyList<string> _assignedServers = Array.Empty<string>();

    /// <summary>Servers of the same game not yet assigned this mod (the "Add to Server ▾" menu),
    /// including incompatible ones (shown disabled with the reason).</summary>
    public IReadOnlyList<ModServerTarget> ServerTargets { get; private set; } = Array.Empty<ModServerTarget>();

    /// <summary>Lower-cased text the keyword filter searches (file, mod name/id, tags).</summary>
    public string SearchText { get; private set; } = string.Empty;

    public string FileName => Item.FileName;

    /// <summary>Applies a fresh snapshot, raising change notifications only for what changed.</summary>
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
