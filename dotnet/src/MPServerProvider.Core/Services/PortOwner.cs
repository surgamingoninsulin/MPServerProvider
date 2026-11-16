using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace MPServerProvider.Core.Services;

internal static class PortOwner
{
    private const int TcpTableOwnerPidListener = 3;
    private const uint NoError = 0;
    private const uint InsufficientBuffer = 122;

    public static string? FindTcpListener(int port)
    {
        if (!OperatingSystem.IsWindows())
        {
            return null;
        }

        try
        {
            var pid = FindPid(port, AddressFamily.InterNetwork) ?? FindPid(port, AddressFamily.InterNetworkV6);
            if (pid is not { } id || id == 0)
            {
                return null;
            }

            using var process = Process.GetProcessById((int)id);
            return process.ProcessName;
        }
        catch
        {
            return null;
        }
    }

    private static uint? FindPid(int port, AddressFamily family)
    {
        var size = 0;
        var af = family == AddressFamily.InterNetwork ? 2 : 23;
        var result = GetExtendedTcpTable(IntPtr.Zero, ref size, false, af, TcpTableOwnerPidListener, 0);
        if (result != InsufficientBuffer || size <= 0)
        {
            return null;
        }

        var buffer = Marshal.AllocHGlobal(size);
        try
        {
            if (GetExtendedTcpTable(buffer, ref size, false, af, TcpTableOwnerPidListener, 0) != NoError)
            {
                return null;
            }

            var count = Marshal.ReadInt32(buffer);
            // MIB_TCPROW_OWNER_PID: state, localAddr, localPort, remoteAddr, remotePort, pid (6 x 4 bytes).
            // MIB_TCP6ROW_OWNER_PID: localAddr[16], localScope, localPort, remoteAddr[16], remoteScope,
            // remotePort, state, pid (56 bytes).
            var rowSize = family == AddressFamily.InterNetwork ? 24 : 56;
            var portOffset = family == AddressFamily.InterNetwork ? 8 : 20;
            var pidOffset = family == AddressFamily.InterNetwork ? 20 : 52;
            for (var i = 0; i < count; i++)
            {
                var row = buffer + 4 + (i * rowSize);
                var rawPort = (uint)Marshal.ReadInt32(row + portOffset);
                var localPort = (int)(((rawPort & 0xFF) << 8) | ((rawPort >> 8) & 0xFF));
                if (localPort == port)
                {
                    return (uint)Marshal.ReadInt32(row + pidOffset);
                }
            }

            return null;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    [DllImport("iphlpapi.dll", SetLastError = true)]
    private static extern uint GetExtendedTcpTable(IntPtr table, ref int size, bool order, int addressFamily, int tableClass, uint reserved);
}
