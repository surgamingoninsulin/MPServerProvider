# MPServerProvider.Core.Services.ServerRepository

Token: `0x02000015`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_paths` : `class:16`  (flags=33, token=0x04000038)
- `_servers` : `class:153<class:220>`  (flags=33, token=0x04000039)
- `Changed` : `class:389`  (flags=1, token=0x0400003a)
- `Deleted` : `class:193<class:220>`  (flags=1, token=0x0400003b)
- `DeleteAttempts` : `i4`  (flags=32849, token=0x0400003c)

## Methods

### `get_Servers()` -> `class:269<class:220>`
- token `0x0600009b`  flags=`2182`  rva=`27805`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `add_Changed(class:389)` -> `void`
- token `0x0600009c`  flags=`2182`  rva=`27816`
- IL size 41 bytes, 20 instructions, max_stack=3
- opcodes: ldloc.1x3, callx2, ldarg.0x2, ldloc.0x2, stloc.0x2, bne.un.sx1, castclassx1, ldarg.1x1, ldfldx1, ldfldax1, ldloc.2x1, retx1, stloc.1x1, stloc.2x1
- calls: System.Delegate.Combine, System.Threading.Interlocked.CompareExchange

### `remove_Changed(class:389)` -> `void`
- token `0x0600009d`  flags=`2182`  rva=`27872`
- IL size 41 bytes, 20 instructions, max_stack=3
- opcodes: ldloc.1x3, callx2, ldarg.0x2, ldloc.0x2, stloc.0x2, bne.un.sx1, castclassx1, ldarg.1x1, ldfldx1, ldfldax1, ldloc.2x1, retx1, stloc.1x1, stloc.2x1
- calls: System.Delegate.Remove, System.Threading.Interlocked.CompareExchange

### `.ctor(class:16)` -> `void`
- token `0x0600009e`  flags=`6278`  rva=`27925`
- IL size 25 bytes, 9 instructions, max_stack=8
- opcodes: ldarg.0x3, stfldx2, callx1, ldarg.1x1, newobjx1, retx1
- calls: System.Collections.Generic.List`1..ctor, System.Object..ctor

### `Reload()` -> `void`
- token `0x0600009f`  flags=`134`  rva=`27952`
- IL size 171 bytes, 64 instructions, max_stack=2
- opcodes: callvirtx11, callx7, ldarg.0x7, ldloc.2x6, ldfldx5, ldloc.0x4, leave.sx4, brfalse.sx3, brtrue.sx3, retx3, ldloc.1x2, popx2, br.sx1, dupx1, endfinallyx1, ldstrx1, stloc.0x1, stloc.1x1, stloc.2x1
- string literals: "server.json"
- calls: System.Collections.Generic.List`1.Clear, MPServerProvider.Core.Services.AppPaths.get_ServersDir, System.IO.Directory.Exists, System.IO.Directory.EnumerateDirectories, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.IO.Path.Combine, MPServerProvider.Core.Json.Json.ReadFile, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Models.ServerInstance.set_SourceFile, MPServerProvider.Core.Services.ServerRepository.RollMonthCounter, MPServerProvider.Core.Services.ServerRepository.MigrateCfxToTxAdmin, System.Collections.Generic.List`1.Add, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, System.Action.Invoke

### `Find(string)` -> `class:220`
- token `0x060000a0`  flags=`134`  rva=`28164`
- IL size 37 bytes, 12 instructions, max_stack=3
- opcodes: ldloc.0x2, newobjx2, callx1, ldarg.0x1, ldarg.1x1, ldfldx1, ldftnx1, retx1, stfldx1, stloc.0x1
- calls: <>c__DisplayClass9_0..ctor, <>c__DisplayClass9_0.<Find>b__0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault

### `RootFor(class:220)` -> `string`
- token `0x060000a1`  flags=`134`  rva=`28213`
- IL size 18 bytes, 6 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x1, ldarg.1x1, ldfldx1, retx1
- calls: MPServerProvider.Core.Services.AppPaths.get_DataRoot, MPServerProvider.Core.Models.ServerInstance.ResolveRoot

