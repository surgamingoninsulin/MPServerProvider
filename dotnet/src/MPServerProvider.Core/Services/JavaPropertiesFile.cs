using System.Text;
using MPServerProvider.Core.Providers;

namespace MPServerProvider.Core.Services;

/// <summary>
/// A layout-preserving editor for Java <c>.properties</c> files (Minecraft Java's
/// <c>server.properties</c>). Unlike <c>java.util.Properties.store</c>, which rewrites the whole
/// file (dropping comments and reordering keys), this only rewrites the lines of the keys that
/// are <see cref="Set"/>: every other line (comments, blank lines, unknown keys, odd spacing,
/// multi-line continuations) is written back byte-for-byte, with the original line endings and
/// text encoding. That matters because users (and plugins) hand-edit this file and expect their
/// comments and ordering to survive.
/// </summary>
public sealed class JavaPropertiesFile
{
    /// <summary>One logical line: a comment/blank line, or a <c>key=value</c> entry that may
    /// span several physical lines via trailing-backslash continuations.</summary>
    private sealed class Segment
    {
        /// <summary>The exact original text, including its line terminator(s).</summary>
        public required string Raw { get; set; }

        /// <summary>The terminator of the segment's last physical line ("" at end of file).</summary>
        public required string Terminator { get; set; }

        /// <summary>Unescaped key, or <c>null</c> for comments and blank lines.</summary>
        public string? Key { get; set; }

        /// <summary>Unescaped value (only meaningful when <see cref="Key"/> is set).</summary>
        public string Value { get; set; } = string.Empty;
    }

    private readonly List<Segment> _segments = new();
    private readonly string _newLine;

    private JavaPropertiesFile(string text, Encoding encoding)
    {
        Encoding = encoding;
        ParseSegments(text);
        _newLine = DetectNewLine(text);
    }

    /// <summary>The encoding the file was read with (and is saved with).</summary>
    public Encoding Encoding { get; }

    /// <summary>Parses <paramref name="text"/> (an empty string for a file that doesn't exist yet).</summary>
    public static JavaPropertiesFile Parse(string text) => new(text, new UTF8Encoding(false));

    /// <summary>
    /// Reads <paramref name="path"/>, or starts an empty document when it doesn't exist (Minecraft
    /// fills in every missing key with its default on the next start). Modern servers read the
    /// file as UTF-8 and fall back to ISO-8859-1, so the same detection is used here; a UTF-8 BOM
    /// is kept when present.
    /// </summary>
    public static JavaPropertiesFile Load(string path)
    {
        if (!File.Exists(path))
        {
            return new JavaPropertiesFile(string.Empty, new UTF8Encoding(false));
        }

        var bytes = File.ReadAllBytes(path);
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            return new JavaPropertiesFile(new UTF8Encoding(false).GetString(bytes, 3, bytes.Length - 3), new UTF8Encoding(true));
        }

