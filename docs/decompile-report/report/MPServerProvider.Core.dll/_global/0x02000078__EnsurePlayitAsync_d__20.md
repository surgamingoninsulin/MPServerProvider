# <EnsurePlayitAsync>d__20

Token: `0x02000078`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000254)
- `<>t__builder` : `valuetype:145<string>`  (flags=6, token=0x04000255)
- `<>4__this` : `class:108`  (flags=6, token=0x04000256)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000257)
- `ct` : `valuetype:121`  (flags=6, token=0x04000258)
- `<doc>5__2` : `class:341`  (flags=1, token=0x04000259)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x0400025a)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x0400025b)

## Methods

### `MoveNext()` -> `void`
- token `0x0600031d`  flags=`481`  rva=`66848`
- IL size 551 bytes, 192 instructions, max_stack=8
- opcodes: ldarg.0x30, callx20, ldfldx13, ldloca.sx12, stfldx11, stloc.sx9, callvirtx7, dupx7, ldfldax6, ldloc.1x6, ldstrx6, brtrue.sx5, ldloc.sx5, stloc.0x5, ldloc.0x4, ldnullx4, initobjx3, ldc.i4.1x3, leavex3, beqx2, brfalse.sx2, ldc.i4.0x2, ldc.i4.m1x2, ldc.i4.sx2, ldsfldx2, leave.sx2, newobjx2, popx2, stloc.2x2, bge.sx1, boxx1, br.sx1, brfalsex1, endfinallyx1, ldftnx1, ldloc.2x1, nopx1, retx1, stloc.1x1, stloc.3x1, stsfldx1, throwx1
- string literals: "Resolving playit agent release…", "https://api.github.com/repos/playit-cloud/playit-agent/releases/latest", "assets", "No Windows binary in the latest playit-agent release.", "browser_download_url", "Downloading playit agent (official)"
- calls: MPServerProvider.Core.Services.TunnelService.get_PlayitInstalled, MPServerProvider.Core.Services.TunnelService.get_PlayitExePath, MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.EnumerateArray, <>c.<EnsurePlayitAsync>b__20_0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault, System.Text.Json.JsonElement.get_ValueKind, System.InvalidOperationException..ctor, System.Text.Json.JsonElement.GetString, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600031e`  flags=`481`  rva=`67464`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

