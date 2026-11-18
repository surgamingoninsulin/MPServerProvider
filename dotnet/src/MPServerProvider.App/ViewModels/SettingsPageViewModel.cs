using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

public sealed partial class SettingsPageViewModel : ObservableObject
{
    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private readonly List<DriveUsage> _drives = new();
    private bool _loadingDrives;
    private bool _isLoadingSettings;

    public ObservableCollection<string> DriveLabels { get; } = new();

    public ObservableCollection<FirewallRuleViewModel> Rules { get; } = new();

    [ObservableProperty] private string _diskUsageText = "—";
    [ObservableProperty] private string _diskUsageCaption = string.Empty;
    [ObservableProperty] private double _diskUsageFraction;

    [ObservableProperty] private string _gameFilesText = "—";
    [ObservableProperty] private string _gameFilesCaption = string.Empty;
    [ObservableProperty] private double _gameFilesFraction;

    [ObservableProperty] private int _selectedDriveIndex;
    [ObservableProperty] private string _installPathText = string.Empty;

    [ObservableProperty] private bool _startOnBoot;
    [ObservableProperty] private bool _startMinimized;
    [ObservableProperty] private bool _restoreServers;
    [ObservableProperty] private bool _newsAutoScroll;
    [ObservableProperty] private string _curseForgeApiKey = string.Empty;
    [ObservableProperty] private string _modIoApiKey = string.Empty;
    [ObservableProperty] private string _modIoApiHost = string.Empty;

    [ObservableProperty] private string _cpuText = "—";
    [ObservableProperty] private double _cpuFraction;
    [ObservableProperty] private string _ramText = "—";
    [ObservableProperty] private double _ramFraction;
    [ObservableProperty] private string _activeServersText = "0";

    [ObservableProperty] private string _specCpu = "—";
    [ObservableProperty] private string _specRam = "—";
    [ObservableProperty] private string _specGpu = "—";
    [ObservableProperty] private string _specOs = "—";

    [ObservableProperty] private bool _noRules = true;

    public SettingsPageViewModel(AppServices services, ToastService toasts)
    {
        _services = services;
        _toasts = toasts;
    }

    public void Initialize()
    {
        LoadSpecs();
        LoadSettings();
        LoadDrives();
        RefreshStorage();
        RefreshRules();
        RefreshLive();

        if (_services.Settings.LoadError is { } loadError)
        {
            _toasts.Warning(loadError);
        }
    }

    public void OnNavigatedTo()
    {
        RefreshStorage();
        RefreshRules();
    }

    private void LoadSettings()
    {
        var settings = _services.Settings.Settings;
        _isLoadingSettings = true;
        try
        {
            StartOnBoot = settings.StartOnBoot;
            StartMinimized = settings.StartMinimizedToTray;
            RestoreServers = settings.RestoreRunningServers;
            NewsAutoScroll = settings.NewsAutoScroll;
            CurseForgeApiKey = settings.CurseForgeApiKey ?? string.Empty;
            ModIoApiKey = settings.ModIoApiKey ?? string.Empty;
            ModIoApiHost = settings.ModIoApiHost ?? string.Empty;
        }
        finally
        {
            _isLoadingSettings = false;
        }
    }

    private async void LoadSpecs()
    {
        try
        {
            var specs = await Task.Run(() => _services.Metrics.Specs());
            SpecCpu = specs.Processor;
            SpecRam = specs.MemoryGb;
            SpecGpu = specs.Graphics;
            SpecOs = specs.OperatingSystem;
        }
        catch
        {
            // Best-effort: the spec tiles keep their "—" placeholders.
        }
    }

    private void LoadDrives()
    {
        _drives.Clear();
        _drives.AddRange(_services.Metrics.Drives());
        var serversRoot = _services.Paths.ServersDir;

        var currentIndex = _drives.FindIndex(d => serversRoot.StartsWith(d.Root, StringComparison.OrdinalIgnoreCase));
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }

        DriveLabels.Clear();
        foreach (var drive in _drives)
        {
            DriveLabels.Add($"{drive.Name} ({FormatGb(drive.TotalBytes)})");
        }

