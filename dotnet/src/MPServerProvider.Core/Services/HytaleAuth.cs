using MPServerProvider.Core.Providers;

namespace MPServerProvider.Core.Services;

public sealed record HytaleAuthState(string? Link, string? Code, bool SignedIn);

public static class HytaleAuth
{
    public const string GameId = "hytale";
    public const string LoginCommand = "/auth login device";
    public const string PersistCommand = "/auth persistence Encrypted";
    public const string DevicePage = "https://accounts.hytale.com/device";

    public static HytaleAuthState Scan(IReadOnlyList<string> console)
    {
        string? link = null;
        string? code = null;
        var linkIndex = -1;
        for (var i = console.Count - 1; i >= 0 && link is null; i--)
        {
            link = HytaleProvider.SignInLink(console[i]);
            if (link is not null)
            {
                linkIndex = i;
            }
        }

        if (linkIndex >= 0)
        {
            // The code is printed with the link or just after it; only look back if it isn't.
            for (var i = linkIndex; i < Math.Min(console.Count, linkIndex + 4) && code is null; i++)
            {
                code = HytaleProvider.DeviceCode(console[i]);
            }

            for (var i = linkIndex - 1; i >= Math.Max(0, linkIndex - 3) && code is null; i--)
            {
                if (HytaleProvider.SignInLink(console[i]) is not null)
                {
                    break;
                }

                code = HytaleProvider.DeviceCode(console[i]);
            }
        }

        var signedIn = false;
        for (var i = Math.Max(0, linkIndex + 1); i < console.Count; i++)
        {
            if (IsSuccessLine(console[i]))
            {
                signedIn = true;
            }
            else if (IsFailureLine(console[i]))
            {
                signedIn = false;
            }
        }

        return new HytaleAuthState(link, code, signedIn);
    }

    internal static bool IsSuccessLine(string line)
    {
        var l = line.ToLowerInvariant();
        return l.Contains("oauth_device")
               || ((l.Contains("authenticat") || l.Contains("logged in") || l.Contains("login")) && (l.Contains("success") || l.Contains("logged in as")));
    }

    internal static bool IsFailureLine(string line)
    {
        var l = line.ToLowerInvariant();
        return (l.Contains("authenticat") || l.Contains("login")) && (l.Contains("fail") || l.Contains("expired") || l.Contains("denied"));
    }
}
