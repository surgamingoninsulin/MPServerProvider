using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

/// <summary>One row in the per-server Mods tab (installed on the server, or available from the library).</summary>
public sealed partial class ServerModRowViewModel : ObservableObject
{
    public ServerModRowViewModel(string key, ModItem item)
    {
        Key = key;
        _item = item;
    }

    public string Key { get; }

    [ObservableProperty]
    private ModItem _item;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _subtitle = string.Empty;

    [ObservableProperty]
    private IReadOnlyList<string> _tags = Array.Empty<string>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasReason))]
    private string? _reason;

    [ObservableProperty]
    private bool _isCompatible = true;

    public bool HasReason => !string.IsNullOrEmpty(Reason);

    /// <summary>A folder in the server's mods/plugins folder (on Minecraft: a plugin's data and
    /// config). Shown with Open (into the File Manager) instead of a compatibility verdict.</summary>
    public bool IsFolder => Item.IsDirectory;

    partial void OnItemChanged(ModItem value) => OnPropertyChanged(nameof(IsFolder));

    public void Update(ModItem item, string title, string subtitle, IReadOnlyList<string> tags, ModCompatibilityResult verdict)
    {
        Item = item;
        Title = title;
        Subtitle = subtitle;
        if (!Tags.SequenceEqual(tags))
        {
            Tags = tags;
        }

        IsCompatible = verdict.IsCompatible;
        Reason = verdict.Reason;
    }
}

/// <summary>
/// The per-server Mods tab inside <see cref="ServerDetailViewModel"/>: what's installed in this
/// server's mods/plugins folder and which library items fit it. For Minecraft Java only jars
/// matching the server variant (Paper → plugins, Fabric → Fabric mods, …) and its Minecraft
/// version are offered; incompatible ones are hidden unless "Show incompatible" is on, and then
/// shown disabled with the reason.
/// <para>
/// Loads only when the tab becomes visible, when the shown server changes while it is visible,
/// after an install/remove, or on Refresh — never from the detail view's 800 ms poll — and does
/// all disk and jar reading on a worker thread. Lists are diffed in place so they don't jump.
/// </para>
/// </summary>
public sealed partial class ServerModsTabViewModel : ObservableObject
{
    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private readonly ServerDetailViewModel _owner;
    private List<ServerModRowViewModel> _allAvailable = new();
    private bool _active;
    private bool _loading;
    private bool _reloadPending;
    private string? _loadedServerName;

    public ServerModsTabViewModel(AppServices services, ToastService toasts, ServerDetailViewModel owner)
    {
        _services = services;
        _toasts = toasts;
        _owner = owner;
        _owner.PropertyChanged += Owner_PropertyChanged;
    }

    public ObservableCollection<ServerModRowViewModel> Installed { get; } = new();

    public ObservableCollection<ServerModRowViewModel> Available { get; } = new();

    [ObservableProperty]
    private string _summary = string.Empty;

    /// <summary>Shown instead of the lists when this server can't take anything (vanilla).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNotice))]
    private string? _notice;

    public bool HasNotice => !string.IsNullOrEmpty(Notice);

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _showIncompatible;

    [ObservableProperty]
    private string _hiddenText = string.Empty;

    [ObservableProperty]
    private bool _isInstalledEmpty = true;

    [ObservableProperty]
    private bool _isAvailableEmpty = true;

    partial void OnShowIncompatibleChanged(bool value) => ApplyAvailableFilter();

    /// <summary>Called by the view when the Mods panel is shown/hidden.</summary>
    public void SetActive(bool active)
    {
        _active = active;
        if (active)
        {
            Reload();
        }
    }

