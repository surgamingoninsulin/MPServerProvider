# MPServerProvider.Core.Services.SystemSpecs

Token: `0x02000017`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `<Processor>k__BackingField` : `string`  (flags=33, token=0x0400003f)
- `<MemoryGb>k__BackingField` : `string`  (flags=33, token=0x04000040)
- `<Graphics>k__BackingField` : `string`  (flags=33, token=0x04000041)
- `<OperatingSystem>k__BackingField` : `string`  (flags=33, token=0x04000042)

## Methods

### `.ctor(string, string, string, string)` -> `void`
- token `0x060000b4`  flags=`6278`  rva=`29616`
- IL size 36 bytes, 15 instructions, max_stack=8
- opcodes: ldarg.0x5, stfldx4, callx1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldarg.sx1, retx1
- calls: System.Object..ctor

### `get_EqualityContract()` -> `class:109`
- token `0x060000b5`  flags=`2177`  rva=`29653`
- IL size 11 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldtokenx1, retx1
- calls: System.Type.GetTypeFromHandle

### `get_Processor()` -> `string`
- token `0x060000b6`  flags=`2182`  rva=`29665`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Processor(string)` -> `void modreq:177`
- token `0x060000b7`  flags=`2182`  rva=`29673`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_MemoryGb()` -> `string`
- token `0x060000b8`  flags=`2182`  rva=`29682`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_MemoryGb(string)` -> `void modreq:177`
- token `0x060000b9`  flags=`2182`  rva=`29690`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Graphics()` -> `string`
- token `0x060000ba`  flags=`2182`  rva=`29699`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Graphics(string)` -> `void modreq:177`
- token `0x060000bb`  flags=`2182`  rva=`29707`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_OperatingSystem()` -> `string`
- token `0x060000bc`  flags=`2182`  rva=`29716`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_OperatingSystem(string)` -> `void modreq:177`
- token `0x060000bd`  flags=`2182`  rva=`29724`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `ToString()` -> `string`
- token `0x060000be`  flags=`198`  rva=`29736`
- IL size 64 bytes, 25 instructions, max_stack=2
- opcodes: ldloc.0x6, callvirtx5, popx4, ldc.i4.sx2, ldstrx2, brfalse.sx1, callx1, ldarg.0x1, newobjx1, retx1, stloc.0x1
- string literals: "SystemSpecs", " { "
- calls: System.Text.StringBuilder..ctor, System.Text.StringBuilder.Append, MPServerProvider.Core.Services.SystemSpecs.PrintMembers, System.Object.ToString

### `PrintMembers(class:201)` -> `bool`
- token `0x060000bf`  flags=`129`  rva=`29812`
- IL size 107 bytes, 39 instructions, max_stack=2
- opcodes: callvirtx8, ldarg.1x8, popx8, callx5, ldarg.0x4, ldstrx4, ldc.i4.1x1, retx1
- string literals: "Processor = ", ", MemoryGb = ", ", Graphics = ", ", OperatingSystem = "
- calls: System.Runtime.CompilerServices.RuntimeHelpers.EnsureSufficientExecutionStack, System.Text.StringBuilder.Append, MPServerProvider.Core.Services.SystemSpecs.get_Processor, MPServerProvider.Core.Services.SystemSpecs.get_MemoryGb, MPServerProvider.Core.Services.SystemSpecs.get_Graphics, MPServerProvider.Core.Services.SystemSpecs.get_OperatingSystem

### `op_Inequality(class:92, class:92)` -> `bool`
- token `0x060000c0`  flags=`2198`  rva=`29931`
- IL size 11 bytes, 6 instructions, max_stack=8
- opcodes: callx1, ceqx1, ldarg.0x1, ldarg.1x1, ldc.i4.0x1, retx1
- calls: MPServerProvider.Core.Services.SystemSpecs.op_Equality

### `op_Equality(class:92, class:92)` -> `bool`
- token `0x060000c1`  flags=`2198`  rva=`29943`
- IL size 19 bytes, 13 instructions, max_stack=8
- opcodes: ldarg.0x3, retx3, ldarg.1x2, beq.sx1, brfalse.sx1, callvirtx1, ldc.i4.0x1, ldc.i4.1x1
- calls: MPServerProvider.Core.Services.SystemSpecs.Equals

### `GetHashCode()` -> `i4`
- token `0x060000c2`  flags=`198`  rva=`29964`
- IL size 109 bytes, 33 instructions, max_stack=3
- opcodes: callx6, callvirtx5, ldarg.0x5, addx4, ldc.i4x4, ldfldx4, mulx4, retx1
- calls: System.Collections.Generic.EqualityComparer`1.get_Default, MPServerProvider.Core.Services.SystemSpecs.get_EqualityContract, System.Collections.Generic.EqualityComparer`1.GetHashCode

### `Equals(object)` -> `bool`
- token `0x060000c3`  flags=`198`  rva=`30085`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, isinstx1, ldarg.0x1, ldarg.1x1, retx1
- calls: MPServerProvider.Core.Services.SystemSpecs.Equals

### `Equals(class:92)` -> `bool`
- token `0x060000c4`  flags=`486`  rva=`30100`
- IL size 125 bytes, 43 instructions, max_stack=3
- opcodes: ldfldx8, ldarg.1x7, callx6, ldarg.0x6, brfalse.sx5, callvirtx5, retx3, beq.sx1, ldc.i4.0x1, ldc.i4.1x1
- calls: MPServerProvider.Core.Services.SystemSpecs.get_EqualityContract, System.Type.op_Equality, System.Collections.Generic.EqualityComparer`1.get_Default, System.Collections.Generic.EqualityComparer`1.Equals

### `<Clone>$()` -> `class:92`
- token `0x060000c5`  flags=`134`  rva=`30237`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, newobjx1, retx1
- calls: MPServerProvider.Core.Services.SystemSpecs..ctor

### `.ctor(class:92)` -> `void`
- token `0x060000c6`  flags=`6273`  rva=`30245`
- IL size 55 bytes, 19 instructions, max_stack=8
- opcodes: ldarg.0x5, ldarg.1x4, ldfldx4, stfldx4, callx1, retx1
- calls: System.Object..ctor

### `Deconstruct(string&, string&, string&, string&)` -> `void`
- token `0x060000c7`  flags=`134`  rva=`30301`
- IL size 34 bytes, 17 instructions, max_stack=8
- opcodes: callx4, ldarg.0x4, stind.refx4, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldarg.sx1, retx1
- calls: MPServerProvider.Core.Services.SystemSpecs.get_Processor, MPServerProvider.Core.Services.SystemSpecs.get_MemoryGb, MPServerProvider.Core.Services.SystemSpecs.get_Graphics, MPServerProvider.Core.Services.SystemSpecs.get_OperatingSystem

