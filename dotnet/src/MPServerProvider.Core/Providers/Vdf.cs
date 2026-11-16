namespace MPServerProvider.Core.Providers;

public sealed class Node
{
    public string? Value { get; set; }

    public SortedDictionary<string, Node> Children { get; } = new(StringComparer.Ordinal);

    public static Node Leaf(string value) => new() { Value = value };

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

public static class Vdf
{
    public const int MaxDepth = 64;

    public static Node Parse(string text)
    {
        var pos = 0;
        var root = new Node();
        ParseBlock(text, ref pos, root, depth: 0);
        return root;
    }

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

    private static void SkipTrivia(string text, ref int pos)
    {
        while (pos < text.Length && char.IsWhiteSpace(text[pos]))
        {
            pos += 1;
        }
    }

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
