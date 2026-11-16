using JsonHelper = MPServerProvider.Core.Json.Json;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class ServicesTests
{
    private static AppPaths TempPaths()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"mpsp-services-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        return AppPaths.ForRoot(dir);
    }

    // ---- BuiltInGames -------------------------------------------------

    [Fact]
    public void BuiltInGames_All_has_the_original_games_and_unique_ids()
    {
        var games = BuiltInGames.All();
        Assert.True(games.Count > 14);

        var ids = games.Select(g => g.Id).ToList();
        Assert.Contains("minecraft-java", ids);
        Assert.Contains("minecraft-bedrock", ids);
        Assert.Contains("fivem", ids);
        Assert.Contains("openrct2", ids);

        var distinct = ids.Distinct().ToList();
        Assert.Equal(ids.Count, distinct.Count);
    }

    [Fact]
    public void BuiltInGames_minecraft_java_matches_known_shape()
    {
        var mc = BuiltInGames.All().Single(g => g.Id == "minecraft-java");
        Assert.Equal("minecraft-java", mc.Install.Provider);
        Assert.Contains("paper", mc.Install.Variants);
        Assert.Equal(25565, mc.Network.DefaultPort);
        Assert.True(mc.Mods.Supported);
    }

    [Fact]
    public void BuiltInGames_every_entry_has_required_fields()
    {
        foreach (var game in BuiltInGames.All())
        {
            Assert.False(string.IsNullOrEmpty(game.Id));
            Assert.False(string.IsNullOrEmpty(game.Name));
            Assert.Equal(1, game.ManifestVersion);
        }
    }

    // ---- BuiltInNews / NewsService -------------------------------------

    [Fact]
    public void BuiltInNews_has_the_eight_original_entries_plus_one_per_added_game()
    {
        var news = BuiltInNews.All();
        var added = BuiltInGames.AddedGames();
        Assert.Equal(8 + added.Count, news.Count);
        Assert.All(news, n => Assert.Equal("Added", n.Tag));
        Assert.All(news, n => Assert.NotNull(n.Image));
        foreach (var game in added)
        {
            var card = Assert.Single(news, n => n.GameId == game.Id);
            Assert.Equal($"Added {game.Name}", card.Title);
            Assert.InRange(card.Summary.Length, 20, 400);
            Assert.DoesNotContain("\n", card.Summary);
        }
    }

    [Fact]
    public void NewsService_Load_returns_items_sorted_newest_first()
    {
        var items = new NewsService().Load();
        Assert.NotEmpty(items);
        for (var i = 0; i < items.Count - 1; i++)
        {
            Assert.True(items[i].Date >= items[i + 1].Date);
        }
    }

    // ---- GameLibraryService --------------------------------------------

    [Fact]
    public void GameLibraryService_Reload_seeds_and_loads_built_in_catalog()
    {
        var paths = TempPaths();
        var svc = new GameLibraryService(paths);
        svc.Reload();

        var games = svc.Games;
        Assert.Equal(BuiltInGames.All().Count, games.Count);
        Assert.All(games, g => Assert.True(g.IsBuiltIn));
        Assert.NotNull(svc.Find("minecraft-java"));
        Assert.Null(svc.Find("does-not-exist"));

        Directory.Delete(paths.DataRoot, recursive: true);
    }

    [Fact]
    public void GameLibraryService_Reload_picks_up_user_added_manifest_as_not_built_in()
    {
        var paths = TempPaths();
        var custom = new GameDefinition
        {
            ManifestVersion = 1,
            Id = "custom-game",
            Name = "Custom Game",
            Genres = new List<string> { "Action" },
        };
        File.WriteAllText(Path.Combine(paths.GamesDir, "custom-game.json"), JsonHelper.Serialize(custom));

        var svc = new GameLibraryService(paths);
        svc.Reload();

        var customGame = svc.Find("custom-game");
        Assert.NotNull(customGame);
        Assert.False(customGame!.IsBuiltIn);
        Assert.Contains("Action", svc.AllGenres());

        Directory.Delete(paths.DataRoot, recursive: true);
    }

    [Fact]
    public void GameLibraryService_AllGenres_are_distinct_and_sorted()
    {
        var paths = TempPaths();
        var svc = new GameLibraryService(paths);
        svc.Reload();

        var genres = svc.AllGenres();
        var sorted = genres.OrderBy(g => g, StringComparer.Ordinal).ToList();
        Assert.Equal(sorted, genres);
        Assert.Equal(genres.Distinct().ToList(), genres);

        Directory.Delete(paths.DataRoot, recursive: true);
    }

    // ---- ServerRepository ------------------------------------------------

    private static ServerInstance SampleServer(string root)
    {
        return new ServerInstance
        {
            ManifestVersion = 1,
            ServerGame = "cubic-odyssey-dedicated-server-beta-version",
            ServerGenres = new List<string> { "Action", "Adventure" },
            ServerVersion = "latest (app 3858450)",
            ServerType = string.Empty,
            ServerName = "My Cubic Odyssey Dedicated Server",
            ServerDescription = "Cubic Odyssey Dedicated Server (Beta Version)",
            ServerRoot = root,
            ServerFile = "server\\CubicOdysseyServer.exe",
            ServerIp = "0.0.0.0",
            ServerPort = 27015,
            RamMinMb = 1024,
            RamMaxMb = 4096,
            JavaMajor = 0,
            StartCommand = "\"{server_root}\\server\\CubicOdysseyServer.exe\"",
            StopCommand = "quit",
            ModsFolder = null,
            AutoRestartOnCrash = true,
            CreateFirewallRule = true,
            Tunnel = new TunnelSpec(),
            Stats = new ServerStats
            {
                TotalUptimeSeconds = 187,
                MonthUptimeSeconds = 187,
                MonthKey = "2026-07",
                CrashCount = 0,
                LastStarted = null,
            },
            SourceFile = null,
        };
    }

    [Fact]
    public void ServerRepository_save_then_reload_round_trips()
    {
        var paths = TempPaths();
        var repo = new ServerRepository(paths);

        var dir = Path.Combine(paths.ServersDir, "My_Cubic_Odyssey_Dedicated_Server");
        Directory.CreateDirectory(dir);
        var server = SampleServer(dir);
        server.SourceFile = Path.Combine(dir, "server.json");
        repo.Save(server);

        Assert.True(File.Exists(Path.Combine(dir, "server.json")));

        repo.Reload();
        var found = repo.Find("My Cubic Odyssey Dedicated Server");
        Assert.NotNull(found);
        Assert.Equal(27015, found!.ServerPort);
        Assert.Equal("cubic-odyssey-dedicated-server-beta-version", found.ServerGame);

        Directory.Delete(paths.DataRoot, recursive: true);
    }

    [Fact]
    public void ServerRepository_name_taken_detects_existing_directory_or_loaded_server()
    {
        var paths = TempPaths();
        var repo = new ServerRepository(paths);
        Assert.False(repo.NameTaken("Fresh Server"));

        var dir = repo.NewServerDir("Fresh Server");
        Directory.CreateDirectory(dir);
        Assert.True(repo.NameTaken("Fresh Server"));

        Directory.Delete(paths.DataRoot, recursive: true);
    }

    [Fact]
    public void ServerRepository_delete_removes_server_from_memory_and_fires_event()
    {
        var paths = TempPaths();
        var repo = new ServerRepository(paths);
        var dir = Path.Combine(paths.ServersDir, "ToDelete");
        Directory.CreateDirectory(dir);
        var server = SampleServer(dir);
        server.ServerName = "ToDelete";
        server.SourceFile = Path.Combine(dir, "server.json");
        repo.Save(server);

        var deleted = false;
        repo.Deleted += _ => deleted = true;

        repo.Delete(server, deleteFiles: true);
        Assert.Null(repo.Find("ToDelete"));
        Assert.False(Directory.Exists(dir));
        Assert.True(deleted);

        Directory.Delete(paths.DataRoot, recursive: true);
    }

    // ---- SettingsService --------------------------------------------------

    [Fact]
    public void SettingsService_Load_defaults_when_no_settings_file_exists()
    {
        var paths = TempPaths();
        var svc = new SettingsService(paths);
        var defaults = new AppSettings();
        Assert.Equal(defaults.StartOnBoot, svc.Settings.StartOnBoot);
        Assert.Equal(defaults.NewsAutoScroll, svc.Settings.NewsAutoScroll);
        Assert.Equal(defaults.AssumedHostingPricePerHour, svc.Settings.AssumedHostingPricePerHour);

        Directory.Delete(paths.DataRoot, recursive: true);
    }

    [Fact]
    public void SettingsService_round_trips_against_real_settings_json_shape()
    {
        // Ground truth: data/settings.json.
        const string realSettingsJson = """
        {
          "start_on_boot": false,
          "start_minimized_to_tray": false,
          "restore_running_servers": false,
          "news_auto_scroll": true,
          "firewall_rules": [
            {
              "name": "minecraft",
              "port": 25565,
              "protocol": "both",
              "active": true,
              "created": "2026-07-15T12:43:16.9806051+02:00"
            }
          ],
          "assumed_hosting_price_per_hour": 0.02
        }
        """;

        var settings = JsonHelper.Deserialize<AppSettings>(realSettingsJson);
        Assert.NotNull(settings);
        Assert.Null(settings!.InstallRoot);
        Assert.True(settings.NewsAutoScroll);
        Assert.Single(settings.FirewallRules);
        Assert.Equal("minecraft", settings.FirewallRules[0].Name);
        Assert.Equal(0.02, settings.AssumedHostingPricePerHour);

        var paths = TempPaths();
        File.WriteAllText(paths.SettingsFile, realSettingsJson);
        var svc = new SettingsService(paths);
        Assert.Equal(settings.NewsAutoScroll, svc.Settings.NewsAutoScroll);
        Assert.Equal(settings.FirewallRules[0].Name, svc.Settings.FirewallRules[0].Name);
        Assert.Equal(settings.AssumedHostingPricePerHour, svc.Settings.AssumedHostingPricePerHour);

        Directory.Delete(paths.DataRoot, recursive: true);
    }

    [Fact]
    public void SettingsService_save_persists_and_set_install_root_overrides_servers_dir()
    {
        var paths = TempPaths();
        var svc = new SettingsService(paths) { ManageStartupEntry = false };

        var newRoot = Path.Combine(paths.DataRoot, "custom-install-root");
        svc.SetInstallRoot(newRoot);

        Assert.Equal(newRoot, paths.ServersDir);
        var onDisk = JsonHelper.Deserialize<AppSettings>(File.ReadAllText(paths.SettingsFile));
        Assert.Equal(newRoot, onDisk!.InstallRoot);

        Directory.Delete(paths.DataRoot, recursive: true);
    }

    // ---- InstallService -----------------------------------------------

    private static GameDefinition SampleGame(string id, string provider)
    {
        return new GameDefinition
        {
            ManifestVersion = 1,
            Id = id,
            Name = $"{id} name",
            Genres = new List<string> { "Sandbox" },
            Install = new InstallSpec { Provider = provider },
            Runtime = new RuntimeSpec { Arguments = "--some-arg", StopCommand = "quit" },
            Mods = new ModsSpec { Supported = true, Folder = "mods" },
            IsBuiltIn = true,
        };
    }

    [Fact]
    public void InstallService_ProviderFor_resolves_known_provider_and_falls_back_to_url()
    {
        var paths = TempPaths();
        var servers = new ServerRepository(paths);
        var svc = new InstallService(paths, servers);

        var steamcmdGame = SampleGame("rust", "steamcmd");
        Assert.Equal("steamcmd", svc.ProviderFor(steamcmdGame).Id);

        var unknownGame = SampleGame("mystery", "not-a-real-provider");
        Assert.Equal("url", svc.ProviderFor(unknownGame).Id);

        Directory.Delete(paths.DataRoot, recursive: true);
    }

    [Fact]
    public void InstallService_BuildInstance_fills_defaults_from_game_and_result()
    {
        var game = SampleGame("rust", "steamcmd");
        var request = new InstallRequest
        {
            Game = game,
            ServerName = "My Rust Server",
            Variant = null,
            Version = "latest (app 258550)",
            TargetDir = "C:\\data\\servers\\My_Rust_Server",
            Port = 28015,
            RamMinMb = 512,
            RamMaxMb = 2048,
            Description = null,
            EulaAccepted = true,
        };
        var result = new InstallResult
        {
            ServerFile = "RustDedicated.exe",
            Version = "latest (app 258550)",
            StartCommand = null,
            JavaMajor = 0,
        };

        var instance = InstallService.BuildInstance(game, request, result);
        Assert.Equal("rust", instance.ServerGame);
        Assert.Equal("My Rust Server", instance.ServerName);
        Assert.Equal("rust name", instance.ServerDescription);
        // FIX: previously asserted the buggy "\"RustDedicated.exe\" --some-arg" shape (a
        // prepended literal server file in front of the game's own arguments template), which
        // garbled any template that references {server_file} itself (e.g. Minecraft Java's
        // `-jar "{server_file}"`). The default StartCommand is now just the game's own Runtime
        // .Arguments template verbatim, left for ServerProcessService.BuildCommandAsync to
        // substitute at launch time — see its own fix for the full placeholder set.
        Assert.Equal("--some-arg", instance.StartCommand);
        Assert.Equal("quit", instance.StopCommand);
        Assert.Equal(28015, instance.ServerPort);
        Assert.Equal("mods", instance.ModsFolder);
    }

    [Fact]
    public void InstallService_BuildInstance_prefers_explicit_start_command_from_result()
    {
        var game = SampleGame("fivem", "cfx");
        var request = new InstallRequest
        {
            Game = game,
            ServerName = "My FiveM Server",
            Variant = null,
            Version = null,
            TargetDir = "C:\\data\\servers\\My_FiveM_Server",
            Port = 30120,
            RamMinMb = 0,
            RamMaxMb = 0,
            Description = "Custom description",
            EulaAccepted = true,
        };
        var result = new InstallResult
        {
            ServerFile = "artifact\\FXServer.exe",
            Version = "7290",
            StartCommand = "\"artifact\\FXServer.exe\" +exec server.cfg",
            JavaMajor = 0,
        };

        var instance = InstallService.BuildInstance(game, request, result);
        Assert.Equal("\"artifact\\FXServer.exe\" +exec server.cfg", instance.StartCommand);
        Assert.Equal("Custom description", instance.ServerDescription);
    }

    // ---- DownloadService -----------------------------------------------

    [Fact]
    public void DownloadService_FormatBytes_matches_thresholds()
    {
        Assert.Equal("512 B", DownloadService.FormatBytes(512));
        Assert.Equal("2.0 KB", DownloadService.FormatBytes(2048));
        Assert.Equal("5.0 MB", DownloadService.FormatBytes(5 * 1024 * 1024));
        Assert.Equal("3.0 GB", DownloadService.FormatBytes(3L * 1024 * 1024 * 1024));
    }

    [Fact]
    public async Task DownloadService_VerifyHashAsync_matches_known_sha256()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"mpsp-download-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        var file = Path.Combine(dir, "hello.txt");
        File.WriteAllText(file, "hello world");

        const string expected = "b94d27b9934d3e08a52e52d7da7dabfac484efe37a5380ee9088f7ace2efcde9";
        Assert.True(await DownloadService.VerifyHashAsync(file, expected));
        Assert.False(await DownloadService.VerifyHashAsync(file, "deadbeef"));

        Directory.Delete(dir, recursive: true);
    }

    [Fact]
    public void DownloadService_FlattenSingleRootFolder_hoists_nested_contents()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"mpsp-download-flatten-{Guid.NewGuid():N}");
        var nested = Path.Combine(dir, "project-v1.2.3");
        Directory.CreateDirectory(nested);
        File.WriteAllText(Path.Combine(nested, "file.txt"), "x");

        DownloadService.FlattenSingleRootFolder(dir);

        Assert.True(File.Exists(Path.Combine(dir, "file.txt")));
        Assert.False(Directory.Exists(nested));

        Directory.Delete(dir, recursive: true);
    }
}