        _loadingDrives = true;
        SelectedDriveIndex = _drives.Count == 0 ? -1 : currentIndex;
        _loadingDrives = false;

        InstallPathText = $"Current path: {serversRoot}";
    }

    private static readonly TimeSpan GameFilesSizeCacheDuration = TimeSpan.FromMinutes(5);

    private long _cachedGameFilesBytes = -1;
    private string? _cachedGameFilesRoot;
    private DateTime _cachedGameFilesAt;
    private bool _sizingGameFiles;
    private bool _refreshingLive;

    private async void RefreshStorage()
    {
        try
        {
            var serversRoot = _services.Paths.ServersDir;
            var drive = await Task.Run(() => _services.Metrics.Drives()
                .FirstOrDefault(d => serversRoot.StartsWith(d.Root, StringComparison.OrdinalIgnoreCase)));

            if (drive is not null)
            {
                DiskUsageText = $"{FormatGb(drive.UsedBytes)} of {FormatGb(drive.TotalBytes)}";
                DiskUsageFraction = drive.UsedPercent;
                DiskUsageCaption = $"{FormatGb(drive.UsedBytes)} used";
            }
            else
            {
                DiskUsageText = "0.0 GB";
                DiskUsageFraction = 0;
                DiskUsageCaption = string.Empty;
            }

            var count = _services.Servers.Servers.Count;
            GameFilesCaption = $"{count} server{(count == 1 ? "" : "s")} installed";

            var cacheValid = _cachedGameFilesBytes >= 0
                && string.Equals(_cachedGameFilesRoot, serversRoot, StringComparison.OrdinalIgnoreCase)
                && DateTime.UtcNow - _cachedGameFilesAt < GameFilesSizeCacheDuration;
            if (!cacheValid && !_sizingGameFiles)
            {
                _sizingGameFiles = true;
                try
                {
                    if (_cachedGameFilesBytes < 0)
                    {
                        GameFilesText = "Calculating…";
                    }

                    _cachedGameFilesBytes = await Task.Run(() => SystemMetricsService.DirectorySizeBytes(serversRoot));
                    _cachedGameFilesRoot = serversRoot;
                    _cachedGameFilesAt = DateTime.UtcNow;
                }
                finally
                {
                    _sizingGameFiles = false;
                }
            }

            var gameFilesBytes = Math.Max(0, _cachedGameFilesBytes);
            GameFilesText = FormatGb(gameFilesBytes);
            GameFilesFraction = drive is { TotalBytes: > 0 }
                ? (double)gameFilesBytes / drive.TotalBytes * 100.0
                : 0;
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't read storage usage: {ex.Message}");
        }
    }

    public async void RefreshLive()
    {
        if (_refreshingLive)
        {
            return;
        }

        _refreshingLive = true;
        try
        {
            var (cpu, ram) = await Task.Run(() => (_services.Metrics.CpuPercent(), _services.Metrics.Ram()));
            CpuText = $"{cpu:0.0}%";
            CpuFraction = cpu;

            var (usedGb, totalBytes, freeBytes) = ram;
            RamText = $"{usedGb:0.0} GB";
            RamFraction = totalBytes > 0 ? (double)(totalBytes - freeBytes) / totalBytes * 100.0 : 0;

            ActiveServersText = _services.Processes.RunningCount.ToString();
        }
        catch
        {
            // Best-effort; metrics can transiently fail (e.g. WMI hiccup) without crashing the UI.
        }
        finally
        {
            _refreshingLive = false;
        }
    }

    private void UpdateSettings(Action<Core.Models.AppSettings> mutate)
    {
        try
        {
            _services.Settings.Update(mutate);
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't save settings: {ex.Message}");
        }
    }

    public void RefreshRules()
    {
        var rules = _services.Settings.Settings.FirewallRules;
        Rules.Clear();
        foreach (var rule in rules)
        {
            Rules.Add(new FirewallRuleViewModel { Rule = rule });
        }

        NoRules = Rules.Count == 0;
    }

    partial void OnStartOnBootChanged(bool value)
    {
        if (_isLoadingSettings)
        {
            return;
        }

        UpdateSettings(s =>
        {
            s.StartOnBoot = value;
            if (!value)
            {
                s.StartMinimizedToTray = false;
            }
        });
        _toasts.Info(value ? "Panel will launch at boot" : "Boot launch disabled");
        LoadSettings();
    }

    partial void OnStartMinimizedChanged(bool value)
    {
        if (!_isLoadingSettings)
        {
            UpdateSettings(s => s.StartMinimizedToTray = value);
        }
    }

    partial void OnRestoreServersChanged(bool value)
    {
        if (!_isLoadingSettings)
        {
            UpdateSettings(s => s.RestoreRunningServers = value);
        }
    }

    partial void OnNewsAutoScrollChanged(bool value)
    {
        if (!_isLoadingSettings)
        {
            UpdateSettings(s => s.NewsAutoScroll = value);
        }
    }

    partial void OnCurseForgeApiKeyChanged(string value)
    {
        if (!_isLoadingSettings)
        {
            UpdateSettings(s => s.CurseForgeApiKey = string.IsNullOrWhiteSpace(value) ? null : value.Trim());
        }
    }

    partial void OnModIoApiKeyChanged(string value)
    {
        if (!_isLoadingSettings)
        {
            UpdateSettings(s => s.ModIoApiKey = string.IsNullOrWhiteSpace(value) ? null : value.Trim());
        }
    }

    partial void OnModIoApiHostChanged(string value)
    {
        if (!_isLoadingSettings)
        {
            UpdateSettings(s => s.ModIoApiHost = string.IsNullOrWhiteSpace(value) ? null : value.Trim());
        }
    }

    [RelayCommand]
    private void OpenCurseForgeConsole()
    {
        if (!Controls.ArtworkImages.OpenInBrowser("https://console.curseforge.com/"))
        {
            _toasts.Error("Couldn't open the CurseForge console in your browser.");
        }
    }

    [RelayCommand]
    private void OpenModIoAccess()
    {
        if (!Controls.ArtworkImages.OpenInBrowser("https://mod.io/me/access"))
        {
            _toasts.Error("Couldn't open mod.io in your browser.");
        }
    }

    partial void OnSelectedDriveIndexChanged(int value)
    {
        if (_loadingDrives || value < 0 || value >= _drives.Count)
        {
            return;
        }

        var drive = _drives[value];
        var newRoot = Path.Combine(drive.Root, "MPServerProvider", "servers");
        if (AppPaths.SamePath(newRoot, _services.Paths.ServersDir))
        {
            return;
        }

        if (_services.Processes.RunningCount > 0)
        {
            _toasts.Warning("Stop all running servers before changing the installation drive.");
            RevertDriveSelection();
            return;
        }

        try
        {
            // Existing servers stay where they are (and stay listed — AppPaths remembers every
            // install location); only new installs go to the new drive.
            _services.Settings.SetInstallRoot(newRoot);
            _services.Servers.Reload();
            _toasts.Info($"New servers will install to {newRoot}. Existing servers stay where they are.");
            RefreshStorage();
            LoadDrives();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Cannot use that drive: {ex.Message}");
            RevertDriveSelection();
        }
    }

    private void RevertDriveSelection()
    {
        // Deferred: changing the bound index from inside its own change handler is unreliable.
        System.Windows.Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            try
            {
                LoadDrives();
            }
            catch
            {
                // Best-effort.
            }
        });
    }

    [RelayCommand]
    private async Task DeleteRuleAsync(FirewallRuleViewModel? ruleVm)
    {
        if (ruleVm is null)
        {
            return;
        }

        try
        {
            await _services.Firewall.DeleteRuleAsync(ruleVm.Rule);
            UpdateSettings(s => s.FirewallRules.RemoveAll(r => r.Name == ruleVm.Rule.Name));
            _toasts.Info($"Removed firewall rule '{ruleVm.Rule.Name}'.");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't delete firewall rule: {ex.Message}");
        }
        finally
        {
            RefreshRules();
        }
    }

    private static string FormatGb(long bytes) => $"{bytes / (1024.0 * 1024.0 * 1024.0):0.0} GB";
}
