# MPServerProvider.Core.Services.UpnpResult

Token: `0x0200001a`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `<Success>k__BackingField` : `bool`  (flags=33, token=0x0400004a)
- `<Message>k__BackingField` : `string`  (flags=33, token=0x0400004b)
- `<ExternalIp>k__BackingField` : `class:429`  (flags=33, token=0x0400004c)

## Methods

### `.ctor(bool, string, class:429)` -> `void`
- token `0x060000e6`  flags=`6278`  rva=`32286`
- IL size 28 bytes, 12 instructions, max_stack=8
- opcodes: ldarg.0x4, stfldx3, callx1, ldarg.1x1, ldarg.2x1, ldarg.3x1, retx1
- calls: System.Object..ctor

### `get_EqualityContract()` -> `class:109`
- token `0x060000e7`  flags=`2177`  rva=`32315`
- IL size 11 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldtokenx1, retx1
- calls: System.Type.GetTypeFromHandle

### `get_Success()` -> `bool`
- token `0x060000e8`  flags=`2182`  rva=`32327`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Success(bool)` -> `void modreq:177`
- token `0x060000e9`  flags=`2182`  rva=`32335`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Message()` -> `string`
- token `0x060000ea`  flags=`2182`  rva=`32344`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Message(string)` -> `void modreq:177`
- token `0x060000eb`  flags=`2182`  rva=`32352`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_ExternalIp()` -> `class:429`
- token `0x060000ec`  flags=`2182`  rva=`32361`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_ExternalIp(class:429)` -> `void modreq:177`
- token `0x060000ed`  flags=`2182`  rva=`32369`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `ToString()` -> `string`
- token `0x060000ee`  flags=`198`  rva=`32380`
- IL size 64 bytes, 25 instructions, max_stack=2
- opcodes: ldloc.0x6, callvirtx5, popx4, ldc.i4.sx2, ldstrx2, brfalse.sx1, callx1, ldarg.0x1, newobjx1, retx1, stloc.0x1
- string literals: "UpnpResult", " { "
- calls: System.Text.StringBuilder..ctor, System.Text.StringBuilder.Append, MPServerProvider.Core.Services.UpnpResult.PrintMembers, System.Object.ToString

### `PrintMembers(class:201)` -> `bool`
- token `0x060000ef`  flags=`129`  rva=`32456`
- IL size 96 bytes, 34 instructions, max_stack=2
- opcodes: callvirtx7, ldarg.1x6, popx6, callx4, ldarg.0x3, ldstrx3, constrained.x1, ldc.i4.1x1, ldloca.sx1, retx1, stloc.0x1
- string literals: "Success = ", ", Message = ", ", ExternalIp = "
- calls: System.Runtime.CompilerServices.RuntimeHelpers.EnsureSufficientExecutionStack, System.Text.StringBuilder.Append, MPServerProvider.Core.Services.UpnpResult.get_Success, System.Object.ToString, MPServerProvider.Core.Services.UpnpResult.get_Message, MPServerProvider.Core.Services.UpnpResult.get_ExternalIp

### `op_Inequality(class:104, class:104)` -> `bool`
- token `0x060000f0`  flags=`2198`  rva=`32564`
- IL size 11 bytes, 6 instructions, max_stack=8
- opcodes: callx1, ceqx1, ldarg.0x1, ldarg.1x1, ldc.i4.0x1, retx1
- calls: MPServerProvider.Core.Services.UpnpResult.op_Equality

### `op_Equality(class:104, class:104)` -> `bool`
- token `0x060000f1`  flags=`2198`  rva=`32576`
- IL size 19 bytes, 13 instructions, max_stack=8
- opcodes: ldarg.0x3, retx3, ldarg.1x2, beq.sx1, brfalse.sx1, callvirtx1, ldc.i4.0x1, ldc.i4.1x1
- calls: MPServerProvider.Core.Services.UpnpResult.Equals

### `GetHashCode()` -> `i4`
- token `0x060000f2`  flags=`198`  rva=`32596`
- IL size 86 bytes, 26 instructions, max_stack=3
- opcodes: callx5, callvirtx4, ldarg.0x4, addx3, ldc.i4x3, ldfldx3, mulx3, retx1
- calls: System.Collections.Generic.EqualityComparer`1.get_Default, MPServerProvider.Core.Services.UpnpResult.get_EqualityContract, System.Collections.Generic.EqualityComparer`1.GetHashCode

### `Equals(object)` -> `bool`
- token `0x060000f3`  flags=`198`  rva=`32694`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, isinstx1, ldarg.0x1, ldarg.1x1, retx1
- calls: MPServerProvider.Core.Services.UpnpResult.Equals

### `Equals(class:104)` -> `bool`
- token `0x060000f4`  flags=`486`  rva=`32708`
- IL size 101 bytes, 36 instructions, max_stack=3
- opcodes: ldarg.1x6, ldfldx6, callx5, ldarg.0x5, brfalse.sx4, callvirtx4, retx3, beq.sx1, ldc.i4.0x1, ldc.i4.1x1
- calls: MPServerProvider.Core.Services.UpnpResult.get_EqualityContract, System.Type.op_Equality, System.Collections.Generic.EqualityComparer`1.get_Default, System.Collections.Generic.EqualityComparer`1.Equals

### `<Clone>$()` -> `class:104`
- token `0x060000f5`  flags=`134`  rva=`32821`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, newobjx1, retx1
- calls: MPServerProvider.Core.Services.UpnpResult..ctor

### `.ctor(class:104)` -> `void`
- token `0x060000f6`  flags=`6273`  rva=`32829`
- IL size 43 bytes, 15 instructions, max_stack=8
- opcodes: ldarg.0x4, ldarg.1x3, ldfldx3, stfldx3, callx1, retx1
- calls: System.Object..ctor

### `Deconstruct(bool&, string&, class:429&)` -> `void`
- token `0x060000f7`  flags=`134`  rva=`32873`
- IL size 25 bytes, 13 instructions, max_stack=8
- opcodes: callx3, ldarg.0x3, stind.refx2, ldarg.1x1, ldarg.2x1, ldarg.3x1, retx1, stind.i1x1
- calls: MPServerProvider.Core.Services.UpnpResult.get_Success, MPServerProvider.Core.Services.UpnpResult.get_Message, MPServerProvider.Core.Services.UpnpResult.get_ExternalIp

