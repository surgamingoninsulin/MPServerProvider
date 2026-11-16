# <InstallVanillaAsync>d__14

Token: `0x02000099`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400033e)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x0400033f)
- `request` : `class:208`  (flags=6, token=0x04000340)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000341)
- `<>4__this` : `class:140`  (flags=6, token=0x04000342)
- `ct` : `valuetype:121`  (flags=6, token=0x04000343)
- `<>8__1` : `class:576`  (flags=1, token=0x04000344)
- `<manifest>5__2` : `class:341`  (flags=1, token=0x04000345)
- `<version>5__3` : `class:341`  (flags=1, token=0x04000346)
- `<javaMajor>5__4` : `i4`  (flags=1, token=0x04000347)
- `<jar>5__5` : `string`  (flags=1, token=0x04000348)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000349)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x0400034a)

## Methods

### `MoveNext()` -> `void`
- token `0x0600036e`  flags=`481`  rva=`86832`
- IL size 1035 bytes, 346 instructions, max_stack=8
- opcodes: ldarg.0x63, ldfldx34, callx32, ldloca.sx26, stfldx26, stloc.sx21, callvirtx19, ldstrx15, ldloc.sx10, dupx9, ldnullx9, ldfldax8, ldloc.0x7, stloc.0x7, initobjx5, brtrue.sx4, ldc.i4.1x4, newobjx4, beqx3, brfalse.sx3, ldc.i4.0x3, ldc.i4.2x3, ldc.i4.m1x3, ldc.i4.sx3, ldloc.1x3, leavex3, bge.sx2, endfinallyx2, leave.sx2, nopx2, ble.unx1, boxx1, brfalsex1, ldftnx1, ldloc.2x1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, subx1, throwx1
- string literals: "Resolving version from Mojang…", "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json", "versions", "Minecraft version ", " not found in Mojang's manifest.", "url", "downloads", "server", "url", "sha1", "javaVersion", "majorVersion", "server.jar", "Downloading Minecraft ", " server"
- calls: <>c__DisplayClass14_0..ctor, MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.EnumerateArray, <>c__DisplayClass14_0.<InstallVanillaAsync>b__0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault, System.Text.Json.JsonElement.get_ValueKind, MPServerProvider.Core.Models.InstallRequest.get_Version, System.String.Concat, System.InvalidOperationException..ctor, System.Text.Json.JsonElement.GetString, System.Text.Json.JsonElement.TryGetProperty, System.Text.Json.JsonElement.GetInt32, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version, MPServerProvider.Core.Models.InstallResult.set_JavaMajor, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600036f`  flags=`481`  rva=`87956`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