### `NewServerDir(string)` -> `string`
- token `0x060000a2`  flags=`134`  rva=`28232`
- IL size 23 bytes, 7 instructions, max_stack=8
- opcodes: callx2, callvirtx1, ldarg.0x1, ldarg.1x1, ldfldx1, retx1
- calls: MPServerProvider.Core.Services.AppPaths.get_ServersDir, MPServerProvider.Core.Services.ServerRepository.Sanitize, System.IO.Path.Combine

### `NameTaken(string)` -> `bool`
- token `0x060000a3`  flags=`134`  rva=`28256`
- IL size 24 bytes, 11 instructions, max_stack=8
- opcodes: callx3, ldarg.0x2, ldarg.1x2, retx2, brtrue.sx1, ldc.i4.1x1
- calls: MPServerProvider.Core.Services.ServerRepository.Find, MPServerProvider.Core.Services.ServerRepository.NewServerDir, System.IO.Directory.Exists

### `Save(class:220)` -> `void`
- token `0x060000a4`  flags=`134`  rva=`28284`
- IL size 101 bytes, 37 instructions, max_stack=2
- opcodes: callvirtx7, ldarg.1x7, callx4, ldarg.0x4, brtrue.sx3, ldfldx3, ldloc.0x2, retx2, br.sx1, dupx1, ldstrx1, popx1, stloc.0x1
- string literals: "server.json"
- calls: MPServerProvider.Core.Models.ServerInstance.get_SourceFile, System.String.IsNullOrEmpty, MPServerProvider.Core.Models.ServerInstance.get_ServerName, MPServerProvider.Core.Services.ServerRepository.NewServerDir, System.IO.Path.Combine, MPServerProvider.Core.Models.ServerInstance.set_SourceFile, MPServerProvider.Core.Json.Json.WriteFile, System.Collections.Generic.List`1.Contains, System.Collections.Generic.List`1.Add, System.Action.Invoke

### `add_Deleted(class:193<class:220>)` -> `void`
- token `0x060000a5`  flags=`2182`  rva=`28400`
- IL size 41 bytes, 20 instructions, max_stack=3
- opcodes: ldloc.1x3, callx2, ldarg.0x2, ldloc.0x2, stloc.0x2, bne.un.sx1, castclassx1, ldarg.1x1, ldfldx1, ldfldax1, ldloc.2x1, retx1, stloc.1x1, stloc.2x1
- calls: System.Delegate.Combine, System.Threading.Interlocked.CompareExchange

### `remove_Deleted(class:193<class:220>)` -> `void`
- token `0x060000a6`  flags=`2182`  rva=`28456`
- IL size 41 bytes, 20 instructions, max_stack=3
- opcodes: ldloc.1x3, callx2, ldarg.0x2, ldloc.0x2, stloc.0x2, bne.un.sx1, castclassx1, ldarg.1x1, ldfldx1, ldfldax1, ldloc.2x1, retx1, stloc.1x1, stloc.2x1
- calls: System.Delegate.Remove, System.Threading.Interlocked.CompareExchange

### `MigrateCfxToTxAdmin(class:220)` -> `void`
- token `0x060000a7`  flags=`129`  rva=`28512`
- IL size 107 bytes, 38 instructions, max_stack=4
- opcodes: callvirtx8, ldarg.1x6, ldstrx5, brtrue.sx3, callx3, retx3, ldloc.0x2, stloc.1x2, br.sx1, brfalse.sx1, ldc.i4.0x1, ldc.i4.1x1, ldloc.1x1, stloc.0x1
- string literals: "fivem", "redm", "+exec server.cfg", "+exec server.cfg", ""
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerGame, System.String.op_Equality, MPServerProvider.Core.Models.ServerInstance.get_StartCommand, System.String.Contains, System.String.Replace, System.String.TrimEnd, MPServerProvider.Core.Models.ServerInstance.set_StartCommand, MPServerProvider.Core.Models.ServerInstance.get_SourceFile, MPServerProvider.Core.Json.Json.WriteFile

