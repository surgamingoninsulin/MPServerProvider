using System.IO.Compression;
using System.Text;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class MinecraftModsTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"mpsp-mcmods-{Guid.NewGuid():N}");

    public MinecraftModsTests() => Directory.CreateDirectory(_root);

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

    private static MemoryStream Jar(params (string Name, string Content)[] entries)
    {
        var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (name, content) in entries)
            {
                using var writer = new StreamWriter(zip.CreateEntry(name).Open(), new UTF8Encoding(false));
                writer.Write(content);
            }
        }

        ms.Position = 0;
        return ms;
    }

    private static MinecraftModInfo Read(params (string Name, string Content)[] entries)
    {
        using var jar = Jar(entries);
        return MinecraftJarReader.Read(jar);
    }

    private static ServerInstance Mc(string type, string version) => new()
    {
        ServerName = $"{type}-{version}",
        ServerGame = "minecraft-java",
        ServerType = type,
        ServerVersion = version,
        ModsFolder = type is "paper" or "purpur" or "velocity" or "bungeecord" ? "plugins" : "mods",
    };

    private static ModItem JarItem(MinecraftModInfo info, string name = "thing.jar", string game = "minecraft-java")
        => new() { FileName = name, GameId = game, Minecraft = info };

    // ---- parsing ---------------------------------------------------------------------------

    [Fact]
    public void Fabric_mod_json_gives_kind_name_version_range_and_dependencies()
    {
        var info = Read(("fabric.mod.json", """
            {
              "schemaVersion": 1,
              "id": "sodium",
              "version": "0.6.0",
              "name": "Sodium",
              "depends": { "fabricloader": ">=0.15", "minecraft": "~1.21.1", "fabric-api": "*", "java": ">=21" }
            }
            """));

        Assert.True(info.IsReadable);
        Assert.Equal(MinecraftPlatform.Fabric, info.Platforms);
        Assert.Equal("Sodium", info.Name);
        Assert.Equal("0.6.0", info.Version);
        Assert.Equal("~1.21.1", info.MinecraftRanges[MinecraftPlatform.Fabric]);
        Assert.Equal(new[] { "fabric-api" }, info.RequiredDependencies);
        Assert.True(info.NeedsFabricApi);
        Assert.Contains("Needs Fabric API", ModCompatibility.Tags(JarItem(info)));
        Assert.Contains("MC 1.21.1–1.21.x · Fabric", ModCompatibility.Tags(JarItem(info)));
    }

    [Fact]
    public void Fabric_minecraft_array_becomes_alternatives()
    {
        var info = Read(("fabric.mod.json", """{"id":"x","version":"1","depends":{"minecraft":["1.20","1.20.1"]}}"""));
        Assert.Equal("1.20 || 1.20.1", info.MinecraftRanges[MinecraftPlatform.Fabric]);
        Assert.Contains("MC 1.20–1.20.1 · Fabric", ModCompatibility.Tags(JarItem(info)));
    }

    [Fact]
    public void Quilt_mod_json_is_read()
    {
        var info = Read(("quilt.mod.json", """
            { "schema_version": 1, "quilt_loader": { "id": "qmod", "version": "2.0", "metadata": { "name": "Q Mod" },
              "depends": [ { "id": "minecraft", "versions": ">=1.20" }, "quilted_fabric_api", { "id": "opt", "optional": true } ] } }
            """));
        Assert.Equal(MinecraftPlatform.Quilt, info.Platforms);
        Assert.Equal("Q Mod", info.Name);
        Assert.Equal(">=1.20", info.MinecraftRanges[MinecraftPlatform.Quilt]);
        Assert.Equal(new[] { "quilted_fabric_api" }, info.RequiredDependencies);
    }

    [Fact]
    public void Forge_mods_toml_reads_minecraft_range_and_resolves_jar_version_from_manifest()
    {
        var info = Read(
            ("META-INF/MANIFEST.MF", "Manifest-Version: 1.0\r\nImplementation-Version: 47.1.3\r\n"),
            ("META-INF/mods.toml", """
                modLoader="javafml"
                loaderVersion="[47,)"
                license="MIT"
                [[mods]]
                modId="jei"
                version="${file.jarVersion}"
                displayName="Just Enough Items" # trailing comment
                description='''
                Multi-line [[not a table]]
                '''
                [[dependencies.jei]]
                    modId="forge"
                    mandatory=true
                    versionRange="[47,)"
                [[dependencies.jei]]
                    modId="minecraft"
                    mandatory=true
                    versionRange="[1.20.1,1.21)"
                [[dependencies.jei]]
                    modId="architectury"
                    mandatory=true
                    versionRange="*"
                [[dependencies.jei]]
                    modId="optionalthing"
                    mandatory=false
                """));

        Assert.Equal(MinecraftPlatform.Forge, info.Platforms);
        Assert.Equal("jei", info.Id);
        Assert.Equal("Just Enough Items", info.Name);
        Assert.Equal("47.1.3", info.Version);
        Assert.Equal("[1.20.1,1.21)", info.MinecraftRanges[MinecraftPlatform.Forge]);
        Assert.Equal(new[] { "architectury" }, info.RequiredDependencies);
        Assert.Contains("MC 1.20.1–1.20.x · Forge", ModCompatibility.Tags(JarItem(info)));
    }

    [Fact]
    public void NeoForge_is_detected_from_file_name_or_dependency()
    {
        var byFile = Read(("META-INF/neoforge.mods.toml", """
            [[mods]]
            modId="cool"
            version="1.0"
            [[dependencies.cool]]
            modId="minecraft"
            type="required"
            versionRange="[1.21.1,1.22)"
            """));
        Assert.Equal(MinecraftPlatform.NeoForge, byFile.Platforms);
        Assert.Equal("[1.21.1,1.22)", byFile.MinecraftRanges[MinecraftPlatform.NeoForge]);

        var byDep = Read(("META-INF/mods.toml", """
            [[mods]]
            modId="cool"
            [[dependencies.cool]]
            modId="neoforge"
            type="required"
            """));
        Assert.Equal(MinecraftPlatform.NeoForge, byDep.Platforms);
    }

    [Fact]
    public void Legacy_mcmod_info_is_a_forge_mod()
    {
        var info = Read(("mcmod.info", """[{"modid":"ic2","name":"IndustrialCraft 2","version":"2.8","mcversion":"1.12.2"}]"""));
        Assert.Equal(MinecraftPlatform.Forge, info.Platforms);
        Assert.Equal("IndustrialCraft 2", info.Name);
        Assert.Equal("1.12.2", info.MinecraftRanges[MinecraftPlatform.Forge]);
    }

    [Fact]
    public void Plugin_yml_gives_bukkit_plugin_with_api_version_and_dependencies()
    {
        var info = Read(("plugin.yml", """
            # EssentialsX
            name: Essentials
            version: '2.21.0'
            main: com.earth2me.essentials.Essentials
            api-version: "1.21"
            depend: [Vault]
            softdepend:
              - LuckPerms
            description: |
              Lots of commands
              more text: with colon
            commands:
              home:
                description: go home
            """));
        Assert.Equal(MinecraftPlatform.Bukkit, info.Platforms);
        Assert.Equal("Essentials", info.Name);
        Assert.Equal("2.21.0", info.Version);
        Assert.Equal("1.21", info.ApiVersion);
        Assert.Equal(new[] { "Vault" }, info.RequiredDependencies);
        Assert.Contains("Bukkit plugin · API 1.21", ModCompatibility.Tags(JarItem(info)));
    }

    [Fact]
    public void Block_list_depend_is_read()
    {
        var info = Read(("plugin.yml", "name: X\nversion: 1\ndepend:\n  - Vault\n  - ProtocolLib\n"));
        Assert.Equal(new[] { "Vault", "ProtocolLib" }, info.RequiredDependencies);
    }

    [Fact]
    public void Paper_plugin_yml_reads_required_server_dependencies()
    {
        var info = Read(("paper-plugin.yml", """
            name: Fancy
            version: 1.0.0
            api-version: '1.21'
            dependencies:
              server:
                Vault:
                  load: BEFORE
                  required: true
                Optional:
                  required: false
            """));
        Assert.Equal(MinecraftPlatform.Paper, info.Platforms);
        Assert.Equal(new[] { "Vault" }, info.RequiredDependencies);
        Assert.Contains("Paper plugin · API 1.21", ModCompatibility.Tags(JarItem(info)));
    }

    [Fact]
    public void Proxy_descriptors_and_multi_platform_jars()
    {
        var info = Read(
            ("plugin.yml", "name: LuckPerms\nversion: 5.4\napi-version: 1.13\n"),
            ("bungee.yml", "name: LuckPerms\nversion: 5.4\n"),
            ("velocity-plugin.json", """{"id":"luckperms","name":"LuckPerms","version":"5.4","dependencies":[{"id":"x","optional":true}]}"""));
        Assert.Equal(MinecraftPlatform.Bukkit | MinecraftPlatform.BungeeCord | MinecraftPlatform.Velocity, info.Platforms);
        Assert.Empty(info.RequiredDependencies);
    }

    [Fact]
    public void Unknown_and_unreadable_jars_are_tagged_not_thrown()
    {
        var unknown = Read(("com/example/Lib.class", "cafebabe"));
        Assert.True(unknown.IsUnknown);
        Assert.Equal(new[] { "Unknown jar" }, ModCompatibility.Tags(JarItem(unknown)));

        using var garbage = new MemoryStream(Encoding.ASCII.GetBytes("this is not a zip"));
        var unreadable = MinecraftJarReader.Read(garbage);
        Assert.False(unreadable.IsReadable);
        Assert.Equal(new[] { "Unreadable jar" }, ModCompatibility.Tags(JarItem(unreadable)));

        var broken = Read(("fabric.mod.json", "{ not json"));
        Assert.True(broken.IsReadable);
        Assert.True(broken.IsUnknown);
    }

    // ---- version ranges --------------------------------------------------------------------

    [Theory]
    [InlineData("26.3 (build 252)", "26.3")]
    [InlineData("1.21.10", "1.21.10")]
    [InlineData("1.21.1 (loader 0.16.5)", "1.21.1")]
    [InlineData("1.20.1-47.2.0", "1.20.1")]
    [InlineData("latest", null)]
    [InlineData("24w14a", null)]
    [InlineData("", null)]
    public void Server_minecraft_version_is_parsed(string serverVersion, string? expected)
        => Assert.Equal(expected, MinecraftVersionRange.ServerMinecraftVersion(serverVersion));

    [Theory]
    [InlineData("~1.21.1", "1.21.1", true)]
    [InlineData("~1.21.1", "1.21.4", true)]
    [InlineData("~1.21.1", "1.21", false)]
    [InlineData("~1.21.1", "1.22", false)]
    [InlineData(">=1.20 <1.21", "1.20.6", true)]
    [InlineData(">=1.20 <1.21", "1.21", false)]
    [InlineData(">= 1.20", "26.3", true)]
    [InlineData("1.21.x", "1.21.10", true)]
    [InlineData("1.21.x", "1.20.4", false)]
    [InlineData("1.20.1", "1.20.1", true)]
    [InlineData("1.20.1", "1.20.2", false)]
    [InlineData("1.20 || 1.20.1", "1.20.1", true)]
    [InlineData("*", "26.3", true)]
    [InlineData(">=1.20.5-alpha.24.12.a", "1.21", true)]
    public void Fabric_predicates(string range, string version, bool expected)
        => Assert.Equal(expected, MinecraftVersionRange.Satisfies(range, version, maven: false));

    [Theory]
    [InlineData("[1.20.1,1.21)", "1.20.1", true)]
    [InlineData("[1.20.1,1.21)", "1.20.4", true)]
    [InlineData("[1.20.1,1.21)", "1.21", false)]
    [InlineData("[1.21]", "1.21", true)]
    [InlineData("[1.21]", "1.21.1", false)]
    [InlineData("[1.20,)", "26.3", true)]
    [InlineData("(,1.20)", "1.19.4", true)]
    [InlineData("1.12.2", "1.12.2", true)]
    [InlineData("1.12.2", "1.12", true)]
    [InlineData("1.12.2", "1.20.1", false)]
    public void Maven_ranges(string range, string version, bool expected)
        => Assert.Equal(expected, MinecraftVersionRange.Satisfies(range, version, maven: true));

    [Theory]
    [InlineData("${mc_version_range}", true)]
    [InlineData("24w14a", false)]
    public void Unparseable_ranges_are_unknown_not_false(string range, bool maven)
        => Assert.Null(MinecraftVersionRange.Satisfies(range, "1.21", maven));

    [Theory]
    [InlineData("1.21", "1.21.10", true)]
    [InlineData("1.21", "26.3", true)]
    [InlineData("1.21", "1.20.4", false)]
    [InlineData("1.20.5", "1.20.4", false)]
    public void Plugin_api_version(string api, string server, bool expected)
        => Assert.Equal(expected, MinecraftVersionRange.SatisfiesApiVersion(api, server));

    [Theory]
    [InlineData("1.21.1", false, "1.21.1")]
    [InlineData(">=1.21", false, "1.21+")]
    [InlineData(">=1.21 <1.22", false, "1.21.x")]
    [InlineData("[1.20,1.21)", true, "1.20.x")]
    [InlineData("[1.20.1,1.21.1]", true, "1.20.1–1.21.1")]
    public void Ranges_describe_shortly(string range, bool maven, string expected)
        => Assert.Equal(expected, MinecraftVersionRange.Describe(range, maven));

    // ---- compatibility ---------------------------------------------------------------------

    private static MinecraftModInfo Fabric(string range) => new()
    {
        Platforms = MinecraftPlatform.Fabric,
        MinecraftRanges = new Dictionary<MinecraftPlatform, string> { [MinecraftPlatform.Fabric] = range },
    };

    private static MinecraftModInfo Forge(string range, MinecraftPlatform platform = MinecraftPlatform.Forge) => new()
    {
        Platforms = platform,
        MinecraftRanges = new Dictionary<MinecraftPlatform, string> { [platform] = range },
    };

    private static MinecraftModInfo Plugin(MinecraftPlatform platform, string? api = null) => new() { Platforms = platform, ApiVersion = api };

    [Fact]
    public void Variant_decides_which_platforms_are_offered()
    {
        var fabricMod = JarItem(Fabric(">=1.21"));
        var forgeMod = JarItem(Forge("[1.21,)"));
        var neoMod = JarItem(Forge("[1.21,)", MinecraftPlatform.NeoForge));
        var bukkit = JarItem(Plugin(MinecraftPlatform.Bukkit, "1.21"));
        var paperOnly = JarItem(Plugin(MinecraftPlatform.Paper, "1.21"));
        var velocity = JarItem(Plugin(MinecraftPlatform.Velocity));
        var bungee = JarItem(Plugin(MinecraftPlatform.BungeeCord));

        bool Ok(ModItem item, ServerInstance server) => ModCompatibility.Check(item, server, null).IsCompatible;

        var paper = Mc("paper", "1.21.10");
        Assert.True(Ok(bukkit, paper));
        Assert.True(Ok(paperOnly, paper));
        Assert.False(Ok(fabricMod, paper));
        Assert.False(Ok(velocity, paper));
        Assert.True(Ok(bukkit, Mc("purpur", "1.21.10")));

        var fabric = Mc("fabric", "1.21.10 (loader 0.16.5)");
        Assert.True(Ok(fabricMod, fabric));
        Assert.False(Ok(forgeMod, fabric));
        Assert.False(Ok(bukkit, fabric));
        Assert.Equal("Bukkit plugin — Fabric takes mods", ModCompatibility.Check(bukkit, fabric, null).Reason);

        var forge = Mc("forge", "1.21.1-52.0.1");
        Assert.True(Ok(forgeMod, forge));
        Assert.False(Ok(neoMod, forge));
        Assert.True(Ok(neoMod, Mc("neoforge", "1.21.1")));

        Assert.True(Ok(velocity, Mc("velocity", "3.4.0-SNAPSHOT")));
        Assert.False(Ok(bukkit, Mc("velocity", "3.4.0-SNAPSHOT")));
        Assert.True(Ok(bungee, Mc("bungeecord", "latest")));

        var vanilla = ModCompatibility.Check(fabricMod, Mc("vanilla", "1.21.10"), null);
        Assert.False(vanilla.IsCompatible);
        Assert.Contains("Vanilla", vanilla.Reason);
    }

    [Fact]
    public void Clear_version_mismatch_is_refused_with_a_reason()
    {
        var verdict = ModCompatibility.Check(JarItem(Forge("[1.20,1.21)")), Mc("forge", "26.3"), null);
        Assert.False(verdict.IsCompatible);
        Assert.Equal("Needs MC 1.20.x, server is 26.3", verdict.Reason);

        var plugin = ModCompatibility.Check(JarItem(Plugin(MinecraftPlatform.Bukkit, "1.21")), Mc("paper", "1.20.4 (build 499)"), null);
        Assert.False(plugin.IsCompatible);
        Assert.Equal("Needs MC 1.21+, server is 1.20.4", plugin.Reason);
    }

    [Fact]
    public void Missing_or_unparseable_version_data_is_lenient()
    {
        Assert.True(ModCompatibility.Check(JarItem(new MinecraftModInfo { Platforms = MinecraftPlatform.Fabric }), Mc("fabric", "1.21"), null).IsCompatible);
        Assert.True(ModCompatibility.Check(JarItem(Fabric("~1.20")), Mc("fabric", "latest"), null).IsCompatible);
        Assert.True(ModCompatibility.Check(JarItem(Plugin(MinecraftPlatform.Bukkit)), Mc("paper", "26.3 (build 1)"), null).IsCompatible);
        Assert.True(ModCompatibility.Check(JarItem(new MinecraftModInfo()), Mc("paper", "26.3"), null).IsCompatible); // unknown jar
    }

    [Fact]
    public void Non_jars_and_other_games_never_reach_minecraft_and_vice_versa()
    {
        var paper = Mc("paper", "1.21.10");
        var folder = new ModItem { FileName = "SomeResource", GameId = "minecraft-java", IsDirectory = true };
        Assert.False(ModCompatibility.Check(folder, paper, null).IsCompatible);

        var zip = new ModItem { FileName = "map.zip", GameId = "minecraft-java" };
        Assert.False(ModCompatibility.Check(zip, paper, null).IsCompatible);

        var gmodAddon = new ModItem { FileName = "addon.gma", GameId = "gmod" };
        Assert.False(ModCompatibility.Check(gmodAddon, paper, null).IsCompatible);

        var corrupt = JarItem(new MinecraftModInfo { IsReadable = false, Error = "bad" });
        Assert.False(ModCompatibility.Check(corrupt, paper, null).IsCompatible);

        // A Minecraft plugin jar dropped into another game's library is still refused there.
        var fivem = new ServerInstance { ServerName = "rp", ServerGame = "fivem", ModsFolder = "resources" };
        var mcJarInFivem = JarItem(Plugin(MinecraftPlatform.Bukkit, "1.21"), game: "fivem");
        Assert.False(ModCompatibility.Check(mcJarInFivem, fivem, null).IsCompatible);
        Assert.True(ModCompatibility.Check(new ModItem { FileName = "esx", GameId = "fivem", IsDirectory = true }, fivem, null).IsCompatible);

        var gmod = new ServerInstance { ServerName = "g", ServerGame = "gmod" };
        var gmodGame = new GameDefinition { Id = "gmod", Mods = new ModsSpec { Supported = true, Extensions = new() { ".gma" } } };
        Assert.True(ModCompatibility.Check(gmodAddon, gmod, gmodGame).IsCompatible);
        Assert.False(ModCompatibility.Check(new ModItem { FileName = "x.txt", GameId = "gmod" }, gmod, gmodGame).IsCompatible);
    }

    // ---- library service -------------------------------------------------------------------

    [Fact]
    public void AssignToServer_refuses_incompatible_jars_in_core_and_scan_reads_metadata()
    {
        var paths = AppPaths.ForRoot(Path.Combine(_root, "data"));
        var repo = new ServerRepository(paths);
        var games = new GameLibraryService(paths);
        games.Reload();

        var dir = repo.NewServerDir("Survival");
        Directory.CreateDirectory(dir);
        var server = Mc("paper", "26.3 (build 252)");
        server.ServerName = "Survival";
        server.ServerRoot = dir;
        server.ServerFile = "server.jar";
        repo.Save(server);

        var libDir = Path.Combine(paths.ModsDir, "minecraft-java");
        Directory.CreateDirectory(libDir);
        void WriteJar(string name, params (string, string)[] entries)
        {
            using var jar = Jar(entries);
            File.WriteAllBytes(Path.Combine(libDir, name), jar.ToArray());
        }

        WriteJar("old-forge.jar", ("META-INF/mods.toml", "[[mods]]\nmodId=\"old\"\n[[dependencies.old]]\nmodId=\"minecraft\"\nversionRange=\"[1.20,1.21)\"\n"));
        WriteJar("good-plugin.jar", ("plugin.yml", "name: Good\nversion: 1\napi-version: '1.21'\n"));
        Directory.CreateDirectory(Path.Combine(libDir, "some-folder"));

        var mods = new ModLibraryService(paths, repo, games);
        var scanned = mods.Scan(readMetadata: true);
        Assert.Equal(MinecraftPlatform.Forge, scanned.Single(m => m.FileName == "old-forge.jar").Minecraft!.Platforms);
        Assert.True(scanned.Single(m => m.FileName == "some-folder").IsDirectory);
        Assert.Null(mods.Scan().Single(m => m.FileName == "good-plugin.jar").Minecraft); // metadata is opt-in

        var forgeJar = scanned.Single(m => m.FileName == "old-forge.jar");
        forgeJar.Minecraft = null; // Core must re-read rather than trust the caller
        var ex = Assert.Throws<ModIncompatibleException>(() => mods.AssignToServer(forgeJar, server));
        Assert.Contains("Forge mod — Paper takes plugins", ex.Message);
        Assert.Throws<ModIncompatibleException>(() => mods.AssignToServer(scanned.Single(m => m.FileName == "some-folder"), server));

        mods.AssignToServer(scanned.Single(m => m.FileName == "good-plugin.jar"), server);
        Assert.True(File.Exists(Path.Combine(dir, "plugins", "good-plugin.jar")));
        Assert.Equal(new[] { "Survival" }, mods.Scan().Single(m => m.FileName == "good-plugin.jar").AssignedServers);
        Assert.Equal("Good", mods.ListServerMods(server).Single().Minecraft!.Name);
    }

    [Fact]
    public void Minecraft_import_takes_only_jars()
    {
        var paths = AppPaths.ForRoot(Path.Combine(_root, "data2"));
        var games = new GameLibraryService(paths);
        games.Reload();
        var mods = new ModLibraryService(paths, new ServerRepository(paths), games);

        var source = Path.Combine(_root, "incoming");
        Directory.CreateDirectory(Path.Combine(source, "valheim-plugin-folder"));
        File.WriteAllText(Path.Combine(source, "a.jar"), "x");
        File.WriteAllText(Path.Combine(source, "addon.gma"), "x");

        Assert.Equal(1, mods.ImportFromFolder(source, "minecraft-java"));
        Assert.Equal(new[] { "a.jar" }, Directory.GetFileSystemEntries(Path.Combine(paths.ModsDir, "minecraft-java")).Select(Path.GetFileName));
    }

    [Fact]
    public void Metadata_is_cached_until_the_file_changes()
    {
        var paths = AppPaths.ForRoot(Path.Combine(_root, "data3"));
        var mods = new ModLibraryService(paths, new ServerRepository(paths), new GameLibraryService(paths));
        var path = Path.Combine(_root, "m.jar");
        using (var jar = Jar(("fabric.mod.json", """{"id":"a","name":"A","version":"1"}""")))
        {
            File.WriteAllBytes(path, jar.ToArray());
        }

        var first = mods.GetMinecraftInfo(path);
        Assert.Same(first, mods.GetMinecraftInfo(path));

        using (var jar = Jar(("fabric.mod.json", """{"id":"a","name":"A","version":"2.0.0"}""")))
        {
            File.WriteAllBytes(path, jar.ToArray());
        }

        File.SetLastWriteTimeUtc(path, DateTime.UtcNow.AddMinutes(1));
        Assert.Equal("2.0.0", mods.GetMinecraftInfo(path).Version);
    }
}
