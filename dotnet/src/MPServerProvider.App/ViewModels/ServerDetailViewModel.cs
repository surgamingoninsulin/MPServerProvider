using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Native;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

public sealed record ConsoleUpdate(bool Reset, string Text);

public sealed partial class ServerDetailViewModel : ObservableObject
{
    private const int ConsoleResetAfterAppendedLines = 3000;

    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private readonly Action _onMutated;
    private readonly DispatcherPollTimer _timer;

    private DateTime _lastCpuSampleAt;
    private TimeSpan _lastCpuSampleTotal;
    private int _lastCpuSamplePid = -1;

    private IReadOnlyList<string> _shownConsole = Array.Empty<string>();
    private int _consoleAppendedSinceReset;
    private bool _isViewVisible = true;
    private bool _inShowOrHide;

    public ServerDetailViewModel(AppServices services, ToastService toasts, Action onMutated)
    {
        _services = services;
        _toasts = toasts;
        _onMutated = onMutated;
        _timer = new DispatcherPollTimer(TimeSpan.FromMilliseconds(800), SafeRefreshLiveFields);
        ResourcePack = new ResourcePackTabViewModel(services, toasts);
        LuckPerms = new LuckPermsTabViewModel(services, toasts);
        Scenarios = new ScenarioDownloaderTabViewModel(services, toasts);
    }

    public ResourcePackTabViewModel ResourcePack { get; }

    public LuckPermsTabViewModel LuckPerms { get; }

    public ScenarioDownloaderTabViewModel Scenarios { get; }

    public ServerInstance? Current { get; private set; }

    public event EventHandler<ConsoleUpdate>? ConsoleUpdated;

    [ObservableProperty]
    private bool _hasServer;

    [ObservableProperty]
    private string _nameText = string.Empty;

    [ObservableProperty]
    private string _statusText = "offline";

    [ObservableProperty]
    private bool _isOnline;

    [ObservableProperty]
    private string _gameText = string.Empty;

    [ObservableProperty]
    private string _commandText = string.Empty;

    // -- Mods tab -------------------------------------------------------------------------

    [ObservableProperty]
    private bool _supportsMods;

    private ServerModsTabViewModel? _modsTab;

    public ServerModsTabViewModel ModsTab => _modsTab ??= new ServerModsTabViewModel(_services, _toasts, this);

    // -- Resources tab ----------------------------------------------------------------------

    [ObservableProperty]
    private string _cpuText = "—";

    [ObservableProperty]
    private string _ramText = "—";

    [ObservableProperty]
    private string _uptimeText = "—";

    [ObservableProperty]
    private string _playersText = "N/A";

    [ObservableProperty]
    private string _connectionText = string.Empty;

    // -- txAdmin tab ------------------------------------------------------------------------

    [ObservableProperty]
    private bool _isFiveMOrRedM;

    [ObservableProperty]
    private string _txAdminUrl = "http://127.0.0.1:40120/";

    // -- Klei Token tab (Don't Starve Together) ---------------------------------------------

    [ObservableProperty]
    private bool _isDontStarve;

    [ObservableProperty]
    private string _kleiToken = string.Empty;

    [ObservableProperty]
    private string _kleiTokenStatus = string.Empty;

    [ObservableProperty]
    private bool _hasKleiToken;

    public string KleiLoginUrl => DstCluster.KleiLoginUrl;

    public event EventHandler? KleiTabRequested;

    // -- Settings tab -----------------------------------------------------------------------

    [ObservableProperty]
    private string _settingDescription = string.Empty;

    [ObservableProperty]
    private string _settingPort = string.Empty;

    [ObservableProperty]
    private string _settingRamMin = string.Empty;

    [ObservableProperty]
    private string _settingRamMax = string.Empty;

    [ObservableProperty]
    private string _settingStartCommand = string.Empty;

    [ObservableProperty]
    private string _settingStopCommand = string.Empty;

    [ObservableProperty]
    private bool _settingAutoRestart;

    // -- File Manager tab -------------------------------------------------------------------

    public System.Collections.ObjectModel.ObservableCollection<FileRowViewModel> Files { get; } = new();

    [ObservableProperty]
    private string _filesPathText = string.Empty;

    [ObservableProperty]
    private string _currentFilesDir = string.Empty;

    [ObservableProperty]
    private bool _canGoUp;