### `Delete(class:220, bool)` -> `void`
- token `0x060000a8`  flags=`134`  rva=`28632`
- IL size 94 bytes, 38 instructions, max_stack=2
- opcodes: callvirtx5, ldarg.1x5, callx4, ldarg.0x4, ldfldx3, popx3, br.sx2, brfalse.sx2, brtrue.sx2, dupx2, leave.sx2, endfinallyx1, ldarg.2x1, nopx1, retx1
- calls: System.Collections.Generic.List`1.Remove, System.Action`1.Invoke, MPServerProvider.Core.Services.ServerRepository.RootFor, MPServerProvider.Core.Services.ServerRepository.ForceDeleteDirectory, MPServerProvider.Core.Models.ServerInstance.get_SourceFile, System.IO.File.Exists, System.IO.File.Delete, System.Action.Invoke

### `ForceDeleteDirectory(string)` -> `void`
- token `0x060000a9`  flags=`150`  rva=`28756`
- IL size 88 bytes, 46 instructions, max_stack=2
- opcodes: callx6, ldarg.0x5, ldc.i4.1x4, ldloc.0x4, popx3, br.sx2, brtrue.sx2, ldc.i4.0x2, ldc.i4.4x2, leave.sx2, retx2, stloc.0x2, addx1, ble.sx1, cgt.unx1, cltx1, dupx1, endfilterx1, isinstx1, ldc.i4x1, mulx1, nopx1
- calls: System.IO.Directory.Exists, MPServerProvider.Core.Services.ServerRepository.ClearReadOnlyAttributes, System.IO.Directory.Delete, System.Threading.Thread.Sleep

### `ClearReadOnlyAttributes(string)` -> `void`
- token `0x060000aa`  flags=`145`  rva=`28872`
- IL size 153 bytes, 60 instructions, max_stack=3
- opcodes: callvirtx12, leave.sx5, boxx4, ldloc.0x4, ldloc.1x4, brfalse.sx3, ldc.i4.1x3, andx2, brtrue.sx2, callx2, dupx2, ldc.i4.sx2, ldloc.2x2, popx2, retx2, br.sx1, endfinallyx1, ldarg.0x1, ldstrx1, newobjx1, nopx1, stloc.0x1, stloc.1x1, stloc.2x1
- string literals: "*"
- calls: System.IO.DirectoryInfo..ctor, System.IO.FileSystemInfo.get_Exists, System.IO.DirectoryInfo.EnumerateFileSystemInfos, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.IO.FileSystemInfo.get_Attributes, System.Enum.HasFlag, System.IO.FileSystemInfo.set_Attributes, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose

### `Sanitize(string)` -> `string`
- token `0x060000ab`  flags=`150`  rva=`29080`
- IL size 67 bytes, 21 instructions, max_stack=3
- opcodes: callx4, newobjx3, ldloc.0x2, ldloc.1x2, retx2, brtrue.sx1, callvirtx1, ldarg.0x1, ldftnx1, ldstrx1, stfldx1, stloc.0x1, stloc.1x1
- string literals: "server"
- calls: <>c__DisplayClass22_0..ctor, System.IO.Path.GetInvalidFileNameChars, System.String.Trim, <>c__DisplayClass22_0.<Sanitize>b__0, System.Func`2..ctor, System.Linq.Enumerable.Select, System.Linq.Enumerable.ToArray, System.String..ctor, System.String.IsNullOrWhiteSpace

### `RollMonthCounter(class:220)` -> `void`
- token `0x060000ac`  flags=`129`  rva=`29160`
- IL size 64 bytes, 22 instructions, max_stack=2
- opcodes: callvirtx6, callx3, ldarg.1x3, ldloc.0x2, brfalse.sx1, conv.i8x1, ldc.i4.0x1, ldloca.sx1, ldstrx1, retx1, stloc.0x1, stloc.1x1
- string literals: "yyyy-MM"
- calls: System.DateTimeOffset.get_Now, System.DateTimeOffset.ToString, MPServerProvider.Core.Models.ServerInstance.get_Stats, MPServerProvider.Core.Models.ServerStats.get_MonthKey, System.String.op_Inequality, MPServerProvider.Core.Models.ServerStats.set_MonthKey, MPServerProvider.Core.Models.ServerStats.set_MonthUptimeSeconds

