using System.IO.Compression;
using System.Text;
using System.Text.Json;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Reads the descriptor files inside a Minecraft mod/plugin jar (a zip) to learn what it is,
/// which Minecraft versions it supports and what it requires:
/// <c>fabric.mod.json</c>, <c>quilt.mod.json</c>, <c>META-INF/mods.toml</c>,
/// <c>META-INF/neoforge.mods.toml</c>, <c>mcmod.info</c>, <c>plugin.yml</c>,
/// <c>paper-plugin.yml</c>, <c>bungee.yml</c> and <c>velocity-plugin.json</c>.
/// <para>
/// Only the zip's central directory and those few small entries are read (never the classes),
/// and every parser is best-effort: a malformed descriptor contributes nothing instead of
/// throwing, and an unopenable file comes back as <see cref="MinecraftModInfo.IsReadable"/> =
/// <c>false</c>. There are no YAML/TOML libraries in this solution, so the YAML and TOML readers
/// below understand just the subset these descriptors use.
/// </para>
/// </summary>
public static class MinecraftJarReader
{
    /// <summary>Descriptors larger than this are ignored (guards against zip bombs).</summary>
    private const long MaxDescriptorBytes = 1024 * 1024;

    private static readonly JsonDocumentOptions JsonOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };

    /// <summary>Ids that are the game/loader itself rather than a real dependency.</summary>
    private static readonly HashSet<string> NonDependencyIds = new(StringComparer.OrdinalIgnoreCase)
    {
        "minecraft", "java", "fabricloader", "fabric-loader", "quilt_loader", "forge", "neoforge",
        "velocity", "bungeecord", "paper", "spigot", "bukkit",
    };

    /// <summary>Reads <paramref name="path"/>; never throws.</summary>
    public static MinecraftModInfo Read(string path)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            return Read(stream);
        }
        catch (Exception ex)
        {
            return new MinecraftModInfo { IsReadable = false, Error = ex.Message };
        }
    }

    /// <summary>Reads a jar from <paramref name="stream"/>; never throws.</summary>
    public static MinecraftModInfo Read(Stream stream)
    {
        ZipArchive zip;
        try
        {
            zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
        }
        catch (Exception ex)
        {
            return new MinecraftModInfo { IsReadable = false, Error = $"Not a valid jar ({ex.Message})" };
        }

        using (zip)
        {
            var builder = new Builder();
            string? manifestVersion = null;
            try
            {
                manifestVersion = ReadManifestVersion(Text(zip, "META-INF/MANIFEST.MF"));
            }
            catch
            {
                // Best-effort only used to resolve ${file.jarVersion}.
            }

            Try(() => ParseFabric(Text(zip, "fabric.mod.json"), builder));
            Try(() => ParseQuilt(Text(zip, "quilt.mod.json"), builder));
            Try(() => ParseModsToml(Text(zip, "META-INF/neoforge.mods.toml"), builder, neoForgeFile: true, manifestVersion));
            Try(() => ParseModsToml(Text(zip, "META-INF/mods.toml"), builder, neoForgeFile: false, manifestVersion));
            Try(() => ParseMcmodInfo(Text(zip, "mcmod.info"), builder));
            Try(() => ParsePaperPluginYml(Text(zip, "paper-plugin.yml"), builder));
            Try(() => ParsePluginYml(Text(zip, "plugin.yml"), builder));
            Try(() => ParseBungeeYml(Text(zip, "bungee.yml"), builder));
            Try(() => ParseVelocity(Text(zip, "velocity-plugin.json"), builder));

            return builder.Build();
        }
    }

    private static void Try(Action parse)
    {
        try
        {
            parse();
        }
        catch
        {
            // A malformed descriptor contributes nothing; the others still count.
        }
    }

    private static string? Text(ZipArchive zip, string name)
    {
        var entry = zip.GetEntry(name);
        if (entry is null || entry.Length > MaxDescriptorBytes)
        {
            return null;
        }

        using var reader = new StreamReader(entry.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
    }

    /// <summary>Accumulates what each descriptor contributes; the first name/version found wins.</summary>
    internal sealed class Builder
    {
        public MinecraftPlatform Platforms;
        public string? Id;
        public string? Name;
        public string? Version;
        public string? ApiVersion;
        public readonly Dictionary<MinecraftPlatform, string> Ranges = new();
        public readonly List<string> Dependencies = new();

        public void Identity(string? id, string? name, string? version)
        {
            Id ??= Clean(id);
            Name ??= Clean(name);
            Version ??= Clean(version);
        }

        public void Depend(string? id)
        {
            if (!string.IsNullOrWhiteSpace(id) && !NonDependencyIds.Contains(id.Trim())
                && !Dependencies.Contains(id.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                Dependencies.Add(id.Trim());
            }
        }

        public MinecraftModInfo Build() => new()
        {
            Platforms = Platforms,
            Id = Id,
            Name = Name,
            Version = Version,
            ApiVersion = ApiVersion,
            MinecraftRanges = Ranges,
            RequiredDependencies = Dependencies,
        };

        /// <summary>Drops unsubstituted build placeholders such as <c>${version}</c>.</summary>
        private static string? Clean(string? value)
            => string.IsNullOrWhiteSpace(value) || value.Contains("${", StringComparison.Ordinal) ? null : value.Trim();
    }

    // -- Fabric / Quilt / mcmod.info / Velocity (JSON) ----------------------------------------

    internal static void ParseFabric(string? json, Builder b)
    {
        if (json is null)
        {
            return;
        }

        using var doc = JsonDocument.Parse(json, JsonOptions);
        var root = doc.RootElement;
        b.Platforms |= MinecraftPlatform.Fabric;
        b.Identity(Str(root, "id"), Str(root, "name"), Str(root, "version"));

        if (root.TryGetProperty("depends", out var depends) && depends.ValueKind == JsonValueKind.Object)
        {
            foreach (var dep in depends.EnumerateObject())
            {
                if (dep.Name == "minecraft")
                {
                    if (VersionPredicate(dep.Value) is { } range)
                    {
                        b.Ranges[MinecraftPlatform.Fabric] = range;
                    }
                }
                else
                {
                    b.Depend(dep.Name);
                }
            }
        }
    }

    internal static void ParseQuilt(string? json, Builder b)
    {
        if (json is null)
        {
            return;
        }

        using var doc = JsonDocument.Parse(json, JsonOptions);
        if (!doc.RootElement.TryGetProperty("quilt_loader", out var loader) || loader.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        b.Platforms |= MinecraftPlatform.Quilt;
        var name = loader.TryGetProperty("metadata", out var meta) && meta.ValueKind == JsonValueKind.Object ? Str(meta, "name") : null;
        b.Identity(Str(loader, "id"), name, Str(loader, "version"));

        if (!loader.TryGetProperty("depends", out var depends) || depends.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        foreach (var dep in depends.EnumerateArray())
        {
            if (dep.ValueKind == JsonValueKind.String)
            {
                b.Depend(dep.GetString());
                continue;
            }

            if (dep.ValueKind != JsonValueKind.Object || Str(dep, "id") is not { } id)
            {
                continue;
            }

            if (id == "minecraft")
            {
                if (dep.TryGetProperty("versions", out var versions) && VersionPredicate(versions) is { } range)
                {
                    b.Ranges[MinecraftPlatform.Quilt] = range;
                }
            }
            else if (!(dep.TryGetProperty("optional", out var optional) && optional.ValueKind == JsonValueKind.True))
            {
                b.Depend(id);
            }
        }
    }

    internal static void ParseMcmodInfo(string? json, Builder b)
    {
        if (json is null)
        {
            return;
        }

        using var doc = JsonDocument.Parse(json, JsonOptions);
        var list = doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("modList", out var modList)
            ? modList
            : doc.RootElement;
        if (list.ValueKind != JsonValueKind.Array || list.GetArrayLength() == 0 || list[0].ValueKind != JsonValueKind.Object)
        {
            return;
        }

        var first = list[0];
        b.Platforms |= MinecraftPlatform.Forge;
        b.Identity(Str(first, "modid"), Str(first, "name"), Str(first, "version"));
        if (Str(first, "mcversion") is { Length: > 0 } mc && !mc.Contains("${", StringComparison.Ordinal)
            && !b.Ranges.ContainsKey(MinecraftPlatform.Forge))
        {
            b.Ranges[MinecraftPlatform.Forge] = mc;
        }
    }

    internal static void ParseVelocity(string? json, Builder b)
    {
        if (json is null)
        {
            return;
        }

        using var doc = JsonDocument.Parse(json, JsonOptions);
        var root = doc.RootElement;
        b.Platforms |= MinecraftPlatform.Velocity;
        b.Identity(Str(root, "id"), Str(root, "name"), Str(root, "version"));
        if (root.TryGetProperty("dependencies", out var deps) && deps.ValueKind == JsonValueKind.Array)
        {
            foreach (var dep in deps.EnumerateArray())
            {
                if (dep.ValueKind == JsonValueKind.Object
                    && !(dep.TryGetProperty("optional", out var optional) && optional.ValueKind == JsonValueKind.True))
                {
                    b.Depend(Str(dep, "id"));
                }
            }
        }
    }

    private static string? Str(JsonElement obj, string name)
        => obj.ValueKind == JsonValueKind.Object && obj.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

    /// <summary>A Fabric/Quilt version requirement: a string, or an array of alternatives.</summary>
    private static string? VersionPredicate(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.String)
        {
            return value.GetString();
        }

        if (value.ValueKind == JsonValueKind.Array)
        {
            var parts = value.EnumerateArray()
                .Where(e => e.ValueKind == JsonValueKind.String)
                .Select(e => e.GetString())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            return parts.Count == 0 ? null : string.Join(" || ", parts);
        }

        return null;
    }

    // -- Forge / NeoForge (TOML) ----------------------------------------------------------------

    /// <summary>
    /// Reads <c>[[mods]]</c> (<c>modId</c>, <c>version</c>, <c>displayName</c>) and
    /// <c>[[dependencies.&lt;id&gt;]]</c> tables. The <c>minecraft</c> dependency's
    /// <c>versionRange</c> is the supported Minecraft range; a <c>neoforge</c> dependency (or the
    /// <c>neoforge.mods.toml</c> file name) marks the jar as NeoForge rather than Forge.
    /// </summary>
    internal static void ParseModsToml(string? toml, Builder b, bool neoForgeFile, string? manifestVersion)
    {
        if (toml is null)
        {
            return;
        }

        var tables = ParseTomlTables(toml);
        var mod = tables.FirstOrDefault(t => t.Name == "mods")?.Values;
        var modId = mod is null ? null : Get(mod, "modId");

        string? range = null;
        var dependsOnNeoForge = false;
        foreach (var table in tables.Where(t => t.Name.StartsWith("dependencies.", StringComparison.Ordinal)))
        {
            var owner = table.Name["dependencies.".Length..];
            if (modId is not null && owner != modId)
            {
                continue;
            }

            var depId = Get(table.Values, "modId");
            if (depId == "minecraft")
            {
                range ??= Get(table.Values, "versionRange");
                continue;
            }

            if (depId == "neoforge")
            {
                dependsOnNeoForge = true;
                continue;
            }

            var mandatory = Get(table.Values, "mandatory");
            var type = Get(table.Values, "type");
            var required = type is not null
                ? type.Equals("required", StringComparison.OrdinalIgnoreCase)
                : !string.Equals(mandatory, "false", StringComparison.OrdinalIgnoreCase);
            if (required)
            {
                b.Depend(depId);
            }
        }

        var platform = neoForgeFile || dependsOnNeoForge ? MinecraftPlatform.NeoForge : MinecraftPlatform.Forge;
        b.Platforms |= platform;

        if (mod is not null)
        {
            var version = Get(mod, "version");
            if (version is not null && version.Contains("${", StringComparison.Ordinal))
            {
                version = manifestVersion;
            }

            b.Identity(modId, Get(mod, "displayName"), version);
        }

        if (range is not null && !range.Contains("${", StringComparison.Ordinal))
        {
            b.Ranges[platform] = range;
        }
    }

    private static string? Get(Dictionary<string, string> values, string key)
        => values.TryGetValue(key, out var v) ? v : null;

    internal sealed record TomlTable(string Name, Dictionary<string, string> Values);

    /// <summary>
    /// Minimal TOML reader: <c>[table]</c>/<c>[[array-table]]</c> headers and <c>key = value</c>
    /// pairs with basic/literal strings, bare scalars and (skipped) multi-line strings. The root
    /// table is returned with an empty name.
    /// </summary>
    internal static List<TomlTable> ParseTomlTables(string toml)
    {
        var tables = new List<TomlTable> { new(string.Empty, new Dictionary<string, string>()) };
        var lines = toml.Replace("\r\n", "\n").Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            if (line.StartsWith('['))
            {
                var name = line.Trim('[', ']', ' ').Trim();
                var close = name.IndexOf(']');
                if (close >= 0)
                {
                    name = name[..close];
                }

                tables.Add(new TomlTable(name.Trim(), new Dictionary<string, string>()));
                continue;
            }

            var eq = line.IndexOf('=');
            if (eq <= 0)
            {
                continue;
            }

            var key = line[..eq].Trim().Trim('"');
            var raw = line[(eq + 1)..].Trim();

            if (raw.StartsWith("'''", StringComparison.Ordinal) || raw.StartsWith("\"\"\"", StringComparison.Ordinal))
            {
                var fence = raw[..3];
                var rest = raw[3..];
                var sb = new StringBuilder();
                while (!rest.Contains(fence, StringComparison.Ordinal) && i + 1 < lines.Length)
                {
                    sb.Append(rest).Append('\n');
                    rest = lines[++i];
                }

                var end = rest.IndexOf(fence, StringComparison.Ordinal);
                sb.Append(end >= 0 ? rest[..end] : rest);
                tables[^1].Values[key] = sb.ToString();
                continue;
            }

            tables[^1].Values[key] = TomlScalar(raw);
        }

        return tables;
    }

    private static string TomlScalar(string raw)
    {
        if (raw.Length > 0 && raw[0] is '"' or '\'')
        {
            var quote = raw[0];
            var end = raw.IndexOf(quote, 1);
            return end > 0 ? raw[1..end] : raw[1..];
        }

        var hash = raw.IndexOf('#');
        return (hash >= 0 ? raw[..hash] : raw).Trim();
    }

    private static string? ReadManifestVersion(string? manifest)
    {
        if (manifest is null)
        {
            return null;
        }

        foreach (var line in manifest.Replace("\r\n", "\n").Split('\n'))
        {
            if (line.StartsWith("Implementation-Version:", StringComparison.OrdinalIgnoreCase))
            {
                return line["Implementation-Version:".Length..].Trim();
            }
        }

        return null;
    }

    // -- Bukkit / Paper / BungeeCord (YAML) -----------------------------------------------------

    internal static void ParsePluginYml(string? yaml, Builder b)
    {
        if (yaml is null)
        {
            return;
        }

        var root = ParseYaml(yaml);
        b.Platforms |= MinecraftPlatform.Bukkit;
        b.Identity(YamlString(root, "name"), YamlString(root, "name"), YamlString(root, "version"));
        b.ApiVersion ??= YamlString(root, "api-version");
        foreach (var dep in YamlList(root, "depend"))
        {
            b.Depend(dep);
        }
    }

    internal static void ParsePaperPluginYml(string? yaml, Builder b)
    {
        if (yaml is null)
        {
            return;
        }

        var root = ParseYaml(yaml);
        b.Platforms |= MinecraftPlatform.Paper;
        b.Identity(YamlString(root, "name"), YamlString(root, "name"), YamlString(root, "version"));
        b.ApiVersion ??= YamlString(root, "api-version");

        // dependencies: { server: { Vault: { load: BEFORE, required: true } } } — required by default.
        if (root.TryGetValue("dependencies", out var deps) && deps is Dictionary<string, object?> depMap
            && depMap.TryGetValue("server", out var server) && server is Dictionary<string, object?> serverDeps)
        {
            foreach (var (name, value) in serverDeps)
            {
                var required = value is not Dictionary<string, object?> props
                    || !(props.TryGetValue("required", out var r) && r is string rs && rs.Equals("false", StringComparison.OrdinalIgnoreCase));
                if (required)
                {
                    b.Depend(name);
                }
            }
        }
    }

    internal static void ParseBungeeYml(string? yaml, Builder b)
    {
        if (yaml is null)
        {
            return;
        }

        var root = ParseYaml(yaml);
        b.Platforms |= MinecraftPlatform.BungeeCord;
        b.Identity(YamlString(root, "name"), YamlString(root, "name"), YamlString(root, "version"));
        foreach (var dep in YamlList(root, "depends").Concat(YamlList(root, "depend")))
        {
            b.Depend(dep);
        }
    }

    private static string? YamlString(Dictionary<string, object?> map, string key)
        => map.TryGetValue(key, out var v) && v is string s && s.Length > 0 ? s : null;

    private static IEnumerable<string> YamlList(Dictionary<string, object?> map, string key)
        => map.TryGetValue(key, out var v)
            ? v switch
            {
                List<string> list => list,
                string s when s.Length > 0 => new[] { s },
                _ => Array.Empty<string>(),
            }
            : Array.Empty<string>();

    /// <summary>
    /// Minimal block-YAML reader for plugin descriptors: nested mappings by indentation,
    /// <c>- item</c> sequences of scalars, <c>[a, b]</c> flow sequences and quoted scalars.
    /// Block scalars (<c>|</c>/<c>&gt;</c>) and anything fancier are skipped.
    /// </summary>
    internal static Dictionary<string, object?> ParseYaml(string yaml)
    {
        var lines = yaml.Replace("\r\n", "\n").Replace('\t', ' ').Split('\n')
            .Where(l => l.Trim().Length > 0 && !l.TrimStart().StartsWith('#') && l.Trim() != "---")
            .ToList();
        var i = 0;
        return ParseMapping(lines, ref i, lines.Count > 0 ? Indent(lines[0]) : 0);
    }

    private static int Indent(string line) => line.Length - line.TrimStart(' ').Length;

    private static Dictionary<string, object?> ParseMapping(List<string> lines, ref int i, int indent)
    {
        var map = new Dictionary<string, object?>(StringComparer.Ordinal);
        while (i < lines.Count)
        {
            var lineIndent = Indent(lines[i]);
            if (lineIndent < indent)
            {
                break;
            }

            if (lineIndent > indent)
            {
                i++; // stray deeper line (e.g. block scalar content): skip
                continue;
            }

            var line = lines[i].Trim();
            var colon = FindKeyColon(line);
            if (colon <= 0)
            {
                i++;
                continue;
            }

            var key = Unquote(line[..colon].Trim());
            var rest = StripComment(line[(colon + 1)..].Trim());
            i++;

            if (rest.Length == 0)
            {
                if (i < lines.Count && Indent(lines[i]) >= indent && lines[i].TrimStart().StartsWith('-')
                    && (Indent(lines[i]) > indent || Indent(lines[i]) == indent))
                {
                    map[key] = ParseSequence(lines, ref i, Indent(lines[i]));
                }
                else if (i < lines.Count && Indent(lines[i]) > indent)
                {
                    map[key] = ParseMapping(lines, ref i, Indent(lines[i]));
                }
                else
                {
                    map[key] = null;
                }
            }
            else if (rest is "|" or ">" or "|-" or ">-" or "|+" or ">+")
            {
                while (i < lines.Count && Indent(lines[i]) > indent)
                {
                    i++;
                }

                map[key] = string.Empty;
            }
            else if (rest.StartsWith('['))
            {
                map[key] = rest.Trim('[', ']')
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(Unquote)
                    .Where(s => s.Length > 0)
                    .ToList();
            }
            else
            {
                map[key] = Unquote(rest);
            }
        }

        return map;
    }

    private static List<string> ParseSequence(List<string> lines, ref int i, int indent)
    {
        var list = new List<string>();
        while (i < lines.Count && Indent(lines[i]) == indent && lines[i].TrimStart().StartsWith('-'))
        {
            var item = StripComment(lines[i].TrimStart()[1..].Trim());
            if (item.Length > 0)
            {
                list.Add(Unquote(item));
            }

            i++;
            while (i < lines.Count && Indent(lines[i]) > indent)
            {
                i++; // nested content under a sequence item: not needed
            }
        }

        return list;
    }

    /// <summary>The <c>key:</c> colon (followed by space or end), ignoring colons inside quotes.</summary>
    private static int FindKeyColon(string line)
    {
        var quote = '\0';
        for (var j = 0; j < line.Length; j++)
        {
            var c = line[j];
            if (quote != '\0')
            {
                if (c == quote) quote = '\0';
            }
            else if (c is '"' or '\'')
            {
                quote = c;
            }
            else if (c == ':' && (j + 1 == line.Length || line[j + 1] == ' '))
            {
                return j;
            }
        }

        return -1;
    }

    private static string StripComment(string value)
    {
        if (value.Length > 0 && value[0] is '"' or '\'')
        {
            return value;
        }

        var hash = value.IndexOf(" #", StringComparison.Ordinal);
        return hash >= 0 ? value[..hash].TrimEnd() : value;
    }

    private static string Unquote(string value)
    {
        var v = value.Trim();
        if (v.Length >= 2 && ((v[0] == '"' && v[^1] == '"') || (v[0] == '\'' && v[^1] == '\'')))
        {
            return v[1..^1];
        }

        return v;
    }
}
