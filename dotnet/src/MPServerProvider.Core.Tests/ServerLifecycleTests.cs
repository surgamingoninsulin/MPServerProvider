using System.Collections.Concurrent;
using System.Diagnostics;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Native;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class ServerLifecycleTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"mpsp-lifecycle-{Guid.NewGuid():N}");

    public ServerLifecycleTests() => Directory.CreateDirectory(_root);

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

    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(10);

    private ServerInstance NewServer(string name = "Srv", string? stopCommand = "stop", bool autoRestart = false)
    {
        var root = Path.Combine(_root, name);
        Directory.CreateDirectory(root);
        return new ServerInstance
        {
            ServerName = name,
            ServerGame = "test-game",
            ServerRoot = root,
            ServerFile = "server.exe",
            StopCommand = stopCommand ?? string.Empty,
            AutoRestartOnCrash = autoRestart,
        };
    }

    private static GameDefinition NewGame(string consoleMethod = "stdin", string executable = "{server_root}\\server.exe") => new()
    {
        Id = "test-game",
        Runtime = new RuntimeSpec { Executable = executable, Arguments = "-port {server_port}", WorkingDir = "{server_root}" },
        Network = new NetworkSpec { ConsoleMethod = consoleMethod },
    };

    private static async Task WaitUntil(Func<bool> condition, string what)
    {
        var sw = Stopwatch.StartNew();
        while (!condition())
        {
            if (sw.Elapsed > Patience)
            {
                throw new TimeoutException($"Timed out waiting for: {what}");
            }

            await Task.Delay(10);
        }
    }

    // ---- #11 exit race / single disposal ---------------------------------------------------------

    [Fact]
    public async Task A_process_that_exits_during_start_is_reported_as_crashed_not_left_running()
    {
        var factory = new ScriptedFactory { ExitDuringStart = true };
        var persistence = new RecordingPersistence();
        using var service = new ServerProcessService(persistence, new SingleGame(NewGame()), factory);
        var states = new ConcurrentQueue<ServerState>();
        service.StateChanged += (_, s) => states.Enqueue(s);
        var server = NewServer();

        await service.StartAsync(server);

        await WaitUntil(() => service.StateOf(server) == ServerState.Crashed, "crashed state");
        Assert.Equal(new[] { ServerState.Starting, ServerState.Running, ServerState.Crashed }, states.ToArray());
        Assert.Equal(1, factory.Runners[0].Disposals);
        Assert.Equal(1, persistence.Stats.CrashCount);
    }

    [Fact]
    public async Task Stop_sends_the_stop_command_awaits_the_exit_and_forgets_the_server()
    {
        var factory = new ScriptedFactory { Configure = r => r.ExitOnLine = "stop" };
        var persistence = new RecordingPersistence();
        using var service = new ServerProcessService(persistence, new SingleGame(NewGame()), factory);
        var server = NewServer();
        await service.StartAsync(server);
        Assert.Equal(ServerState.Running, service.StateOf(server));

        await service.StopAsync(server);

        var runner = factory.Runners.Single();
        Assert.Contains("stop", runner.Written);
        Assert.Equal(0, runner.Kills);
        Assert.Equal(0, runner.Interrupts);
        Assert.Equal(1, runner.Disposals);
        Assert.Equal(ServerState.Stopped, service.StateOf(server));
        Assert.Equal(0, service.RunningCount);
        Assert.Empty(service.ActiveServers);
        Assert.Null(service.ProcessOf(server));
        Assert.Equal(1, persistence.StatsUpdates); // uptime recorded once
        Assert.Equal(0, persistence.Stats.CrashCount);
        Assert.Equal(0, persistence.Saves); // never re-saves a captured instance (#5)
        Assert.Contains(service.ConsoleOf(server), l => l.Contains("Server stopped"));
    }

    [Fact]
    public async Task A_duplicate_exit_notification_is_processed_once()
    {
        var factory = new ScriptedFactory();
        var persistence = new RecordingPersistence();
        using var service = new ServerProcessService(persistence, new SingleGame(NewGame()), factory);
        var server = NewServer();
        await service.StartAsync(server);
        var runner = factory.Runners.Single();

        runner.Exit(3);
        runner.Exit(3);
        await WaitUntil(() => service.StateOf(server) == ServerState.Crashed, "crashed");
        await Task.Delay(100);

        Assert.Equal(1, runner.Disposals);
        Assert.Equal(1, persistence.StatsUpdates);
        Assert.Equal(1, persistence.Stats.CrashCount);
    }

    // ---- #14 graceful stop without a stop command, then kill ----------------------------------

    [Fact]
    public async Task Stop_without_a_stop_command_sends_ctrl_c_instead_of_killing()
    {
        var factory = new ScriptedFactory { Configure = r => r.ExitOnInterrupt = true };
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(NewGame()), factory);
        var server = NewServer(stopCommand: null);
        await service.StartAsync(server);

        await service.StopAsync(server, TimeSpan.FromSeconds(5));

        var runner = factory.Runners.Single();
        Assert.Equal(1, runner.Interrupts);
        Assert.False(runner.LastInterruptWasBreak);
        Assert.Equal(0, runner.Kills);
        Assert.Equal(ServerState.Stopped, service.StateOf(server));
    }

    [Fact]
    public async Task Stop_kills_the_process_when_it_ignores_every_graceful_signal()
    {
        var factory = new ScriptedFactory();
        var persistence = new RecordingPersistence();
        using var service = new ServerProcessService(persistence, new SingleGame(NewGame()), factory);
        var server = NewServer(stopCommand: "quit");
        await service.StartAsync(server);

        await service.StopAsync(server, TimeSpan.FromMilliseconds(100));

        var runner = factory.Runners.Single();
        Assert.Contains("quit", runner.Written);
        Assert.Equal(1, runner.Interrupts); // Ctrl+C as the second, harder signal
        Assert.Equal(1, runner.Kills);
        Assert.Equal(1, runner.Disposals);
        Assert.Equal(ServerState.Stopped, service.StateOf(server));
        Assert.Equal(0, persistence.Stats.CrashCount); // a requested stop is not a crash
    }

    [Fact]
    public async Task StopAll_stops_servers_in_parallel()
    {
        // Each runner only exits once BOTH have received their stop command: a sequential
        // StopAll would deadlock into the kill path.
        var received = new CountdownEvent(2);
        var factory = new ScriptedFactory
        {
            Configure = r => r.OnLine = line =>
            {
                if (line == "stop")
                {
                    received.Signal();
                    _ = Task.Run(() =>
                    {
                        received.Wait(Patience);
                        r.Exit(0);
                    });
                }
            },
        };
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(NewGame()), factory);
        var a = NewServer("A");
        var b = NewServer("B");
        await service.StartAsync(a);
        await service.StartAsync(b);

        await service.StopAllAsync(TimeSpan.FromSeconds(5));

        Assert.All(factory.Runners, r => Assert.Equal(0, r.Kills));
        Assert.Equal(ServerState.Stopped, service.StateOf(a));
        Assert.Equal(ServerState.Stopped, service.StateOf(b));
    }

    // ---- #3 crash auto-restart ------------------------------------------------------------------

    [Fact]
    public async Task Crash_reports_crashed_before_restarting_and_stops_after_max_consecutive_crashes()
    {
        var factory = new ScriptedFactory();
        var persistence = new RecordingPersistence();
        using var service = new ServerProcessService(persistence, new SingleGame(NewGame()), factory)
        {
            AutoRestartDelay = TimeSpan.FromMilliseconds(20),
        };
        var states = new ConcurrentQueue<ServerState>();
        service.StateChanged += (_, s) => states.Enqueue(s);
        var server = NewServer(autoRestart: true);
        await service.StartAsync(server);

        for (var crash = 1; crash <= ServerProcessService.MaxConsecutiveCrashes + 1; crash++)
        {
            var expectedRunners = crash;
            await WaitUntil(() => factory.Runners.Count == expectedRunners && service.StateOf(server) == ServerState.Running, $"run #{crash}");
            factory.Runners[^1].Exit(1);
        }

        await WaitUntil(() => service.StateOf(server) == ServerState.Crashed && !service.IsAutoRestartPending(server), "final crash");
        await Task.Delay(100);

        // 1 start + MaxConsecutiveCrashes restarts, then suspended.
        Assert.Equal(1 + ServerProcessService.MaxConsecutiveCrashes, factory.Runners.Count);
        Assert.Equal(ServerProcessService.MaxConsecutiveCrashes + 1, persistence.Stats.CrashCount);
        Assert.Contains(service.ConsoleOf(server), l => l.Contains("auto-restart suspended"));

        // Every Crashed precedes the next Starting/Running; none follows a successful restart.
        var sequence = states.ToArray();
        var expected = new List<ServerState> { ServerState.Starting, ServerState.Running };
        for (var i = 0; i < ServerProcessService.MaxConsecutiveCrashes; i++)
        {
            expected.AddRange(new[] { ServerState.Crashed, ServerState.Starting, ServerState.Running });
        }

        expected.Add(ServerState.Crashed);
        Assert.Equal(expected, sequence);
    }

    [Fact]
    public async Task Crash_counter_resets_after_a_stable_run()
    {
        var factory = new ScriptedFactory();
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(NewGame()), factory)
        {
            AutoRestartDelay = TimeSpan.FromMilliseconds(10),
            CrashCounterResetUptime = TimeSpan.Zero, // every run counts as stable
        };
        var server = NewServer(autoRestart: true);
        await service.StartAsync(server);

        const int crashes = ServerProcessService.MaxConsecutiveCrashes + 3;
        for (var crash = 1; crash <= crashes; crash++)
        {
            var expectedRunners = crash;
            await WaitUntil(() => factory.Runners.Count == expectedRunners && service.StateOf(server) == ServerState.Running, $"run #{crash}");
            factory.Runners[^1].Exit(1);
        }

        await WaitUntil(() => factory.Runners.Count == crashes + 1 && service.StateOf(server) == ServerState.Running, "still restarting");
        await service.StopAsync(server);
    }

    [Fact]
    public async Task Stop_during_the_auto_restart_delay_cancels_the_restart()
    {
        var factory = new ScriptedFactory();
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(NewGame()), factory)
        {
            AutoRestartDelay = TimeSpan.FromSeconds(30),
        };
        var server = NewServer(autoRestart: true);
        await service.StartAsync(server);
        factory.Runners[0].Exit(1);
        await WaitUntil(() => service.IsAutoRestartPending(server), "restart pending");
        Assert.Equal(ServerState.Crashed, service.StateOf(server));
        Assert.Single(service.ActiveServers);

        await service.StopAsync(server);

        Assert.Equal(ServerState.Stopped, service.StateOf(server));
        Assert.False(service.IsAutoRestartPending(server));
        await Task.Delay(100);
        Assert.Single(factory.Runners);
    }

    // ---- Java provisioning ---------------------------------------------------------------------

    [Fact]
    public async Task First_start_java_download_reports_progress_prefers_server_java_major_and_is_cancellable_by_stop()
    {
        var requestedMajor = 0;
        var progressSeen = new TaskCompletionSource();
        Task<string> Resolver(int major, IProgress<InstallProgress> progress, CancellationToken ct)
        {
            requestedMajor = major;
            progress.Report(InstallProgress.Indeterminate("Downloading Temurin JRE 17"));
            return Task.Delay(Timeout.Infinite, ct).ContinueWith<string>(_ => throw new OperationCanceledException(ct), TaskScheduler.Default);
        }

        var game = NewGame(executable: "{java}");
        game.Runtime.Requires = new RequiresSpec { Java = 21 };
        var factory = new ScriptedFactory();
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(game), factory, Resolver);
        var server = NewServer();
        server.JavaMajor = 17;
        var reports = new ConcurrentQueue<string>();
        var progress = new SyncProgress(p =>
        {
            reports.Enqueue(p.Status);
            progressSeen.TrySetResult();
        });

        var start = service.StartAsync(server, progress);
        await progressSeen.Task.WaitAsync(Patience);
        Assert.Equal(ServerState.Starting, service.StateOf(server));

        await service.StopAsync(server);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => start);
        Assert.Equal(17, requestedMajor);
        Assert.Contains("Downloading Temurin JRE 17", reports);
        Assert.Equal(ServerState.Stopped, service.StateOf(server));
        Assert.Empty(factory.Runners);
        Assert.Contains(service.ConsoleOf(server), l => l.Contains("Downloading Temurin JRE 17"));
    }

    // ---- #12 console buffer / subscriber isolation ----------------------------------------------

    [Fact]
    public async Task Console_buffer_survives_concurrent_writers_and_readers()
    {
        var running = new RunningServer(new ServerInstance());
        var writers = Enumerable.Range(0, 4).Select(w => Task.Run(() =>
        {
            for (var i = 0; i < 5000; i++)
            {
                running.Append($"w{w}-{i}");
            }
        })).ToArray();
        var reader = Task.Run(async () =>
        {
            while (!writers.All(t => t.IsCompleted))
            {
                Assert.True(running.ConsoleSnapshot().Count <= 2000);
                await Task.Yield();
            }
        });

        await Task.WhenAll(writers.Append(reader));

        var snapshot = running.ConsoleSnapshot();
        Assert.Equal(2000, snapshot.Count);
        Assert.All(snapshot, Assert.NotNull);
        var single = new RunningServer(new ServerInstance());
        for (var i = 0; i < 2500; i++)
        {
            single.Append(i.ToString());
        }

        Assert.Equal(Enumerable.Range(500, 2000).Select(i => i.ToString()), single.ConsoleSnapshot());
    }

    [Fact]
    public async Task Throwing_subscribers_never_break_start_output_or_exit_handling()
    {
        var factory = new ScriptedFactory();
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(NewGame()), factory);
        service.StateChanged += (_, _) => throw new InvalidOperationException("bad state subscriber");
        service.OutputReceived += (_, _) => throw new InvalidOperationException("bad output subscriber");
        var server = NewServer();

        await service.StartAsync(server);
        factory.Runners[0].Emit("hello from the server");
        factory.Runners[0].Exit(0);

        await WaitUntil(() => service.StateOf(server) == ServerState.Crashed, "crashed");
        Assert.Contains("hello from the server", service.ConsoleOf(server));
    }

    // ---- #13 log tail ---------------------------------------------------------------------------

    [Theory]
    [InlineData("stdin", false)]
    [InlineData("rcon", false)]
    [InlineData("logfile", true)]
    [InlineData("LOGFILE", true)]
    public void Only_logfile_console_games_tail_a_log(string method, bool tails)
        => Assert.Equal(tails, ServerProcessService.UsesLogFileConsole(NewGame(method)));

    [Fact]
    public async Task Stdin_games_never_tail_the_log_so_lines_are_not_duplicated()
    {
        var factory = new ScriptedFactory();
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(NewGame("stdin")), factory);
        var server = NewServer();
        Directory.CreateDirectory(Path.Combine(server.ServerRoot, "logs"));
        File.WriteAllText(Path.Combine(server.ServerRoot, "logs", "latest.log"), "old\n");

        await service.StartAsync(server);
        await Task.Delay(200);

        Assert.DoesNotContain(service.ConsoleOf(server), l => l.Contains("Tailing log file"));
        factory.Runners[0].Exit(0);
    }

    [Fact]
    public async Task Logfile_games_tail_new_lines_and_release_the_file_when_the_server_stops()
    {
        var factory = new ScriptedFactory { Configure = r => r.ExitOnLine = "stop" };
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(NewGame("logfile")), factory);
        var server = NewServer();
        var log = Path.Combine(server.ServerRoot, "server.log");
        File.WriteAllText(log, "from the previous run\n");

        await service.StartAsync(server);
        await WaitUntil(() => service.ConsoleOf(server).Any(l => l.Contains("Tailing log file")), "tail attached");
        using (var writer = new StreamWriter(new FileStream(log, FileMode.Append, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete)))
        {
            writer.WriteLine("fresh line");
        }

        await WaitUntil(() => service.ConsoleOf(server).Contains("fresh line"), "tailed line");
        Assert.DoesNotContain("from the previous run", service.ConsoleOf(server));

        // Even while tailing, the log can be deleted (FileShare.Delete) — e.g. rotation.
        File.Delete(log);
        await service.StopAsync(server);
        Assert.False(File.Exists(log));
    }

    // ---- #9 elevation policy -------------------------------------------------------------------

    [Theory]
    [InlineData("minecraft-java")]
    [InlineData("valheim")]
    public void When_elevated_every_game_is_launched_de_elevated(string gameId)
    {
        var normal = new ScriptedFactory();
        var deElevated = new ScriptedFactory();
        var factory = new DefaultServerRunnerFactory(() => true, normal, () => deElevated);

        factory.Start($"{gameId}.exe", string.Empty, _root, _ => { }, () => { });

        Assert.Single(deElevated.Runners);
        Assert.Empty(normal.Runners);
    }

    [Fact]
    public void When_de_elevation_fails_the_start_is_aborted_instead_of_running_as_admin()
    {
        var normal = new ScriptedFactory();
        var factory = new DefaultServerRunnerFactory(() => true, normal, () => throw new System.ComponentModel.Win32Exception(1314));

        var ex = Assert.Throws<InvalidOperationException>(() => factory.Start("server.exe", string.Empty, _root, _ => { }, () => { }));

        Assert.Contains("administrator", ex.Message);
        Assert.Empty(normal.Runners);
    }

    [Fact]
    public void When_not_elevated_the_normal_launcher_is_used()
    {
        var normal = new ScriptedFactory();
        var factory = new DefaultServerRunnerFactory(() => false, normal, () => throw new InvalidOperationException("must not be used"));

        factory.Start("server.exe", string.Empty, _root, _ => { }, () => { });

        Assert.Single(normal.Runners);
    }

    // ---- real processes -------------------------------------------------------------------------

    private static string Cmd => Path.Combine(Environment.SystemDirectory, "cmd.exe");

    private static GameDefinition CmdGame(string arguments) => new()
    {
        Id = "test-game",
        Runtime = new RuntimeSpec { Executable = Cmd, Arguments = arguments, WorkingDir = "{server_root}" },
        Network = new NetworkSpec { ConsoleMethod = "stdin" },
    };

    [Fact]
    public async Task Real_process_console_commands_and_graceful_stop_work()
    {
        var game = CmdGame("/q /k echo ready");
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(game), new ProcessServerRunnerFactory());
        var server = NewServer(stopCommand: "exit");

        await service.StartAsync(server);
        await WaitUntil(() => service.ConsoleOf(server).Contains("ready"), "ready banner");
        var pid = service.ProcessOf(server)!.ProcessId;

        service.SendCommand(server, "echo pong-%COMPUTERNAME%");
        await WaitUntil(() => service.ConsoleOf(server).Any(l => l.EndsWith(">pong-" + Environment.MachineName, StringComparison.OrdinalIgnoreCase)), "command output");

        await service.StopAsync(server, TimeSpan.FromSeconds(10));

        Assert.Equal(ServerState.Stopped, service.StateOf(server));
        Assert.DoesNotContain(service.ConsoleOf(server), l => l.Contains("forcing it to close"));
        Assert.Throws<ArgumentException>(() => Process.GetProcessById(pid));
    }

    [Fact]
    public async Task Real_process_tree_that_ignores_stop_is_killed()
    {
        // cmd waits on a child (ping); stdin "exit" is never read while ping runs.
        var game = CmdGame("/c ping -n 120 127.0.0.1 >nul");
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(game), new ProcessServerRunnerFactory());
        var server = NewServer(stopCommand: "exit");

        await service.StartAsync(server);
        var pid = service.ProcessOf(server)!.ProcessId;
        var sw = Stopwatch.StartNew();

        await service.StopAsync(server, TimeSpan.FromMilliseconds(300));

        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(15), $"stop took {sw.Elapsed}");
        Assert.Equal(ServerState.Stopped, service.StateOf(server));
        Assert.Throws<ArgumentException>(() => Process.GetProcessById(pid));
    }

    [Fact]
    public async Task Real_process_that_exits_on_its_own_is_detected()
    {
        var game = CmdGame("/c echo bye & exit 7");
        var persistence = new RecordingPersistence();
        using var service = new ServerProcessService(persistence, new SingleGame(game), new ProcessServerRunnerFactory());
        var server = NewServer();

        await service.StartAsync(server);

        await WaitUntil(() => service.StateOf(server) == ServerState.Crashed, "exit detected");
        Assert.Contains(service.ConsoleOf(server), l => l.Contains("code 7"));
        Assert.Equal(1, persistence.Stats.CrashCount);
    }

    [Fact]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public async Task Redirected_token_launch_runner_tracks_console_stdin_exit_and_kill()
    {
        // Exercises DeElevatedServerRunner's pipe/job/wait-handle plumbing with this process's own
        // token (the desktop-user token is only meaningful when the test host is elevated).
        var factory = new DeElevatedServerRunnerFactory(DeElevatedLauncher.TokenSource.CurrentProcess);
        var game = CmdGame("/q /k echo ready");
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(game), factory);
        var server = NewServer(stopCommand: "exit");

        await service.StartAsync(server);
        await WaitUntil(() => service.ConsoleOf(server).Contains("ready"), "ready banner via stdout pipe");
        var runner = service.ProcessOf(server)!;
        Assert.IsType<DeElevatedServerRunner>(runner);
        Assert.False(runner.IsDeElevated);
        using (var live = Process.GetProcessById(runner.ProcessId))
        {
            Assert.True(live.WorkingSet64 > 0); // metrics can sample it
        }

        service.SendCommand(server, "echo piped-stdin");
        await WaitUntil(() => service.ConsoleOf(server).Any(l => l.EndsWith(">piped-stdin", StringComparison.Ordinal)), "stdin pipe round trip");

        await service.StopAsync(server, TimeSpan.FromSeconds(10));
        Assert.Equal(ServerState.Stopped, service.StateOf(server));
        Assert.Contains(service.ConsoleOf(server), l => l.Contains("Server stopped (exit code 0)"));

        // Kill path: a tree that ignores "exit".
        var stubborn = new ServerProcessService(new RecordingPersistence(), new SingleGame(CmdGame("/c ping -n 120 127.0.0.1 >nul")), factory);
        using (stubborn)
        {
            var other = NewServer("Stubborn", stopCommand: "exit");
            await stubborn.StartAsync(other);
            var pid = stubborn.ProcessOf(other)!.ProcessId;
            await stubborn.StopAsync(other, TimeSpan.FromMilliseconds(300));
            Assert.Throws<ArgumentException>(() => Process.GetProcessById(pid));
        }
    }

    // ---- fakes ----------------------------------------------------------------------------------

    private sealed class SingleGame(GameDefinition game) : IGameLookup
    {
        public GameDefinition? Find(string gameId) => gameId == game.Id ? game : null;
    }

    private sealed class SyncProgress(Action<InstallProgress> report) : IProgress<InstallProgress>
    {
        public void Report(InstallProgress value) => report(value);
    }

    private sealed class RecordingPersistence : IServerPersistence
    {
        private readonly object _lock = new();

        public ServerStats Stats { get; } = new();

        public int Saves { get; private set; }

        public int StatsUpdates { get; private set; }

        public void Save(ServerInstance server)
        {
            lock (_lock)
            {
                Saves++;
            }
        }

        public bool UpdateStats(string serverName, Action<ServerStats> mutate)
        {
            lock (_lock)
            {
                StatsUpdates++;
                mutate(Stats);
            }

            return true;
        }
    }

    private sealed class ScriptedFactory : IServerRunnerFactory
    {
        private readonly List<ScriptedRunner> _runners = new();

        public Action<ScriptedRunner>? Configure { get; init; }

        public bool ExitDuringStart { get; init; }

        public IReadOnlyList<ScriptedRunner> Runners
        {
            get { lock (_runners) { return _runners.ToList(); } }
        }

        public IServerRunner Start(string executable, string arguments, string workingDirectory, Action<string> onOutputLine, Action onExited)
        {
            var runner = new ScriptedRunner(onOutputLine, onExited);
            Configure?.Invoke(runner);
            lock (_runners)
            {
                _runners.Add(runner);
            }

            if (ExitDuringStart)
            {
                runner.Exit(1); // before Start even returns
            }

            return runner;
        }
    }

    private sealed class ScriptedRunner(Action<string> onOutputLine, Action onExited) : IServerRunner
    {
        private int _disposals;
        private int _kills;
        private int _interrupts;

        public int ProcessId => 4242;

        public bool HasExited { get; private set; }

        public int ExitCode { get; private set; }

        public event EventHandler? Exited;

        public string? ExitOnLine { get; set; }

        public bool ExitOnInterrupt { get; set; }

        public Action<string>? OnLine { get; set; }

        public ConcurrentQueue<string> Written { get; } = new();

        public int Disposals => _disposals;

        public int Kills => _kills;

        public int Interrupts => _interrupts;

        public bool LastInterruptWasBreak { get; private set; }

        public Task WriteLineAsync(string line, CancellationToken ct = default)
        {
            Written.Enqueue(line);
            OnLine?.Invoke(line);
            if (line == ExitOnLine)
            {
                _ = Task.Run(() => Exit(0));
            }

            return Task.CompletedTask;
        }

        public bool TrySendInterrupt(bool ctrlBreak = false)
        {
            Interlocked.Increment(ref _interrupts);
            LastInterruptWasBreak = ctrlBreak;
            if (ExitOnInterrupt)
            {
                _ = Task.Run(() => Exit(0));
            }

            return true;
        }

        public void Kill()
        {
            Interlocked.Increment(ref _kills);
            _ = Task.Run(() => Exit(-1));
        }

        public void Emit(string line) => onOutputLine(line);

        public void Exit(int code)
        {
            HasExited = true;
            ExitCode = code;
            onExited();
            Exited?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() => Interlocked.Increment(ref _disposals);
    }
}
