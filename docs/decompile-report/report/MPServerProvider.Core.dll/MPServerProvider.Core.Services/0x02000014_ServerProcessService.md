# MPServerProvider.Core.Services.ServerProcessService

Token: `0x02000014`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_running` : `class:361<string,class:400>`  (flags=33, token=0x04000030)
- `_repository` : `class:84`  (flags=33, token=0x04000031)
- `_games` : `class:52`  (flags=33, token=0x04000032)
- `_java` : `class:132`  (flags=33, token=0x04000033)
- `_rcon` : `class:72`  (flags=33, token=0x04000034)
- `_rconPassword` : `string`  (flags=1, token=0x04000035)
- `StateChanged` : `class:357<class:220,valuetype:232>`  (flags=1, token=0x04000036)
- `OutputReceived` : `class:357<class:220,string>`  (flags=1, token=0x04000037)

## Methods

### `add_StateChanged(class:357<class:220,valuetype:232>)` -> `void`
- token `0x0600007d`  flags=`2182`  rva=`24968`
- IL size 41 bytes, 20 instructions, max_stack=3
- opcodes: ldloc.1x3, callx2, ldarg.0x2, ldloc.0x2, stloc.0x2, bne.un.sx1, castclassx1, ldarg.1x1, ldfldx1, ldfldax1, ldloc.2x1, retx1, stloc.1x1, stloc.2x1
- calls: System.Delegate.Combine, System.Threading.Interlocked.CompareExchange

### `remove_StateChanged(class:357<class:220,valuetype:232>)` -> `void`
- token `0x0600007e`  flags=`2182`  rva=`25024`
- IL size 41 bytes, 20 instructions, max_stack=3
- opcodes: ldloc.1x3, callx2, ldarg.0x2, ldloc.0x2, stloc.0x2, bne.un.sx1, castclassx1, ldarg.1x1, ldfldx1, ldfldax1, ldloc.2x1, retx1, stloc.1x1, stloc.2x1
- calls: System.Delegate.Remove, System.Threading.Interlocked.CompareExchange

### `add_OutputReceived(class:357<class:220,string>)` -> `void`
- token `0x0600007f`  flags=`2182`  rva=`25080`
- IL size 41 bytes, 20 instructions, max_stack=3
- opcodes: ldloc.1x3, callx2, ldarg.0x2, ldloc.0x2, stloc.0x2, bne.un.sx1, castclassx1, ldarg.1x1, ldfldx1, ldfldax1, ldloc.2x1, retx1, stloc.1x1, stloc.2x1
- calls: System.Delegate.Combine, System.Threading.Interlocked.CompareExchange

### `remove_OutputReceived(class:357<class:220,string>)` -> `void`
- token `0x06000080`  flags=`2182`  rva=`25136`
- IL size 41 bytes, 20 instructions, max_stack=3
- opcodes: ldloc.1x3, callx2, ldarg.0x2, ldloc.0x2, stloc.0x2, bne.un.sx1, castclassx1, ldarg.1x1, ldfldx1, ldfldax1, ldloc.2x1, retx1, stloc.1x1, stloc.2x1
- calls: System.Delegate.Remove, System.Threading.Interlocked.CompareExchange

### `.ctor(class:84, class:52, class:132)` -> `void`
- token `0x06000081`  flags=`6278`  rva=`25192`
- IL size 73 bytes, 24 instructions, max_stack=3
- opcodes: ldarg.0x7, stfldx5, newobjx3, callx2, ldarg.1x2, callvirtx1, ldarg.2x1, ldarg.3x1, ldftnx1, retx1
- calls: System.StringComparer.get_OrdinalIgnoreCase, System.Collections.Concurrent.ConcurrentDictionary`2..ctor, MPServerProvider.Core.Services.RconService..ctor, System.Object..ctor, MPServerProvider.Core.Services.ServerProcessService.<.ctor>b__12_0, System.Action`1..ctor, MPServerProvider.Core.Services.ServerRepository.add_Deleted

### `StateOf(class:220)` -> `valuetype:232`
- token `0x06000082`  flags=`134`  rva=`25280`
- IL size 30 bytes, 12 instructions, max_stack=3
- opcodes: callvirtx3, retx2, brtrue.sx1, ldarg.0x1, ldarg.1x1, ldc.i4.0x1, ldfldx1, ldloc.0x1, ldloca.sx1
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.Collections.Concurrent.ConcurrentDictionary`2.TryGetValue, RunningServer.get_State

