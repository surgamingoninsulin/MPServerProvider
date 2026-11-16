using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class ServerUpdateCheckTests
{
    [Theory]
    [InlineData("26.2 (build 120)", "26.3")]
    [InlineData("1.21.10", "26.3")]
    [InlineData("26.2-66.0.1", "26.3")]
    [InlineData("26.3 (build 252)", null)]
    [InlineData("26.3", null)]
    [InlineData("latest", null)]
    public void Minecraft_compares_with_newest(string installed, string? expected)
        => Assert.Equal(expected, ServerUpdateCheck.NewerMinecraftVersion(installed, new[] { "26.3", "26.2", "1.21.11" }));

    [Fact]
    public void Velocity_snapshot_versions_compare()
    {
        Assert.Equal("4.3.2-SNAPSHOT", ServerUpdateCheck.NewerMinecraftVersion("4.3.1", new[] { "4.3.2-SNAPSHOT", "4.3.1" }));
        Assert.Null(ServerUpdateCheck.NewerMinecraftVersion("4.3.2-SNAPSHOT", new[] { "4.3.2-SNAPSHOT", "4.3.1" }));
    }

    [Theory]
    [InlineData("build 38386", "build 38435")]
    [InlineData("build 38435", null)]
    [InlineData("build 38621", null)]
    public void Cfx_compares_with_recommended(string installed, string? expected)
        => Assert.Equal(expected, ServerUpdateCheck.NewerCfxBuild(installed,
            new[] { "build 38435 (recommended)", "build 38621 (latest)", "build 7290 (optional, critical)", "build 38386" }));
}
