using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MPServerProvider.Core.Services;

/// <summary>The server resource pack settings as stored in <c>server.properties</c>.</summary>
/// <param name="Url">Direct download URL (<c>resource-pack</c>), empty for none.</param>
/// <param name="Sha1">Lowercase hex SHA-1 of the zip (<c>resource-pack-sha1</c>), empty for none.</param>
/// <param name="Required">Kick players who decline the pack (<c>require-resource-pack</c>, 1.17+).</param>
/// <param name="PromptJson">Raw JSON text component shown on the accept screen
/// (<c>resource-pack-prompt</c>, 1.17+), empty for the default prompt.</param>
public sealed record ResourcePackSettings(string Url, string Sha1, bool Required, string PromptJson)
{
    public static ResourcePackSettings None { get; } = new(string.Empty, string.Empty, false, string.Empty);

    /// <summary>Whether a pack is configured at all.</summary>
    public bool HasPack => Url.Length > 0;
}

/// <summary>A pack captured from mcpacks.dev's upload result (or pack details) page.</summary>
public sealed record HostedPackInfo(string DownloadUrl, string Sha1, string FileName);

/// <summary>
/// Reads and writes the four resource-pack keys of a Minecraft Java <c>server.properties</c>
/// (<c>resource-pack</c>, <c>resource-pack-sha1</c>, <c>require-resource-pack</c>,
/// <c>resource-pack-prompt</c>) without touching any other line, plus the helpers the Resource
/// Pack tab needs around that (validation, SHA-1 of a local zip, the prompt's JSON text component,
/// and parsing mcpacks.dev's page data). Kept in Core so it is unit tested.
/// </summary>
public static class ResourcePackProperties
{
    public const string FileName = "server.properties";
    public const string UrlKey = "resource-pack";
    public const string Sha1Key = "resource-pack-sha1";
    public const string RequiredKey = "require-resource-pack";
    public const string PromptKey = "resource-pack-prompt";

    private static readonly Regex Sha1Pattern = new("^[0-9a-f]{40}$", RegexOptions.CultureInvariant);

