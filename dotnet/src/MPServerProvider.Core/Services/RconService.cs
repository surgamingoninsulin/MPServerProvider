using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text;

namespace MPServerProvider.Core.Services;

public sealed record RconPacket(int Id, int PacketType, string Body)
{
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

public sealed class RconService : IDisposable
{
    public const int ServerDataAuth = 3;
    public const int ServerDataExecCommand = 2;
    public const int ServerDataAuthResponse = 2;
    public const int ServerDataResponseValue = 0;

    private TcpClient? _client;
    private NetworkStream? _stream;
    private int _nextRequestId = 1;

    public bool Connected { get; private set; }

    private int AllocId()
    {
        var id = _nextRequestId;
        _nextRequestId = Math.Max(1, unchecked(_nextRequestId + 1));
        return id;
    }

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
