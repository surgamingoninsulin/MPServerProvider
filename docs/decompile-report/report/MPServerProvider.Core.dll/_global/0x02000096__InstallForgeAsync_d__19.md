# <InstallForgeAsync>d__19

Token: `0x02000096`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400031d)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x0400031e)
- `progress` : `class:233<class:200>`  (flags=6, token=0x0400031f)
- `<>4__this` : `class:140`  (flags=6, token=0x04000320)
- `ct` : `valuetype:121`  (flags=6, token=0x04000321)
- `request` : `class:208`  (flags=6, token=0x04000322)
- `<promos>5__2` : `class:341`  (flags=1, token=0x04000323)
- `<forgeVersion>5__3` : `string`  (flags=1, token=0x04000324)
- `<jar>5__4` : `string`  (flags=1, token=0x04000325)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000326)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x04000327)

## Methods

### `MoveNext()` -> `void`
- token `0x06000368`  flags=`481`  rva=`83472`
- IL size 1270 bytes, 427 instructions, max_stack=8
- opcodes: ldarg.0x65, callx45, ldfldx36, callvirtx33, ldloca.sx33, ldloc.sx25, ldstrx24, stfldx21, stloc.sx21, dupx11, ldc.i4.sx9, ldnullx9, ldfldax8, stloc.0x7, brfalse.sx6, ldc.i4.1x6, brtrue.sx5, ldc.i4.0x5, ldloc.0x5, initobjx4, ldc.i4.2x4, newobjx4, br.sx3, ldc.i4.m1x3, ldloc.1x3, leavex3, subx3, beqx2, ldc.i4.4x2, leave.sx2, stelem.i2x2, addx1, bge.sx1, ble.sx1, ble.unx1, blt.sx1, brtruex1, conv.i4x1, endfinallyx1, ldelem.refx1, ldlenx1, ldloc.2x1, newarrx1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, throwx1
- string literals: "Resolving Forge version…", "https://files.minecraftforge.net/net/minecraftforge/forge/promotions_slim.json", "promos", "-recommended", "-latest", "https://files.minecraftforge.net/maven/net/minecraftforge/forge/maven-metadata.xml", "-", "", "<version>", "</version>", "No Forge version found for Minecraft ", ".", "https://maven.minecraftforge.net/net/minecraftforge/forge/", "-", "/forge-", "-", "-installer.jar", "forge-installer.jar", "Downloading Forge ", "-", " installer…", "Downloading Forge ", "-", "-"
- calls: MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, MPServerProvider.Core.Models.InstallRequest.get_Version, System.String.Concat, System.Text.Json.JsonElement.TryGetProperty, System.Text.Json.JsonElement.GetString, System.String.Split, System.String.Trim, System.String.StartsWith, System.String.EndsWith, System.String.get_Length, System.String.Substring, System.String.Compare, System.String.IsNullOrEmpty, System.InvalidOperationException..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version, MPServerProvider.Core.Providers.MinecraftJavaProvider.GuessJavaMajor, MPServerProvider.Core.Models.InstallResult.set_JavaMajor, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException...

### `SetStateMachine(class:129)` -> `void`
- token `0x06000369`  flags=`481`  rva=`84808`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

