# <LoadCoverAsync>d__22

Token: `0x02000026`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000147)
- `<>t__builder` : `valuetype:437`  (flags=6, token=0x04000148)
- `<>4__this` : `class:40`  (flags=6, token=0x04000149)
- `<hasArtworkDefined>5__2` : `bool`  (flags=1, token=0x0400014a)
- `<>7__wrap2` : `string[]`  (flags=1, token=0x0400014b)
- `<>7__wrap3` : `i4`  (flags=1, token=0x0400014c)
- `<>u__1` : `valuetype:361<string>`  (flags=1, token=0x0400014d)

## Methods

### `MoveNext()` -> `void`
- token `0x06000169`  flags=`481`  rva=`42628`
- IL size 472 bytes, 174 instructions, max_stack=5
- opcodes: ldarg.0x25, callx19, ldloc.sx13, stfldx11, ldfldx9, stloc.sx9, callvirtx8, ldloc.1x8, ldc.i4.0x5, brfalse.sx4, dupx4, ldc.i4.1x4, ldfldax4, ldloca.sx4, brtrue.sx3, ldsfldx3, leavex3, leave.sx3, stloc.0x3, addx2, conv.i4x2, initobjx2, ldc.i4.sx2, ldelem.refx2, ldlenx2, ldloc.3x2, stelem.refx2, bltx1, blt.sx1, brx1, br.sx1, brfalsex1, brtruex1, ldc.i4.2x1, ldc.i4.m1x1, ldloc.0x1, ldloc.2x1, ldnullx1, newarrx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1
- calls: MPServerProvider.App.Views.GameCardVm.get_Game, MPServerProvider.App.Views.GameCardVm.set_Cover, MPServerProvider.App.AppServices.get_Paths, MPServerProvider.Core.Services.AppPaths.get_GameArtOverrideDir, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.Concat, System.IO.Path.Combine, System.IO.File.Exists, MPServerProvider.App.Views.GameCardVm.TrySetCover, MPServerProvider.Core.Models.GameDefinition.get_Artwork, MPServerProvider.Core.Models.ArtworkSpec.get_Cover, MPServerProvider.Core.Models.ArtworkSpec.get_Hero, System.String.IsNullOrWhiteSpace, MPServerProvider.App.AppServices.get_Artwork, MPServerProvider.Core.Services.ArtworkCacheService.GetLocalPathAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x0600016a`  flags=`481`  rva=`43140`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

