using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

/// <summary>
/// Network-free tests for <see cref="TunnelService"/>: UPnP SOAP/SSDP/description parsing,
/// address classification and LAN-IP selection, and the playit.gg agent's output/IPC/release
/// handling (sample lines taken from the playit-agent v2.0.2 source).
/// </summary>
public class TunnelServiceTests
{
    // -- SOAP ---------------------------------------------------------------------------------

    private const string ConflictFault = """
        <?xml version="1.0"?>
        <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/" s:encodingStyle="http://schemas.xmlsoap.org/soap/encoding/">
        <s:Body><s:Fault><faultcode>s:Client</faultcode><faultstring>UPnPError</faultstring>
        <detail><UPnPError xmlns="urn:schemas-upnp-org:control-1-0"><errorCode>718</errorCode>
        <errorDescription>ConflictInMappingEntry</errorDescription></UPnPError></detail></s:Fault></s:Body></s:Envelope>
        """;

    [Fact]
    public void ParseSoapFault_reads_upnp_error_code_and_description()
    {
        var (code, description) = TunnelService.ParseSoapFault(ConflictFault);
        Assert.Equal(718, code);
        Assert.Equal("ConflictInMappingEntry", description);
    }

    [Fact]
    public void ParseSoapFault_handles_prefixed_tags_and_garbage()
    {
        var (code, _) = TunnelService.ParseSoapFault("<e:UPnPError><e:errorCode>606</e:errorCode><e:errorDescription>Action not authorized</e:errorDescription></e:UPnPError>");
        Assert.Equal(606, code);
        Assert.Equal((null, null), TunnelService.ParseSoapFault(string.Empty));
        Assert.Null(TunnelService.ParseSoapFault("<html>Internal error</html>").Code);
    }

    [Theory]
    [InlineData(718, "already forwarded to another device")]
    [InlineData(725, "only accepts permanent")]
    [InlineData(606, "Allow UPnP to change settings")]
    [InlineData(401, "doesn't support this UPnP action")]
    [InlineData(999, "UPnP error 999")]
    public void FriendlySoapError_explains_common_codes(int code, string expected)
    {
        var text = TunnelService.FriendlySoapError(code, "desc", 25565, "TCP", code == 718 ? "192.168.1.23" : null);
        Assert.Contains(expected, text);
        if (code == 718)
        {
            Assert.Contains("192.168.1.23", text);
        }
    }

    [Fact]
    public void ParseSpecificEntry_reads_existing_mapping()
    {
        const string response = """
            <s:Envelope><s:Body><u:GetSpecificPortMappingEntryResponse xmlns:u="urn:schemas-upnp-org:service:WANIPConnection:1">
            <NewInternalPort>25565</NewInternalPort><NewInternalClient>192.168.1.50</NewInternalClient><NewEnabled>1</NewEnabled>
            <NewPortMappingDescription>MPServerProvider My &amp; Server</NewPortMappingDescription><NewLeaseDuration>3600</NewLeaseDuration>
            </u:GetSpecificPortMappingEntryResponse></s:Body></s:Envelope>
            """;
        var entry = TunnelService.ParseSpecificEntry(response);
        Assert.NotNull(entry);
        Assert.Equal(25565, entry!.Value.InternalPort);
        Assert.Equal("192.168.1.50", entry.Value.InternalClient);
        Assert.Equal("MPServerProvider My & Server", entry.Value.Description);
        Assert.Equal(3600, entry.Value.LeaseSeconds);
    }

    [Fact]
    public void AddPortMapping_envelope_escapes_description_and_sets_lease()
    {
        var device = new UpnpDevice("http://192.168.1.1:5000/ctl", "urn:schemas-upnp-org:service:WANIPConnection:2", IPAddress.Parse("192.168.1.50"));
        var body = TunnelService.BuildAddPortMappingEnvelope(device, new PortMapping(7777, 7777, PortProtocol.Udp), "MPServerProvider <A&B>", 3600);
        Assert.Contains("<NewPortMappingDescription>MPServerProvider &lt;A&amp;B&gt;</NewPortMappingDescription>", body);
        Assert.Contains("<NewLeaseDuration>3600</NewLeaseDuration>", body);
        Assert.Contains("<NewProtocol>UDP</NewProtocol>", body);
        Assert.StartsWith("<u:AddPortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:2\">", body);
    }

