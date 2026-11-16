using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Json;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

/// <summary>
/// Port of <c>MPServerProvider.App.Views.MyServersPage</c>'s own code-behind logic (decompile
/// token 0x02000016, see <c>docs/decompile-report/report/MPServerProvider.App.dll/
/// MPServerProvider.App.Views/0x02000016_MyServersPage.md</c>): loads <see cref="AppServices"/>'s
/// <c>Servers</c> repository, renders the searchable left-hand list, and drives selection into
/// the embedded <see cref="ServerDetailViewModel"/>. Mirrors the already-proven Rust reference's
/// <c>MyServersController</c> (<c>crates/mpsp-app/src/pages/my_servers.rs</c>): a steady poll
/// (rather than a push subscription) keeps the list's status dots/active count fresh while the
/// page is on screen.
/// <para>
/// HARDENING: the poll no longer re-reads every <c>server.json</c> and rebuilds the list every
/// 1.5 s. Each tick only reads in-memory process state and diffs the rows in place (by server
/// name); the disk is re-scanned only every <see cref="DiskReloadEveryTicks"/> ticks and after
/// the app's own changes. The old <c>Clear()</c>+rebuild made the ListBox push a <c>null</c>
/// selection on every tick, which re-seeded the Settings tab and reset the File Manager — the
/// "my settings get reverted" bug. Selection changes caused by a refresh are now ignored.
/// </para>
/// </summary>
public sealed partial class MyServersViewModel : ObservableObject, IDisposable
{
    /// <summary>Re-scan the servers folders from disk once every N poll ticks (~30 s).</summary>
    private const int DiskReloadEveryTicks = 20;

    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private readonly DispatcherPollTimer _timer;
    private bool _refreshing;
    private bool _running;
    private int _tick;
    private string? _lastTickError;

    public MyServersViewModel(AppServices services, ToastService toasts)
    {
        _services = services;
        _toasts = toasts;
        Detail = new ServerDetailViewModel(services, toasts, OnServerMutated);
        _timer = new DispatcherPollTimer(TimeSpan.FromMilliseconds(1500), OnTick);
    }

    public ObservableCollection<ServerRowViewModel> Servers { get; } = new();

    public ServerDetailViewModel Detail { get; }

    [ObservableProperty]
    private ServerRowViewModel? _selectedServer;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _activeCountText = "0 active servers";

    [ObservableProperty]
    private bool _hasSelection;

    /// <summary>Drives the empty-state ("No Servers Yet") vs. the sidebar list's visibility.</summary>
    [ObservableProperty]
    private bool _hasAnyServers;

    partial void OnSelectedServerChanged(ServerRowViewModel? value)
    {
        if (_refreshing)
        {
            // Transient selection churn from the in-place row diff (a replaced row briefly
            // deselects): never forward it to the detail panel. RefreshRows restores the
            // selection itself once the diff is done.
            return;
        }

        HasSelection = value is not null;
        if (value is not null)
        {
            Detail.Show(value.Server);
        }
        else
        {
            Detail.Hide();
        }
    }

    partial void OnSearchTextChanged(string value) => SafeRun(RefreshRows);

    /// <summary>
    /// Starts polling (call when the page becomes visible, e.g. from
    /// <c>INavigationAware.OnNavigatedTo</c>). Idempotent; does an immediate disk scan.
    /// </summary>
    public void Start()
    {
        SafeRun(() =>
        {
            ReloadFromDisk();
            RefreshRows();
        });

        if (!_running)
        {
            _running = true;
            _timer.Start();
        }
    }

    /// <summary>Stops polling (call from <c>INavigationAware.OnNavigatedFrom</c>).</summary>
    public void Stop()
    {
        _running = false;
        _timer.Stop();
        Detail.SetViewVisible(false);
    }

    private void OnTick()
    {
        SafeRun(() =>
        {
            if (++_tick % DiskReloadEveryTicks == 0)
            {
                ReloadFromDisk();
            }

            RefreshRows();
        });
    }

