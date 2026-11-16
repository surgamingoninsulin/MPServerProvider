using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;

namespace MPServerProvider.Core.Services;

/// <summary>
/// A decoded RCON packet. Port of the anonymous tuple the original's <c>ReadPacketAsync</c>
/// returns (a <c>(int Id, int Type, string Body)</c>-shaped record, per the decompile).
/// </summary>
public sealed record RconPacket(int Id, int PacketType, string Body)
{
    /// <summary>
    /// Port of <c>RconService.BuildPacket(int type, string body)</c>. Encodes
    /// <c>(size, id, type, body, "\0", "\0")</c> with <c>size</c> covering everything after the
    /// size field itself: <c>4 (id) + 4 (type) + body.Length + 1 (body NUL) + 1 (trailing NUL)</c>.
    /// </summary>
    public static byte[] Encode(int id, int packetType, string body)
    {
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        var payloadLen = 4 + 4 + bodyBytes.Length + 1 + 1;

        var output = new byte[4 + payloadLen];
        BinaryPrimitives.WriteInt32LittleEndian(output, payloadLen);
        BinaryPrimitives.WriteInt32LittleEndian(output.AsSpan(4), id);
        BinaryPrimitives.WriteInt32LittleEndian(output.AsSpan(8), packetType);
        bodyBytes.CopyTo(output, 12);
        // Trailing two NUL bytes are already zero by array initialization.
        return output;
    }

    /// <summary>
    /// Decodes a single packet's payload (everything *after* the leading size <c>int32</c>).
    /// </summary>
    public static RconPacket DecodePayload(ReadOnlySpan<byte> payload)
    {
        if (payload.Length < 10)
        {
            throw new FormatException("RCON packet shorter than id+type+terminators.");
        }

        var id = BinaryPrimitives.ReadInt32LittleEndian(payload);
        var packetType = BinaryPrimitives.ReadInt32LittleEndian(payload[4..]);

        if (payload[^1] != 0 || payload[^2] != 0)
        {
            throw new FormatException("RCON packet missing double NUL terminator.");
        }

        var bodyBytes = payload[8..^2];
        var body = Encoding.UTF8.GetString(bodyBytes);
        return new RconPacket(id, packetType, body);
    }

    /// <summary>Reads one full RCON packet (size prefix + payload) from <paramref name="stream"/>.</summary>
    public static async Task<RconPacket> ReadAsync(NetworkStream stream, CancellationToken ct = default)
    {
        var sizeBuf = new byte[4];
        await ReadExactAsync(stream, sizeBuf, ct).ConfigureAwait(false);
        var size = BinaryPrimitives.ReadInt32LittleEndian(sizeBuf);
        if (size < 10 || size > 4096 + 14)
        {
            throw new FormatException("Implausible RCON packet size.");
        }

        var payload = new byte[size];
        await ReadExactAsync(stream, payload, ct).ConfigureAwait(false);
        return DecodePayload(payload);
    }

    public static async Task WriteAsync(NetworkStream stream, int id, int packetType, string body, CancellationToken ct = default)
    {
        var bytes = Encode(id, packetType, body);
        await stream.WriteAsync(bytes, ct).ConfigureAwait(false);
    }

