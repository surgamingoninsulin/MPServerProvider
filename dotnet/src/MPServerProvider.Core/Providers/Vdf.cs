namespace MPServerProvider.Core.Providers;

/// <summary>
/// One node of a parsed VDF/KeyValues document.
/// Mirrors the decompiled <c>Node</c> type: a node either holds a scalar <see cref="Value"/> (a
/// leaf <c>"key" "value"</c> pair) or a set of named <see cref="Children"/> (a
/// <c>"key" { ... }</c> block) — in Valve's format a given node is one or the other, never both.
/// <para>
/// Children are stored in a <see cref="SortedDictionary{TKey,TValue}"/> (ordered by key) rather
/// than an insertion-ordered map: <c>Vdf.ToAppInfo</c>'s decompiled call edges show
/// <c>Dictionary.TryAdd</c>/<c>set_Item</c> plus an explicit <c>OrderBy</c>/<c>StringComparer
/// .OrdinalIgnoreCase</c> sort right before picking the first <c>launch</c> entry, i.e. the
/// original never relies on insertion order either — it always re-sorts by key — so an
/// ordered-by-key map reproduces the same externally observable behavior with a simpler type.
/// This mirrors the proven Rust port's choice of a <c>BTreeMap</c> for the same reason.
/// </para>
/// </summary>
public sealed class Node
{
    public string? Value { get; set; }

    public SortedDictionary<string, Node> Children { get; } = new(StringComparer.Ordinal);

    public static Node Leaf(string value) => new() { Value = value };

    /// <summary>
    /// Case-insensitive child lookup (Valve's own dumps are consistently lowercase, but
    /// <c>Vdf.ToAppInfo</c>'s use of <c>StringComparer.OrdinalIgnoreCase</c> elsewhere in the
    /// same method suggests key lookups throughout are meant to be case-insensitive).
    /// </summary>
    public Node? Find(string key)
    {
        foreach (var (k, v) in Children)
        {
            if (string.Equals(k, key, StringComparison.OrdinalIgnoreCase))
            {
                return v;
            }
        }

        return null;
    }
}

/// <summary>
/// Valve KeyValues (VDF) parser, and the SteamCMD-specific <c>app_info_print</c>/
/// <c>appmanifest_*.acf</c> -&gt; <see cref="SteamAppInfo"/> projection.
/// Ported from <c>MPServerProvider.Core.Providers.Vdf</c> (decompile token 0x02000025):
/// a minimal recursive-descent parser for Valve's "KeyValues" text format (the format used by
/// SteamCMD's <c>app_info_print</c> output and <c>appmanifest_*.acf</c> files): nested
/// <c>"key" "value"</c> pairs and <c>"key" { ... }</c> blocks, whitespace-separated, with no
/// escape-sequence handling evidenced in the decompiled <c>ReadQuoted</c>/<c>SkipTrivia</c>
/// bodies (both only call <c>Char.IsWhiteSpace</c>/<c>StringBuilder.Append</c> — no
/// <c>\"</c>-unescaping call edge), so this doesn't attempt escape handling either. This restores,
/// in C#, the exact parser algorithm already proven correct by the Rust port's
/// <c>providers/vdf.rs</c>.
/// </summary>
public static class Vdf
{
    /// <summary>Maximum block nesting accepted by <see cref="Parse"/>.</summary>
    public const int MaxDepth = 64;

    /// <summary>Port of <c>Vdf.Parse(string text) -&gt; Node</c>: parses <paramref name="text"/>
    /// into a synthetic root node whose <see cref="Node.Children"/> are the document's top-level
    /// keys.</summary>
    public static Node Parse(string text)
    {
        var pos = 0;
        var root = new Node();
        ParseBlock(text, ref pos, root, depth: 0);
        return root;
    }

