# <WritePacketAsync>d__3

Token: `0x0200005e`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001c9)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x040001ca)
- `payload` : `u1[]`  (flags=6, token=0x040001cb)
- `stream` : `class:309`  (flags=6, token=0x040001cc)
- `ct` : `valuetype:121`  (flags=6, token=0x040001cd)
- `<>u__1` : `valuetype:245`  (flags=1, token=0x040001ce)

## Methods

### `MoveNext()` -> `void`
- token `0x060002b8`  flags=`481`  rva=`55620`
- IL size 207 bytes, 77 instructions, max_stack=3
- opcodes: ldarg.0x16, callx9, ldfldx6, stfldx5, ldfldax4, ldloca.sx4, callvirtx3, ldloc.1x3, leave.sx3, stloc.0x3, dupx2, ldc.i4.sx2, stloc.2x2, brfalse.sx1, brtrue.sx1, conv.i4x1, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldlenx1, ldloc.0x1, ldloc.2x1, ldloc.sx1, newobjx1, retx1, stloc.1x1, stloc.3x1, stloc.sx1
- calls: System.IO.MemoryStream..ctor, MPServerProvider.Core.Services.QueryService.WriteVarInt, System.ReadOnlySpan`1.op_Implicit, System.IO.Stream.Write, System.IO.MemoryStream.ToArray, System.ReadOnlyMemory`1.op_Implicit, System.IO.Stream.WriteAsync, System.Threading.Tasks.ValueTask.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter.GetResult, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002b9`  flags=`481`  rva=`55856`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

