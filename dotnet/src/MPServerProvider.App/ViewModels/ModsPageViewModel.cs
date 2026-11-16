using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

public sealed partial class ModsPageViewModel : ObservableObject
{
    private const string AllGamesLabel = "All Games";

    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private readonly List<string?> _gameFilterIds = new();
    private readonly Dictionary<string, ModCardViewModel> _cardsByKey = new(StringComparer.OrdinalIgnoreCase);

    public ObservableCollection<string> GameFilterNames { get; } = new();

    public ObservableCollection<GameCheckOption> GameChecks { get; } = new();

    public ObservableCollection<ModCardViewModel> Cards { get; } = new();

    [ObservableProperty]
    private int _selectedGameIndex;

    [ObservableProperty]
    private string _keyword = string.Empty;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _emptyTitle = "No Mods Imported Yet";

    [ObservableProperty]
    private string _emptyText = "Press Import from Folder to add mods and plugins from your PC";

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private string _filterSummary = "Filters (0)";

    private static readonly TimeSpan SearchDebounce = TimeSpan.FromMilliseconds(300);

    private readonly System.Windows.Threading.DispatcherTimer _searchDebounce;
    private List<CardData> _snapshot = new();
    private bool _scanRunning;
    private bool _scanPending;
    private bool _hasScanned;
    private bool _suppressFilter;

    public ModsPageViewModel(AppServices services, ToastService toasts)
    {
        _services = services;
        _toasts = toasts;
        _searchDebounce = new System.Windows.Threading.DispatcherTimer { Interval = SearchDebounce };
        _searchDebounce.Tick += (_, _) =>
        {
            _searchDebounce.Stop();
            ApplyFilters();
        };
    }

    partial void OnSelectedGameIndexChanged(int value)
    {
        if (!_suppressFilter)
        {
            ApplyFilters();
        }
    }

    partial void OnKeywordChanged(string value)
    {
        _searchDebounce.Stop();
        _searchDebounce.Start();
    }

    public void Initialize()
    {
        try
        {
            BuildFilters();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't load the game list: {ex.Message}");
        }

        Refresh();
    }

    private void BuildFilters()
    {
        var moddable = _services.Games.Games
            .Where(g => g.Mods.Supported)
            .OrderBy(g => g.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        if (_gameFilterIds.Skip(1).SequenceEqual(moddable.Select(g => (string?)g.Id))
            && GameFilterNames.Skip(1).SequenceEqual(moddable.Select(g => g.Name)))
        {
            return;
        }

        var previousSelectedId = SelectedGameIndex > 0 && SelectedGameIndex < _gameFilterIds.Count
            ? _gameFilterIds[SelectedGameIndex]
            : null;
        var previousChecks = GameChecks.ToDictionary(c => c.Id, c => c.IsChecked);

        _suppressFilter = true;
        try
        {
            GameFilterNames.Clear();
            _gameFilterIds.Clear();
            GameFilterNames.Add(AllGamesLabel);
            _gameFilterIds.Add(null);
            foreach (var game in moddable)
            {
                GameFilterNames.Add(game.Name);
                _gameFilterIds.Add(game.Id);
            }

            var restored = previousSelectedId is null ? 0 : _gameFilterIds.IndexOf(previousSelectedId);

            // Clearing the items made the ComboBox drop its selection; bounce the value so the
            // binding always pushes the restored index back to it.
            SelectedGameIndex = -1;
            SelectedGameIndex = restored < 0 ? 0 : restored;

            GameChecks.Clear();
            foreach (var game in moddable)
            {
                var isChecked = !previousChecks.TryGetValue(game.Id, out var wasChecked) || wasChecked;
                var option = new GameCheckOption(game.Id, game.Name) { IsChecked = isChecked };
                option.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(GameCheckOption.IsChecked) && !_suppressFilter)
                    {
                        ApplyFilters();
                    }
                };
                GameChecks.Add(option);
            }
        }
        finally
        {
            _suppressFilter = false;
        }

