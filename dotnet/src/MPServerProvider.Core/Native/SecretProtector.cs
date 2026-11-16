using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace MPServerProvider.Core.Native;

[SupportedOSPlatform("windows")]
public static class SecretProtector
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("MPServerProvider.v1");

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
