using System.Collections.Concurrent;
using System.Diagnostics;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Native;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Abstraction over "a launched server process" used by <see cref="ServerProcessService"/>.
/// Not evidenced in the decompile by this exact shape — introduced purely so the service's
/// start/stop/send-command/exit-handling logic can be unit-tested without spawning real OS
/// processes, mirroring how the Rust port's <c>ToolDownloader</c>/<c>ToolPaths</c> traits kept
/// its <c>tunnel</c> module testable against small fakes. <see cref="ProcessServerRunner"/> (normal
/// launch) and <see cref="DeElevatedServerRunner"/> (launch as the unelevated desktop user while
/// the app is elevated) are the real implementations; the service treats both identically.
/// </summary>
public interface IServerRunner : IDisposable
{
    int ProcessId { get; }

    bool HasExited { get; }

    int ExitCode { get; }

    /// <summary><c>true</c> when the process runs as the unelevated desktop user although this
    /// app is elevated.</summary>
    bool IsDeElevated => false;

    /// <summary>Raised once when the underlying process exits, on or off the main thread.</summary>
    event EventHandler? Exited;

    Task WriteLineAsync(string line, CancellationToken ct = default);

    /// <summary>
    /// Sends Ctrl+C (or Ctrl+Break) to the process's console — the graceful stop for servers
    /// without a stdin stop command. Returns <c>false</c> when it couldn't be sent.
    /// </summary>
    bool TrySendInterrupt(bool ctrlBreak = false) => false;

    /// <summary>Terminates the process and everything it started. Never throws.</summary>
    void Kill();
}

/// <summary>Creates <see cref="IServerRunner"/>s. See <see cref="IServerRunner"/>'s doc comment.</summary>
public interface IServerRunnerFactory
{
    /// <summary>
    /// Starts <paramref name="executable"/> with <paramref name="arguments"/> in
    /// <paramref name="workingDirectory"/>, forwarding stdout/stderr lines through
    /// <paramref name="onOutputLine"/> as they arrive. <paramref name="onExited"/> is subscribed
    /// BEFORE the process starts, so even a process that exits within milliseconds is reported
    /// (exactly once). Throws if the process could not be started.
    /// </summary>
    IServerRunner Start(string executable, string arguments, string workingDirectory, Action<string> onOutputLine, Action onExited);
}

/// <summary>
/// Default, real <see cref="IServerRunner"/>: a plain <see cref="Process"/> with redirected
/// stdin/stdout/stderr, matching the original's direct <c>ProcessStartInfo</c>-based spawn for
/// the case where the app isn't elevated (so there is nothing to drop). The process is put into a
/// <see cref="KillOnCloseJob"/> so it can't outlive the app.
/// </summary>
public sealed class ProcessServerRunner : IServerRunner
{
    private readonly Process _process;
    private KillOnCloseJob? _job;
    private int _exitRaised;
    private int _disposed;
    private volatile bool _killed;

    internal ProcessServerRunner(Process process)
    {
        _process = process;
        _process.Exited += OnProcessExited;
    }

    public int ProcessId => _process.Id;

    public bool HasExited => _process.HasExited;

    public int ExitCode => _process.ExitCode;

    public event EventHandler? Exited;

    public async Task WriteLineAsync(string line, CancellationToken ct = default)
    {
        await _process.StandardInput.WriteLineAsync(line).WaitAsync(ct).ConfigureAwait(false);
        await _process.StandardInput.FlushAsync(ct).ConfigureAwait(false);
    }

    public bool TrySendInterrupt(bool ctrlBreak = false)
        => OperatingSystem.IsWindows() && !_process.HasExited && ConsoleCtrl.TrySend(_process.Id, ctrlBreak);

    public void Kill()
    {
        _killed = true;
        if (OperatingSystem.IsWindows())
        {
            try
            {
                _job?.Terminate();
            }
            catch
            {
                // Fall back to the tree kill below.
            }
        }

        try
        {
            if (!_process.HasExited)
            {
                _process.Kill(entireProcessTree: true);
            }
        }
        catch
        {
            // Best-effort: already exited, or access denied for a child that is going anyway.
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        if (OperatingSystem.IsWindows() && _job is { } job)
        {
            bool exitedOnItsOwn;
            try
            {
                exitedOnItsOwn = _process.HasExited && !_killed;
            }
            catch
            {
                exitedOnItsOwn = false;
            }

            if (exitedOnItsOwn)
            {
                job.Release();
            }
            else
            {
                job.Dispose();
            }
        }

        _process.Dispose();
    }

    /// <summary>Puts the (started) process into a kill-on-close job. Best-effort.</summary>
    internal void AttachJob()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var job = KillOnCloseJob.TryCreate();
        if (job is not null && !job.TryAssign(_process.SafeHandle))
        {
            job.Dispose();
            job = null;
        }

        _job = job;
    }

    private void OnProcessExited(object? sender, EventArgs e)
    {
        if (Interlocked.Exchange(ref _exitRaised, 1) != 0)
        {
            return;
        }

        try
        {
            Exited?.Invoke(this, EventArgs.Empty);
        }
        catch
        {
            // Never let a subscriber's exception escape onto a thread-pool thread.
        }
    }
}

/// <summary>Production <see cref="IServerRunnerFactory"/> backed by <see cref="Process"/>.</summary>
public sealed class ProcessServerRunnerFactory : IServerRunnerFactory
{
    public IServerRunner Start(string executable, string arguments, string workingDirectory, Action<string> onOutputLine, Action onExited)
    {
        var psi = new ProcessStartInfo(executable)
        {
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
        process.OutputDataReceived += (_, e) => Forward(onOutputLine, e.Data);
        process.ErrorDataReceived += (_, e) => Forward(onOutputLine, e.Data);

        // Subscribe before Start(): a process that dies instantly must still be reported.
        var runner = new ProcessServerRunner(process);
        runner.Exited += (_, _) => onExited();

        try
        {
            // Never start while ConsoleCtrl has "ignore Ctrl+C" set: the child would inherit it.
            lock (OperatingSystem.IsWindows() ? ConsoleCtrl.LaunchGate : psi)
            {
                if (!process.Start())
                {
                    throw new InvalidOperationException($"Could not start '{executable}'.");
                }
            }
        }
        catch
        {
            process.Dispose();
            throw;
        }

        runner.AttachJob();
        try
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
        catch
        {
            runner.Kill();
            runner.Dispose();
            throw;
        }

        return runner;
    }

    private static void Forward(Action<string> onOutputLine, string? line)
    {
        if (line is null)
        {
            return;
        }

        try
        {
            onOutputLine(line);
        }
        catch
        {
            // Output consumers must never throw out of the process's event thread.
        }
    }
}

/// <summary>
/// The factory the app uses. When this app is elevated (it must be, to edit firewall rules),
/// EVERY server is launched as the unelevated desktop user through
/// <see cref="DeElevatedServerRunner"/> — no per-game exceptions. If dropping elevation fails the
/// start is aborted with a clear error; a server is never silently started as administrator.
/// When the app isn't elevated, there is nothing to drop and <see cref="ProcessServerRunnerFactory"/>
/// is used.
/// </summary>
public sealed class DefaultServerRunnerFactory : IServerRunnerFactory
{
    private readonly Func<bool> _isElevated;
    private readonly IServerRunnerFactory _normal;
    private readonly Func<IServerRunnerFactory> _deElevated;

