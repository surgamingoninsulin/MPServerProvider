# <<PrefetchArtworkAsync>b__2>d

Token: `0x0200005b`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000201)
- `<>t__builder` : `valuetype:437`  (flags=6, token=0x04000202)
- `<>4__this` : `class:328`  (flags=6, token=0x04000203)
- `url` : `string`  (flags=6, token=0x04000204)
- `<>8__1` : `class:332`  (flags=1, token=0x04000205)
- `<>u__1` : `valuetype:361<string>`  (flags=1, token=0x04000206)

## Methods

### `MoveNext()` -> `void`
- token `0x060001ee`  flags=`481`  rva=`57732`
- IL size 267 bytes, 92 instructions, max_stack=3
- opcodes: ldarg.0x20, stfldx10, callx8, ldfldx8, ldfldax5, ldloca.sx4, callvirtx3, ldloc.1x3, stloc.0x3, dupx2, initobjx2, ldc.i4.sx2, ldnullx2, leave.sx2, newobjx2, stloc.2x2, brfalse.sx1, brtrue.sx1, ldc.i4.0x1, ldc.i4.m1x1, ldftnx1, ldloc.0x1, ldloc.2x1, ldloc.3x1, ldloc.sx1, leavex1, popx1, retx1, stloc.1x1, stloc.sx1
- calls: <>c__DisplayClass2_1..ctor, MPServerProvider.App.AppServices.get_Artwork, MPServerProvider.Core.Services.ArtworkCacheService.GetLocalPathAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Threading.Interlocked.Increment, System.Windows.Threading.DispatcherObject.get_Dispatcher, <>c__DisplayClass2_1.<PrefetchArtworkAsync>b__3, System.Action..ctor, System.Windows.Threading.Dispatcher.Invoke, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x060001ef`  flags=`481`  rva=`58028`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