    /// <summary>
    /// Runs a refresh step, turning any failure (e.g. the servers drive was removed) into one
    /// toast per distinct message instead of an exception escaping a timer tick (which used to
    /// stack a modal crash dialog per tick).
    /// </summary>
    private void SafeRun(Action action)
    {
        try
        {
            action();
            _lastTickError = null;
        }
        catch (Exception ex)
        {
            if (ex.Message != _lastTickError)
            {
                _lastTickError = ex.Message;
                _toasts.Error($"Could not refresh the server list: {ex.Message}");
            }
        }
    }

    private void ReloadFromDisk() => _services.Servers.Reload();

    /// <summary>
    /// Port of <c>MyServersPage.ReloadList()</c>/<c>RefreshStates()</c>, as an in-place diff:
    /// unchanged rows are kept, changed rows are replaced at their index, removed/added rows are
    /// removed/inserted, and the selection is restored without ever notifying the detail panel
    /// of a transient <c>null</c>.
    /// </summary>
    /// <summary>Selects the server named <paramref name="serverName"/> (e.g. when another page
    /// links to it), clearing a search filter that would hide it.</summary>
    public void Select(string serverName)
    {
        if (!Servers.Any(r => r.Server.ServerName == serverName) && SearchText.Length > 0)
        {
            SearchText = string.Empty;
        }

        RefreshRows();
        if (Servers.FirstOrDefault(r => r.Server.ServerName == serverName) is { } row)
        {
            SelectedServer = row;
        }
    }

    private void RefreshRows()
    {
        var all = _services.Servers.Servers.OrderBy(s => s.ServerName, StringComparer.OrdinalIgnoreCase).ToList();
        HasAnyServers = all.Count > 0;

        var selectedName = SelectedServer?.Server.ServerName;
        var filter = SearchText.Trim();

        var active = 0;
        var desired = new List<(ServerInstance Server, string GameName, ServerState State)>();
        foreach (var server in all)
        {
            var state = _services.Processes.StateOf(server);
            if (state is ServerState.Running or ServerState.Starting)
            {
                active++;
            }

            if (!string.IsNullOrEmpty(filter) &&
                server.ServerName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            var gameName = _services.Games.Find(server.ServerGame)?.Name ?? server.ServerGame;
            desired.Add((server, gameName, state));
        }

        ActiveCountText = $"{active} active server{(active == 1 ? "" : "s")}";

        _refreshing = true;
        try
        {
            var desiredNames = new HashSet<string>(desired.Select(d => d.Server.ServerName), StringComparer.Ordinal);
            for (var i = Servers.Count - 1; i >= 0; i--)
            {
                if (!desiredNames.Contains(Servers[i].Server.ServerName))
                {
                    Servers.RemoveAt(i);
                }
            }

            for (var i = 0; i < desired.Count; i++)
            {
                var (server, gameName, state) = desired[i];
                var existingIndex = -1;
                for (var j = i; j < Servers.Count; j++)
                {
                    if (Servers[j].Server.ServerName == server.ServerName)
                    {
                        existingIndex = j;
                        break;
                    }
                }

                if (existingIndex < 0)
                {
                    Servers.Insert(i, new ServerRowViewModel(server, gameName, state));
                    continue;
                }

                if (existingIndex != i)
                {
                    Servers.Move(existingIndex, i);
                }

                var row = Servers[i];
                if (!ReferenceEquals(row.Server, server) || row.State != state || row.Subtitle != gameName || row.Name != server.ServerName)
                {
                    Servers[i] = new ServerRowViewModel(server, gameName, state);
                }
            }

            while (Servers.Count > desired.Count)
            {
                Servers.RemoveAt(Servers.Count - 1);
            }
        }
        finally
        {
            _refreshing = false;
        }

        var restored = selectedName is null
            ? null
            : Servers.FirstOrDefault(r => r.Server.ServerName == selectedName);
        if (!ReferenceEquals(restored, SelectedServer))
        {
            // Same server -> Detail.Show sees the same name and keeps the user's in-progress
            // edits; a genuinely removed server -> Hide.
            SelectedServer = restored;
        }
        else
        {
            HasSelection = restored is not null;
        }
    }

    private void OnServerMutated()
    {
        SafeRun(() =>
        {
            ReloadFromDisk();
            RefreshRows();
        });
    }

    [RelayCommand]
    private void ConnectTunnel()
    {
        var dialog = new Views.Dialogs.TunnelDialog(_services, _toasts, SelectedServer?.Server);
        dialog.Owner = Application.Current?.MainWindow;
        dialog.ShowDialog();
    }

    /// <summary>
    /// Port of <c>MyServersPage.OnImportServerConfig</c>: opens a native file picker for a
    /// previously-exported <c>server.json</c> and imports it as a new server instance, matching
    /// the proven Rust reference's <c>rfd</c> file-dialog based import.
    /// <para>
    /// SECURITY: an imported file is untrusted. Its paths (<c>server_root</c>, <c>mods_folder</c>,
    /// <c>server_file</c>) are never used — the import gets a fresh folder of its own, the mods
    /// folder comes from the game definition, and stats are reset. A custom start command is
    /// shown to the user for confirmation before it is kept.
    /// </para>
    /// </summary>
    [RelayCommand]
    private void ImportServerConfig()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Import Server Config",
            Filter = "Server config (*.json)|*.json|All files (*.*)|*.*",
        };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            var text = File.ReadAllText(dialog.FileName);
            // Reads both the portable export (ServerExportData) and an old full server.json;
            // either way only settings come through — never paths or stats.
            var imported = Json.Deserialize<ServerExportData>(text)?.ToServerInstance();
            if (imported is null || string.IsNullOrWhiteSpace(imported.ServerName))
            {
                _toasts.Error("That file doesn't look like a valid server config.");
                return;
            }