    private static readonly JsonSerializerOptions PromptJsonOptions = new()
    {
        // Keep quotes/apostrophes/accents readable; the properties writer \u-escapes non-ASCII anyway.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public static string PathFor(string serverRoot) => Path.Combine(serverRoot, FileName);

    /// <summary>Current settings from <paramref name="serverRoot"/>'s <c>server.properties</c>
    /// (all empty/false when the file or a key is missing).</summary>
    public static ResourcePackSettings Read(string serverRoot) => Read(JavaPropertiesFile.Load(PathFor(serverRoot)));

    public static ResourcePackSettings Read(JavaPropertiesFile file) => new(
        (file.Get(UrlKey) ?? string.Empty).Trim(),
        (file.Get(Sha1Key) ?? string.Empty).Trim(),
        string.Equals((file.Get(RequiredKey) ?? string.Empty).Trim(), "true", StringComparison.OrdinalIgnoreCase),
        file.Get(PromptKey) ?? string.Empty);

    /// <summary>
    /// Writes <paramref name="settings"/> into <paramref name="serverRoot"/>'s
    /// <c>server.properties</c> (created if missing; Minecraft adds the remaining defaults on the
    /// next start). Only the four resource-pack keys change. Throws
    /// <see cref="ArgumentException"/> when <see cref="Validate"/> rejects the settings.
    /// </summary>
    public static void Apply(string serverRoot, ResourcePackSettings settings)
    {
        var normalized = Normalize(settings);
        if (Validate(normalized) is { } error)
        {
            throw new ArgumentException(error, nameof(settings));
        }

        var path = PathFor(serverRoot);
        var file = JavaPropertiesFile.Load(path);
        Apply(file, normalized);
        file.Save(path);
    }

    /// <summary>In-memory variant of <see cref="Apply(string, ResourcePackSettings)"/>.</summary>
    public static void Apply(JavaPropertiesFile file, ResourcePackSettings settings)
    {
        var normalized = Normalize(settings);
        file.Set(UrlKey, normalized.Url);
        file.Set(Sha1Key, normalized.Sha1);
        file.Set(RequiredKey, normalized.Required ? "true" : "false");
        file.Set(PromptKey, normalized.PromptJson);
    }

    /// <summary>Clears the pack (vanilla defaults: empty URL/hash/prompt, not required).</summary>
    public static void Remove(string serverRoot) => Apply(serverRoot, ResourcePackSettings.None);

    /// <summary>Trims the URL/hash/prompt and lowercases the hash (Minecraft compares it
    /// case-insensitively, but lowercase is what every tool prints and what users compare).</summary>
    public static ResourcePackSettings Normalize(ResourcePackSettings settings) => settings with
    {
        Url = (settings.Url ?? string.Empty).Trim(),
        Sha1 = (settings.Sha1 ?? string.Empty).Trim().ToLowerInvariant(),
        PromptJson = (settings.PromptJson ?? string.Empty).Trim(),
    };

    /// <summary>A user-facing reason the settings can't be applied, or <c>null</c> when they're fine.</summary>
    public static string? Validate(ResourcePackSettings settings)
    {
        var s = Normalize(settings);
        if (s.Url.Length == 0)
        {
            if (s.Sha1.Length > 0 || s.Required)
            {
                return "Enter the resource pack's download URL (or use Remove pack to clear it).";
            }

            return null;
        }

        if (!Uri.TryCreate(s.Url, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
        {
            return "The pack URL must be a full http:// or https:// link to the .zip file.";
        }

        if (s.Url.Any(char.IsWhiteSpace))
        {
            return "The pack URL can't contain spaces.";
        }

        if (s.Sha1.Length > 0 && !Sha1Pattern.IsMatch(s.Sha1))
        {
            return "The SHA-1 must be exactly 40 hexadecimal characters (0-9, a-f).";
        }

        if (s.PromptJson.Length > 0)
        {
            try
            {
                using var _ = JsonDocument.Parse(s.PromptJson);
            }
            catch (JsonException)
            {
                return "The prompt message isn't a valid JSON text component.";
            }
        }

        return null;
    }

    /// <summary>Whether <paramref name="sha1"/> looks like a SHA-1 hex digest (any case).</summary>
    public static bool IsSha1(string? sha1) => sha1 is not null && Sha1Pattern.IsMatch(sha1.Trim().ToLowerInvariant());

    /// <summary>Lowercase hex SHA-1 of a local file (what <c>resource-pack-sha1</c> expects).</summary>
    public static string ComputeSha1(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16, FileOptions.SequentialScan);
        return Convert.ToHexString(SHA1.HashData(stream)).ToLowerInvariant();
    }

    /// <summary>
    /// <c>resource-pack-prompt</c> is a JSON text component, not plain text: wrap the user's text
    /// as <c>{"text":"…"}</c> with proper JSON escaping (quotes, backslashes, newlines). Empty
    /// text means "no custom prompt".
    /// </summary>
    public static string PromptJsonFromText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return JsonSerializer.Serialize(new Dictionary<string, string> { ["text"] = text.Trim() }, PromptJsonOptions);
    }

    /// <summary>
    /// The readable text of a prompt component (for showing/editing it): a JSON string, or the
    /// concatenated <c>text</c> of an object and its <c>extra</c> children, or an array of those.
    /// Anything that isn't JSON (e.g. a plain string some other tool wrote) is returned as-is.
    /// </summary>
    public static string PromptTextFromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return string.Empty;
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            var sb = new System.Text.StringBuilder();
            AppendComponentText(doc.RootElement, sb);
            return sb.ToString();
        }
        catch (JsonException)
        {
            return json;
        }
    }

    /// <summary>Whether <paramref name="json"/> uses more than a plain <c>{"text":"…"}</c> /
    /// string component (colors, formatting, children), which editing it as plain text would lose.</summary>
    public static bool IsStyledPrompt(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            return root.ValueKind switch
            {
                JsonValueKind.String => false,
                JsonValueKind.Object => root.EnumerateObject().Any(p => p.Name != "text"),
                _ => true,
            };
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static void AppendComponentText(JsonElement element, System.Text.StringBuilder sb)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                sb.Append(element.GetString());
                break;
            case JsonValueKind.Array:
                foreach (var child in element.EnumerateArray())
                {
                    AppendComponentText(child, sb);
                }

                break;
            case JsonValueKind.Object:
                if (element.TryGetProperty("text", out var text) && text.ValueKind == JsonValueKind.String)
                {
                    sb.Append(text.GetString());
                }

                if (element.TryGetProperty("extra", out var extra))
                {
                    AppendComponentText(extra, sb);
                }

                break;
        }
    }

    /// <summary>
    /// Whether a server version supports <c>require-resource-pack</c> and
    /// <c>resource-pack-prompt</c> (added in Java Edition 1.17). <c>null</c> when the version
    /// string can't be read (snapshots, custom names), so callers only warn when sure.
    /// Year-based versions (26.x) count as newer.
    /// </summary>
    public static bool? SupportsRequiredPrompt(string? version)
    {
        var match = Regex.Match(version ?? string.Empty, @"^\s*(\d+)\.(\d+)");
        if (!match.Success)
        {
            return null;
        }

        var major = int.Parse(match.Groups[1].Value);
        var minor = int.Parse(match.Groups[2].Value);
        return major > 1 || (major == 1 && minor >= 17);
    }

    /// <summary>
    /// mcpacks.dev is a Laravel + Inertia.js app: the upload form posts to <c>/upload</c> and the
    /// result is an Inertia page (<c>{"component":"UploadSuccess","props":{"pack":{…}}}</c>,
    /// the same shape for <c>PackDetails</c>) delivered either as the XHR response to the
    /// <c>X-Inertia</c> request or embedded in the HTML / <c>history.state.page</c>. Extracts
    /// <c>pack.download_url</c> and <c>pack.sha1_hash</c> from that JSON; <c>null</c> when it
    /// isn't such a page or the values don't look right.
    /// </summary>
    public static HostedPackInfo? TryParseMcPacksPage(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            // Accept the page object itself, or a wrapper holding it (history.state = {page: …}).
            if (root.TryGetProperty("page", out var page) && page.ValueKind == JsonValueKind.Object)
            {
                root = page;
            }

            if (!root.TryGetProperty("props", out var props) || props.ValueKind != JsonValueKind.Object ||
                !props.TryGetProperty("pack", out var pack) || pack.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            return TryCreateHostedPack(
                GetString(pack, "download_url"),
                GetString(pack, "sha1_hash"),
                GetString(pack, "original_filename"));
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>Validates captured values (absolute http(s) URL, 40-hex SHA-1) before they are
    /// offered to the user; page data is untrusted.</summary>
    public static HostedPackInfo? TryCreateHostedPack(string? url, string? sha1, string? fileName)
    {
        url = url?.Trim();
        sha1 = sha1?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(url) || !IsSha1(sha1) ||
            !Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https") ||
            url.Any(char.IsWhiteSpace))
        {
            return null;
        }

        var name = (fileName ?? string.Empty).Trim();
        if (name.Length > 200)
        {
            name = name[..200];
        }

        return new HostedPackInfo(url, sha1!, name);
    }

    private static string? GetString(JsonElement obj, string name)
        => obj.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
}
