using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;
using JsonHelper = MPServerProvider.Core.Json.Json;

namespace MPServerProvider.Core.Tests;

/// <summary>Restore-running-servers tracking (#31) and the portable export format (#2).</summary>
public class RestoreAndExportTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"mpsp-restore-{Guid.NewGuid():N}");

    public RestoreAndExportTests() => Directory.CreateDirectory(_root);

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

    // ---- #31 restore servers that were running ---------------------------------------------------

    [Fact]
    public async Task Tracker_remembers_running_servers_across_shutdown_and_restores_them()
    {
        var paths = AppPaths.ForRoot(Path.Combine(_root, "data"));
        var settings = new SettingsService(paths) { ManageStartupEntry = false };
        var repo = new ServerRepository(paths);
        var a = Server(repo, "Alpha");
        var b = Server(repo, "Beta");
        var factory = new CountingFactory();
        using (var processes = new ServerProcessService(new RepoPersistence(repo), new OneGame(), factory))
        {
            var tracker = new RunningServerTracker(settings, processes, repo);
            await processes.StartAsync(a);
            await processes.StartAsync(b);
            await processes.StopAsync(b); // stopped by the user: forgotten
            Assert.Equal(new[] { "Alpha" }, tracker.Remembered);

            // App exit: freeze, then stop everything — Alpha must stay remembered.
            tracker.Freeze();
            await processes.StopAllAsync();
            Assert.Equal(new[] { "Alpha" }, tracker.Remembered);
        }

        // Next launch.
        var settings2 = new SettingsService(paths) { ManageStartupEntry = false };
        Assert.Equal(new[] { "Alpha" }, settings2.Settings.LastRunningServers);
        var repo2 = new ServerRepository(paths);
        repo2.Reload();
        var factory2 = new CountingFactory();
        using var processes2 = new ServerProcessService(new RepoPersistence(repo2), new OneGame(), factory2);
        var tracker2 = new RunningServerTracker(settings2, processes2, repo2);

        var results = await tracker2.RestoreAsync(repo2.Find, processes2);

        Assert.Equal(new[] { ("Alpha", (string?)null) }, results);
        Assert.Equal(ServerState.Running, processes2.StateOf(repo2.Find("Alpha")!));
        Assert.Equal(ServerState.Stopped, processes2.StateOf(repo2.Find("Beta")!));
        await processes2.StopAllAsync();
    }

    [Fact]
    public async Task Tracker_forgets_deleted_servers_and_reports_failed_restores()
    {
        var paths = AppPaths.ForRoot(Path.Combine(_root, "data2"));
        var settings = new SettingsService(paths) { ManageStartupEntry = false };
        settings.Update(s => s.LastRunningServers = new List<string> { "Gone", "Broken" });
        var repo = new ServerRepository(paths);
        var broken = Server(repo, "Broken");
        Directory.Delete(broken.ServerRoot, recursive: true); // working dir missing -> start fails
        using var processes = new ServerProcessService(new RepoPersistence(repo), new OneGame(), new CountingFactory());
        var tracker = new RunningServerTracker(settings, processes, repo);

        var results = await tracker.RestoreAsync(repo.Find, processes);

        var failure = Assert.Single(results);
        Assert.Equal("Broken", failure.Name);
        Assert.NotNull(failure.Error);
        Assert.DoesNotContain("Gone", tracker.Remembered);
        Assert.DoesNotContain("Broken", tracker.Remembered); // failed start reported Stopped
    }

    // ---- #2 portable export ---------------------------------------------------------------------

    [Fact]
    public void Export_contains_no_absolute_paths_and_round_trips()
    {
        var root = Path.Combine(_root, "servers", "Mine");
        var server = new ServerInstance
        {
            ServerName = "Mine",
            ServerGame = "minecraft-java",
            ServerRoot = root,
            SourceFile = Path.Combine(root, "server.json"),
            ServerFile = Path.Combine(root, "paper.jar"),
            ModsFolder = Path.Combine(root, "plugins"),
            ServerPort = 25570,
            JavaMajor = 17,
            StartCommand = "-jar \"{server_file}\" nogui",
            StopCommand = "stop",
            AutoRestartOnCrash = true,
            Stats = new ServerStats { CrashCount = 9 },
        };

        var json = JsonHelper.Serialize(ServerExportData.FromServer(server));

        Assert.DoesNotContain(_root.Replace("\\", "\\\\"), json);
        Assert.DoesNotContain("server_root", json);
        Assert.DoesNotContain("source_file", json);
        Assert.DoesNotContain("crash_count", json);

        var imported = JsonHelper.Deserialize<ServerExportData>(json)!.ToServerInstance();
        Assert.Equal("paper.jar", imported.ServerFile);
        Assert.Equal("plugins", imported.ModsFolder);
        Assert.Equal(25570, imported.ServerPort);
        Assert.Equal(17, imported.JavaMajor);
        Assert.Equal("stop", imported.StopCommand);
        Assert.True(imported.AutoRestartOnCrash);
        Assert.Equal(string.Empty, imported.ServerRoot);
        Assert.Null(imported.SourceFile);
        Assert.Equal(0, imported.Stats.CrashCount);
    }

    [Fact]
    public void Import_accepts_the_old_full_server_json_shape_but_drops_its_paths()
    {
        const string oldShape = """
        {
          "server_game": "valheim",
          "server_name": "Old",
          "server_root": "C:\\Users\\victim\\Documents",
          "server_file": "..\\..\\evil.exe",
          "mods_folder": "\\\\attacker\\share",
          "server_port": 2456,
          "start_command": "-name \"{server_name}\"",
          "stats": { "crash_count": 4 },
          "tunnel": { "mode": "upnp" }
        }
        """;

        var imported = JsonHelper.Deserialize<ServerExportData>(oldShape)!.ToServerInstance();

        Assert.Equal("Old", imported.ServerName);
        Assert.Equal("valheim", imported.ServerGame);
        Assert.Equal(2456, imported.ServerPort);
        Assert.Equal("-name \"{server_name}\"", imported.StartCommand);
        Assert.Equal(string.Empty, imported.ServerRoot);
        Assert.Equal(string.Empty, imported.ServerFile);
        Assert.Null(imported.ModsFolder);
        Assert.Equal(0, imported.Stats.CrashCount);
    }

    // ---- helpers --------------------------------------------------------------------------------

    private static ServerInstance Server(ServerRepository repo, string name)
    {
        var server = new ServerInstance
        {
            ServerName = name,
            ServerGame = "g",
            ServerRoot = repo.NewServerDir(name),
            StopCommand = "stop",
        };
        repo.Save(server);
        return server;
    }

    private sealed class OneGame : IGameLookup
    {
        public GameDefinition? Find(string gameId) => new()
        {
            Id = gameId,
            Runtime = new RuntimeSpec { Executable = "{server_root}\\server.exe", WorkingDir = "{server_root}" },
        };
    }

    private sealed class RepoPersistence(ServerRepository repo) : IServerPersistence
    {
        public void Save(ServerInstance server) => repo.Save(server);

        public bool UpdateStats(string serverName, Action<ServerStats> mutate) => repo.UpdateStats(serverName, mutate);
    }

    private sealed class CountingFactory : IServerRunnerFactory
    {
        public IServerRunner Start(string executable, string arguments, string workingDirectory, Action<string> onOutputLine, Action onExited)
            => new StopOnCommandRunner(onExited);
    }

    private sealed class StopOnCommandRunner(Action onExited) : IServerRunner
    {
        public int ProcessId => 1;

        public bool HasExited { get; private set; }

        public int ExitCode => 0;

        public event EventHandler? Exited { add { } remove { } }

        public Task WriteLineAsync(string line, CancellationToken ct = default)
        {
            if (line == "stop" && !HasExited)
            {
                HasExited = true;
                _ = Task.Run(onExited);
            }

            return Task.CompletedTask;
        }

        public void Kill()
        {
            HasExited = true;
            _ = Task.Run(onExited);
        }

        public void Dispose()
        {
        }
    }
}
