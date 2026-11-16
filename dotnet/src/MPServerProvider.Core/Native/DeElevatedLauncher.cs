using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace MPServerProvider.Core.Native;

[SupportedOSPlatform("windows")]
public static class DeElevatedLauncher
{
    private const uint PROCESS_QUERY_INFORMATION = 0x0400;
    private const uint TOKEN_DUPLICATE = 0x0002;
    private const uint TOKEN_ALL_ACCESS = 0x000F01FF;
    private const uint TOKEN_QUERY = 0x0008;

    private const int SecurityImpersonation = 2;
    private const int TokenPrimary = 1;

    private const uint LOGON_WITH_PROFILE = 0x00000001;
    private const uint CREATE_SUSPENDED = 0x00000004;
    private const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;
    private const uint CREATE_NO_WINDOW = 0x08000000;

    private const int STARTF_USESHOWWINDOW = 0x00000001;
    private const int STARTF_USESTDHANDLES = 0x00000100;
    private const short SW_HIDE = 0;
    private const uint HANDLE_FLAG_INHERIT = 0x00000001;

    public enum TokenSource
    {
        DesktopUser,

        CurrentProcess,
    }

    public sealed class RedirectedLaunch : IDisposable
    {
        private IntPtr _thread;

        internal RedirectedLaunch(SafeProcessHandle process, IntPtr thread, int processId, SafeFileHandle stdIn, SafeFileHandle stdOut, SafeFileHandle stdErr)
        {
            Process = process;
            _thread = thread;
            ProcessId = processId;
            StandardInput = stdIn;
            StandardOutput = stdOut;
            StandardError = stdErr;
        }

        public SafeProcessHandle Process { get; }

        public int ProcessId { get; }

        public SafeFileHandle StandardInput { get; }

        public SafeFileHandle StandardOutput { get; }

        public SafeFileHandle StandardError { get; }

