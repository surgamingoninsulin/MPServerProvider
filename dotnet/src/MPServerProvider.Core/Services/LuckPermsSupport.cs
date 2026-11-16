using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>What a LuckPerms console line means for the embedded web editor flow.</summary>
public enum LuckPermsEventKind
{
    None,

    /// <summary>"Preparing a new editor session, please wait..."</summary>
    Preparing,

    /// <summary>The editor link (<c>https://luckperms.net/editor/&lt;code&gt;</c>).</summary>
    EditorUrl,

    /// <summary>"Editor window connected successfully" — live (socket) mode.</summary>
    SocketConnected,

    /// <summary>"An editor window has connected, but it is not yet trusted" + the trust command.</summary>
    TrustRequired,

    /// <summary>"The editor session has been marked as trusted".</summary>
    Trusted,

    /// <summary>"Web editor data was applied …" / "Changes have been received from the connected web editor session".</summary>
    ChangesApplied,

    /// <summary>"No changes were applied from the web editor …".</summary>
    NoChanges,

    /// <summary>"Unable to communicate with the editor" / "Unable to open editor" / "Unable to read data using the given code".</summary>
    Error,
}

/// <summary>A parsed LuckPerms console line. <see cref="Value"/> is the editor URL or the trust
/// nonce; <see cref="Alias"/> is the command alias (<c>lp</c>, <c>lpb</c>, <c>lpv</c>).</summary>
public sealed record LuckPermsEvent(LuckPermsEventKind Kind, string Value = "", string Alias = "lp");

/// <summary>
/// Support for the LuckPerms tab: finding the plugin jar and reading the console output of the
/// <c>lp editor</c> web-editor flow. LuckPerms is a jar for every platform it supports (Bukkit/
/// Paper <c>plugins/</c>, Fabric/Forge/NeoForge <c>mods/</c>, Sponge, and Hytale builds), so
/// detection is generic over the folders a server keeps plugins/mods in rather than tied to one
/// game id; a future Hytale definition works as long as its mods folder holds the jar.
/// <para>
/// Console messages are LuckPerms' English defaults (<c>luckperms_en.properties</c>); a server
/// with a translated LuckPerms still gets the editor link (matched by URL shape) but not the
/// status hints.
/// </para>
/// </summary>
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

    /// <summary>
    /// Full path of an installed LuckPerms jar (<c>LuckPerms*.jar</c>, case-insensitive) in the
    /// server's <c>plugins/</c> or <c>mods/</c> folder, the game definition's mods folder, or
    /// the server's own <see cref="ServerInstance.ModsFolder"/>; <c>null</c> when none.
    /// </summary>
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

    /// <summary>Whether a file name is a LuckPerms jar (<c>LuckPerms-Bukkit-5.4.141.jar</c>,
    /// <c>luckperms-fabric.jar</c>, …). Disabled jars (<c>.jar.disabled</c>) don't count.</summary>
    public static bool IsLuckPermsJarName(string fileName)
        => fileName.StartsWith("LuckPerms", StringComparison.OrdinalIgnoreCase) &&
           fileName.EndsWith(".jar", StringComparison.OrdinalIgnoreCase);

    /// <summary>Classifies one (ANSI-stripped) console line.</summary>
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

    /// <summary>
    /// Finds the <c>/lp applyedits &lt;code&gt;</c> command the web editor shows (and copies) after
    /// Save when it isn't connected live. Returns the alias and code, or <c>null</c>.
    /// </summary>
    public static (string Alias, string Code)? FindApplyEdits(string? text)
        => text is not null && ApplyEditsPattern.Match(text) is { Success: true } m
            ? (m.Groups[1].Value.ToLowerInvariant(), m.Groups[2].Value)
            : null;

    /// <summary>Finds a <c>/lp trusteditor &lt;nonce&gt;</c> command in page or console text.</summary>
    public static (string Alias, string Nonce)? FindTrustEditor(string? text)
        => text is not null && TrustPattern.Match(text) is { Success: true } m
            ? (m.Groups[1].Value.ToLowerInvariant(), m.Groups[2].Value)
            : null;

    /// <summary>
    /// Builds a console command (no leading slash: server consoles take bare commands) from
    /// untrusted page/console data, or <c>null</c> when the alias/argument don't look like a
    /// LuckPerms alias and a plain token, so nothing else can be smuggled into the console.
    /// </summary>
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