    [Fact]
    public void MappingDescription_uses_prefix_and_server_name()
    {
        Assert.Equal("MPServerProvider Valheim World", TunnelService.MappingDescription("Valheim World"));
        Assert.True(TunnelService.MappingDescription(new string('x', 200)).Length <= 60);
    }

    // -- SSDP and device description -----------------------------------------------------------

    [Fact]
    public void ParseSsdpLocation_reads_header_case_insensitively()
    {
        const string response = "HTTP/1.1 200 OK\r\nCACHE-CONTROL: max-age=120\r\nST: urn:schemas-upnp-org:device:InternetGatewayDevice:1\r\nLocation: http://192.168.1.1:49152/rootDesc.xml\r\n\r\n";
        Assert.Equal("http://192.168.1.1:49152/rootDesc.xml", TunnelService.ParseSsdpLocation(response));
        Assert.Null(TunnelService.ParseSsdpLocation("HTTP/1.1 200 OK\r\nLOCATION: file:///c:/x.xml\r\n\r\n"));
        Assert.Null(TunnelService.ParseSsdpLocation("HTTP/1.1 200 OK\r\n\r\n"));
    }

    [Fact]
    public void Ssdp_reply_must_come_from_gateway_and_point_at_it()
    {
        var gateways = new HashSet<IPAddress> { IPAddress.Parse("192.168.1.1") };
        Assert.True(TunnelService.IsAcceptableSsdpResponder(IPAddress.Parse("192.168.1.1"), "http://192.168.1.1:5000/desc.xml", gateways));
        // Another LAN device answering:
        Assert.False(TunnelService.IsAcceptableSsdpResponder(IPAddress.Parse("192.168.1.77"), "http://192.168.1.77/desc.xml", gateways));
        // The gateway's reply pointing somewhere else (SSRF):
        Assert.False(TunnelService.IsAcceptableSsdpResponder(IPAddress.Parse("192.168.1.1"), "http://10.0.0.5/desc.xml", gateways));
        Assert.False(TunnelService.IsAcceptableSsdpResponder(IPAddress.Parse("192.168.1.1"), "http://router.local/desc.xml", gateways));
    }

    [Fact]
    public void ParseDeviceDescription_finds_all_wan_services_resolved_against_urlbase()
    {
        const string xml = """
            <?xml version="1.0"?>
            <root xmlns="urn:schemas-upnp-org:device-1-0">
              <URLBase>http://192.168.1.1:5000/</URLBase>
              <device>
                <deviceType>urn:schemas-upnp-org:device:InternetGatewayDevice:1</deviceType>
                <friendlyName>FRITZ!Box 7590</friendlyName>
                <deviceList><device><deviceType>urn:schemas-upnp-org:device:WANDevice:1</deviceType>
                  <deviceList><device><deviceType>urn:schemas-upnp-org:device:WANConnectionDevice:1</deviceType>
                    <friendlyName>WANConnectionDevice</friendlyName>
                    <serviceList>
                      <service><serviceType>urn:schemas-upnp-org:service:WANPPPConnection:1</serviceType>
                        <controlURL>/igdupnp/control/WANPPPConn1</controlURL></service>
                      <service><serviceType>urn:schemas-upnp-org:service:WANIPConnection:1</serviceType>
                        <controlURL>/igdupnp/control/WANIPConn1</controlURL></service>
                      <service><serviceType>urn:schemas-upnp-org:service:WANCommonInterfaceConfig:1</serviceType>
                        <controlURL>/igdupnp/control/WANCommonIFC1</controlURL></service>
                    </serviceList>
                  </device></deviceList>
                </device></deviceList>
              </device>
            </root>
            """;

        var services = TunnelService.ParseDeviceDescription(xml, "http://192.168.1.1:49000/igddesc.xml");

        Assert.Equal(2, services.Count);
        Assert.Equal("urn:schemas-upnp-org:service:WANIPConnection:1", services[0].ServiceType);
        Assert.Equal("http://192.168.1.1:5000/igdupnp/control/WANIPConn1", services[0].ControlUrl);
        Assert.Equal("urn:schemas-upnp-org:service:WANPPPConnection:1", services[1].ServiceType);
    }

