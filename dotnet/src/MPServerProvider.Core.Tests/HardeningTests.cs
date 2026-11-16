using System.Buffers.Binary;
using System.Formats.Tar;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Providers;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

/// <summary>
/// Regression tests for the security/robustness audit fixes (item numbers refer to the audit).
/// </summary>
public class HardeningTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"mpsp-hardening-{Guid.NewGuid():N}");

    public HardeningTests() => Directory.CreateDirectory(_root);

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

    private AppPaths Paths(string name = "data") => AppPaths.ForRoot(Path.Combine(_root, name));

    private static ServerInstance Server(string name, string root) => new()
    {
        ServerName = name,
        ServerGame = "valheim",
        ServerRoot = root,
        ServerFile = "server.exe",
        ServerPort = 2456,
    };

    // ---- #1 working directory --------------------------------------------------------------

    [Fact]
    public void Working_dir_template_is_substituted_not_appended()
    {
        var server = Server("A", @"C:\servers\A");
        var game = new GameDefinition { Id = "g", Runtime = new RuntimeSpec { WorkingDir = "{server_root}" } };
        Assert.Equal(@"C:\servers\A", ServerProcessService.ResolveWorkingDirectory(server, game));

        game.Runtime.WorkingDir = "{server_root}\\bin";
        Assert.Equal(@"C:\servers\A\bin", ServerProcessService.ResolveWorkingDirectory(server, game));

        game.Runtime.WorkingDir = "bin";
        Assert.Equal(Path.Combine(@"C:\servers\A", "bin"), ServerProcessService.ResolveWorkingDirectory(server, game));

        game.Runtime.WorkingDir = string.Empty;
        Assert.Equal(@"C:\servers\A", ServerProcessService.ResolveWorkingDirectory(server, game));
    }

    [Fact]
    public async Task Start_passes_the_resolved_server_root_as_working_directory()
    {
        var root = Path.Combine(_root, "srv");
        Directory.CreateDirectory(root);
        var game = new GameDefinition
        {
            Id = "minecraft-test",
            Runtime = new RuntimeSpec { Executable = "{server_root}\\server.exe", Arguments = "-name \"{server_name}\"", WorkingDir = "{server_root}" },
        };
        var factory = new RecordingRunnerFactory();
        using var service = new ServerProcessService(new NullPersistence(), new SingleGame(game), factory);

        var server = Server("Has\"Quote", root);
        server.ServerGame = game.Id;
        await service.StartAsync(server);

        Assert.Equal(root, factory.WorkingDirectory);
        Assert.Equal(Path.Combine(root, "server.exe").Replace('/', '\\'), factory.Executable!.Replace('/', '\\'));
        Assert.Equal("-name \"HasQuote\"", factory.Arguments); // quote stripped from {server_name}
    }

    [Fact]
    public async Task Start_fails_clearly_when_working_directory_is_missing()
    {
        var game = new GameDefinition { Id = "minecraft-test", Runtime = new RuntimeSpec { Executable = "x.exe", WorkingDir = "{server_root}" } };
        using var service = new ServerProcessService(new NullPersistence(), new SingleGame(game), new RecordingRunnerFactory());
        var server = Server("Missing", Path.Combine(_root, "does-not-exist"));
        server.ServerGame = game.Id;

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.StartAsync(server));
        Assert.Contains("Working directory not found", ex.Message);
    }

    // ---- #2 / #21 repository delete containment & failure ----------------------------------

    [Fact]
    public void Delete_refuses_folders_outside_the_servers_dir()
    {
        var paths = Paths();
        var repo = new ServerRepository(paths);
        var victim = Path.Combine(_root, "Documents");
        Directory.CreateDirectory(victim);
        File.WriteAllText(Path.Combine(victim, "precious.txt"), "x");

        var evil = Server("Evil", victim);
        Assert.Throws<InvalidOperationException>(() => repo.Delete(evil, deleteFiles: true));
        Assert.True(File.Exists(Path.Combine(victim, "precious.txt")));

        var relative = Server("Dot", ".");
        Assert.Throws<InvalidOperationException>(() => repo.Delete(relative, deleteFiles: true));

        var traversal = Server("Up", Path.Combine(paths.ServersDir, "..", "..", "Documents"));
        Assert.Throws<InvalidOperationException>(() => repo.Delete(traversal, deleteFiles: true));
        Assert.True(File.Exists(Path.Combine(victim, "precious.txt")));
    }

    [Fact]
    public void Delete_removes_a_server_inside_the_servers_dir_and_forgets_it()
    {
        var repo = new ServerRepository(Paths());
        var server = Server("Good", repo.NewServerDir("Good"));
        repo.Save(server);
        Assert.True(Directory.Exists(server.ServerRoot));

        repo.Delete(server, deleteFiles: true);

        Assert.False(Directory.Exists(server.ServerRoot));
        Assert.Null(repo.Find("Good"));
    }

    [Fact]
    public void Delete_refuses_while_in_use_and_keeps_the_server()
    {
        var repo = new ServerRepository(Paths()) { InUseCheck = _ => true };
        var server = Server("Busy", repo.NewServerDir("Busy"));
        repo.Save(server);

        Assert.Throws<InvalidOperationException>(() => repo.Delete(server, deleteFiles: true));
        Assert.NotNull(repo.Find("Busy"));
        Assert.True(Directory.Exists(server.ServerRoot));
    }

    [Fact]
    public void Delete_surfaces_a_locked_file_and_keeps_the_server_listed()
    {
        var repo = new ServerRepository(Paths());
        var server = Server("Locked", repo.NewServerDir("Locked"));
        repo.Save(server);
        var lockedFile = Path.Combine(server.ServerRoot, "world.db");
        using (new FileStream(lockedFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
            Assert.Throws<IOException>(() => repo.Delete(server, deleteFiles: true));
        }

        Assert.NotNull(repo.Find("Locked"));
    }

    [Theory]
    [InlineData("My Server", true)]
    [InlineData("Bob & Alice's (EU)", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("bad\"name", false)]
    [InlineData("new\nline", false)]
    [InlineData("..", false)]
    public void Server_name_validation(string name, bool ok)
        => Assert.Equal(ok, ServerRepository.ValidateServerName(name) is null);

    // ---- #5 repository owns instances --------------------------------------------------------

    [Fact]
    public void Reload_updates_existing_instances_in_place()
    {
        var paths = Paths();
        var repo = new ServerRepository(paths);
        var server = Server("Inplace", repo.NewServerDir("Inplace"));
        repo.Save(server);
        repo.Reload();
        var owned = repo.Find("Inplace")!;

        // Another writer changes the file on disk.
        var onDisk = MPServerProvider.Core.Json.Json.Deserialize<ServerInstance>(File.ReadAllText(owned.SourceFile!))!;
        onDisk.ServerPort = 9999;
        File.WriteAllText(owned.SourceFile!, MPServerProvider.Core.Json.Json.Serialize(onDisk));

        repo.Reload();

        Assert.Same(owned, repo.Find("Inplace"));
        Assert.Equal(9999, owned.ServerPort);
    }

    [Fact]
    public void Save_of_a_copy_updates_the_owned_instance_and_UpdateStats_persists_only_stats()
    {
        var repo = new ServerRepository(Paths());
        var owned = Server("Owner", repo.NewServerDir("Owner"));
        repo.Save(owned);

        var copy = Server("Owner", owned.ServerRoot);
        copy.ServerPort = 1234;
        repo.Save(copy);

        Assert.Same(owned, repo.Find("Owner"));
        Assert.Equal(1234, owned.ServerPort);

        Assert.True(repo.UpdateStats("Owner", s => s.CrashCount = 7));
        Assert.False(repo.UpdateStats("Nobody", s => s.CrashCount = 1));

        var reread = MPServerProvider.Core.Json.Json.Deserialize<ServerInstance>(File.ReadAllText(owned.SourceFile!))!;
        Assert.Equal(7, reread.Stats.CrashCount);
        Assert.Equal(1234, reread.ServerPort);
        Assert.Empty(Directory.GetFiles(owned.ServerRoot, "*.tmp"));
    }

    [Fact]
    public void Save_never_overwrites_another_servers_record_in_the_same_folder()
    {
        var paths = Paths();
        var repo = new ServerRepository(paths);
        repo.Save(Server("Twin", repo.NewServerDir("Twin")));

        // A fresh repository (not yet reloaded) gets a brand-new instance with the same name.
        var other = new ServerRepository(paths);
        Assert.Throws<InvalidOperationException>(() => other.Save(Server("Twin", string.Empty)));
    }

    // ---- #19 atomic JSON + corrupt settings --------------------------------------------------

    [Fact]
    public void Atomic_write_replaces_contents_and_leaves_no_temp_files()
    {
        var path = Path.Combine(_root, "x", "file.json");
        MPServerProvider.Core.Json.Json.WriteAllTextAtomic(path, "one");
        MPServerProvider.Core.Json.Json.WriteAllTextAtomic(path, "two");
        Assert.Equal("two", File.ReadAllText(path));
        Assert.Single(Directory.GetFiles(Path.GetDirectoryName(path)!));
    }

    [Fact]
    public void Corrupt_settings_are_backed_up_and_defaults_used()
    {
        var paths = Paths();
        File.WriteAllText(paths.SettingsFile, "{ \"start_on_boot\": tr");

        var settings = new SettingsService(paths);

        Assert.NotNull(settings.Settings);
        Assert.NotNull(settings.LoadError);
        Assert.True(File.Exists(paths.SettingsFile + ".corrupt"));
    }

    // ---- #20 install drive change keeps existing servers ------------------------------------

    [Fact]
    public void Changing_servers_dir_keeps_existing_servers_visible()
    {
        var paths = Paths();
        var repo = new ServerRepository(paths);
        repo.Save(Server("OldDrive", repo.NewServerDir("OldDrive")));

        paths.OverrideServersDir(Path.Combine(_root, "newdrive", "servers"));
        repo.Reload();
        Assert.NotNull(repo.Find("OldDrive"));

        // And it is remembered across restarts.
        var reopened = AppPaths.ForRoot(paths.DataRoot, Path.Combine(_root, "newdrive", "servers"));
        var repo2 = new ServerRepository(reopened);
        repo2.Reload();
        Assert.NotNull(repo2.Find("OldDrive"));
    }

    [Fact]
    public void Strict_containment_rules()
    {
        var baseDir = Path.Combine(_root, "servers");
        Assert.True(AppPaths.IsStrictlyInside(Path.Combine(baseDir, "a"), baseDir));
        Assert.False(AppPaths.IsStrictlyInside(baseDir, baseDir));
        Assert.False(AppPaths.IsStrictlyInside(baseDir + "-evil", baseDir));
        Assert.False(AppPaths.IsStrictlyInside(Path.Combine(baseDir, "..", "x"), baseDir));
        Assert.False(AppPaths.IsStrictlyInside(".", baseDir));
        Assert.False(AppPaths.IsStrictlyInside(@"\\host\share\x", baseDir));
    }

    // ---- #2 / #34 mod library --------------------------------------------------------------

    [Theory]
    [InlineData("mods", true)]
    [InlineData("BepInEx/plugins", true)]
    [InlineData("..", false)]
    [InlineData("mods/../../x", false)]
    [InlineData(@"C:\Windows", false)]
    [InlineData(@"\\host\share", false)]
    [InlineData("C:relative", false)]
    public void Mods_folder_must_be_a_plain_relative_path(string folder, bool ok)
        => Assert.Equal(ok, ModLibraryService.IsSafeRelativeFolder(folder));

    [Fact]
    public void Folder_mods_count_as_assigned()
    {
        var paths = Paths();
        var repo = new ServerRepository(paths);
        var server = Server("Modded", repo.NewServerDir("Modded"));
        server.ServerGame = "rust";
        server.ModsFolder = "oxide";
        repo.Save(server);
        Directory.CreateDirectory(Path.Combine(server.ServerRoot, "oxide", "MyFolderMod"));

        var mods = new ModLibraryService(paths, repo, new GameLibraryService(paths));
        Assert.Equal(new[] { "Modded" }, mods.FindAssignments("rust", "MyFolderMod"));

        server.ModsFolder = @"..\..\escape";
        Assert.Null(mods.ServerModsDir(server));
    }

    // ---- #23 firewall -----------------------------------------------------------------------

    [Fact]
    public void Firewall_rule_names_are_validated_and_only_app_rules_deleted()
    {
        Assert.True(FirewallService.IsValidRuleName("Valheim (EU) - 2456"));
        Assert.False(FirewallService.IsValidRuleName("x\" program=\"C:\\evil.exe"));
        Assert.False(FirewallService.IsValidRuleName(new string('a', 65)));

        Assert.Null(FirewallService.BuildDeleteRuleArguments("all"));
        Assert.Null(FirewallService.BuildDeleteRuleArguments(FirewallService.RulePrefix + "x\" dir=out"));
        Assert.Equal("advfirewall firewall delete rule name=\"MPSM - Valheim\"", FirewallService.BuildDeleteRuleArguments("MPSM - Valheim"));

        Assert.Contains("profile=private,domain", FirewallService.BuildAddRuleArguments("MPSM - V", "UDP", 2456));
    }

    [Fact]
    public async Task Firewall_add_rejects_injection_before_running_netsh()
    {
        var firewall = new FirewallService();
        await Assert.ThrowsAsync<ArgumentException>(() => firewall.AddRuleAsync("x\" program=\"evil.exe", 25565, "tcp"));
    }

    // ---- #24 config escaping ----------------------------------------------------------------

    [Fact]
    public void Toml_and_properties_values_are_escaped()
    {
        Assert.Equal("a\\\"b\\nPrivate = false", ConfigText.TomlString("a\"b\nPrivate = false"));
        Assert.Equal("x\\\\y", ConfigText.TomlString("x\\y"));
        Assert.Equal("evil\\nonline-mode\\=false", ConfigText.JavaPropertiesValue("evil\nonline-mode=false"));
        Assert.Equal("caf\\u00E9", ConfigText.JavaPropertiesValue("café"));
        Assert.Equal("a b", ConfigText.SingleLine("a\nb"));
    }

    [Fact]
    public void BeamMp_config_cannot_be_injected_through_the_name()
    {
        var dir = Path.Combine(_root, "beammp");
        Directory.CreateDirectory(dir);
        BeamMpProvider.WriteConfig(new InstallRequest
        {
            Game = new GameDefinition { Id = "beammp" },
            ServerName = "Evil\"\nPrivate = false\n#",
            TargetDir = dir,
            Port = 30814,
        });

        var lines = File.ReadAllLines(Path.Combine(dir, "ServerConfig.toml"));
        Assert.Single(lines, l => l.StartsWith("Private", StringComparison.Ordinal));
        Assert.Contains("Private = true", lines);
    }

    // ---- #10 / #25 downloads -----------------------------------------------------------------

    [Theory]
    [InlineData("https://cdn.example.com/files/server.zip?token=abc", "server.zip")]
    [InlineData("https://evil.tld/a/..%5C..%5C..%5CStartup%5Cx.bat", "x.bat")]
    [InlineData("https://example.com/", "server-download")]
    public void Url_provider_derives_safe_file_names(string url, string expected)
    {
        var name = UrlProvider.FileNameFor(url);
        Assert.Equal(expected, name);
        Assert.Equal(name, Path.GetFileName(name));
    }

    [Fact]
    public void Url_provider_rejects_plain_http()
        => Assert.Throws<InvalidOperationException>(() => UrlProvider.FileNameFor("http://example.com/server.zip"));

    [Theory]
    [InlineData("paper-1.21.jar", "paper-1.21.jar")]
    [InlineData("../../evil.jar", "evil.jar")]
    [InlineData("..\\..\\evil.jar", "evil.jar")]
    [InlineData("..", "fallback")]
    [InlineData("C:evil.jar", "fallback")]
    [InlineData(null, "fallback")]
    public void Safe_file_name_strips_traversal(string? input, string expected)
        => Assert.Equal(expected, DownloadHelper.SafeFileName(input, "fallback"));

    [Fact]
    public async Task Download_refuses_http_and_leaves_no_partial_file()
    {
        var dest = Path.Combine(_root, "dl", "file.bin");
        await Assert.ThrowsAsync<InvalidOperationException>(() => DownloadHelper.DownloadFileAsync("http://example.com/x", dest));
        Assert.False(File.Exists(dest));
        Assert.False(File.Exists(dest + ".part"));
    }

    [Fact]
    public async Task TarGz_is_decompressed_and_single_root_flattened()
    {
        var source = Path.Combine(_root, "tarsrc", "wrapper");
        Directory.CreateDirectory(Path.Combine(source, "bin"));
        File.WriteAllText(Path.Combine(source, "bin", "server.sh"), "echo hi");
        var archive = Path.Combine(_root, "server.tar.gz");
        await using (var file = File.Create(archive))
        await using (var gz = new GZipStream(file, CompressionLevel.Fastest))
        {
            TarFile.CreateFromDirectory(Path.Combine(_root, "tarsrc"), gz, includeBaseDirectory: false);
        }

        var target = Path.Combine(_root, "tartarget");
        Directory.CreateDirectory(target);
        File.Copy(archive, Path.Combine(target, "server.tar.gz"));
        await DownloadHelper.ExtractTarGzAsync(Path.Combine(target, "server.tar.gz"), target);
        DownloadHelper.TryDeleteFile(Path.Combine(target, "server.tar.gz"));
        DownloadHelper.FlattenSingleRootFolder(target);

        Assert.True(File.Exists(Path.Combine(target, "bin", "server.sh")));
        Assert.False(Directory.Exists(Path.Combine(target, "wrapper")));
    }

    [Fact]
    public void Adoptium_checksum_is_read()
    {
        using var doc = JsonDocument.Parse("""{"binary":{"package":{"link":"https://x/y.zip","checksum":"abc123"}}}""");
        var (link, sha) = JavaRuntimeProvider.ParseAdoptiumPackage(doc.RootElement);
        Assert.Equal("https://x/y.zip", link);
        Assert.Equal("abc123", sha);
    }

    // ---- #15 install cleanup / manual provider ----------------------------------------------

    [Fact]
    public async Task Failed_install_removes_the_half_created_folder_and_frees_the_name()
    {
        var paths = Paths();
        var repo = new ServerRepository(paths);
        var installer = new InstallService(paths, repo);
        var game = new GameDefinition
        {
            Id = "custom",
            Name = "Custom",
            Install = new InstallSpec { Provider = "url", Url = "http://insecure.example.com/server.zip", Archive = "zip" },
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => installer.InstallAsync(
            game, null, null, "Broken", 1234, 0, 0, null, false, new Progress<InstallProgress>()));

        Assert.False(Directory.Exists(repo.NewServerDir("Broken")));
        Assert.False(repo.NameTaken("Broken"));
    }

    [Fact]
    public async Task Install_rejects_names_with_quotes()
    {
        var paths = Paths();
        var installer = new InstallService(paths, new ServerRepository(paths));
        await Assert.ThrowsAsync<InvalidOperationException>(() => installer.InstallAsync(
            new GameDefinition { Id = "g" }, null, null, "a\"b", 1, 0, 0, null, false, new Progress<InstallProgress>()));
    }

    [Fact]
    public async Task Manual_install_is_cancellable_and_rejects_copying_into_itself()
    {
        var source = Path.Combine(_root, "manual-src");
        Directory.CreateDirectory(source);
        File.WriteAllText(Path.Combine(source, "server.exe"), "x");
        var provider = new ManualProvider();

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => provider.InstallAsync(
            new InstallRequest { Game = new GameDefinition(), Version = source, TargetDir = Path.Combine(_root, "manual-dst") },
            new Progress<InstallProgress>(), cts.Token));

        await Assert.ThrowsAsync<InvalidOperationException>(() => provider.InstallAsync(
            new InstallRequest { Game = new GameDefinition(), Version = source, TargetDir = Path.Combine(source, "nested") },
            new Progress<InstallProgress>(), CancellationToken.None));

        var result = await provider.InstallAsync(
            new InstallRequest { Game = new GameDefinition(), Version = source, TargetDir = Path.Combine(_root, "manual-ok") },
            new Progress<InstallProgress>(), CancellationToken.None);
        Assert.Equal("server.exe", result.ServerFile);
    }

    // ---- #16 steamcmd -----------------------------------------------------------------------

    [Fact]
    public void SteamCmd_success_detection_and_no_password_on_command_line()
    {
        Assert.True(SteamCmdProvider.InstallSucceeded(7, "...\nSuccess! App '896660' fully installed.\n", 896660));
        Assert.False(SteamCmdProvider.InstallSucceeded(0, "ERROR! Failed to install app '896660' (No subscription)", 896660));
        Assert.False(SteamCmdProvider.InstallSucceeded(8, "Loading Steam API...OK", 896660));
        Assert.True(SteamCmdProvider.InstallSucceeded(0, "Loading Steam API...OK", 896660));

        var spec = new InstallSpec { Username = "user", Password = "not-a-dpapi-blob" };
        Assert.DoesNotContain("not-a-dpapi-blob", SteamCmdProvider.LoginArguments(spec));
        Assert.Throws<InvalidOperationException>(() => SteamCmdProvider.LoginArguments(new InstallSpec { Username = "a\" +quit" }));
    }

    // ---- #22 / #34 minecraft ------------------------------------------------------------------

    [Fact]
    public void Forge_versions_compare_numerically()
    {
        Assert.Equal("47.2.10", MinecraftJavaProvider.PickNewestVersion(new[] { "47.2.9", "47.2.10", "47.1.99" }));
        Assert.True(MinecraftJavaProvider.CompareVersions("1.20.10", "1.20.9") > 0);
        Assert.Equal(0, MinecraftJavaProvider.CompareVersions("1.20", "1.20.0"));
    }

    [Fact]
    public void Forge_launch_detection_for_modern_and_legacy_layouts()
    {
        var modern = Path.Combine(_root, "forge-modern");
        var argsDir = Path.Combine(modern, "libraries", "net", "minecraftforge", "forge", "1.20.1-47.2.10");
        Directory.CreateDirectory(argsDir);
        File.WriteAllText(Path.Combine(argsDir, "win_args.txt"), "-cp x");
        File.WriteAllText(Path.Combine(modern, "user_jvm_args.txt"), "# args");

        var detected = MinecraftJavaProvider.DetectForgeLaunch(modern, "1.20.1", "47.2.10");
        Assert.NotNull(detected);
        Assert.Contains("@user_jvm_args.txt", detected!.Value.StartCommand);
        Assert.Contains("@libraries/net/minecraftforge/forge/1.20.1-47.2.10/win_args.txt", detected.Value.StartCommand);
        Assert.DoesNotContain("-jar", detected.Value.StartCommand);

        var legacy = Path.Combine(_root, "forge-legacy");
        Directory.CreateDirectory(legacy);
        File.WriteAllText(Path.Combine(legacy, "forge-installer.jar"), "x");
        File.WriteAllText(Path.Combine(legacy, "forge-1.12.2-14.23.5.2860.jar"), "x");
        var legacyDetected = MinecraftJavaProvider.DetectForgeLaunch(legacy, "1.12.2", "14.23.5.2860");
        Assert.Equal(("forge-1.12.2-14.23.5.2860.jar", (string?)null), legacyDetected);

        var empty = Path.Combine(_root, "forge-empty");
        Directory.CreateDirectory(empty);
        File.WriteAllText(Path.Combine(empty, "forge-installer.jar"), "x");
        Assert.Null(MinecraftJavaProvider.DetectForgeLaunch(empty, "1.20.1", "47.2.10"));
    }

    // ---- #26 query / #27 rcon / #29 artwork / #31 metrics / #34 vdf --------------------------

    [Theory]
    [InlineData(0, false)]
    [InlineData(-5, false)]
    [InlineData(100, true)]
    [InlineData(1 << 20, true)]
    [InlineData((1 << 20) + 1, false)]
    [InlineData(int.MaxValue, false)]
    public void Status_packet_length_is_bounded(int length, bool ok)
    {
        if (ok)
        {
            QueryService.ValidatePacketLength(length);
        }
        else
        {
            Assert.Throws<FormatException>(() => QueryService.ValidatePacketLength(length));
        }
    }

    [Theory]
    [InlineData("minecraft-java")]
    [InlineData("minecraft-bedrock")]
    public async Task Minecraft_query_kinds_from_game_definitions_are_supported(string kind)
    {
        var ex = await Record.ExceptionAsync(() => QueryService.QueryAsync(kind, "127.0.0.1", 1));
        Assert.IsNotType<NotSupportedException>(ex);
    }

    [Fact]
    public async Task Rcon_auth_times_out_against_a_silent_server()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            var accept = listener.AcceptTcpClientAsync();
            using var rcon = new RconService();
            await Assert.ThrowsAsync<TimeoutException>(() => rcon.ConnectAsync("127.0.0.1", port, "pw", TimeSpan.FromMilliseconds(500)));
            Assert.False(rcon.Connected);
            (await accept).Dispose();
        }
        finally
        {
            listener.Stop();
        }
    }

    [Fact]
    public async Task Rcon_skips_stale_responses_and_matches_request_ids()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            var serverTask = Task.Run(async () =>
            {
                using var client = await listener.AcceptTcpClientAsync();
                var stream = client.GetStream();
                var auth = await RconPacket.ReadAsync(stream);
                await RconPacket.WriteAsync(stream, auth.Id, RconService.ServerDataAuthResponse, string.Empty);
                var cmd = await RconPacket.ReadAsync(stream);
                await RconPacket.WriteAsync(stream, cmd.Id + 1000, RconService.ServerDataResponseValue, "stale");
                await RconPacket.WriteAsync(stream, cmd.Id, RconService.ServerDataResponseValue, "fresh");
                await Task.Delay(200);
            });

            using var rcon = new RconService();
            await rcon.ConnectAsync("127.0.0.1", port, "pw", TimeSpan.FromSeconds(5));
            var reply = await rcon.SendCommandAsync("list", TimeSpan.FromSeconds(5));
            Assert.Equal("fresh", reply);
            await serverTask;
        }
        finally
        {
            listener.Stop();
        }
    }

    [Theory]
    [InlineData("image/png", true)]
    [InlineData("IMAGE/JPEG", true)]
    [InlineData("text/html", false)]
    [InlineData(null, false)]
    public void Artwork_cache_only_accepts_images(string? mediaType, bool ok)
        => Assert.Equal(ok, ArtworkCacheService.IsImageContentType(mediaType));

    [Fact]
    public void Ram_total_is_reported_and_directory_size_counts_files()
    {
        using var metrics = new SystemMetricsService();
        var (_, total, free) = metrics.Ram();
        Assert.True(total > 0);
        Assert.True(free <= total);

        var dir = Path.Combine(_root, "size");
        Directory.CreateDirectory(Path.Combine(dir, "sub"));
        File.WriteAllBytes(Path.Combine(dir, "a.bin"), new byte[10]);
        File.WriteAllBytes(Path.Combine(dir, "sub", "b.bin"), new byte[5]);
        Assert.Equal(15, SystemMetricsService.DirectorySizeBytes(dir));
        Assert.Equal(0, SystemMetricsService.DirectorySizeBytes(Path.Combine(_root, "missing")));
    }

    [Fact]
    public void Vdf_rejects_pathologically_deep_nesting()
    {
        var deep = string.Concat(Enumerable.Repeat("\"k\" {", 10_000));
        Assert.Throws<FormatException>(() => Vdf.Parse(deep));

        var ok = string.Concat(Enumerable.Repeat("\"k\" {", 10)) + new string('}', 10);
        Assert.NotNull(Vdf.Parse(ok));
    }

    // ---- fakes --------------------------------------------------------------------------------

    private sealed class NullPersistence : IServerPersistence
    {
        public void Save(ServerInstance server)
        {
        }

        public bool UpdateStats(string serverName, Action<ServerStats> mutate) => false;
    }

    private sealed class SingleGame(GameDefinition game) : IGameLookup
    {
        public GameDefinition? Find(string gameId) => gameId == game.Id ? game : null;
    }

    private sealed class RecordingRunnerFactory : IServerRunnerFactory
    {
        public string? Executable { get; private set; }
        public string? Arguments { get; private set; }
        public string? WorkingDirectory { get; private set; }

        public IServerRunner Start(string executable, string arguments, string workingDirectory, Action<string> onOutputLine, Action onExited)
        {
            Executable = executable;
            Arguments = arguments;
            WorkingDirectory = workingDirectory;
            return new FakeRunner();
        }
    }

    private sealed class FakeRunner : IServerRunner
    {
        public int ProcessId => 1;
        public bool HasExited => false;
        public int ExitCode => 0;
        public event EventHandler? Exited { add { } remove { } }
        public Task WriteLineAsync(string line, CancellationToken ct = default) => Task.CompletedTask;
        public void Kill()
        {
        }

        public void Dispose()
        {
        }
    }
}
