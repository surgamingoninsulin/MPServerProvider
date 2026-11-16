using System.Diagnostics;
using System.Management;
using System.Runtime.Versioning;

namespace MPServerProvider.Core.Services;

public sealed record SystemSpecs(string Processor, string MemoryGb, string Graphics, string OperatingSystem);

public sealed record DriveUsage(string Name, string Root, long TotalBytes, long FreeBytes)
{
    public long UsedBytes => TotalBytes - FreeBytes;

    public double UsedPercent => TotalBytes == 0 ? 0.0 : (double)UsedBytes / TotalBytes * 100.0;
}

[SupportedOSPlatform("windows")]
public sealed class SystemMetricsService : IDisposable
{
    public static readonly string[] RealFileSystems = { "NTFS", "ReFS", "exFAT" };

    private readonly PerformanceCounter? _cpuCounter;
    private SystemSpecs? _specs;

    public SystemMetricsService()
    {
        // Constructs the "% Processor Time"/"_Total" counter and warms it
        // up with one throwaway NextValue() call (PerformanceCounter's first read is always 0).
        // Guarded: a corrupt/rebuilding performance-counter category (surprisingly common across
        // real Windows installs) used to throw straight out of this constructor, which runs
        // during AppServices.Initialize() at startup — i.e. it could take down the whole app
        // before a single window ever showed, not just the Settings page.
        try
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _cpuCounter.NextValue();
        }
        catch
        {
            _cpuCounter = null;
        }
    }

    public double CpuPercent()
    {
        if (_cpuCounter is null)
        {
            return 0.0;
        }

        try
        {
            var value = (double)_cpuCounter.NextValue();
            return Math.Clamp(value, 0.0, 100.0);
        }
        catch
        {
            return 0.0;
        }
    }

    public (double UsedGb, long TotalBytes, long FreeBytes) Ram()
    {
        // Fast path: GlobalMemoryStatusEx is a cheap kernel call, unlike a WMI query (which
        // the Settings page used to run on the UI thread every 2 s).
        if (OperatingSystem.IsWindows())
        {
            var status = new MemoryStatusEx { Length = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MemoryStatusEx>() };
            if (GlobalMemoryStatusEx(ref status) && status.TotalPhys > 0)
            {
                var totalBytes = (long)status.TotalPhys;
                var freeBytes = (long)status.AvailPhys;
                return ((totalBytes - freeBytes) / (1024.0 * 1024.0 * 1024.0), totalBytes, freeBytes);
            }
        }

        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (ManagementBaseObject obj in searcher.Get())
            {
                var totalKb = Convert.ToInt64(obj["TotalVisibleMemorySize"]);
                var freeKb = Convert.ToInt64(obj["FreePhysicalMemory"]);
                var totalBytes = totalKb * 1024;
                var freeBytes = freeKb * 1024;
                var usedBytes = totalBytes - freeBytes;
                return (usedBytes / (1024.0 * 1024.0 * 1024.0), totalBytes, freeBytes);
            }
        }
        catch
        {
            // Best-effort, matching QueryNames' tolerance below: WMI can transiently fail (COM
            // marshaling hiccups, the WMI service being slow to start, locked-down environments).
            // This used to be unguarded, which crashed the whole app the moment SettingsPage's
            // one-shot Specs() call hit a bad WMI response on startup.
        }

        return (0.0, 0, 0);
    }

    public SystemSpecs Specs()
    {
        if (_specs is not null)
        {
            return _specs;
        }

        var processor = QueryNames("SELECT Name FROM Win32_Processor") is { Count: > 0 } cpuNames
            ? string.Join(" / ", cpuNames)
            : "Unknown";

        var graphics = QueryNames("SELECT Name FROM Win32_VideoController") is { Count: > 0 } gpuNames
            ? string.Join(" / ", gpuNames)
            : "Unknown";

        // The spec is the machine's installed (total) RAM, not how much is in use right now.
        var (_, totalBytes, _) = Ram();
        var memoryGb = $"{totalBytes / (1024.0 * 1024.0 * 1024.0):0.0} GB RAM";

        // "win32 10.0.x" ("win32" is just Windows' API family name) used to be shown, which
        // read as a 32-bit OS. Show the product name and the real architecture instead.
        var operatingSystem = DescribeOperatingSystem();

        _specs = new SystemSpecs(processor, memoryGb, graphics, operatingSystem);
        return _specs;
    }

    internal static string DescribeOperatingSystem()
    {
        var bits = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture switch
        {
            System.Runtime.InteropServices.Architecture.X64 => "64-bit",
            System.Runtime.InteropServices.Architecture.Arm64 => "ARM 64-bit",
            System.Runtime.InteropServices.Architecture.X86 => "32-bit",
            var other => other.ToString(),
        };

        string? caption = null;
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementBaseObject obj in searcher.Get())
            {
                caption = (obj["Caption"] as string)?.Trim();
                break;
            }
        }
        catch
        {
            // Fall back to the generic name below.
        }

        if (string.IsNullOrWhiteSpace(caption))
        {
            // Build 22000+ is Windows 11 even though its version number still says 10.0.
            caption = Environment.OSVersion.Version.Build >= 22000 ? "Windows 11" : $"Windows {Environment.OSVersion.Version.Major}";
        }

        return $"{caption} {bits} (build {Environment.OSVersion.Version.Build})";
    }

    private static List<string> QueryNames(string query)
    {
        var names = new List<string>();
        try
        {
            using var searcher = new ManagementObjectSearcher(query);
            foreach (ManagementBaseObject obj in searcher.Get())
            {
                if (obj["Name"] is string name && !string.IsNullOrWhiteSpace(name))
                {
                    names.Add(name);
                }
            }
        }
        catch
        {
            // Best-effort: WMI can be unavailable/locked down in some environments; the
            // call site falls back to "Unknown".
        }
        return names;
    }

    public List<DriveUsage> Drives()
    {
        var drives = new List<DriveUsage>();
        foreach (var drive in DriveInfo.GetDrives())
        {
            if (drive.DriveType != DriveType.Fixed)
            {
                continue;
            }
            if (!drive.IsReady || !RealFileSystems.Contains(drive.DriveFormat))
            {
                continue;
            }

            drives.Add(new DriveUsage(drive.Name, drive.RootDirectory.FullName, drive.TotalSize, drive.AvailableFreeSpace));
        }
        return drives;
    }

    public static long DirectorySizeBytes(string path)
    {
        if (!Directory.Exists(path))
        {
            return 0;
        }

        // IgnoreInaccessible: one access-denied folder used to throw out of the enumeration and
        // abort the whole Settings page refresh. Reparse points are skipped so a junction can't
        // make the walk count (or loop through) folders outside the tree.
        var options = new System.IO.EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            AttributesToSkip = FileAttributes.ReparsePoint,
        };

        long total = 0;
        try
        {
            foreach (var file in new DirectoryInfo(path).EnumerateFiles("*", options))
            {
                try
                {
                    total += file.Length;
                }
                catch
                {
                    // Best-effort: a file disappearing mid-walk must not abort the whole sum.
                }
            }
        }
        catch
        {
            // Best-effort: the folder itself vanished/became unreadable mid-walk.
        }

        return total;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct MemoryStatusEx
    {
        public uint Length;
        public uint MemoryLoad;
        public ulong TotalPhys;
        public ulong AvailPhys;
        public ulong TotalPageFile;
        public ulong AvailPageFile;
        public ulong TotalVirtual;
        public ulong AvailVirtual;
        public ulong AvailExtendedVirtual;
    }

    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx buffer);

    public void Dispose() => _cpuCounter?.Dispose();
}
