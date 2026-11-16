using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public sealed class UpnpSoapException : Exception
{
    public UpnpSoapException(string action, int? errorCode, string? errorDescription, int httpStatus)
        : base($"{action} failed: " + (errorCode is { } code ? $"UPnP error {code} ({errorDescription})" : $"HTTP {httpStatus}"))
    {
        Action = action;
        ErrorCode = errorCode;
        ErrorDescription = errorDescription;
        HttpStatus = httpStatus;
    }

    public string Action { get; }

    public int? ErrorCode { get; }

    public string? ErrorDescription { get; }

    public int HttpStatus { get; }
}

public enum ExternalIpKind
{
    Public,

    Private,

    CarrierGradeNat,

    Unknown,
}

public sealed record LocalInterfaceInfo(
    string Name,
    string Description,
    NetworkInterfaceType Type,
    IPAddress Address,
    IPAddress Mask,
    IReadOnlyList<IPAddress> Gateways);

public sealed class OwnedPortMapping
{
    public string ServerName { get; set; } = string.Empty;

    public int ExternalPort { get; set; }

    public int InternalPort { get; set; }

    public string Protocol { get; set; } = "TCP";

    public string InternalClient { get; set; } = string.Empty;

    public string ControlUrl { get; set; } = string.Empty;

    public string ServiceType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int LeaseSeconds { get; set; }

    public DateTimeOffset CreatedUtc { get; set; }

    public override string ToString() => $"{ExternalPort}/{Protocol}";
}

public sealed record UpnpForwardResult(
    bool Success,
    string Message,
    string? ExternalIp,
    ExternalIpKind ExternalIpKind,
    string? Warning,
    IReadOnlyList<OwnedPortMapping> Mappings,
    IReadOnlyList<string> Failures);

public sealed partial class TunnelService
{
    public const string MappingDescriptionPrefix = "MPServerProvider";

    public const int FallbackLeaseSeconds = 3600;

    public static readonly TimeSpan DeviceCacheLifetime = TimeSpan.FromMinutes(10);

    public static readonly TimeSpan SoapTimeout = TimeSpan.FromSeconds(8);

    private static readonly string[] SsdpSearchTargets =
    {
        "urn:schemas-upnp-org:device:InternetGatewayDevice:1",
        "urn:schemas-upnp-org:device:InternetGatewayDevice:2",
        "urn:schemas-upnp-org:service:WANIPConnection:1",
        "urn:schemas-upnp-org:service:WANIPConnection:2",
        "urn:schemas-upnp-org:service:WANPPPConnection:1",
    };

    internal static readonly string[] SupportedServiceTypes =
    {
        "urn:schemas-upnp-org:service:WANIPConnection:2",
        "urn:schemas-upnp-org:service:WANIPConnection:1",
        "urn:schemas-upnp-org:service:WANPPPConnection:1",
    };

    private static readonly string[] VirtualAdapterMarkers =
    {
        "virtual", "vmware", "virtualbox", "hyper-v", "vethernet", "loopback", "tap-", "tap adapter",
        "wireguard", "zerotier", "hamachi", "tailscale", "openvpn", "wintun", "npcap", "docker", "wsl",
        "bluetooth", "teredo", "isatap", "pseudo",
    };

    private readonly SemaphoreSlim _upnpLock = new(1, 1);
    private readonly object _ownedLock = new();
    private readonly List<OwnedPortMapping> _owned = new();
    private readonly Dictionary<string, Timer> _renewTimers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, (string? Ip, ExternalIpKind Kind)> _externalIps = new(StringComparer.OrdinalIgnoreCase);
    private IReadOnlyList<UpnpDevice>? _cachedDevices;
    private DateTimeOffset _cachedDevicesAt;

    private string? OwnedMappingsFile => _stateDir is null ? null : Path.Combine(_stateDir, "upnp-mappings.json");

    // -- Public UPnP API ----------------------------------------------------------------------

