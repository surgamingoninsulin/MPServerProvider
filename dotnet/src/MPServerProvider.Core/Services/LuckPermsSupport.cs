using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public enum LuckPermsEventKind
{
    None,

    Preparing,

    EditorUrl,

    SocketConnected,

    TrustRequired,

    Trusted,

    ChangesApplied,

    NoChanges,

    Error,
}

public sealed record LuckPermsEvent(LuckPermsEventKind Kind, string Value = "", string Alias = "lp");

public static class LuckPermsSupport
{
    private static readonly string[] CandidateFolders = { "plugins", "mods" };

    private static readonly Regex EditorUrlPattern = new(
        @"https?://[^\s""'<>]+/editor/[A-Za-z0-9_-]+", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex TrustPattern = new(
        @"/?\b(lp[bv]?)\s+trusteditor\s+([A-Za-z0-9_-]{1,64})\b", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex ApplyEditsPattern = new(
        @"/?\b(lp[bv]?)\s+applyedits\s+([A-Za-z0-9_-]{1,64})\b", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static readonly Regex SafeToken = new("^[A-Za-z0-9_-]{1,64}$", RegexOptions.CultureInvariant);

    public static string? FindJar(ServerInstance server, GameDefinition? game = null)
    {
        if (string.IsNullOrWhiteSpace(server.ServerRoot))
        {
            return null;
        }

        var folders = new List<string>();
        foreach (var name in CandidateFolders)
        {
            folders.Add(Path.Combine(server.ServerRoot, name));
        }

        if (game?.Mods is { Folder.Length: > 0 } mods)
        {
            folders.Add(server.ResolveRoot(mods.Folder));
        }

        if (!string.IsNullOrWhiteSpace(server.ModsFolder))
        {
            folders.Add(server.ResolveRoot(server.ModsFolder));
        }

        foreach (var folder in folders.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    continue;
                }

                var jar = Directory.EnumerateFiles(folder, "*.jar")
                    .FirstOrDefault(f => IsLuckPermsJarName(Path.GetFileName(f)));
                if (jar is not null)
                {
                    return jar;
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Unreadable folder: treat as "not installed there".
            }
        }

        return null;
    }

    public static bool IsLuckPermsJarName(string fileName)
        => fileName.StartsWith("LuckPerms", StringComparison.OrdinalIgnoreCase) &&
           fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase);

    public static LuckPermsEvent Parse(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return new LuckPermsEvent(LuckPermsEventKind.None);
        }

        // The app echoes commands it sends ("> lp applyedits abc"); those aren't server output.
        if (line.StartsWith("> ", StringComparison.Ordinal))
        {
            return new LuckPermsEvent(LuckPermsEventKind.None);
        }

        if (TrustPattern.Match(line) is { Success: true } trust)
        {
            return new LuckPermsEvent(LuckPermsEventKind.TrustRequired, trust.Groups[2].Value, trust.Groups[1].Value.ToLowerInvariant());
        }

        if (EditorUrlPattern.Match(line) is { Success: true } url)
        {
            return new LuckPermsEvent(LuckPermsEventKind.EditorUrl, url.Value);
        }

        if (Contains(line, "Preparing a new editor session"))
        {
            return new LuckPermsEvent(LuckPermsEventKind.Preparing);
        }

        if (Contains(line, "Editor window connected successfully") || Contains(line, "Editor window reconnected successfully"))
        {
            return new LuckPermsEvent(LuckPermsEventKind.SocketConnected);
        }

        if (Contains(line, "editor window has connected, but it is not yet trusted"))
        {
            return new LuckPermsEvent(LuckPermsEventKind.TrustRequired);
        }

        if (Contains(line, "editor session has been marked as trusted"))
        {
            return new LuckPermsEvent(LuckPermsEventKind.Trusted);
        }

        if (Contains(line, "Web editor data was applied") ||
            Contains(line, "Changes have been received from the connected web editor session"))
        {
            return new LuckPermsEvent(LuckPermsEventKind.ChangesApplied);
        }

        if (Contains(line, "No changes were applied from the web editor"))
        {
            return new LuckPermsEvent(LuckPermsEventKind.NoChanges);
        }

        if (Contains(line, "Unable to communicate with the editor") ||
            Contains(line, "Unable to open editor") ||
            Contains(line, "Unable to read data using the given code") ||
            Contains(line, "Unable to trust the given session"))
        {
            return new LuckPermsEvent(LuckPermsEventKind.Error, line.Trim());
        }

        return new LuckPermsEvent(LuckPermsEventKind.None);
    }

    public static (string Alias, string Code)? FindApplyEdits(string? text)
        => text is not null && ApplyEditsPattern.Match(text) is { Success: true } m
            ? (m.Groups[1].Value.ToLowerInvariant(), m.Groups[2].Value)
            : null;

    public static (string Alias, string Nonce)? FindTrustEditor(string? text)
        => text is not null && TrustPattern.Match(text) is { Success: true } m
            ? (m.Groups[1].Value.ToLowerInvariant(), m.Groups[2].Value)
            : null;

    public static string? BuildCommand(string alias, string subcommand, string argument)
    {
        alias = (alias ?? string.Empty).Trim().ToLowerInvariant();
        if (alias is not ("lp" or "lpb" or "lpv") || !SafeToken.IsMatch(argument ?? string.Empty) ||
            subcommand is not ("applyedits" or "trusteditor"))
        {
            return null;
        }

        return $"{alias} {subcommand} {argument}";
    }

    private static bool Contains(string line, string text) => line.Contains(text, StringComparison.OrdinalIgnoreCase);
}
