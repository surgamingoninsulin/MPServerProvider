using System.Text.RegularExpressions;

namespace MPServerProvider.Core.Services;

public static class MinecraftVersionRange
{
    internal readonly record struct Interval(int[]? Lower, bool LowerInclusive, int[]? Upper, bool UpperInclusive)
    {
        public static readonly Interval Any = new(null, true, null, true);

        public bool Contains(int[] v)
        {
            if (Lower is not null)
            {
                var c = Compare(v, Lower);
                if (c < 0 || (c == 0 && !LowerInclusive))
                {
                    return false;
                }
            }

            if (Upper is not null)
            {
                var c = Compare(v, Upper);
                if (c > 0 || (c == 0 && !UpperInclusive))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsExact => Lower is not null && Upper is not null && LowerInclusive && UpperInclusive && Compare(Lower, Upper) == 0;

        public Interval Intersect(Interval other)
        {
            var (lower, lowerInc) = (Lower, LowerInclusive);
            if (other.Lower is not null)
            {
                var c = lower is null ? -1 : Compare(lower, other.Lower);
                if (c < 0 || (c == 0 && !other.LowerInclusive))
                {
                    (lower, lowerInc) = (other.Lower, other.LowerInclusive);
                }
            }

            var (upper, upperInc) = (Upper, UpperInclusive);
            if (other.Upper is not null)
            {
                var c = upper is null ? 1 : Compare(upper, other.Upper);
                if (c > 0 || (c == 0 && !other.UpperInclusive))
                {
                    (upper, upperInc) = (other.Upper, other.UpperInclusive);
                }
            }

            return new Interval(lower, lowerInc, upper, upperInc);
        }
    }

    private static readonly Regex LeadingVersion = new(@"^\s*v?(\d+(?:\.\d+)+|\d+(?=\s|$))", RegexOptions.CultureInvariant);

    public static string? ServerMinecraftVersion(string? serverVersion)
    {
        if (string.IsNullOrWhiteSpace(serverVersion))
        {
            return null;
        }

        var match = LeadingVersion.Match(serverVersion);
        return match.Success && match.Groups[1].Value.Contains('.') ? match.Groups[1].Value : null;
    }

    internal static int[]? ParseVersion(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var s = text.Trim();
        var cut = s.IndexOfAny(new[] { '-', '+', ' ' });
        if (cut >= 0)
        {
            s = s[..cut];
        }

        var parts = s.Split('.');
        if (parts.Length == 0 || parts.Length > 4)
        {
            return null;
        }

        var result = new int[parts.Length];
        for (var i = 0; i < parts.Length; i++)
        {
            if (!int.TryParse(parts[i], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out result[i]))
            {
                return null;
            }
        }

        return result;
    }

    internal static int Compare(int[] a, int[] b)
    {
        var n = Math.Max(a.Length, b.Length);
        for (var i = 0; i < n; i++)
        {
            var x = i < a.Length ? a[i] : 0;
            var y = i < b.Length ? b[i] : 0;
            if (x != y)
            {
                return x < y ? -1 : 1;
            }
        }

        return 0;
    }

    public static bool? Satisfies(string? range, string? version, bool maven)
    {
        var v = ParseVersion(version);
        var intervals = maven ? ParseMaven(range) : ParseFabric(range);
        if (v is null || intervals is null)
        {
            return null;
        }

        return intervals.Any(i => i.Contains(v));
    }

    public static bool? SatisfiesApiVersion(string? apiVersion, string? serverVersion)
    {
        var api = ParseVersion(apiVersion);
        var v = ParseVersion(serverVersion);
        if (api is null || v is null)
        {
            return null;
        }

        // Compare at the precision the plugin declared (1.21 vs a 1.21.10 server is a match).
        var trimmed = v.Length > api.Length ? v[..api.Length] : v;
        return Compare(trimmed, api) >= 0;
    }

    public static string Describe(string? range, bool maven)
    {
        if (string.IsNullOrWhiteSpace(range))
        {
            return "any";
        }

        var intervals = maven ? ParseMaven(range) : ParseFabric(range);
        if (intervals is null || intervals.Count == 0)
        {
            return range.Trim();
        }

        if (intervals.All(i => i.IsExact))
        {
            var points = intervals.Select(i => i.Lower!).OrderBy(p => p, Comparer<int[]>.Create(Compare)).ToList();
            return points.Count == 1 ? Format(points[0]) : $"{Format(points[0])}–{Format(points[^1])}";
        }

        // Overall hull of the alternatives: lowest lower bound .. highest upper bound.
        int[]? lower = null;
        var lowerUnbounded = false;
        int[]? upper = null;
        var upperInclusive = false;
        var upperUnbounded = false;
        foreach (var i in intervals)
        {
            if (i.Lower is null) lowerUnbounded = true;
            else if (lower is null || Compare(i.Lower, lower) < 0) lower = i.Lower;

            if (i.Upper is null) upperUnbounded = true;
            else if (upper is null || Compare(i.Upper, upper) > 0 || (Compare(i.Upper, upper) == 0 && i.UpperInclusive))
            {
                upper = i.Upper;
                upperInclusive = i.UpperInclusive;
            }
        }

        if (lowerUnbounded) lower = null;
        if (upperUnbounded) upper = null;

        if (lower is null && upper is null)
        {
            return "any";
        }

        var upperText = upper is null ? null : DescribeUpper(upper, upperInclusive);
        if (lower is null)
        {
            return $"≤{upperText}";
        }

        if (upperText is null)
        {
            return $"{Format(lower)}+";
        }

        // [1.21, 1.22) → "1.21.x"; [1.21.1, 1.22) → "1.21.1–1.21.x".
        if (upperText == $"{Family(lower)}.x")
        {
            return lower.Length < 3 || lower[2] == 0 ? upperText : $"{Format(lower)}–{upperText}";
        }

        return $"{Format(lower)}–{upperText}";
    }

    private static string DescribeUpper(int[] upper, bool inclusive)
    {
        if (inclusive)
        {
            return Format(upper);
        }

        // "< 1.22" means "up to 1.21.x"; "< 1.21.2" means "up to 1.21.1".
        var patch = upper.Length >= 3 ? upper[2] : 0;
        if (patch > 0)
        {
            return $"{upper[0]}.{(upper.Length > 1 ? upper[1] : 0)}.{patch - 1}";
        }

        var minor = upper.Length >= 2 ? upper[1] : 0;
        return minor > 0 ? $"{upper[0]}.{minor - 1}.x" : $"<{Format(upper)}";
    }

    private static string Family(int[] v) => $"{v[0]}.{(v.Length > 1 ? v[1] : 0)}";

    private static string Format(int[] v)
    {
        // Drop a trailing ".0" patch ("1.21.0" → "1.21") — that's how Minecraft names them.
        var len = v.Length;
        while (len > 2 && v[len - 1] == 0)
        {
            len--;
        }

        return string.Join('.', v.Take(len));
    }

    private static Interval FamilyOf(int[] v)
    {
        var major = v[0];
        var minor = v.Length > 1 ? v[1] : 0;
        return new Interval(v, true, new[] { major, minor + 1 }, false);
    }

    // -- Fabric / Quilt predicates -------------------------------------------------------------

    internal static List<Interval>? ParseFabric(string? predicate)
    {
        if (predicate is null)
        {
            return null;
        }

        var result = new List<Interval>();
        foreach (var alternative in predicate.Split("||"))
        {
            var text = alternative.Trim();
            if (text.Length == 0)
            {
                continue;
            }

            // Tolerate a space between operator and version (">= 1.20").
            text = Regex.Replace(text, @"(>=|<=|>|<|=|~|\^)\s+", "$1");
            var interval = Interval.Any;
            foreach (var token in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (ParseFabricComparator(token) is not { } part)
                {
                    return null;
                }

                interval = interval.Intersect(part);
            }

            result.Add(interval);
        }

        return result.Count == 0 ? null : result;
    }

    private static Interval? ParseFabricComparator(string token)
    {
        if (token is "*" or "x" or "X")
        {
            return Interval.Any;
        }

        string op;
        if (token.StartsWith(">=", StringComparison.Ordinal) || token.StartsWith("<=", StringComparison.Ordinal))
        {
            op = token[..2];
        }
        else if (token[0] is '>' or '<' or '=' or '~' or '^')
        {
            op = token[..1];
        }
        else
        {
            op = string.Empty;
        }

        var body = token[op.Length..];

        // Wildcards: 1.21.x / 1.21.* / 1.x.
        var wildcard = Regex.Match(body, @"^(\d+(?:\.\d+)*)\.[xX*]$");
        if (wildcard.Success)
        {
            var prefix = ParseVersion(wildcard.Groups[1].Value);
            if (prefix is null)
            {
                return null;
            }

            var upper = (int[])prefix.Clone();
            upper[^1]++;
            return new Interval(prefix, true, upper, false);
        }

        var v = ParseVersion(body);
        if (v is null)
        {
            return null;
        }

        switch (op)
        {
            case ">=": return new Interval(v, true, null, true);
            case ">": return new Interval(v, false, null, true);
            case "<=": return new Interval(null, true, v, true);
            case "<": return new Interval(null, true, v, false);
            case "~": return FamilyOf(v);
            case "^": return new Interval(v, true, new[] { v[0] + 1 }, false);
            default: return new Interval(v, true, v, true);
        }
    }

    // -- Forge / NeoForge Maven ranges ---------------------------------------------------------

    internal static List<Interval>? ParseMaven(string? spec)
    {
        if (string.IsNullOrWhiteSpace(spec))
        {
            return null;
        }

        var text = spec.Trim();
        if (text.Contains("${", StringComparison.Ordinal))
        {
            return null;
        }

        if (text[0] is not ('[' or '('))
        {
            // Soft requirement: the whole minor line (1.12.2 → [1.12, 1.13)).
            return ParseVersion(text) is { } soft
                ? new List<Interval> { FamilyOf(new[] { soft[0], soft.Length > 1 ? soft[1] : 0 }) }
                : null;
        }

        var result = new List<Interval>();
        foreach (Match m in Regex.Matches(text, @"([\[\(])([^\]\)]*)([\]\)])"))
        {
            var lowerInc = m.Groups[1].Value == "[";
            var upperInc = m.Groups[3].Value == "]";
            var body = m.Groups[2].Value;
            var comma = body.IndexOf(',');
            if (comma < 0)
            {
                // [1.21] = exactly that version.
                var exact = ParseVersion(body);
                if (exact is null)
                {
                    return null;
                }

                result.Add(new Interval(exact, true, exact, true));
                continue;
            }

            var lowText = body[..comma].Trim();
            var highText = body[(comma + 1)..].Trim();
            var low = lowText.Length == 0 ? null : ParseVersion(lowText);
            var high = highText.Length == 0 ? null : ParseVersion(highText);
            if ((lowText.Length > 0 && low is null) || (highText.Length > 0 && high is null))
            {
                return null;
            }

            result.Add(new Interval(low, lowerInc, high, upperInc));
        }

        return result.Count == 0 ? null : result;
    }
}