    public IReadOnlyList<OwnedPortMapping> MappingsFor(string serverName)
    {
        lock (_ownedLock)
        {
            return _owned.Where(m => string.Equals(m.ServerName, serverName, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    public (string? Ip, ExternalIpKind Kind) ExternalIpFor(string serverName)
    {
        lock (_ownedLock)
        {
            return _externalIps.TryGetValue(serverName, out var v) ? v : (null, ExternalIpKind.Unknown);
        }
    }

    public async Task<UpnpForwardResult> ForwardServerPortsAsync(ServerInstance server, string? gameProtocol, CancellationToken ct = default)
    {
        var wanted = BuildPortMappings(server.ServerPort, gameProtocol, server.Tunnel?.ExtraPorts ?? new List<string>());
        if (wanted.Count == 0)
        {
            return Fail($"{server.ServerName} has no valid port to forward.");
        }

        var description = MappingDescription(server.ServerName);

        await _upnpLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            IReadOnlyList<UpnpDevice> devices;
            try
            {
                devices = await GetDevicesAsync(forceRefresh: false, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log($"UPnP discovery failed: {ex.Message}");
                return Fail(ex is UpnpDiscoveryException
                    ? ex.Message
                    : $"Could not search for a UPnP router: {ex.Message}");
            }

            string? lastDeviceError = null;
            for (var pass = 0; pass < 2; pass++)
            {
                foreach (var device in devices)
                {
                    var outcome = await TryForwardOnDeviceAsync(device, server.ServerName, wanted, description, ct).ConfigureAwait(false);
                    if (outcome.Created.Count > 0 || outcome.AlreadyOurs.Count > 0)
                    {
                        return await FinishForwardAsync(server.ServerName, device, outcome, ct).ConfigureAwait(false);
                    }
                    lastDeviceError = outcome.Failures.FirstOrDefault() ?? lastDeviceError;
                    if (outcome.Fatal)
                    {
                        return new UpnpForwardResult(false, lastDeviceError ?? "Port forwarding failed.", null,
                            ExternalIpKind.Unknown, null, Array.Empty<OwnedPortMapping>(), outcome.Failures);
                    }
                }

                if (pass == 0)
                {
                    // The cached router list may be stale (router rebooted, different network):
                    // search once more before giving up.
                    try
                    {
                        devices = await GetDevicesAsync(forceRefresh: true, ct).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        lastDeviceError ??= ex.Message;
                        break;
                    }
                }
            }

            return Fail(lastDeviceError ?? "The router did not accept any port mapping.");
        }
        finally
        {
            _upnpLock.Release();
        }

        static UpnpForwardResult Fail(string message) =>
            new(false, message, null, ExternalIpKind.Unknown, null, Array.Empty<OwnedPortMapping>(), new[] { message });
    }

    public async Task<string> RemoveServerPortsAsync(string serverName, CancellationToken ct = default)
    {
        await _upnpLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            StopRenewal(serverName);
            var mine = MappingsFor(serverName);
            if (mine.Count == 0)
            {
                return "There are no port mappings from this app to remove.";
            }

            var removed = 0;
            var failures = new List<string>();
            foreach (var mapping in mine)
            {
                try
                {
                    await DeleteOwnedMappingAsync(mapping, ct).ConfigureAwait(false);
                    removed++;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    // The router is unreachable or refused: forget the record anyway. It was
                    // either already gone or will expire/need manual removal; retrying forever
                    // would only keep failing.
                    failures.Add($"{mapping}: {DescribeError(ex, mapping.ExternalPort, mapping.Protocol)}");
                    Log($"Removing {mapping} for {serverName} failed: {ex.Message}");
                }
                ForgetOwned(mapping);
            }

            lock (_ownedLock)
            {
                _externalIps.Remove(serverName);
            }
            SaveOwned();

            var summary = $"Removed {removed} of {mine.Count} port mapping(s) for {serverName}.";
            if (failures.Count > 0)
            {
                summary += " Some could not be removed from the router: " + string.Join("; ", failures);
            }
            Log(summary);
            return summary;
        }
        finally
        {
            _upnpLock.Release();
        }
    }

    public async Task RemoveAllOwnedMappingsAsync(CancellationToken ct = default)
    {
        List<string> servers;
        lock (_ownedLock)
        {
            servers = _owned.Select(m => m.ServerName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }
        foreach (var server in servers)
        {
            try
            {
                await RemoveServerPortsAsync(server, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                Log($"Cleaning up mappings for {server} failed: {ex.Message}");
            }
        }
    }

    // -- Discovery ----------------------------------------------------------------------------

    public sealed class UpnpDiscoveryException(string message) : Exception(message);

    public async Task<UpnpDevice> DiscoverRouterAsync(CancellationToken ct = default)
    {
        var devices = await DiscoverRoutersAsync(ct).ConfigureAwait(false);
        return devices[0];
    }

    public async Task<IReadOnlyList<UpnpDevice>> DiscoverRoutersAsync(CancellationToken ct = default)
    {
        var interfaces = GetLocalInterfaces();
        if (interfaces.Count == 0)
        {
            throw new UpnpDiscoveryException("This PC has no active network connection to search for a router on.");
        }

        var gateways = interfaces.SelectMany(i => i.Gateways).ToHashSet();
        if (gateways.Count == 0)
        {
            throw new UpnpDiscoveryException("This PC has no default gateway (router), so there is nothing to forward ports on.");
        }

        var responses = new List<(string Location, IPAddress LocalIp)>();
        var ignored = 0;
        for (var attempt = 0; attempt < DiscoveryAttempts && responses.Count == 0; attempt++)
        {
            var raw = await SsdpSearchAsync(interfaces, ct).ConfigureAwait(false);
            foreach (var r in raw)
            {
                // Only trust the router itself: the reply must come from a default gateway and
                // its LOCATION must point at that same address. Anything else on the LAN could
                // otherwise make the app fetch/POST to arbitrary URLs.
                if (IsAcceptableSsdpResponder(r.Source, r.Location, gateways))
                {
                    if (!responses.Any(x => x.Location == r.Location))
                    {
                        responses.Add((r.Location, r.LocalIp));
                    }
                }
                else
                {
                    ignored++;
                }
            }
        }

        if (responses.Count == 0)
        {
            Log($"SSDP: no reply from a default gateway ({ignored} other replies ignored).");
            throw new UpnpDiscoveryException(
                "Your router didn't answer the UPnP search. Make sure UPnP is enabled in its settings, or use the playit.gg tab instead (it works without router changes).");
        }

        var devices = new List<UpnpDevice>();
        var errors = new List<string>();
        foreach (var group in responses.GroupBy(r => r.Location, StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                var found = await FetchDevicesAsync(group.Key, group.First().LocalIp, interfaces, ct).ConfigureAwait(false);
                foreach (var device in found)
                {
                    if (!devices.Any(d => string.Equals(d.ControlUrl, device.ControlUrl, StringComparison.OrdinalIgnoreCase)
                                          && d.ServiceType == device.ServiceType))
                    {
                        devices.Add(device);
                    }
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                Log($"Reading UPnP device description {group.Key} failed: {ex.Message}");
            }
        }

        if (devices.Count == 0)
        {
            throw new UpnpDiscoveryException(
                "A UPnP device answered but it doesn't offer port forwarding (no WANIPConnection/WANPPPConnection service)." +
                (errors.Count > 0 ? $" ({errors[0]})" : string.Empty));
        }

        return devices
            .OrderByDescending(d => IsGatewayDevice(d, gateways))
            .ThenBy(d => Array.IndexOf(SupportedServiceTypes, d.ServiceType))
            .ToList();
    }

    private async Task<IReadOnlyList<UpnpDevice>> GetDevicesAsync(bool forceRefresh, CancellationToken ct)
    {
        if (!forceRefresh && _cachedDevices is { Count: > 0 } cached && DateTimeOffset.UtcNow - _cachedDevicesAt < DeviceCacheLifetime)
        {
            return cached;
        }
        var devices = await DiscoverRoutersAsync(ct).ConfigureAwait(false);
        _cachedDevices = devices;
        _cachedDevicesAt = DateTimeOffset.UtcNow;
        return devices;
    }

    private static bool IsGatewayDevice(UpnpDevice device, HashSet<IPAddress> gateways) =>
        Uri.TryCreate(device.ControlUrl, UriKind.Absolute, out var uri)
        && IPAddress.TryParse(uri.Host, out var host)
        && gateways.Contains(host);

    internal static bool IsAcceptableSsdpResponder(IPAddress source, string location, ISet<IPAddress> gateways)
    {
        if (source.IsIPv4MappedToIPv6)
        {
            source = source.MapToIPv4();
        }
        return gateways.Contains(source)
               && Uri.TryCreate(location, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
               && IPAddress.TryParse(uri.Host, out var host)
               && host.Equals(source);
    }

    private async Task<List<(string Location, IPAddress LocalIp, IPAddress Source)>> SsdpSearchAsync(IReadOnlyList<LocalInterfaceInfo> interfaces, CancellationToken ct)
    {
        var tasks = interfaces
            .Where(i => !IsVirtualAdapter(i))
            .DefaultIfEmpty()
            .Select(i => i is null ? SsdpSearchOnAsync(IPAddress.Any, ct) : SsdpSearchOnAsync(i.Address, ct))
            .ToList();
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);
        return results.SelectMany(r => r).ToList();
    }

    private async Task<List<(string Location, IPAddress LocalIp, IPAddress Source)>> SsdpSearchOnAsync(IPAddress localAddress, CancellationToken ct)
    {
        var found = new List<(string Location, IPAddress LocalIp, IPAddress Source)>();
        try
        {
            using var udp = new UdpClient(AddressFamily.InterNetwork);
            udp.Client.Bind(new IPEndPoint(localAddress, 0));
            if (!localAddress.Equals(IPAddress.Any))
            {
                try
                {
                    udp.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, localAddress.GetAddressBytes());
                }
                catch (SocketException)
                {
                    // Not fatal: the OS then uses its default multicast interface.
                }
            }
            udp.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 4);

            var target = new IPEndPoint(IPAddress.Parse(SsdpMulticastAddress), SsdpMulticastPort);
            for (var round = 0; round < 2; round++)
            {
                foreach (var st in SsdpSearchTargets)
                {
                    var bytes = Encoding.ASCII.GetBytes(BuildSsdpSearch(st));
                    await udp.SendAsync(bytes, bytes.Length, target).ConfigureAwait(false);
                }
            }

            using var window = CancellationTokenSource.CreateLinkedTokenSource(ct);
            window.CancelAfter(DiscoveryTimeout);
            while (true)
            {
                UdpReceiveResult result;
                try
                {
                    result = await udp.ReceiveAsync(window.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (SocketException)
                {
                    break;
                }

                var location = ParseSsdpLocation(Encoding.ASCII.GetString(result.Buffer));
                if (location is not null && !found.Any(f => f.Location == location))
                {
                    var local = localAddress.Equals(IPAddress.Any) ? (LocalLanIp() ?? localAddress) : localAddress;
                    found.Add((location, local, result.RemoteEndPoint.Address));
                }
            }
        }
        catch (SocketException ex)
        {
            Log($"SSDP search on {localAddress} failed: {ex.Message}");
        }

        ct.ThrowIfCancellationRequested();
        return found;
    }

    internal static string BuildSsdpSearch(string searchTarget) =>
        "M-SEARCH * HTTP/1.1\r\n" +
        $"HOST: {SsdpMulticastAddress}:{SsdpMulticastPort}\r\n" +
        "MAN: \"ssdp:discover\"\r\n" +
        "MX: 2\r\n" +
        $"ST: {searchTarget}\r\n\r\n";

    internal static string? ParseSsdpLocation(string response)
    {
        foreach (var line in response.Split('\n'))
        {
            var idx = line.IndexOf(':');
            if (idx <= 0)
            {
                continue;
            }
            if (!line[..idx].Trim().Equals("location", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            var value = line[(idx + 1)..].Trim();
            return Uri.TryCreate(value, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                ? value
                : null;
        }
        return null;
    }

    private async Task<IReadOnlyList<UpnpDevice>> FetchDevicesAsync(string location, IPAddress responderLocalIp, IReadOnlyList<LocalInterfaceInfo> interfaces, CancellationToken ct)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(SoapTimeout);
        string xml;
        try
        {
            xml = await _http.GetStringAsync(location, timeout.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException($"The router at {location} did not send its description in time.");
        }

        var locationHost = new Uri(location).Host;
        // The control URL must stay on the router that answered (a URLBase pointing elsewhere
        // would turn SOAP calls into requests to arbitrary hosts).
        var services = ParseDeviceDescription(xml, location)
            .Where(s => string.Equals(new Uri(s.ControlUrl).Host, locationHost, StringComparison.OrdinalIgnoreCase))
            .ToList();
        var gateway = IPAddress.TryParse(locationHost, out var g) ? g : null;
        var lanIp = (gateway is null ? null : SelectLanIp(interfaces, gateway)) ?? responderLocalIp;
        return services
            .Select(s => new UpnpDevice(s.ControlUrl, s.ServiceType, lanIp) { Location = location, FriendlyName = s.FriendlyName })
            .ToList();
    }

    internal static IReadOnlyList<(string ServiceType, string ControlUrl, string? FriendlyName)> ParseDeviceDescription(string xml, string location)
    {
        var doc = XDocument.Parse(xml);
        var urlBase = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "URLBase")?.Value.Trim();
        var baseUri = Uri.TryCreate(urlBase, UriKind.Absolute, out var b) ? b : new Uri(location);

        var results = new List<(string, string, string?)>();
        foreach (var service in doc.Descendants().Where(e => e.Name.LocalName == "service"))
        {
            var type = Child(service, "serviceType");
            var control = Child(service, "controlURL");
            if (type is null || control is null || !SupportedServiceTypes.Contains(type))
            {
                continue;
            }
            if (!Uri.TryCreate(baseUri, control, out var controlUri)
                || (controlUri.Scheme != Uri.UriSchemeHttp && controlUri.Scheme != Uri.UriSchemeHttps))
            {
                continue;
            }
            var device = service.Ancestors().FirstOrDefault(e => e.Name.LocalName == "device");
            var friendly = device is null ? null : Child(device, "friendlyName");
            results.Add((type, controlUri.ToString(), friendly));
        }

        return results.OrderBy(r => Array.IndexOf(SupportedServiceTypes, r.Item1)).ToList();

        static string? Child(XElement parent, string name) =>
            parent.Elements().FirstOrDefault(e => e.Name.LocalName == name)?.Value.Trim();
    }

    // -- Forwarding ---------------------------------------------------------------------------

    private sealed class DeviceOutcome
    {
        public List<OwnedPortMapping> Created { get; } = new();

        public List<string> AlreadyOurs { get; } = new();

        public List<string> Failures { get; } = new();

        public bool Fatal { get; set; }
    }

    private async Task<DeviceOutcome> TryForwardOnDeviceAsync(UpnpDevice device, string serverName, IReadOnlyList<PortMapping> wanted, string description, CancellationToken ct)
    {
        var outcome = new DeviceOutcome();
        var existing = MappingsFor(serverName);

        foreach (var mapping in wanted)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var lease = await AddWithLeaseFallbackAsync(device, mapping, description, ct).ConfigureAwait(false);
                var owned = new OwnedPortMapping
                {
                    ServerName = serverName,
                    ExternalPort = mapping.ExternalPort,
                    InternalPort = mapping.InternalPort,
                    Protocol = mapping.SoapProtocol,
                    InternalClient = device.LocalIp.ToString(),
                    ControlUrl = device.ControlUrl,
                    ServiceType = device.ServiceType,
                    Description = description,
                    LeaseSeconds = lease,
                    CreatedUtc = DateTimeOffset.UtcNow,
                };
                outcome.Created.Add(owned);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (UpnpSoapException ex) when (ex.ErrorCode == 718)
            {
                // ConflictInMappingEntry: find out who holds the port.
                var holder = await TryGetSpecificEntryAsync(device, mapping, ct).ConfigureAwait(false);
                if (holder is { } entry && entry.InternalClient == device.LocalIp.ToString() && entry.InternalPort == mapping.InternalPort)
                {
                    // Already forwarded to this PC. Count it as ours only if we made it (or a
                    // previous run did); a rule the user made by hand is left alone.
                    if (entry.Description.StartsWith(MappingDescriptionPrefix, StringComparison.Ordinal)
                        && !existing.Any(e => e.ExternalPort == mapping.ExternalPort && e.Protocol == mapping.SoapProtocol))
                    {
                        outcome.Created.Add(new OwnedPortMapping
                        {
                            ServerName = serverName,
                            ExternalPort = mapping.ExternalPort,
                            InternalPort = mapping.InternalPort,
                            Protocol = mapping.SoapProtocol,
                            InternalClient = device.LocalIp.ToString(),
                            ControlUrl = device.ControlUrl,
                            ServiceType = device.ServiceType,
                            Description = entry.Description,
                            LeaseSeconds = entry.LeaseSeconds,
                            CreatedUtc = DateTimeOffset.UtcNow,
                        });
                    }
                    else
                    {
                        outcome.AlreadyOurs.Add($"{mapping.ExternalPort}/{mapping.SoapProtocol}");
                    }
                }
                else
                {
                    outcome.Failures.Add(FriendlySoapError(718, ex.ErrorDescription, mapping.ExternalPort, mapping.SoapProtocol, holder?.InternalClient));
                }
            }
            catch (UpnpSoapException ex) when (ex.ErrorCode is 606 or 401 && outcome.Created.Count == 0)
            {
                outcome.Failures.Add(FriendlySoapError(ex.ErrorCode, ex.ErrorDescription, mapping.ExternalPort, mapping.SoapProtocol, null));
                // Not authorized / unsupported: the other mappings will fail the same way here.
                outcome.Fatal = ex.ErrorCode == 606;
                break;
            }
            catch (Exception ex)
            {
                outcome.Failures.Add(DescribeError(ex, mapping.ExternalPort, mapping.SoapProtocol));
                if (ex is HttpRequestException or TimeoutException && outcome.Created.Count == 0)
                {
                    break; // router unreachable: try the next device
                }
            }
        }

        return outcome;
    }

    private async Task<UpnpForwardResult> FinishForwardAsync(string serverName, UpnpDevice device, DeviceOutcome outcome, CancellationToken ct)
    {
        lock (_ownedLock)
        {
            foreach (var created in outcome.Created)
            {
                _owned.RemoveAll(o => o.ExternalPort == created.ExternalPort && o.Protocol == created.Protocol
                                      && string.Equals(o.ControlUrl, created.ControlUrl, StringComparison.OrdinalIgnoreCase));
                _owned.Add(created);
            }
        }
        SaveOwned();

        string? externalIp = null;
        try
        {
            externalIp = await GetExternalIpAsync(device, ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log($"GetExternalIPAddress failed: {ex.Message}");
        }

        var kind = ClassifyIp(externalIp);
        var warning = ExternalIpWarning(kind, externalIp);
        lock (_ownedLock)
        {
            _externalIps[serverName] = (externalIp, kind);
        }

        ScheduleRenewal(serverName);

        var mappedNames = outcome.Created.Select(c => c.ToString()).Concat(outcome.AlreadyOurs).ToList();
        var message = $"Forwarded {string.Join(", ", mappedNames)} to {device.LocalIp}"
                      + (device.FriendlyName is { Length: > 0 } fn ? $" on {fn}." : ".");
        if (outcome.Created.Any(c => c.LeaseSeconds > 0))
        {
            message += " The router only allows temporary mappings, so they are renewed while the server runs.";
        }
        if (outcome.Failures.Count > 0)
        {
            message += " Not forwarded: " + string.Join(" ", outcome.Failures);
        }

        Log($"{serverName}: {message} External IP: {externalIp ?? "unknown"} ({kind}).");
        return new UpnpForwardResult(true, message, externalIp, kind, warning, MappingsFor(serverName), outcome.Failures);
    }

    private async Task<int> AddWithLeaseFallbackAsync(UpnpDevice device, PortMapping mapping, string description, CancellationToken ct)
    {
        try
        {
            await AddPortMappingAsync(device, mapping, description, 0, ct).ConfigureAwait(false);
            return 0;
        }
        catch (UpnpSoapException first) when (first.ErrorCode is not (718 or 606 or 401) && first.ErrorCode is not null)
        {
            // Many IGDv2 routers (and some v1 ones) reject permanent leases (402/501/724-ish
            // replies). Retry with a finite lease that gets renewed while the server runs.
            try
            {
                await AddPortMappingAsync(device, mapping, description, FallbackLeaseSeconds, ct).ConfigureAwait(false);
                return FallbackLeaseSeconds;
            }
            catch (UpnpSoapException second) when (second.ErrorCode == 725)
            {
                // OnlyPermanentLeasesSupported: the permanent attempt was the right one, so its
                // error is the real reason.
                throw first;
            }
        }
    }

    private void ScheduleRenewal(string serverName)
    {
        var finite = MappingsFor(serverName).Where(m => m.LeaseSeconds > 0).ToList();
        StopRenewal(serverName);
        if (finite.Count == 0)
        {
            return;
        }

        var period = TimeSpan.FromSeconds(Math.Max(60, finite.Min(m => m.LeaseSeconds) / 2));
        var timer = new Timer(_ => _ = RenewAsync(serverName), null, period, period);
        lock (_ownedLock)
        {
            _renewTimers[serverName] = timer;
        }
    }

    private void StopRenewal(string serverName)
    {
        lock (_ownedLock)
        {
            if (_renewTimers.Remove(serverName, out var timer))
            {
                timer.Dispose();
            }
        }
    }

    private async Task RenewAsync(string serverName)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            foreach (var mapping in MappingsFor(serverName).Where(m => m.LeaseSeconds > 0))
            {
                var device = DeviceFor(mapping);
                await AddPortMappingAsync(device, new PortMapping(mapping.ExternalPort, mapping.InternalPort, ParseProtocol(mapping.Protocol)),
                    mapping.Description, mapping.LeaseSeconds, cts.Token).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Log($"Renewing UPnP leases for {serverName} failed: {ex.Message}");
        }
    }

    private async Task DeleteOwnedMappingAsync(OwnedPortMapping mapping, CancellationToken ct)
    {
        var device = DeviceFor(mapping);
        var portMapping = new PortMapping(mapping.ExternalPort, mapping.InternalPort, ParseProtocol(mapping.Protocol));

        // Only delete it if the router still has *our* entry: if someone else has since taken
        // the port, leave theirs alone.
        var entry = await TryGetSpecificEntryAsync(device, portMapping, ct).ConfigureAwait(false);
        if (entry is { } current
            && (current.InternalClient != mapping.InternalClient
                || !current.Description.StartsWith(MappingDescriptionPrefix, StringComparison.Ordinal)))
        {
            Log($"{mapping} now points at {current.InternalClient} ('{current.Description}'); not removing it.");
            return;
        }

        try
        {
            await UnmapPortAsync(device, portMapping, ct).ConfigureAwait(false);
        }
        catch (UpnpSoapException ex) when (ex.ErrorCode == 714)
        {
            // NoSuchEntryInArray: already gone (expired or the router rebooted).
        }
    }

    private static UpnpDevice DeviceFor(OwnedPortMapping mapping) =>
        new(mapping.ControlUrl, mapping.ServiceType,
            IPAddress.TryParse(mapping.InternalClient, out var ip) ? ip : IPAddress.Any);

    private void ForgetOwned(OwnedPortMapping mapping)
    {
        lock (_ownedLock)
        {
            _owned.RemoveAll(o => o.ExternalPort == mapping.ExternalPort && o.Protocol == mapping.Protocol
                                  && string.Equals(o.ControlUrl, mapping.ControlUrl, StringComparison.OrdinalIgnoreCase));
        }
    }

    private void LoadOwned()
    {
        try
        {
            if (OwnedMappingsFile is { } file && Json.Json.ReadFile<List<OwnedPortMapping>>(file) is { } list)
            {
                lock (_ownedLock)
                {
                    _owned.Clear();
                    _owned.AddRange(list.Where(m => m.ExternalPort is > 0 and <= 65535 && !string.IsNullOrEmpty(m.ControlUrl)));
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Could not read saved UPnP mappings: {ex.Message}");
        }
    }

    private void SaveOwned()
    {
        if (OwnedMappingsFile is not { } file)
        {
            return;
        }
        try
        {
            List<OwnedPortMapping> snapshot;
            lock (_ownedLock)
            {
                snapshot = _owned.ToList();
            }
            var temp = file + ".tmp";
            Json.Json.WriteFile(temp, snapshot);
            File.Move(temp, file, overwrite: true);
        }
        catch (Exception ex)
        {
            Log($"Could not save UPnP mappings: {ex.Message}");
        }
    }

    // -- SOAP ---------------------------------------------------------------------------------

    public async Task<UpnpResult> MapPortAsync(UpnpDevice device, PortMapping mapping, string description, CancellationToken ct = default)
    {
        try
        {
            await AddWithLeaseFallbackAsync(device, mapping, description, ct).ConfigureAwait(false);
            string? externalIp = null;
            try
            {
                externalIp = await GetExternalIpAsync(device, ct).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // The external IP is a nice-to-have; its failure doesn't fail the mapping.
            }
            return UpnpResult.Ok($"Mapped {mapping.ExternalPort}:{mapping.InternalPort} ({mapping.SoapProtocol})", externalIp);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return UpnpResult.Failure(DescribeError(ex, mapping.ExternalPort, mapping.SoapProtocol));
        }
    }

    public async Task UnmapPortAsync(UpnpDevice device, PortMapping mapping, CancellationToken ct = default)
    {
        await SendSoapAsync(device, "DeletePortMapping", BuildDeletePortMappingEnvelope(device, mapping), ct).ConfigureAwait(false);
    }

    public async Task<string> GetExternalIpAsync(UpnpDevice device, CancellationToken ct = default)
    {
        var body = $"<u:GetExternalIPAddress xmlns:u=\"{device.ServiceType}\"></u:GetExternalIPAddress>";
        var response = await SendSoapAsync(device, "GetExternalIPAddress", body, ct).ConfigureAwait(false);
        return ExtractXmlTag(response, "NewExternalIPAddress")
            ?? throw new InvalidOperationException("Router response did not contain NewExternalIPAddress.");
    }

    private async Task AddPortMappingAsync(UpnpDevice device, PortMapping mapping, string description, int leaseSeconds, CancellationToken ct) =>
        await SendSoapAsync(device, "AddPortMapping", BuildAddPortMappingEnvelope(device, mapping, description, leaseSeconds), ct).ConfigureAwait(false);

    internal readonly record struct PortMappingEntry(int InternalPort, string InternalClient, string Description, int LeaseSeconds);

    private async Task<PortMappingEntry?> TryGetSpecificEntryAsync(UpnpDevice device, PortMapping mapping, CancellationToken ct)
    {
        try
        {
            var body =
                $"<u:GetSpecificPortMappingEntry xmlns:u=\"{device.ServiceType}\">" +
                "<NewRemoteHost></NewRemoteHost>" +
                $"<NewExternalPort>{mapping.ExternalPort}</NewExternalPort>" +
                $"<NewProtocol>{mapping.SoapProtocol}</NewProtocol>" +
                "</u:GetSpecificPortMappingEntry>";
            var response = await SendSoapAsync(device, "GetSpecificPortMappingEntry", body, ct).ConfigureAwait(false);
            return ParseSpecificEntry(response);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return null; // 714 NoSuchEntryInArray, or the router doesn't implement the action
        }
    }

    internal static PortMappingEntry? ParseSpecificEntry(string response)
    {
        var client = ExtractXmlTag(response, "NewInternalClient");
        if (client is null)
        {
            return null;
        }
        _ = int.TryParse(ExtractXmlTag(response, "NewInternalPort"), out var port);
        _ = int.TryParse(ExtractXmlTag(response, "NewLeaseDuration"), out var lease);
        var description = WebUtility.HtmlDecode(ExtractXmlTag(response, "NewPortMappingDescription") ?? string.Empty);
        return new PortMappingEntry(port, client, description, lease);
    }

    private async Task<string> SendSoapAsync(UpnpDevice device, string action, string innerBody, CancellationToken ct)
    {
        var envelope =
            "<?xml version=\"1.0\"?>" +
            "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
            "s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
            $"<s:Body>{innerBody}</s:Body></s:Envelope>";

        using var request = new HttpRequestMessage(HttpMethod.Post, device.ControlUrl)
        {
            Content = new StringContent(envelope, Encoding.UTF8, "text/xml"),
        };
        request.Content.Headers.ContentType!.CharSet = "\"utf-8\"";
        request.Headers.TryAddWithoutValidation("SOAPAction", $"\"{device.ServiceType}#{action}\"");

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(SoapTimeout);
        try
        {
            using var response = await _http.SendAsync(request, timeout.Token).ConfigureAwait(false);
            var text = await response.Content.ReadAsStringAsync(timeout.Token).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var (code, description) = ParseSoapFault(text);
                throw new UpnpSoapException(action, code, description, (int)response.StatusCode);
            }
            return text;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException($"The router did not answer {action} within {SoapTimeout.TotalSeconds:0} seconds.");
        }
    }

    internal static (int? Code, string? Description) ParseSoapFault(string body)
    {
        if (string.IsNullOrEmpty(body))
        {
            return (null, null);
        }
        var codeText = ExtractXmlTag(body, "errorCode");
        var description = ExtractXmlTag(body, "errorDescription") ?? ExtractXmlTag(body, "faultstring");
        return (int.TryParse(codeText, out var code) ? code : null, description);
    }

    internal static string FriendlySoapError(int? code, string? description, int port, string protocol, string? conflictingClient)
    {
        var what = $"Port {port}/{protocol}";
        return code switch
        {
            718 => $"{what} is already forwarded to another device" +
                   (string.IsNullOrEmpty(conflictingClient) ? "" : $" ({conflictingClient})") +
                   ". Remove that rule in your router, or change this server's port.",
            725 => $"{what}: the router only accepts permanent port mappings.",
            606 => "The router refused: UPnP is on, but changing port mappings is not allowed. Look for an option like \"Allow UPnP to change settings\" in your router, or use playit.gg.",
            401 => "The router doesn't support this UPnP action (invalid action). Use playit.gg, or forward the port by hand in your router.",
            402 => $"{what}: the router rejected the request's values (invalid arguments).",
            501 => $"{what}: the router failed to apply the mapping (action failed). Try again, or restart the router.",
            714 => $"{what} is not mapped on the router.",
            715 or 726 => "The router only accepts mappings for any remote host (wildcard).",
            716 or 727 => "The router only accepts wildcard external ports.",
            724 => $"{what}: the router requires the external and internal ports to be the same.",
            728 => "The router's port-mapping table is full. Remove unused mappings in the router.",
            729 => $"{what}: the router blocks this port (conflict with its own services).",
            null => $"{what}: the router answered with an error" + (description is { Length: > 0 } d ? $" ({d})." : "."),
            _ => $"{what}: UPnP error {code}" + (description is { Length: > 0 } d2 ? $" ({d2})." : "."),
        };
    }

    private static string DescribeError(Exception ex, int port, string protocol) => ex switch
    {
        UpnpSoapException soap => FriendlySoapError(soap.ErrorCode, soap.ErrorDescription, port, protocol, null),
        TimeoutException => $"Port {port}/{protocol}: the router did not answer in time.",
        HttpRequestException http => $"Port {port}/{protocol}: could not reach the router ({http.Message}).",
        _ => $"Port {port}/{protocol}: {ex.Message}",
    };

    // -- Address helpers ----------------------------------------------------------------------

    public static ExternalIpKind ClassifyIp(string? ip)
    {
        if (string.IsNullOrWhiteSpace(ip) || !IPAddress.TryParse(ip.Trim(), out var address)
            || address.AddressFamily != AddressFamily.InterNetwork)
        {
            return ExternalIpKind.Unknown;
        }
        var b = address.GetAddressBytes();
        if (b.All(x => x == 0))
        {
            return ExternalIpKind.Unknown;
        }
        if (b[0] == 10 || (b[0] == 172 && b[1] >= 16 && b[1] <= 31) || (b[0] == 192 && b[1] == 168))
        {
            return ExternalIpKind.Private;
        }
        if (b[0] == 100 && b[1] >= 64 && b[1] <= 127)
        {
            return ExternalIpKind.CarrierGradeNat;
        }
        return ExternalIpKind.Public;
    }

    public static string? ExternalIpWarning(ExternalIpKind kind, string? ip) => kind switch
    {
        ExternalIpKind.Private =>
            $"Your router's internet address is {ip}, a private address: another router or modem is in front of it (double NAT). " +
            "Players on the internet won't reach this forward. Forward the port on the outer device too, or use playit.gg.",
        ExternalIpKind.CarrierGradeNat =>
            $"Your router's internet address is {ip}, a carrier-grade NAT address shared by your ISP. " +
            "Port forwarding can't work on this connection; use playit.gg instead.",
        ExternalIpKind.Unknown =>
            "The router didn't report a usable internet address (it may be offline). The forward may not be reachable.",
        _ => null,
    };

    public static IPAddress? SelectLanIp(IEnumerable<LocalInterfaceInfo> interfaces, IPAddress gateway)
    {
        var candidates = interfaces
            .Where(i => i.Address.AddressFamily == AddressFamily.InterNetwork
                        && !IPAddress.IsLoopback(i.Address)
                        && !IsLinkLocal(i.Address)
                        && i.Type != NetworkInterfaceType.Loopback)
            .ToList();

        static int Score(LocalInterfaceInfo i, IPAddress gw)
        {
            var score = 0;
            if (i.Gateways.Contains(gw))
            {
                score += 4;
            }
            if (SameSubnet(i.Address, gw, i.Mask))
            {
                score += 2;
            }
            if (!IsVirtualAdapter(i))
            {
                score += 1;
            }
            return score;
        }

        return candidates
            .Where(i => SameSubnet(i.Address, gateway, i.Mask) || i.Gateways.Contains(gateway))
            .OrderByDescending(i => Score(i, gateway))
            .Select(i => i.Address)
            .FirstOrDefault();
    }

    internal static bool IsVirtualAdapter(LocalInterfaceInfo info)
    {
        if (info.Type is NetworkInterfaceType.Loopback or NetworkInterfaceType.Tunnel or NetworkInterfaceType.Ppp)
        {
            return true;
        }
        var text = (info.Name + " " + info.Description).ToLowerInvariant();
        return VirtualAdapterMarkers.Any(text.Contains);
    }

    private static bool IsLinkLocal(IPAddress address)
    {
        var b = address.GetAddressBytes();
        return b.Length == 4 && b[0] == 169 && b[1] == 254;
    }

    internal static bool SameSubnet(IPAddress a, IPAddress b, IPAddress mask)
    {
        var ab = a.GetAddressBytes();
        var bb = b.GetAddressBytes();
        var mb = mask.GetAddressBytes();
        if (ab.Length != 4 || bb.Length != 4 || mb.Length != 4 || mb.All(x => x == 0))
        {
            return false;
        }
        for (var i = 0; i < 4; i++)
        {
            if ((ab[i] & mb[i]) != (bb[i] & mb[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static IReadOnlyList<LocalInterfaceInfo> GetLocalInterfaces()
    {
        var list = new List<LocalInterfaceInfo>();
        try
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up || nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }
                IPInterfaceProperties props;
                try
                {
                    props = nic.GetIPProperties();
                }
                catch (NetworkInformationException)
                {
                    continue;
                }
                var gateways = props.GatewayAddresses
                    .Select(g => g.Address)
                    .Where(a => a.AddressFamily == AddressFamily.InterNetwork && !a.Equals(IPAddress.Any))
                    .ToList();
                foreach (var unicast in props.UnicastAddresses)
                {
                    if (unicast.Address.AddressFamily != AddressFamily.InterNetwork || IPAddress.IsLoopback(unicast.Address))
                    {
                        continue;
                    }
                    list.Add(new LocalInterfaceInfo(nic.Name, nic.Description, nic.NetworkInterfaceType, unicast.Address,
                        unicast.IPv4Mask ?? IPAddress.Parse("255.255.255.0"), gateways));
                }
            }
        }
        catch (NetworkInformationException)
        {
            // Fall through with whatever was collected.
        }
        return list;
    }

    // -- Port list ----------------------------------------------------------------------------

    public static IReadOnlyList<PortMapping> BuildPortMappings(int serverPort, string? gameProtocol, IEnumerable<string> extraPorts)
    {
        var result = new List<PortMapping>();
        void Add(int port, IEnumerable<string> protocols)
        {
            if (port is < 1 or > 65535)
            {
                return;
            }
            foreach (var proto in protocols)
            {
                var mapping = new PortMapping(port, port, ParseProtocol(proto));
                if (!result.Contains(mapping))
                {
                    result.Add(mapping);
                }
            }
        }

        var defaultProtocols = FirewallService.ProtocolsFor(gameProtocol ?? "both");
        Add(serverPort, defaultProtocols);

        foreach (var raw in extraPorts ?? Enumerable.Empty<string>())
        {
            if (TryParseExtraPort(raw, out var from, out var to, out var proto))
            {
                for (var p = from; p <= to; p++)
                {
                    Add(p, proto is null ? defaultProtocols : FirewallService.ProtocolsFor(proto));
                }
            }
        }
        return result;
    }

    public static bool TryParseExtraPort(string? raw, out int from, out int to, out string? protocol)
    {
        from = to = 0;
        protocol = null;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }
        var match = Regex.Match(raw.Trim(), @"^(\d{1,5})(?:\s*-\s*(\d{1,5}))?(?:\s*/\s*(tcp|udp|both))?$", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            return false;
        }
        from = int.Parse(match.Groups[1].Value);
        to = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : from;
        protocol = match.Groups[3].Success ? match.Groups[3].Value.ToLowerInvariant() : null;
        return from is >= 1 and <= 65535 && to >= from && to <= 65535 && to - from < 16;
    }

    public static string MappingDescription(string serverName)
    {
        var name = Regex.Replace(serverName ?? string.Empty, @"[\x00-\x1F]", " ").Trim();
        var text = $"{MappingDescriptionPrefix} {name}".Trim();
        return text.Length > 60 ? text[..60] : text; // some routers truncate or reject long descriptions
    }

    private static PortProtocol ParseProtocol(string protocol) =>
        protocol.Equals("UDP", StringComparison.OrdinalIgnoreCase) ? PortProtocol.Udp : PortProtocol.Tcp;

    internal static string BuildAddPortMappingEnvelope(UpnpDevice device, PortMapping mapping, string description, int leaseSeconds = 0) =>
        $"<u:AddPortMapping xmlns:u=\"{device.ServiceType}\">" +
        "<NewRemoteHost></NewRemoteHost>" +
        $"<NewExternalPort>{mapping.ExternalPort}</NewExternalPort>" +
        $"<NewProtocol>{mapping.SoapProtocol}</NewProtocol>" +
        $"<NewInternalPort>{mapping.InternalPort}</NewInternalPort>" +
        $"<NewInternalClient>{device.LocalIp}</NewInternalClient>" +
        "<NewEnabled>1</NewEnabled>" +
        $"<NewPortMappingDescription>{SecurityElement.Escape(description)}</NewPortMappingDescription>" +
        $"<NewLeaseDuration>{Math.Max(0, leaseSeconds)}</NewLeaseDuration>" +
        "</u:AddPortMapping>";

    internal static string BuildDeletePortMappingEnvelope(UpnpDevice device, PortMapping mapping) =>
        $"<u:DeletePortMapping xmlns:u=\"{device.ServiceType}\">" +
        "<NewRemoteHost></NewRemoteHost>" +
        $"<NewExternalPort>{mapping.ExternalPort}</NewExternalPort>" +
        $"<NewProtocol>{mapping.SoapProtocol}</NewProtocol>" +
        "</u:DeletePortMapping>";

    internal static string? ExtractControlUrl(string xml, string serviceName)
    {
        var marker = $":{serviceName}:";
        var idx = xml.IndexOf(marker, StringComparison.Ordinal);
        return idx < 0 ? null : ExtractXmlTag(xml[idx..], "controlURL");
    }

    internal static string? ExtractXmlTag(string xml, string tag)
    {
        var match = Regex.Match(xml, $@"<(?:[A-Za-z0-9_]+:)?{tag}(?:\s[^>]*)?>(.*?)</(?:[A-Za-z0-9_]+:)?{tag}\s*>", RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    private static IPAddress? LocalLanIp()
    {
        try
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect("8.8.8.8", 80);
            return ((IPEndPoint)socket.LocalEndPoint!).Address;
        }
        catch (SocketException)
        {
            return null;
        }
    }
}
