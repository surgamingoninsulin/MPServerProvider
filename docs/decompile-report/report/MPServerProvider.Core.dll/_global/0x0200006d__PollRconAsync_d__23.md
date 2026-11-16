# <PollRconAsync>d__23

Token: `0x0200006d`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400021f)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x04000220)
- `<>4__this` : `class:80`  (flags=6, token=0x04000221)
- `running` : `class:400`  (flags=6, token=0x04000222)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000223)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x04000224)

## Methods

### `MoveNext()` -> `void`
- token `0x060002fe`  flags=`481`  rva=`61768`
- IL size 362 bytes, 129 instructions, max_stack=4
- opcodes: ldarg.0x22, callx12, ldfldx8, stfldx8, ldloca.sx7, ldfldax6, callvirtx5, leave.sx5, stloc.0x5, dupx4, brtrue.sx3, initobjx3, ldloc.0x3, ldloc.1x3, ldloc.sx3, stloc.sx3, brfalse.sx2, ldc.i4.1x2, ldc.i4.m1x2, ldc.i4.sx2, ldloc.2x2, stloc.3x2, beqx1, bne.un.sx1, brx1, brtruex1, ldc.i4x1, ldc.i4.0x1, ldc.i4.2x1, ldc.r8x1, ldloc.3x1, ldstrx1, leavex1, newobjx1, nopx1, popx1, retx1, stloc.1x1, stloc.2x1
- string literals: "status"
- calls: System.TimeSpan.FromSeconds, System.Nullable`1..ctor, MPServerProvider.Core.Services.RconService.SendCommandMultiAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Services.ServerProcessService.AppendLine, System.Threading.Tasks.Task.Delay, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, RunningServer.get_State, MPServerProvider.Core.Services.RconService.get_Connected, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002ff`  flags=`481`  rva=`62196`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

