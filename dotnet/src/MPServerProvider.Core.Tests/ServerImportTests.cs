using System.IO.Compression;
using System.Text;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class ServerImportTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"mpsp-import-{Guid.NewGuid():N}");

    public ServerImportTests() => Directory.CreateDirectory(_root);

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

    private static GameDefinition Game(string id) => BuiltInGames.All().First(g => g.Id == id);

    private string Folder(string name)
    {
        var dir = Path.Combine(_root, name);
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static void Jar(string path, params (string Name, string Content)[] entries)
    {
        using var zip = ZipFile.Open(path, ZipArchiveMode.Create);
        foreach (var (name, content) in entries)
        {
            using var writer = new StreamWriter(zip.CreateEntry(name).Open(), new UTF8Encoding(false));
            writer.Write(content);
        }
    }

    private static (string, string) Manifest(string mainClass) => ("META-INF/MANIFEST.MF", $"Manifest-Version: 1.0\r\nMain-Class: {mainClass}\r\n");

    private (ServerImportService Service, ServerRepository Repo, AppPaths Paths) Service()
    {
        var paths = AppPaths.ForRoot(Path.Combine(_root, "data"));
        var repo = new ServerRepository(paths);
        return (new ServerImportService(repo), repo, paths);
    }

    [Fact]
    public void Detects_purpur_from_paperclip_jar_and_version_history()
    {
        var dir = Folder("purpur");
        Jar(Path.Combine(dir, "purpur-1.21.11-2568.jar"),
            Manifest("io.papermc.paperclip.Main"),
            ("META-INF/versions.list", "abc\t1.21.11\t1.21.11/purpur-1.21.11.jar\n"));
        File.WriteAllText(Path.Combine(dir, "version_history.json"), "{\"currentVersion\":\"1.21.11-2568-f57bd86 (MC: 1.21.11)\"}");
        File.WriteAllText(Path.Combine(dir, "server.properties"), "motd=hi\nserver-port=25570\n");
        File.WriteAllText(Path.Combine(dir, "eula.txt"), "eula=true\n");

        var d = ServerImportService.Detect(Game("minecraft-java"), dir);

        Assert.True(d.IsValid, d.Problem);
        Assert.Equal("purpur", d.ServerType);
        Assert.Equal("1.21.11 (build 2568)", d.Version);
        Assert.Equal("purpur-1.21.11-2568.jar", d.ServerFile);
        Assert.Equal(25570, d.Port);
        Assert.Equal("plugins", d.ModsFolder);
        Assert.Equal(21, d.JavaMajor);
        Assert.Empty(d.Notes);
    }

    [Fact]
    public void Detects_fabric_launcher_and_vanilla_server_jar()
    {
        var fabric = Folder("fabric");
        Jar(Path.Combine(fabric, "fabric-server-launch.jar"),
            Manifest("net.fabricmc.loader.impl.launch.server.FabricServerLauncher"),
            ("install.properties", "fabric-loader-version=0.16.14\ngame-version=1.21.1\n"));
        var f = ServerImportService.Detect(Game("minecraft-java"), fabric);
        Assert.Equal("fabric", f.ServerType);
        Assert.Equal("1.21.1", f.Version);
        Assert.Equal("mods", f.ModsFolder);
        Assert.Contains(f.Notes, n => n.Contains("EULA"));

        var vanilla = Folder("vanilla");
        Jar(Path.Combine(vanilla, "server.jar"), Manifest("net.minecraft.bundler.Main"), ("version.json", "{\"id\":\"1.20.4\"}"));
        var v = ServerImportService.Detect(Game("minecraft-java"), vanilla);
        Assert.Equal(string.Empty, v.ServerType);
        Assert.Equal("1.20.4", v.Version);
        Assert.Equal(17, v.JavaMajor);
    }

    [Fact]
    public void Detects_forge_argument_files_and_skips_installer()
    {
        var dir = Folder("forge");
        Jar(Path.Combine(dir, "forge-1.20.1-47.2.0-installer.jar"), Manifest("net.minecraftforge.installer.SimpleInstaller"));
        var lib = Path.Combine(dir, "libraries", "net", "minecraftforge", "forge", "1.20.1-47.2.0");
        Directory.CreateDirectory(lib);
        File.WriteAllText(Path.Combine(lib, "win_args.txt"), "-cp x");
        File.WriteAllText(Path.Combine(dir, "user_jvm_args.txt"), "");

        var d = ServerImportService.Detect(Game("minecraft-java"), dir);

        Assert.Equal("forge", d.ServerType);
        Assert.Equal("1.20.1-47.2.0", d.Version);
        Assert.Contains("@user_jvm_args.txt @libraries/net/minecraftforge/forge/1.20.1-47.2.0/win_args.txt", d.StartCommand);
    }

    [Theory]
    [InlineData("21.1.77", "1.21.1")]
    [InlineData("21.0.167", "1.21")]
    [InlineData("20.4.237", "1.20.4")]
    [InlineData("26.1.0.5-beta", "26.1")]
    public void NeoForge_versions_map_to_minecraft(string neo, string mc)
        => Assert.Equal(mc, ServerImportService.NeoForgeMinecraftVersion(neo));

    [Fact]
    public void Folder_without_a_server_is_refused()
    {
        var dir = Folder("empty");
        File.WriteAllText(Path.Combine(dir, "readme.txt"), "x");

        Assert.False(ServerImportService.Detect(Game("minecraft-java"), dir).IsValid);
        var valheim = ServerImportService.Detect(Game("valheim"), dir);
        Assert.False(valheim.IsValid);
        Assert.Contains("valheim_server.exe", valheim.Problem);
    }

    [Fact]
    public void Generic_game_uses_the_definition_executable()
    {
        var dir = Folder("valheim");
        File.WriteAllText(Path.Combine(dir, "valheim_server.exe"), "MZ");

        var d = ServerImportService.Detect(Game("valheim"), dir);

        Assert.True(d.IsValid, d.Problem);
        Assert.Equal("valheim_server.exe", d.ServerFile);
    }

    [Fact]
    public async Task Copy_import_copies_files_registers_and_leaves_source_untouched()
    {
        var dir = Folder("src");
        Jar(Path.Combine(dir, "server.jar"), Manifest("net.minecraft.bundler.Main"), ("version.json", "{\"id\":\"1.21.1\"}"));
        File.WriteAllText(Path.Combine(dir, "server.properties"), "server-port=25565\n");
        Directory.CreateDirectory(Path.Combine(dir, "world", "region"));
        File.WriteAllText(Path.Combine(dir, "world", "region", "r.0.0.mca"), "data");
        var before = Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Select(f => (f, File.ReadAllText(f))).ToList();

        var (service, repo, _) = Service();
        var server = await service.ImportAsync(Game("minecraft-java"), dir, "Imported", 25570, copy: true, new Progress<InstallProgress>());

        Assert.NotEqual(dir, server.ServerRoot);
        Assert.True(repo.IsInsideServersDirs(server.ServerRoot));
        Assert.True(File.Exists(Path.Combine(server.ServerRoot, "world", "region", "r.0.0.mca")));
        Assert.Contains("server-port=25570", File.ReadAllText(Path.Combine(server.ServerRoot, "server.properties")));
        Assert.Equal(before, Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Select(f => (f, File.ReadAllText(f))).ToList());

        repo.Reload();
        Assert.NotNull(repo.Find("Imported"));

        // Same name, and the folder of an existing server, are both refused.
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ImportAsync(Game("minecraft-java"), dir, "Imported", 25570, copy: true, new Progress<InstallProgress>()));
        Assert.NotNull(service.CheckFolder(server.ServerRoot));
    }

    [Fact]
    public async Task In_place_import_keeps_files_where_they_are_and_delete_leaves_them()
    {
        var dir = Folder("inplace");
        File.WriteAllText(Path.Combine(dir, "valheim_server.exe"), "MZ");

        var (service, repo, _) = Service();
        var server = await service.ImportAsync(Game("valheim"), dir, "Valheim Here", 2456, copy: false, new Progress<InstallProgress>());

        Assert.Equal(Path.TrimEndingDirectorySeparator(Path.GetFullPath(dir)), server.ServerRoot);
        Assert.NotNull(service.CheckFolder(dir));

        repo.Reload();
        var loaded = repo.Find("Valheim Here")!;
        Assert.Equal(server.ServerRoot, loaded.ServerRoot);

        repo.Delete(loaded, deleteFiles: true);
        Assert.True(File.Exists(Path.Combine(dir, "valheim_server.exe")));
        Assert.False(File.Exists(loaded.SourceFile));
        Assert.Null(repo.Find("Valheim Here"));
    }

    [Fact]
    public void Folder_containing_the_servers_folder_is_refused()
    {
        var (service, _, paths) = Service();
        Assert.NotNull(service.CheckFolder(_root));
        Assert.NotNull(service.CheckFolder(paths.ServersDir));
    }
}
