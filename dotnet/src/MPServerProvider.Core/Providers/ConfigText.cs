using System.Text;

namespace MPServerProvider.Core.Providers;

internal static class ConfigText
{
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

    public static string SingleLine(string? value)
        => new((value ?? string.Empty).Select(c => char.IsControl(c) ? ' ' : c).ToArray());
}
