using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class HytaleAuthTests
{
    [Fact]
    public void Nothing_in_the_console_means_not_signed_in()
        => Assert.Equal(new HytaleAuthState(null, null, false), HytaleAuth.Scan(new[] { "[Server] Starting", "[Server] Listening on 0.0.0.0:5520" }));

    [Fact]
    public void Finds_the_newest_link_and_code()
    {
        var state = HytaleAuth.Scan(new[]
        {
            "> /auth login device",
            "Visit https://accounts.hytale.com/device?user_code=OLD1-OLD1",
            "> /auth login device",
            "Visit: https://accounts.hytale.com/device",
            "Enter code: ABCD-EFGH",
        });
        Assert.Equal("https://accounts.hytale.com/device", state.Link);
        Assert.Equal("ABCD-EFGH", state.Code);
        Assert.False(state.SignedIn);
    }

    [Fact]
    public void Success_after_the_link_means_signed_in()
    {
        var state = HytaleAuth.Scan(new[]
        {
            "Visit https://accounts.hytale.com/device?user_code=WXYZ-1234",
            "Authentication successful! Mode: OAUTH_DEVICE",
        });
        Assert.True(state.SignedIn);
        Assert.Equal("WXYZ-1234", state.Code);
    }

    [Fact]
    public void Expired_code_after_success_is_not_signed_in()
        => Assert.False(HytaleAuth.Scan(new[]
        {
            "Visit https://accounts.hytale.com/device?user_code=WXYZ-1234",
            "Authentication failed: device code expired",
        }).SignedIn);
}