    public void Show(ServerInstance server)
    {
        // Show/Hide are driven by MyServersViewModel's poll timer; a modal "save changes?"
        // prompt below pumps messages, so the next tick must not re-enter and stack prompts.
        if (_inShowOrHide)
        {
            return;
        }
        _inShowOrHide = true;
        try
        {
            var isSameServer = Current is { } current && current.ServerName == server.ServerName;
            if (!isSameServer && Current is not null)
            {
                // Switching servers: offer to save unsaved editor changes first.
                PromptSaveEditorBeforeLeaving("switching servers");
                ForceCloseEditor();
            }

            Current = server;
            HasServer = true;
            if (!isSameServer)
            {
                SeedSettingsFields(server);
                LoadKleiToken(server);
                CurrentFilesDir = server.ServerRoot;
                ResetConsole();
                RefreshFiles();
            }
            ResourcePack.SetServer(server);
            LuckPerms.SetServer(server);
            Scenarios.SetServer(server);
            RefreshLiveFields();
            UpdateTimer();
        }
        catch (Exception ex)
        {
            ReportError("Could not show the server", ex);
        }
        finally
        {
            _inShowOrHide = false;
        }
    }

    public void Hide()
    {
        if (_inShowOrHide)
        {
            return;
        }
        _inShowOrHide = true;
        try
        {
            PromptSaveEditorBeforeLeaving("closing this server");
        }
        catch (Exception ex)
        {
            ReportError("Could not save the open file", ex);
        }
        finally
        {
            _inShowOrHide = false;
        }
        ForceCloseEditor();
        Current = null;
        HasServer = false;
        ResourcePack.SetServer(null);
        LuckPerms.SetServer(null);
        Scenarios.SetServer(null);
        ResetConsole();
        _timer.Stop();
    }

    public void SetViewVisible(bool visible)
    {
        _isViewVisible = visible;
        UpdateTimer();
        if (visible)
        {
            SafeRefreshLiveFields();
        }
    }

    private void UpdateTimer()
    {
        if (Current is not null && _isViewVisible)
        {
            _timer.Start();
        }
        else
        {
            _timer.Stop();
        }
    }

    public void ReportError(string what, Exception ex)
    {
        try
        {
            _toasts.Error($"{what}: {ex.Message}");
        }
        catch
        {
            // Reporting must never throw out of an event handler.
        }
    }

    private void SeedSettingsFields(ServerInstance server)
    {
        SettingDescription = server.ServerDescription;
        SettingPort = server.ServerPort.ToString();
        SettingRamMin = server.RamMinMb.ToString();
        SettingRamMax = server.RamMaxMb.ToString();
        SettingStartCommand = server.StartCommand;
        SettingStopCommand = server.StopCommand;
        SettingAutoRestart = server.AutoRestartOnCrash;
    }

    private void SafeRefreshLiveFields()
    {
        try
        {
            RefreshLiveFields();
        }
        catch
        {
            // Timer tick: best-effort, a transient failure must not spam toasts every 800 ms.
        }
    }

    public void RefreshLiveFields()
    {
        if (Current is not { } server)
        {
            return;
        }

        var state = _services.Processes.StateOf(server);
        IsOnline = state is ServerState.Running or ServerState.Starting;
        StatusText = state switch
        {
            ServerState.Running => "online",
            ServerState.Starting => _startStatus is { Length: > 0 } preparing ? $"starting · {preparing}" : "starting",
            ServerState.Stopping => "stopping",
            ServerState.Crashed => "crashed",
            ServerState.Installing => "installing",
            _ => "offline",
        };

        var game = _services.Games.Find(server.ServerGame);
        NameText = server.ServerName;
        GameText = $"{(game?.Name ?? server.ServerGame)}  •  {server.ServerVersion}  •  port {server.ServerPort}";
        IsFiveMOrRedM = server.ServerGame is "fivem" or "redm";
        IsDontStarve = server.ServerGame == DstCluster.GameId;
        SupportsMods = game?.Mods.Supported ?? false;
        ConnectionText = $"{server.ServerIp}:{server.ServerPort}";

        UpdateConsole(_services.Processes.ConsoleOf(server));

        var uptime = _services.Processes.UptimeOf(server);
        UptimeText = uptime == TimeSpan.Zero
            ? "—"
            : $"{(int)uptime.TotalHours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}";

        RefreshResourceTiles();
        ResourcePack.RefreshLive();
        LuckPerms.RefreshLive();
    }

    // -- Console (incremental) --------------------------------------------------------------

    public void ResyncConsole()
    {
        ResetConsole();
        SafeRefreshLiveFields();
    }

    private void ResetConsole()
    {
        _shownConsole = Array.Empty<string>();
        _consoleAppendedSinceReset = 0;
        ConsoleUpdated?.Invoke(this, new ConsoleUpdate(true, string.Empty));
    }

