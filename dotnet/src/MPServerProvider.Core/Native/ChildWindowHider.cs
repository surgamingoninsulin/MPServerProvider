using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace MPServerProvider.Core.Native;

/// <summary>
/// Some dedicated servers (Unreal Engine games launched with "-log", e.g. Abiotic Factor, ARK,
/// Astroneer) open their own visible log window on Windows regardless of
/// <see cref="System.Diagnostics.ProcessStartInfo.CreateNoWindow"/> - that flag only suppresses a
/// console host, not a GUI window the child creates for itself. This polls for any top-level
/// window owned by the child process for a short time after launch and hides it as soon as it
/// appears; the app's own Console tab already gets the same output via redirected stdout, so
/// hiding the window loses nothing.
/// </summary>
[SupportedOSPlatform("windows")]
public static class ChildWindowHider
{
    private const int SW_HIDE = 0;
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x80;

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    /// <summary>Watches for up to <paramref name="duration"/> and hides any visible top-level
    /// window the process opens, including ones that appear after engine init finishes. Runs on
    /// the thread pool and returns immediately; safe to fire-and-forget.</summary>
    public static async Task WatchAndHideAsync(int processId, TimeSpan duration, CancellationToken ct)
    {
        var deadline = DateTime.UtcNow + duration;
        try
        {
            while (DateTime.UtcNow < deadline && !ct.IsCancellationRequested)
            {
                HideWindowsOf(processId);
                await Task.Delay(300, ct).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Server stopped/the app is shutting down; nothing left to hide.
        }
    }

    private static void HideWindowsOf(int processId)
    {
        try
        {
            EnumWindows((hWnd, _) =>
            {
                GetWindowThreadProcessId(hWnd, out var ownerPid);
                if (ownerPid == (uint)processId && IsWindowVisible(hWnd) && !IsToolWindow(hWnd))
                {
                    ShowWindow(hWnd, SW_HIDE);
                }
                return true;
            }, IntPtr.Zero);
        }
        catch
        {
            // Best-effort cosmetic cleanup; never worth failing the server over.
        }
    }

    private static bool IsToolWindow(IntPtr hWnd) =>
        (GetWindowLongPtr(hWnd, GWL_EXSTYLE).ToInt64() & WS_EX_TOOLWINDOW) != 0;
}
