# <InstallAsync>d__6

Token: `0x0200008a`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040002c8)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x040002c9)
- `progress` : `class:233<class:200>`  (flags=6, token=0x040002ca)
- `<>4__this` : `class:124`  (flags=6, token=0x040002cb)
- `ct` : `valuetype:121`  (flags=6, token=0x040002cc)
- `request` : `class:208`  (flags=6, token=0x040002cd)
- `<doc>5__2` : `class:341`  (flags=1, token=0x040002ce)
- `<tag>5__3` : `string`  (flags=1, token=0x040002cf)
- `<exe>5__4` : `string`  (flags=1, token=0x040002d0)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x040002d1)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x040002d2)

## Methods

### `MoveNext()` -> `void`
- token `0x06000345`  flags=`481`  rva=`74860`
- IL size 726 bytes, 241 instructions, max_stack=8
- opcodes: ldarg.0x45, callx25, ldfldx22, stfldx17, ldloca.sx14, callvirtx13, ldstrx12, stloc.sx11, dupx8, ldnullx8, ldfldax6, ldloc.sx5, stloc.0x5, brtrue.sx4, ldloc.0x4, initobjx3, ldc.i4.1x3, newobjx3, popx3, beqx2, brfalse.sx2, ldc.i4.0x2, ldc.i4.m1x2, ldc.i4.sx2, ldloc.1x2, ldsfldx2, leavex2, leave.sx2, bge.sx1, boxx1, endfinallyx1, ldftnx1, ldloc.2x1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, stsfldx1, throwx1
- string literals: "Resolving BeamMP-Server release…", "https://api.github.com/repos/BeamMP/BeamMP-Server/releases/latest", "tag_name", "assets", "No Windows executable found in the latest BeamMP-Server release.", "BeamMP-Server.exe", "browser_download_url", "Downloading BeamMP-Server ", "Resources", "Client", "Resources", "Server"
- calls: MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.GetString, System.Text.Json.JsonElement.EnumerateArray, <>c.<InstallAsync>b__6_0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault, System.Text.Json.JsonElement.get_ValueKind, System.InvalidOperationException..ctor, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, System.String.Concat, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.IO.Directory.CreateDirectory, MPServerProvider.Core.Providers.BeamMpProvider.WriteConfig, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000346`  flags=`481`  rva=`75652`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