    private void UpdateConsole(IReadOnlyList<string> snapshot)
    {
        var shown = _shownConsole;
        if (snapshot.Count == shown.Count && (snapshot.Count == 0 || snapshot[^1] == shown[^1]) &&
            (snapshot.Count == 0 || snapshot[0] == shown[0]))
        {
            return; // unchanged
        }

        var appendFrom = FindAppendStart(shown, snapshot);
        if (appendFrom < 0 || _consoleAppendedSinceReset > ConsoleResetAfterAppendedLines)
        {
            _shownConsole = snapshot;
            _consoleAppendedSinceReset = 0;
            ConsoleUpdated?.Invoke(this, new ConsoleUpdate(true, string.Join(Environment.NewLine, snapshot)));
            return;
        }

        if (appendFrom >= snapshot.Count)
        {
            _shownConsole = snapshot;
            return;
        }

        var sb = new StringBuilder();
        for (var i = appendFrom; i < snapshot.Count; i++)
        {
            if (sb.Length > 0 || shown.Count > 0)
            {
                sb.Append(Environment.NewLine);
            }
            sb.Append(snapshot[i]);
        }
        _consoleAppendedSinceReset += snapshot.Count - appendFrom;
        _shownConsole = snapshot;
        ConsoleUpdated?.Invoke(this, new ConsoleUpdate(false, sb.ToString()));
    }

    internal static int FindAppendStart(IReadOnlyList<string> shown, IReadOnlyList<string> next)
    {
        if (shown.Count == 0)
        {
            return 0;
        }

        // d = number of lines dropped from the front of the rolling buffer since last time.
        for (var dropped = 0; dropped < shown.Count; dropped++)
        {
            var overlap = shown.Count - dropped;
            if (overlap > next.Count)
            {
                continue;
            }
            if (next[0] != shown[dropped] || next[overlap - 1] != shown[^1])
            {
                continue;
            }
            var match = true;
            for (var i = 1; i < overlap - 1; i++)
            {
                if (!ReferenceEquals(next[i], shown[dropped + i]) && next[i] != shown[dropped + i])
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                return overlap;
            }
        }
        return -1;
    }

    private void RefreshResourceTiles()
    {
        if (Current is not { } server || !IsOnline)
        {
            CpuText = "—";
            RamText = "—";
            _lastCpuSamplePid = -1;
            return;
        }

        var runner = _services.Processes.ProcessOf(server);
        if (runner is null)
        {
            CpuText = "—";
            RamText = "—";
            return;
        }

        try
        {
            using var process = Process.GetProcessById(runner.ProcessId);
            var now = DateTime.UtcNow;
            var totalCpu = process.TotalProcessorTime;

            if (_lastCpuSamplePid == runner.ProcessId && _lastCpuSampleAt != default)
            {
                var elapsedMs = (now - _lastCpuSampleAt).TotalMilliseconds;
                var cpuMs = (totalCpu - _lastCpuSampleTotal).TotalMilliseconds;
                if (elapsedMs > 0)
                {
                    var percent = cpuMs / elapsedMs / Environment.ProcessorCount * 100.0;
                    CpuText = $"{Math.Clamp(percent, 0, 100):0.0}%";
                }
            }
            else
            {
                CpuText = "…";
            }

            _lastCpuSamplePid = runner.ProcessId;
            _lastCpuSampleAt = now;
            _lastCpuSampleTotal = totalCpu;

            var ramMb = process.WorkingSet64 / (1024.0 * 1024.0);
            RamText = $"{ramMb:0.0} MB";
        }
        catch
        {
            // Best-effort: the process may have exited between the StateOf() check and here.
            CpuText = "—";
            RamText = "—";
        }
    }

    // -- Lifecycle commands -----------------------------------------------------------------

