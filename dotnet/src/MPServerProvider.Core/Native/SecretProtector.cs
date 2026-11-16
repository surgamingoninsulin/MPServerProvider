using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace MPServerProvider.Core.Native;

/// <summary>
/// Near-literal restoration of <c>MPServerProvider.Core.Services.SecretProtector</c>
/// (see <c>docs/decompile-report/report/SECRET_PROTECTOR.md</c>).
/// The original is plain Windows DPAPI with a static entropy string — no custom crypto, and
/// since this is C# calling the exact same BCL API the original used
/// (<see cref="ProtectedData"/>, itself a thin wrapper over <c>CryptProtectData</c>/
/// <c>CryptUnprotectData</c>), this is a byte-for-byte restoration rather than a reinterpretation:
/// any blob the original app protected on disk will still decrypt correctly here, and vice versa.
/// </summary>
[SupportedOSPlatform("windows")]
public static class SecretProtector
{
    /// <summary>
    /// Matches the original's static field:
    /// <c>Encoding.UTF8.GetBytes("MPServerProvider.v1")</c>.
    /// </summary>
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("MPServerProvider.v1");

    /// <summary>
    /// Protects <paramref name="plaintext"/> for the current Windows user via DPAPI, returning
    /// a base64-encoded ciphertext. Mirrors the original's null/empty-string guard.
    /// </summary>
    public static string Protect(string? plaintext)
    {
        if (string.IsNullOrEmpty(plaintext))
        {
            return plaintext ?? string.Empty;
        }

        var bytes = Encoding.UTF8.GetBytes(plaintext);
        var protectedBytes = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(protectedBytes);
    }

    /// <summary>
    /// Unprotects a base64-encoded blob previously produced by <see cref="Protect"/> (or by the
    /// original app, since this is the same DPAPI call). Mirrors the original's
    /// null/empty-string guard.
    /// </summary>
    public static string Unprotect(string? base64)
    {
        if (string.IsNullOrEmpty(base64))
        {
            return base64 ?? string.Empty;
        }

        var bytes = Convert.FromBase64String(base64);
        var plaintextBytes = ProtectedData.Unprotect(bytes, Entropy, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(plaintextBytes);
    }
}
