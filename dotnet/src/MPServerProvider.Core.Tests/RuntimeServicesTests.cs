using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class RuntimeServicesTests
{
    // -- FirewallService --------------------------------------------------------------------

    [Theory]
    [InlineData("tcp", new[] { "TCP" })]
    [InlineData("TCP", new[] { "TCP" })]
    [InlineData("udp", new[] { "UDP" })]
    [InlineData("UDP", new[] { "UDP" })]
    [InlineData("both", new[] { "TCP", "UDP" })]
    [InlineData("", new[] { "TCP", "UDP" })]
    public void FirewallService_ProtocolsFor_matches_decompiled_literals(string protocol, string[] expected)
    {
        Assert.Equal(expected, FirewallService.ProtocolsFor(protocol));
    }

    [Fact]
    public void FirewallService_rule_name_uses_mpsm_prefix()
    {
        Assert.Equal("MPSM - My Server", FirewallService.RulePrefix + "My Server");
    }

    // -- TunnelService / UpnpResult ----------------------------------------------------------

    [Fact]
    public void TunnelService_add_port_mapping_envelope_has_correct_soap_shape()
    {
        var device = new UpnpDevice("http://192.168.1.1:5000/ctl/IPConn", "urn:schemas-upnp-org:service:WANIPConnection:1", System.Net.IPAddress.Parse("192.168.1.50"));
        var mapping = new PortMapping(25565, 25565, PortProtocol.Tcp);

        var body = TunnelService.BuildAddPortMappingEnvelope(device, mapping, "MP Server Provider");

        Assert.StartsWith("<u:AddPortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">", body);
        Assert.Contains("<NewExternalPort>25565</NewExternalPort>", body);
        Assert.Contains("<NewProtocol>TCP</NewProtocol>", body);
        Assert.Contains("<NewInternalClient>192.168.1.50</NewInternalClient>", body);
        Assert.EndsWith("</u:AddPortMapping>", body);
    }

    [Fact]
    public void TunnelService_delete_port_mapping_envelope_has_correct_soap_shape()
    {
        var device = new UpnpDevice("http://192.168.1.1:5000/ctl/IPConn", "urn:schemas-upnp-org:service:WANIPConnection:1", System.Net.IPAddress.Parse("192.168.1.50"));
        var mapping = new PortMapping(7777, 7777, PortProtocol.Udp);

        var body = TunnelService.BuildDeletePortMappingEnvelope(device, mapping);

        Assert.Contains("<NewExternalPort>7777</NewExternalPort>", body);
        Assert.Contains("<NewProtocol>UDP</NewProtocol>", body);
        Assert.DoesNotContain("AddPortMapping", body);
    }

    [Fact]
    public void TunnelService_extract_control_url_finds_wanip_connection_service()
    {
        const string xml = """
            <root><device><serviceList>
            <service><serviceType>urn:schemas-upnp-org:service:WANIPConnection:1</serviceType>
            <controlURL>/upnp/control/WANIPConn1</controlURL></service>
            </serviceList></device></root>
            """;

        Assert.Equal("/upnp/control/WANIPConn1", TunnelService.ExtractControlUrl(xml, "WANIPConnection"));
    }

    [Fact]
    public void TunnelService_extract_xml_tag_handles_missing_tag()
    {
        Assert.Null(TunnelService.ExtractXmlTag("<a><b>1</b></a>", "missing"));
    }

    [Fact]
    public void UpnpResult_ok_and_failure_constructors()
    {
        var ok = UpnpResult.Ok("mapped", "1.2.3.4");
        Assert.True(ok.Success);
        Assert.Equal("1.2.3.4", ok.ExternalIp);

        var fail = UpnpResult.Failure("nope");
        Assert.False(fail.Success);
        Assert.Null(fail.ExternalIp);
    }

    [Fact]
    public void TunnelService_playit_exe_path_matches_decompiled_literals()
    {
        var service = new TunnelService();
        var path = service.PlayitExePath(new FakeToolPaths());
        Assert.Equal(Path.Combine(@"C:\tools", "playit", "playit.exe"), path);
    }

    private sealed class FakeToolPaths : TunnelService.IToolPaths
    {
        public string ToolsDir => @"C:\tools";
    }

    // -- QueryService -------------------------------------------------------------------------

    [Fact]
    public void QueryService_a2s_info_request_matches_known_bytes()
    {
        var bytes = QueryService.BuildA2SInfoRequest();
        var expected = new List<byte> { 0xFF, 0xFF, 0xFF, 0xFF, 0x54 };
        expected.AddRange(System.Text.Encoding.ASCII.GetBytes("Source Engine Query\0"));
        Assert.Equal(expected.ToArray(), bytes);
    }

    [Fact]
    public void QueryService_parse_a2s_info_response_hand_built_buffer()
    {
        var buf = new List<byte> { 0xFF, 0xFF, 0xFF, 0xFF, 0x49, 17 };
        buf.AddRange(System.Text.Encoding.ASCII.GetBytes("My Server\0"));
        buf.AddRange(System.Text.Encoding.ASCII.GetBytes("de_dust2\0"));
        buf.AddRange(System.Text.Encoding.ASCII.GetBytes("csgo\0"));
        buf.AddRange(System.Text.Encoding.ASCII.GetBytes("Counter-Strike\0"));
        buf.AddRange(BitConverter.GetBytes((short)730));
        buf.Add(5); // players
        buf.Add(10); // max players
        buf.Add(0); // bots

        var result = QueryService.ParseA2SInfoResponse(buf.ToArray());

        Assert.NotNull(result);
        Assert.Equal(5, result!.Value.Players);
        Assert.Equal(10, result.Value.MaxPlayers);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(127)]
    [InlineData(128)]
    [InlineData(255)]
    [InlineData(2097151)]
    [InlineData(int.MaxValue)]
    public void QueryService_varint_round_trips_small_and_large_values(int value)
    {
        var buf = new List<byte>();
        QueryService.WriteVarInt(buf, value);
        var (decoded, consumed) = QueryService.ReadVarInt(buf.ToArray());
        Assert.Equal(value, decoded);
        Assert.Equal(buf.Count, consumed);
    }

    [Fact]
    public void QueryService_mc_packet_framing_round_trips_length_prefix()
    {
        var packet = QueryService.BuildMcPacket(0x00, System.Text.Encoding.ASCII.GetBytes("hello"));
        var (length, lengthBytes) = QueryService.ReadVarInt(packet);
        Assert.Equal(packet.Length - lengthBytes, length);
    }

    [Fact]
    public void QueryService_unconnected_ping_has_correct_magic_and_id()
    {
        var ping = QueryService.BuildUnconnectedPing(1234, 5678);
        Assert.Equal(0x01, ping[0]);
        Assert.Equal(QueryService.RakNetMagic, ping[9..25]);
    }

    [Fact]
    public void QueryService_parse_unconnected_pong_hand_built_buffer()
    {
        const string payload = "MCPE;Dedicated Server;671;1.20.0;3;10;12345;server;Survival;1;19132;19133;";
        var buf = new List<byte> { 0x1c };
        buf.AddRange(ToBigEndian(0L)); // timestamp
        buf.AddRange(ToBigEndian(0L)); // server guid
        buf.AddRange(QueryService.RakNetMagic);
        var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
        buf.Add((byte)(payloadBytes.Length >> 8));
        buf.Add((byte)(payloadBytes.Length & 0xFF));
        buf.AddRange(payloadBytes);

        var result = QueryService.ParseUnconnectedPong(buf.ToArray());

        Assert.NotNull(result);
        Assert.Equal(3, result!.Value.Players);
        Assert.Equal(10, result.Value.MaxPlayers);
    }

    [Fact]
    public void QueryService_parse_fivem_info_response_extracts_client_counts()
    {
        var body = new System.Text.StringBuilder();
        body.Append("\uFFFD\uFFFD\uFFFD\uFFFDinfoResponse\n");
        body.Append('\u0001').Append("clients").Append('\u0000').Append('4').Append('\u0000');
        body.Append('\u0001').Append("sv_maxclients").Append('\u0000').Append("32").Append('\u0000');

        var result = QueryService.ParseFiveMInfoResponse(System.Text.Encoding.UTF8.GetBytes(body.ToString()));

        Assert.NotNull(result);
        Assert.Equal(4, result!.Value.Players);
        Assert.Equal(32, result.Value.MaxPlayers);
    }

    private static byte[] ToBigEndian(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return bytes;
    }

    // -- RconService / RconPacket --------------------------------------------------------------

    [Fact]
    public void RconPacket_encode_round_trips_through_decode_payload()
    {
        var bytes = RconPacket.Encode(7, RconService.ServerDataExecCommand, "status");
        var size = BitConverter.ToInt32(bytes, 0);
        Assert.Equal(bytes.Length - 4, size);

        var decoded = RconPacket.DecodePayload(bytes.AsSpan(4));
        Assert.Equal(7, decoded.Id);
        Assert.Equal(RconService.ServerDataExecCommand, decoded.PacketType);
        Assert.Equal("status", decoded.Body);
    }

    [Fact]
    public void RconPacket_encode_matches_known_good_auth_packet_shape()
    {
        var bytes = RconPacket.Encode(1, RconService.ServerDataAuth, "password");

        var expected = new List<byte>();
        var payloadLen = 4 + 4 + "password".Length + 1 + 1;
        expected.AddRange(BitConverter.GetBytes(payloadLen));
        expected.AddRange(BitConverter.GetBytes(1));
        expected.AddRange(BitConverter.GetBytes(RconService.ServerDataAuth));
        expected.AddRange(System.Text.Encoding.ASCII.GetBytes("password"));
        expected.Add(0);
        expected.Add(0);

        Assert.Equal(expected.ToArray(), bytes);
    }

    [Fact]
    public void RconPacket_decode_payload_rejects_missing_terminators()
    {
        var bytes = RconPacket.Encode(1, RconService.ServerDataExecCommand, "x");
        bytes[^1] = 1; // corrupt the final NUL terminator
        Assert.Throws<FormatException>(() => RconPacket.DecodePayload(bytes.AsSpan(4)));
    }

    [Fact]
    public void RconPacket_decode_payload_rejects_too_short_packets()
    {
        Assert.Throws<FormatException>(() => RconPacket.DecodePayload(new byte[4]));
    }

    [Fact]
    public void RconPacket_empty_body_round_trips()
    {
        var bytes = RconPacket.Encode(42, RconService.ServerDataResponseValue, "");
        var decoded = RconPacket.DecodePayload(bytes.AsSpan(4));
        Assert.Equal("", decoded.Body);
        Assert.Equal(42, decoded.Id);
    }

    // -- SystemMetricsService / DriveUsage -----------------------------------------------------

    [Fact]
    public void DriveUsage_used_bytes_and_percent()
    {
        var drive = new DriveUsage("C:", @"C:\", 1000, 250);
        Assert.Equal(750, drive.UsedBytes);
        Assert.True(Math.Abs(drive.UsedPercent - 75.0) < 0.0001);
    }

    [Fact]
    public void DriveUsage_zero_total_does_not_divide_by_zero()
    {
        var drive = new DriveUsage("X:", @"X:\", 0, 0);
        Assert.Equal(0.0, drive.UsedPercent);
    }

    [Fact]
    public void SystemMetricsService_directory_size_bytes_of_missing_path_is_zero()
    {
        var path = Path.Combine("Z:", "this", "path", "does", "not", "exist", "hopefully");
        Assert.Equal(0, SystemMetricsService.DirectorySizeBytes(path));
    }

    [Fact]
    public void SystemMetricsService_real_file_systems_matches_decompiled_cctor_literals()
    {
        Assert.Equal(new[] { "NTFS", "ReFS", "exFAT" }, SystemMetricsService.RealFileSystems);
    }

    // -- ArtworkCacheService -------------------------------------------------------------------

    [Fact]
    public void ArtworkCacheService_sanitize_cache_key_strips_unsafe_characters()
    {
        Assert.Equal("730", ArtworkCacheService.SanitizeCacheKey("730"));
        Assert.Equal("my_weird_key", ArtworkCacheService.SanitizeCacheKey("my/weird:key"));
    }

    // -- ServerProcessService.SplitCommandLine --------------------------------------------------

    [Fact]
    public void ServerProcessService_split_command_line_handles_quoted_executable()
    {
        var (file, args) = ServerProcessService.SplitCommandLine("\"C:\\Program Files\\Java\\bin\\java.exe\" -Xmx2G -jar server.jar");
        Assert.Equal(@"C:\Program Files\Java\bin\java.exe", file);
        Assert.Equal("-Xmx2G -jar server.jar", args);
    }

    [Fact]
    public void ServerProcessService_split_command_line_handles_unquoted_executable()
    {
        var (file, args) = ServerProcessService.SplitCommandLine("server.exe -port 27015");
        Assert.Equal("server.exe", file);
        Assert.Equal("-port 27015", args);
    }

    [Fact]
    public void ServerProcessService_split_command_line_handles_no_arguments()
    {
        var (file, args) = ServerProcessService.SplitCommandLine("server.exe");
        Assert.Equal("server.exe", file);
        Assert.Equal("", args);
    }
}
