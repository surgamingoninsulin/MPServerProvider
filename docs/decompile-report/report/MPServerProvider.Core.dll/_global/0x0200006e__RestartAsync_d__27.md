# <RestartAsync>d__27

Token: `0x0200006e`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000225)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x04000226)
- `<>4__this` : `class:80`  (flags=6, token=0x04000227)
- `server` : `class:220`  (flags=6, token=0x04000228)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x04000229)

## Methods

### `MoveNext()` -> `void`
- token `0x06000300`  flags=`481`  rva=`62212`
- IL size 374 bytes, 134 instructions, max_stack=3
- opcodes: ldarg.0x29, callx14, stfldx11, ldloca.sx10, ldfldax8, ldfldx7, stloc.0x7, dupx6, stloc.2x6, initobjx4, brtrue.sx3, callvirtx3, ldc.i4.m1x3, ldloc.2x3, leave.sx3, ldc.i4.sx2, ldloc.1x2, leavex2, ldc.i4x1, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.2x1, ldloc.0x1, ldloc.3x1, ldloc.sx1, retx1, stloc.1x1, stloc.sx1, switchx1
- calls: MPServerProvider.Core.Services.ServerProcessService.StopAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Threading.Tasks.Task.Delay, MPServerProvider.Core.Services.ServerProcessService.StartAsync, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000301`  flags=`481`  rva=`62628`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

