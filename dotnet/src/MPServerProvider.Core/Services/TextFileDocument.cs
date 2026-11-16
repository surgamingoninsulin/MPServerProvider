using System.Text;

namespace MPServerProvider.Core.Services;

/// <summary>Why a file can or can't be opened in the built-in text editor.</summary>
public enum TextEditability
{
    Editable,
    NotFound,
    BinaryExtension,
    BinaryContent,
    TooLarge,
}

/// <summary>
/// Decides whether a server file is safe to show in the built-in text editor: a known-binary
/// extension blocklist (archives, executables, images, game data packs, databases, ...) plus a
/// content sniff of the first <see cref="SniffBytes"/> bytes for NUL bytes, and a size cap so a
/// multi-hundred-MB log can't freeze the UI.
/// </summary>
public static class TextFileDetector
{
    /// <summary>Default maximum size for the built-in editor (5 MB).</summary>
    public const long DefaultMaxEditableBytes = 5L * 1024 * 1024;

    /// <summary>How many leading bytes are sniffed for NUL bytes.</summary>
    public const int SniffBytes = 8192;

    private static readonly HashSet<string> BinaryExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        // archives / packages
        ".jar", ".zip", ".7z", ".rar", ".gz", ".tgz", ".bz2", ".xz", ".tar", ".cab", ".msi", ".nupkg", ".apk", ".vpk", ".pak", ".pk3", ".pk4", ".ucas", ".utoc", ".rpf", ".scs", ".gma", ".bsa", ".ba2", ".mcpack", ".mcworld", ".mcaddon",
        // executables / libraries / compiled code
        ".exe", ".dll", ".so", ".dylib", ".sys", ".bin", ".class", ".pdb", ".lib", ".a", ".o", ".obj", ".pyc", ".luac", ".wasm", ".node",
        // images / media / fonts
        ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".ico", ".tga", ".tif", ".tiff", ".dds", ".psd", ".heic",
        ".mp3", ".wav", ".ogg", ".flac", ".m4a", ".mp4", ".mkv", ".avi", ".mov", ".webm", ".ttf", ".otf", ".woff", ".woff2",
        // data / databases / world saves
        ".db", ".sqlite", ".sqlite3", ".db3", ".mdb", ".dat", ".dat_old", ".mca", ".mcr", ".nbt", ".ldb", ".sav", ".pdf", ".iso", ".img", ".vdi", ".vmdk",
    };

    /// <summary>True when the extension is on the known-binary blocklist.</summary>
    public static bool IsKnownBinaryExtension(string path)
    {
        var ext = Path.GetExtension(path);
        return !string.IsNullOrEmpty(ext) && BinaryExtensions.Contains(ext);
    }

    /// <summary>
    /// True when <paramref name="sample"/> looks like binary data: it contains a NUL byte and is
    /// not UTF-16/UTF-32 text announced by a byte-order mark (which legitimately contains NULs).
    /// </summary>
    public static bool LooksBinary(ReadOnlySpan<byte> sample)
    {
        if (HasUtf16Or32Bom(sample))
        {
            return false;
        }
        return sample.IndexOf((byte)0) >= 0;
    }

    /// <summary>Classifies a file on disk for the built-in editor.</summary>
    public static TextEditability Classify(string path, long maxBytes = DefaultMaxEditableBytes)
    {
        if (!File.Exists(path))
        {
            return TextEditability.NotFound;
        }
        if (IsKnownBinaryExtension(path))
        {
            return TextEditability.BinaryExtension;
        }

        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        if (stream.Length > maxBytes)
        {
            return TextEditability.TooLarge;
        }

        var buffer = new byte[(int)Math.Min(SniffBytes, stream.Length)];
        var read = 0;
        while (read < buffer.Length)
        {
            var n = stream.Read(buffer, read, buffer.Length - read);
            if (n == 0)
            {
                break;
            }
            read += n;
        }
        return LooksBinary(buffer.AsSpan(0, read)) ? TextEditability.BinaryContent : TextEditability.Editable;
    }

    internal static bool HasUtf16Or32Bom(ReadOnlySpan<byte> bytes) =>
        (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE) ||
        (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF) ||
        (bytes.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF);
}

/// <summary>
/// A text file loaded for editing, remembering how it was encoded on disk (encoding, whether it
/// had a byte-order mark, and its dominant line ending) so saving writes it back the same way.
/// <see cref="Text"/> always uses <c>\n</c> line endings internally; <see cref="Save"/>
/// converts every line break back to <see cref="LineEnding"/>.
/// </summary>
public sealed class TextFileDocument
{
    private TextFileDocument(string text, Encoding encoding, bool hasBom, string lineEnding)
    {
        Text = text;
        Encoding = encoding;
        HasBom = hasBom;
        LineEnding = lineEnding;
    }

    /// <summary>Content with every line break normalized to <c>\n</c>.</summary>
    public string Text { get; }

    public Encoding Encoding { get; }

