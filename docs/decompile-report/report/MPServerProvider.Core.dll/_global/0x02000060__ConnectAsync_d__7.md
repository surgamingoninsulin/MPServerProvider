# <ConnectAsync>d__7

Token: `0x02000060`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001d3)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x040001d4)
- `<>4__this` : `class:72`  (flags=6, token=0x040001d5)
- `host` : `string`  (flags=6, token=0x040001d6)
- `port` : `i4`  (flags=6, token=0x040001d7)
- `ct` : `valuetype:121`  (flags=6, token=0x040001d8)
- `password` : `string`  (flags=6, token=0x040001d9)
- `<client>5__2` : `class:345`  (flags=1, token=0x040001da)
- `<>u__1` : `valuetype:245`  (flags=1, token=0x040001db)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x040001dc)
- `<>u__3` : `valuetype:137<class:380>`  (flags=1, token=0x040001dd)

## Methods

### `MoveNext()` -> `void`
- token `0x060002ce`  flags=`481`  rva=`56632`
- IL size 738 bytes, 262 instructions, max_stack=4
- opcodes: ldarg.0x55, callx26, stfldx22, ldfldx20, ldloca.sx17, stloc.sx13, ldfldax12, stloc.0x11, dupx10, callvirtx9, ldloc.1x9, ldloc.sx6, brtrue.sx5, initobjx5, ldc.i4.m1x5, leavex4, ldloc.3x3, ldnullx3, leave.sx3, brfalse.sx2, ldc.i4.0x2, ldc.i4.2x2, ldc.i4.3x2, ldc.i4.sx2, bne.un.sx1, br.sx1, cgt.unx1, ldc.i4.1x1, ldc.i4.4x1, ldloc.0x1, ldloc.2x1, newobjx1, popx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, switchx1
- calls: MPServerProvider.Core.Services.RconService.Disconnect, System.Net.Sockets.TcpClient..ctor, System.Net.Sockets.TcpClient.ConnectAsync, System.Threading.Tasks.ValueTask.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter.GetResult, System.Net.Sockets.TcpClient.GetStream, MPServerProvider.Core.Services.RconService.BuildPacket, System.ReadOnlyMemory`1.op_Implicit, System.IO.Stream.WriteAsync, System.IO.Stream.FlushAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Services.RconService.ReadPacketAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, RconPacket.get_Type, RconPacket.get_Body, MPServerProvider.Core.Services.RconService.set_Connected, MPServerProvider.Core.Services.RconService.get_Connected, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002cf`  flags=`481`  rva=`57412`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

