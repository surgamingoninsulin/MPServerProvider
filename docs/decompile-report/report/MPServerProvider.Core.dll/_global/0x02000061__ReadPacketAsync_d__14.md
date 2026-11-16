# <ReadPacketAsync>d__14

Token: `0x02000061`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001de)
- `<>t__builder` : `valuetype:145<class:380>`  (flags=6, token=0x040001df)
- `<>4__this` : `class:72`  (flags=6, token=0x040001e0)
- `ct` : `valuetype:121`  (flags=6, token=0x040001e1)
- `<sizeBuf>5__2` : `u1[]`  (flags=1, token=0x040001e2)
- `<size>5__3` : `i4`  (flags=1, token=0x040001e3)
- `<buf>5__4` : `u1[]`  (flags=1, token=0x040001e4)
- `<offset>5__5` : `i4`  (flags=1, token=0x040001e5)
- `<>u__1` : `valuetype:253<i4>`  (flags=1, token=0x040001e6)

## Methods

### `MoveNext()` -> `void`
- token `0x060002d0`  flags=`481`  rva=`57428`
- IL size 571 bytes, 210 instructions, max_stack=5
- opcodes: ldarg.0x46, ldfldx24, callx17, stfldx17, stloc.sx10, ldloca.sx8, ldnullx7, ldfldax6, ldloc.sx6, leavex5, stloc.0x5, brtrue.sx4, callvirtx4, dupx4, ldc.i4.0x4, stloc.2x4, ldc.i4.4x3, ldc.i4.sx3, ldloc.1x3, ldloc.3x3, initobjx2, ldc.i4.1x2, ldc.i4.m1x2, ldloc.0x2, leave.sx2, newarrx2, stloc.3x2, subx2, addx1, beqx1, bge.sx1, bltx1, brx1, brfalse.sx1, ldc.i4.8x1, ldloc.2x1, newobjx1, retx1, stloc.1x1
- calls: System.Memory`1.op_Implicit, System.IO.Stream.ReadAsync, System.Threading.Tasks.ValueTask`1.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult, System.ReadOnlySpan`1.op_Implicit, System.BitConverter.ToInt32, System.MemoryExtensions.AsMemory, System.Text.Encoding.get_UTF8, System.Text.Encoding.GetString, System.String.TrimEnd, RconPacket..ctor, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002d1`  flags=`481`  rva=`58040`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

