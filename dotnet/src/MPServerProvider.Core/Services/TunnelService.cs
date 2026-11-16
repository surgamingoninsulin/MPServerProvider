using System.Net;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public sealed record UpnpResult(bool Success, string Message, string? ExternalIp)
{
    public static UpnpResult Ok(string message, string? externalIp) => new(true, message, externalIp);

    public static UpnpResult Failure(string message) => new(false, message, null);
}

public enum PortProtocol
{
    Tcp,
    Udp,
}

public readonly record struct PortMapping(int ExternalPort, int InternalPort, PortProtocol Protocol)
{
    public string SoapProtocol => Protocol == PortProtocol.Tcp ? "TCP" : "UDP";
}

public sealed record UpnpDevice(string ControlUrl, string ServiceType, IPAddress LocalIp)
{
    public string? Location { get; init; }

    public string? FriendlyName { get; init; }
}

public enum TunnelNoticeLevel
{
    Info,
    Success,
    Warning,
    Error,
}

public sealed record TunnelNotice(TunnelNoticeLevel Level, string Message);

public sealed partial class TunnelService : IDisposable
{
    public const string PlayitReleasesApi = "https://api.github.com/repos/playit-cloud/playit-agent/releases/latest";

    private const string SsdpMulticastAddress = "239.255.255.250";
    private const int SsdpMulticastPort = 1900;

    public static readonly TimeSpan DiscoveryTimeout = TimeSpan.FromSeconds(2.5);

    public const int DiscoveryAttempts = 2;

    private readonly HttpClient _http;
    private readonly string? _stateDir;
    private readonly string? _toolsDir;
    private readonly object _logLock = new();

    public TunnelService()
        : this(null, null)
    {
    }

    public TunnelService(string? stateDir, string? toolsDir)
    {
        _stateDir = stateDir;
        _toolsDir = toolsDir;
        _http = new HttpClient(new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(5),
            UseProxy = false, // routers are on the LAN; a system proxy would only get in the way
        })
        {
            Timeout = TimeSpan.FromSeconds(30),
        };
        _githubHttp = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
        _githubHttp.DefaultRequestHeaders.UserAgent.ParseAdd("MPServerProvider/1.0");

