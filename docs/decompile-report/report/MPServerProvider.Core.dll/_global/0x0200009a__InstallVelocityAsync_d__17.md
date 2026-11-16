# <InstallVelocityAsync>d__17

Token: `0x0200009a`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400034b)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x0400034c)
- `progress` : `class:233<class:200>`  (flags=6, token=0x0400034d)
- `<>4__this` : `class:140`  (flags=6, token=0x0400034e)
- `request` : `class:208`  (flags=6, token=0x0400034f)
- `ct` : `valuetype:121`  (flags=6, token=0x04000350)
- `<build>5__2` : `class:341`  (flags=1, token=0x04000351)
- `<name>5__3` : `string`  (flags=1, token=0x04000352)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000353)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x04000354)

## Methods

### `MoveNext()` -> `void`
- token `0x06000370`  flags=`481`  rva=`87972`
- IL size 645 bytes, 220 instructions, max_stack=8
- opcodes: ldarg.0x39, callx22, ldfldx19, ldloca.sx18, callvirtx14, stfldx14, stloc.sx13, ldstrx10, dupx7, ldloc.sx7, ldfldax6, ldnullx6, stloc.0x5, brfalse.sx4, ldloc.0x4, initobjx3, ldc.i4.1x3, ldc.i4.sx3, beqx2, brtrue.sx2, ldc.i4.0x2, ldc.i4.m1x2, ldloc.1x2, leavex2, leave.sx2, bge.sx1, endfinallyx1, ldloc.2x1, newobjx1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "Resolving latest Velocity build…", "https://fill.papermc.io/v3/projects/velocity/versions/", "/builds/latest", "downloads", "server:default", "name", "url", "checksums", "sha256", "Downloading Velocity "
- calls: MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Models.InstallRequest.get_Version, System.String.Concat, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.GetString, System.Text.Json.JsonElement.TryGetProperty, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version, MPServerProvider.Core.Models.InstallResult.set_JavaMajor, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000371`  flags=`481`  rva=`88684`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

