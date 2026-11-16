using System.Text;

namespace MPServerProvider.Core.Providers;

/// <summary>
/// Escaping helpers for user-supplied text (server name, description) written into generated
/// server config files. Without these a quote or newline in a name could break the file or inject
/// extra keys (e.g. <c>online-mode=false</c>, <c>Private = false</c>, an <c>AuthKey</c>).
/// </summary>
internal static class ConfigText
{
    /// <summary>
    /// Escapes <paramref name="value"/> for use inside a TOML basic string (<c>"..."</c>):
    /// backslash and quote are escaped, newlines/tabs use their short escapes, any other control
    /// character becomes <c>\uXXXX</c>.
    /// </summary>
    public static string TomlString(string? value)
    {
        var sb = new StringBuilder();
        foreach (var c in value ?? string.Empty)
        {
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    if (char.IsControl(c))
                    {
                        sb.Append("\\u").Append(((int)c).ToString("X4"));
                    }
                    else
                    {
                        sb.Append(c);
                    }

                    break;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Escapes <paramref name="value"/> as a Java <c>.properties</c> value (what Minecraft Java
    /// reads <c>server.properties</c> with): backslash, <c>=</c>, <c>:</c>, <c>#</c>, <c>!</c> are
    /// backslash-escaped, line breaks/tabs use <c>\n</c>/<c>\r</c>/<c>\t</c>, and every
    /// non-printable-ASCII character becomes a <c>\uXXXX</c> escape.
    /// </summary>
    public static string JavaPropertiesValue(string? value)
    {
        var sb = new StringBuilder();
        foreach (var c in value ?? string.Empty)
        {
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                case '=': case ':': case '#': case '!': sb.Append('\\').Append(c); break;
                default:
                    if (c < 0x20 || c > 0x7e)
                    {
                        sb.Append("\\u").Append(((int)c).ToString("X4"));
                    }
                    else
                    {
                        sb.Append(c);
                    }

                    break;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// For config parsers without an escape syntax (Bedrock's <c>server.properties</c>): keeps
    /// the value on one line by replacing every control character with a space.
    /// </summary>
    public static string SingleLine(string? value)
        => new((value ?? string.Empty).Select(c => char.IsControl(c) ? ' ' : c).ToArray());
}
