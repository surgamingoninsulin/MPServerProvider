using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Native;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

/// <summary>
/// Port of <c>MPServerProvider.App.Views.TunnelDialog</c> (decompile token 0x0200001d), driving
/// <see cref="TunnelService"/>: UPnP forwarding of the selected server's ports (per the game's
/// TCP/UDP protocol plus extra ports), the playit.gg agent (install/start/stop, claim link,
/// tunnel addresses), and the per-server "enable when this server starts" preference stored in
/// <see cref="ServerInstance.Tunnel"/>. All service work is awaited off the UI thread; every
/// command catches its own exceptions, and service events are marshalled to the dispatcher.
/// </summary>
public sealed partial class TunnelDialogViewModel : ObservableObject, IDisposable
{
    private const int MaxOutputChars = 40_000;

    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private readonly Dispatcher _dispatcher;
    private bool _disposed;

    public TunnelDialogViewModel(AppServices services, ToastService toasts, ServerInstance? preselected)
    {
        _services = services;
        _toasts = toasts;
        _dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        Servers = new ObservableCollection<ServerInstance>(
            services.Servers.Servers.OrderBy(s => s.ServerName, StringComparer.OrdinalIgnoreCase));

        _services.Tunnel.PlayitStatusChanged += OnPlayitStatusChanged;
        _services.Tunnel.PlayitOutputReceived += OnPlayitOutput;

        PlayitOutput = string.Join(Environment.NewLine, _services.Tunnel.PlayitOutputSnapshot());
        ApplyPlayitStatus(_services.Tunnel.PlayitCurrentStatus);

        SelectedServer = preselected is not null
            ? Servers.FirstOrDefault(s => s.ServerName == preselected.ServerName)
            : Servers.FirstOrDefault();
    }

    public ObservableCollection<ServerInstance> Servers { get; }

