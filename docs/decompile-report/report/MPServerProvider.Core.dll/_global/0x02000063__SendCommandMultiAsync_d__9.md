# <SendCommandMultiAsync>d__9

Token: `0x02000063`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001ef)
- `<>t__builder` : `valuetype:145<string>`  (flags=6, token=0x040001f0)
- `<>4__this` : `class:72`  (flags=6, token=0x040001f1)
- `command` : `string`  (flags=6, token=0x040001f2)
- `ct` : `valuetype:121`  (flags=6, token=0x040001f3)
- `pollTime` : `valuetype:261<valuetype:89>`  (flags=6, token=0x040001f4)
- `<sb>5__2` : `class:201`  (flags=1, token=0x040001f5)
- `<timeout>5__3` : `valuetype:89`  (flags=1, token=0x040001f6)
- `<deadline>5__4` : `valuetype:353`  (flags=1, token=0x040001f7)
- `<>u__1` : `valuetype:245`  (flags=1, token=0x040001f8)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x040001f9)
- `<>u__3` : `valuetype:137<class:380>`  (flags=1, token=0x040001fa)

## Methods

### `MoveNext()` -> `void`
- token `0x060002d4`  flags=`481`  rva=`58572`
- IL size 783 bytes, 264 instructions, max_stack=3
- opcodes: ldarg.0x52, callx32, ldfldx21, stfldx20, ldloca.sx15, stloc.sx12, ldfldax11, callvirtx10, dupx9, stloc.0x9, brtrue.sx8, ldloc.sx8, ldloc.1x6, leavex5, initobjx4, ldc.i4.m1x4, br.sx3, brfalse.sx3, ldc.i4.0x3, ldc.i4.sx3, brfalsex2, ldc.i4.2x2, ldnullx2, leave.sx2, popx2, stloc.2x2, brx1, cgtx1, ldc.i4.1x1, ldc.i4.3x1, ldc.r8x1, ldloc.0x1, ldloc.2x1, ldloc.3x1, ldstrx1, newobjx1, retx1, stloc.1x1, stloc.3x1, switchx1
- string literals: "[RCON] Not connected."
- calls: MPServerProvider.Core.Services.RconService.get_Connected, MPServerProvider.Core.Services.RconService.BuildPacket, System.ReadOnlyMemory`1.op_Implicit, System.IO.Stream.WriteAsync, System.Threading.Tasks.ValueTask.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter.GetResult, System.IO.Stream.FlushAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Text.StringBuilder..ctor, System.Nullable`1.get_HasValue, System.TimeSpan.FromMilliseconds, System.Nullable`1.GetValueOrDefault, System.DateTime.get_UtcNow, System.DateTime.op_Addition, System.Net.Sockets.TcpClient.get_Available, MPServerProvider.Core.Services.RconService.ReadPacketAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, RconPacket.get_Body, System.String.IsNullOrWhiteSpace, System.Text.StringBuilder.AppendLine, System.Threading.Tasks.Task.Delay, System.DateTime.op_LessThan, System.Threading.CancellationToken.get_IsCancellationRequested, System.Object.ToString, System.String.TrimEnd, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002d5`  flags=`481`  rva=`59396`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

