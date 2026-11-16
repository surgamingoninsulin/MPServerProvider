# <InstallPurpurAsync>d__16

Token: `0x02000098`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000333)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x04000334)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000335)
- `<>4__this` : `class:140`  (flags=6, token=0x04000336)
- `request` : `class:208`  (flags=6, token=0x04000337)
- `ct` : `valuetype:121`  (flags=6, token=0x04000338)
- `<versionDoc>5__2` : `class:341`  (flags=1, token=0x04000339)
- `<latestBuild>5__3` : `string`  (flags=1, token=0x0400033a)
- `<jar>5__4` : `string`  (flags=1, token=0x0400033b)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x0400033c)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x0400033d)

## Methods

### `MoveNext()` -> `void`
- token `0x0600036c`  flags=`481`  rva=`86028`
- IL size 722 bytes, 239 instructions, max_stack=8
- opcodes: ldarg.0x47, callx25, ldfldx24, ldloca.sx17, stfldx17, callvirtx16, ldstrx13, stloc.sx9, ldnullx8, dupx7, ldfldax6, stloc.0x5, ldloc.0x4, ldloc.sx4, initobjx3, ldc.i4.1x3, ldc.i4.sx3, beqx2, brfalse.sx2, brtrue.sx2, ldc.i4.0x2, ldc.i4.m1x2, ldloc.1x2, leavex2, leave.sx2, newobjx2, bge.sx1, endfinallyx1, ldc.i4.3x1, ldloc.2x1, ldloc.3x1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "Resolving latest Purpur build…", "https://api.purpurmc.org/v2/purpur/", "builds", "latest", "purpur_server.jar", "https://api.purpurmc.org/v2/purpur", "/", "/", "/download", "Downloading Purpur ", " build ", " (build ", ")"
- calls: MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Models.InstallRequest.get_Version, System.String.Concat, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.GetString, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version, MPServerProvider.Core.Providers.MinecraftJavaProvider.GuessJavaMajor, MPServerProvider.Core.Models.InstallResult.set_JavaMajor, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600036d`  flags=`481`  rva=`86816`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