    [Fact]
    public void ParseDeviceDescription_prefers_wanipconnection_v2_and_uses_location_without_urlbase()
    {
        const string xml = """
            <root><device><serviceList>
              <service><serviceType>urn:schemas-upnp-org:service:WANIPConnection:1</serviceType><controlURL>ctl/IPConn</controlURL></service>
              <service><serviceType>urn:schemas-upnp-org:service:WANIPConnection:2</serviceType><controlURL>/ctl/IPConn2</controlURL></service>
            </serviceList></device></root>
            """;
        var services = TunnelService.ParseDeviceDescription(xml, "http://10.0.0.1:1900/desc/root.xml");
        Assert.Equal("urn:schemas-upnp-org:service:WANIPConnection:2", services[0].ServiceType);
        Assert.Equal("http://10.0.0.1:1900/ctl/IPConn2", services[0].ControlUrl);
        Assert.Equal("http://10.0.0.1:1900/desc/ctl/IPConn", services[1].ControlUrl);
    }

    // -- External IP classification -----------------------------------------------------------

    [Theory]
    [InlineData("203.0.113.7", ExternalIpKind.Public)]
    [InlineData("8.8.8.8", ExternalIpKind.Public)]
    [InlineData("10.20.30.40", ExternalIpKind.Private)]
    [InlineData("172.16.0.1", ExternalIpKind.Private)]
    [InlineData("172.31.255.254", ExternalIpKind.Private)]
    [InlineData("172.32.0.1", ExternalIpKind.Public)]
    [InlineData("192.168.0.1", ExternalIpKind.Private)]
    [InlineData("100.64.0.1", ExternalIpKind.CarrierGradeNat)]
    [InlineData("100.127.255.255", ExternalIpKind.CarrierGradeNat)]
    [InlineData("100.128.0.1", ExternalIpKind.Public)]
    [InlineData("0.0.0.0", ExternalIpKind.Unknown)]
    [InlineData("", ExternalIpKind.Unknown)]
    [InlineData(null, ExternalIpKind.Unknown)]
    [InlineData("not-an-ip", ExternalIpKind.Unknown)]
    public void ClassifyIp_detects_private_and_cgnat(string? ip, ExternalIpKind expected)
    {
        Assert.Equal(expected, TunnelService.ClassifyIp(ip));
    }

    [Fact]
    public void ExternalIpWarning_points_to_playit_for_cgnat()
    {
        Assert.Contains("playit.gg", TunnelService.ExternalIpWarning(ExternalIpKind.CarrierGradeNat, "100.70.1.2"));
        Assert.Contains("double NAT", TunnelService.ExternalIpWarning(ExternalIpKind.Private, "192.168.0.10"));
        Assert.Null(TunnelService.ExternalIpWarning(ExternalIpKind.Public, "203.0.113.7"));
    }

    // -- LAN IP selection ---------------------------------------------------------------------

    private static LocalInterfaceInfo Nic(string name, string description, NetworkInterfaceType type, string ip, string mask, params string[] gateways) =>
        new(name, description, type, IPAddress.Parse(ip), IPAddress.Parse(mask), gateways.Select(IPAddress.Parse).ToList());

    [Fact]
    public void SelectLanIp_picks_physical_adapter_routing_to_gateway()
    {
        var interfaces = new[]
        {
            Nic("vEthernet (Default Switch)", "Hyper-V Virtual Ethernet Adapter", NetworkInterfaceType.Ethernet, "192.168.1.200", "255.255.255.0"),
            Nic("Ethernet", "Intel(R) Ethernet Connection I219-V", NetworkInterfaceType.Ethernet, "192.168.1.50", "255.255.255.0", "192.168.1.1"),
            Nic("OpenVPN", "TAP-Windows Adapter V9", NetworkInterfaceType.Ethernet, "10.8.0.2", "255.255.255.0", "10.8.0.1"),
            Nic("Loopback", "Software Loopback Interface 1", NetworkInterfaceType.Loopback, "127.0.0.1", "255.0.0.0"),
        };

        Assert.Equal(IPAddress.Parse("192.168.1.50"), TunnelService.SelectLanIp(interfaces, IPAddress.Parse("192.168.1.1")));
    }

