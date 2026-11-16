# <DownloadItemsAsync>d__8

Token: `0x0200007f`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400027e)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x0400027f)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000280)
- `game` : `class:160`  (flags=6, token=0x04000281)
- `<>4__this` : `class:112`  (flags=6, token=0x04000282)
- `ct` : `valuetype:121`  (flags=6, token=0x04000283)
- `<>8__1` : `class:500`  (flags=1, token=0x04000284)
- `<>8__2` : `class:504`  (flags=1, token=0x04000285)
- `<appId>5__2` : `i4`  (flags=1, token=0x04000286)
- `<ids>5__3` : `class:153<i8>`  (flags=1, token=0x04000287)
- `<steamCmd>5__4` : `string`  (flags=1, token=0x04000288)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000289)
- `<>7__wrap4` : `valuetype:205<i8>`  (flags=1, token=0x0400028a)
- `<cacheDir>5__6` : `string`  (flags=1, token=0x0400028b)
- `<process>5__7` : `class:169`  (flags=1, token=0x0400028c)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x0400028d)

## Methods

### `MoveNext()` -> `void`
- token `0x0600032e`  flags=`481`  rva=`69416`
- IL size 1389 bytes, 458 instructions, max_stack=4
- opcodes: ldarg.0x78, callx54, ldfldx47, callvirtx28, stfldx28, ldloca.sx24, ldloc.sx20, dupx16, ldfldax15, ldstrx14, stloc.sx13, ldc.i4.1x12, ldnullx9, newobjx8, ldloc.0x7, leave.sx7, brfalse.sx6, ldc.i4.0x6, stelem.refx6, brtrue.sx5, ldc.i4.sx5, stloc.0x5, bge.sx3, endfinallyx3, initobjx3, leavex3, nopx3, beqx2, brx2, br.sx2, brfalsex2, ldc.i4.3x2, ldc.i4.m1x2, ldloc.1x2, beq.sx1, brtruex1, constrained.x1, ldc.i4.2x1, ldc.i4.4x1, ldc.i4.5x1, ldc.i4.6x1, ldftnx1, ldloc.3x1, newarrx1, popx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, throwx1
- string literals: "Game '", "' has no steamcmd app_id.", "Workshop item ", " already cached.", "SteamCMD: downloading workshop item ", "…", "+force_install_dir "", "" +login anonymous +workshop_download_item ", " ", " +quit", "steamapps", "workshop", "content", "steamapps"
- calls: <>c__DisplayClass8_0..ctor, MPServerProvider.Core.Models.GameDefinition.get_Install, MPServerProvider.Core.Models.InstallSpec.get_AppId, System.Nullable`1.get_HasValue, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.Concat, System.InvalidOperationException..ctor, System.Nullable`1.GetValueOrDefault, MPServerProvider.Core.Models.InstallSpec.get_WorkshopIds, System.Collections.Generic.List`1.get_Count, MPServerProvider.Core.Services.WorkshopService.EnsureSteamCmdAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Collections.Generic.List`1.GetEnumerator, <>c__DisplayClass8_1..ctor, Enumerator.get_Current, System.Threading.CancellationToken.ThrowIfCancellationRequested, MPServerProvider.Core.Services.AppPaths.get_WorkshopDir, System.Int32.ToString, System.Int64.ToString, System.IO.Path.Combine, System.IO.Directory.Exists, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, System.Diagnostics.ProcessStartInfo..ctor, System.Diagnostics.ProcessStartInfo.set_FileName, System.Diagnostics.ProcessStartInfo.set_Arguments, System.Diagnostics.ProcessStartInfo.set_UseShellExecute, System.Diagnostics.ProcessStartInfo.set_CreateNoWindow, System.Diagnostics.ProcessStartInfo.set_RedirectStandardOutput, System.Diagnostics.ProcessStartInfo.set_RedirectStandardError, System.Diagnostics.Process.Start, <>c__DisplayClass8_1.<DownloadItemsAsync>b__0, System.Diagnostics.DataReceivedEventHandler..ctor...

### `SetStateMachine(class:129)` -> `void`
- token `0x0600032f`  flags=`481`  rva=`70944`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