    private void Owner_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // NameText changes exactly when a different server is shown.
        if (_active && e.PropertyName == nameof(ServerDetailViewModel.NameText)
            && _owner.Current?.ServerName != _loadedServerName)
        {
            Reload();
        }
    }

    [RelayCommand]
    private void Refresh() => Reload();

    private sealed record Snapshot(
        string ServerName,
        string Summary,
        string? Notice,
        List<(ModItem Item, string Title, string Subtitle, IReadOnlyList<string> Tags, ModCompatibilityResult Verdict)> Installed,
        List<(ModItem Item, string Title, string Subtitle, IReadOnlyList<string> Tags, ModCompatibilityResult Verdict)> Available);

    /// <summary>Re-reads the server folder and library off the UI thread; coalesces overlapping calls.</summary>
    public async void Reload()
    {
        if (_loading)
        {
            _reloadPending = true;
            return;
        }

        _loading = true;
        IsBusy = true;
        try
        {
            do
            {
                _reloadPending = false;
                if (_owner.Current is not { } server)
                {
                    return;
                }

                var snapshot = await Task.Run(() => BuildSnapshot(server));
                if (_owner.Current?.ServerName != snapshot.ServerName)
                {
                    _reloadPending = true; // server switched meanwhile
                    continue;
                }

                Apply(snapshot);
            }
            while (_reloadPending);
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't load this server's mods: {ex.Message}");
        }
        finally
        {
            _loading = false;
            IsBusy = false;
        }
    }

    private Snapshot BuildSnapshot(ServerInstance server)
    {
        var game = _services.Games.Find(server.ServerGame);
        var isMc = ModCompatibility.IsMinecraftJava(server.ServerGame);
        var folder = _services.Mods.ServerModsDir(server);
        var folderName = server.ModsFolder ?? game?.Mods.Folder ?? "mods";

        string summary;
        string? notice = null;
        if (folder is null)
        {
            summary = "This server has no mods folder configured.";
            notice = summary;
        }
        else if (isMc)
        {
            var mcVersion = MinecraftVersionRange.ServerMinecraftVersion(server.ServerVersion);
            var variant = ModCompatibility.VariantLabel(server);
            summary = $"{variant}{(mcVersion is null ? string.Empty : $" · MC {mcVersion}")} — takes {ModCompatibility.AcceptedDescription(server)} in “{folderName}”";
            if (ModCompatibility.AcceptedPlatforms(server) == MinecraftPlatform.None)
            {
                notice = "Vanilla Minecraft servers can't load mods or plugins. Create a Paper or Purpur server for plugins, or a Fabric/Forge server for mods.";
            }
        }
        else
        {
            summary = $"{game?.Name ?? server.ServerGame} — {game?.Mods.Kind ?? "mod"}s go in “{folderName}”";
        }

        var installedItems = folder is null ? new List<ModItem>() : _services.Mods.ListServerMods(server);
        var installedNames = new HashSet<string>(installedItems.Select(i => i.FileName), StringComparer.OrdinalIgnoreCase);

        // Folders next to the jars are plugin data/config folders, not things to load: no
        // compatibility verdict for them, and they're listed after the mods/plugins.
        var installed = installedItems
            .Select(i => Row(i, i.IsDirectory ? ModCompatibilityResult.Ok : ModCompatibility.Check(i, server, game)))
            .OrderBy(r => r.Item.IsDirectory)
            .ThenBy(r => r.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        var library = folder is null || notice is not null
            ? new List<ModItem>()
            : _services.Mods.Scan(_services.Workshop, readMetadata: true)
                .Where(i => string.Equals(i.GameId, server.ServerGame, StringComparison.OrdinalIgnoreCase))
                .Where(i => !installedNames.Contains(i.FileName))
                .ToList();

        var available = library
            .Select(i => Row(i, ModCompatibility.Check(i, server, game)))
            .OrderByDescending(r => r.Verdict.IsCompatible)
            .ThenBy(r => r.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        return new Snapshot(server.ServerName, summary, notice, installed, available);
    }

    private static (ModItem Item, string Title, string Subtitle, IReadOnlyList<string> Tags, ModCompatibilityResult Verdict) Row(
        ModItem item, ModCompatibilityResult verdict)
    {
        var info = item.Minecraft;
        var title = info?.Name is { Length: > 0 } name
            ? info.Version is { Length: > 0 } v ? $"{name} {v}" : name
            : item.FileName;
        var subtitle = title == item.FileName
            ? ModsPageViewModel.FormatBytes(item.SizeBytes)
            : $"{item.FileName}  •  {ModsPageViewModel.FormatBytes(item.SizeBytes)}";
        return (item, title, subtitle, ModCompatibility.Tags(item), verdict);
    }

    private void Apply(Snapshot snapshot)
    {
        _loadedServerName = snapshot.ServerName;
        Summary = snapshot.Summary;
        Notice = snapshot.Notice;

        Sync(Installed, snapshot.Installed.Select(r => Materialize(Installed, r)).ToList());
        _allAvailable = snapshot.Available.Select(r => Materialize(_allAvailable, r)).ToList();
        IsInstalledEmpty = Installed.Count == 0;
        ApplyAvailableFilter();
    }

    private static ServerModRowViewModel Materialize(
        IEnumerable<ServerModRowViewModel> existing,
        (ModItem Item, string Title, string Subtitle, IReadOnlyList<string> Tags, ModCompatibilityResult Verdict) data)
    {
        var key = data.Item.LibraryPath ?? data.Item.FileName;
        var row = existing.FirstOrDefault(r => string.Equals(r.Key, key, StringComparison.OrdinalIgnoreCase))
            ?? new ServerModRowViewModel(key, data.Item);
        row.Update(data.Item, data.Title, data.Subtitle, data.Tags, data.Verdict);
        return row;
    }

    private void ApplyAvailableFilter()
    {
        var hidden = _allAvailable.Count(r => !r.IsCompatible);
        Sync(Available, ShowIncompatible ? _allAvailable : _allAvailable.Where(r => r.IsCompatible).ToList());
        HiddenText = hidden == 0 ? string.Empty
            : ShowIncompatible ? $"{hidden} incompatible item(s) shown greyed out"
            : $"{hidden} incompatible item(s) hidden";
        IsAvailableEmpty = Available.Count == 0;
    }

    /// <summary>In-place diff (remove / move / insert) so the lists keep their scroll position.</summary>
    private static void Sync(ObservableCollection<ServerModRowViewModel> target, IReadOnlyList<ServerModRowViewModel> wanted)
    {
        var wantedSet = new HashSet<ServerModRowViewModel>(wanted);
        for (var i = target.Count - 1; i >= 0; i--)
        {
            if (!wantedSet.Contains(target[i]))
            {
                target.RemoveAt(i);
            }
        }

        for (var i = 0; i < wanted.Count; i++)
        {
            if (i < target.Count && ReferenceEquals(target[i], wanted[i]))
            {
                continue;
            }

            var existing = target.IndexOf(wanted[i]);
            if (existing >= 0)
            {
                target.Move(existing, i);
            }
            else
            {
                target.Insert(i, wanted[i]);
            }
        }
    }

    [RelayCommand]
    private async Task Install(ServerModRowViewModel? row)
    {
        if (row is null || _owner.Current is not { } server)
        {
            return;
        }

        try
        {
            var item = row.Item;
            await Task.Run(() => _services.Mods.AssignToServer(item, server));
            _toasts.Success($"Added {row.Title} to {server.ServerName}.");
            Reload();
        }
        catch (ModIncompatibleException ex)
        {
            _toasts.Warning(ex.Message);
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't add mod: {ex.Message}");
        }
    }

    /// <summary>Opens a folder row in the File Manager tab.</summary>
    [RelayCommand]
    private void OpenFolder(ServerModRowViewModel? row)
    {
        if (row is { IsFolder: true, Item.LibraryPath: { } path })
        {
            _owner.ShowInFiles(path);
        }
    }

    [ObservableProperty]
    private bool _isInstallingDrop;

    /// <summary>Installs files dropped from Explorer onto the Mods tab (same compatibility
    /// rules as installing from the library).</summary>
    public async Task InstallDroppedAsync(IReadOnlyList<string> paths)
    {
        if (_owner.Current is not { } server || paths.Count == 0 || IsInstallingDrop)
        {
            return;
        }

        IsInstallingDrop = true;
        try
        {
            var result = await Task.Run(() => _services.Mods.InstallFilesToServer(paths, server));
            ReportDrop(result, server);
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't add the dropped files: {ex.Message}");
        }
        finally
        {
            IsInstallingDrop = false;
            Reload();
        }
    }

    private void ReportDrop(ModDropResult result, ServerInstance server)
    {
        if (result.Count > 0)
        {
            var names = result.Added.Concat(result.Replaced).ToList();
            var what = names.Count == 1 ? names[0] : $"{names.Count} items";
            var running = _services.Processes.StateOf(server) is ServerState.Running or ServerState.Starting;
            _toasts.Success($"Added {what} to {server.ServerName}.{(running ? " Restart the server to load it." : string.Empty)}");
        }

        foreach (var (added, other) in result.Duplicates)
        {
            _toasts.Warning($"{other} looks like another version of {added}. Remove the one you don't want — two copies can stop the server from starting.");
        }

        if (result.Skipped.Count > 0)
        {
            var lines = result.Skipped.Take(4).Select(s => $"{s.Name}: {s.Reason}");
            var more = result.Skipped.Count > 4 ? $" (+{result.Skipped.Count - 4} more)" : string.Empty;
            _toasts.Warning($"Not added — {string.Join("; ", lines)}{more}");
        }
        else if (result.Count == 0)
        {
            _toasts.Info("Nothing to add.");
        }
    }

    [RelayCommand]
    private async Task Remove(ServerModRowViewModel? row)
    {
        if (row is null || _owner.Current is not { } server)
        {
            return;
        }

        try
        {
            var item = row.Item;
            if (item.IsDirectory && !Views.Dialogs.ConfirmDialog.Show(
                    System.Windows.Application.Current.MainWindow,
                    "Delete Folder",
                    $"Delete the folder '{item.FileName}' and everything in it? If it belongs to a plugin, that plugin's settings and data are lost. This cannot be undone.",
                    confirmText: "Delete"))
            {
                return;
            }

            await Task.Run(() => _services.Mods.RemoveFromServer(item, server));
            _toasts.Info($"Removed {item.FileName} from {server.ServerName}.");
            Reload();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't remove mod: {ex.Message}");
        }
    }
}
