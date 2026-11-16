# MPServerProvider.Core.Services.DriveUsage

Token: `0x02000018`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `<Name>k__BackingField` : `string`  (flags=33, token=0x04000043)
- `<Root>k__BackingField` : `string`  (flags=33, token=0x04000044)
- `<TotalBytes>k__BackingField` : `i8`  (flags=33, token=0x04000045)
- `<FreeBytes>k__BackingField` : `i8`  (flags=33, token=0x04000046)

## Methods

### `.ctor(string, string, i8, i8)` -> `void`
- token `0x060000c8`  flags=`6278`  rva=`30336`
- IL size 36 bytes, 15 instructions, max_stack=8
- opcodes: ldarg.0x5, stfldx4, callx1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldarg.sx1, retx1
- calls: System.Object..ctor

### `get_EqualityContract()` -> `class:109`
- token `0x060000c9`  flags=`2177`  rva=`30373`
- IL size 11 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldtokenx1, retx1
- calls: System.Type.GetTypeFromHandle

### `get_Name()` -> `string`
- token `0x060000ca`  flags=`2182`  rva=`30385`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Name(string)` -> `void modreq:177`
- token `0x060000cb`  flags=`2182`  rva=`30393`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Root()` -> `string`
- token `0x060000cc`  flags=`2182`  rva=`30402`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Root(string)` -> `void modreq:177`
- token `0x060000cd`  flags=`2182`  rva=`30410`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_TotalBytes()` -> `i8`
- token `0x060000ce`  flags=`2182`  rva=`30419`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_TotalBytes(i8)` -> `void modreq:177`
- token `0x060000cf`  flags=`2182`  rva=`30427`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_FreeBytes()` -> `i8`
- token `0x060000d0`  flags=`2182`  rva=`30436`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_FreeBytes(i8)` -> `void modreq:177`
- token `0x060000d1`  flags=`2182`  rva=`30444`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_UsedBytes()` -> `i8`
- token `0x060000d2`  flags=`2182`  rva=`30453`
- IL size 14 bytes, 6 instructions, max_stack=8
- opcodes: callx2, ldarg.0x2, retx1, subx1
- calls: MPServerProvider.Core.Services.DriveUsage.get_TotalBytes, MPServerProvider.Core.Services.DriveUsage.get_FreeBytes

### `get_UsedPercent()` -> `r8`
- token `0x060000d3`  flags=`2182`  rva=`30468`
- IL size 44 bytes, 15 instructions, max_stack=8
- opcodes: callx3, ldarg.0x3, conv.r8x2, ldc.r8x2, retx2, brfalse.sx1, divx1, mulx1
- calls: MPServerProvider.Core.Services.DriveUsage.get_TotalBytes, MPServerProvider.Core.Services.DriveUsage.get_UsedBytes

### `ToString()` -> `string`
- token `0x060000d4`  flags=`198`  rva=`30516`
- IL size 64 bytes, 25 instructions, max_stack=2
- opcodes: ldloc.0x6, callvirtx5, popx4, ldc.i4.sx2, ldstrx2, brfalse.sx1, callx1, ldarg.0x1, newobjx1, retx1, stloc.0x1
- string literals: "DriveUsage", " { "
- calls: System.Text.StringBuilder..ctor, System.Text.StringBuilder.Append, MPServerProvider.Core.Services.DriveUsage.PrintMembers, System.Object.ToString

### `PrintMembers(class:201)` -> `bool`
- token `0x060000d5`  flags=`129`  rva=`30592`
- IL size 213 bytes, 73 instructions, max_stack=2
- opcodes: callvirtx16, ldarg.1x12, popx12, callx7, ldarg.0x6, ldstrx6, constrained.x4, ldloca.sx4, stloc.0x3, ldc.i4.1x1, retx1, stloc.1x1
- string literals: "Name = ", ", Root = ", ", TotalBytes = ", ", FreeBytes = ", ", UsedBytes = ", ", UsedPercent = "
- calls: System.Runtime.CompilerServices.RuntimeHelpers.EnsureSufficientExecutionStack, System.Text.StringBuilder.Append, MPServerProvider.Core.Services.DriveUsage.get_Name, MPServerProvider.Core.Services.DriveUsage.get_Root, MPServerProvider.Core.Services.DriveUsage.get_TotalBytes, System.Object.ToString, MPServerProvider.Core.Services.DriveUsage.get_FreeBytes, MPServerProvider.Core.Services.DriveUsage.get_UsedBytes, MPServerProvider.Core.Services.DriveUsage.get_UsedPercent

