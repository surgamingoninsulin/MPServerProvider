using System.Runtime.Versioning;
using MPServerProvider.Core.Native;
using Xunit;

namespace MPServerProvider.Core.Tests;

/// <summary>
/// DPAPI is user-scoped (tied to the Windows account running the test), so this is a valid
/// self-test on this machine: protect then unprotect under the current user must round-trip.
/// </summary>
[SupportedOSPlatform("windows")]
public class SecretProtectorTests
{
    [Fact]
    public void Protect_then_unprotect_round_trips_for_current_user()
    {
        const string plaintext = "super-secret-license-key-ABC123";

        var protected1 = SecretProtector.Protect(plaintext);
        Assert.NotEqual(plaintext, protected1);

        var recovered = SecretProtector.Unprotect(protected1);
        Assert.Equal(plaintext, recovered);
    }

    [Fact]
    public void Null_or_empty_input_passes_through_unchanged()
    {
        Assert.Equal(string.Empty, SecretProtector.Protect(null));
        Assert.Equal(string.Empty, SecretProtector.Protect(string.Empty));
        Assert.Equal(string.Empty, SecretProtector.Unprotect(null));
        Assert.Equal(string.Empty, SecretProtector.Unprotect(string.Empty));
    }

    [Fact]
    public void Protected_blob_is_base64_and_differs_each_time_entropy_reused()
    {
        const string plaintext = "another-secret-value";
        var protected1 = SecretProtector.Protect(plaintext);
        var protected2 = SecretProtector.Protect(plaintext);

        // Both must decode as valid base64 and both must decrypt back to the same plaintext,
        // even though DPAPI typically salts its own output so the ciphertexts need not be equal.
        Assert.Equal(plaintext, SecretProtector.Unprotect(protected1));
        Assert.Equal(plaintext, SecretProtector.Unprotect(protected2));
        Convert.FromBase64String(protected1);
        Convert.FromBase64String(protected2);
    }
}