        if (_stateDir is not null)
        {
            try
            {
                Directory.CreateDirectory(_stateDir);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Persistence is best-effort.
            }
        }
        LoadOwned();
        RefreshPlayitInstallState();
    }

    public event Action<TunnelNotice>? Notice;

    public interface IToolPaths
    {
        string ToolsDir { get; }
    }

    // -- Automation ---------------------------------------------------------------------------

    private readonly HashSet<string> _playitAutoServers = new(StringComparer.OrdinalIgnoreCase);
    private bool _playitStartedByAutomation;

    public void AttachAutomation(ServerProcessService processes, Func<ServerInstance, string?> gameProtocolOf, ServerRepository? repository = null)
    {
        if (repository is not null)
        {
            // A deleted server's forwards must not outlive it.
            repository.Deleted += server =>
            {
                if (server is not null && MappingsFor(server.ServerName).Count > 0)
                {
                    _ = Task.Run(() => AutoRemoveAsync(server.ServerName));
                }
            };
        }

        processes.StateChanged += (server, state) =>
        {
            try
            {
                OnServerStateChanged(processes, gameProtocolOf, server, state);
            }
            catch (Exception ex)
            {
                Log($"Tunnel automation failed for {server?.ServerName}: {ex}");
            }
        };
    }

    private void OnServerStateChanged(ServerProcessService processes, Func<ServerInstance, string?> gameProtocolOf, ServerInstance server, ServerState state)
    {
        var spec = server.Tunnel ?? new TunnelSpec();
        var mode = spec.NormalizedMode;

        if (state == ServerState.Running && spec.AutoStart)
        {
            if (mode == TunnelSpec.ModeUpnp)
            {
                _ = Task.Run(() => AutoForwardAsync(processes, gameProtocolOf, server));
            }
            else if (mode == TunnelSpec.ModePlayit)
            {
                _ = Task.Run(() => AutoStartPlayitAsync(server));
            }
            return;
        }

        if (state is ServerState.Stopped or ServerState.Crashed)
        {
            // Only tear down if the server really is down: a crash reports Crashed before its
            // automatic restart, and the forwards should survive that short gap.
            if (processes.StateOf(server) is ServerState.Running or ServerState.Starting
                || processes.IsAutoRestartPending(server))
            {
                return;
            }

            if (MappingsFor(server.ServerName).Count > 0)
            {
                _ = Task.Run(() => AutoRemoveAsync(server.ServerName));
            }

            bool stopAgent;
            lock (_playitAutoServers)
            {
                stopAgent = _playitAutoServers.Remove(server.ServerName)
                            && _playitAutoServers.Count == 0
                            && _playitStartedByAutomation;
            }
            if (stopAgent)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await StopPlayitAgentAsync().ConfigureAwait(false);
                        Raise(TunnelNoticeLevel.Info, "playit.gg agent stopped (no running server needs it).");
                    }
                    catch (Exception ex)
                    {
                        Log($"Stopping playit after {server.ServerName} stopped failed: {ex.Message}");
                    }
                });
            }
        }
    }

    private async Task AutoForwardAsync(ServerProcessService processes, Func<ServerInstance, string?> gameProtocolOf, ServerInstance server)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            string? protocol = null;
            try
            {
                protocol = gameProtocolOf(server);
            }
            catch (Exception ex)
            {
                Log($"Looking up the network protocol for {server.ServerName} failed: {ex.Message}");
            }

            var result = await ForwardServerPortsAsync(server, protocol, cts.Token).ConfigureAwait(false);

            // The server may have stopped while the router was busy: don't leave ports open.
            if (processes.StateOf(server) is not (ServerState.Running or ServerState.Starting))
            {
                await RemoveServerPortsAsync(server.ServerName, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            if (result.Success)
            {
                var address = result.ExternalIp is { Length: > 0 } ip && result.ExternalIpKind == ExternalIpKind.Public
                    ? $" Players join at {ip}:{server.ServerPort}."
                    : string.Empty;
                Raise(TunnelNoticeLevel.Success, $"UPnP: forwarded ports for {server.ServerName}.{address}");
                if (result.Warning is { } warning)
                {
                    Raise(TunnelNoticeLevel.Warning, warning);
                }
            }
            else
            {
                Raise(TunnelNoticeLevel.Warning, $"UPnP for {server.ServerName}: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            Log($"Auto UPnP for {server.ServerName} failed: {ex}");
            Raise(TunnelNoticeLevel.Warning, $"UPnP for {server.ServerName} failed: {ex.Message}");
        }
    }

    private async Task AutoRemoveAsync(string serverName)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var summary = await RemoveServerPortsAsync(serverName, cts.Token).ConfigureAwait(false);
            Log(summary);
        }
        catch (Exception ex)
        {
            Log($"Auto UPnP removal for {serverName} failed: {ex.Message}");
        }
    }

    private async Task AutoStartPlayitAsync(ServerInstance server)
    {
        try
        {
            bool alreadyRunning = IsPlayitRunning;
            lock (_playitAutoServers)
            {
                _playitAutoServers.Add(server.ServerName);
                if (!alreadyRunning)
                {
                    _playitStartedByAutomation = true;
                }
            }
            if (alreadyRunning)
            {
                return;
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(6));
            await StartPlayitAgentAsync(installIfMissing: true, cts.Token).ConfigureAwait(false);
            Raise(TunnelNoticeLevel.Info, $"playit.gg agent started for {server.ServerName}.");
        }
        catch (Exception ex)
        {
            Log($"Auto playit for {server.ServerName} failed: {ex}");
            Raise(TunnelNoticeLevel.Warning, $"playit.gg for {server.ServerName}: {ex.Message}");
        }
    }

    public async Task CleanupStaleMappingsAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            await RemoveAllOwnedMappingsAsync(cts.Token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log($"Startup UPnP cleanup failed: {ex.Message}");
        }
    }

    public void Shutdown(TimeSpan timeout)
    {
        try
        {
            using var cts = new CancellationTokenSource(timeout);
            Task.Run(() => RemoveAllOwnedMappingsAsync(cts.Token)).Wait(timeout);
        }
        catch (Exception ex)
        {
            Log($"Removing UPnP mappings at exit failed: {ex.Message}");
        }
        StopPlayit();
    }

    public void Dispose()
    {
        StopPlayit();
        lock (_ownedLock)
        {
            foreach (var timer in _renewTimers.Values)
            {
                timer.Dispose();
            }
            _renewTimers.Clear();
        }
        _http.Dispose();
        _githubHttp.Dispose();
    }

    // -- Notices and log ----------------------------------------------------------------------

    private void Raise(TunnelNoticeLevel level, string message)
    {
        Log($"[{level}] {message}");
        try
        {
            Notice?.Invoke(new TunnelNotice(level, message));
        }
        catch (Exception ex)
        {
            Log($"Notice handler failed: {ex.Message}");
        }
    }

    internal void Log(string message)
    {
        System.Diagnostics.Debug.WriteLine($"[Tunnel] {message}");
        if (_stateDir is null)
        {
            return;
        }
        try
        {
            lock (_logLock)
            {
                var path = Path.Combine(_stateDir, "tunnel.log");
                var info = new FileInfo(path);
                if (info.Exists && info.Length > 1_000_000)
                {
                    File.Move(path, path + ".old", overwrite: true);
                }
                File.AppendAllText(path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
        }
        catch (Exception)
        {
            // Logging must never break tunnel work.
        }
    }
}
