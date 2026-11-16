# <InstallPaperAsync>d__15

Token: `0x02000097`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000328)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x04000329)
- `progress` : `class:233<class:200>`  (flags=6, token=0x0400032a)
- `<>4__this` : `class:140`  (flags=6, token=0x0400032b)
- `request` : `class:208`  (flags=6, token=0x0400032c)
- `ct` : `valuetype:121`  (flags=6, token=0x0400032d)
- `<build>5__2` : `class:341`  (flags=1, token=0x0400032e)
- `<buildId>5__3` : `i4`  (flags=1, token=0x0400032f)
- `<name>5__4` : `string`  (flags=1, token=0x04000330)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000331)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x04000332)

## Methods

### `MoveNext()` -> `void`
- token `0x0600036a`  flags=`481`  rva=`84824`
- IL size 1073 bytes, 363 instructions, max_stack=8
- opcodes: ldarg.0x53, callx44, ldloca.sx39, stloc.sx28, ldfldx27, callvirtx21, ldstrx19, stfldx18, ldloc.sx13, dupx9, ldfldax8, ldloc.0x7, stloc.0x7, ldnullx6, brfalse.sx5, initobjx5, ldc.i4.2x5, ldc.i4.sx5, leave.sx5, ldc.i4.1x4, brtrue.sx3, ldc.i4.0x3, ldc.i4.m1x3, ldloc.1x3, leavex3, newobjx3, beqx2, bge.sx2, endfinallyx2, nopx2, beq.sx1, ble.unx1, ldloc.2x1, popx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, subx1
- string literals: "Resolving latest Paper build…", "https://fill.papermc.io/v3/projects/paper/versions/", "/builds/latest", "id", "downloads", "server:default", "name", "url", "checksums", "sha256", "Downloading Paper ", " build ", "https://fill.papermc.io/v3/projects/paper/versions/", "version", "java", "version", "minimum", " (build ", ")"
- calls: MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Models.InstallRequest.get_Version, System.String.Concat, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.GetInt32, System.Text.Json.JsonElement.GetString, System.Text.Json.JsonElement.TryGetProperty, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.IDisposable.Dispose, MPServerProvider.Core.Providers.MinecraftJavaProvider.GuessJavaMajor, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version, MPServerProvider.Core.Models.InstallResult.set_JavaMajor, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600036b`  flags=`481`  rva=`86012`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