    public DefaultServerRunnerFactory()
        : this(IsElevatedFailClosed, new ProcessServerRunnerFactory(), CreateDeElevatedFactory)
    {
    }

    internal DefaultServerRunnerFactory(Func<bool> isElevated, IServerRunnerFactory normal, Func<IServerRunnerFactory> deElevated)
    {
        _isElevated = isElevated;
        _normal = normal;
        _deElevated = deElevated;
    }

    public IServerRunner Start(string executable, string arguments, string workingDirectory, Action<string> onOutputLine, Action onExited)
    {
        if (!_isElevated())
        {
            return _normal.Start(executable, arguments, workingDirectory, onOutputLine, onExited);
        }

        try
        {
            return _deElevated().Start(executable, arguments, workingDirectory, onOutputLine, onExited);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Could not start the server without administrator rights ({ex.Message}). " +
                "It was not started, so it never runs as administrator. Starting MP Server Provider " +
                "without \"Run as administrator\" avoids this.",
                ex);
        }
    }

    /// <summary>When elevation can't be determined, assume elevated: the de-elevated path is the
    /// safe one.</summary>
    private static bool IsElevatedFailClosed()
    {
        try
        {
            return OperatingSystem.IsWindows() && DeElevatedLauncher.IsCurrentProcessElevated();
        }
        catch
        {
            return true;
        }
    }

    private static IServerRunnerFactory CreateDeElevatedFactory()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Dropping administrator rights is only supported on Windows.");
        }

        return new DeElevatedServerRunnerFactory();
    }
}

/// <summary>
/// Minimal persistence interface this module needs.
/// </summary>
public interface IServerPersistence
{
    void Save(ServerInstance server);

    /// <summary>
    /// Changes only the statistics of the stored server named <paramref name="serverName"/> and
    /// persists them (see <see cref="ServerRepository.UpdateStats"/>). Returns <c>false</c> when
    /// no such server is stored. Never re-saves a stale captured instance, so settings saved while
    /// the server ran survive its exit.
    /// </summary>
    bool UpdateStats(string serverName, Action<ServerStats> mutate);
}

/// <summary>
/// Minimal game-catalog lookup this module needs.
/// // TODO: reconcile with Services.GameLibraryService once that lands.
/// </summary>
public interface IGameLookup
{
    GameDefinition? Find(string gameId);
}

/// <summary>
/// One tracked running (or recently-exited-but-not-yet-reaped) server. Port of the original's
/// internal <c>RunningServer</c> helper type (evidenced in the decompile via
/// <c>RunningServer.get_State</c>/<c>get_Process</c>/<c>get_StartedAt</c>/<c>get_StdIn</c>/
/// <c>ConsoleSnapshot</c>/<c>Append</c>/<c>RegisterCrashForRestart</c> call edges from
/// <see cref="ServerProcessService"/>'s own methods).
/// <para>
/// HARDENING: the console is a lock-guarded ring buffer (it is written from the stdout and stderr
/// threads and the log tail while the UI snapshots it); <see cref="Process"/> is swapped
/// atomically so exactly one exit path disposes it; <see cref="Lifetime"/> ends background work
/// (the log tail) when the process exits or a start is cancelled; and <see cref="Exit"/> completes
/// once the exit has been fully processed, which is what <see cref="ServerProcessService.StopAsync"/>
/// awaits.
/// </para>
/// </summary>
public sealed class RunningServer
{
    private const int MaxConsoleLines = 2000;

    private readonly object _consoleLock = new();
    private readonly string[] _console = new string[MaxConsoleLines];
    private int _consoleStart;
    private int _consoleCount;

    private readonly object _restartLock = new();
    private CancellationTokenSource? _autoRestart;

    private IServerRunner? _process;
    private int _exitHandled;
    private int _stopBegun;

    internal RunningServer(ServerInstance server)
    {
        Server = server;
    }

    public ServerInstance Server { get; }

    public ServerState State { get; set; } = ServerState.Stopped;

    public DateTimeOffset? StartedAt { get; set; }

    public IServerRunner? Process
    {
        get => Volatile.Read(ref _process);
        set => Volatile.Write(ref _process, value);
    }

    public int ConsecutiveCrashes { get; set; }

    /// <summary>Classes already explained by a plugin-error hint this run (one hint each).</summary>
    internal HashSet<string> HintedErrorClasses { get; } = new(StringComparer.Ordinal);

    /// <summary>Set before a stop is requested, so the exit is recorded as a stop, not a crash.</summary>
    internal volatile bool StopRequested;

    /// <summary>Cancelled when the process exits or a pending start is aborted.</summary>
    internal CancellationTokenSource Lifetime { get; } = new();

