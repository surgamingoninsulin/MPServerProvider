using System.IO.Compression;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Explains Java linkage errors (<c>AbstractMethodError</c>, <c>NoSuchMethodError</c>, …) in a
/// Minecraft server's console. They mean a plugin/mod was compiled against a different version
/// of the server or of a plugin it depends on — e.g. ConstructionWand 0.9.0 on Rebar 0.45 kicks
/// every joining player with <c>EncoderException: AbstractMethodError: …ConstructionWandPlugin
/// .getDefaultLanguage()</c>. The class named in the error is looked up in the server's jars so
/// the hint names the plugin file to update or remove.
/// </summary>
public static partial class PluginErrorHint
{
    // Package prefixes of the JDK, the server itself and common shaded libraries: never "the
    // plugin at fault".
    private static readonly string[] PlatformPrefixes =
    {
        "java/", "javax/", "jdk/", "sun/", "kotlin/", "net/minecraft/", "com/mojang/", "org/bukkit/",
        "org/spigotmc/", "io/papermc/", "com/destroystokyo/", "org/purpurmc/", "io/netty/",
        "net/fabricmc/", "net/minecraftforge/", "net/neoforged/", "org/slf4j/", "org/apache/",
    };

    [GeneratedRegex(@"\b(AbstractMethodError|NoSuchMethodError|NoSuchFieldError|NoClassDefFoundError|IncompatibleClassChangeError)\b")]
    private static partial Regex ErrorRegex();

    // Class-like tokens: three or more identifier segments joined by '/' or '.'.
    [GeneratedRegex(@"[A-Za-z_$][\w$]*(?:[/.][A-Za-z_$][\w$]*){2,}")]
    private static partial Regex ClassTokenRegex();

    /// <summary>
    /// A one-line <c>[MP Server Provider]</c> hint for <paramref name="line"/>, or <c>null</c>.
    /// <paramref name="hinted"/> (per server run) makes sure each class is explained once.
    /// </summary>
    public static string? ForLine(string line, ServerInstance server, HashSet<string> hinted)
    {
        if (line.StartsWith("[MP Server Provider]", StringComparison.Ordinal)
            || !ModCompatibility.IsMinecraftJava(server.ServerGame))
        {
            return null;
        }

        var error = ErrorRegex().Match(line);
        if (!error.Success
            || string.IsNullOrWhiteSpace(server.ModsFolder)
            || !ModLibraryService.IsSafeRelativeFolder(server.ModsFolder))
        {
            return null;
        }

        var dir = Path.Combine(server.ServerRoot, server.ModsFolder);
        if (!Directory.Exists(dir))
        {
            return null;
        }

        foreach (Match token in ClassTokenRegex().Matches(line, error.Index + error.Length))
        {
            var classPath = token.Value.Replace('.', '/');
            if (PlatformPrefixes.Any(p => classPath.StartsWith(p, StringComparison.Ordinal)))
            {
                continue;
            }

            lock (hinted)
            {
                if (!hinted.Add(classPath))
                {
                    return null;
                }
            }

            try
            {
                if (FindJarContaining(dir, classPath) is { } jar)
                {
                    return Describe(error.Value, jar, dir);
                }
            }
            catch
            {
                // Best-effort: an unreadable jar just means no hint.
            }
        }

        return null;
    }

    /// <summary>The jar in <paramref name="dir"/> holding the class <paramref name="classPath"/>
    /// (<c>a/b/C</c>, possibly followed by a member name, which is trimmed off).</summary>
    internal static string? FindJarContaining(string dir, string classPath)
    {
        var candidates = new List<string>();
        var path = classPath;
        while (path.Count(c => c == '/') >= 2)
        {
            candidates.Add(path + ".class");
            path = path[..path.LastIndexOf('/')];
        }

        foreach (var jar in Directory.EnumerateFiles(dir, "*.jar"))
        {
            try
            {
                using var zip = ZipFile.OpenRead(jar);
                if (candidates.Any(c => zip.GetEntry(c) is not null))
                {
                    return jar;
                }
            }
            catch (InvalidDataException)
            {
                // Not a valid zip; skip.
            }
            catch (IOException)
            {
                // Locked/unreadable; skip.
            }
        }

        return null;
    }

    private static string Describe(string error, string jar, string dir)
    {
        var file = Path.GetFileName(jar);
        var info = MinecraftJarReader.Read(jar);
        var name = info.Name ?? Path.GetFileNameWithoutExtension(file);

        var installedDeps = new List<string>();
        if (info.RequiredDependencies.Count > 0)
        {
            foreach (var other in Directory.EnumerateFiles(dir, "*.jar"))
            {
                if (string.Equals(other, jar, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var dep = MinecraftJarReader.Read(other);
                if (dep.Name is { } depName && info.RequiredDependencies.Contains(depName, StringComparer.OrdinalIgnoreCase))
                {
                    installedDeps.Add(dep.Version is { Length: > 0 } v ? $"{depName} {v}" : depName);
                }
            }
        }

        var cause = installedDeps.Count > 0
            ? $"was built for a different version of {string.Join(" / ", installedDeps)} than the one installed"
            : "was built for a different server or plugin version";
        return $"[MP Server Provider] Plugin problem: {name} ({file}) {cause} ({error}). " +
               $"Players who join can get kicked with this error. Install a {name} build made for these versions, or remove {file} in the Mods tab.";
    }
}
