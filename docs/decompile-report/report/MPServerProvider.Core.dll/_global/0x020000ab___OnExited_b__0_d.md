# <<OnExited>b__0>d

Token: `0x020000ab`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400039f)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x040003a0)
- `<>4__this` : `class:424`  (flags=6, token=0x040003a1)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x040003a2)

## Methods

### `MoveNext()` -> `void`
- token `0x060003a8`  flags=`481`  rva=`96192`
- IL size 275 bytes, 107 instructions, max_stack=3
- opcodes: ldarg.0x20, callx10, stfldx8, ldfldx6, ldfldax6, ldloca.sx6, leave.sx5, stloc.0x5, dupx4, stloc.2x4, callvirtx3, ldc.i4.1x3, ldloc.0x3, beq.sx2, brtrue.sx2, initobjx2, ldc.i4.m1x2, ldc.i4.sx2, ldloc.1x2, ldloc.2x2, brfalse.sx1, ldc.i4x1, ldc.i4.0x1, ldloc.3x1, leavex1, nopx1, popx1, retx1, stloc.1x1, stloc.3x1
- calls: System.Threading.Tasks.Task.Delay, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, RunningServer.get_Server, MPServerProvider.Core.Services.ServerProcessService.StartAsync, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060003a9`  flags=`481`  rva=`96508`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

