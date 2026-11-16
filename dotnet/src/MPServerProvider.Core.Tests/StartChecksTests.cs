using System.Net;
using System.Net.Sockets;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class StartChecksTests
{
    [Theory]
    [InlineData("\u001b[31mUnable to open 'park.park'\u001b[0m", "Unable to open 'park.park'")]
    [InlineData("Building object index (2517 items)\u001b[0m", "Building object index (2517 items)")]
    [InlineData("\u001b[1;33m[WARN]\u001b[m done", "[WARN] done")]
    [InlineData("\u001b]0;FXServer\u0007ready", "ready")]
    [InlineData("plain [31m text stays", "plain [31m text stays")]
    [InlineData("tab\tkept", "tab\tkept")]
    public void Strips_terminal_codes(string input, string expected)
        => Assert.Equal(expected, ServerProcessService.StripTerminalCodes(input));

    [Fact]
    public void Syncs_server_port_into_server_properties()
    {
        var root = Directory.CreateTempSubdirectory("mpsp-props").FullName;
        try
        {
            File.WriteAllLines(Path.Combine(root, "server.properties"), new[] { "motd=hi", "server-port=25565", "level-name=world" });
            var server = new ServerInstance { ServerName = "s", ServerRoot = root, ServerPort = 25570 };

            ServerProcessService.SyncPropertiesPort(server);

            Assert.Equal(new[] { "motd=hi", "server-port=25570", "level-name=world" }, File.ReadAllLines(Path.Combine(root, "server.properties")));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Missing_park_file_is_explained()
    {
        var root = Directory.CreateTempSubdirectory("mpsp-park").FullName;
        try
        {
            var args = $"host \"{Path.Combine(root, "park.park")}\" --port 11753 --headless";
            Assert.Contains("park.park", ServerProcessService.DescribeMissingSaveFile(args, root));

            File.WriteAllText(Path.Combine(root, "park.park"), "x");
            Assert.Null(ServerProcessService.DescribeMissingSaveFile(args, root));
            Assert.Null(ServerProcessService.DescribeMissingSaveFile("-Xmx2G -jar \"server.jar\" nogui", root));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Suggests_next_port_when_default_is_taken()
    {
        using var listener = new TcpListener(IPAddress.Any, 0);
        listener.Start();
        var busy = ((IPEndPoint)listener.LocalEndpoint).Port;

        var suggested = ServerProcessService.SuggestFreePort(busy, "tcp", new[] { busy + 1 });

        Assert.True(suggested > busy + 1);
        Assert.Equal(busy + 2 <= 65535 && !ServerProcessService.IsPortInUse(busy + 2, true, false) ? busy + 2 : suggested, suggested);
    }
}
