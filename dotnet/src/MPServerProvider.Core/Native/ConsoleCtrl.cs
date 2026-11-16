using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace MPServerProvider.Core.Native;

[SupportedOSPlatform("windows")]
public static class ConsoleCtrl
{
    private const uint CTRL_C_EVENT = 0;
    private const uint CTRL_BREAK_EVENT = 1;

    private delegate bool HandlerRoutine(uint ctrlType);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleCtrlHandler(HandlerRoutine? handler, bool add);

    private static readonly object Gate = new();

    private static readonly TimeSpan DeliveryGrace = TimeSpan.FromMilliseconds(250);

    public static object LaunchGate => Gate;

    // Kept in a static field so the delegate is never collected while registered.
    private static HandlerRoutine? _swallowHandler;

    public static bool TrySend(int processId, bool ctrlBreak = false)
    {
        lock (Gate)
        {
            try
            {
                _swallowHandler ??= type => type is CTRL_C_EVENT or CTRL_BREAK_EVENT;

                if (!AttachConsole((uint)processId))
                {
                    return false;
                }

                var sent = false;
                var ignoring = false;
                try
                {
                    // Registered after attaching (a handler added while this process had no
                    // console doesn't take effect); re-registering just moves it to the front.
                    // It covers Ctrl+Break; Ctrl+C is additionally ignored at the OS level, which
                    // needs no managed code — so a delivery that lands while this app is shutting
                    // down can't kill it either.
                    SetConsoleCtrlHandler(_swallowHandler, false);
                    if (!SetConsoleCtrlHandler(_swallowHandler, true))
                    {
                        return false;
                    }

                    ignoring = SetConsoleCtrlHandler(null, true);
                    sent = GenerateConsoleCtrlEvent(ctrlBreak ? CTRL_BREAK_EVENT : CTRL_C_EVENT, 0);
                }
                finally
                {
                    FreeConsole();
                    if (ignoring)
                    {
                        // The event is delivered asynchronously. The "ignore Ctrl+C" flag is
                        // inherited by processes started meanwhile (they could then never be
                        // stopped with Ctrl+C), so launches wait on LaunchGate until it's cleared.
                        Thread.Sleep(DeliveryGrace);
                        SetConsoleCtrlHandler(null, false);
                    }
                }

                return sent;
            }
            catch
            {
                return false;
            }
        }
    }
}