        ApplyFilters();
    }

    public async void Refresh()
    {
        if (_scanRunning)
        {
            _scanPending = true;
            return;
        }

        _scanRunning = true;
        IsScanning = !_hasScanned;
        try
        {
            do
            {
                _scanPending = false;
                _snapshot = await Task.Run(BuildSnapshot);
                _hasScanned = true;
                ApplyFilters();
            }
            while (_scanPending);
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't scan the mod library: {ex.Message}");
        }
        finally
        {
            _scanRunning = false;
            IsScanning = false;
        }
    }

    private sealed record CardData(
        string Key,
        ModItem Item,
        string GameName,
        string Detail,
        IReadOnlyList<string> Tags,
        string? Warning,
        IReadOnlyList<ModServerTarget> Targets);

    private List<CardData> BuildSnapshot()
    {
        var mods = _services.Mods.Scan(_services.Workshop, readMetadata: true);
        var servers = _services.Servers.Servers
            .Where(s => _services.Mods.ServerModsDir(s) is not null)
            .ToList();
        var gameNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var games = new Dictionary<string, GameDefinition?>(StringComparer.OrdinalIgnoreCase);
        GameDefinition? GameOf(string id)
        {
            if (!games.TryGetValue(id, out var game))
            {
                games[id] = game = _services.Games.Find(id);
            }

            return game;
        }

        var result = new List<CardData>(mods.Count);
        foreach (var item in mods)
        {
            if (!gameNames.TryGetValue(item.GameId, out var gameName))
            {
                gameNames[item.GameId] = gameName = GameOf(item.GameId)?.Name ?? item.GameId;
            }

            var size = FormatBytes(item.SizeBytes);
            var assigned = item.AssignedServers;
            var detail = assigned.Count switch
            {
                0 => $"not assigned  •  {size}",
                1 => $"on {assigned[0]}  •  {size}",
                _ => $"on {assigned.Count} servers  •  {size}",
            };

            var targets = servers
                .Where(s => string.Equals(s.ServerGame, item.GameId, StringComparison.OrdinalIgnoreCase))
                .Where(s => !assigned.Contains(s.ServerName))
                .Select(s =>
                {
                    var verdict = ModCompatibility.Check(item, s, GameOf(s.ServerGame));
                    return new ModServerTarget(s.ServerName, verdict.IsCompatible, verdict.Reason);
                })
                .OrderByDescending(t => t.IsCompatible)
                .ThenBy(t => t.ServerName, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            string? warning = null;
            if (ModCompatibility.IsMinecraftJava(item.GameId) && (item.IsDirectory || !ModCompatibility.IsJarFile(item.FileName)))
            {
                warning = "Minecraft only loads .jar files — this can't be added to a server.";
            }
            else if (item.Minecraft is { IsReadable: false })
            {
                warning = "This jar can't be opened (corrupt or incomplete download).";
            }

            result.Add(new CardData(
                item.LibraryPath ?? $"{item.GameId}/{item.FileName}",
                item,
                gameName,
                detail,
                ModCompatibility.Tags(item),
                warning,
                targets));
        }

        return result
            .OrderBy(c => c.GameName, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(c => c.Item.FileName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    private void ApplyFilters()
    {
        try
        {
            ApplyFiltersCore();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't filter mods: {ex.Message}");
        }
    }

    private void ApplyFiltersCore()
    {
        var selectedId = SelectedGameIndex > 0 && SelectedGameIndex < _gameFilterIds.Count
            ? _gameFilterIds[SelectedGameIndex]
            : null;
        var uncheckedIds = new HashSet<string>(GameChecks.Where(c => !c.IsChecked).Select(c => c.Id));
        var keyword = Keyword.Trim().ToLowerInvariant();

        // Update every card's data first (so SearchText reflects the newest scan), then filter.
        var liveKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ordered = new List<ModCardViewModel>(_snapshot.Count);
        foreach (var data in _snapshot)
        {
            liveKeys.Add(data.Key);
            if (!_cardsByKey.TryGetValue(data.Key, out var card))
            {
                card = new ModCardViewModel(data.Key, data.Item);
                _cardsByKey[data.Key] = card;
            }

            card.Update(data.Item, data.GameName, data.Detail, data.Tags, data.Warning, data.Targets);
            ordered.Add(card);
        }

        foreach (var stale in _cardsByKey.Keys.Where(k => !liveKeys.Contains(k)).ToList())
        {
            _cardsByKey.Remove(stale);
        }

        var wanted = ordered
            .Where(c => selectedId is null || c.Item.GameId == selectedId)
            .Where(c => !uncheckedIds.Contains(c.Item.GameId))
            .Where(c => keyword.Length == 0 || c.SearchText.Contains(keyword, StringComparison.Ordinal))
            .ToList();

        SyncCards(wanted);

        IsEmpty = Cards.Count == 0;
        var libraryEmpty = _snapshot.Count == 0;
        EmptyTitle = !_hasScanned ? "Loading mods…" : libraryEmpty ? "No Mods Imported Yet" : "No Mods Match the Filters";
        EmptyText = !_hasScanned ? "Reading your mod library"
            : libraryEmpty ? "Press Import from Folder to add mods and plugins from your PC"
            : "Change the keywords or game filters, or press Reset";

        var activeFilters = (selectedId is null ? 0 : 1)
            + (keyword.Length == 0 ? 0 : 1)
            + (uncheckedIds.Count > 0 ? 1 : 0);
        FilterSummary = $"Filters ({activeFilters})";
    }

    private void SyncCards(List<ModCardViewModel> wanted)
    {
        var wantedSet = new HashSet<ModCardViewModel>(wanted);
        for (var i = Cards.Count - 1; i >= 0; i--)
        {
            if (!wantedSet.Contains(Cards[i]))
            {
                Cards.RemoveAt(i);
            }
        }

        for (var i = 0; i < wanted.Count; i++)
        {
            var card = wanted[i];
            if (i < Cards.Count && ReferenceEquals(Cards[i], card))
            {
                continue;
            }

            var existing = Cards.IndexOf(card);
            if (existing >= 0)
            {
                Cards.Move(existing, i);
            }
            else
            {
                Cards.Insert(i, card);
            }
        }
    }

    [RelayCommand]
    private void ResetFilters()
    {
        _suppressFilter = true;
        try
        {
            Keyword = string.Empty;
            _searchDebounce.Stop();
            SelectedGameIndex = 0;
            foreach (var option in GameChecks)
            {
                option.IsChecked = true;
            }
        }
        finally
        {
            _suppressFilter = false;
        }

        _searchDebounce.Stop();
        ApplyFilters();
    }

    [RelayCommand]
    private async Task ImportFromFolder()
    {
        // Require an explicit game: with "All Games" selected this used to silently import
        // into whichever moddable game happened to sort first.
        var gameId = SelectedGameIndex > 0 && SelectedGameIndex < _gameFilterIds.Count
            ? _gameFilterIds[SelectedGameIndex]
            : null;

        if (gameId is null)
        {
            _toasts.Warning("Choose the game these mods are for in the \"Show\" filter first, then import.");
            return;
        }

        var dialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = $"Select a folder of mods/plugins to import for {GameFilterNames[SelectedGameIndex]}",
        };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            var folder = dialog.FolderName;
            var count = await Task.Run(() => _services.Mods.ImportFromFolder(folder, gameId));
            if (count == 0 && ModCompatibility.IsMinecraftJava(gameId))
            {
                _toasts.Warning("No .jar files found in that folder — Minecraft mods and plugins are .jar files.");
            }
            else
            {
                _toasts.Success($"Imported {count} mod(s).");
            }

            Refresh();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Import failed: {ex.Message}");
        }
    }

    [ObservableProperty]
    private bool _isImportingDrop;

    public string? DropTargetGameId(IReadOnlyList<string> paths)
    {
        if (SelectedGameIndex > 0 && SelectedGameIndex < _gameFilterIds.Count)
        {
            return _gameFilterIds[SelectedGameIndex];
        }

        return paths.Count > 0 && paths.All(p => System.IO.File.Exists(p) && ModCompatibility.IsJarFile(p))
            ? ModCompatibility.MinecraftJavaId
            : null;
    }

    public async Task ImportDroppedAsync(IReadOnlyList<string> paths)
    {
        if (paths.Count == 0 || IsImportingDrop)
        {
            return;
        }

        if (DropTargetGameId(paths) is not { } gameId)
        {
            _toasts.Warning("Choose the game these mods are for in the \"Show\" filter first, then drop them again.");
            return;
        }

        IsImportingDrop = true;
        try
        {
            var result = await Task.Run(() => _services.Mods.ImportFilesToLibrary(paths, gameId));
            if (result.Count > 0)
            {
                var gameName = _services.Games.Find(gameId)?.Name ?? gameId;
                _toasts.Success($"Imported {(result.Count == 1 ? result.Added.Concat(result.Replaced).First() : $"{result.Count} items")} for {gameName}.");
            }

            if (result.Skipped.Count > 0)
            {
                var lines = result.Skipped.Take(4).Select(s => $"{s.Name}: {s.Reason}");
                var more = result.Skipped.Count > 4 ? $" (+{result.Skipped.Count - 4} more)" : string.Empty;
                _toasts.Warning($"Not imported — {string.Join("; ", lines)}{more}");
            }

            Refresh();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Import failed: {ex.Message}");
        }
        finally
        {
            IsImportingDrop = false;
        }
    }

    public async Task AssignToServerAsync(ModCardViewModel card, string serverName)
    {
        try
        {
            var server = _services.Servers.Find(serverName);
            if (server is null)
            {
                return;
            }

            var item = card.Item;
            await Task.Run(() => _services.Mods.AssignToServer(item, server));
            _toasts.Success($"Added {item.FileName} to {server.ServerName}.");
            Refresh();
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

    public async Task RemoveAssignmentAsync(ModCardViewModel card, string serverName)
    {
        try
        {
            var server = _services.Servers.Find(serverName);
            if (server is null)
            {
                return;
            }

            var item = card.Item;
            await Task.Run(() => _services.Mods.RemoveFromServer(item, server));
            _toasts.Info($"Removed {item.FileName} from {serverName}.");
            Refresh();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't remove mod: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task DeleteMod(ModCardViewModel? card)
    {
        if (card is null)
        {
            return;
        }

        try
        {
            var confirmed = Views.Dialogs.ConfirmDialog.Show(
                System.Windows.Application.Current?.MainWindow,
                "Delete Mod",
                $"Delete '{card.Item.FileName}' from the mod library? Copies already added to servers are not removed.",
                confirmText: "Delete");
            if (!confirmed)
            {
                return;
            }

            var item = card.Item;
            await Task.Run(() => _services.Mods.DeleteFromLibrary(item));
            _toasts.Info($"Deleted {item.FileName} from the library.");
            Refresh();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't delete mod: {ex.Message}");
        }
    }

    public static string FormatBytes(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        double value = bytes;
        var unit = 0;
        while (value >= 1024.0 && unit < units.Length - 1)
        {
            value /= 1024.0;
            unit++;
        }

        return unit == 0 ? $"{bytes} B" : $"{value:0.0} {units[unit]}";
    }
}