            var game = _services.Games.Find(imported.ServerGame);
            if (game is null)
            {
                _toasts.Error($"That config is for an unknown game ('{imported.ServerGame}'). Add the game to your library first.");
                return;
            }

            imported.ServerName = imported.ServerName.Trim();
            if (ServerRepository.ValidateServerName(imported.ServerName) is { } nameError)
            {
                _toasts.Error($"Can't import: {nameError}");
                return;
            }

            // Unique name (re-checked after every suffix, so "X (Imported)" can't collide too).
            var baseName = imported.ServerName;
            var n = 1;
            while (_services.Servers.NameTaken(imported.ServerName))
            {
                imported.ServerName = n == 1 ? $"{baseName} (Imported)" : $"{baseName} (Imported {n})";
                n++;
            }

            if (ServerRepository.ValidateServerName(imported.ServerName) is { } suffixedError)
            {
                _toasts.Error($"Can't import: {suffixedError}");
                return;
            }

            // Never trust paths from the file: fresh folder, definition-derived mods folder.
            imported.SourceFile = null;
            imported.ServerRoot = _services.Servers.NewServerDir(imported.ServerName);
            imported.ModsFolder = InstallService.ModsFolderFor(game, imported.ServerType);
            imported.ServerGenres = game.Genres;
            imported.Stats = new ServerStats();
            imported.CreateFirewallRule = false;

            var defaultArgs = game.Runtime.Arguments ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(imported.StartCommand) && imported.StartCommand.Trim() != defaultArgs.Trim())
            {
                var keep = Views.Dialogs.ConfirmDialog.Show(
                    Application.Current?.MainWindow,
                    "Custom Start Command",
                    $"This config uses a custom start command:\n\n{imported.StartCommand}\n\n" +
                    "Start commands run programs on this PC. Only keep it if you trust whoever gave you this file. " +
                    "Otherwise the game's default command is used.",
                    confirmText: "Keep It",
                    destructive: true);
                if (!keep)
                {
                    imported.StartCommand = defaultArgs;
                }
            }

            _services.Servers.Save(imported);
            _toasts.Success($"Imported '{imported.ServerName}'. Its server files aren't included — install or copy them into {imported.ServerRoot}.");
            OnServerMutated();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not import server config: {ex.Message}");
        }
    }

    public void Dispose() => _timer.Dispose();
}
