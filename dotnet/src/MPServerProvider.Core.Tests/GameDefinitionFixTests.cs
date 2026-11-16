using System.Text.Json;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Providers;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class MojangJavaMetadataTests
{
    [Fact]
    public void Reads_java_major_from_the_root_of_mojangs_per_version_json()
    {
        // Shape of https://piston-meta.mojang.com/v1/packages/.../26.3.json (trimmed): javaVersion
        // is a ROOT key; downloads.server only has sha1/size/url.
        using var doc = System.Text.Json.JsonDocument.Parse("""
            {
              "id": "26.3",
              "javaVersion": { "component": "java-runtime-epsilon", "majorVersion": 25 },
              "downloads": { "server": { "sha1": "33680f5f", "size": 1, "url": "https://piston-data.mojang.com/x/server.jar" } }
            }
            """);
        Assert.Equal(25, MinecraftJavaProvider.ReadMojangJavaMajor(doc.RootElement));
    }

    [Fact]
    public void Missing_or_malformed_java_version_reads_as_null()
    {
        using var none = System.Text.Json.JsonDocument.Parse("""{ "id": "1.6.4", "downloads": { "server": { "url": "https://x/server.jar" } } }""");
        Assert.Null(MinecraftJavaProvider.ReadMojangJavaMajor(none.RootElement));

        using var bad = System.Text.Json.JsonDocument.Parse("""{ "javaVersion": { "majorVersion": "twenty" } }""");
        Assert.Null(MinecraftJavaProvider.ReadMojangJavaMajor(bad.RootElement));
    }
}

public class JavaRuntimeFallbackTests
{
    // Adoptium /v3/info/available_releases as of Nov 2026.
    private static readonly int[] Available = { 8, 11, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 };
    private static readonly int[] Lts = { 8, 11, 17, 21, 25 };

    [Theory]
    [InlineData(9, 11)]   // never published -> next LTS up
    [InlineData(15, 17)]
    [InlineData(22, 25)]  // newer LTS preferred over the next feature release
    [InlineData(26, 27)]  // no newer LTS -> next feature release
    public void Falls_back_to_the_nearest_newer_published_java(int requested, int expected)
    {
        Assert.Equal(expected, JavaRuntimeProvider.PickFallbackMajor(requested, Available, Lts));
    }

    [Fact]
    public void Never_falls_back_to_an_older_java()
    {
        Assert.Null(JavaRuntimeProvider.PickFallbackMajor(30, Available, Lts));
    }
}

public class MinecraftJavaMajorMigrationTests
{
    [Theory]
    [InlineData("vanilla", "26.3", 8, 25)]        // wrong-path install fell back to Java 8
    [InlineData("paper", "26.1.2 (build 74)", 21, 25)]
    [InlineData("forge", "1.12.2-14.23.5.2860", 8, 8)] // deliberately old runtime stays
    [InlineData("vanilla", "1.21.4", 21, 21)]
    [InlineData("vanilla", "26.3", 0, 0)]         // unset -> definition default, not touched
    [InlineData("velocity", "3.5.1", 17, 17)]     // proxy versions aren't Minecraft versions
    public void Raises_recorded_java_major_only_when_the_version_needs_more(string type, string version, int recorded, int expected)
    {
        var server = new ServerInstance { ServerGame = "minecraft-java", ServerType = type, ServerVersion = version, JavaMajor = recorded };
        var changed = MPServerProvider.Core.Services.ServerRepository.MigrateMinecraftJavaMajor(server);
        Assert.Equal(expected, server.JavaMajor);
        Assert.Equal(expected != recorded, changed);
    }
}

public class BuiltInDefinitionRepairTests : IDisposable
{
    private readonly AppPaths _paths;