        try
        {
            return new JavaPropertiesFile(new UTF8Encoding(false, throwOnInvalidBytes: true).GetString(bytes), new UTF8Encoding(false));
        }
        catch (DecoderFallbackException)
        {
            return new JavaPropertiesFile(Encoding.Latin1.GetString(bytes), Encoding.Latin1);
        }
    }

    /// <summary>Every key in file order (duplicates included once, at their first position).</summary>
    public IReadOnlyList<string> Keys =>
        _segments.Where(s => s.Key is not null).Select(s => s.Key!).Distinct(StringComparer.Ordinal).ToList();

    /// <summary>Unescaped value of <paramref name="key"/> (the last occurrence wins, as in Java),
    /// or <c>null</c> when the key isn't present.</summary>
    public string? Get(string key)
    {
        for (var i = _segments.Count - 1; i >= 0; i--)
        {
            if (string.Equals(_segments[i].Key, key, StringComparison.Ordinal))
            {
                return _segments[i].Value;
            }
        }

        return null;
    }

    /// <summary>
    /// Sets <paramref name="key"/> to <paramref name="value"/> (escaped for the properties
    /// format). Existing occurrences are rewritten in place (all of them, so a duplicate can't
    /// silently override the new value); a missing key is appended at the end.
    /// </summary>
    public void Set(string key, string value)
    {
        var line = key + "=" + EscapeValue(value);
        var found = false;
        foreach (var segment in _segments)
        {
            if (!string.Equals(segment.Key, key, StringComparison.Ordinal))
            {
                continue;
            }

            found = true;
            segment.Raw = line + segment.Terminator;
            segment.Value = value;
        }

        if (found)
        {
            return;
        }

        if (_segments.Count > 0 && _segments[^1].Terminator.Length == 0)
        {
            // The last line had no newline: give it one before appending.
            _segments[^1].Raw += _newLine;
            _segments[^1].Terminator = _newLine;
        }

        _segments.Add(new Segment { Raw = line + _newLine, Terminator = _newLine, Key = key, Value = value });
    }

    /// <summary>The document text: untouched lines verbatim, changed keys rewritten.</summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var segment in _segments)
        {
            sb.Append(segment.Raw);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Writes the document to <paramref name="path"/> through a temporary file + replace, so a
    /// crash or full disk mid-write can't leave the server with a truncated <c>server.properties</c>.
    /// </summary>
    public void Save(string path)
    {
        var temp = path + ".mpsp-tmp";
        var preamble = Encoding.GetPreamble();
        var body = Encoding.GetBytes(ToString());
        using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            stream.Write(preamble);
            stream.Write(body);
            stream.Flush(flushToDisk: true);
        }

        File.Move(temp, path, overwrite: true);
    }

    /// <summary>
    /// Escapes a value for the properties format: <see cref="ConfigText.JavaPropertiesValue"/>
    /// (backslash, <c>=</c>, <c>:</c>, <c>#</c>, <c>!</c>, control and non-ASCII characters),
    /// plus a leading space, which <c>Properties.load</c> would otherwise strip.
    /// </summary>
    public static string EscapeValue(string? value)
    {
        var escaped = ConfigText.JavaPropertiesValue(value);
        return escaped.StartsWith(' ') ? "\\" + escaped : escaped;
    }

    /// <summary>Resolves <c>\uXXXX</c>, <c>\t</c>/<c>\n</c>/<c>\r</c>/<c>\f</c> and
    /// "backslash + any other character = that character", like <c>Properties.load</c>.</summary>
    public static string Unescape(string text)
    {
        if (!text.Contains('\\'))
        {
            return text;
        }

        var sb = new StringBuilder(text.Length);
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c != '\\' || i == text.Length - 1)
            {
                sb.Append(c);
                continue;
            }

            var next = text[++i];
            switch (next)
            {
                case 't': sb.Append('\t'); break;
                case 'n': sb.Append('\n'); break;
                case 'r': sb.Append('\r'); break;
                case 'f': sb.Append('\f'); break;
                case 'u' when i + 4 < text.Length &&
                              int.TryParse(text.AsSpan(i + 1, 4), System.Globalization.NumberStyles.HexNumber, null, out var code):
                    sb.Append((char)code);
                    i += 4;
                    break;
                default: sb.Append(next); break;
            }
        }

        return sb.ToString();
    }

    // -- parsing ---------------------------------------------------------------------------------

    private void ParseSegments(string text)
    {
        var physical = SplitPhysicalLines(text);
        var index = 0;
        while (index < physical.Count)
        {
            var (content, terminator) = physical[index];
            var raw = new StringBuilder(content).Append(terminator);
            var trimmed = content.TrimStart(' ', '\t', '\f');

            if (trimmed.Length == 0 || trimmed[0] is '#' or '!')
            {
                _segments.Add(new Segment { Raw = raw.ToString(), Terminator = terminator });
                index++;
                continue;
            }

            // Join continuation lines (odd number of trailing backslashes): the backslash and the
            // line break are dropped, and so is the next line's leading whitespace.
            var logical = new StringBuilder(trimmed);
            while (EndsWithContinuation(logical) && index + 1 < physical.Count && terminator.Length > 0)
            {
                logical.Length--;
                index++;
                (content, terminator) = physical[index];
                raw.Append(content).Append(terminator);
                logical.Append(content.TrimStart(' ', '\t', '\f'));
            }

            if (EndsWithContinuation(logical))
            {
                logical.Length--; // dangling backslash at end of file
            }

            var (key, value) = SplitEntry(logical.ToString());
            _segments.Add(new Segment { Raw = raw.ToString(), Terminator = terminator, Key = key, Value = value });
            index++;
        }
    }

    private static bool EndsWithContinuation(StringBuilder line)
    {
        var count = 0;
        for (var i = line.Length - 1; i >= 0 && line[i] == '\\'; i--)
        {
            count++;
        }

        return count % 2 == 1;
    }

    /// <summary>Splits a logical line into its unescaped key and value: the key ends at the first
    /// unescaped <c>=</c>, <c>:</c> or whitespace; one <c>=</c>/<c>:</c> separator and the
    /// whitespace around it are skipped.</summary>
    private static (string Key, string Value) SplitEntry(string line)
    {
        var i = 0;
        while (i < line.Length)
        {
            var c = line[i];
            if (c == '\\')
            {
                i += 2;
                continue;
            }

            if (c is '=' or ':' or ' ' or '\t' or '\f')
            {
                break;
            }

            i++;
        }

        var keyEnd = Math.Min(i, line.Length);
        var j = keyEnd;
        while (j < line.Length && line[j] is ' ' or '\t' or '\f')
        {
            j++;
        }

        if (j < line.Length && line[j] is '=' or ':')
        {
            j++;
            while (j < line.Length && line[j] is ' ' or '\t' or '\f')
            {
                j++;
            }
        }

        return (Unescape(line[..keyEnd]), Unescape(line[j..]));
    }

    private static List<(string Content, string Terminator)> SplitPhysicalLines(string text)
    {
        var lines = new List<(string, string)>();
        var start = 0;
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                lines.Add((text[start..i], "\n"));
                start = i + 1;
            }
            else if (text[i] == '\r')
            {
                var crlf = i + 1 < text.Length && text[i + 1] == '\n';
                lines.Add((text[start..i], crlf ? "\r\n" : "\r"));
                if (crlf)
                {
                    i++;
                }

                start = i + 1;
            }
        }

        if (start < text.Length)
        {
            lines.Add((text[start..], string.Empty));
        }

        return lines;
    }

    private static string DetectNewLine(string text)
    {
        var lf = text.IndexOf('\n');
        if (lf > 0 && text[lf - 1] == '\r')
        {
            return "\r\n";
        }

        if (lf >= 0)
        {
            return "\n";
        }

        return text.Contains('\r') ? "\r" : Environment.NewLine;
    }
}
