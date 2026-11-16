using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Result of a successful server query. Mirrors the original's
/// <c>(int Players, int MaxPlayers)?</c> return shape (a <c>ValueTuple&lt;int,int&gt;?</c> in
/// the decompile).
/// </summary>
public readonly record struct QueryResult(int Players, int MaxPlayers);

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.QueryService</c> (decompile token 0x02000011,
/// see <c>docs/decompile-report/report/MPServerProvider.Core.dll/MPServerProvider.Core.Services/
/// 0x02000011_QueryService.md</c>).
///
/// <para>
/// Decompile evidence confirms the dispatcher shape (<c>QueryAsync(game, host, port)</c>
/// fanning out to <c>QueryA2SAsync</c>/<c>QueryMinecraftJavaAsync</c>/<c>QueryBedrockAsync</c>/
/// <c>QueryFiveMAsync</c>), a <c>RakNetMagic</c> static byte array field (confirming a real
/// RakNet unconnected-ping for Bedrock), and <c>BuildA2SInfo</c> which prepends the literal
/// ASCII string <c>"Source Engine Query\0"</c> to the <c>0xFFFFFFFF</c> prefix + <c>0x54</c>
/// (<c>'T'</c>) request byte — the standard A2S_INFO request. <c>WriteVarInt</c>/
/// <c>ReadVarIntAsync</c> confirm a real Minecraft varint-prefixed packet framer.
/// </para>
/// <para>
/// Each protocol below is implemented to its public specification (A2S, Minecraft Java Server
/// List Ping, RakNet unconnected ping for Bedrock, and the FiveM/txAdmin <c>getinfo</c>
/// out-of-band UDP query) rather than guessed, since all four are externally standardized — the
/// same approach the already-verified Rust port took
/// (<c>crates/mpsp-core/src/services/query.rs</c>).
/// </para>
/// </summary>
public static class QueryService
{
    /// <summary>
    /// Default per-attempt timeout, matching the original's static <c>Timeout</c> field.
    /// INFERRED exact duration: the literal wasn't recoverable from metadata; 5s is the
    /// conventional query timeout.
    /// </summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// RakNet's fixed "offline message data ID", used to mark unconnected (pre-handshake)
    /// packets. Matches the original's <c>RakNetMagic</c> static field.
    /// </summary>
    public static readonly byte[] RakNetMagic =
    {
        0x00, 0xff, 0xff, 0x00, 0xfe, 0xfe, 0xfe, 0xfe, 0xfd, 0xfd, 0xfd, 0xfd, 0x12, 0x34, 0x56, 0x78,
    };

    /// <summary>
    /// Dispatcher. Port of <c>QueryService.QueryAsync(string game, string host, int port)</c>.
    /// <paramref name="kind"/> is matched case-insensitively against well-known query-kind
    /// keywords (the original's actual dispatch presumably keys off
    /// <c>GameDefinition.Network.Query</c> — kept as a plain string here so this module stays
    /// decoupled from catalog-loading).
    /// </summary>
    public static Task<QueryResult?> QueryAsync(string kind, string host, int port, CancellationToken ct = default)
    {
        return kind.ToLowerInvariant() switch
        {
            "a2s" or "source" or "steam" => QueryA2SAsync(host, port, ct),
            // The shipped game definitions use "minecraft-java"/"minecraft-bedrock" (dash), which
            // previously fell through to NotSupportedException so player counts were always 0.
            "minecraft" or "minecraft_java" or "minecraft-java" or "java" => QueryMinecraftJavaAsync(host, port, ct),
            "bedrock" or "minecraft_bedrock" or "minecraft-bedrock" => QueryBedrockAsync(host, port, ct),
            "fivem" or "cfx" or "txadmin" => QueryFiveMAsync(host, port, ct),
            _ => throw new NotSupportedException($"Unsupported game query kind: {kind}"),
        };
    }

    /// <summary>
    /// Builds the A2S_INFO request payload. Port of <c>QueryService.BuildA2SInfo(byte[])</c>:
    /// <c>0xFF 0xFF 0xFF 0xFF</c> (the UDP out-of-band prefix every A2S packet uses) + <c>0x54</c>
    /// (<c>'T'</c>, the A2S_INFO request type) + the literal ASCII string
    /// <c>"Source Engine Query\0"</c>.
    /// </summary>
    public static byte[] BuildA2SInfoRequest()
    {
        var buf = new List<byte> { 0xFF, 0xFF, 0xFF, 0xFF, 0x54 };
        buf.AddRange(Encoding.ASCII.GetBytes("Source Engine Query\0"));
        return buf.ToArray();
    }

    /// <summary>
    /// Port of <c>QueryService.QueryA2SAsync(string host, int port)</c>. Sends A2S_INFO; if the
    /// server replies with an S2C_CHALLENGE (<c>0x41</c>) instead of an A2S_INFO response
    /// (<c>0x49</c>), re-sends the request with the 4-byte challenge appended, per the
    /// documented A2S challenge handshake required by all post-2020 Source servers.
    /// </summary>
    public static async Task<QueryResult?> QueryA2SAsync(string host, int port, CancellationToken ct = default)
    {
        using var client = new UdpClient();
        client.Connect(host, port);

        var request = BuildA2SInfoRequest();
        await client.SendAsync(request, ct).ConfigureAwait(false);

        var response = await ReceiveWithTimeoutAsync(client, ct).ConfigureAwait(false);

        if (response.Length >= 9 && response[4] == 0x41)
        {
            var challenge = response[5..9];
            var retry = request.Concat(challenge).ToArray();
            await client.SendAsync(retry, ct).ConfigureAwait(false);
            response = await ReceiveWithTimeoutAsync(client, ct).ConfigureAwait(false);
        }

        return ParseA2SInfoResponse(response);
    }

    /// <summary>
    /// Parses an A2S_INFO response body (including the <c>0xFFFFFFFF</c> prefix and <c>0x49</c>
    /// type byte) per the published format:
    /// <c>header(4) | type(1)=0x49 | protocol(1) | name(cstr) | map(cstr) | folder(cstr) |
    /// game(cstr) | app_id(i16 LE) | players(u8) | max_players(u8) | bots(u8) | ...</c>.
    /// </summary>
    internal static QueryResult? ParseA2SInfoResponse(byte[] data)
    {
        if (data.Length < 6 || data[4] != 0x49)
        {
            throw new FormatException("Not an A2S_INFO response.");
        }

        var i = 6; // skip header(4) + type(1) + protocol(1)
        for (var field = 0; field < 4; field++)
        {
            while (i < data.Length && data[i] != 0)
            {
                i++;
            }
            if (i >= data.Length)
            {
                throw new FormatException("Truncated string field in A2S_INFO response.");
            }
            i++; // skip NUL
        }
        i += 2; // app id (i16 LE)

        if (i + 2 > data.Length)
        {
            throw new FormatException("Truncated player-count fields in A2S_INFO response.");
        }

        return new QueryResult(data[i], data[i + 1]);
    }

    // ---------------------------------------------------------------------------------------
    // Minecraft Java Server List Ping (varint-framed)
    // ---------------------------------------------------------------------------------------

    /// <summary>Port of <c>QueryService.WriteVarInt(Stream, int)</c>: standard protobuf-style LEB128 varint.</summary>
    public static void WriteVarInt(List<byte> output, int value)
    {
        var unsigned = unchecked((uint)value);
        while (true)
        {
            var b = (byte)(unsigned & 0x7F);
            unsigned >>= 7;
            if (unsigned != 0)
            {
                b |= 0x80;
            }
            output.Add(b);
            if (unsigned == 0)
            {
                break;
            }
        }
    }

    /// <summary>Port of <c>QueryService.ReadVarIntAsync(Stream, CancellationToken)</c>.</summary>
    public static (int Value, int Consumed) ReadVarInt(ReadOnlySpan<byte> data)
    {
        var value = 0;
        var position = 0;
        var consumed = 0;
        while (true)
        {
            if (consumed >= data.Length)
            {
                throw new FormatException("Truncated varint.");
            }
            var b = data[consumed];
            consumed++;
            value |= (b & 0x7F) << position;
            if ((b & 0x80) == 0)
            {
                break;
            }
            position += 7;
            if (position >= 32)
            {
                throw new FormatException("Varint too long.");
            }
        }
        return (value, consumed);
    }

    private static void WriteMcString(List<byte> output, string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        WriteVarInt(output, bytes.Length);
        output.AddRange(bytes);
    }

    /// <summary>
    /// Builds a length-prefixed Minecraft packet: <c>varint(len) | varint(packet_id) | data...</c>.
    /// Port of <c>QueryService.WritePacketAsync(Stream, byte[], CancellationToken)</c>.
    /// </summary>
    public static byte[] BuildMcPacket(int packetId, byte[] payload)
    {
        var body = new List<byte>();
        WriteVarInt(body, packetId);
        body.AddRange(payload);

        var framed = new List<byte>();
        WriteVarInt(framed, body.Count);
        framed.AddRange(body);
        return framed.ToArray();
    }

    /// <summary>
    /// Port of <c>QueryService.QueryMinecraftJavaAsync(string host, int port)</c>: the standard
    /// Server List Ping handshake (Handshake packet with state=1 "status", then an empty Status
    /// Request packet), reading the Status Response packet and pulling
    /// <c>players.online</c>/<c>players.max</c> from its JSON body.
    /// </summary>
    public static async Task<QueryResult?> QueryMinecraftJavaAsync(string host, int port, CancellationToken ct = default)
    {
        using var client = new TcpClient();
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(DefaultTimeout);
        await client.ConnectAsync(host, port, timeoutCts.Token).ConfigureAwait(false);

        var stream = client.GetStream();

        var handshakePayload = new List<byte>();
        WriteVarInt(handshakePayload, -1); // protocol version: -1 = "don't care"/ping
        WriteMcString(handshakePayload, host);
        var portBytes = new byte[2];
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16BigEndian(portBytes, (ushort)port);
        handshakePayload.AddRange(portBytes);
        WriteVarInt(handshakePayload, 1);
        var handshake = BuildMcPacket(0x00, handshakePayload.ToArray());

        var statusRequest = BuildMcPacket(0x00, Array.Empty<byte>());

        await stream.WriteAsync(handshake, timeoutCts.Token).ConfigureAwait(false);
        await stream.WriteAsync(statusRequest, timeoutCts.Token).ConfigureAwait(false);

        var lengthBytes = new List<byte>();
        var oneByte = new byte[1];
        do
        {
            await ReadExactAsync(stream, oneByte, timeoutCts.Token).ConfigureAwait(false);
            lengthBytes.Add(oneByte[0]);
            if (lengthBytes.Count > 5)
            {
                throw new FormatException("Varint too long.");
            }
        } while ((oneByte[0] & 0x80) != 0);

        var (packetLength, _) = ReadVarInt(lengthBytes.ToArray());
        ValidatePacketLength(packetLength);
        var body = new byte[packetLength];
        await ReadExactAsync(stream, body, timeoutCts.Token).ConfigureAwait(false);

        var (_, idLen) = ReadVarInt(body);
        var (jsonLen, jsonLenBytes) = ReadVarInt(body.AsSpan(idLen));
        var jsonStart = idLen + jsonLenBytes;
        if (jsonLen < 0 || (long)jsonStart + jsonLen > body.Length)
        {
            throw new FormatException("Truncated status JSON.");
        }

        var jsonEnd = jsonStart + jsonLen;

        var json = Encoding.UTF8.GetString(body, jsonStart, jsonEnd - jsonStart);

        using var document = JsonDocument.Parse(json);
        var hasPlayers = document.RootElement.TryGetProperty("players", out var playersEl) && playersEl.ValueKind == JsonValueKind.Object;
        var online = hasPlayers && playersEl.TryGetProperty("online", out var onlineEl) ? onlineEl.GetInt32() : 0;
        var max = hasPlayers && playersEl.TryGetProperty("max", out var maxEl) ? maxEl.GetInt32() : 0;

        return new QueryResult(online, max);
    }

    /// <summary>Largest Minecraft status packet accepted (real ones are a few KB, favicon included).</summary>
    public const int MaxStatusPacketLength = 1 << 20;

    /// <summary>
    /// Rejects a server-supplied packet length that is non-positive or larger than
    /// <see cref="MaxStatusPacketLength"/>, so a broken/malicious server can't make the app
    /// allocate gigabytes (or throw an <see cref="OverflowException"/> for a negative length).
    /// </summary>
    internal static void ValidatePacketLength(int packetLength)
    {
        if (packetLength is <= 0 or > MaxStatusPacketLength)
        {
            throw new FormatException($"Implausible status packet length {packetLength}.");
        }
    }

    private static async Task ReadExactAsync(Stream stream, byte[] buffer, CancellationToken ct)
    {
        var offset = 0;
        while (offset < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(offset), ct).ConfigureAwait(false);
            if (read == 0)
            {
                throw new EndOfStreamException("Connection closed before expected data was read.");
            }
            offset += read;
        }
    }

    // ---------------------------------------------------------------------------------------
    // Minecraft Bedrock (RakNet unconnected ping)
    // ---------------------------------------------------------------------------------------

    /// <summary>
    /// Builds a RakNet <c>ID_UNCONNECTED_PING</c> packet: <c>0x01 | timestamp(i64 BE) |
    /// magic(16 bytes) | client_guid(i64 BE)</c>.
    /// </summary>
    public static byte[] BuildUnconnectedPing(long timestamp, long clientGuid)
    {
        var buf = new List<byte>(1 + 8 + 16 + 8) { 0x01 };
        buf.AddRange(BigEndianBytes(timestamp));
        buf.AddRange(RakNetMagic);
        buf.AddRange(BigEndianBytes(clientGuid));
        return buf.ToArray();
    }

    private static byte[] BigEndianBytes(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return bytes;
    }

    /// <summary>
    /// Port of <c>QueryService.QueryBedrockAsync(string host, int port)</c>. Bedrock's
    /// <c>ID_UNCONNECTED_PONG</c> response carries a semicolon-separated "extended server info"
    /// string whose 5th/6th fields (0-indexed 4/5) are current/max player counts, per the
    /// de-facto Bedrock server status format
    /// (<c>MCPE;name;protocol;version;players;maxPlayers;...</c>).
    /// </summary>
    public static async Task<QueryResult?> QueryBedrockAsync(string host, int port, CancellationToken ct = default)
    {
        using var client = new UdpClient();
        client.Connect(host, port);

        var ping = BuildUnconnectedPing(0, unchecked((long)0x123456789abcdef0UL));
        await client.SendAsync(ping, ct).ConfigureAwait(false);

        var response = await ReceiveWithTimeoutAsync(client, ct).ConfigureAwait(false);
        return ParseUnconnectedPong(response);
    }

    internal static QueryResult? ParseUnconnectedPong(byte[] data)
    {
        if (data.Length == 0 || data[0] != 0x1c)
        {
            throw new FormatException("Not an ID_UNCONNECTED_PONG packet.");
        }

        // header(1) + timestamp(8) + server_guid(8) + magic(16) + string_len(u16 BE) + string.
        const int strLenOffset = 1 + 8 + 8 + 16;
        if (data.Length < strLenOffset + 2)
        {
            throw new FormatException("Truncated pong header.");
        }

        var strLen = (data[strLenOffset] << 8) | data[strLenOffset + 1];
        var strStart = strLenOffset + 2;
        if (strStart + strLen > data.Length)
        {
            throw new FormatException("Truncated pong payload.");
        }

        var payload = Encoding.UTF8.GetString(data, strStart, strLen);
        var fields = payload.Split(';');
        if (fields.Length < 6)
        {
            throw new FormatException("Too few MCPE; fields.");
        }

        _ = int.TryParse(fields[4], out var players);
        _ = int.TryParse(fields[5], out var maxPlayers);
        return new QueryResult(players, maxPlayers);
    }

    // ---------------------------------------------------------------------------------------
    // FiveM / txAdmin
    // ---------------------------------------------------------------------------------------

    /// <summary>
    /// Port of <c>QueryService.QueryFiveMAsync(string host, int port)</c>. FiveM servers answer
    /// the GameSpy-derived out-of-band UDP query <c>"\xFF\xFF\xFF\xFFgetinfo xxx"</c> with
    /// <c>"\xFF\xFF\xFF\xFFinfoResponse\n"</c> followed by a flat <c>\x01key\x00value\x00...</c>
    /// dictionary that includes <c>clients</c>/<c>sv_maxclients</c> — the same query documented
    /// by the CitizenFX/FiveM server query protocol.
    /// </summary>
    public static async Task<QueryResult?> QueryFiveMAsync(string host, int port, CancellationToken ct = default)
    {
        using var client = new UdpClient();
        client.Connect(host, port);

        var request = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }.Concat(Encoding.ASCII.GetBytes("getinfo xxx")).ToArray();
        await client.SendAsync(request, ct).ConfigureAwait(false);

        var response = await ReceiveWithTimeoutAsync(client, ct).ConfigureAwait(false);
        return ParseFiveMInfoResponse(response);
    }

    internal static QueryResult? ParseFiveMInfoResponse(byte[] data)
    {
        var text = Encoding.UTF8.GetString(data);
        const string marker = "infoResponse\n";
        var idx = text.IndexOf(marker, StringComparison.Ordinal);
        if (idx >= 0)
        {
            text = text[(idx + marker.Length)..];
        }

        // Body is NUL-delimited `\x01key\x00value\x00...` pairs.
        var parts = text.Split('\u0000');
        var map = new Dictionary<string, string>();
        string? currentKey = null;
        foreach (var part in parts)
        {
            var cleaned = part.TrimStart('\u0001');
            if (currentKey is null)
            {
                currentKey = cleaned;
            }
            else
            {
                map[currentKey] = cleaned;
                currentKey = null;
            }
        }

        var players = map.TryGetValue("clients", out var clientsStr) && int.TryParse(clientsStr, out var clients) ? clients : 0;
        var maxPlayers = map.TryGetValue("sv_maxclients", out var maxStr) && int.TryParse(maxStr, out var max) ? max : 0;
        return new QueryResult(players, maxPlayers);
    }

    private static async Task<byte[]> ReceiveWithTimeoutAsync(UdpClient client, CancellationToken ct)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(DefaultTimeout);
        try
        {
            var result = await client.ReceiveAsync(timeoutCts.Token).ConfigureAwait(false);
            return result.Buffer;
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Timed out waiting for a query response.");
        }
    }
}
