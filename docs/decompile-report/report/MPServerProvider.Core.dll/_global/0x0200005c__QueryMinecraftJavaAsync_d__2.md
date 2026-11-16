# <QueryMinecraftJavaAsync>d__2

Token: `0x0200005c`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001b3)
- `<>t__builder` : `valuetype:145<valuetype:261<valuetype:305<i4,i4>>>`  (flags=6, token=0x040001b4)
- `host` : `string`  (flags=6, token=0x040001b5)
- `port` : `i4`  (flags=6, token=0x040001b6)
- `<client>5__2` : `class:345`  (flags=1, token=0x040001b7)
- `<cts>5__3` : `class:329`  (flags=1, token=0x040001b8)
- `<stream>5__4` : `class:309`  (flags=1, token=0x040001b9)
- `<jsonLength>5__5` : `i4`  (flags=1, token=0x040001ba)
- `<buffer>5__6` : `u1[]`  (flags=1, token=0x040001bb)
- `<read>5__7` : `i4`  (flags=1, token=0x040001bc)
- `<>u__1` : `valuetype:245`  (flags=1, token=0x040001bd)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x040001be)
- `<>u__3` : `valuetype:137<i4>`  (flags=1, token=0x040001bf)
- `<>u__4` : `valuetype:253<i4>`  (flags=1, token=0x040001c0)

## Methods

### `MoveNext()` -> `void`
- token `0x060002b4`  flags=`481`  rva=`53732`
- IL size 1396 bytes, 484 instructions, max_stack=5
- opcodes: ldarg.0x104, callx44, ldfldx43, stfldx38, ldloca.sx29, callvirtx27, stloc.sx23, ldfldax16, stloc.0x15, dupx14, ldloc.sx12, initobjx8, ldc.i4.m1x8, ldloc.2x8, ldnullx8, brtrue.sx7, ldc.i4.0x7, leavex7, ldloc.0x6, newobjx5, brfalse.sx4, bge.sx3, endfinallyx3, ldc.i4.1x3, ldc.i4.6x3, ldloc.3x3, ldstrx3, ble.un.sx2, conv.u1x2, ldc.i4.sx2, leave.sx2, newarrx2, nopx2, popx2, addx1, andx1, bltx1, brx1, ldc.i4x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.4x1, ldc.i4.5x1, ldc.i4.8x1, ldloc.1x1, ldsfldx1, retx1, shrx1, stloc.1x1, stloc.2x1, stloc.3x1, subx1, switchx1
- string literals: "players", "online", "max"
- calls: System.Net.Sockets.TcpClient..ctor, System.Threading.CancellationTokenSource..ctor, System.Threading.CancellationTokenSource.get_Token, System.Net.Sockets.TcpClient.ConnectAsync, System.Threading.Tasks.ValueTask.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter.GetResult, System.Net.Sockets.TcpClient.GetStream, System.IO.MemoryStream..ctor, System.IO.Stream.WriteByte, MPServerProvider.Core.Services.QueryService.WriteVarInt, System.Text.Encoding.get_UTF8, System.Text.Encoding.GetByteCount, System.Text.Encoding.GetBytes, System.ReadOnlySpan`1.op_Implicit, System.IO.Stream.Write, System.IO.MemoryStream.ToArray, MPServerProvider.Core.Services.QueryService.WritePacketAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Services.QueryService.ReadVarIntAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.MemoryExtensions.AsMemory, System.IO.Stream.ReadAsync, System.Threading.Tasks.ValueTask`1.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult, System.Text.Encoding.GetString, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.GetInt32, System.ValueTuple`2..ctor, System.Nullable`1..ctor, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException...

### `SetStateMachine(class:129)` -> `void`
- token `0x060002b5`  flags=`481`  rva=`55240`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