    public BuiltInDefinitionRepairTests()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"mpsp-fix-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        _paths = AppPaths.ForRoot(dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_paths.DataRoot, recursive: true); } catch { }
    }

    private static GameDefinition BuiltIn(string id) => BuiltInGames.All().Single(g => g.Id == id);

    private void WriteExisting(GameDefinition game)
        => File.WriteAllText(Path.Combine(_paths.GamesDir, $"{game.Id}.json"), Json.Json.Serialize(game));

    private GameLibraryService Reload()
    {
        var svc = new GameLibraryService(_paths);
        svc.Reload();
        svc.Reload(); // idempotent
        return svc;
    }

    [Fact]
    public void Built_ins_come_from_code_not_from_providers_json()
    {
        var svc = Reload();

        Assert.Empty(Directory.GetFiles(_paths.GamesDir, "*.json"));
        foreach (var builtIn in BuiltInGames.All())
        {
            var loaded = svc.Find(builtIn.Id)!;
            Assert.True(loaded.IsBuiltIn, builtIn.Id);
            Assert.Null(loaded.SourceFile);
        }
    }

    [Fact]
    public void Old_seeded_copy_of_a_built_in_is_moved_to_backup_and_code_wins()
    {
        var old = BuiltIn("openrct2");
        old.Runtime.Arguments = "host {server_file} --port {server_port} --headless";
        WriteExisting(old);

        var svc = Reload();

        Assert.Equal(BuiltIn("openrct2").Runtime.Arguments, svc.Find("openrct2")!.Runtime.Arguments);
        Assert.False(File.Exists(Path.Combine(_paths.GamesDir, "openrct2.json")));
        var backup = Path.Combine(svc.BuiltInBackupDir, "openrct2.json");
        Assert.True(File.Exists(backup));
        Assert.Equal(old.Runtime.Arguments, Json.Json.Deserialize<GameDefinition>(File.ReadAllText(backup))!.Runtime.Arguments);
    }

    [Fact]
    public void Retiring_never_overwrites_an_earlier_backup()
    {
        WriteExisting(BuiltIn("rust"));
        var svc = Reload();
        WriteExisting(BuiltIn("rust"));
        svc.Reload();

        Assert.True(File.Exists(Path.Combine(svc.BuiltInBackupDir, "rust.json")));
        Assert.True(File.Exists(Path.Combine(svc.BuiltInBackupDir, "rust (2).json")));
    }

    [Fact]
    public void Custom_game_from_providers_json_is_loaded_next_to_built_ins()
    {
        var custom = new GameDefinition
        {
            Id = "my-custom-game",
            Name = "My Custom Game",
            Install = new InstallSpec { Provider = "manual" },
            Runtime = new RuntimeSpec { Executable = "{server_root}\\server.exe" },
        };
        WriteExisting(custom);

        var svc = Reload();

        var loaded = svc.Find("my-custom-game")!;
        Assert.False(loaded.IsBuiltIn);
        Assert.Equal(Path.Combine(_paths.GamesDir, "my-custom-game.json"), loaded.SourceFile);
        Assert.Equal(BuiltInGames.All().Count + 1, svc.Games.Count);
    }

    private const string OldPixarkArgs = "CubeWorld_Light?listen?SessionName={server_name}?Port={server_port}?QueryPort=27015 -server -log";

    [Fact]
    public void PixARK_launch_has_the_settings_snails_guide_requires()
    {
        var pixark = BuiltIn("pixark");
        var args = pixark.Runtime.Arguments;
        foreach (var required in new[] { "-CubePort=15000", "-NoBattlEye", "-CULTUREFORCOOKING=en", "-NoHangDetection", "-QueryPort=27015", "-Port={server_port}", "?MaxPlayers=", "?RCONPort=27020" })
        {
            Assert.Contains(required, args);
        }

        Assert.StartsWith("\"CubeWorld_Light?listen?SessionName={server_name}?", args);
        Assert.Contains(pixark.Details!.Ports, p => p.Port == "15000" && p.Protocol == "TCP");
        Assert.Contains(pixark.Details.Ports, p => p.Port == "27014-27015" && p.Protocol == "UDP");
        Assert.Equal("ShooterGame\\Binaries\\Win64\\PixARKServer.exe", pixark.Runtime.Executable.Replace("{server_root}\\", string.Empty));
        Assert.Equal(824360, pixark.Install.AppId);
    }

    [Fact]
    public void Installed_server_with_stale_default_start_command_is_migrated()
    {
        var pixark = new ServerInstance { ServerGame = "pixark", StartCommand = OldPixarkArgs };
        Assert.True(ServerRepository.MigrateStaleStartCommand(pixark));
        Assert.Equal(BuiltIn("pixark").Runtime.Arguments, pixark.StartCommand);
        Assert.False(ServerRepository.MigrateStaleStartCommand(pixark)); // idempotent

        var edited = new ServerInstance { ServerGame = "pixark", StartCommand = OldPixarkArgs + " -CubePort=14999" };
        Assert.False(ServerRepository.MigrateStaleStartCommand(edited));

        var other = new ServerInstance { ServerGame = "rust", StartCommand = OldPixarkArgs };
        Assert.False(ServerRepository.MigrateStaleStartCommand(other));
    }
}

