using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class PlayitPortCheckTests
{
    [Theory]
    [InlineData("127.0.0.1:25565", 25565)]
    [InlineData("localhost:25566", 25566)]
    [InlineData("127.0.0.1:19132 (disabled: over limit)", 19132)]
    [InlineData("nowhere", null)]
    public void Reads_destination_port(string destination, int? expected)
        => Assert.Equal(expected, new PlayitTunnel("x.joinmc.link", destination).DestinationPort);

    [Fact]
    public void Tunnel_on_default_port_while_server_moved_suggests_that_port()
    {
        var check = PlayitPortCheck.For(new[] { new PlayitTunnel("abc.joinmc.link", "127.0.0.1:25565") }, serverPort: 25566);

        Assert.Empty(check.Matching);
        Assert.Equal(25565, check.SuggestedPort);
    }

    [Fact]
    public void Matching_tunnel_needs_no_fix()
    {
        var check = PlayitPortCheck.For(new[]
        {
            new PlayitTunnel("abc.joinmc.link", "127.0.0.1:25566"),
            new PlayitTunnel("other.gl.at.ply.gg:1234", "127.0.0.1:30120"),
        }, serverPort: 25566);

        Assert.Single(check.Matching);
        Assert.Null(check.SuggestedPort);
    }
}