    [Fact]
    public void SelectLanIp_uses_subnet_match_and_skips_unrelated_and_link_local()
    {
        var interfaces = new[]
        {
            Nic("Wi-Fi", "Realtek Wireless", NetworkInterfaceType.Wireless80211, "169.254.10.10", "255.255.0.0"),
            Nic("Ethernet 2", "Realtek PCIe GbE", NetworkInterfaceType.Ethernet, "10.0.0.23", "255.255.255.0"),
            Nic("ZeroTier", "ZeroTier Virtual Port", NetworkInterfaceType.Ethernet, "172.22.0.5", "255.255.0.0"),
        };

        Assert.Equal(IPAddress.Parse("10.0.0.23"), TunnelService.SelectLanIp(interfaces, IPAddress.Parse("10.0.0.1")));
        Assert.Null(TunnelService.SelectLanIp(interfaces, IPAddress.Parse("192.168.5.1")));
    }

    // -- Port list ----------------------------------------------------------------------------

    [Fact]
    public void BuildPortMappings_follows_game_protocol_and_extra_ports()
    {
        var both = TunnelService.BuildPortMappings(25565, "both", new[] { "25575/tcp", "19132/udp", "27015-27016", "junk", "70000" });
        Assert.Equal(new[]
        {
            "25565/TCP", "25565/UDP", "25575/TCP", "19132/UDP", "27015/TCP", "27015/UDP", "27016/TCP", "27016/UDP",
        }, both.Select(m => $"{m.ExternalPort}/{m.SoapProtocol}"));

        var udp = TunnelService.BuildPortMappings(2456, "udp", new[] { "2457", "2456/udp" });
        Assert.Equal(new[] { "2456/UDP", "2457/UDP" }, udp.Select(m => $"{m.ExternalPort}/{m.SoapProtocol}"));

        Assert.Empty(TunnelService.BuildPortMappings(0, "tcp", Array.Empty<string>()));
    }

    [Theory]
    [InlineData("27015", true)]
    [InlineData("27015/UDP", true)]
    [InlineData("8080 / tcp", true)]
    [InlineData("1-16/both", true)]
    [InlineData("1-17", false)]
    [InlineData("0", false)]
    [InlineData("65536", false)]
    [InlineData("27016-27015", false)]
    [InlineData("27015/sctp", false)]
    [InlineData("", false)]
    public void TryParseExtraPort_validates(string raw, bool ok)
    {
        Assert.Equal(ok, TunnelService.TryParseExtraPort(raw, out _, out _, out _));
    }

    // -- TunnelSpec ---------------------------------------------------------------------------

    [Fact]
    public void TunnelSpec_round_trips_new_fields_and_loads_old_files()
    {
        var spec = new TunnelSpec { Mode = "upnp", AutoStart = true, ExtraPorts = { "27015/udp" } };
        var json = Json.Json.Serialize(spec);
        Assert.Contains("\"auto_start\": true", json);
        Assert.Contains("\"extra_ports\"", json);
        Assert.DoesNotContain("normalized_mode", json);

        var back = Json.Json.Deserialize<TunnelSpec>(json)!;
        Assert.Equal(TunnelSpec.ModeUpnp, back.NormalizedMode);
        Assert.True(back.AutoStart);
        Assert.Equal(new[] { "27015/udp" }, back.ExtraPorts);

        var old = Json.Json.Deserialize<TunnelSpec>("{ \"mode\": \"PLAYIT\" }")!;
        Assert.Equal(TunnelSpec.ModePlayit, old.NormalizedMode);
        Assert.False(old.AutoStart);
        Assert.Empty(old.ExtraPorts);
        Assert.Equal(TunnelSpec.ModeNone, new TunnelSpec { Mode = "ngrok" }.NormalizedMode);
    }

    // -- playit output ------------------------------------------------------------------------

