# RconPacket

Token: `0x0200005f`  Flags: `1048835`  Extends: `0x01000015`

## Fields

- `<Size>k__BackingField` : `i4`  (flags=33, token=0x040001cf)
- `<Id>k__BackingField` : `i4`  (flags=33, token=0x040001d0)
- `<Type>k__BackingField` : `i4`  (flags=33, token=0x040001d1)
- `<Body>k__BackingField` : `string`  (flags=33, token=0x040001d2)

## Methods

### `.ctor(i4, i4, i4, string)` -> `void`
- token `0x060002ba`  flags=`6278`  rva=`55870`
- IL size 36 bytes, 15 instructions, max_stack=8
- opcodes: ldarg.0x5, stfldx4, callx1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldarg.sx1, retx1
- calls: System.Object..ctor

### `get_EqualityContract()` -> `class:109`
- token `0x060002bb`  flags=`2177`  rva=`55907`
- IL size 11 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldtokenx1, retx1
- calls: System.Type.GetTypeFromHandle

### `get_Size()` -> `i4`
- token `0x060002bc`  flags=`2182`  rva=`55919`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Size(i4)` -> `void modreq:177`
- token `0x060002bd`  flags=`2182`  rva=`55927`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Id()` -> `i4`
- token `0x060002be`  flags=`2182`  rva=`55936`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Id(i4)` -> `void modreq:177`
- token `0x060002bf`  flags=`2182`  rva=`55944`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Type()` -> `i4`
- token `0x060002c0`  flags=`2182`  rva=`55953`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Type(i4)` -> `void modreq:177`
- token `0x060002c1`  flags=`2182`  rva=`55961`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Body()` -> `string`
- token `0x060002c2`  flags=`2182`  rva=`55970`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Body(string)` -> `void modreq:177`
- token `0x060002c3`  flags=`2182`  rva=`55978`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `ToString()` -> `string`
- token `0x060002c4`  flags=`198`  rva=`55988`
- IL size 64 bytes, 25 instructions, max_stack=2
- opcodes: ldloc.0x6, callvirtx5, popx4, ldc.i4.sx2, ldstrx2, brfalse.sx1, callx1, ldarg.0x1, newobjx1, retx1, stloc.0x1
- string literals: "RconPacket", " { "
- calls: System.Text.StringBuilder..ctor, System.Text.StringBuilder.Append, RconPacket.PrintMembers, System.Object.ToString

### `PrintMembers(class:201)` -> `bool`
- token `0x060002c5`  flags=`129`  rva=`56064`
- IL size 149 bytes, 51 instructions, max_stack=2
- opcodes: callvirtx11, ldarg.1x8, popx8, callx5, ldarg.0x4, ldstrx4, constrained.x3, ldloca.sx3, stloc.0x3, ldc.i4.1x1, retx1
- string literals: "Size = ", ", Id = ", ", Type = ", ", Body = "
- calls: System.Runtime.CompilerServices.RuntimeHelpers.EnsureSufficientExecutionStack, System.Text.StringBuilder.Append, RconPacket.get_Size, System.Object.ToString, RconPacket.get_Id, RconPacket.get_Type, RconPacket.get_Body

### `op_Inequality(class:380, class:380)` -> `bool`
- token `0x060002c6`  flags=`2198`  rva=`56225`
- IL size 11 bytes, 6 instructions, max_stack=8
- opcodes: callx1, ceqx1, ldarg.0x1, ldarg.1x1, ldc.i4.0x1, retx1
- calls: RconPacket.op_Equality

### `op_Equality(class:380, class:380)` -> `bool`
- token `0x060002c7`  flags=`2198`  rva=`56237`
- IL size 19 bytes, 13 instructions, max_stack=8
- opcodes: ldarg.0x3, retx3, ldarg.1x2, beq.sx1, brfalse.sx1, callvirtx1, ldc.i4.0x1, ldc.i4.1x1
- calls: RconPacket.Equals

### `GetHashCode()` -> `i4`
- token `0x060002c8`  flags=`198`  rva=`56260`
- IL size 109 bytes, 33 instructions, max_stack=3
- opcodes: callx6, callvirtx5, ldarg.0x5, addx4, ldc.i4x4, ldfldx4, mulx4, retx1
- calls: System.Collections.Generic.EqualityComparer`1.get_Default, RconPacket.get_EqualityContract, System.Collections.Generic.EqualityComparer`1.GetHashCode

### `Equals(object)` -> `bool`
- token `0x060002c9`  flags=`198`  rva=`56381`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, isinstx1, ldarg.0x1, ldarg.1x1, retx1
- calls: RconPacket.Equals

### `Equals(class:380)` -> `bool`
- token `0x060002ca`  flags=`486`  rva=`56396`
- IL size 125 bytes, 43 instructions, max_stack=3
- opcodes: ldfldx8, ldarg.1x7, callx6, ldarg.0x6, brfalse.sx5, callvirtx5, retx3, beq.sx1, ldc.i4.0x1, ldc.i4.1x1
- calls: RconPacket.get_EqualityContract, System.Type.op_Equality, System.Collections.Generic.EqualityComparer`1.get_Default, System.Collections.Generic.EqualityComparer`1.Equals

### `<Clone>$()` -> `class:380`
- token `0x060002cb`  flags=`134`  rva=`56533`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, newobjx1, retx1
- calls: RconPacket..ctor

### `.ctor(class:380)` -> `void`
- token `0x060002cc`  flags=`6273`  rva=`56541`
- IL size 55 bytes, 19 instructions, max_stack=8
- opcodes: ldarg.0x5, ldarg.1x4, ldfldx4, stfldx4, callx1, retx1
- calls: System.Object..ctor

### `Deconstruct(i4&, i4&, i4&, string&)` -> `void`
- token `0x060002cd`  flags=`134`  rva=`56597`
- IL size 34 bytes, 17 instructions, max_stack=8
- opcodes: callx4, ldarg.0x4, stind.i4x3, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldarg.sx1, retx1, stind.refx1
- calls: RconPacket.get_Size, RconPacket.get_Id, RconPacket.get_Type, RconPacket.get_Body