    [RelayCommand]
    private async Task Start()
    {
        if (Current is not { } server)
        {
            return;
        }
        if (server.ServerGame == DstCluster.GameId && DstCluster.ReadToken(_services.Servers.RootFor(server)) is null)
        {
            _toasts.Warning("Don't Starve Together needs a Klei cluster token first. Paste it in the Klei Token tab.");
            KleiTabRequested?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (_services.Games.Find(server.ServerGame) is { } game
            && !Views.Dialogs.CreativerseWorldDialog.EnsureWorld(System.Windows.Application.Current?.MainWindow, server, game, _services.Settings))
        {
            return;
        }

        _startStatus = null;
        var announcedDownload = false;
        var progress = new Progress<InstallProgress>(p =>
        {
            // Only slow preparation reports progress (a first-start Java or .NET download).
            _startStatus = p.Status;
            if (!announcedDownload)
            {
                announcedDownload = true;
                _toasts.Info($"{server.ServerName}: {p.Status} (first start only). Click Stop to cancel.");
            }
            SafeRefreshLiveFields();
        });
        try
        {
            await _services.Processes.StartAsync(server, progress);
            _toasts.Success($"Starting {server.ServerName}…");
        }
        catch (OperationCanceledException)
        {
            _toasts.Info($"Start of {server.ServerName} cancelled.");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not start {server.ServerName}: {ex.Message}");
        }
        finally
        {
            _startStatus = null;
        }
        SafeRefreshLiveFields();
        SafeMutated();
    }

    private string? _startStatus;

    [RelayCommand]
    private async Task Stop()
    {
        if (Current is not { } server)
        {
            return;
        }
        try
        {
            await _services.Processes.StopAsync(server);
            _toasts.Info($"{server.ServerName} stopped.");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not stop {server.ServerName}: {ex.Message}");
        }
        SafeRefreshLiveFields();
        SafeMutated();
    }

    [RelayCommand]
    private async Task Restart()
    {
        if (Current is not { } server)
        {
            return;
        }
        try
        {
            await _services.Processes.RestartAsync(server);
            _toasts.Success($"Restarting {server.ServerName}…");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not restart {server.ServerName}: {ex.Message}");
        }
        SafeRefreshLiveFields();
        SafeMutated();
    }

    private void SafeMutated()
    {
        try
        {
            _onMutated();
        }
        catch (Exception ex)
        {
            ReportError("Could not refresh the server list", ex);
        }
    }

    [RelayCommand]
    private void OpenFolder()
    {
        if (Current is not { } server)
        {
            return;
        }
        try
        {
            var root = _services.Servers.RootFor(server);
            Directory.CreateDirectory(root);
            OpenFolderInExplorer(root);
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not open folder: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Export()
    {
        if (Current is not { } server)
        {
            return;
        }

        try
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Export Server Config",
                FileName = $"{server.ServerName}.json",
                Filter = "Server config (*.json)|*.json|All files (*.*)|*.*",
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            // Portable: settings only, no absolute paths or stats (importable on any PC).
            File.WriteAllText(dialog.FileName, Core.Json.Json.Serialize(ServerExportData.FromServer(server)));
            _toasts.Success($"Exported {server.ServerName}.");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not export: {ex.Message}");
        }
    }

    // -- Console tab ---------------------------------------------------------------------

    [RelayCommand]
    private void SendCommand()
    {
        if (Current is not { } server || string.IsNullOrWhiteSpace(CommandText))
        {
            return;
        }
        try
        {
            var command = CommandText;
            CommandText = string.Empty;
            _services.Processes.SendCommand(server, command);
            RefreshLiveFields();
        }
        catch (Exception ex)
        {
            ReportError("Could not send the command", ex);
        }
    }

    // -- Klei Token tab -------------------------------------------------------------------

    private void LoadKleiToken(ServerInstance server)
    {
        if (server.ServerGame != DstCluster.GameId)
        {
            KleiToken = string.Empty;
            HasKleiToken = false;
            KleiTokenStatus = string.Empty;
            return;
        }

        var token = DstCluster.ReadToken(_services.Servers.RootFor(server));
        KleiToken = token ?? string.Empty;
        HasKleiToken = token is not null;
        KleiTokenStatus = token is null
            ? "No token saved yet. Sign in below, copy the token from Game Servers and paste it here."
            : "Token saved. Restart the server after changing it.";
    }

    [RelayCommand]
    private void SaveKleiToken()
    {
        if (Current is not { } server)
        {
            return;
        }

        try
        {
            DstCluster.SaveToken(_services.Servers.RootFor(server), KleiToken, server.ServerName, server.ServerPort);
            KleiToken = KleiToken.Trim();
            HasKleiToken = true;
            KleiTokenStatus = IsOnline
                ? "Token saved. Restart the server to use it."
                : "Token saved. You can start the server now.";
            _toasts.Success("Klei cluster token saved.");
        }
        catch (Exception ex)
        {
            KleiTokenStatus = ex.Message;
            _toasts.Error($"Couldn't save the token: {ex.Message}");
        }
    }

    [RelayCommand]
    private void OpenKleiInBrowser()
    {
        if (!Controls.ArtworkImages.OpenInBrowser(DstCluster.KleiLoginUrl))
        {
            _toasts.Error("Couldn't open the Klei account page.");
        }
    }

    // -- txAdmin tab ----------------------------------------------------------------------

    [RelayCommand]
    private void OpenTxAdminInBrowser()
    {
        try
        {
            Process.Start(new ProcessStartInfo(TxAdminUrl) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not open txAdmin: {ex.Message}");
        }
    }

    // -- Settings tab --------------------------------------------------------------------

    [RelayCommand]
    private void SaveSettings()
    {
        if (Current is not { } server)
        {
            return;
        }

        try
        {
            server.ServerDescription = SettingDescription;
            if (int.TryParse(SettingPort, out var port))
            {
                server.ServerPort = port;
            }
            if (int.TryParse(SettingRamMin, out var ramMin))
            {
                server.RamMinMb = ramMin;
            }
            if (int.TryParse(SettingRamMax, out var ramMax))
            {
                server.RamMaxMb = ramMax;
            }
            server.StartCommand = SettingStartCommand;
            server.StopCommand = SettingStopCommand;
            server.AutoRestartOnCrash = SettingAutoRestart;

            _services.Servers.Save(server);
            _toasts.Success("Server settings saved.");
            RefreshLiveFields();
            _onMutated();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not save server settings: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task UpdateServer()
    {
        if (Current is not { } server)
        {
            return;
        }

        try
        {
            if (_services.Games.Find(server.ServerGame) is not { } game)
            {
                _toasts.Error($"Can't update {server.ServerName}: its game definition '{server.ServerGame}' is missing.");
                return;
            }

            if (game.Install.Provider == "manual")
            {
                _toasts.Info($"{game.Name} servers are installed manually: replace the files in its folder (Open Folder) to update it.");
                return;
            }

            var state = _services.Processes.StateOf(server);
            var busy = state is ServerState.Running or ServerState.Starting or ServerState.Stopping
                       || _services.Processes.IsAutoRestartPending(server);
            if (busy)
            {
                var stop = Views.Dialogs.ConfirmDialog.Show(
                    Application.Current.MainWindow,
                    "Update Server",
                    $"'{server.ServerName}' is running. Stop it now so it can be updated?",
                    confirmText: "Stop and Continue",
                    destructive: false);
                if (!stop)
                {
                    return;
                }

                try
                {
                    await _services.Processes.StopAsync(server);
                }
                catch (InvalidOperationException) when (_services.Processes.StateOf(server) == ServerState.Stopped)
                {
                    // It stopped on its own in the meantime.
                }
            }

            ForceCloseEditor();
            var dialog = new Views.Dialogs.UpdateServerDialog(_services, _toasts, server, game) { Owner = Application.Current.MainWindow };
            if (dialog.ShowDialog() == true)
            {
                RefreshLiveFields();
                _onMutated();
            }
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not update {server.ServerName}: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task DeleteServer()
    {
        if (Current is not { } server)
        {
            return;
        }

        try
        {
            var state = _services.Processes.StateOf(server);
            var busy = state is ServerState.Running or ServerState.Starting or ServerState.Stopping
                       || _services.Processes.IsAutoRestartPending(server);
            var confirmed = Views.Dialogs.ConfirmDialog.Show(
                Application.Current.MainWindow,
                "Delete Server",
                busy
                    ? $"'{server.ServerName}' is running. Stop it, then delete it and all of its files? This cannot be undone."
                    : $"Delete '{server.ServerName}' and all of its files? This cannot be undone.",
                confirmText: busy ? "Stop and Delete" : "Delete");
            if (!confirmed)
            {
                return;
            }

            if (busy)
            {
                // Never delete files out from under a live process.
                _toasts.Info($"Stopping {server.ServerName} before deleting it…");
                try
                {
                    await _services.Processes.StopAsync(server);
                }
                catch (InvalidOperationException) when (_services.Processes.StateOf(server) == ServerState.Stopped)
                {
                    // It stopped on its own in the meantime.
                }
            }

            ForceCloseEditor();
            _services.Servers.Delete(server, deleteFiles: true);
            _toasts.Info($"{server.ServerName} deleted.");
            Hide();
            _onMutated();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not delete {server.ServerName}: {ex.Message}");
        }
    }

    // -- File Manager tab ------------------------------------------------------------------

    private string ServerRoot => Current?.ServerRoot ?? string.Empty;

    [RelayCommand]
    private void RefreshFiles()
    {
        if (Current is null)
        {
            return;
        }

        try
        {
            var root = ServerRoot;
            if (string.IsNullOrEmpty(CurrentFilesDir) ||
                !Directory.Exists(CurrentFilesDir) ||
                !PathGuard.TryResolveInsideRoot(root, CurrentFilesDir, out _, allowRootItself: true))
            {
                CurrentFilesDir = root;
            }

            var fullRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
            var fullDir = Path.TrimEndingDirectorySeparator(Path.GetFullPath(CurrentFilesDir));
            CanGoUp = !string.Equals(fullRoot, fullDir, StringComparison.OrdinalIgnoreCase);
            var relative = Path.GetRelativePath(fullRoot, fullDir);
            FilesPathText = relative == "." ? Path.GetFileName(fullRoot) + Path.DirectorySeparatorChar : Path.Combine(Path.GetFileName(fullRoot), relative);

            Files.Clear();
            if (!Directory.Exists(CurrentFilesDir))
            {
                return;
            }

            foreach (var dir in Directory.EnumerateDirectories(CurrentFilesDir).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
            {
                Files.Add(new FileRowViewModel(Path.GetFileName(dir), true, string.Empty, dir));
            }
            foreach (var file in Directory.EnumerateFiles(CurrentFilesDir).OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            {
                if (file.EndsWith(".mpsp-tmp", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                long size = 0;
                try { size = new FileInfo(file).Length; } catch { /* best-effort */ }
                Files.Add(new FileRowViewModel(Path.GetFileName(file), false, FileRowViewModel.HumanReadableSize(size), file));
            }
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not list files: {ex.Message}");
        }
    }

    [RelayCommand]
    private void UpFolder()
    {
        if (Current is null)
        {
            return;
        }
        try
        {
            var root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(ServerRoot));
            var current = Path.TrimEndingDirectorySeparator(Path.GetFullPath(CurrentFilesDir));
            if (!string.Equals(current, root, StringComparison.OrdinalIgnoreCase))
            {
                var parent = Directory.GetParent(current)?.FullName ?? root;
                CurrentFilesDir = PathGuard.IsInsideRoot(root, parent, allowRootItself: true) ? parent : root;
            }
            RefreshFiles();
        }
        catch (Exception ex)
        {
            ReportError("Could not go up a folder", ex);
        }
    }

    public void OpenEntry(FileRowViewModel entry)
    {
        if (Current is null)
        {
            return;
        }

        try
        {
            if (!PathGuard.TryResolveInsideRoot(ServerRoot, entry.FullPath, out var full))
            {
                _toasts.Error($"'{entry.Name}' is outside this server's folder (or is a link) and can't be opened here.");
                return;
            }

            if (entry.IsDirectory)
            {
                CurrentFilesDir = full;
                RefreshFiles();
                return;
            }

            switch (TextFileDetector.Classify(full))
            {
                case TextEditability.Editable:
                    OpenInEditor(full);
                    break;
                case TextEditability.NotFound:
                    _toasts.Warning($"'{entry.Name}' no longer exists.");
                    RefreshFiles();
                    break;
                case TextEditability.TooLarge:
                    _toasts.Warning($"'{entry.Name}' is larger than {TextFileDetector.DefaultMaxEditableBytes / (1024 * 1024)} MB, too big for the built-in editor — showing it in Explorer.");
                    RevealInExplorer(full);
                    break;
                default:
                    OpenNonEditable(full, entry.Name);
                    break;
            }
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not open {entry.Name}: {ex.Message}");
        }
    }

    private void OpenNonEditable(string fullPath, string name)
    {
        if (ShellOpenPolicy.IsUnsafeToShellOpen(fullPath))
        {
            _toasts.Info($"'{name}' can't be edited here — showing it in Explorer.");
            RevealInExplorer(fullPath);
            return;
        }

        // Images, archives, ... : hand them to their default app, but de-elevated so nothing a
        // downloaded file triggers ever runs with administrator rights.
        if (IsElevated())
        {
            try
            {
                DeElevatedLauncher.Launch($"explorer.exe \"{fullPath}\"");
                return;
            }
            catch
            {
                RevealInExplorer(fullPath);
                return;
            }
        }
        Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
    }

    private static bool IsElevated()
    {
        try
        {
            return OperatingSystem.IsWindows() && DeElevatedLauncher.IsCurrentProcessElevated();
        }
        catch
        {
            return true; // assume the worst
        }
    }

    private static void RevealInExplorer(string fullPath)
    {
        var args = $"/select,\"{fullPath}\"";
        if (IsElevated())
        {
            try
            {
                DeElevatedLauncher.Launch($"explorer.exe {args}");
                return;
            }
            catch
            {
                // Fall through: explorer.exe hands the request to the already-running
                // (non-elevated) shell process anyway.
            }
        }
        Process.Start(new ProcessStartInfo("explorer.exe", args) { UseShellExecute = false });
    }

    private static void OpenFolderInExplorer(string folder)
    {
        var args = $"\"{folder}\"";
        if (IsElevated())
        {
            try
            {
                DeElevatedLauncher.Launch($"explorer.exe {args}");
                return;
            }
            catch
            {
                // fall back below
            }
        }
        Process.Start(new ProcessStartInfo("explorer.exe", args) { UseShellExecute = false });
    }

    [RelayCommand]
    private void OpenFilesInExplorer()
    {
        try
        {
            if (Current is null || !PathGuard.TryResolveInsideRoot(ServerRoot, CurrentFilesDir, out var dir, allowRootItself: true))
            {
                return;
            }
            OpenFolderInExplorer(dir);
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not open folder: {ex.Message}");
        }
    }

    [RelayCommand]
    private void NewFile()
    {
        if (Current is null)
        {
            return;
        }
        try
        {
            var dir = PathGuard.EnsureInsideRoot(ServerRoot, CurrentFilesDir, allowRootItself: true);
            var path = Path.Combine(dir, "New File.txt");
            var i = 1;
            while (File.Exists(path) || Directory.Exists(path))
            {
                path = Path.Combine(dir, $"New File ({++i}).txt");
            }
            path = PathGuard.EnsureInsideRoot(ServerRoot, path);
            File.WriteAllText(path, string.Empty);
            RefreshFiles();
            OpenInEditor(path);
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not create file: {ex.Message}");
        }
    }

    public event EventHandler? FilesTabRequested;

    public void ShowInFiles(string folder)
    {
        if (Current is null || !PathGuard.TryResolveInsideRoot(ServerRoot, folder, out var full, allowRootItself: true)
            || !Directory.Exists(full))
        {
            _toasts.Warning("That folder isn't inside this server (or no longer exists).");
            return;
        }

        CurrentFilesDir = full;
        RefreshFiles();
        FilesTabRequested?.Invoke(this, EventArgs.Empty);
    }

    [ObservableProperty]
    private bool _isCopyingFiles;

    public async Task CopyDroppedFilesAsync(IReadOnlyList<string> paths)
    {
        if (Current is null || paths.Count == 0 || IsCopyingFiles)
        {
            return;
        }

        try
        {
            var root = ServerRoot;
            var dir = PathGuard.EnsureInsideRoot(root, CurrentFilesDir, allowRootItself: true);
            var sources = paths
                .Select(p => Path.TrimEndingDirectorySeparator(Path.GetFullPath(p)))
                .Where(p => File.Exists(p) || Directory.Exists(p))
                .ToList();

            var intoItself = sources.Where(s => Directory.Exists(s) && PathGuard.IsInsideRoot(s, dir, allowRootItself: true)).ToList();
            sources = sources.Except(intoItself).Where(s =>
                !string.Equals(Path.GetDirectoryName(s), Path.TrimEndingDirectorySeparator(dir), StringComparison.OrdinalIgnoreCase)).ToList();
            if (sources.Count == 0)
            {
                _toasts.Info(intoItself.Count > 0 ? "A folder can't be copied into itself." : "Those files are already in this folder.");
                return;
            }

            var clashes = sources.Where(s => File.Exists(Path.Combine(dir, Path.GetFileName(s))) || Directory.Exists(Path.Combine(dir, Path.GetFileName(s)))).ToList();
            var replace = true;
            if (clashes.Count > 0)
            {
                var names = string.Join(", ", clashes.Take(5).Select(Path.GetFileName)) + (clashes.Count > 5 ? $" and {clashes.Count - 5} more" : string.Empty);
                replace = Views.Dialogs.ConfirmDialog.Show(
                    Application.Current.MainWindow,
                    "Replace existing files?",
                    $"{names} already exist{(clashes.Count == 1 ? "s" : string.Empty)} here. Replace {(clashes.Count == 1 ? "it" : "them")}, or skip and copy only the rest?",
                    confirmText: "Replace", destructive: true, cancelText: "Skip");
                if (!replace)
                {
                    sources = sources.Except(clashes).ToList();
                }
            }

            if (EditorFilePath is { } open && sources.Any(s =>
                    string.Equals(Path.Combine(dir, Path.GetFileName(s)), open, StringComparison.OrdinalIgnoreCase)))
            {
                ForceCloseEditor(); // its file is about to be overwritten
            }

            IsCopyingFiles = true;
            var copied = await Task.Run(() =>
            {
                var count = 0;
                foreach (var source in sources)
                {
                    var target = PathGuard.EnsureInsideRoot(root, Path.Combine(dir, Path.GetFileName(source)));
                    FileDrop.Copy(source, target);
                    count++;
                }

                return count;
            });

            if (copied > 0)
            {
                _toasts.Success(copied == 1 ? $"Copied {Path.GetFileName(sources[0])}." : $"Copied {copied} items.");
            }
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't copy the dropped files: {ex.Message}");
        }
        finally
        {
            IsCopyingFiles = false;
            RefreshFiles();
        }
    }

    public void DeleteEntry(FileRowViewModel entry)
    {
        if (Current is null)
        {
            return;
        }

        try
        {
            if (!PathGuard.TryResolveInsideRoot(ServerRoot, entry.FullPath, out var full))
            {
                _toasts.Error($"'{entry.Name}' is outside this server's folder (or is a link) and can't be deleted here.");
                return;
            }

            var confirmed = Views.Dialogs.ConfirmDialog.Show(
                Application.Current.MainWindow,
                entry.IsDirectory ? "Delete Folder" : "Delete File",
                entry.IsDirectory
                    ? $"Delete the folder '{entry.Name}' and everything in it? This cannot be undone."
                    : $"Delete '{entry.Name}'? This cannot be undone.",
                confirmText: "Delete");
            if (!confirmed)
            {
                return;
            }

            // If the open editor file is (inside) what's being deleted, close it — there's
            // nothing left to save to.
            if (EditorFilePath is { } open &&
                (string.Equals(open, full, StringComparison.OrdinalIgnoreCase) ||
                 (entry.IsDirectory && PathGuard.IsInsideRoot(full, open))))
            {
                ForceCloseEditor();
            }

            if (entry.IsDirectory)
            {
                Directory.Delete(full, recursive: true);
            }
            else
            {
                File.Delete(full);
            }
            RefreshFiles();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not delete {entry.Name}: {ex.Message}");
            RefreshFiles();
        }
    }

    // -- Built-in text editor --------------------------------------------------------------

    private TextFileDocument? _editorDocument;

    public string? EditorFilePath { get; private set; }

    public Func<string>? EditorTextProvider { get; set; }

    public event EventHandler<string>? EditorTextLoaded;

    [ObservableProperty]
    private bool _isEditorOpen;

    [ObservableProperty]
    private bool _isEditorDirty;

    [ObservableProperty]
    private string _editorFileName = string.Empty;

    [ObservableProperty]
    private string _editorRelativePath = string.Empty;

    [ObservableProperty]
    private string _editorInfoText = string.Empty;

    public void MarkEditorDirty()
    {
        if (IsEditorOpen && !IsEditorDirty)
        {
            IsEditorDirty = true;
        }
    }

    private void OpenInEditor(string fullPath)
    {
        if (Current is null)
        {
            return;
        }
        if (string.Equals(EditorFilePath, fullPath, StringComparison.OrdinalIgnoreCase) && IsEditorOpen)
        {
            return; // already open
        }
        if (!ConfirmDiscardEditorChanges("open another file"))
        {
            return;
        }

        var safePath = PathGuard.EnsureInsideRoot(ServerRoot, fullPath);
        var editability = TextFileDetector.Classify(safePath);
        if (editability != TextEditability.Editable)
        {
            _toasts.Warning($"'{Path.GetFileName(safePath)}' can't be opened in the editor ({editability}).");
            return;
        }

        var document = TextFileDocument.Load(safePath);
        _editorDocument = document;
        EditorFilePath = safePath;
        EditorFileName = Path.GetFileName(safePath);
        EditorRelativePath = Path.GetRelativePath(ServerRoot, safePath);
        EditorInfoText = document.Description;
        IsEditorOpen = true;
        IsEditorDirty = false;
        EditorTextLoaded?.Invoke(this, TextFileDocument.NormalizeLineEndings(document.Text, "\r\n"));
        IsEditorDirty = false;
    }

    [RelayCommand]
    private void SaveEditor()
    {
        if (!IsEditorOpen || _editorDocument is null || EditorFilePath is null || Current is null)
        {
            return;
        }
        try
        {
            var path = PathGuard.EnsureInsideRoot(ServerRoot, EditorFilePath);
            var text = EditorTextProvider?.Invoke() ?? throw new InvalidOperationException("The editor isn't ready.");
            _editorDocument.Save(path, text);
            IsEditorDirty = false;
            _toasts.Success($"Saved {EditorFileName}.");
            RefreshFiles();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not save {EditorFileName}: {ex.Message}");
        }
    }

    [RelayCommand]
    private void RevertEditor()
    {
        if (!IsEditorOpen || EditorFilePath is null)
        {
            return;
        }
        try
        {
            if (IsEditorDirty && !Views.Dialogs.ConfirmDialog.Show(
                    Application.Current.MainWindow,
                    "Revert Changes",
                    $"Discard your unsaved changes to '{EditorFileName}' and reload it from disk?",
                    confirmText: "Revert"))
            {
                return;
            }

            var path = EditorFilePath;
            IsEditorDirty = false;
            EditorFilePath = null; // force a reload
            OpenInEditor(path);
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not reload {EditorFileName}: {ex.Message}");
            ForceCloseEditor();
        }
    }

    [RelayCommand]
    private void CloseEditor()
    {
        try
        {
            if (ConfirmDiscardEditorChanges("close it"))
            {
                ForceCloseEditor();
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not close the editor", ex);
        }
    }

    public bool ConfirmDiscardEditorChanges(string action)
    {
        if (!IsEditorOpen || !IsEditorDirty)
        {
            return true;
        }
        return Views.Dialogs.ConfirmDialog.Show(
            Application.Current.MainWindow,
            "Unsaved Changes",
            $"'{EditorFileName}' has unsaved changes. Discard them and {action}?",
            confirmText: "Discard",
            destructive: true,
            cancelText: "Keep Editing");
    }

    public void PromptSaveEditorBeforeLeaving(string action)
    {
        if (!IsEditorOpen || !IsEditorDirty)
        {
            return;
        }
        var save = Views.Dialogs.ConfirmDialog.Show(
            Application.Current.MainWindow,
            "Unsaved Changes",
            $"'{EditorFileName}' has unsaved changes. Save them before {action}?",
            confirmText: "Save",
            destructive: false,
            cancelText: "Discard");
        if (save)
        {
            SaveEditor();
        }
    }

    private void ForceCloseEditor()
    {
        _editorDocument = null;
        EditorFilePath = null;
        EditorFileName = string.Empty;
        EditorRelativePath = string.Empty;
        EditorInfoText = string.Empty;
        IsEditorDirty = false;
        IsEditorOpen = false;
        EditorTextLoaded?.Invoke(this, string.Empty);
        IsEditorDirty = false;
    }
}