### `UptimeOf(class:220)` -> `valuetype:89`
- token `0x06000083`  flags=`134`  rva=`25324`
- IL size 68 bytes, 23 instructions, max_stack=3
- opcodes: callx4, callvirtx4, ldloca.sx3, ldloc.0x2, retx2, stloc.1x2, brfalse.sx1, brtrue.sx1, ldarg.0x1, ldarg.1x1, ldfldx1, ldsfldx1
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.Collections.Concurrent.ConcurrentDictionary`2.TryGetValue, RunningServer.get_StartedAt, System.Nullable`1.get_HasValue, System.DateTimeOffset.get_Now, System.Nullable`1.get_Value, System.DateTimeOffset.op_Subtraction

### `ConsoleOf(class:220)` -> `class:269<string>`
- token `0x06000084`  flags=`134`  rva=`25404`
- IL size 36 bytes, 14 instructions, max_stack=3
- opcodes: callvirtx3, retx2, brtrue.sx1, callx1, ldarg.0x1, ldarg.1x1, ldfldx1, ldloc.0x1, ldloc.1x1, ldloca.sx1, stloc.1x1
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.Collections.Concurrent.ConcurrentDictionary`2.TryGetValue, System.Array.Empty, RunningServer.ConsoleSnapshot

### `get_RunningCount()` -> `i4`
- token `0x06000085`  flags=`2182`  rva=`25452`
- IL size 48 bytes, 14 instructions, max_stack=8
- opcodes: dupx2, ldsfldx2, brtrue.sx1, callx1, callvirtx1, ldarg.0x1, ldfldx1, ldftnx1, newobjx1, popx1, retx1, stsfldx1
- calls: System.Collections.Concurrent.ConcurrentDictionary`2.get_Values, <>c.<get_RunningCount>b__17_0, System.Func`2..ctor, System.Linq.Enumerable.Count

### `ProcessOf(class:220)` -> `class:169`
- token `0x06000086`  flags=`134`  rva=`25504`
- IL size 30 bytes, 12 instructions, max_stack=3
- opcodes: callvirtx3, retx2, brtrue.sx1, ldarg.0x1, ldarg.1x1, ldfldx1, ldloc.0x1, ldloca.sx1, ldnullx1
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.Collections.Concurrent.ConcurrentDictionary`2.TryGetValue, RunningServer.get_Process

### `StartAsync(class:220)` -> `class:221`
- token `0x06000087`  flags=`134`  rva=`25548`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `ShouldDropElevation(class:160)` -> `bool`
- token `0x06000088`  flags=`145`  rva=`25623`
- IL size 23 bytes, 10 instructions, max_stack=8
- opcodes: ldarg.0x2, retx2, brtrue.sx1, callx1, callvirtx1, ldc.i4.0x1, ldc.i4.5x1, ldstrx1
- string literals: "minecraft"
- calls: MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.StartsWith

### `TryStartDeElevated(class:400, class:160, string, string, string)` -> `bool`
- token `0x06000089`  flags=`129`  rva=`25648`
- IL size 176 bytes, 64 instructions, max_stack=5
- opcodes: ldloc.0x8, callx6, callvirtx6, ldfldx4, ldarg.0x3, ldc.i4.0x3, ldloc.1x3, ldstrx3, newobjx3, retx3, brtrue.sx2, ldarg.sx2, ldftnx2, leave.sx2, stfldx2, stloc.2x2, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.1x1, ldloc.2x1, ldloc.3x1, nopx1, stloc.0x1, stloc.1x1, stloc.3x1
- string literals: "[MP Server Provider] Running as your normal user account (not administrator).", "[MP Server Provider] Could not drop administrator rights (", "); starting normally."
- calls: <>c__DisplayClass21_0..ctor, MPServerProvider.Core.Services.ServerProcessService.ShouldDropElevation, MPServerProvider.Core.Services.DeElevatedLauncher.IsCurrentProcessElevated, <>c__DisplayClass21_0.<TryStartDeElevated>b__0, System.Action`1..ctor, MPServerProvider.Core.Services.DeElevatedLauncher.Start, MPServerProvider.Core.Services.DeElevatedProcess.get_Process, <>c__DisplayClass21_0.<TryStartDeElevated>b__1, System.EventHandler..ctor, System.Diagnostics.Process.add_Exited, RunningServer.set_Process, RunningServer.set_DeElevated, MPServerProvider.Core.Services.ServerProcessService.AppendLine, System.Exception.get_Message, System.String.Concat