    private static async Task ReadExactAsync(Stream stream, byte[] buffer, CancellationToken ct)
    {
        var offset = 0;
        while (offset < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(offset), ct).ConfigureAwait(false);
            if (read == 0)
            {
                throw new EndOfStreamException("Connection closed before a full RCON packet was read.");
            }
            offset += read;
        }
    }
}

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.RconService</c> (decompile token 0x02000012,
/// see <c>docs/decompile-report/report/MPServerProvider.Core.dll/MPServerProvider.Core.Services/
/// 0x02000012_RconService.md</c>): the Source Engine RCON protocol
/// (<see href="https://developer.valvesoftware.com/wiki/Source_RCON_Protocol"/>), implemented to
/// spec since it's externally standardized. The decompile confirms the shape: a
/// <c>TcpClient</c>/<c>Stream</c> pair, a <c>BuildPacket(int type, string body)</c> helper using
/// a little-endian <c>BinaryWriter</c>, and <c>ReadPacketAsync</c>/<c>ConnectAsync</c>/
/// <c>SendCommandAsync</c>/<c>SendCommandMultiAsync</c>.
/// </summary>
public sealed class RconService : IDisposable
{
    /// <summary><c>SERVERDATA_AUTH</c> — client -&gt; server, authenticate with the RCON password.</summary>
    public const int ServerDataAuth = 3;
    /// <summary><c>SERVERDATA_EXECCOMMAND</c> — client -&gt; server, run a console command.</summary>
    public const int ServerDataExecCommand = 2;
    /// <summary><c>SERVERDATA_AUTH_RESPONSE</c> — server -&gt; client, reply to an auth packet.</summary>
    public const int ServerDataAuthResponse = 2;
    /// <summary><c>SERVERDATA_RESPONSE_VALUE</c> — server -&gt; client, command output.</summary>
    public const int ServerDataResponseValue = 0;

    private TcpClient? _client;
    private NetworkStream? _stream;
    private int _nextRequestId = 1;

    /// <summary>Port of <c>RconService.get_Connected()</c>.</summary>
    public bool Connected { get; private set; }

    private int AllocId()
    {
        var id = _nextRequestId;
        _nextRequestId = Math.Max(1, unchecked(_nextRequestId + 1));
        return id;
    }

    /// <summary>
    /// Port of <c>RconService.ConnectAsync(string host, int port, string password, ...)</c>:
    /// opens the TCP connection, sends <c>SERVERDATA_AUTH</c>, and verifies the
    /// <c>SERVERDATA_AUTH_RESPONSE</c> echoes the same request id (a response with
    /// <c>id == -1</c> means the password was rejected, per spec). Skips any empty
    /// <c>SERVERDATA_RESPONSE_VALUE</c> some Source RCON implementations send before the real
    /// auth response.
    /// </summary>
    /// <remarks>
    /// HARDENING: any previous connection is disposed first; the whole handshake (connect, auth
    /// write and auth reply) is bounded by <paramref name="connectTimeout"/> (a silent server used
    /// to hang the auth read forever); and every failure path disposes the socket.
    /// </remarks>
    public async Task ConnectAsync(string host, int port, string password, TimeSpan connectTimeout, CancellationToken ct = default)
    {
        Disconnect();

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(connectTimeout);

        var client = new TcpClient();
        try
        {
            await client.ConnectAsync(host, port, timeoutCts.Token).ConfigureAwait(false);

            _client = client;
            _stream = client.GetStream();
            Connected = false;

            var authId = AllocId();
            await RconPacket.WriteAsync(_stream, authId, ServerDataAuth, password, timeoutCts.Token).ConfigureAwait(false);

            while (true)
            {
                var packet = await RconPacket.ReadAsync(_stream, timeoutCts.Token).ConfigureAwait(false);
                if (packet.PacketType == ServerDataAuthResponse)
                {
                    if (packet.Id == authId)
                    {
                        Connected = true;
                        return;
                    }

                    throw new InvalidOperationException("RCON authentication failed.");
                }
            }
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            client.Dispose();
            Disconnect();
            throw new TimeoutException("Timed out connecting to the RCON server.");
        }
        catch
        {
            client.Dispose();
            Disconnect();
            throw;
        }
    }

    /// <summary>
    /// Reads packets until one answering request <paramref name="id"/> arrives, discarding late
    /// replies to earlier (timed-out) commands so they are never reported as this command's output.
    /// </summary>
    private static async Task<RconPacket> ReadResponseAsync(NetworkStream stream, int id, CancellationToken ct)
    {
        while (true)
        {
            var packet = await RconPacket.ReadAsync(stream, ct).ConfigureAwait(false);
            if (packet.Id == id)
            {
                return packet;
            }
        }
    }

    /// <summary>Port of <c>RconService.SendCommandAsync(string command, ...)</c>.</summary>
    public async Task<string> SendCommandAsync(string command, TimeSpan responseTimeout, CancellationToken ct = default)
    {
        if (!Connected || _stream is null)
        {
            throw new InvalidOperationException("Not connected.");
        }

        var id = AllocId();
        var stream = _stream;

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(responseTimeout);
        try
        {
            await RconPacket.WriteAsync(stream, id, ServerDataExecCommand, command, timeoutCts.Token).ConfigureAwait(false);
            var packet = await ReadResponseAsync(stream, id, timeoutCts.Token).ConfigureAwait(false);
            return packet.Body;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            // A half-read packet leaves the stream unframed: drop the connection.
            Disconnect();
            throw new TimeoutException("Timed out waiting for an RCON response.");
        }
        catch (Exception ex) when (ex is IOException or SocketException or FormatException or ObjectDisposedException or OperationCanceledException)
        {
            Disconnect();
            throw;
        }
    }

    /// <summary>
    /// Port of <c>RconService.SendCommandMultiAsync(string command, TimeSpan[] delays, ...)</c>:
    /// sends the command once, then waits for *multiple* response packets (some servers
    /// fragment long responses across several <c>SERVERDATA_RESPONSE_VALUE</c> packets), waiting
    /// up to each entry in <paramref name="delays"/> between reads and concatenating everything
    /// received.
    /// </summary>
    public async Task<string> SendCommandMultiAsync(string command, IReadOnlyList<TimeSpan> delays, CancellationToken ct = default)
    {
        if (!Connected || _stream is null)
        {
            throw new InvalidOperationException("Not connected.");
        }

        var id = AllocId();
        var stream = _stream;
        try
        {
            await RconPacket.WriteAsync(stream, id, ServerDataExecCommand, command, ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException)
        {
            Disconnect();
            throw;
        }

        var combined = new StringBuilder();
        foreach (var delay in delays)
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(delay);
            try
            {
                var packet = await ReadResponseAsync(stream, id, timeoutCts.Token).ConfigureAwait(false);
                combined.Append(packet.Body);
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                // No more data within this delay window (the normal end of a fragmented reply);
                // replies arriving later are skipped by id on the next command.
                break;
            }
            catch (Exception ex) when (ex is IOException or SocketException or FormatException or ObjectDisposedException)
            {
                Disconnect();
                throw;
            }
        }
        return combined.ToString();
    }

    /// <summary>Port of <c>RconService.Disconnect()</c>.</summary>
    public void Disconnect()
    {
        Connected = false;
        _stream?.Dispose();
        _stream = null;
        _client?.Dispose();
        _client = null;
    }

    public void Dispose() => Disconnect();
}
