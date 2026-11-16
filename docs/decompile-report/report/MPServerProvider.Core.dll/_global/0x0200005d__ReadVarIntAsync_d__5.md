# <ReadVarIntAsync>d__5

Token: `0x0200005d`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001c1)
- `<>t__builder` : `valuetype:145<i4>`  (flags=6, token=0x040001c2)
- `stream` : `class:309`  (flags=6, token=0x040001c3)
- `ct` : `valuetype:121`  (flags=6, token=0x040001c4)
- `<value>5__2` : `i4`  (flags=1, token=0x040001c5)
- `<position>5__3` : `i4`  (flags=1, token=0x040001c6)
- `<single>5__4` : `u1[]`  (flags=1, token=0x040001c7)
- `<>u__1` : `valuetype:253<i4>`  (flags=1, token=0x040001c8)

## Methods

### `MoveNext()` -> `void`
- token `0x060002b6`  flags=`481`  rva=`55256`
- IL size 319 bytes, 125 instructions, max_stack=5
- opcodes: ldarg.0x29, ldfldx12, stfldx12, callx7, ldc.i4.0x6, ldc.i4.sx5, ldfldax4, ldloca.sx4, andx3, brtrue.sx3, stloc.0x3, dupx2, ldc.i4.1x2, ldelem.u1x2, ldnullx2, leave.sx2, newobjx2, stloc.2x2, throwx2, addx1, bltx1, brfalse.sx1, callvirtx1, initobjx1, ldc.i4x1, ldc.i4.7x1, ldc.i4.m1x1, ldloc.0x1, ldloc.1x1, ldloc.2x1, ldloc.sx1, ldstrx1, leavex1, newarrx1, orx1, retx1, shlx1, stloc.1x1, stloc.3x1, stloc.sx1
- string literals: "VarInt too big"
- calls: System.MemoryExtensions.AsMemory, System.IO.Stream.ReadAsync, System.Threading.Tasks.ValueTask`1.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult, System.IO.EndOfStreamException..ctor, System.IO.InvalidDataException..ctor, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002b7`  flags=`481`  rva=`55604`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

