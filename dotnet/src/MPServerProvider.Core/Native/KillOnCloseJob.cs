using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

namespace MPServerProvider.Core.Native;

/// <summary>
/// A Win32 Job object configured with <c>JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE</c>, one per launched
/// server. Every process in the job (the server and anything it spawns) is terminated when the
/// job handle is closed — including when this app crashes or is killed, because the OS closes
/// the handle then — so servers can never be left running invisibly after the panel is gone.
/// <see cref="Terminate"/> kills the whole tree at once (used by Stop's last-resort kill).
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class KillOnCloseJob : IDisposable
{
    private const int JobObjectExtendedLimitInformation = 9;
    private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000;

    [StructLayout(LayoutKind.Sequential)]
    private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public uint LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public uint ActiveProcessLimit;
        public UIntPtr Affinity;
        public uint PriorityClass;
        public uint SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct IO_COUNTERS
    {
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }

    private sealed class SafeJobHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeJobHandle()
            : base(ownsHandle: true)
        {
        }

        protected override bool ReleaseHandle() => CloseHandle(handle);
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateJobObjectW")]
    private static extern SafeJobHandle CreateJobObject(IntPtr lpJobAttributes, string? lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetInformationJobObject(SafeJobHandle hJob, int infoClass, ref JOBOBJECT_EXTENDED_LIMIT_INFORMATION info, uint cbInfo);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AssignProcessToJobObject(SafeJobHandle hJob, SafeHandle hProcess);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool TerminateJobObject(SafeJobHandle hJob, uint uExitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    private readonly SafeJobHandle _handle;

    private KillOnCloseJob(SafeJobHandle handle)
    {
        _handle = handle;
    }

    /// <summary>Creates a new kill-on-close job. Throws <see cref="Win32Exception"/> on failure.</summary>
    public static KillOnCloseJob Create()
    {
        var handle = CreateJobObject(IntPtr.Zero, null);
        if (handle.IsInvalid)
        {
            var error = Marshal.GetLastWin32Error();
            handle.Dispose();
            throw new Win32Exception(error, "CreateJobObject failed");
        }

        var job = new KillOnCloseJob(handle);
        if (!job.SetKillOnClose(true))
        {
            var error = Marshal.GetLastWin32Error();
            job.Dispose();
            throw new Win32Exception(error, "SetInformationJobObject failed");
        }

        return job;
    }

    /// <summary>Like <see cref="Create"/>, but returns <c>null</c> instead of throwing.</summary>
    public static KillOnCloseJob? TryCreate()
    {
        try
        {
            return Create();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Puts <paramref name="processHandle"/> into this job. Returns <c>false</c> on failure.</summary>
    public bool TryAssign(SafeHandle processHandle)
    {
        try
        {
            return !_handle.IsClosed && AssignProcessToJobObject(_handle, processHandle);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Terminates every process in the job (the whole server process tree).</summary>
    public void Terminate(uint exitCode = 1)
    {
        if (!_handle.IsClosed)
        {
            TerminateJobObject(_handle, exitCode);
        }
    }

    /// <summary>
    /// Closes the job WITHOUT killing what is still in it: clears the kill-on-close limit first.
    /// Used after the server's main process exited on its own, so a launcher that intentionally
    /// left a background process running (e.g. a <c>.bat</c> that <c>start</c>s the real server)
    /// isn't killed behind the user's back.
    /// </summary>
    public void Release()
    {
        if (_handle.IsClosed)
        {
            return;
        }

        SetKillOnClose(false);
        _handle.Dispose();
    }

    /// <summary>Closes the job handle, which terminates every process still in it.</summary>
    public void Dispose() => _handle.Dispose();

    private bool SetKillOnClose(bool enabled)
    {
        var info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
        info.BasicLimitInformation.LimitFlags = enabled ? JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE : 0;
        return SetInformationJobObject(_handle, JobObjectExtendedLimitInformation, ref info, (uint)Marshal.SizeOf<JOBOBJECT_EXTENDED_LIMIT_INFORMATION>());
    }
}
