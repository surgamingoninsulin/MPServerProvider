# MPServerProvider.Core.Services.SystemMetricsService

Token: `0x02000019`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_cpuCounter` : `class:405`  (flags=33, token=0x04000047)
- `_specs` : `class:92`  (flags=1, token=0x04000048)
- `RealFileSystems` : `string[]`  (flags=49, token=0x04000049)

## Methods

### `.ctor()` -> `void`
- token `0x060000de`  flags=`6278`  rva=`31224`
- IL size 45 bytes, 13 instructions, max_stack=8
- opcodes: ldarg.0x3, ldstrx3, callx1, callvirtx1, ldfldx1, newobjx1, popx1, retx1, stfldx1
- string literals: "Processor", "% Processor Time", "_Total"
- calls: System.Diagnostics.PerformanceCounter..ctor, System.Object..ctor, System.Diagnostics.PerformanceCounter.NextValue

### `CpuPercent()` -> `r8`
- token `0x060000df`  flags=`134`  rva=`31272`
- IL size 45 bytes, 15 instructions, max_stack=3
- opcodes: ldc.r4x2, leave.sx2, stloc.0x2, callx1, callvirtx1, conv.r8x1, ldarg.0x1, ldc.r8x1, ldfldx1, ldloc.0x1, popx1, retx1
- calls: System.Diagnostics.PerformanceCounter.NextValue, System.Math.Clamp

### `Ram()` -> `valuetype:365<r8,i8,i8>`
- token `0x060000e0`  flags=`134`  rva=`31348`
- IL size 175 bytes, 70 instructions, max_stack=3
- opcodes: callvirtx8, ldloc.sx5, leave.sx5, conv.i8x4, ldloc.1x4, ldloc.0x3, ldstrx3, mulx3, newobjx3, stloc.sx3, brfalse.sx2, conv.r8x2, endfinallyx2, ldc.i4x2, ldc.i4.0x2, ldc.r8x2, ldloc.2x2, retx2, unbox.anyx2, br.sx1, brtrue.sx1, divx1, dupx1, ldloc.3x1, popx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1, subx1
- string literals: "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem", "TotalVisibleMemorySize", "FreePhysicalMemory"
- calls: System.Management.ManagementObjectSearcher..ctor, System.Management.ManagementObjectSearcher.Get, System.Management.ManagementObjectCollection.GetEnumerator, ManagementObjectEnumerator.get_Current, System.Management.ManagementBaseObject.get_Item, System.ValueTuple`3..ctor, ManagementObjectEnumerator.MoveNext, System.IDisposable.Dispose

### `Specs()` -> `class:92`
- token `0x060000e1`  flags=`134`  rva=`31576`
- IL size 411 bytes, 128 instructions, max_stack=7
- opcodes: callx18, ldstrx10, dupx8, ldloca.sx8, ldsfldx8, newobjx7, ldloc.sx6, callvirtx5, ldarg.0x5, popx5, brtrue.sx4, ldfldx4, ldftnx4, leave.sx4, stsfldx4, brfalse.sx3, endfinallyx2, ldc.i4.1x2, retx2, stloc.0x2, stloc.1x2, stloc.2x2, stloc.sx2, conv.r8x1, divx1, ldc.i4.6x1, ldc.i4.7x1, ldc.r8x1, ldloc.0x1, ldloc.1x1, ldloc.2x1, ldloc.3x1, stfldx1, stloc.3x1
- string literals: "Unknown", "Unknown", "", "SELECT Name FROM Win32_Processor", " / ", "SELECT Name FROM Win32_VideoController", " / ", "0.0", " GB RAM", "win32 "
- calls: System.Management.ManagementObjectSearcher..ctor, System.Management.ManagementObjectSearcher.Get, System.Linq.Enumerable.Cast, <>c.<Specs>b__5_0, System.Func`2..ctor, System.Linq.Enumerable.Select, <>c.<Specs>b__5_1, System.Linq.Enumerable.Where, System.String.Join, System.IDisposable.Dispose, <>c.<Specs>b__5_2, <>c.<Specs>b__5_3, MPServerProvider.Core.Services.SystemMetricsService.Ram, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Environment.get_OSVersion, System.OperatingSystem.get_Version, MPServerProvider.Core.Services.SystemSpecs..ctor

### `Drives()` -> `class:269<class:96>`
- token `0x060000e2`  flags=`134`  rva=`32040`
- IL size 83 bytes, 23 instructions, max_stack=3
- opcodes: callx4, dupx4, ldsfldx4, brtrue.sx2, ldftnx2, newobjx2, popx2, stsfldx2, retx1
- calls: System.IO.DriveInfo.GetDrives, <>c.<Drives>b__7_0, System.Func`2..ctor, System.Linq.Enumerable.Where, <>c.<Drives>b__7_1, System.Linq.Enumerable.Select, System.Linq.Enumerable.ToList

### `DirectorySizeBytes(string)` -> `i8`
- token `0x060000e3`  flags=`150`  rva=`32136`
- IL size 71 bytes, 30 instructions, max_stack=3
- opcodes: callx3, brtrue.sx2, conv.i8x2, dupx2, ldarg.0x2, ldc.i4.0x2, ldsfldx2, leave.sx2, popx2, retx2, stloc.0x2, ldc.i4.1x1, ldftnx1, ldloc.0x1, ldstrx1, newobjx1, nopx1, stsfldx1
- string literals: "*"
- calls: System.IO.Directory.Exists, System.IO.Directory.EnumerateFiles, <>c.<DirectorySizeBytes>b__8_0, System.Func`2..ctor, System.Linq.Enumerable.Sum

### `Dispose()` -> `void`
- token `0x060000e4`  flags=`486`  rva=`32236`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callvirtx1, ldarg.0x1, ldfldx1, retx1
- calls: System.ComponentModel.Component.Dispose

### `.cctor()` -> `void`
- token `0x060000e5`  flags=`6289`  rva=`32249`
- IL size 36 bytes, 16 instructions, max_stack=8
- opcodes: dupx3, ldstrx3, stelem.refx3, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.3x1, newarrx1, retx1, stsfldx1
- string literals: "NTFS", "ReFS", "exFAT"

