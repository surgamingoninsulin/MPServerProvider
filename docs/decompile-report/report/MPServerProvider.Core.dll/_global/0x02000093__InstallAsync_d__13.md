# <InstallAsync>d__13

Token: `0x02000093`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040002ff)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x04000300)
- `request` : `class:208`  (flags=6, token=0x04000301)
- `<>4__this` : `class:140`  (flags=6, token=0x04000302)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000303)
- `ct` : `valuetype:121`  (flags=6, token=0x04000304)
- `<>u__1` : `valuetype:137<class:212>`  (flags=1, token=0x04000305)

## Methods

### `MoveNext()` -> `void`
- token `0x06000362`  flags=`481`  rva=`80192`
- IL size 1058 bytes, 358 instructions, max_stack=4
- opcodes: ldarg.0x79, callx38, ldfldx33, stfldx23, ldloca.sx21, ldfldax16, stloc.sx16, stloc.0x15, dupx14, ldloc.sx14, callvirtx9, brtrue.sx8, initobjx7, ldc.i4.m1x7, ldloc.1x7, stloc.3x7, brx6, ldstrx6, leavex6, brtruex5, leave.sx3, ldc.i4.sx2, br.sx1, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.4x1, ldc.i4.5x1, ldc.i4.6x1, ldloc.0x1, ldloc.2x1, ldloc.3x1, popx1, retx1, stloc.1x1, stloc.2x1, switchx1
- string literals: "paper", "fabric", "purpur", "velocity", "bungeecord", "forge"
- calls: MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Directory.CreateDirectory, MPServerProvider.Core.Models.InstallRequest.get_Variant, System.String.op_Equality, MPServerProvider.Core.Providers.MinecraftJavaProvider.InstallPaperAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.Core.Providers.MinecraftJavaProvider.InstallFabricAsync, MPServerProvider.Core.Providers.MinecraftJavaProvider.InstallPurpurAsync, MPServerProvider.Core.Providers.MinecraftJavaProvider.InstallVelocityAsync, MPServerProvider.Core.Providers.MinecraftJavaProvider.InstallBungeeCordAsync, MPServerProvider.Core.Providers.MinecraftJavaProvider.InstallForgeAsync, MPServerProvider.Core.Providers.MinecraftJavaProvider.InstallVanillaAsync, MPServerProvider.Core.Providers.MinecraftJavaProvider.WriteCommonFiles, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000363`  flags=`481`  rva=`81292`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