    [Fact]
    public void Playit_parser_reads_setup_flow_lines()
    {
        // From playit-cli main.rs run_setup_flow/claim_exchange (println of write_screen text).
        Assert.Equal(PlayitLineKind.None, PlayitOutputParser.Parse("Open this link to finish setting up playit:").Kind);

        var claim = PlayitOutputParser.Parse("https://playit.gg/claim/1a2b3c4d5e");
        Assert.Equal(PlayitLineKind.ClaimUrl, claim.Kind);
        Assert.Equal("https://playit.gg/claim/1a2b3c4d5e", claim.Url);

        Assert.Equal(PlayitLineKind.WaitingForApproval, PlayitOutputParser.Parse("Approve this program in your browser:").Kind);
        Assert.Equal(PlayitLineKind.Approved, PlayitOutputParser.Parse("Program approved. Finishing setup...").Kind);
        Assert.Equal(PlayitLineKind.SetupComplete, PlayitOutputParser.Parse("playit setup is complete. The background service is ready.").Kind);
        Assert.Equal(PlayitLineKind.Fatal, PlayitOutputParser.Parse("Setup was not approved in the browser.").Kind);

        var login = PlayitOutputParser.Parse("https://playit.gg/login/guest-account/9f8e7d6c5b4a");
        Assert.Equal(PlayitLineKind.LoginUrl, login.Kind);

        Assert.Equal(PlayitLineKind.PortableMode, PlayitOutputParser.Parse(
            "The playit background service is not installed, so playit is running in this window.").Kind);
    }

    [Fact]
    public void Playit_parser_reads_daemon_log_lines()
    {
        // Format from playit-cli client.rs apply_stdout_update: "{ts} {LEVEL:>5} {target}: {message}".
        var connected = PlayitOutputParser.Parse(
            "2026-11-15T10:00:00.123456Z  INFO playitd::daemon: playit connected; tunnels loaded, agent_id=5c4b…, tunnel_count=1, pending_tunnel_count=0, disabled_tunnel_count=0, account_status=Verified");
        Assert.Equal(PlayitLineKind.Connected, connected.Kind);

        var crashed = PlayitOutputParser.Parse(
            "2026-11-15T10:00:05.000001Z ERROR playitd::daemon: playit agent stopped unexpectedly, reason=connection reset, retry_in_secs=5");
        Assert.Equal(PlayitLineKind.Error, crashed.Kind);

        var invalid = PlayitOutputParser.Parse("2026-11-15T10:00:01.000000Z  WARN playitd::daemon: configured agent secret is no longer valid, error=InvalidSecret");
        Assert.Equal(PlayitLineKind.Fatal, invalid.Kind);

        Assert.Equal(PlayitLineKind.Fatal, PlayitOutputParser.Parse(
            "playit is already running in another window. Use that window, or close it and try again.").Kind);
        Assert.Equal(PlayitLineKind.Fatal, PlayitOutputParser.Parse(
            "The playit service cannot start because this account is over the agent limit.").Kind);
    }

    [Fact]
    public void Playit_parser_reads_tunnel_lines_and_strips_ansi()
    {
        var tui = PlayitOutputParser.Parse("● mc-example.gl.joinmc.link => 127.0.0.1:25565");
        Assert.Equal(PlayitLineKind.Tunnel, tui.Kind);
        Assert.Equal(new PlayitTunnel("mc-example.gl.joinmc.link", "127.0.0.1:25565"), tui.Tunnel);

        var legacy = PlayitOutputParser.Parse("\u001b[32mexample-12345.playit.gg:12345 => 127.0.0.1:7777 (valheim)\u001b[0m");
        Assert.Equal(new PlayitTunnel("example-12345.playit.gg:12345", "127.0.0.1:7777"), legacy.Tunnel);

        var ip = PlayitOutputParser.Parse("147.185.221.20:41234 => 127.0.0.1:25565");
        Assert.Equal("147.185.221.20:41234", ip.Tunnel!.Address);
    }

    [Fact]
    public void Playit_redact_hides_session_keys_and_secrets()
    {
        var text = PlayitOutputParser.Redact("https://playit.gg/login/guest-account/abcdef0123456789");
        Assert.DoesNotContain("abcdef0123456789", text);
        Assert.DoesNotContain("deadbeef", PlayitOutputParser.Redact("secret_key=deadbeefcafebabe"));
        Assert.DoesNotContain(new string('a', 64), PlayitOutputParser.Redact("key " + new string('a', 64)));
        Assert.Equal("https://playit.gg/claim/1a2b3c4d5e", PlayitOutputParser.Redact("https://playit.gg/claim/1a2b3c4d5e"));
    }

