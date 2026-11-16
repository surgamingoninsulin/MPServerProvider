using System.IO.Compression;
using System.Text;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

/// <summary>
/// Drag-and-drop installs (<see cref="ModLibraryService.InstallFilesToServer"/>,
/// <see cref="ModLibraryService.ImportFilesToLibrary"/>, <see cref="FileDrop"/>), the console's
/// plugin linkage-error hint (<see cref="PluginErrorHint"/>) and the mcstatus.io response parser
/// (<see cref="ReachabilityCheck"/>).
/// </summary>
public class DropAndPluginHintTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"mpsp-drop-{Guid.NewGuid():N}");

    public DropAndPluginHintTests() => Directory.CreateDirectory(_root);

    public void Dispose()
    {
        try
        {
            Directory.Delete(_root, recursive: true);
        }
        catch
        {
            // Best-effort temp cleanup.
        }
    }

    private static void WriteJar(string path, params (string Name, string Content)[] entries)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using var zip = ZipFile.Open(path, ZipArchiveMode.Create);
        foreach (var (name, content) in entries)
        {
            using var writer = new StreamWriter(zip.CreateEntry(name).Open(), new UTF8Encoding(false));
            writer.Write(content);
        }
    }

    private (ModLibraryService Mods, ServerInstance Server, string PluginsDir) PaperServer(string version = "26.3 (build 2658)")
    {
        var paths = AppPaths.ForRoot(Path.Combine(_root, "data"));
        var repo = new ServerRepository(paths);
        var games = new GameLibraryService(paths);
        games.Reload();
        var dir = repo.NewServerDir("Pylon");
        Directory.CreateDirectory(dir);
        var server = new ServerInstance
        {
            ServerName = "Pylon",
            ServerGame = "minecraft-java",
            ServerType = "purpur",
            ServerVersion = version,
            ServerRoot = dir,
            ServerFile = "purpur_server.jar",
            ModsFolder = "plugins",
        };
        repo.Save(server);
        return (new ModLibraryService(paths, repo, games), server, Path.Combine(dir, "plugins"));
    }

    [Fact]
    public void Dropped_jars_install_with_the_compatibility_gate_and_report_skips_and_duplicates()
    {
        var (mods, server, plugins) = PaperServer();
        var incoming = Path.Combine(_root, "incoming");
        WriteJar(Path.Combine(incoming, "good-2.0.jar"), ("plugin.yml", "name: Good\nversion: 2.0\napi-version: '1.21'\n"));
        WriteJar(Path.Combine(incoming, "fabric-mod.jar"), ("fabric.mod.json", """{"id":"f","name":"F","version":"1"}"""));
        File.WriteAllText(Path.Combine(incoming, "readme.txt"), "x");
        WriteJar(Path.Combine(plugins, "good-1.0.jar"), ("plugin.yml", "name: Good\nversion: 1.0\napi-version: '1.21'\n"));

        var result = mods.InstallFilesToServer(
            new[] { Path.Combine(incoming, "good-2.0.jar"), Path.Combine(incoming, "fabric-mod.jar"), Path.Combine(incoming, "readme.txt") },
            server);

        Assert.Equal(new[] { "good-2.0.jar" }, result.Added);
        Assert.True(File.Exists(Path.Combine(plugins, "good-2.0.jar")));
        Assert.Equal(("good-2.0.jar", "good-1.0.jar"), result.Duplicates.Single());
        Assert.Contains(result.Skipped, s => s.Name == "fabric-mod.jar" && s.Reason.Contains("takes plugins"));
        Assert.Contains(result.Skipped, s => s.Name == "readme.txt" && s.Reason.Contains(".jar"));
        Assert.False(File.Exists(Path.Combine(plugins, "fabric-mod.jar")));
    }

    [Fact]
    public void Dropped_folder_on_minecraft_installs_the_jars_inside_it()
    {
        var (mods, server, plugins) = PaperServer();
        var folder = Path.Combine(_root, "pack");
        WriteJar(Path.Combine(folder, "a.jar"), ("plugin.yml", "name: A\nversion: 1\n"));
        WriteJar(Path.Combine(folder, "b.jar"), ("plugin.yml", "name: B\nversion: 1\n"));
        Directory.CreateDirectory(Path.Combine(_root, "empty"));

        var result = mods.InstallFilesToServer(new[] { folder, Path.Combine(_root, "empty") }, server);

        Assert.Equal(2, result.Count);
        Assert.True(File.Exists(Path.Combine(plugins, "a.jar")));
        Assert.False(Directory.Exists(Path.Combine(plugins, "pack")));
        Assert.Contains(result.Skipped, s => s.Name == "empty");

        // Dropping a jar onto the folder it already lives in is a no-op.
        var again = mods.InstallFilesToServer(new[] { Path.Combine(plugins, "a.jar") }, server);
        Assert.Equal(0, again.Count);
        Assert.Single(again.Skipped);
    }

    [Fact]
    public void Dropped_files_import_into_the_library_for_the_game()
    {
        var (mods, _, _) = PaperServer();
        var incoming = Path.Combine(_root, "lib-in");
        WriteJar(Path.Combine(incoming, "x.jar"), ("plugin.yml", "name: X\nversion: 1\n"));
        File.WriteAllText(Path.Combine(incoming, "x.zip"), "x");

        var result = mods.ImportFilesToLibrary(new[] { Path.Combine(incoming, "x.jar"), Path.Combine(incoming, "x.zip") }, "minecraft-java");

        Assert.Equal(new[] { "x.jar" }, result.Added);
        Assert.Single(result.Skipped);
        Assert.Contains(mods.Scan(), m => m.FileName == "x.jar" && m.GameId == "minecraft-java");
    }

    [Fact]
    public void FileDrop_merges_folders_and_skips_links()
    {
        var source = Path.Combine(_root, "src");
        Directory.CreateDirectory(Path.Combine(source, "sub"));
        File.WriteAllText(Path.Combine(source, "sub", "a.txt"), "new");
        var target = Path.Combine(_root, "dst", "src");
        Directory.CreateDirectory(Path.Combine(target, "sub"));
        File.WriteAllText(Path.Combine(target, "sub", "a.txt"), "old");
        File.WriteAllText(Path.Combine(target, "keep.txt"), "keep");

        FileDrop.Copy(source, target);

        Assert.Equal("new", File.ReadAllText(Path.Combine(target, "sub", "a.txt")));
        Assert.True(File.Exists(Path.Combine(target, "keep.txt")));

        var file = Path.Combine(_root, "one.txt");
        File.WriteAllText(file, "1");
        FileDrop.Copy(file, Path.Combine(_root, "dst", "one.txt"));
        Assert.Equal("1", File.ReadAllText(Path.Combine(_root, "dst", "one.txt")));
    }

    [Fact]
    public void Linkage_error_names_the_plugin_and_its_dependency_once()
    {
        var (_, server, plugins) = PaperServer();
        WriteJar(Path.Combine(plugins, "ConstructionWand-0.9.0.jar"),
            ("plugin.yml", "name: ConstructionWand\nversion: 0.9.0\nmain: com.balugaq.constructionwand.implementation.ConstructionWandPlugin\ndepend:\n  - Rebar\napi-version: \"1.21\"\n"),
            ("com/balugaq/constructionwand/implementation/ConstructionWandPlugin.class", "x"));
        WriteJar(Path.Combine(plugins, "rebar-0.45.0-26.3.jar"),
            ("paper-plugin.yml", "name: Rebar\nversion: 0.45.0-26.3\napi-version: \"26.3\"\n"));

        const string line = "[11:01:12 INFO]: Steve lost connection: Internal Exception: io.netty.handler.codec.EncoderException: " +
                            "java.lang.AbstractMethodError: Method com/balugaq/constructionwand/implementation/ConstructionWandPlugin.getDefaultLanguage()Ljava/util/Locale; is abstract";
        var hinted = new HashSet<string>();

        var hint = PluginErrorHint.ForLine(line, server, hinted);

        Assert.NotNull(hint);
        Assert.StartsWith("[MP Server Provider]", hint);
        Assert.Contains("ConstructionWand-0.9.0.jar", hint);
        Assert.Contains("Rebar 0.45.0-26.3", hint);
        Assert.Contains("AbstractMethodError", hint);
        Assert.Null(PluginErrorHint.ForLine(line, server, hinted)); // once per class
        Assert.Null(PluginErrorHint.ForLine(hint!, server, new HashSet<string>())); // never explains itself
        Assert.Null(PluginErrorHint.ForLine("[INFO]: Done (3.2s)!", server, new HashSet<string>()));
    }

    [Fact]
    public void Mcstatus_response_is_parsed()
    {
        var online = ReachabilityCheck.Parse(
            """{"online":true,"host":"x","port":25565,"version":{"name_clean":"Purpur 26.3"},"players":{"online":1,"max":20}}""",
            "play.example.net:25565");
        Assert.True(online.Online);
        Assert.Equal("Online from the internet: play.example.net:25565 · Purpur 26.3 · 1/20 players", online.Text);

        var offline = ReachabilityCheck.Parse("""{"online":false,"host":"x","port":25565}""", "1.2.3.4:25565");
        Assert.False(offline.Online);
        Assert.Contains("not reachable", offline.Text);
    }
}