### `ConnectRconAsync(class:400, class:220, class:160)` -> `class:221`
- token `0x0600008a`  flags=`129`  rva=`25852`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `PollRconAsync(class:400)` -> `class:221`
- token `0x0600008b`  flags=`129`  rva=`25944`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `StartLogTail(class:400, class:220, class:160)` -> `void`
- token `0x0600008c`  flags=`129`  rva=`26020`
- IL size 218 bytes, 78 instructions, max_stack=6
- opcodes: callx9, ldloc.0x8, ldstrx8, dupx6, ldfldx5, ldarg.0x4, ldloc.1x4, stelem.refx4, newobjx3, stfldx3, brtrue.sx2, ldftnx2, popx2, retx2, callvirtx1, ldarg.1x1, ldarg.2x1, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.4x1, ldloc.2x1, ldnullx1, ldsfldx1, newarrx1, stloc.0x1, stloc.1x1, stloc.2x1, stsfldx1
- string literals: "console.log", "logs", "latest.log", "server.log", "log", "server.log", "[MP Server Provider] No log file found for logfile console mode.", "[MP Server Provider] Tailing log file: "
- calls: <>c__DisplayClass24_0..ctor, MPServerProvider.Core.Services.ServerRepository.RootFor, System.IO.Path.Combine, System.IO.File.Exists, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault, MPServerProvider.Core.Services.ServerProcessService.AppendLine, System.String.Concat, <>c__DisplayClass24_0.<StartLogTail>b__0, System.Func`1..ctor, System.Threading.Tasks.Task.Run

### `SetRconPassword(string)` -> `void`
- token `0x0600008d`  flags=`134`  rva=`26250`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `StopAsync(class:220, valuetype:261<valuetype:89>)` -> `class:221`
- token `0x0600008e`  flags=`134`  rva=`26260`
- IL size 71 bytes, 23 instructions, max_stack=2
- opcodes: ldloca.sx8, stfldx5, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `RestartAsync(class:220)` -> `class:221`
- token `0x0600008f`  flags=`134`  rva=`26344`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `SendCommand(class:220, string)` -> `void`
- token `0x06000090`  flags=`134`  rva=`26420`
- IL size 117 bytes, 45 instructions, max_stack=4
- opcodes: callvirtx9, brfalse.sx4, ldarg.0x4, ldarg.2x3, ldfldx3, ldloc.0x3, ldloc.1x3, callx2, ldloc.3x2, ldloca.sx2, retx2, brtrue.sx1, initobjx1, ldarg.1x1, ldloc.2x1, ldstrx1, popx1, stloc.1x1, stloc.3x1
- string literals: "> "
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.Collections.Concurrent.ConcurrentDictionary`2.TryGetValue, System.String.Concat, MPServerProvider.Core.Services.ServerProcessService.AppendLine, MPServerProvider.Core.Services.RconService.get_Connected, MPServerProvider.Core.Services.RconService.SendCommandAsync, RunningServer.get_Process, System.Diagnostics.Process.get_HasExited, RunningServer.get_StdIn, System.IO.TextWriter.WriteLine, System.IO.TextWriter.Flush

