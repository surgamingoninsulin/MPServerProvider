# <SendCommandAsync>d__8

Token: `0x02000062`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001e7)
- `<>t__builder` : `valuetype:145<string>`  (flags=6, token=0x040001e8)
- `<>4__this` : `class:72`  (flags=6, token=0x040001e9)
- `command` : `string`  (flags=6, token=0x040001ea)
- `ct` : `valuetype:121`  (flags=6, token=0x040001eb)
- `<>u__1` : `valuetype:245`  (flags=1, token=0x040001ec)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x040001ed)
- `<>u__3` : `valuetype:137<class:380>`  (flags=1, token=0x040001ee)

## Methods

### `MoveNext()` -> `void`
- token `0x060002d2`  flags=`481`  rva=`58056`
- IL size 459 bytes, 165 instructions, max_stack=3
- opcodes: ldarg.0x31, callx17, ldfldx12, stfldx11, ldloca.sx10, dupx8, ldfldax8, stloc.sx8, stloc.0x7, brtrue.sx6, ldloc.1x5, callvirtx4, ldloc.sx4, initobjx3, ldc.i4.m1x3, leavex3, leave.sx3, ldc.i4.2x2, ldc.i4.sx2, ldstrx2, popx2, stloc.2x2, br.sx1, brfalse.sx1, ldc.i4.0x1, ldc.i4.1x1, ldloc.0x1, ldloc.2x1, ldloc.3x1, ldnullx1, retx1, stloc.1x1, stloc.3x1, switchx1
- string literals: "[RCON] Not connected.", ""
- calls: MPServerProvider.Core.Services.RconService.get_Connected, MPServerProvider.Core.Services.RconService.BuildPacket, System.ReadOnlyMemory`1.op_Implicit, System.IO.Stream.WriteAsync, System.Threading.Tasks.ValueTask.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter.GetResult, System.IO.Stream.FlushAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Services.RconService.ReadPacketAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, RconPacket.get_Body, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002d3`  flags=`481`  rva=`58556`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