### `op_Inequality(class:96, class:96)` -> `bool`
- token `0x060000d6`  flags=`2198`  rva=`30817`
- IL size 11 bytes, 6 instructions, max_stack=8
- opcodes: callx1, ceqx1, ldarg.0x1, ldarg.1x1, ldc.i4.0x1, retx1
- calls: MPServerProvider.Core.Services.DriveUsage.op_Equality

### `op_Equality(class:96, class:96)` -> `bool`
- token `0x060000d7`  flags=`2198`  rva=`30829`
- IL size 19 bytes, 13 instructions, max_stack=8
- opcodes: ldarg.0x3, retx3, ldarg.1x2, beq.sx1, brfalse.sx1, callvirtx1, ldc.i4.0x1, ldc.i4.1x1
- calls: MPServerProvider.Core.Services.DriveUsage.Equals

### `GetHashCode()` -> `i4`
- token `0x060000d8`  flags=`198`  rva=`30852`
- IL size 109 bytes, 33 instructions, max_stack=3
- opcodes: callx6, callvirtx5, ldarg.0x5, addx4, ldc.i4x4, ldfldx4, mulx4, retx1
- calls: System.Collections.Generic.EqualityComparer`1.get_Default, MPServerProvider.Core.Services.DriveUsage.get_EqualityContract, System.Collections.Generic.EqualityComparer`1.GetHashCode

### `Equals(object)` -> `bool`
- token `0x060000d9`  flags=`198`  rva=`30973`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, isinstx1, ldarg.0x1, ldarg.1x1, retx1
- calls: MPServerProvider.Core.Services.DriveUsage.Equals

### `Equals(class:96)` -> `bool`
- token `0x060000da`  flags=`486`  rva=`30988`
- IL size 125 bytes, 43 instructions, max_stack=3
- opcodes: ldfldx8, ldarg.1x7, callx6, ldarg.0x6, brfalse.sx5, callvirtx5, retx3, beq.sx1, ldc.i4.0x1, ldc.i4.1x1
- calls: MPServerProvider.Core.Services.DriveUsage.get_EqualityContract, System.Type.op_Equality, System.Collections.Generic.EqualityComparer`1.get_Default, System.Collections.Generic.EqualityComparer`1.Equals

### `<Clone>$()` -> `class:96`
- token `0x060000db`  flags=`134`  rva=`31125`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, newobjx1, retx1
- calls: MPServerProvider.Core.Services.DriveUsage..ctor

### `.ctor(class:96)` -> `void`
- token `0x060000dc`  flags=`6273`  rva=`31133`
- IL size 55 bytes, 19 instructions, max_stack=8
- opcodes: ldarg.0x5, ldarg.1x4, ldfldx4, stfldx4, callx1, retx1
- calls: System.Object..ctor

### `Deconstruct(string&, string&, i8&, i8&)` -> `void`
- token `0x060000dd`  flags=`134`  rva=`31189`
- IL size 34 bytes, 17 instructions, max_stack=8
- opcodes: callx4, ldarg.0x4, stind.i8x2, stind.refx2, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldarg.sx1, retx1
- calls: MPServerProvider.Core.Services.DriveUsage.get_Name, MPServerProvider.Core.Services.DriveUsage.get_Root, MPServerProvider.Core.Services.DriveUsage.get_TotalBytes, MPServerProvider.Core.Services.DriveUsage.get_FreeBytes

