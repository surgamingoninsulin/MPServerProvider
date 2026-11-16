# <<StartLogTail>b__0>d

Token: `0x020000aa`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000399)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x0400039a)
- `<>4__this` : `class:420`  (flags=6, token=0x0400039b)
- `<reader>5__2` : `class:229`  (flags=1, token=0x0400039c)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x0400039d)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x0400039e)

## Methods

### `MoveNext()` -> `void`
- token `0x060003a6`  flags=`481`  rva=`95684`
- IL size 404 bytes, 156 instructions, max_stack=5
- opcodes: ldarg.0x26, ldfldx12, callx11, stfldx10, callvirtx7, ldfldax6, ldloca.sx6, ldc.i4.1x5, ldloc.0x5, leave.sx5, stloc.0x5, dupx4, ldloc.1x4, popx4, brfalse.sx3, ldc.i4.0x3, stloc.sx3, beqx2, brtrue.sx2, initobjx2, ldc.i4.2x2, ldc.i4.3x2, ldc.i4.m1x2, ldc.i4.sx2, ldloc.2x2, ldloc.sx2, leavex2, nopx2, stloc.3x2, bge.sx1, ble.un.sx1, brx1, br.sx1, conv.i8x1, endfinallyx1, ldc.i4x1, ldloc.3x1, ldnullx1, newobjx1, retx1, stloc.1x1, stloc.2x1
- calls: System.IO.File.Open, System.IO.StreamReader..ctor, System.IO.StreamReader.get_BaseStream, System.IO.Stream.Seek, System.IO.TextReader.ReadLineAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.Core.Services.ServerProcessService.AppendLine, System.Threading.Tasks.Task.Delay, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, RunningServer.get_State, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060003a7`  flags=`481`  rva=`96176`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