### `StopAllAsync()` -> `class:221`
- token `0x06000091`  flags=`134`  rva=`26552`
- IL size 55 bytes, 17 instructions, max_stack=2
- opcodes: ldloca.sx6, callx3, stfldx3, ldfldax2, ldarg.0x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `BuildCommandAsync(class:220)` -> `class:117<valuetype:365<string,string,string>>`
- token `0x06000092`  flags=`129`  rva=`26620`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `SplitCommandLine(string)` -> `valuetype:305<string,string>`
- token `0x06000093`  flags=`150`  rva=`26696`
- IL size 115 bytes, 52 instructions, max_stack=4
- opcodes: callvirtx10, ldarg.0x9, ldc.i4.1x5, ldc.i4.0x3, ldc.i4.sx3, ldloc.0x3, ldloc.1x3, newobjx3, retx3, addx2, ble.sx1, blt.sx1, brfalse.sx1, ldstrx1, starg.sx1, stloc.0x1, stloc.1x1, subx1
- string literals: ""
- calls: System.String.Trim, System.String.StartsWith, System.String.IndexOf, System.String.Substring, System.String.TrimStart, System.ValueTuple`2..ctor

### `OnExited(class:400)` -> `void`
- token `0x06000094`  flags=`129`  rva=`26824`
- IL size 457 bytes, 158 instructions, max_stack=5
- opcodes: callx21, ldloc.0x21, ldfldx18, callvirtx16, ldloca.sx11, ldarg.0x8, ldstrx6, brtrue.sx5, popx5, dupx4, br.sx3, brfalse.sx3, ldc.i4.0x3, ldc.i4.1x3, ldloc.2x3, retx3, ldc.i4.sx2, ldnullx2, leave.sx2, newobjx2, stfldx2, stloc.2x2, addx1, ceqx1, initobjx1, ldarg.1x1, ldc.i4.3x1, ldc.i4.4x1, ldftnx1, ldloc.1x1, ldloc.3x1, ldloc.sx1, stloc.0x1, stloc.1x1, stloc.sx1
- string literals: "[MP Server Provider] Server stopped (exit code ", ").", "[MP Server Provider] Server exited unexpectedly (code ", ").", "[MP Server Provider] auto_restart_on_crash is on — restarting in 5 s…", "[MP Server Provider] Crashed repeatedly — auto-restart suspended."
- calls: <>c__DisplayClass32_0..ctor, RunningServer.get_State, RunningServer.get_Process, System.Diagnostics.Process.get_ExitCode, MPServerProvider.Core.Services.ServerProcessService.RecordUptime, System.ComponentModel.Component.Dispose, RunningServer.set_Process, RunningServer.get_DeElevated, MPServerProvider.Core.Services.DeElevatedProcess.Dispose, RunningServer.set_DeElevated, RunningServer.set_StartedAt, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Services.ServerProcessService.AppendLine, MPServerProvider.Core.Services.ServerProcessService.SetState, RunningServer.get_Server, MPServerProvider.Core.Models.ServerInstance.get_Stats, MPServerProvider.Core.Models.ServerStats.get_CrashCount, MPServerProvider.Core.Models.ServerStats.set_CrashCount, MPServerProvider.Core.Models.ServerInstance.get_AutoRestartOnCrash, RunningServer.RegisterCrashForRestart, <>c__DisplayClass32_0.<OnExited>b__0, System.Func`1..ctor, System.Threading.Tasks.Task.Run

