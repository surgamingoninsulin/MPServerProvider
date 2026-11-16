using MPServerProvider.Core.Models;
using MPServerProvider.Core.Providers;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class SteamLoginTests
{
    private static GameDefinition Game(string id) => BuiltInGames.All().Single(g => g.Id == id);

    private static InstallRequest Request(string id, SteamLogin? login = null)
        => new() { Game = Game(id), SteamLogin = login };

    [Fact]
    public void Only_games_whose_server_isnt_free_ask_for_a_login()
    {
        Assert.True(SteamCmdProvider.NeedsSteamLogin(Game("dayz")));
        Assert.True(SteamCmdProvider.NeedsSteamLogin(Game("starbound")));
        Assert.False(SteamCmdProvider.NeedsSteamLogin(Game("dayz-experimental")));
        Assert.False(SteamCmdProvider.NeedsSteamLogin(Game("valheim")));
        Assert.False(SteamCmdProvider.NeedsSteamLogin(Game("hytale"))); // manual install, not SteamCMD
    }

    [Fact]
    public void Typed_login_line_includes_guard_code_only_when_given()
    {
        Assert.Equal("login \"me\" \"pw\"", SteamCmdProvider.TypedLoginLine(new SteamLogin(" me ", "pw")));
        Assert.Equal("login \"me\" \"pw\" \"AB12C\"", SteamCmdProvider.TypedLoginLine(new SteamLogin("me", "pw", "AB12C")));
    }

    [Fact]
    public void Typed_login_with_a_quote_is_refused()
        => Assert.Throws<InvalidOperationException>(() => SteamCmdProvider.TypedLoginLine(new SteamLogin("me", "p\"w")));

    [Fact]
    public void Password_never_shows_in_ToString()
        => Assert.DoesNotContain("secret", new SteamLogin("me", "secret", "CODE1").ToString());

    [Theory]
    [InlineData("FAILED (Invalid Password)", "wrong")]
    [InlineData("FAILED (Two-factor code mismatch)", "Steam Guard code")]
    [InlineData("This computer has not been authenticated for your account using Steam Guard.", "Steam Guard code")]
    [InlineData("FAILED (Rate Limit Exceeded)", "too many attempts")]
    [InlineData("ERROR! Failed to install app '223350' (No subscription)", "doesn't own DayZ")]
    public void Login_failures_get_a_plain_english_reason(string output, string expected)
        => Assert.Contains(expected, SteamCmdProvider.LoginProblem(output, Request("dayz", new SteamLogin("me", "pw"))));

    [Fact]
    public void Missing_cached_login_asks_for_the_password_again()
        => Assert.Contains("password again", SteamCmdProvider.LoginProblem("Cached credentials not found.", Request("dayz", new SteamLogin("me"))));

    [Fact]
    public void Unrelated_failure_is_not_blamed_on_the_login()
        => Assert.Null(SteamCmdProvider.LoginProblem("ERROR! Failed to install app '896660' (Disk write failure)", Request("valheim")));
}
