using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class HytaleAuthAutomationTests
{
    private static ServerInstance NewServer(string name = "Hytale1") => new()
    {
        ServerName = name,
        ServerGame = HytaleAuth.GameId,
        ServerRoot = Path.Combine(Path.GetTempPath(), $"mpsp-hytale-{Guid.NewGuid():N}"),
        ServerFile = "HytaleServer.jar",
    };

    private static GameDefinition HytaleGame() => new()
    {
        Id = HytaleAuth.GameId,
        Runtime = new RuntimeSpec { Executable = "{server_root}\\HytaleServer.jar", WorkingDir = "{server_root}" },
        Network = new NetworkSpec { ConsoleMethod = "stdin" },
    };

    private static (ServerProcessService Service, ScriptedFactory Factory, ServerInstance Server) StartHytaleServer()
    {
        var factory = new ScriptedFactory();
        var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(HytaleGame()), factory);
        var server = NewServer();
        Directory.CreateDirectory(server.ServerRoot);
        service.StartAsync(server).GetAwaiter().GetResult();
        return (service, factory, server);
    }

    [Fact]
    public async Task Missing_tokens_warning_triggers_device_login()
    {
        var (service, factory, server) = StartHytaleServer();
        using var _ = service;
        var automation = new HytaleAuthAutomation(service);

        factory.Runners[0].Emit("[WARN] [HytaleServer] No server tokens configured. Use /auth login to authenticate.");
        await WaitUntil(() => factory.Runners[0].Written.Contains(HytaleAuth.LoginCommand), "login command sent");
    }

    [Fact]
    public async Task Sign_in_link_and_code_are_reported_to_subscribers()
    {
        var (service, factory, server) = StartHytaleServer();
        using var _ = service;
        var automation = new HytaleAuthAutomation(service);

        ServerInstance? reportedServer = null;
        string? reportedLink = null;
        string? reportedCode = null;
        automation.LoginPromptUpdated += (s, link, code) =>
        {
            reportedServer = s;
            reportedLink = link;
            reportedCode = code;
        };

        factory.Runners[0].Emit("Visit: https://accounts.hytale.com/device");
        factory.Runners[0].Emit("Enter code: ABCD-EFGH");

        await WaitUntil(() => reportedCode is not null, "sign-in code reported");
        Assert.Equal(server.ServerName, reportedServer?.ServerName);
        Assert.Equal("https://accounts.hytale.com/device", reportedLink);
        Assert.Equal("ABCD-EFGH", reportedCode);
    }

    [Fact]
    public async Task Successful_login_persists_the_token_so_the_server_is_not_asked_again()
    {
        var (service, factory, server) = StartHytaleServer();
        using var _ = service;
        var automation = new HytaleAuthAutomation(service);

        var succeeded = false;
        automation.LoginSucceeded += _ => succeeded = true;

        factory.Runners[0].Emit("Visit: https://accounts.hytale.com/device");
        factory.Runners[0].Emit("Enter code: ABCD-EFGH");
        factory.Runners[0].Emit("Authentication successful! Mode: OAUTH_DEVICE");

        await WaitUntil(() => succeeded, "login reported as succeeded");
        await WaitUntil(() => factory.Runners[0].Written.Contains(HytaleAuth.PersistCommand), "persistence command sent");
    }

    [Fact]
    public async Task Non_hytale_servers_are_ignored()
    {
        var factory = new ScriptedFactory();
        var minecraftGame = new GameDefinition
        {
            Id = "minecraft-java",
            Runtime = new RuntimeSpec { Executable = "{server_root}\\server.jar", WorkingDir = "{server_root}" },
            Network = new NetworkSpec { ConsoleMethod = "stdin" },
        };
        using var service = new ServerProcessService(new RecordingPersistence(), new SingleGame(minecraftGame), factory);
        var server = new ServerInstance
        {
            ServerName = "Mc1",
            ServerGame = minecraftGame.Id,
            ServerRoot = Path.Combine(Path.GetTempPath(), $"mpsp-mc-{Guid.NewGuid():N}"),
            ServerFile = "server.jar",
        };
        Directory.CreateDirectory(server.ServerRoot);
        await service.StartAsync(server);
        var automation = new HytaleAuthAutomation(service);

        factory.Runners[0].Emit("[WARN] No server tokens configured. Use /auth login to authenticate.");
        await Task.Delay(200);

        Assert.DoesNotContain(HytaleAuth.LoginCommand, factory.Runners[0].Written);
    }

    private static async Task WaitUntil(Func<bool> condition, string what)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (!condition())
        {
            if (sw.Elapsed > TimeSpan.FromSeconds(10))
            {
                throw new TimeoutException($"Timed out waiting for: {what}");
            }

            await Task.Delay(10);
        }
    }

    private sealed class SingleGame(GameDefinition game) : IGameLookup
    {
        public GameDefinition? Find(string gameId) => gameId == game.Id ? game : null;
    }

    private sealed class RecordingPersistence : IServerPersistence
    {
        public void Save(ServerInstance server)
        {
        }

        public bool UpdateStats(string serverName, Action<ServerStats> mutate) => false;
    }

    private sealed class ScriptedFactory : IServerRunnerFactory
    {
        private readonly List<ScriptedRunner> _runners = new();

        public IReadOnlyList<ScriptedRunner> Runners
        {
            get { lock (_runners) { return _runners.ToList(); } }
        }

        public IServerRunner Start(string executable, string arguments, string workingDirectory, Action<string> onOutputLine, Action onExited)
        {
            var runner = new ScriptedRunner(onOutputLine, onExited);
            lock (_runners)
            {
                _runners.Add(runner);
            }

            return runner;
        }
    }

    private sealed class ScriptedRunner(Action<string> onOutputLine, Action onExited) : IServerRunner
    {
        public int ProcessId => 4242;

        public bool HasExited { get; private set; }

        public int ExitCode { get; private set; }

        public event EventHandler? Exited;

        public System.Collections.Concurrent.ConcurrentQueue<string> Written { get; } = new();

        public Task WriteLineAsync(string line, CancellationToken ct = default)
        {
            Written.Enqueue(line);
            return Task.CompletedTask;
        }

        public bool TrySendInterrupt(bool ctrlBreak = false) => true;

        public void Kill() => Exit(-1);

        public void Emit(string line) => onOutputLine(line);

        public void Exit(int code)
        {
            HasExited = true;
            ExitCode = code;
            onExited();
            Exited?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
        }
    }
}
