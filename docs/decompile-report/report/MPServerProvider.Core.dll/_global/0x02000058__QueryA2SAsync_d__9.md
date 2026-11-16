# <QueryA2SAsync>d__9

Token: `0x02000058`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000194)
- `<>t__builder` : `valuetype:145<valuetype:261<valuetype:305<i4,i4>>>`  (flags=6, token=0x04000195)
- `host` : `string`  (flags=6, token=0x04000196)
- `port` : `i4`  (flags=6, token=0x04000197)
- `<udp>5__2` : `class:325`  (flags=1, token=0x04000198)
- `<cts>5__3` : `class:329`  (flags=1, token=0x04000199)
- `<>u__1` : `valuetype:253<i4>`  (flags=1, token=0x0400019a)
- `<>u__2` : `valuetype:253<valuetype:317>`  (flags=1, token=0x0400019b)

## Methods

### `MoveNext()` -> `void`
- token `0x060002ac`  flags=`481`  rva=`50364`
- IL size 865 bytes, 340 instructions, max_stack=4
- opcodes: ldarg.0x52, callx26, ldloca.sx23, stloc.sx21, stfldx20, ldfldx18, ldloc.sx16, ldfldax10, ldloc.3x10, callvirtx9, stloc.0x9, dupx8, initobjx8, ldc.i4.1x7, ldloc.0x6, addx5, brtrue.sx5, ldc.i4.sx5, ldelem.u1x5, ldnullx5, leavex5, conv.i4x4, ldc.i4.0x4, ldc.i4.4x4, ldc.i4.m1x4, ldlenx4, newobjx4, bge.sx3, blt.sx3, brfalse.sx3, ldc.i4.2x3, leave.sx3, endfinallyx2, ldc.i4.3x2, ldc.i4.6x2, nopx2, popx2, stloc.3x2, subx2, beq.sx1, ble.unx1, ble.un.sx1, bltx1, bne.unx1, br.sx1, ldc.i4.5x1, ldloc.1x1, ldloc.2x1, ldsfldx1, retx1, stloc.1x1, stloc.2x1, switchx1
- calls: System.Net.Sockets.UdpClient..ctor, System.Net.Sockets.UdpClient.Connect, MPServerProvider.Core.Services.QueryService.BuildA2SInfo, System.ReadOnlyMemory`1.op_Implicit, System.Net.Sockets.UdpClient.SendAsync, System.Threading.Tasks.ValueTask`1.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult, System.Threading.CancellationTokenSource..ctor, System.Threading.CancellationTokenSource.get_Token, System.Net.Sockets.UdpClient.ReceiveAsync, System.Net.Sockets.UdpReceiveResult.get_Buffer, System.MemoryExtensions.AsSpan, System.Span`1.ToArray, System.ValueTuple`2..ctor, System.Nullable`1..ctor, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002ad`  flags=`481`  rva=`51320`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