    /// <summary>
    /// Port of <c>Vdf.ParseBlock(string, ref int, Node)</c>: consumes <c>"key" "value"</c> and
    /// <c>"key" { ... }</c> pairs into <paramref name="node"/>.Children until end of input or a
    /// closing <c>}</c> (which is itself consumed here, mirroring the decompiled body's single
    /// length check plus recursive call on <c>{</c>).
    /// </summary>
    private static void ParseBlock(string text, ref int pos, Node node, int depth)
    {
        // Deeply nested input would otherwise recurse until an uncatchable StackOverflowException
        // takes down the whole app; real Steam app info is ~10 levels deep.
        if (depth > MaxDepth)
        {
            throw new FormatException($"VDF nesting deeper than {MaxDepth} levels.");
        }

        while (true)
        {
            SkipTrivia(text, ref pos);
            if (pos >= text.Length)
            {
                return;
            }

            if (text[pos] == '}')
            {
                pos += 1;
                return;
            }

            if (text[pos] != '"')
            {
                // Malformed/unexpected token: bail rather than loop forever.
                pos += 1;
                continue;
            }

            var key = ReadQuoted(text, ref pos);
            SkipTrivia(text, ref pos);

            if (pos < text.Length && text[pos] == '{')
            {
                pos += 1;
                var child = new Node();
                ParseBlock(text, ref pos, child, depth + 1);
                node.Children[key] = child;
            }
            else if (pos < text.Length && text[pos] == '"')
            {
                var value = ReadQuoted(text, ref pos);
                node.Children[key] = Node.Leaf(value);
            }
            else
            {
                // Key with no value/block (shouldn't happen in well-formed VDF).
                node.Children[key] = new Node();
            }
        }
    }

    /// <summary>Port of <c>Vdf.SkipTrivia(string, ref int)</c>: advances past whitespace only
    /// (the decompiled body's only call edge besides string indexing is
    /// <c>Char.IsWhiteSpace</c>).</summary>
    private static void SkipTrivia(string text, ref int pos)
    {
        while (pos < text.Length && char.IsWhiteSpace(text[pos]))
        {
            pos += 1;
        }
    }

    /// <summary>Port of <c>Vdf.ReadQuoted(string, ref int)</c>: reads a <c>"..."</c> token and
    /// returns its contents, unescaped (see type docs).</summary>
    private static string ReadQuoted(string text, ref int pos)
    {
        // Caller has already confirmed text[pos] == '"'.
        pos += 1;
        var start = pos;
        while (pos < text.Length && text[pos] != '"')
        {
            pos += 1;
        }

        var value = text[start..pos];
        if (pos < text.Length)
        {
            pos += 1; // consume closing quote
        }

        return value;
    }

    /// <summary>Port of <c>Vdf.ToAppInfo(Node root) -&gt; SteamAppInfo</c>.
    /// Expects <paramref name="root"/> to be the synthetic parse root whose single child is the
    /// app id node (SteamCMD's <c>app_info_print</c> output wraps everything in one top-level
    /// <c>"&lt;appid&gt;" { ... }</c> block).</summary>
    public static SteamAppInfo? ToAppInfo(Node root)
    {
        if (root.Children.Count == 0)
        {
            return null;
        }

        var app = root.Children.Values.First();

        var common = app.Find("common");
        var config = app.Find("config");

        var name = common?.Find("name")?.Value ?? string.Empty;
        int? parentAppId = null;
        var parentRaw = common?.Find("parent")?.Value;
        if (parentRaw is not null && int.TryParse(parentRaw, out var parsedParent))
        {
            parentAppId = parsedParent;
        }

        var installDir = config?.Find("installdir")?.Value;

        var launch = config?.Find("launch");
        var firstLaunch = launch?.Children.Values.FirstOrDefault();
        var executable = firstLaunch?.Find("executable")?.Value;
        var workingDir = firstLaunch?.Find("workingdir")?.Value;
        var arguments = firstLaunch?.Find("arguments")?.Value;

        var freeToDownload = common?.Find("freetodownload")?.Value == "1";

        return new SteamAppInfo
        {
            Name = name,
            ParentAppId = parentAppId,
            InstallDir = installDir,
            Executable = executable,
            WorkingDir = workingDir,
            Arguments = arguments,
            FreeToDownload = freeToDownload,
        };
    }
}