        public void Resume()
        {
            var thread = Interlocked.Exchange(ref _thread, IntPtr.Zero);
            if (thread == IntPtr.Zero)
            {
                return;
            }

            try
            {
                if (ResumeThread(thread) == uint.MaxValue)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "ResumeThread failed");
                }
            }
            finally
            {
                CloseHandle(thread);
            }
        }

        public void Dispose()
        {
            var thread = Interlocked.Exchange(ref _thread, IntPtr.Zero);
            if (thread != IntPtr.Zero)
            {
                CloseHandle(thread);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetHandleInformation(SafeHandle hObject, uint dwMask, uint dwFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint ResumeThread(IntPtr hThread);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    // Launches are serialized on ConsoleCtrl.LaunchGate: the child-side pipe ends are inheritable
    // for the duration of a launch (a concurrent launch must not inherit another server's pipes),
    // and a child must not inherit the temporary "ignore Ctrl+C" flag ConsoleCtrl sets.

    [StructLayout(LayoutKind.Sequential)]
    private struct STARTUPINFO
    {
        public int cb;
        public string? lpReserved;
        public string? lpDesktop;
        public string? lpTitle;
        public int dwX;
        public int dwY;
        public int dwXSize;
        public int dwYSize;
        public int dwXCountChars;
        public int dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetShellWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool DuplicateTokenEx(
        IntPtr hExistingToken,
        uint dwDesiredAccess,
        IntPtr lpTokenAttributes,
        int impersonationLevel,
        int tokenType,
        out IntPtr phNewToken);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CreateProcessWithTokenW(
        IntPtr hToken,
        uint dwLogonFlags,
        string? lpApplicationName,
        StringBuilder lpCommandLine,
        uint dwCreationFlags,
        IntPtr lpEnvironment,
        string? lpCurrentDirectory,
        ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CreateProcessAsUser(
        IntPtr hToken,
        string? lpApplicationName,
        StringBuilder lpCommandLine,
        IntPtr lpProcessAttributes,
        IntPtr lpThreadAttributes,
        bool bInheritHandles,
        uint dwCreationFlags,
        IntPtr lpEnvironment,
        string? lpCurrentDirectory,
        ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private static IntPtr OpenShellUserToken()
    {
        var shellHwnd = GetShellWindow();
        if (shellHwnd == IntPtr.Zero)
        {
            throw new InvalidOperationException("No shell window — cannot obtain the desktop user's token.");
        }

        var threadId = GetWindowThreadProcessId(shellHwnd, out var shellPid);
        if (threadId == 0 || shellPid == 0)
        {
            throw new InvalidOperationException("Could not identify the shell process.");
        }

        var explorerProcess = OpenProcess(PROCESS_QUERY_INFORMATION, false, shellPid);
        if (explorerProcess == IntPtr.Zero)
        {
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "OpenProcess(explorer) failed");
        }

        try
        {
            if (!OpenProcessToken(explorerProcess, TOKEN_DUPLICATE, out var explorerToken))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "OpenProcessToken(explorer) failed");
            }

            try
            {
                if (!DuplicateTokenEx(
                        explorerToken,
                        TOKEN_ALL_ACCESS,
                        IntPtr.Zero,
                        SecurityImpersonation,
                        TokenPrimary,
                        out var duplicatedToken))
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "DuplicateTokenEx failed");
                }

                return duplicatedToken;
            }
            finally
            {
                CloseHandle(explorerToken);
            }
        }
        finally
        {
            CloseHandle(explorerProcess);
        }
    }

    public static int Launch(string commandLine, string? workingDirectory = null)
    {
        var token = OpenShellUserToken();
        try
        {
            var startupInfo = new STARTUPINFO
            {
                cb = Marshal.SizeOf<STARTUPINFO>(),
            };
            var creationFlags = CREATE_NO_WINDOW | CREATE_UNICODE_ENVIRONMENT;

            var cmdLinePrimary = new StringBuilder(commandLine);
            if (CreateProcessWithTokenW(
                    token,
                    LOGON_WITH_PROFILE,
                    null,
                    cmdLinePrimary,
                    creationFlags,
                    IntPtr.Zero,
                    workingDirectory,
                    ref startupInfo,
                    out var processInfoPrimary))
            {
                CloseHandle(processInfoPrimary.hThread);
                CloseHandle(processInfoPrimary.hProcess);
                return processInfoPrimary.dwProcessId;
            }

            var withTokenError = Marshal.GetLastWin32Error();

            var cmdLineFallback = new StringBuilder(commandLine);
            if (CreateProcessAsUser(
                    token,
                    null,
                    cmdLineFallback,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    creationFlags,
                    IntPtr.Zero,
                    workingDirectory,
                    ref startupInfo,
                    out var processInfoFallback))
            {
                CloseHandle(processInfoFallback.hThread);
                CloseHandle(processInfoFallback.hProcess);
                return processInfoFallback.dwProcessId;
            }

            var asUserError = Marshal.GetLastWin32Error();
            throw new System.ComponentModel.Win32Exception(
                asUserError,
                $"Could not start the server as the desktop user (CreateProcessWithTokenW error {withTokenError}, CreateProcessAsUser error {asUserError}).");
        }
        finally
        {
            CloseHandle(token);
        }
    }

    private static IntPtr OpenCurrentProcessToken()
    {
        if (!OpenProcessToken(GetCurrentProcess(), TOKEN_DUPLICATE | TOKEN_QUERY, out var own))
        {
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "OpenProcessToken(self) failed");
        }

        try
        {
            if (!DuplicateTokenEx(own, TOKEN_ALL_ACCESS, IntPtr.Zero, SecurityImpersonation, TokenPrimary, out var duplicated))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "DuplicateTokenEx failed");
            }

            return duplicated;
        }
        finally
        {
            CloseHandle(own);
        }
    }

    public static RedirectedLaunch LaunchSuspendedRedirected(string commandLine, string? workingDirectory, TokenSource tokenSource = TokenSource.DesktopUser)
    {
        lock (ConsoleCtrl.LaunchGate)
        {
            SafeFileHandle? stdInRead = null, stdInWrite = null, stdOutRead = null, stdOutWrite = null, stdErrRead = null, stdErrWrite = null;
            var token = IntPtr.Zero;
            var success = false;
            try
            {
                CreateInheritablePipe(out stdInRead, out stdInWrite, parentEndIsRead: false);
                CreateInheritablePipe(out stdOutRead, out stdOutWrite, parentEndIsRead: true);
                CreateInheritablePipe(out stdErrRead, out stdErrWrite, parentEndIsRead: true);

                token = tokenSource == TokenSource.CurrentProcess ? OpenCurrentProcessToken() : OpenShellUserToken();

                var startupInfo = new STARTUPINFO
                {
                    cb = Marshal.SizeOf<STARTUPINFO>(),
                    dwFlags = STARTF_USESTDHANDLES | STARTF_USESHOWWINDOW,
                    wShowWindow = SW_HIDE,
                    hStdInput = stdInRead.DangerousGetHandle(),
                    hStdOutput = stdOutWrite.DangerousGetHandle(),
                    hStdError = stdErrWrite.DangerousGetHandle(),
                };
                var creationFlags = CREATE_SUSPENDED | CREATE_NO_WINDOW | CREATE_UNICODE_ENVIRONMENT;

                PROCESS_INFORMATION info;
                if (!CreateProcessWithTokenW(token, LOGON_WITH_PROFILE, null, new StringBuilder(commandLine), creationFlags,
                        IntPtr.Zero, workingDirectory, ref startupInfo, out info))
                {
                    var withTokenError = Marshal.GetLastWin32Error();
                    if (!CreateProcessAsUser(token, null, new StringBuilder(commandLine), IntPtr.Zero, IntPtr.Zero, true,
                            creationFlags, IntPtr.Zero, workingDirectory, ref startupInfo, out info))
                    {
                        var asUserError = Marshal.GetLastWin32Error();
                        throw new System.ComponentModel.Win32Exception(
                            asUserError,
                            $"Could not start the server as the desktop user (CreateProcessWithTokenW error {withTokenError}, CreateProcessAsUser error {asUserError}).");
                    }
                }

                var launch = new RedirectedLaunch(
                    new SafeProcessHandle(info.hProcess, ownsHandle: true),
                    info.hThread,
                    info.dwProcessId,
                    stdInWrite,
                    stdOutRead,
                    stdErrRead);
                success = true;
                return launch;
            }
            finally
            {
                // The child has its own copies of these now (or never started): this process must
                // not keep them, or the child's stdout would never reach EOF.
                stdInRead?.Dispose();
                stdOutWrite?.Dispose();
                stdErrWrite?.Dispose();
                if (!success)
                {
                    stdInWrite?.Dispose();
                    stdOutRead?.Dispose();
                    stdErrRead?.Dispose();
                }

                if (token != IntPtr.Zero)
                {
                    CloseHandle(token);
                }
            }
        }
    }

    private static void CreateInheritablePipe(out SafeFileHandle read, out SafeFileHandle write, bool parentEndIsRead)
    {
        var attributes = new SECURITY_ATTRIBUTES
        {
            nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>(),
            bInheritHandle = true,
        };
        if (!CreatePipe(out read, out write, ref attributes, 0))
        {
            throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "CreatePipe failed");
        }

        if (!SetHandleInformation(parentEndIsRead ? read : write, HANDLE_FLAG_INHERIT, 0))
        {
            var error = Marshal.GetLastWin32Error();
            read.Dispose();
            write.Dispose();
            throw new System.ComponentModel.Win32Exception(error, "SetHandleInformation failed");
        }
    }

    public static bool IsCurrentProcessElevated()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