    [Theory]
    [InlineData("https://playit.gg/claim/abc123", true)]
    [InlineData("https://www.playit.gg/account/agents", true)]
    [InlineData("http://playit.gg/claim/abc123", false)]
    [InlineData("https://playit.gg.evil.com/claim", false)]
    [InlineData("https://evilplayit.gg/claim", false)]
    [InlineData("https://user@playit.gg/claim", false)]
    [InlineData("https://playit.gg:8443/claim", false)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData(null, false)]
    public void IsTrustedPlayitUrl_only_allows_https_playit_gg(string? url, bool expected)
    {
        Assert.Equal(expected, PlayitOutputParser.IsTrustedPlayitUrl(url));
    }

    // -- playit IPC ---------------------------------------------------------------------------

    [Fact]
    public void Ipc_state_response_running_lists_tunnels()
    {
        // Shape from playit-ipc ipc.rs/model.rs (serde: message_kind/data, type/data, state/data).
        const string line = """{"message_kind":"response","data":{"ipc_version":3,"request_id":1,"response":{"type":"state","data":{"state":"running","data":{"version":"2.0.2","tunnels":[{"id":"7c1d","display_address":"mc-example.gl.joinmc.link","destination":"127.0.0.1:25565","is_disabled":false,"disabled_reason":null},{"id":"8d2e","display_address":"old.playit.gg:1234","destination":"127.0.0.1:7777","is_disabled":true,"disabled_reason":"over limit"}],"pending_tunnels":[{"id":"9","status_msg":"allocating"}],"notices":[{"priority":"High","message":"Verify your email","resolve_link":null}],"account_status":"guest","agent_id":"a1","login_link":"https://playit.gg/login/xyz","start_time":1763200000000}}}}}""";

        var lifecycle = PlayitOutputParser.ParseIpcStateResponse(line);

        Assert.NotNull(lifecycle);
        Assert.Equal("running", lifecycle!.State);
        Assert.Equal(2, lifecycle.Tunnels.Count);
        Assert.Equal(new PlayitTunnel("mc-example.gl.joinmc.link", "127.0.0.1:25565"), lifecycle.Tunnels[0]);
        Assert.Contains("disabled: over limit", lifecycle.Tunnels[1].Destination);
        Assert.Equal(1, lifecycle.PendingTunnels);
        Assert.Equal("https://playit.gg/login/xyz", lifecycle.LoginLink);
        Assert.Equal(new[] { "Verify your email" }, lifecycle.Notices);
    }

    [Fact]
    public void Ipc_state_response_other_states_and_errors()
    {
        var waiting = PlayitOutputParser.ParseIpcStateResponse(
            """{"message_kind":"response","data":{"ipc_version":3,"request_id":1,"response":{"type":"state","data":{"state":"waiting_for_secret"}}}}""");
        Assert.Equal("waiting_for_secret", waiting!.State);
        Assert.Empty(waiting.Tunnels);

        var invalid = PlayitOutputParser.ParseIpcStateResponse(
            """{"message_kind":"response","data":{"ipc_version":3,"request_id":1,"response":{"type":"state","data":{"state":"has_invalid_secret","data":{"code":"invalid_secret","message":"secret rejected","retryable":false,"details":null}}}}}""");
        Assert.Equal("has_invalid_secret", invalid!.State);
        Assert.Equal("secret rejected", invalid.ErrorMessage);

        var error = PlayitOutputParser.ParseIpcStateResponse(
            """{"message_kind":"response","data":{"ipc_version":3,"request_id":1,"response":{"type":"error","data":{"code":"access_denied","message":"not allowed","retryable":false}}}}""");
        Assert.Equal("error", error!.State);

        Assert.Null(PlayitOutputParser.ParseIpcStateResponse("""{"message_kind":"event","data":{"ipc_version":3,"event":{"type":"log","data":{}}}}"""));
        Assert.Null(PlayitOutputParser.ParseIpcStateResponse("not json"));
        Assert.Equal(3, PlayitOutputParser.ParseIpcHelloVersion("""{"message_kind":"hello","data":{"protocol":{"ipc_version":3,"capabilities":["structured_responses"]}}}"""));
        Assert.Null(PlayitOutputParser.ParseIpcHelloVersion(null));
    }

    // -- playit release download ---------------------------------------------------------------