    public bool HasBom { get; }

    /// <summary><c>"\r\n"</c>, <c>"\n"</c> or <c>"\r"</c>.</summary>
    public string LineEnding { get; }

    /// <summary>Short human label, e.g. "UTF-8 BOM · CRLF".</summary>
    public string Description
    {
        get
        {
            var enc = Encoding switch
            {
                UTF8Encoding => HasBom ? "UTF-8 BOM" : "UTF-8",
                UnicodeEncoding u when u.CodePage == 1201 => "UTF-16 BE",
                UnicodeEncoding => "UTF-16 LE",
                UTF32Encoding => "UTF-32",
                _ => Encoding.WebName.ToUpperInvariant(),
            };
            var eol = LineEnding switch { "\r\n" => "CRLF", "\r" => "CR", _ => "LF" };
            return $"{enc} · {eol}";
        }
    }

    /// <summary>Decodes raw file bytes, detecting BOM / encoding / line endings.</summary>
    public static TextFileDocument Parse(byte[] bytes)
    {
        Encoding encoding;
        var hasBom = true;
        int skip;

        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            skip = 3;
        }
        else if (bytes.Length >= 4 && bytes[0] == 0xFF && bytes[1] == 0xFE && bytes[2] == 0x00 && bytes[3] == 0x00)
        {
            encoding = new UTF32Encoding(bigEndian: false, byteOrderMark: true);
            skip = 4;
        }
        else if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
        {
            encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
            skip = 2;
        }
        else if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
        {
            encoding = new UnicodeEncoding(bigEndian: true, byteOrderMark: true);
            skip = 2;
        }
        else
        {
            hasBom = false;
            skip = 0;
            var strictUtf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
            try
            {
                _ = strictUtf8.GetCharCount(bytes);
                encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            }
            catch (DecoderFallbackException)
            {
                // Not valid UTF-8 (legacy ANSI file): Latin-1 maps every byte 1:1 to a char, so
                // an unedited round trip writes back exactly the same bytes.
                encoding = Encoding.Latin1;
            }
        }

        var raw = encoding.GetString(bytes, skip, bytes.Length - skip);
        var lineEnding = DetectLineEnding(raw);
        return new TextFileDocument(NormalizeLineEndings(raw, "\n"), encoding, hasBom, lineEnding);
    }

    /// <summary>Reads and decodes a file.</summary>
    public static TextFileDocument Load(string path) => Parse(File.ReadAllBytes(path));

    /// <summary>
    /// Encodes <paramref name="text"/> (any mix of line breaks) using this document's encoding,
    /// BOM and line ending.
    /// </summary>
    public byte[] Encode(string text)
    {
        var normalized = NormalizeLineEndings(text, LineEnding);
        var body = Encoding.GetBytes(normalized);
        if (!HasBom)
        {
            return body;
        }
        var preamble = Encoding.GetPreamble();
        var result = new byte[preamble.Length + body.Length];
        preamble.CopyTo(result, 0);
        body.CopyTo(result, preamble.Length);
        return result;
    }

    /// <summary>
    /// Writes <paramref name="text"/> to <paramref name="path"/> via a temp file + replace so a
    /// failed write never leaves a half-written config behind.
    /// </summary>
    public void Save(string path, string text)
    {
        var bytes = Encode(text);
        var temp = path + ".mpsp-tmp";
        File.WriteAllBytes(temp, bytes);
        try
        {
            File.Move(temp, path, overwrite: true);
        }
        catch
        {
            try { File.Delete(temp); } catch { /* best-effort cleanup */ }
            throw;
        }
    }

    /// <summary>Returns the dominant line ending in <paramref name="text"/> (CRLF on a tie / none).</summary>
    public static string DetectLineEnding(string text)
    {
        int crlf = 0, lf = 0, cr = 0;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '\r')
            {
                if (i + 1 < text.Length && text[i + 1] == '\n')
                {
                    crlf++;
                    i++;
                }
                else
                {
                    cr++;
                }
            }
            else if (c == '\n')
            {
                lf++;
            }
        }

        if (crlf == 0 && lf == 0 && cr == 0)
        {
            return Environment.NewLine == "\n" ? "\n" : "\r\n";
        }
        if (lf > crlf && lf >= cr)
        {
            return "\n";
        }
        if (cr > crlf && cr > lf)
        {
            return "\r";
        }
        return "\r\n";
    }

    /// <summary>Converts every CRLF / CR / LF in <paramref name="text"/> to <paramref name="lineEnding"/>.</summary>
    public static string NormalizeLineEndings(string text, string lineEnding)
    {
        if (text.Length == 0)
        {
            return text;
        }
        var sb = new StringBuilder(text.Length + 16);
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '\r')
            {
                if (i + 1 < text.Length && text[i + 1] == '\n')
                {
                    i++;
                }
                sb.Append(lineEnding);
            }
            else if (c == '\n')
            {
                sb.Append(lineEnding);
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