    /// <summary>Opened once the start finished (successfully or not); exit processing waits for
    /// it so it never races the start path.</summary>
    internal TaskCompletionSource StartGate { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Completed once the process exit (or failed start) has been fully processed.</summary>
    internal TaskCompletionSource Exit { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Atomically takes ownership of <see cref="Process"/> (the caller disposes it).</summary>
    internal IServerRunner? TakeProcess() => Interlocked.Exchange(ref _process, null);

    /// <summary><c>true</c> for the first caller only: exit is processed exactly once.</summary>
    internal bool TryMarkExitHandled() => Interlocked.Exchange(ref _exitHandled, 1) == 0;

    /// <summary><c>true</c> for the first caller only: one stop sequence at a time.</summary>
    internal bool TryBeginStop() => Interlocked.Exchange(ref _stopBegun, 1) == 0;

    internal bool IsAutoRestartPending
    {
        get { lock (_restartLock) { return _autoRestart is not null; } }
    }

    internal CancellationToken BeginAutoRestart()
    {
        lock (_restartLock)
        {
            _autoRestart = new CancellationTokenSource();
            return _autoRestart.Token;
        }
    }

    /// <summary>Ends the pending auto-restart; returns <c>false</c> if it was cancelled.</summary>
    internal bool EndAutoRestart()
    {
        lock (_restartLock)
        {
            var pending = _autoRestart;
            _autoRestart = null;
            var proceed = pending is { IsCancellationRequested: false };
            pending?.Dispose();
            return proceed;
        }
    }

    /// <summary>Cancels a pending auto-restart (manual Start/Stop supersede it).</summary>
    internal void CancelAutoRestart()
    {
        lock (_restartLock)
        {
            _autoRestart?.Cancel();
        }
    }

    /// <summary>Port of <c>RunningServer.Append(string)</c>: a capped, thread-safe ring buffer.</summary>
    public void Append(string line)
    {
        lock (_consoleLock)
        {
            if (_consoleCount < MaxConsoleLines)
            {
                _console[(_consoleStart + _consoleCount) % MaxConsoleLines] = line;
                _consoleCount++;
            }
            else
            {
                _console[_consoleStart] = line;
                _consoleStart = (_consoleStart + 1) % MaxConsoleLines;
            }
        }
    }

    /// <summary>Port of <c>RunningServer.ConsoleSnapshot()</c>.</summary>
    public IReadOnlyList<string> ConsoleSnapshot()
    {
        lock (_consoleLock)
        {
            var snapshot = new string[_consoleCount];
            for (var i = 0; i < _consoleCount; i++)
            {
                snapshot[i] = _console[(_consoleStart + i) % MaxConsoleLines];
            }

            return snapshot;
        }
    }

    /// <summary>Port of <c>RunningServer.RegisterCrashForRestart()</c>.</summary>
    public int RegisterCrashForRestart() => ++ConsecutiveCrashes;
}

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.ServerProcessService</c> (decompile token
/// 0x02000014, see <c>docs/decompile-report/report/MPServerProvider.Core.dll/
/// MPServerProvider.Core.Services/0x02000014_ServerProcessService.md</c>).
///
/// <para>
/// Decompile evidence confirms the overall shape: a dictionary keyed by server name (case
/// insensitive — <c>StringComparer.OrdinalIgnoreCase</c>), a shared <see cref="RconService"/>,
/// <c>StateChanged</c>/<c>OutputReceived</c> events, and the method set
/// <c>StartAsync</c>/<c>StopAsync</c>/<c>RestartAsync</c>/<c>StopAllAsync</c>/
/// <c>SendCommand</c>/<c>StateOf</c>/<c>UptimeOf</c>/<c>ConsoleOf</c>/<c>Dispose</c>, plus the
/// de-elevation integration (<see cref="DefaultServerRunnerFactory"/>), RCON wiring
/// (<c>ConnectRconAsync</c>/<c>PollRconAsync</c>/<c>SetRconPassword</c>), and log-tailing
/// (<c>StartLogTail</c>, which probes <c>console.log</c>, <c>logs/latest.log</c>,
/// <c>server.log</c>, <c>log/server.log</c> under the server's root — string literals confirmed
/// verbatim in the decompile) with crash-restart bookkeeping (<c>OnExited</c>/
/// <c>RecordUptime</c>, <c>AutoRestartOnCrash</c>, a logged "restarting in 5 s…" message).
/// </para>
/// <para>
/// HARDENING: every launch (elevated or not) goes through one <see cref="IServerRunnerFactory"/>,
/// whose exit callback is wired before the process starts; exit processing runs exactly once and
/// is the only place that disposes the process and records uptime/crash stats (through
/// <see cref="IServerPersistence.UpdateStats"/>, never a stale instance); Stop awaits that
/// processing instead of polling; a crash is reported (<see cref="ServerState.Crashed"/>) before
/// any auto-restart, the consecutive-crash counter survives restarts and only resets after a
/// stable run; and no exception escapes an event or fire-and-forget path.
/// </para>
/// </summary>
public sealed partial class ServerProcessService : IDisposable
{
    /// <summary>
    /// Max consecutive crashes before auto-restart is suspended, matching the original's
    /// "Crashed repeatedly — auto-restart suspended." behavior. INFERRED: the exact threshold
    /// literal wasn't recoverable from metadata; 3 is a conventional "stop flapping" cutoff.
    /// </summary>
    public const int MaxConsecutiveCrashes = 3;

    /// <summary>
    /// Default graceful window for Stop (stop command / Ctrl+C) before the process tree is
    /// killed. Generous: a big world can take a while to save.
    /// </summary>
    public static readonly TimeSpan DefaultStopTimeout = TimeSpan.FromSeconds(30);

    /// <summary>Extra time given to a second, harder signal (Ctrl+C after a stop command, or
    /// Ctrl+Break after Ctrl+C) before the kill.</summary>
    internal TimeSpan SecondSignalTimeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>How long to wait for the exit notification after a kill.</summary>
    private static readonly TimeSpan KillConfirmTimeout = TimeSpan.FromSeconds(10);

    private static readonly string[] LogFileCandidates = { "console.log", "logs/latest.log", "server.log", "log/server.log" };

    private readonly ConcurrentDictionary<string, RunningServer> _running = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>The last finished run of each server, so its console stays readable after Stop
    /// (finished runs are removed from <see cref="_running"/>).</summary>
    private readonly ConcurrentDictionary<string, RunningServer> _finished = new(StringComparer.OrdinalIgnoreCase);

    private readonly object _startLock = new();
    private readonly IServerPersistence _repository;
    private readonly IGameLookup _games;
    private readonly IServerRunnerFactory _runnerFactory;
    private readonly Func<int, IProgress<InstallProgress>, CancellationToken, Task<string>> _resolveJava;
    private readonly RconService _rcon = new();
    private string? _rconPassword;

    public ServerProcessService(
        IServerPersistence repository,
        IGameLookup games,
        IServerRunnerFactory? runnerFactory = null,
        Func<int, IProgress<InstallProgress>, CancellationToken, Task<string>>? javaResolver = null)
    {
        _repository = repository;
        _games = games;
        _runnerFactory = runnerFactory ?? new DefaultServerRunnerFactory();
        if (javaResolver is null)
        {
            var java = new Providers.JavaRuntimeProvider();
            javaResolver = java.ResolveAsync;
        }

        _resolveJava = javaResolver;
    }

    /// <summary>
    /// Delay before an auto-restart, matching the decompiled "auto_restart_on_crash is on —
    /// restarting in 5 s…" message.
    /// </summary>
    public TimeSpan AutoRestartDelay { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>A run that lasted at least this long before crashing counts as stable: the
    /// consecutive-crash counter starts over.</summary>
    public TimeSpan CrashCounterResetUptime { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>Port of <c>add_StateChanged</c>/<c>remove_StateChanged</c>.</summary>
    public event Action<ServerInstance, ServerState>? StateChanged;

    /// <summary>Port of <c>add_OutputReceived</c>/<c>remove_OutputReceived</c>.</summary>
    public event Action<ServerInstance, string>? OutputReceived;

    /// <summary>Port of <c>ServerProcessService.StateOf(ServerInstance)</c>.</summary>
    public ServerState StateOf(ServerInstance server) =>
        _running.TryGetValue(server.ServerName, out var r) ? r.State : ServerState.Stopped;

    /// <summary><c>true</c> while a crashed server waits for its automatic restart.</summary>
    public bool IsAutoRestartPending(ServerInstance server) =>
        _running.TryGetValue(server.ServerName, out var r) && r.IsAutoRestartPending;

    /// <summary>Port of <c>ServerProcessService.UptimeOf(ServerInstance)</c>.</summary>
    public TimeSpan UptimeOf(ServerInstance server) =>
        _running.TryGetValue(server.ServerName, out var r) && r.StartedAt is { } startedAt
            ? DateTimeOffset.Now - startedAt
            : TimeSpan.Zero;

    /// <summary>Port of <c>ServerProcessService.ConsoleOf(ServerInstance)</c>. After a server
    /// stopped, its last run's console stays available until it is started again.</summary>
    public IReadOnlyList<string> ConsoleOf(ServerInstance server)
    {
        if (_running.TryGetValue(server.ServerName, out var r) || _finished.TryGetValue(server.ServerName, out r))
        {
            return r.ConsoleSnapshot();
        }

        return Array.Empty<string>();
    }

    /// <summary>Port of <c>ServerProcessService.ProcessOf(ServerInstance)</c>.</summary>
    public IServerRunner? ProcessOf(ServerInstance server) =>
        _running.TryGetValue(server.ServerName, out var r) ? r.Process : null;

    /// <summary>Port of <c>ServerProcessService.get_RunningCount()</c>.</summary>
    public int RunningCount =>
        _running.Values.Count(r => r.State is ServerState.Running or ServerState.Starting);

    /// <summary>
    /// Servers that still need stopping before the app exits: running, starting, stopping, or
    /// crashed with an auto-restart pending.
    /// </summary>
    public IReadOnlyList<ServerInstance> ActiveServers =>
        _running.Values
            .Where(r => r.State is ServerState.Running or ServerState.Starting or ServerState.Stopping || r.IsAutoRestartPending)
            .Select(r => r.Server)
            .ToList();

    /// <summary>
    /// <c>true</c> when the game's console is read from a log file because stdout isn't usable.
    /// Every launch captures stdout/stderr, so log tailing is only needed (and only done) for
    /// <c>console_method: "logfile"</c> games — tailing alongside stdout showed every line twice
    /// (Minecraft writes everything to both).
    /// </summary>
    internal static bool UsesLogFileConsole(GameDefinition game)
    {
        var method = game.Network?.ConsoleMethod;
        return string.Equals(method, "logfile", StringComparison.OrdinalIgnoreCase)
               || string.Equals(method, "log", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Port of <c>ServerProcessService.BuildCommandAsync(ServerInstance)</c>: resolves the
    /// executable, argument string, and working directory to launch, substituting the
    /// well-known placeholders a game definition's <c>Runtime.Arguments</c> template may
    /// contain.
    /// <para>
    /// FIX: the placeholder names here previously didn't match what any real game definition
    /// actually uses — every <c>data/providers/json/*.json</c> template uses
    /// <c>{server_port}</c>/<c>{ram_min_mb}</c>/<c>{ram_max_mb}</c>/<c>{server_name}</c>/
    /// <c>{java}</c>, not the <c>{port}</c>/<c>{ram_min}</c>/<c>{ram_max}</c> this method
    /// substituted (and <c>{java}</c> wasn't substituted at all).
    /// </para>
    /// <para>
    /// Resolving <c>{java}</c> may download a Java runtime on a server's first start: that is
    /// reported through <paramref name="progress"/> (and the console) and is cancellable.
    /// </para>
    /// </summary>
    private async Task<(string Executable, string Arguments, string WorkingDir)> BuildCommandAsync(
        RunningServer running,
        GameDefinition game,
        IProgress<InstallProgress>? progress,
        CancellationToken ct)
    {
        var server = running.Server;
        if (string.IsNullOrEmpty(game.Runtime.Executable))
        {
            throw new InvalidOperationException($"No runtime executable configured for game '{game.Id}'.");
        }

        // FIX (audit #1): the working-dir template ("{server_root}" in every shipped definition)
        // used to be passed to ResolveRoot unsubstituted.
        var workingDir = ResolveWorkingDirectory(server, game);
        if (!Directory.Exists(workingDir))
        {
            throw new InvalidOperationException($"Working directory not found: {workingDir}");
        }

        // {java} needs an actual resolved java.exe path — only resolve it when a template
        // actually references it, so non-Java games never pay for a Java lookup.
        var needsJava = game.Runtime.Executable.Contains("{java}") ||
            (string.IsNullOrEmpty(server.StartCommand) ? game.Runtime.Arguments ?? string.Empty : server.StartCommand).Contains("{java}");
        string? javaPath = null;
        if (needsJava)
        {
            // The installed server's own requirement (set by the Minecraft provider per version —
            // e.g. Java 8 for Forge 1.12) wins over the game definition's generic default.
            var javaMajor = server.JavaMajor > 0 ? server.JavaMajor : game.Runtime.Requires?.Java ?? 21;
            var javaProgress = new CallbackProgress<InstallProgress>(p =>
            {
                AppendLine(running, $"[MP Server Provider] {p.Status}");
                try
                {
                    progress?.Report(p);
                }
                catch
                {
                    // Progress reporting is cosmetic.
                }
            });
            javaPath = await _resolveJava(javaMajor, javaProgress, ct).ConfigureAwait(false);
        }

        string Substitute(string template) => SubstitutePlaceholders(template, server, javaPath);

        var executable = Substitute(game.Runtime.Executable);
        var arguments = string.IsNullOrEmpty(server.StartCommand)
            ? Substitute(game.Runtime.Arguments ?? string.Empty)
            : Substitute(server.StartCommand);

        return (executable, arguments, workingDir);
    }

    /// <summary>
    /// Substitutes the well-known launch placeholders (see BUILD_NOTES.md) in
    /// <paramref name="template"/>. <c>{server_name}</c> is stripped of quotes and control
    /// characters first so a crafted name can't inject extra launch arguments.
    /// </summary>
    internal static string SubstitutePlaceholders(string template, ServerInstance server, string? javaPath) => (template ?? string.Empty)
        .Replace("{server_port}", server.ServerPort.ToString())
        .Replace("{server_ip}", server.ServerIp)
        .Replace("{ram_min_mb}", server.RamMinMb.ToString())
        .Replace("{ram_max_mb}", server.RamMaxMb.ToString())
        .Replace("{server_file}", server.ServerFile)
        .Replace("{server_root}", server.ServerRoot)
        .Replace("{server_name}", new string(server.ServerName.Where(c => c != '"' && !char.IsControl(c)).ToArray()))
        .Replace("{java}", javaPath ?? "java");

    /// <summary>
    /// Resolves the directory a server is launched in (and whose log files are tailed):
    /// the game's <c>working_dir</c> template with placeholders substituted, resolved against
    /// the server root. Defaults to <c>{server_root}</c>.
    /// </summary>
    internal static string ResolveWorkingDirectory(ServerInstance server, GameDefinition game)
    {
        var template = string.IsNullOrWhiteSpace(game.Runtime.WorkingDir) ? "{server_root}" : game.Runtime.WorkingDir;
        return server.ResolveRoot(SubstitutePlaceholders(template, server, javaPath: null));
    }

    /// <summary>
    /// Port of <c>ServerProcessService.SplitCommandLine(string)</c>: splits a single
    /// command-line string into <c>(fileName, arguments)</c>, honoring a quoted first token
    /// (e.g. <c>"\"C:\Program Files\java\bin\java.exe\" -Xmx2G -jar server.jar"</c>).
    /// </summary>
    public static (string FileName, string Arguments) SplitCommandLine(string commandLine)
    {
        var trimmed = commandLine.Trim();
        if (trimmed.StartsWith('"'))
        {
            var end = trimmed.IndexOf('"', 1);
            if (end >= 0)
            {
                var file = trimmed[1..end];
                var rest = trimmed[(end + 1)..].TrimStart();
                return (file, rest);
            }
        }

        var spaceIdx = trimmed.IndexOfAny(new[] { ' ', '\t' });
        return spaceIdx < 0
            ? (trimmed, string.Empty)
            : (trimmed[..spaceIdx], trimmed[(spaceIdx + 1)..].TrimStart());
    }

    /// <summary>Port of <c>ServerProcessService.StartAsync(ServerInstance)</c>.</summary>
    public Task StartAsync(ServerInstance server, CancellationToken ct = default) =>
        StartCoreAsync(server, carriedCrashes: 0, progress: null, ct);

    /// <summary>
    /// Starts <paramref name="server"/>, reporting slow preparation (a first-start Java download)
    /// through <paramref name="progress"/>. Cancelling <paramref name="ct"/> — or calling
    /// <see cref="StopAsync"/> while it is still preparing — aborts the start with an
    /// <see cref="OperationCanceledException"/>.
    /// </summary>
    public Task StartAsync(ServerInstance server, IProgress<InstallProgress>? progress, CancellationToken ct = default) =>
        StartCoreAsync(server, carriedCrashes: 0, progress, ct);

    private async Task StartCoreAsync(ServerInstance server, int carriedCrashes, IProgress<InstallProgress>? progress, CancellationToken ct)
    {
        var running = new RunningServer(server) { State = ServerState.Starting, ConsecutiveCrashes = carriedCrashes };
        lock (_startLock)
        {
            if (_running.TryGetValue(server.ServerName, out var existing))
            {
                if (existing.State is ServerState.Running or ServerState.Starting)
                {
                    throw new InvalidOperationException($"Server '{server.ServerName}' is already running.");
                }

                if (existing.State == ServerState.Stopping)
                {
                    throw new InvalidOperationException($"Server '{server.ServerName}' is still stopping.");
                }

                // A crashed server waiting for its auto-restart: this start supersedes it.
                existing.CancelAutoRestart();
            }

            _running[server.ServerName] = running;
        }

        SetState(running, ServerState.Starting);

        GameDefinition game;
        IServerRunner runner;
        using (var startCts = CancellationTokenSource.CreateLinkedTokenSource(ct, running.Lifetime.Token))
        {
            try
            {
                game = _games.Find(server.ServerGame)
                    ?? throw new InvalidOperationException($"No runtime executable configured for game '{server.ServerGame}'.");
                if (DescribePortConflict(server, game) is { } conflict)
                {
                    throw new InvalidOperationException(conflict);
                }

                SyncPropertiesPort(server, line => AppendLine(running, line));

                var (executable, arguments, workingDir) = await BuildCommandAsync(running, game, progress, startCts.Token).ConfigureAwait(false);
                startCts.Token.ThrowIfCancellationRequested();

                if (DescribeMissingSaveFile(arguments, server.ServerRoot) is { } missing)
                {
                    throw new InvalidOperationException(missing);
                }

                runner = _runnerFactory.Start(
                    executable,
                    arguments,
                    workingDir,
                    line => AppendLine(running, line),
                    () => _ = OnExitedAsync(running));
            }
            catch (Exception ex)
            {
                FailStart(running, ex);
                if (ex is OperationCanceledException && !ct.IsCancellationRequested)
                {
                    throw new OperationCanceledException($"Starting '{server.ServerName}' was cancelled.", ex);
                }

                throw;
            }
        }

        running.Process = runner;
        running.StartedAt = DateTimeOffset.Now;
        if (runner.IsDeElevated)
        {
            AppendLine(running, "[MP Server Provider] Running as your normal user account (not administrator).");
        }

        SetState(running, ServerState.Running);
        running.StartGate.TrySetResult();

        if (running.StopRequested)
        {
            // Stop was clicked in the instant between launch and now: honour it.
            _ = StopQuietlyAsync(server);
            return;
        }

        if (UsesLogFileConsole(game))
        {
            StartLogTail(running, game);
        }
    }

    private async Task StopQuietlyAsync(ServerInstance server)
    {
        try
        {
            await StopAsync(server).ConfigureAwait(false);
        }
        catch
        {
            // Fire-and-forget.
        }
    }

    /// <summary>Unwinds a start that never produced a process.</summary>
    /// <summary>
    /// Explains why <paramref name="server"/>'s port is unavailable, or returns <c>null</c> when
    /// it is free. Without this the game itself started, failed to bind ("Address already in
    /// use") and showed up as a crash, which says nothing about the real cause.
    /// </summary>
    internal string? DescribePortConflict(ServerInstance server, GameDefinition game)
    {
        var port = server.ServerPort;
        if (port is <= 0 or > 65535)
        {
            return null;
        }

        RunningServer[] others;
        lock (_startLock)
        {
            others = _running.Values
                .Where(r => r.Server.ServerName != server.ServerName
                    && r.Server.ServerPort == port
                    && r.State is ServerState.Running or ServerState.Starting)
                .ToArray();
        }

        var protocol = game.Network.Protocol?.ToLowerInvariant();
        string FreeHint()
        {
            int[] taken;
            lock (_startLock)
            {
                taken = _running.Values.Select(r => r.Server.ServerPort).ToArray();
            }

            var free = SuggestFreePort(port + 1, protocol, taken.Append(port));
            return free != port + 1 || !IsPortInUse(free, protocol is not "udp", protocol is "udp" or "both")
                ? $" (port {free} is free)"
                : string.Empty;
        }

        if (others.Length > 0)
        {
            return $"Port {port} is already used by your server '{others[0].Server.ServerName}'. " +
                   $"Stop that server first, or give this one a different port in Server Settings{FreeHint()}.";
        }

        var checkTcp = protocol is not "udp";
        var checkUdp = protocol is "udp" or "both";
        if (!IsPortInUse(port, checkTcp, checkUdp))
        {
            return null;
        }

        var owner = checkTcp ? PortOwner.FindTcpListener(port) : null;
        var who = owner switch
        {
            null => "another program on this PC (often another copy of this game's server, or the game itself hosting a world)",
            "wslrelay" => "a program running inside WSL or Docker (forwarded to Windows by wslrelay.exe)",
            "com.docker.backend" or "vpnkit" => $"a Docker container ({owner})",
            _ => $"another program: {owner}.exe",
        };

        return $"Port {port} is already in use by {who}. Stop that program, or give this server a " +
               $"different port in Server Settings{FreeHint()}.";
    }

    /// <summary>
    /// Minecraft Java/Bedrock read their port from <c>server.properties</c>, not the command
    /// line, so the port set in Server Settings never reached the game. Write it there before
    /// each start (only touching the <c>server-port</c> line, and only when it differs).
    /// </summary>
    internal static void SyncPropertiesPort(ServerInstance server, Action<string>? log = null)
    {
        if (server.ServerPort is <= 0 or > 65535 || string.IsNullOrWhiteSpace(server.ServerRoot))
        {
            return;
        }

        var path = Path.Combine(server.ServerRoot, "server.properties");
        try
        {
            if (!File.Exists(path))
            {
                return;
            }

            var lines = File.ReadAllLines(path).ToList();
            var index = lines.FindIndex(l => l.TrimStart().StartsWith("server-port=", StringComparison.Ordinal));
            var wanted = $"server-port={server.ServerPort}";
            if (index >= 0 && lines[index].Trim() == wanted)
            {
                return;
            }

            if (index >= 0)
            {
                lines[index] = wanted;
            }
            else
            {
                lines.Add(wanted);
            }

            File.WriteAllLines(path, lines);
            log?.Invoke($"[MP Server Provider] Set server-port={server.ServerPort} in server.properties.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            log?.Invoke($"[MP Server Provider] Couldn't update server.properties: {ex.Message}");
        }
    }

    private static readonly string[] SaveFileExtensions = { ".park", ".sv6", ".sv4", ".sc6", ".sc4", ".sea" };

    /// <summary>
    /// A server told to host a save/scenario file (OpenRCT2's <c>host "…\park.park"</c>) that
    /// doesn't exist yet only prints "Unable to open …" and quits. Explain what to put where.
    /// </summary>
    internal static string? DescribeMissingSaveFile(string arguments, string serverRoot)
    {
        foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(arguments ?? string.Empty, "\"([^\"]+)\"|(\\S+)"))
        {
            var token = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
            if (!SaveFileExtensions.Contains(Path.GetExtension(token), StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var path = Path.IsPathRooted(token) ? token : Path.Combine(serverRoot, token);
            if (!File.Exists(path))
            {
                return $"There's no park to host yet: '{Path.GetFileName(path)}' is missing from the server folder. " +
                       $"In OpenRCT2, open or create a park and use File > Save Game As, then copy that save into the " +
                       $"server folder (Open Folder) and name it '{Path.GetFileName(path)}'.";
            }
        }

        return null;
    }

    /// <summary>
    /// First port at or above <paramref name="preferred"/> that no other installed server is
    /// configured for and no program on this PC is listening on (game default first, so the
    /// usual port is kept whenever it is free).
    /// </summary>
    public static int SuggestFreePort(int preferred, string? protocol, IEnumerable<int> reservedByServers)
    {
        if (preferred is <= 0 or > 65535)
        {
            preferred = 25565;
        }

        var reserved = reservedByServers.ToHashSet();
        var p = protocol?.ToLowerInvariant();
        var tcp = p is not "udp";
        var udp = p is "udp" or "both";
        for (var port = preferred; port <= Math.Min(65535, preferred + 200); port++)
        {
            if (!reserved.Contains(port) && !IsPortInUse(port, tcp, udp))
            {
                return port;
            }
        }

        return preferred;
    }

    internal static bool IsPortInUse(int port, bool tcp, bool udp)
    {
        try
        {
            var properties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            if (tcp && properties.GetActiveTcpListeners().Any(e => e.Port == port))
            {
                return true;
            }

            if (udp && properties.GetActiveUdpListeners().Any(e => e.Port == port))
            {
                return true;
            }
        }
        catch
        {
            // Can't tell: let the server try, as before.
        }

        return false;
    }

    private void FailStart(RunningServer running, Exception ex)
    {
        running.TryMarkExitHandled();
        running.Lifetime.Cancel();
        AppendLine(running, ex is OperationCanceledException
            ? "[MP Server Provider] Start cancelled."
            : $"[MP Server Provider] Could not start: {ex.Message}");
        Retire(running);
        SetState(running, ServerState.Stopped);
        running.StartGate.TrySetResult();
        running.Exit.TrySetResult();
    }

    /// <summary>Removes a finished run from the running table (keeping its console readable).</summary>
    private void Retire(RunningServer running)
    {
        _finished[running.Server.ServerName] = running;
        _running.TryRemove(new KeyValuePair<string, RunningServer>(running.Server.ServerName, running));
    }

    /// <summary>
    /// Port of <c>ServerProcessService.StartLogTail(RunningServer, ServerInstance,
    /// GameDefinition)</c>: waits (up to a minute) for one of the well-known log files to exist
    /// (exact relative paths confirmed in the decompile) and tails it, forwarding new lines like
    /// live stdout. Ends when the run's <see cref="RunningServer.Lifetime"/> is cancelled (exit or
    /// stop); the file is opened with <see cref="FileShare.Delete"/> so it never blocks log
    /// rotation or deleting the server.
    /// </summary>
    private void StartLogTail(RunningServer running, GameDefinition game)
    {
        var token = running.Lifetime.Token;
        var root = ResolveWorkingDirectory(running.Server, game);

        _ = Task.Run(async () =>
        {
            try
            {
                string? found = null;
                var existedAtStart = false;
                for (var attempt = 0; attempt < 60 && !token.IsCancellationRequested; attempt++)
                {
                    found = LogFileCandidates.Select(rel => Path.Combine(root, rel)).FirstOrDefault(File.Exists);
                    if (found is not null)
                    {
                        existedAtStart = attempt == 0;
                        break;
                    }

                    await Task.Delay(1000, token).ConfigureAwait(false);
                }

                if (found is null)
                {
                    AppendLine(running, "[MP Server Provider] No log file found for logfile console mode.");
                    return;
                }

                AppendLine(running, $"[MP Server Provider] Tailing log file: {found}");
                using var fileStream = new FileStream(found, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var reader = new StreamReader(fileStream);
                if (existedAtStart)
                {
                    // A log left over from the previous run: only show what this run writes.
                    fileStream.Seek(0, SeekOrigin.End);
                }

                while (!token.IsCancellationRequested)
                {
                    var line = await reader.ReadLineAsync(token).ConfigureAwait(false);
                    if (line is null)
                    {
                        await Task.Delay(500, token).ConfigureAwait(false);
                        continue;
                    }

                    AppendLine(running, line);
                }
            }
            catch (OperationCanceledException)
            {
                // The run ended.
            }
            catch (Exception ex)
            {
                // File deleted/rotated away mid-tail, access denied, ...: stop tailing.
                AppendLine(running, $"[MP Server Provider] Stopped reading the log file: {ex.Message}");
            }
        });
    }

    /// <summary>Port of <c>ServerProcessService.ConnectRconAsync(RunningServer, ServerInstance, GameDefinition)</c>.</summary>
    public Task ConnectRconAsync(ServerInstance server, int port, CancellationToken ct = default) =>
        _rcon.ConnectAsync(server.ServerIp, port, _rconPassword ?? string.Empty, TimeSpan.FromSeconds(5), ct);

    /// <summary>Port of <c>ServerProcessService.PollRconAsync(RunningServer)</c>: best-effort reconnection attempt, swallowing failures.</summary>
    public async Task PollRconAsync(ServerInstance server, int port, CancellationToken ct = default)
    {
        if (_rcon.Connected)
        {
            return;
        }
        try
        {
            await ConnectRconAsync(server, port, ct).ConfigureAwait(false);
        }
        catch
        {
            // Best-effort: the server may simply not have RCON enabled, or not be ready yet.
        }
    }

    /// <summary>Port of <c>ServerProcessService.SetRconPassword(string)</c>.</summary>
    public void SetRconPassword(string password) => _rconPassword = password;

    /// <summary>Port of <c>ServerProcessService.SendCommand(ServerInstance, string)</c>.
    /// Fire-and-forget; never throws.</summary>
    public void SendCommand(ServerInstance server, string command)
    {
        if (!_running.TryGetValue(server.ServerName, out var running))
        {
            return;
        }

        _ = SendCommandCoreAsync(running, command);
    }

    /// <summary>Echoes and delivers <paramref name="command"/> (RCON when connected, otherwise
    /// stdin). Returns <c>true</c> when it was delivered. Never throws.</summary>
    private async Task<bool> SendCommandCoreAsync(RunningServer running, string command)
    {
        try
        {
            AppendLine(running, $"> {command}");

            if (_rcon.Connected)
            {
                await _rcon.SendCommandAsync(command, TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                return true;
            }

            if (running.Process is { HasExited: false } process)
            {
                await process.WriteLineAsync(command).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                return true;
            }
        }
        catch
        {
            // Best-effort: stdin may already be closed if the process is exiting, or RCON dropped.
        }

        return false;
    }

    /// <summary>
    /// Port of <c>ServerProcessService.StopAsync(ServerInstance, TimeSpan?)</c>. Graceful first:
    /// the server's stop command over stdin, or — for games without one (e.g. Valheim) — Ctrl+C
    /// to its console. If it hasn't exited within <paramref name="gracefulTimeout"/>, a harder
    /// signal gets a few more seconds, then the whole process tree is killed. Returns once the
    /// exit has been fully processed (uptime recorded, state Stopped). Stopping a server that is
    /// still preparing to start (e.g. downloading Java) cancels the start; stopping a crashed
    /// server cancels its pending auto-restart.
    /// </summary>
    public async Task StopAsync(ServerInstance server, TimeSpan? gracefulTimeout = null, CancellationToken ct = default)
    {
        if (!_running.TryGetValue(server.ServerName, out var running))
        {
            throw new InvalidOperationException($"Server '{server.ServerName}' is not running.");
        }

        if (running.State is ServerState.Crashed or ServerState.Stopped && running.Process is null)
        {
            running.CancelAutoRestart();
            Retire(running);
            SetState(running, ServerState.Stopped);
            return;
        }

        if (running.State == ServerState.Starting && running.Process is null)
        {
            running.StopRequested = true;
            running.Lifetime.Cancel();
            await running.Exit.Task.WaitAsync(ct).ConfigureAwait(false);
            return;
        }

        if (!running.TryBeginStop())
        {
            // Another Stop is already in progress: just wait for it.
            await running.Exit.Task.WaitAsync(ct).ConfigureAwait(false);
            return;
        }

        running.StopRequested = true;
        running.CancelAutoRestart();
        SetState(running, ServerState.Stopping);

        var exit = running.Exit.Task;
        var process = running.Process;
        if (process is not null && !exit.IsCompleted)
        {
            var usedStopCommand = !string.IsNullOrWhiteSpace(server.StopCommand)
                && await SendCommandCoreAsync(running, server.StopCommand).ConfigureAwait(false);
            if (!usedStopCommand && await Task.Run(() => SafeInterrupt(process, ctrlBreak: false)).ConfigureAwait(false))
            {
                AppendLine(running, "[MP Server Provider] Sent Ctrl+C — waiting for the server to save and exit…");
            }

            var exited = await WaitAsync(exit, gracefulTimeout ?? DefaultStopTimeout, ct).ConfigureAwait(false);

            if (!exited && await Task.Run(() => SafeInterrupt(process, ctrlBreak: !usedStopCommand)).ConfigureAwait(false))
            {
                AppendLine(running, usedStopCommand
                    ? "[MP Server Provider] Still running — sent Ctrl+C."
                    : "[MP Server Provider] Still running — sent Ctrl+Break.");
                exited = await WaitAsync(exit, SecondSignalTimeout, ct).ConfigureAwait(false);
            }

            if (!exited)
            {
                AppendLine(running, "[MP Server Provider] The server did not stop in time — forcing it to close.");
                process.Kill();
                exited = await WaitAsync(exit, KillConfirmTimeout, ct).ConfigureAwait(false);
            }

            if (!exited)
            {
                // No exit notification even after the kill: process the exit ourselves (the
                // exactly-once guard makes a late notification a no-op).
                await OnExitedAsync(running).ConfigureAwait(false);
            }
        }
        else if (process is null)
        {
            await OnExitedAsync(running).ConfigureAwait(false);
        }

        await exit.WaitAsync(ct).ConfigureAwait(false);
        _rcon.Disconnect();
    }

    private static bool SafeInterrupt(IServerRunner process, bool ctrlBreak)
    {
        try
        {
            return process.TrySendInterrupt(ctrlBreak);
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> WaitAsync(Task task, TimeSpan timeout, CancellationToken ct)
    {
        try
        {
            await task.WaitAsync(timeout, ct).ConfigureAwait(false);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    /// <summary>Port of <c>ServerProcessService.RestartAsync(ServerInstance)</c>.</summary>
    public async Task RestartAsync(ServerInstance server, CancellationToken ct = default)
    {
        if (_running.ContainsKey(server.ServerName))
        {
            await StopAsync(server, ct: ct).ConfigureAwait(false);
        }
        await StartAsync(server, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Port of <c>ServerProcessService.StopAllAsync()</c>: stops every active server IN PARALLEL,
    /// each with a generous graceful window (<see cref="DefaultStopTimeout"/> unless
    /// <paramref name="gracefulTimeout"/> says otherwise), so every server gets the same chance to
    /// save. Never throws.
    /// </summary>
    public async Task StopAllAsync(TimeSpan? gracefulTimeout = null)
    {
        var servers = _running.Values.Select(r => r.Server).ToList();
        await Task.WhenAll(servers.Select(async server =>
        {
            try
            {
                await StopAsync(server, gracefulTimeout ?? DefaultStopTimeout).ConfigureAwait(false);
            }
            catch
            {
                // Best-effort shutdown: one server's failure to stop cleanly must not block the rest.
            }
        })).ConfigureAwait(false);
    }

    /// <summary>
    /// Port of <c>ServerProcessService.OnExited(RunningServer)</c>. The ONLY place that disposes
    /// a run's process and records its uptime/crash stats, guarded to run exactly once per run.
    /// A requested stop ends as Stopped. Anything else is a crash: Crashed is raised right away;
    /// if <see cref="ServerInstance.AutoRestartOnCrash"/> is on, a restart follows after
    /// <see cref="AutoRestartDelay"/>, carrying the consecutive-crash count, up to
    /// <see cref="MaxConsecutiveCrashes"/> crashes in a row (a run that lasted
    /// <see cref="CrashCounterResetUptime"/> resets the count). Never throws.
    /// </summary>
    private async Task OnExitedAsync(RunningServer running)
    {
        try
        {
            await running.StartGate.Task.ConfigureAwait(false);
            if (!running.TryMarkExitHandled())
            {
                return;
            }

            var server = running.Server;
            var process = running.TakeProcess();
            var exitCode = SafeExitCode(process);
            var startedAt = running.StartedAt;
            running.StartedAt = null;
            running.Lifetime.Cancel();
            try
            {
                process?.Dispose();
            }
            catch
            {
                // Disposal is best-effort.
            }

            var uptime = startedAt is { } started ? DateTimeOffset.Now - started : TimeSpan.Zero;
            var crashed = !running.StopRequested;
            RecordStats(running, uptime, crashed);

            if (!crashed)
            {
                AppendLine(running, $"[MP Server Provider] Server stopped (exit code {exitCode}).");
                Retire(running);
                SetState(running, ServerState.Stopped);
                running.Exit.TrySetResult();
                return;
            }

            AppendLine(running, $"[MP Server Provider] Server exited unexpectedly (code {exitCode}).");
            if (uptime >= CrashCounterResetUptime)
            {
                running.ConsecutiveCrashes = 0;
            }

            var crashes = running.RegisterCrashForRestart();
            var restart = server.AutoRestartOnCrash && crashes <= MaxConsecutiveCrashes;
            var restartToken = restart ? running.BeginAutoRestart() : CancellationToken.None;

            SetState(running, ServerState.Crashed);
            running.Exit.TrySetResult();

            if (!restart)
            {
                if (server.AutoRestartOnCrash)
                {
                    AppendLine(running, "[MP Server Provider] Crashed repeatedly — auto-restart suspended.");
                }
                return;
            }

            AppendLine(running, $"[MP Server Provider] auto_restart_on_crash is on — restarting in {AutoRestartDelay.TotalSeconds:0} s…");
            try
            {
                await Task.Delay(AutoRestartDelay, restartToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Stopped or started manually meanwhile.
            }

            if (!running.EndAutoRestart()
                || !_running.TryGetValue(server.ServerName, out var current)
                || !ReferenceEquals(current, running))
            {
                return;
            }

            try
            {
                await StartCoreAsync(server, crashes, progress: null, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                AppendLine(running, $"[MP Server Provider] Auto-restart failed: {ex.Message}");
                // The failed attempt retired itself; the server is still crashed.
                _running.TryAdd(server.ServerName, running);
                SetState(running, ServerState.Crashed);
            }
        }
        catch (Exception ex)
        {
            try
            {
                running.Append($"[MP Server Provider] Internal error while handling the server's exit: {ex.Message}");
            }
            catch
            {
                // Nothing more to do.
            }

            running.Exit.TrySetResult();
        }
    }

    private static int SafeExitCode(IServerRunner? process)
    {
        try
        {
            return process is { HasExited: true } ? process.ExitCode : 0;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Port of <c>ServerProcessService.RecordUptime(RunningServer)</c>: adds the run's uptime (and
    /// the crash, before persisting — it used to be counted after the save and never stored) to
    /// the stored server's stats via <see cref="IServerPersistence.UpdateStats"/>.
    /// </summary>
    private void RecordStats(RunningServer running, TimeSpan uptime, bool crashed)
    {
        var elapsed = Math.Max(0, (long)uptime.TotalSeconds);

        void Mutate(ServerStats stats)
        {
            stats.TotalUptimeSeconds += elapsed;
            stats.MonthUptimeSeconds += elapsed;
            if (crashed)
            {
                stats.CrashCount++;
            }
        }

        try
        {
            if (!_repository.UpdateStats(running.Server.ServerName, Mutate))
            {
                // Not (or no longer) stored, e.g. deleted: keep the in-memory numbers right.
                Mutate(running.Server.Stats);
            }
        }
        catch (Exception ex)
        {
            AppendLine(running, $"[MP Server Provider] Could not save the server's statistics: {ex.Message}");
        }
    }

    private void SetState(RunningServer running, ServerState state)
    {
        running.State = state;
        var handlers = StateChanged;
        if (handlers is null)
        {
            return;
        }

        foreach (var handler in handlers.GetInvocationList())
        {
            try
            {
                ((Action<ServerInstance, ServerState>)handler)(running.Server, state);
            }
            catch
            {
                // A subscriber's failure must not break state tracking (or escape a process thread).
            }
        }
    }

    private void AppendLine(RunningServer running, string line)
    {
        line = StripTerminalCodes(line);
        running.Append(line);
        var handlers = OutputReceived;
        if (handlers is not null)
        {
            foreach (var handler in handlers.GetInvocationList())
            {
                try
                {
                    ((Action<ServerInstance, string>)handler)(running.Server, line);
                }
                catch
                {
                    // Output subscribers must never break the pump threads.
                }
            }
        }

        if (PluginErrorHint.ForLine(line, running.Server, running.HintedErrorClasses) is { } hint)
        {
            AppendLine(running, hint);
        }
    }

    /// <summary>
    /// Removes terminal control sequences from a console line. Servers like OpenRCT2, Paper and
    /// FXServer color their output with ANSI escapes (<c>ESC[31m</c> … <c>ESC[0m</c>); the
    /// console box shows plain text, so they appeared as stray "[31m"/"[0m" symbols.
    /// </summary>
    internal static string StripTerminalCodes(string line)
    {
        if (line.IndexOf('\u001b') < 0 && line.IndexOf('\u009b') < 0)
        {
            return line;
        }

        line = TerminalCodeRegex().Replace(line, string.Empty);

        // Any other lone control characters (bell, a stray ESC) except tab.
        return new string(line.Where(c => c == '\t' || !char.IsControl(c)).ToArray());
    }

    // CSI sequences (ESC[ … final byte, or the 8-bit CSI), OSC sequences (ESC] … BEL/ESC\), and
    // two-character ESC sequences (e.g. ESC(B, ESC=).
    [System.Text.RegularExpressions.GeneratedRegex(@"(?:\u001b\[|\u009b)[0-?]*[ -/]*[@-~]|\u001b\][^\u0007\u001b]*(?:\u0007|\u001b\\)?|\u001b[ -/]*[0-~]")]
    private static partial System.Text.RegularExpressions.Regex TerminalCodeRegex();

    /// <summary>Port of <c>ServerProcessService.Dispose()</c>: force-kills every tracked process
    /// tree and clears the running-server table. Call <see cref="StopAllAsync"/> first for a
    /// graceful shutdown.</summary>
    public void Dispose()
    {
        foreach (var running in _running.Values)
        {
            running.CancelAutoRestart();
            running.Lifetime.Cancel();
            var process = running.TakeProcess();
            if (process is null)
            {
                continue;
            }

            running.TryMarkExitHandled();
            try
            {
                process.Kill();
                process.Dispose();
            }
            catch
            {
                // Best-effort teardown.
            }
        }
        _running.Clear();
        _rcon.Dispose();
    }

    /// <summary>Synchronous <see cref="IProgress{T}"/> (unlike <see cref="Progress{T}"/> it doesn't
    /// post to a captured context).</summary>
    private sealed class CallbackProgress<T>(Action<T> report) : IProgress<T>
    {
        public void Report(T value) => report(value);
    }
}