    private static readonly string[] V202AssetNames =
    {
        "playit-2.0.2.pkg", "playit-cli-linux-amd64", "playit-gui-windows-x86-signed.exe", "playit-gui-windows-x86.exe",
        "playit-gui-windows-x86_64-signed.exe", "playit-gui-windows-x86_64.exe", "playit-linux-amd64",
        "playit-windows-x86-signed.exe", "playit-windows-x86-signed.msi", "playit-windows-x86.exe", "playit-windows-x86.msi",
        "playit-windows-x86_64-signed.exe", "playit-windows-x86_64-signed.msi", "playit-windows-x86_64.exe",
        "playit-windows-x86_64.msi", "playitd-tray-windows-x86_64-signed.exe", "playitd-windows-x86_64-signed.exe",
        "playitd-windows-x86_64.exe", "playit_amd64.deb",
    };

    private static IEnumerable<GitHubReleaseAsset> Assets(params string[] names) =>
        names.Select(n => new GitHubReleaseAsset(n, $"https://github.com/playit-cloud/playit-agent/releases/download/v2.0.2/{n}", 100, null));

    [Fact]
    public void SelectPlayitAsset_picks_signed_cli_exe_for_arch()
    {
        Assert.Equal("playit-windows-x86_64-signed.exe", TunnelService.SelectPlayitAsset(Assets(V202AssetNames), Architecture.X64)!.Name);
        Assert.Equal("playit-windows-x86_64-signed.exe", TunnelService.SelectPlayitAsset(Assets(V202AssetNames), Architecture.Arm64)!.Name);
        Assert.Equal("playit-windows-x86-signed.exe", TunnelService.SelectPlayitAsset(Assets(V202AssetNames), Architecture.X86)!.Name);
        Assert.Equal("playit-windows-x86_64.exe", TunnelService.SelectPlayitAsset(Assets("playit-windows-x86_64.exe", "playit-gui-windows-x86_64-signed.exe"), Architecture.X64)!.Name);
        Assert.Null(TunnelService.SelectPlayitAsset(Assets("playitd-windows-x86_64.exe", "playit-windows-x86_64.msi"), Architecture.X64));
    }

    [Fact]
    public void ParseReleaseAssets_reads_name_url_size_digest()
    {
        const string json = """
            {"tag_name":"v2.0.2","assets":[
              {"name":"playit-windows-x86_64-signed.exe","size":7872608,"digest":"sha256:b56ab11d8e6d4cd866acf2eb79ba9aeb70d1a2a97db2cc23bd297066306ba86e",
               "browser_download_url":"https://github.com/playit-cloud/playit-agent/releases/download/v2.0.2/playit-windows-x86_64-signed.exe"},
              {"name":"no-url","size":1}
            ]}
            """;
        var assets = TunnelService.ParseReleaseAssets(json);
        var asset = Assert.Single(assets);
        Assert.Equal(7872608, asset.Size);
        Assert.StartsWith("sha256:b56ab", asset.Digest);
    }

    [Fact]
    public void IsValidPeExecutable_checks_mz_pe_and_machine()
    {
        var head = new byte[512];
        head[0] = (byte)'M';
        head[1] = (byte)'Z';
        BitConverter.GetBytes(0x80).CopyTo(head, 0x3C);
        head[0x80] = (byte)'P';
        head[0x81] = (byte)'E';
        BitConverter.GetBytes((ushort)0x8664).CopyTo(head, 0x84);

        Assert.True(TunnelService.IsValidPeExecutable(head, 0x8664));
        Assert.False(TunnelService.IsValidPeExecutable(head, 0x014C));

        var html = System.Text.Encoding.ASCII.GetBytes("<!DOCTYPE html><html>Not Found</html>".PadRight(128));
        Assert.False(TunnelService.IsValidPeExecutable(html, 0x8664));

        var badOffset = (byte[])head.Clone();
        BitConverter.GetBytes(100_000).CopyTo(badOffset, 0x3C);
        Assert.False(TunnelService.IsValidPeExecutable(badOffset, 0x8664));
    }

    [Fact]
    public void New_service_without_state_reports_no_mappings()
    {
        using var service = new TunnelService();
        Assert.Empty(service.MappingsFor("anything"));
        Assert.False(service.IsPlayitRunning);
    }
}