### `RecordUptime(class:400)` -> `void`
- token `0x06000095`  flags=`129`  rva=`27312`
- IL size 122 bytes, 44 instructions, max_stack=3
- opcodes: callvirtx12, callx5, ldarg.1x5, ldloca.sx3, addx2, dupx2, ldloc.0x2, leave.sx2, retx2, stloc.1x2, brtrue.sx1, conv.i8x1, ldarg.0x1, ldfldx1, popx1, stloc.0x1, stloc.2x1
- calls: RunningServer.get_StartedAt, System.Nullable`1.get_HasValue, System.DateTimeOffset.get_Now, System.Nullable`1.get_Value, System.DateTimeOffset.op_Subtraction, System.TimeSpan.get_TotalSeconds, RunningServer.get_Server, MPServerProvider.Core.Models.ServerInstance.get_Stats, MPServerProvider.Core.Models.ServerStats.get_TotalUptimeSeconds, MPServerProvider.Core.Models.ServerStats.set_TotalUptimeSeconds, MPServerProvider.Core.Models.ServerStats.get_MonthUptimeSeconds, MPServerProvider.Core.Models.ServerStats.set_MonthUptimeSeconds, MPServerProvider.Core.Services.ServerRepository.Save

### `SetState(class:400, valuetype:232)` -> `void`
- token `0x06000096`  flags=`129`  rva=`27464`
- IL size 31 bytes, 14 instructions, max_stack=8
- opcodes: callvirtx3, ldarg.1x2, ldarg.2x2, retx2, brtrue.sx1, dupx1, ldarg.0x1, ldfldx1, popx1
- calls: RunningServer.set_State, RunningServer.get_Server, System.Action`2.Invoke

### `AppendLine(class:400, string)` -> `void`
- token `0x06000097`  flags=`129`  rva=`27496`
- IL size 31 bytes, 14 instructions, max_stack=8
- opcodes: callvirtx3, ldarg.1x2, ldarg.2x2, retx2, brtrue.sx1, dupx1, ldarg.0x1, ldfldx1, popx1
- calls: RunningServer.Append, RunningServer.get_Server, System.Action`2.Invoke

### `Dispose()` -> `void`
- token `0x06000098`  flags=`486`  rva=`27528`
- IL size 109 bytes, 43 instructions, max_stack=2
- opcodes: callvirtx9, ldloc.0x4, br.sx3, brtrue.sx3, ldarg.0x3, ldfldx3, leave.sx3, popx3, callx2, dupx2, ldloc.1x2, brfalse.sx1, endfinallyx1, ldc.i4.1x1, retx1, stloc.0x1, stloc.1x1
- calls: System.Collections.Concurrent.ConcurrentDictionary`2.get_Values, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, RunningServer.get_Process, System.Diagnostics.Process.Kill, System.ComponentModel.Component.Dispose, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, System.Collections.Concurrent.ConcurrentDictionary`2.Clear, MPServerProvider.Core.Services.RconService.Dispose

### `<.ctor>b__12_0(class:220)` -> `void`
- token `0x06000099`  flags=`129`  rva=`27680`
- IL size 61 bytes, 26 instructions, max_stack=3
- opcodes: callvirtx4, popx3, brtrue.sx2, callx2, dupx2, ldloc.0x2, leave.sx2, retx2, br.sx1, brfalse.sx1, ldarg.0x1, ldarg.1x1, ldc.i4.1x1, ldfldx1, ldloca.sx1
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.Collections.Concurrent.ConcurrentDictionary`2.TryRemove, RunningServer.get_Process, System.Diagnostics.Process.Kill, System.ComponentModel.Component.Dispose

### `<StopAllAsync>b__29_1(class:400)` -> `class:221`
- token `0x0600009a`  flags=`129`  rva=`27772`
- IL size 32 bytes, 8 instructions, max_stack=8
- opcodes: callx2, callvirtx1, ldarg.0x1, ldarg.1x1, ldc.r8x1, newobjx1, retx1
- calls: RunningServer.get_Server, System.TimeSpan.FromSeconds, System.Nullable`1..ctor, MPServerProvider.Core.Services.ServerProcessService.StopAsync

