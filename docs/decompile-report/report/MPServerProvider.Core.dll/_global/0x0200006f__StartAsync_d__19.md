# <StartAsync>d__19

Token: `0x0200006f`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400022a)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x0400022b)
- `server` : `class:220`  (flags=6, token=0x0400022c)
- `<>4__this` : `class:80`  (flags=6, token=0x0400022d)
- `<>8__1` : `class:412`  (flags=1, token=0x0400022e)
- `<game>5__2` : `class:160`  (flags=1, token=0x0400022f)
- `<consoleMethod>5__3` : `string`  (flags=1, token=0x04000230)
- `<>u__1` : `valuetype:137<valuetype:365<string,string,string>>`  (flags=1, token=0x04000231)

## Methods

### `MoveNext()` -> `void`
- token `0x06000302`  flags=`481`  rva=`62644`
- IL size 1819 bytes, 571 instructions, max_stack=6
- opcodes: ldfldx96, callvirtx77, ldarg.0x74, callx44, ldloc.sx43, dupx41, newobjx20, ldc.i4.1x16, ldloc.1x16, stloc.sx16, ldloca.sx15, stfldx15, ldstrx12, ldc.i4.0x8, ldftnx8, ldnullx7, popx7, brtrue.sx5, ldc.i4.2x5, br.sx4, brfalsex4, ldc.i4.sx4, ldfldax4, leave.sx3, stloc.0x3, brx2, ceqx2, ldloc.0x2, leavex2, stloc.3x2, bgt.un.sx1, brfalse.sx1, brtruex1, initobjx1, ldc.i4.4x1, ldc.i4.m1x1, ldloc.2x1, ldloc.3x1, nopx1, retx1, rethrowx1, stloc.1x1, stloc.2x1, subx1
- string literals: "stdin", "logfile", "[MP Server Provider] Started: ", " ", " (logfile mode)", "rcon", "[MP Server Provider] Started: ", " ", " (RCON mode)", "[MP Server Provider] Started: ", " ", "[MP Server Provider] Failed to start: "
- calls: <>c__DisplayClass19_0..ctor, MPServerProvider.Core.Services.ServerProcessService.StateOf, MPServerProvider.Core.Models.ServerInstance.get_ServerName, <>c__DisplayClass19_0.<StartAsync>b__0, System.Func`2..ctor, System.Collections.Concurrent.ConcurrentDictionary`2.GetOrAdd, RunningServer.set_Server, RunningServer.ClearConsole, MPServerProvider.Core.Services.ServerProcessService.SetState, MPServerProvider.Core.Models.ServerInstance.get_ServerGame, MPServerProvider.Core.Services.GameLibraryService.Find, MPServerProvider.Core.Models.GameDefinition.get_Network, MPServerProvider.Core.Models.NetworkSpec.get_ConsoleMethod, MPServerProvider.Core.Services.ServerProcessService.BuildCommandAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.Core.Models.GameDefinition.get_Runtime, MPServerProvider.Core.Models.RuntimeSpec.get_StopCommand, MPServerProvider.Core.Models.ServerInstance.get_StopCommand, System.String.op_Equality, System.Diagnostics.ProcessStartInfo..ctor, System.Diagnostics.ProcessStartInfo.set_FileName, System.Diagnostics.ProcessStartInfo.set_Arguments, System.Diagnostics.ProcessStartInfo.set_WorkingDirectory, System.Diagnostics.ProcessStartInfo.set_UseShellExecute, System.Diagnostics.ProcessStartInfo.set_CreateNoWindow, System.Diagnostics.ProcessStartInfo.set_RedirectStandardInput, System.Diagnostics.ProcessStartInfo.set_RedirectStandardOutput, System.Diagnostics.ProcessStartInfo.set_RedirectStandardError, System.Diagnostics.Process..ctor, System.Diagnostics.Process.set_StartInfo, System.Diagnostics.Process.set_EnableRaisingEvents, <>c__DisplayClass19_0.<StartAsync>b__1, System.EventHandler..ctor, System.Diagnostics.Process.add_Exited, System.Diagnostics.Process.Start, RunningServer.set_Process, System.DateTimeOffset.get_Now...

### `SetStateMachine(class:129)` -> `void`
- token `0x06000303`  flags=`481`  rva=`64528`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

