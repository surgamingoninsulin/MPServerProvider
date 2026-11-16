using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace MPServerProvider.Core.Native;

/// <summary>
/// Sends Ctrl+C / Ctrl+Break to a console server process, the way pressing them in its console
/// window would. Used to stop servers that have no stdin stop command (e.g. Valheim, which saves
/// the world on Ctrl+C) gracefully before resorting to a kill.
/// <para>
/// A GUI process has no console of its own, so this temporarily attaches to the target's console
/// (<c>AttachConsole</c>), raises the event for every process on it
/// (<c>GenerateConsoleCtrlEvent(…, 0)</c>), and detaches again. This process is attached at the
/// moment the event is raised, so it receives the event too: a handler that swallows Ctrl+C and
/// Ctrl+Break is registered first (registered handlers, unlike the "ignore Ctrl+C" flag, are not
/// inherited by servers started later). Calls are serialized: a process can only be attached to
/// one console at a time. When this process already has a console (e.g. a test host) attaching
/// fails and nothing is sent.
/// </para>
/// </summary>
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

    /// <summary>Time allowed for an event to reach every process before the temporary
    /// "ignore Ctrl+C" flag is cleared.</summary>
    private static readonly TimeSpan DeliveryGrace = TimeSpan.FromMilliseconds(250);

    /// <summary>
    /// Server launches lock this so a child never inherits the temporary "ignore Ctrl+C" flag
    /// set while an event is being delivered.
    /// </summary>
    public static object LaunchGate => Gate;

    // Kept in a static field so the delegate is never collected while registered.
    private static HandlerRoutine? _swallowHandler;

    /// <summary>Sends Ctrl+C (or Ctrl+Break) to the console of <paramref name="processId"/>.
    /// Returns <c>true</c> when the event was raised.</summary>
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
