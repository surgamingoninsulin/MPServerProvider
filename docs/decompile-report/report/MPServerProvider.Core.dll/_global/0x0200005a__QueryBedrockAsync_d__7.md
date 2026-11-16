# <QueryBedrockAsync>d__7

Token: `0x0200005a`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001a2)
- `<>t__builder` : `valuetype:145<valuetype:261<valuetype:305<i4,i4>>>`  (flags=6, token=0x040001a3)
- `host` : `string`  (flags=6, token=0x040001a4)
- `port` : `i4`  (flags=6, token=0x040001a5)
- `<udp>5__2` : `class:325`  (flags=1, token=0x040001a6)
- `<cts>5__3` : `class:329`  (flags=1, token=0x040001a7)
- `<>u__1` : `valuetype:253<i4>`  (flags=1, token=0x040001a8)
- `<>u__2` : `valuetype:253<valuetype:317>`  (flags=1, token=0x040001a9)

## Methods

### `MoveNext()` -> `void`
- token `0x060002b0`  flags=`481`  rva=`52032`
- IL size 708 bytes, 264 instructions, max_stack=6
- opcodes: ldarg.0x35, callx28, ldloca.sx20, ldloc.sx14, stfldx14, callvirtx13, ldfldx13, stloc.sx11, ldc.i4.1x6, ldc.i4.sx6, ldfldax6, ldloc.0x6, initobjx5, ldc.i4.0x5, ldloc.2x5, newobjx5, stloc.0x5, brfalse.sx4, dupx4, ldnullx4, conv.i4x3, ldelem.u1x3, ldlenx3, leavex3, leave.sx3, addx2, beq.sx2, bge.sx2, blt.sx2, brtrue.sx2, endfinallyx2, ldc.i4.2x2, ldc.i4.m1x2, ldelem.refx2, ldsfldx2, nopx2, subx2, beqx1, ble.un.sx1, brfalsex1, ldc.i4.4x1, ldc.i4.5x1, ldc.i4.6x1, ldc.i4.8x1, ldc.i8x1, ldloc.1x1, orx1, popx1, retx1, shlx1, stloc.1x1, stloc.2x1, stloc.3x1
- calls: System.Net.Sockets.UdpClient..ctor, System.Net.Sockets.UdpClient.Connect, System.IO.MemoryStream..ctor, System.IO.Stream.WriteByte, System.DateTimeOffset.get_Now, System.DateTimeOffset.ToUnixTimeMilliseconds, System.BitConverter.GetBytes, System.ReadOnlySpan`1.op_Implicit, System.IO.Stream.Write, System.IO.MemoryStream.ToArray, System.ReadOnlyMemory`1.op_Implicit, System.Net.Sockets.UdpClient.SendAsync, System.Threading.Tasks.ValueTask`1.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult, System.Threading.CancellationTokenSource..ctor, System.Threading.CancellationTokenSource.get_Token, System.Net.Sockets.UdpClient.ReceiveAsync, System.Net.Sockets.UdpReceiveResult.get_Buffer, System.Text.Encoding.get_UTF8, System.Math.Min, System.Text.Encoding.GetString, System.String.Split, System.Int32.TryParse, System.ValueTuple`2..ctor, System.Nullable`1..ctor, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002b1`  flags=`481`  rva=`52828`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

