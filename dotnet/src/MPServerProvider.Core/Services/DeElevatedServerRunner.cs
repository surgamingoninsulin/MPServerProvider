using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Microsoft.Win32.SafeHandles;
using MPServerProvider.Core.Native;

namespace MPServerProvider.Core.Services;

/// <summary>
/// <see cref="IServerRunner"/> for a server launched as the unelevated desktop user while this
/// app runs elevated (supersedes the old PID-only <c>DeElevatedProcess</c>, which left such
/// servers untracked: Stop didn't stop them, exits went unnoticed, the console stayed empty and
/// they outlived the app). It behaves exactly like <see cref="ProcessServerRunner"/>:
/// <list type="bullet">
/// <item>stdin/stdout/stderr are anonymous pipes (<c>STARTF_USESTDHANDLES</c>), so the console
/// shows output and Send/stop commands reach the server;</item>
/// <item>exit is detected with a thread-pool wait on the process handle, registered before the
/// (suspended) process is resumed, and raises <see cref="Exited"/> exactly once;</item>
/// <item>the process is put in a <see cref="KillOnCloseJob"/> before it runs, so <see cref="Kill"/>
/// takes down the whole tree and the server dies with the app if the app crashes;</item>
/// <item><see cref="ProcessId"/> is the real PID, so CPU/RAM metrics work.</item>
/// </list>
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class DeElevatedServerRunner : IServerRunner
{
    private const uint WAIT_OBJECT_0 = 0;
    private const uint STILL_ACTIVE = 259;

    /// <summary>How long exit notification waits for the output pipes to drain (a grandchild
    /// that inherited them can keep them open indefinitely).</summary>
    private static readonly TimeSpan OutputDrainTimeout = TimeSpan.FromSeconds(2);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(SafeProcessHandle hHandle, uint dwMilliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetExitCodeProcess(SafeProcessHandle hProcess, out uint lpExitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool TerminateProcess(SafeProcessHandle hProcess, uint uExitCode);

    private readonly SafeProcessHandle _process;
    private readonly KillOnCloseJob? _job;
    private readonly FileStream _stdinStream;
    private readonly StreamWriter _stdin;
    private readonly SemaphoreSlim _stdinLock = new(1, 1);
    private readonly Action<string> _onOutputLine;
    private readonly ProcessWaitHandle _waitHandle;
    private RegisteredWaitHandle? _registration;
    private Task _stdoutPump = Task.CompletedTask;
    private Task _stderrPump = Task.CompletedTask;
    private int _exitRaised;
    private int _disposed;
    private volatile bool _killed;

    private DeElevatedServerRunner(DeElevatedLauncher.RedirectedLaunch launch, KillOnCloseJob? job, Action<string> onOutputLine)
    {
        _process = launch.Process;
        ProcessId = launch.ProcessId;
        _job = job;
        _onOutputLine = onOutputLine;
        _stdinStream = new FileStream(launch.StandardInput, FileAccess.Write, 4096, isAsync: false);
        _stdin = new StreamWriter(_stdinStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)) { AutoFlush = false };
        _waitHandle = new ProcessWaitHandle(_process);
        _stdoutPump = StartPump(launch.StandardOutput);
        _stderrPump = StartPump(launch.StandardError);
    }

    public int ProcessId { get; }

    public bool IsDeElevated { get; private init; } = true;

    public bool HasExited => _process.IsClosed || WaitForSingleObject(_process, 0) == WAIT_OBJECT_0;

    public int ExitCode
    {
        get
        {
            if (!GetExitCodeProcess(_process, out var code))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "GetExitCodeProcess failed");
            }

            if (code == STILL_ACTIVE && !HasExited)
            {
                throw new InvalidOperationException("The process has not exited yet.");
            }

            return unchecked((int)code);
        }
    }

    public event EventHandler? Exited;

    /// <summary>
    /// Launches <paramref name="executable"/> as the unelevated desktop user (or, for
    /// diagnostics, with <paramref name="tokenSource"/>). <paramref name="onExited"/> is wired
    /// before the process runs, so even an instant exit is reported. Throws when the launch fails;
    /// nothing is left running in that case.
    /// </summary>
    public static DeElevatedServerRunner Start(
        string executable,
        string arguments,
        string workingDirectory,
        Action<string> onOutputLine,
        Action? onExited,
        DeElevatedLauncher.TokenSource tokenSource = DeElevatedLauncher.TokenSource.DesktopUser)
    {
        var commandLine = string.IsNullOrWhiteSpace(arguments) ? $"\"{executable}\"" : $"\"{executable}\" {arguments}";
        var launch = DeElevatedLauncher.LaunchSuspendedRedirected(commandLine, workingDirectory, tokenSource);
        DeElevatedServerRunner? runner = null;
        try
        {
            var job = KillOnCloseJob.TryCreate();
            if (job is not null && !job.TryAssign(launch.Process))
            {
                job.Dispose();
                job = null;
            }

            runner = new DeElevatedServerRunner(launch, job, onOutputLine)
            {
                IsDeElevated = tokenSource == DeElevatedLauncher.TokenSource.DesktopUser,
            };
            if (onExited is not null)
            {
                runner.Exited += (_, _) => onExited();
            }

            runner._registration = ThreadPool.RegisterWaitForSingleObject(
                runner._waitHandle,
                static (state, _) => ((DeElevatedServerRunner)state!).OnProcessSignaled(),
                runner,
                Timeout.Infinite,
                executeOnlyOnce: true);

            launch.Resume();
            return runner;
        }
        catch
        {
            try
            {
                TerminateProcess(launch.Process, 1);
            }
            catch
            {
                // Best-effort: the launch is being abandoned.
            }

            if (runner is not null)
            {
                runner.Dispose();
            }
            else
            {
                launch.StandardInput.Dispose();
                launch.StandardOutput.Dispose();
                launch.StandardError.Dispose();
                launch.Process.Dispose();
            }

            launch.Dispose();
            throw;
        }
    }

    public async Task WriteLineAsync(string line, CancellationToken ct = default)
    {
        await _stdinLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            await _stdin.WriteLineAsync(line.AsMemory(), ct).ConfigureAwait(false);
            await _stdin.FlushAsync(ct).ConfigureAwait(false);
        }
        finally
        {
            _stdinLock.Release();
        }
    }

    public bool TrySendInterrupt(bool ctrlBreak = false) => !HasExited && ConsoleCtrl.TrySend(ProcessId, ctrlBreak);

    public void Kill()
    {
        _killed = true;
        try
        {
            _job?.Terminate();
        }
        catch
        {
            // Fall through to terminating the main process directly.
        }

        try
        {
            if (!_process.IsClosed && !HasExited)
            {
                TerminateProcess(_process, 1);
            }
        }
        catch
        {
            // Best-effort: it may have exited in the meantime.
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _registration?.Unregister(null);

        if (_job is not null)
        {
            bool exitedOnItsOwn;
            try
            {
                exitedOnItsOwn = HasExited && !_killed;
            }
            catch
            {
                exitedOnItsOwn = false;
            }

            // Natural exit: leave anything it intentionally started alone. Still running (app
            // shutting down): closing the job kills the tree.
            if (exitedOnItsOwn)
            {
                _job.Release();
            }
            else
            {
                _job.Dispose();
            }
        }

        try
        {
            _stdin.Dispose();
        }
        catch
        {
            // The pipe may already be broken (child gone).
        }

        _stdinLock.Dispose();
        _waitHandle.Dispose();
        _process.Dispose();
    }

    private Task StartPump(SafeFileHandle readEnd) => Task.Factory.StartNew(
        () =>
        {
            try
            {
                using var stream = new FileStream(readEnd, FileAccess.Read, 4096, isAsync: false);
                using var reader = new StreamReader(stream, new UTF8Encoding(false), detectEncodingFromByteOrderMarks: false);
                string? line;
                while ((line = reader.ReadLine()) is not null)
                {
                    try
                    {
                        _onOutputLine(line);
                    }
                    catch
                    {
                        // A failing output consumer must not stop the pump (or crash the app).
                    }
                }
            }
            catch
            {
                // Broken pipe / handle closed: the output simply ends.
            }
        },
        CancellationToken.None,
        TaskCreationOptions.LongRunning,
        TaskScheduler.Default);

    private void OnProcessSignaled()
    {
        try
        {
            // Let the last lines the server printed reach the console before reporting the exit.
            Task.WhenAll(_stdoutPump, _stderrPump).Wait(OutputDrainTimeout);
        }
        catch
        {
            // Draining is best-effort.
        }

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

    /// <summary>Waitable view of a process handle (signaled when the process exits). Does not
    /// own the handle.</summary>
    private sealed class ProcessWaitHandle : WaitHandle
    {
        private readonly SafeProcessHandle _process;
        private bool _addedRef;

        public ProcessWaitHandle(SafeProcessHandle process)
        {
            _process = process;
            process.DangerousAddRef(ref _addedRef);
            SafeWaitHandle = new SafeWaitHandle(process.DangerousGetHandle(), ownsHandle: false);
        }

        protected override void Dispose(bool explicitDisposing)
        {
            base.Dispose(explicitDisposing);
            if (_addedRef)
            {
                _addedRef = false;
                _process.DangerousRelease();
            }
        }
    }
}

/// <summary><see cref="IServerRunnerFactory"/> that launches every server through
/// <see cref="DeElevatedServerRunner"/> (as the unelevated desktop user).</summary>
[SupportedOSPlatform("windows")]
public sealed class DeElevatedServerRunnerFactory : IServerRunnerFactory
{
    private readonly DeElevatedLauncher.TokenSource _tokenSource;

    public DeElevatedServerRunnerFactory(DeElevatedLauncher.TokenSource tokenSource = DeElevatedLauncher.TokenSource.DesktopUser)
    {
        _tokenSource = tokenSource;
    }

    public IServerRunner Start(string executable, string arguments, string workingDirectory, Action<string> onOutputLine, Action onExited)
        => DeElevatedServerRunner.Start(executable, arguments, workingDirectory, onOutputLine, onExited, _tokenSource);
}