public class GithubReleaseAssetTests
{
    // Asset names of https://api.github.com/repos/OpenRCT2/OpenRCT2/releases/latest (v0.5.7, Nov 2026).
    private const string Release = """
        {
          "tag_name": "v0.5.7",
          "assets": [
            { "name": "OpenRCT2-v0.5.7-Linux-noble-x86_64.tar.gz", "browser_download_url": "https://github.com/OpenRCT2/OpenRCT2/releases/download/v0.5.7/OpenRCT2-v0.5.7-Linux-noble-x86_64.tar.gz" },
            { "name": "OpenRCT2-v0.5.7-macos-universal.zip", "browser_download_url": "https://github.com/OpenRCT2/OpenRCT2/releases/download/v0.5.7/OpenRCT2-v0.5.7-macos-universal.zip" },
            { "name": "OpenRCT2-v0.5.7-windows-installer-x64.exe", "browser_download_url": "https://github.com/OpenRCT2/OpenRCT2/releases/download/v0.5.7/OpenRCT2-v0.5.7-windows-installer-x64.exe" },
            { "name": "OpenRCT2-v0.5.7-windows-portable-arm64.zip", "browser_download_url": "https://github.com/OpenRCT2/OpenRCT2/releases/download/v0.5.7/OpenRCT2-v0.5.7-windows-portable-arm64.zip" },
            { "name": "OpenRCT2-v0.5.7-windows-portable-win32.zip", "browser_download_url": "https://github.com/OpenRCT2/OpenRCT2/releases/download/v0.5.7/OpenRCT2-v0.5.7-windows-portable-win32.zip" },
            { "name": "OpenRCT2-v0.5.7-windows-portable-x64.zip", "browser_download_url": "https://github.com/OpenRCT2/OpenRCT2/releases/download/v0.5.7/OpenRCT2-v0.5.7-windows-portable-x64.zip" },
            { "name": "OpenRCT2-v0.5.7-windows-symbols-x64.zip", "browser_download_url": "https://github.com/OpenRCT2/OpenRCT2/releases/download/v0.5.7/OpenRCT2-v0.5.7-windows-symbols-x64.zip" }
          ]
        }
        """;

    [Fact]
    public void Built_in_pattern_picks_the_windows_x64_portable_zip()
    {
        var spec = BuiltInGames.All().Single(g => g.Id == "openrct2").Install;
        using var doc = JsonDocument.Parse(Release);
        var (url, tag) = UrlProvider.PickReleaseAsset(doc.RootElement, spec.AssetPattern!, spec.GithubRepo!);
        Assert.Equal("https://github.com/OpenRCT2/OpenRCT2/releases/download/v0.5.7/OpenRCT2-v0.5.7-windows-portable-x64.zip", url);
        Assert.Equal("v0.5.7", tag);
        Assert.Equal("OpenRCT2-v0.5.7-windows-portable-x64.zip", UrlProvider.FileNameFor(url));
    }

    [Fact]
    public void No_matching_asset_is_a_clear_error()
    {
        using var doc = JsonDocument.Parse(Release);
        var ex = Assert.Throws<InvalidOperationException>(() => UrlProvider.PickReleaseAsset(doc.RootElement, @"^nothing\.zip$", "OpenRCT2/OpenRCT2"));
        Assert.Contains("v0.5.7", ex.Message);
    }

    [Theory]
    [InlineData("OpenRCT2/OpenRCT2", "https://api.github.com/repos/OpenRCT2/OpenRCT2/releases/latest")]
    [InlineData(" owner/repo.name/ ", "https://api.github.com/repos/owner/repo.name/releases/latest")]
    public void Builds_latest_release_api_url(string repo, string expected)
        => Assert.Equal(expected, UrlProvider.LatestReleaseApiUrl(repo));

    [Theory]
    [InlineData("OpenRCT2")]
    [InlineData("a/b/c")]
    [InlineData("../evil/x")]
    [InlineData("owner/re po")]
    public void Rejects_malformed_repo_names(string repo)
        => Assert.Throws<InvalidOperationException>(() => UrlProvider.LatestReleaseApiUrl(repo));
}

public class CubicOdysseyDefinitionTests
{
    [Fact]
    public void Installs_anonymously_and_passes_the_port()
    {
        // Verified Nov 2026: `steamcmd +login anonymous +app_update 3858450` reports
        // "Success! App '3858450' fully installed." and the shipped help/server_manual.txt
        // documents -Port=BIND_PORT.
        var game = BuiltInGames.All().Single(g => g.Id == "cubic-odyssey");
        Assert.Equal("steamcmd", game.Install.Provider);
        Assert.Equal(3858450, game.Install.AppId);
        Assert.True(game.Install.Anonymous);
        Assert.Equal("{server_root}\\server\\CubicOdysseyServer.exe", game.Runtime.Executable);
        Assert.Equal("-Port={server_port}", game.Runtime.Arguments);
        Assert.Contains(game.Details!.Ports, p => p.Port == "27015");
        Assert.DoesNotContain(game.Details.SetupTips, t => t.Contains("needs a Steam account", StringComparison.Ordinal));
    }
}