    public ObservableCollection<PlayitTunnel> PlayitTunnels { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ForwardPortsCommand))]
    [NotifyCanExecuteChangedFor(nameof(RemoveMappingsCommand))]
    private ServerInstance? _selectedServer;

    [ObservableProperty]
    private string _serverSummary = string.Empty;

    [ObservableProperty]
    private string _plannedMappingsText = string.Empty;

    [ObservableProperty]
    private string _extraPortsText = string.Empty;

    [ObservableProperty]
    private string _extraPortsError = string.Empty;

    // -- UPnP state --

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ForwardPortsCommand))]
    [NotifyCanExecuteChangedFor(nameof(RemoveMappingsCommand))]
    private bool _upnpBusy;

    [ObservableProperty]
    private string _upnpStatusText = string.Empty;

    [ObservableProperty]
    private string _externalAddress = string.Empty;

    [ObservableProperty]
    private string _upnpWarning = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveMappingsCommand))]
    private string _activeMappingsText = string.Empty;

    [ObservableProperty]
    private bool _upnpAutoEnable;

    // -- playit state --

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(InstallPlayitCommand))]
    [NotifyCanExecuteChangedFor(nameof(StartPlayitCommand))]
    [NotifyCanExecuteChangedFor(nameof(StopPlayitCommand))]
    private PlayitState _playitState;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(InstallPlayitCommand))]
    [NotifyCanExecuteChangedFor(nameof(StartPlayitCommand))]
    [NotifyCanExecuteChangedFor(nameof(StopPlayitCommand))]
    private bool _playitBusy;

    [ObservableProperty]
    private string _playitStateText = string.Empty;

    [ObservableProperty]
    private string _playitMessage = string.Empty;

    [ObservableProperty]
    private string _playitAgentText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenClaimLinkCommand))]
    private string _claimUrl = string.Empty;

    [ObservableProperty]
    private string _dashboardUrl = TunnelService.PlayitDefaultDashboardUrl;

    [ObservableProperty]
    private string _playitOutput = string.Empty;

    [ObservableProperty]
    private string _playitPortHint = string.Empty;

    [ObservableProperty]
    private double _installProgress;

    [ObservableProperty]
    private bool _playitAutoEnable;

    public bool HasSelectedServer => SelectedServer is not null;

    public bool HasExternalAddress => ExternalAddress.Length > 0;

    public bool HasUpnpWarning => UpnpWarning.Length > 0;

    public bool HasActiveMappings => ActiveMappingsText.Length > 0;

    public bool HasExtraPortsError => ExtraPortsError.Length > 0;

    public bool HasClaimUrl => ClaimUrl.Length > 0;

    public bool HasPlayitTunnels => PlayitTunnels.Count > 0;

    public bool IsPlayitNotInstalled => PlayitState == PlayitState.NotInstalled;

    public bool IsInstalling => PlayitState == PlayitState.Installing;

    /// <summary>"ok", "warn", "error" or "idle", for the status dot color.</summary>
    public string PlayitStateKey => PlayitState switch
    {
        PlayitState.Connected => "ok",
        PlayitState.Starting or PlayitState.WaitingForClaim or PlayitState.Installing => "warn",
        PlayitState.Error => "error",
        _ => "idle",
    };

    partial void OnExternalAddressChanged(string value) => OnPropertyChanged(nameof(HasExternalAddress));

    partial void OnUpnpWarningChanged(string value) => OnPropertyChanged(nameof(HasUpnpWarning));

    partial void OnActiveMappingsTextChanged(string value) => OnPropertyChanged(nameof(HasActiveMappings));

    partial void OnExtraPortsErrorChanged(string value) => OnPropertyChanged(nameof(HasExtraPortsError));

    partial void OnClaimUrlChanged(string value) => OnPropertyChanged(nameof(HasClaimUrl));

    partial void OnPlayitStateChanged(PlayitState value)
    {
        OnPropertyChanged(nameof(PlayitStateKey));
        OnPropertyChanged(nameof(IsPlayitNotInstalled));
        OnPropertyChanged(nameof(IsInstalling));
    }

    partial void OnSelectedServerChanged(ServerInstance? value)
    {
        OnPropertyChanged(nameof(HasSelectedServer));
        OnPropertyChanged(nameof(CanTestReachability));
        ReachabilityText = string.Empty;
        LoadServer();
    }

    partial void OnExtraPortsTextChanged(string value)
    {
        if (_loading || SelectedServer is not { } server)
        {
            return;
        }
        var entries = value.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        var bad = entries.Where(e => !TunnelService.TryParseExtraPort(e, out _, out _, out _)).ToList();
        ExtraPortsError = bad.Count == 0
            ? string.Empty
            : $"Ignored: {string.Join(", ", bad)} (use e.g. 27015, 27015/udp, 8080/tcp or 27015-27017/udp).";
        server.Tunnel ??= new TunnelSpec();
        server.Tunnel.ExtraPorts = entries.Where(e => !bad.Contains(e)).ToList();
        SaveServer(server);
        RefreshPlannedMappings();
    }

    partial void OnUpnpAutoEnableChanged(bool value)
    {
        if (_loading || SelectedServer is not { } server)
        {
            return;
        }
        server.Tunnel ??= new TunnelSpec();
        if (value)
        {
            server.Tunnel.Mode = TunnelSpec.ModeUpnp;
            server.Tunnel.AutoStart = true;
        }
        else if (server.Tunnel.NormalizedMode == TunnelSpec.ModeUpnp)
        {
            server.Tunnel.Mode = TunnelSpec.ModeNone;
            server.Tunnel.AutoStart = false;
        }
        SaveServer(server);
        SyncAutoFlags(server);
    }

    partial void OnPlayitAutoEnableChanged(bool value)
    {
        if (_loading || SelectedServer is not { } server)
        {
            return;
        }
        server.Tunnel ??= new TunnelSpec();
        if (value)
        {
            server.Tunnel.Mode = TunnelSpec.ModePlayit;
            server.Tunnel.AutoStart = true;
        }
        else if (server.Tunnel.NormalizedMode == TunnelSpec.ModePlayit)
        {
            server.Tunnel.Mode = TunnelSpec.ModeNone;
            server.Tunnel.AutoStart = false;
        }
        SaveServer(server);
        SyncAutoFlags(server);
    }

    private bool _loading;

    private void LoadServer()
    {
        _loading = true;
        try
        {
            var server = SelectedServer;
            ExtraPortsText = server is null ? string.Empty : string.Join(", ", server.Tunnel?.ExtraPorts ?? new List<string>());
            ExtraPortsError = string.Empty;
            if (server is not null)
            {
                SyncAutoFlags(server);
            }
            else
            {
                UpnpAutoEnable = PlayitAutoEnable = false;
            }
            UpnpStatusText = server is null ? "Pick a server first." : string.Empty;
        }
        finally
        {
            _loading = false;
        }
        RefreshPlannedMappings();
        RefreshUpnpState();
    }

    private void SyncAutoFlags(ServerInstance server)
    {
        var wasLoading = _loading;
        _loading = true;
        try
        {
            var mode = server.Tunnel?.NormalizedMode ?? TunnelSpec.ModeNone;
            var auto = server.Tunnel?.AutoStart == true;
            UpnpAutoEnable = auto && mode == TunnelSpec.ModeUpnp;
            PlayitAutoEnable = auto && mode == TunnelSpec.ModePlayit;
        }
        finally
        {
            _loading = wasLoading;
        }
    }

    private string? GameProtocol(ServerInstance server)
    {
        try
        {
            return _services.Games.Find(server.ServerGame)?.Network?.Protocol;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void RefreshPlannedMappings()
    {
        if (SelectedServer is not { } server)
        {
            ServerSummary = PlannedMappingsText = PlayitPortHint = string.Empty;
            return;
        }
        var protocol = GameProtocol(server);
        var protocols = FirewallService.ProtocolsFor(protocol ?? "both");
        var protoText = string.Join(" + ", protocols);
        ServerSummary = $"Port {server.ServerPort} · {protoText}";
        var planned = TunnelService.BuildPortMappings(server.ServerPort, protocol, server.Tunnel?.ExtraPorts ?? new List<string>());
        PlannedMappingsText = planned.Count == 0
            ? "No valid port to forward."
            : "Will forward: " + string.Join(", ", planned.Select(p => $"{p.ExternalPort}/{p.SoapProtocol}"));
        var playitKind = protocols.Count > 1 ? "TCP+UDP" : protoText;
        PlayitPortHint = $"On playit.gg, add a {playitKind} tunnel to local address 127.0.0.1, port {server.ServerPort}.";
        RefreshPlayitPortCheck();
    }

    // -- playit tunnel ↔ server port check --

    [ObservableProperty]
    private string _playitMismatchText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UsePlayitPortCommand))]
    private int? _playitSuggestedPort;

    public bool HasPlayitMismatch => PlayitMismatchText.Length > 0;

    public string UsePlayitPortText => PlayitSuggestedPort is { } port ? $"Use port {port} for this server" : string.Empty;

    partial void OnPlayitMismatchTextChanged(string value) => OnPropertyChanged(nameof(HasPlayitMismatch));

    partial void OnPlayitSuggestedPortChanged(int? value) => OnPropertyChanged(nameof(UsePlayitPortText));

    private void RefreshPlayitPortCheck()
    {
        if (SelectedServer is not { } server || PlayitTunnels.Count == 0)
        {
            PlayitMismatchText = string.Empty;
            PlayitSuggestedPort = null;
            return;
        }

        var check = PlayitPortCheck.For(PlayitTunnels, server.ServerPort);
        if (check.Matching.Count > 0 || check.OtherPorts.Count == 0)
        {
            PlayitMismatchText = string.Empty;
            PlayitSuggestedPort = null;
            return;
        }

        var ports = string.Join(", ", check.OtherPorts.Select(t => t.DestinationPort).Distinct());
        PlayitMismatchText =
            $"No playit tunnel points at this server: your tunnels forward to local port {ports}, but {server.ServerName} " +
            $"listens on port {server.ServerPort}, so players (and status sites like mcstatus.io) see it as offline. " +
            (check.SuggestedPort is { } suggested
                ? $"Switch this server to port {suggested}, or change the tunnel's local port to {server.ServerPort} on playit.gg."
                : $"Change the tunnel's local port to {server.ServerPort} on playit.gg.");
        PlayitSuggestedPort = check.SuggestedPort;
    }

    private bool CanUsePlayitPort() => PlayitSuggestedPort is not null && SelectedServer is not null;

    /// <summary>Moves the selected server onto the port its playit tunnel forwards to (restarting
    /// it if it is running, so Minecraft's server.properties picks the new port up).</summary>
    [RelayCommand(CanExecute = nameof(CanUsePlayitPort))]
    private async Task UsePlayitPort()
    {
        if (SelectedServer is not { } server || PlayitSuggestedPort is not { } port)
        {
            return;
        }

        try
        {
            var others = _services.Servers.Servers.Where(s => s.ServerName != server.ServerName && s.ServerPort == port).ToList();
            if (others.Count > 0)
            {
                _toasts.Warning($"Port {port} is also set for '{others[0].ServerName}'. Don't run both at the same time.");
            }

            server.ServerPort = port;
            SaveServer(server);

            var state = _services.Processes.StateOf(server);
            if (state is ServerState.Running or ServerState.Starting)
            {
                _toasts.Info($"Restarting {server.ServerName} on port {port}…");
                await _services.Processes.RestartAsync(server);
            }
            else
            {
                _toasts.Success($"{server.ServerName} now uses port {port}. Start it and your playit address will reach it.");
            }

            RefreshPlannedMappings();
        }
        catch (Exception ex)
        {
            _toasts.Error($"Couldn't change the port: {ex.Message}");
        }
    }

    private void RefreshUpnpState()
    {
        if (SelectedServer is not { } server)
        {
            ActiveMappingsText = ExternalAddress = UpnpWarning = string.Empty;
            return;
        }
        var mappings = _services.Tunnel.MappingsFor(server.ServerName);
        ActiveMappingsText = mappings.Count == 0
            ? string.Empty
            : "Forwarded by this app: " + string.Join(", ", mappings.Select(m => m.ToString()))
              + (mappings.Any(m => m.LeaseSeconds > 0) ? " (temporary lease, renewed while the server runs)" : string.Empty);
        var (ip, kind) = _services.Tunnel.ExternalIpFor(server.ServerName);
        ExternalAddress = mappings.Count > 0 && !string.IsNullOrEmpty(ip) ? $"{ip}:{server.ServerPort}" : string.Empty;
        UpnpWarning = mappings.Count > 0 ? TunnelService.ExternalIpWarning(kind, ip) ?? string.Empty : string.Empty;

        // Most home routers (including Ziggo/Compal Connect Boxes) don't support NAT loopback:
        // the public address fails from inside the house even when it works for everyone else.
        var lan = mappings.Select(m => m.InternalClient).FirstOrDefault(c => !string.IsNullOrEmpty(c));
        LocalJoinText = $"Joining from this PC? Use localhost:{server.ServerPort}"
            + (lan is null ? "." : $", or {lan}:{server.ServerPort} from another device on your network.")
            + " Your own public address often can't be reached from inside your network (routers without NAT loopback), "
            + "so use “Test from internet” or mobile data to check it.";
    }

    [ObservableProperty]
    private string _localJoinText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasReachabilityText))]
    private string _reachabilityText = string.Empty;

    [ObservableProperty]
    private bool _reachabilityOnline;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(TestReachabilityCommand))]
    private bool _isTestingReachability;

    public bool HasReachabilityText => ReachabilityText.Length > 0;

    /// <summary>The internet check uses mcstatus.io, which only knows Minecraft.</summary>
    public bool CanTestReachability => ReachabilityCheck.IsSupported(SelectedServer?.ServerGame);

    [RelayCommand(CanExecute = nameof(CanRunReachabilityTest))]
    private async Task TestReachability(string? address)
    {
        if (SelectedServer is not { } server || string.IsNullOrWhiteSpace(address))
        {
            return;
        }

        IsTestingReachability = true;
        ReachabilityOnline = false;
        ReachabilityText = $"Checking {address} from the internet…";
        try
        {
            var result = await ReachabilityCheck.CheckAsync(server.ServerGame, address);
            ReachabilityOnline = result.Online;
            ReachabilityText = result.Online
                ? result.Text
                : result.Text + (_services.Processes.StateOf(server) is ServerState.Running ? string.Empty : " (The server isn't running right now.)");
        }
        catch (Exception ex)
        {
            ReachabilityText = $"Couldn't reach mcstatus.io to test the address: {ex.Message}";
        }
        finally
        {
            IsTestingReachability = false;
        }
    }

    private bool CanRunReachabilityTest(string? address) => !IsTestingReachability;

    private void SaveServer(ServerInstance server)
    {
        try
        {
            _services.Servers.Save(server);
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not save tunnel settings: {ex.Message}");
        }
    }

    // -- UPnP commands --

    private bool CanForward() => !UpnpBusy && SelectedServer is not null;

    [RelayCommand(CanExecute = nameof(CanForward))]
    private async Task ForwardPorts()
    {
        if (SelectedServer is not { } server)
        {
            return;
        }

        UpnpBusy = true;
        UpnpStatusText = "Searching for your router and forwarding ports…";
        try
        {
            var protocol = GameProtocol(server);
            var result = await Task.Run(() => _services.Tunnel.ForwardServerPortsAsync(server, protocol));
            UpnpStatusText = result.Message;
            RefreshUpnpState();
            if (result.Success)
            {
                _toasts.Success($"Ports forwarded for {server.ServerName}.");
                if (result.Warning is { } warning)
                {
                    _toasts.Warning("Port forwarding may not be reachable from the internet; see the dialog.");
                    UpnpWarning = warning;
                }
            }
            else
            {
                _toasts.Error("UPnP port forwarding failed.");
            }
        }
        catch (Exception ex)
        {
            UpnpStatusText = $"UPnP failed: {ex.Message}";
            _toasts.Error($"UPnP port forward failed: {ex.Message}");
        }
        finally
        {
            UpnpBusy = false;
        }
    }

    private bool CanRemove() => !UpnpBusy && SelectedServer is not null && HasActiveMappings;

    [RelayCommand(CanExecute = nameof(CanRemove))]
    private async Task RemoveMappings()
    {
        if (SelectedServer is not { } server)
        {
            return;
        }

        UpnpBusy = true;
        UpnpStatusText = "Removing port mappings…";
        try
        {
            var summary = await Task.Run(() => _services.Tunnel.RemoveServerPortsAsync(server.ServerName));
            UpnpStatusText = summary;
            _toasts.Info("Port mappings removed.");
        }
        catch (Exception ex)
        {
            UpnpStatusText = $"Could not remove mappings: {ex.Message}";
            _toasts.Error($"Could not remove mappings: {ex.Message}");
        }
        finally
        {
            RefreshUpnpState();
            UpnpBusy = false;
        }
    }

    // -- playit commands --

    private bool CanInstall() => !PlayitBusy && PlayitState == PlayitState.NotInstalled;

    [RelayCommand(CanExecute = nameof(CanInstall))]
    private async Task InstallPlayit()
    {
        PlayitBusy = true;
        InstallProgress = 0;
        try
        {
            var progress = new Progress<double>(p => InstallProgress = p * 100);
            await Task.Run(() => _services.Tunnel.InstallPlayitAsync(progress));
            _toasts.Success("playit.gg agent installed.");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not install the playit.gg agent: {ex.Message}");
        }
        finally
        {
            PlayitBusy = false;
        }
    }

    private bool CanStart() => !PlayitBusy && PlayitState is PlayitState.Stopped or PlayitState.Error or PlayitState.NotInstalled;

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartPlayit()
    {
        PlayitBusy = true;
        try
        {
            await Task.Run(() => _services.Tunnel.StartPlayitAgentAsync(installIfMissing: true));
        }
        catch (Exception ex)
        {
            PlayitMessage = ex.Message;
            _toasts.Error($"Could not start the playit.gg agent: {ex.Message}");
        }
        finally
        {
            PlayitBusy = false;
        }
    }

    private bool CanStop() => !PlayitBusy && PlayitState is PlayitState.Starting or PlayitState.WaitingForClaim or PlayitState.Connected
                              || (!PlayitBusy && PlayitState == PlayitState.Error && _services.Tunnel.IsPlayitRunning);

    [RelayCommand(CanExecute = nameof(CanStop))]
    private async Task StopPlayit()
    {
        PlayitBusy = true;
        try
        {
            await Task.Run(() => _services.Tunnel.StopPlayitAgentAsync());
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not stop the playit.gg agent: {ex.Message}");
        }
        finally
        {
            PlayitBusy = false;
        }
    }

    private bool CanOpenClaim() => HasClaimUrl;

    [RelayCommand(CanExecute = nameof(CanOpenClaim))]
    private void OpenClaimLink() => OpenPlayitUrl(ClaimUrl);

    [RelayCommand]
    private void OpenDashboard() => OpenPlayitUrl(DashboardUrl);

    /// <summary>
    /// Opens a playit.gg link in the default browser, only if it's https on playit.gg. When the
    /// app is elevated the browser is started through Explorer with the user's normal token.
    /// </summary>
    private void OpenPlayitUrl(string url)
    {
        if (!PlayitOutputParser.IsTrustedPlayitUrl(url))
        {
            _toasts.Warning("That link isn't an https://playit.gg address, so it wasn't opened.");
            return;
        }
        var safe = new Uri(url).AbsoluteUri;
        try
        {
            if (OperatingSystem.IsWindows() && DeElevatedLauncher.IsCurrentProcessElevated())
            {
                var explorer = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
                DeElevatedLauncher.Launch($"\"{explorer}\" \"{safe}\"");
            }
            else
            {
                Process.Start(new ProcessStartInfo(safe) { UseShellExecute = true })?.Dispose();
            }
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not open the browser ({ex.Message}). Copy the link instead.");
        }
    }

    // -- Service events --

    private void OnPlayitStatusChanged(PlayitStatus status)
    {
        if (_disposed)
        {
            return;
        }
        _dispatcher.BeginInvoke(() =>
        {
            if (!_disposed)
            {
                ApplyPlayitStatus(status);
            }
        });
    }

    private void OnPlayitOutput(string line)
    {
        if (_disposed)
        {
            return;
        }
        _dispatcher.BeginInvoke(() =>
        {
            if (_disposed)
            {
                return;
            }
            var text = PlayitOutput.Length == 0 ? line : PlayitOutput + Environment.NewLine + line;
            if (text.Length > MaxOutputChars)
            {
                text = text[^MaxOutputChars..];
            }
            PlayitOutput = text;
        });
    }

    private void ApplyPlayitStatus(PlayitStatus status)
    {
        PlayitState = status.State;
        PlayitStateText = status.State switch
        {
            PlayitState.NotInstalled => "Not installed",
            PlayitState.Stopped => "Stopped",
            PlayitState.Installing => "Installing…",
            PlayitState.Starting => "Starting…",
            PlayitState.WaitingForClaim => "Waiting for claim",
            PlayitState.Connected => status.UsingInstalledService ? "Connected (playit service)" : "Connected",
            PlayitState.Error => "Error",
            _ => status.State.ToString(),
        };
        PlayitMessage = status.Message;
        PlayitAgentText = status.AgentPath is { } path ? $"Agent: {path}" : "Agent: not installed";
        ClaimUrl = status.ClaimUrl ?? string.Empty;
        DashboardUrl = status.DashboardUrl;

        if (!PlayitTunnels.SequenceEqual(status.Tunnels))
        {
            PlayitTunnels.Clear();
            foreach (var tunnel in status.Tunnels)
            {
                PlayitTunnels.Add(tunnel);
            }
            OnPropertyChanged(nameof(HasPlayitTunnels));
            RefreshPlayitPortCheck();
        }
        StopPlayitCommand.NotifyCanExecuteChanged();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _services.Tunnel.PlayitStatusChanged -= OnPlayitStatusChanged;
        _services.Tunnel.PlayitOutputReceived -= OnPlayitOutput;
    }
}
