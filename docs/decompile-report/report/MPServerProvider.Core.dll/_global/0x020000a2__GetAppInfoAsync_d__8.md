# <GetAppInfoAsync>d__8

Token: `0x020000a2`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400036c)
- `<>t__builder` : `valuetype:145<class:144>`  (flags=6, token=0x0400036d)
- `<>4__this` : `class:152`  (flags=6, token=0x0400036e)
- `progress` : `class:233<class:200>`  (flags=6, token=0x0400036f)
- `ct` : `valuetype:121`  (flags=6, token=0x04000370)
- `appId` : `i4`  (flags=6, token=0x04000371)
- `<process>5__2` : `class:169`  (flags=1, token=0x04000372)
- `<stdout>5__3` : `string`  (flags=1, token=0x04000373)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000374)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x04000375)

## Methods

### `MoveNext()` -> `void`
- token `0x06000399`  flags=`481`  rva=`90436`
- IL size 774 bytes, 282 instructions, max_stack=4
- opcodes: ldarg.0x43, callx30, ldloca.sx21, ldfldx19, callvirtx17, dupx14, stfldx13, stloc.sx13, ldc.i4.1x10, ldloc.sx9, ldfldax8, brtrue.sx7, stloc.0x7, ldloc.0x6, ldstrx6, newobjx5, popx5, ldc.i4.2x4, ldc.i4.sx4, leave.sx4, brfalse.sx3, initobjx3, ldc.i4.0x3, ldc.i4.m1x3, ldnullx3, leavex3, stloc.2x3, br.sx2, nopx2, beqx1, beq.sx1, bge.sx1, ble.unx1, endfinallyx1, ldloc.1x1, ldloc.2x1, ldloc.3x1, retx1, stloc.1x1, stloc.3x1, subx1
- string literals: "SteamCMD: reading app info for ", "…", "+login anonymous +app_info_update 1 +app_info_print ", " +quit", """, """
- calls: System.Progress`1..ctor, MPServerProvider.Core.Providers.SteamCmdProvider.EnsureSteamCmdAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, System.Diagnostics.ProcessStartInfo..ctor, System.Diagnostics.ProcessStartInfo.set_FileName, System.Diagnostics.ProcessStartInfo.set_Arguments, System.Diagnostics.ProcessStartInfo.set_UseShellExecute, System.Diagnostics.ProcessStartInfo.set_CreateNoWindow, System.Diagnostics.ProcessStartInfo.set_RedirectStandardOutput, System.Diagnostics.ProcessStartInfo.set_RedirectStandardError, System.Diagnostics.Process.Start, System.Diagnostics.Process.get_StandardOutput, System.IO.TextReader.ReadToEndAsync, System.Diagnostics.Process.WaitForExitAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.String.Contains, MPServerProvider.Core.Providers.Vdf.Parse, MPServerProvider.Core.Providers.Vdf.ToAppInfo, MPServerProvider.Core.Providers.SteamAppInfo.get_Name, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Providers.SteamAppInfo.get_Executable, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600039a`  flags=`481`  rva=`91300`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

