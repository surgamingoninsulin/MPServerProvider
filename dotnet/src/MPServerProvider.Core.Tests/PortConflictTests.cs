using System.Net;
using System.Net.Sockets;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class PortConflictTests
{
    [Fact]
    public void Detects_a_listening_tcp_port_and_names_its_owner()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;

        Assert.True(ServerProcessService.IsPortInUse(port, tcp: true, udp: false));
        Assert.Equal(System.Diagnostics.Process.GetCurrentProcess().ProcessName, PortOwner.FindTcpListener(port));

        listener.Stop();
        Assert.False(ServerProcessService.IsPortInUse(port, tcp: true, udp: false));
    }

    [Fact]
    public void Detects_a_bound_udp_port()
    {
        using var socket = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
        var port = ((IPEndPoint)socket.Client.LocalEndPoint!).Port;

        Assert.True(ServerProcessService.IsPortInUse(port, tcp: false, udp: true));
    }
}
