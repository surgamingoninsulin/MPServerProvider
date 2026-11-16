# <InstallFabricAsync>d__21

Token: `0x02000095`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400030e)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x0400030f)
- `request` : `class:208`  (flags=6, token=0x04000310)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000311)
- `<>4__this` : `class:140`  (flags=6, token=0x04000312)
- `ct` : `valuetype:121`  (flags=6, token=0x04000313)
- `<>8__1` : `class:580`  (flags=1, token=0x04000314)
- `<loaders>5__2` : `class:341`  (flags=1, token=0x04000315)
- `<loader>5__3` : `string`  (flags=1, token=0x04000316)
- `<installers>5__4` : `class:341`  (flags=1, token=0x04000317)
- `<jar>5__5` : `string`  (flags=1, token=0x04000318)
- `<javaMajor>5__6` : `i4`  (flags=1, token=0x04000319)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x0400031a)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x0400031b)
- `<manifest>5__7` : `class:341`  (flags=1, token=0x0400031c)

## Methods

### `MoveNext()` -> `void`
- token `0x06000366`  flags=`481`  rva=`81648`
- IL size 1647 bytes, 553 instructions, max_stack=8
- opcodes: ldarg.0x91, callx54, ldfldx48, ldloca.sx43, stfldx38, callvirtx28, stloc.sx28, ldstrx21, dupx17, ldloc.sx16, ldloc.0x13, ldnullx13, ldfldax12, stloc.0x11, initobjx9, brtrue.sx7, leave.sx6, newobjx6, brfalse.sx5, ldc.i4.0x5, ldc.i4.m1x5, ldloc.1x5, leavex5, beqx4, bge.sx4, endfinallyx4, ldc.i4.1x4, ldc.i4.2x4, ldc.i4.3x4, ldc.i4.4x4, ldc.i4.sx4, ldsfldx4, nopx4, popx4, ble.unx3, boxx3, ldftnx3, subx3, brfalsex2, stsfldx2, beq.sx1, ldloc.2x1, ldloc.3x1, retx1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "Resolving Fabric loader…", "https://meta.fabricmc.net/v2/versions/loader", "version", "https://meta.fabricmc.net/v2/versions/installer", "version", "fabric_server.jar", "https://meta.fabricmc.net/v2", "/versions/loader/", "/", "/", "/server/jar", "Downloading Fabric ", " server launcher", "mods", "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json", "versions", "url", "javaVersion", "majorVersion", " (loader ", ")"
- calls: <>c__DisplayClass21_0..ctor, MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.EnumerateArray, <>c.<InstallFabricAsync>b__21_0, System.Func`2..ctor, System.Linq.Enumerable.First, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.GetString, <>c.<InstallFabricAsync>b__21_1, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, MPServerProvider.Core.Models.InstallRequest.get_Version, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, System.String.Concat, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.IO.Directory.CreateDirectory, <>c__DisplayClass21_0.<InstallFabricAsync>b__2, System.Linq.Enumerable.FirstOrDefault, System.Text.Json.JsonElement.get_ValueKind, System.Text.Json.JsonElement.TryGetProperty, System.Text.Json.JsonElement.GetInt32, System.IDisposable.Dispose, MPServerProvider.Core.Providers.MinecraftJavaProvider.GuessJavaMajor, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version...

### `SetStateMachine(class:129)` -> `void`
- token `0x06000367`  flags=`481`  rva=`83456`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

