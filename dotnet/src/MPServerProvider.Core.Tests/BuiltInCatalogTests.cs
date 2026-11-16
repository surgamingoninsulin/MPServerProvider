using MPServerProvider.Core.Models;
using MPServerProvider.Core.Providers;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class BuiltInCatalogTests
{
    private static readonly string[] KnownProviders = { "steamcmd", "url", "manual", "cfx", "minecraft-java", "minecraft-bedrock", "beammp", "hytale" };

    public static IEnumerable<object[]> BuiltInIds()
        => BuiltInGames.All().Select(g => new object[] { g.Id });

    [Theory]
    [MemberData(nameof(BuiltInIds))]
    public void Definition_is_complete_and_launchable(string id)
    {
        var game = BuiltInGames.All().Single(g => g.Id == id);

        Assert.Matches("^[a-z0-9]+(-[a-z0-9]+)*$", game.Id);
        Assert.False(string.IsNullOrWhiteSpace(game.Name));
        Assert.False(string.IsNullOrWhiteSpace(game.Description));
        Assert.NotEmpty(game.Genres);
        Assert.Contains(game.Install.Provider, KnownProviders);
        Assert.InRange(game.Network.DefaultPort, 1, 65535);
        Assert.Contains(game.Network.Protocol, new[] { "udp", "tcp", "both" });
        Assert.Contains(game.Network.ConsoleMethod, new[] { "stdin", "logfile" });
        Assert.StartsWith("{server_root}", game.Runtime.WorkingDir);

        if (game.Install.Provider == "steamcmd")
        {
            Assert.True(game.Install.AppId > 0, id);
            Assert.Null(game.Install.Username);
            Assert.Null(game.Install.Password);
            // Throws for a branch SteamCMD can't take.
            SteamCmdProvider.BranchName(game.Install);
            // CryoFall's server is a framework-dependent .NET app with no .exe of its own.
            if (game.Runtime.Executable == "{dotnet}")
            {
                Assert.Contains("{server_root}\\", game.Runtime.Arguments);
                return;
            }

            Assert.StartsWith("{server_root}\\", game.Runtime.Executable);
            Assert.True(
                game.Runtime.Executable.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                || game.Runtime.Executable.EndsWith(".bat", StringComparison.OrdinalIgnoreCase),
                $"{id}: {game.Runtime.Executable}");
        }

        if (game.Artwork.Cover is { } cover)
        {
            Assert.True(cover.StartsWith("https://", StringComparison.Ordinal) || cover.StartsWith("asset:", StringComparison.Ordinal), $"{id}: {cover}");
        }
    }

    [Fact]
    public void Ids_and_names_are_unique()
    {
        var games = BuiltInGames.All();
        Assert.Equal(games.Count, games.Select(g => g.Id).Distinct().Count());
        Assert.Equal(games.Count, games.Select(g => g.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("public", null)]
    [InlineData(" public ", null)]
    [InlineData("experimental", "experimental")]
    [InlineData("win32_beta", "win32_beta")]
    [InlineData("1.2-preview", "1.2-preview")]
    public void Branch_name_is_normalized(string? branch, string? expected)
        => Assert.Equal(expected, SteamCmdProvider.BranchName(new InstallSpec { Branch = branch }));

    [Theory]
    [InlineData("beta validate +quit")]
    [InlineData("x\"y")]
    [InlineData("a;b")]
    public void Branch_name_with_unsafe_characters_is_refused(string branch)
        => Assert.Throws<InvalidOperationException>(() => SteamCmdProvider.BranchName(new InstallSpec { Branch = branch }));
}
