# SteamProgress

Token: `0x0200009d`  Flags: `1048842`  Extends: `0x0100001f`

## Fields

- `<Percent>k__BackingField` : `r8`  (flags=33, token=0x0400035b)
- `<State>k__BackingField` : `string`  (flags=33, token=0x0400035c)
- `<Done>k__BackingField` : `i8`  (flags=33, token=0x0400035d)
- `<Total>k__BackingField` : `i8`  (flags=33, token=0x0400035e)

## Methods

### `.ctor(r8, string, i8, i8)` -> `void`
- token `0x0600037d`  flags=`6278`  rva=`88907`
- IL size 30 bytes, 13 instructions, max_stack=8
- opcodes: ldarg.0x4, stfldx4, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldarg.sx1, retx1

### `get_Percent()` -> `r8`
- token `0x0600037e`  flags=`2182`  rva=`88938`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Percent(r8)` -> `void modreq:177`
- token `0x0600037f`  flags=`2182`  rva=`88946`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_State()` -> `string`
- token `0x06000380`  flags=`2182`  rva=`88955`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_State(string)` -> `void modreq:177`
- token `0x06000381`  flags=`2182`  rva=`88963`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Done()` -> `i8`
- token `0x06000382`  flags=`2182`  rva=`88972`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Done(i8)` -> `void modreq:177`
- token `0x06000383`  flags=`2182`  rva=`88980`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Total()` -> `i8`
- token `0x06000384`  flags=`2182`  rva=`88989`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Total(i8)` -> `void modreq:177`
- token `0x06000385`  flags=`2182`  rva=`88997`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `ToString()` -> `string`
- token `0x06000386`  flags=`198`  rva=`89008`
- IL size 64 bytes, 25 instructions, max_stack=2
- opcodes: ldloc.0x6, callvirtx5, popx4, ldc.i4.sx2, ldstrx2, brfalse.sx1, callx1, ldarg.0x1, newobjx1, retx1, stloc.0x1
- string literals: "SteamProgress", " { "
- calls: System.Text.StringBuilder..ctor, System.Text.StringBuilder.Append, SteamProgress.PrintMembers, System.Object.ToString

### `PrintMembers(class:201)` -> `bool`
- token `0x06000387`  flags=`129`  rva=`89084`
- IL size 144 bytes, 50 instructions, max_stack=2
- opcodes: callvirtx11, ldarg.1x8, popx8, callx4, ldarg.0x4, ldstrx4, constrained.x3, ldloca.sx3, stloc.1x2, ldc.i4.1x1, retx1, stloc.0x1
- string literals: "Percent = ", ", State = ", ", Done = ", ", Total = "
- calls: System.Text.StringBuilder.Append, SteamProgress.get_Percent, System.Object.ToString, SteamProgress.get_State, SteamProgress.get_Done, SteamProgress.get_Total

### `op_Inequality(valuetype:628, valuetype:628)` -> `bool`
- token `0x06000388`  flags=`2198`  rva=`89240`
- IL size 11 bytes, 6 instructions, max_stack=8
- opcodes: callx1, ceqx1, ldarg.0x1, ldarg.1x1, ldc.i4.0x1, retx1
- calls: SteamProgress.op_Equality

### `op_Equality(valuetype:628, valuetype:628)` -> `bool`
- token `0x06000389`  flags=`2198`  rva=`89252`
- IL size 9 bytes, 4 instructions, max_stack=8
- opcodes: callx1, ldarg.1x1, ldarga.sx1, retx1
- calls: SteamProgress.Equals

### `GetHashCode()` -> `i4`
- token `0x0600038a`  flags=`198`  rva=`89264`
- IL size 86 bytes, 26 instructions, max_stack=3
- opcodes: callx4, callvirtx4, ldarg.0x4, ldfldx4, addx3, ldc.i4x3, mulx3, retx1
- calls: System.Collections.Generic.EqualityComparer`1.get_Default, System.Collections.Generic.EqualityComparer`1.GetHashCode

### `Equals(object)` -> `bool`
- token `0x0600038b`  flags=`198`  rva=`89362`
- IL size 23 bytes, 10 instructions, max_stack=8
- opcodes: ldarg.1x2, retx2, brfalse.sx1, callx1, isinstx1, ldarg.0x1, ldc.i4.0x1, unbox.anyx1
- calls: SteamProgress.Equals

### `Equals(valuetype:628)` -> `bool`
- token `0x0600038c`  flags=`486`  rva=`89388`
- IL size 97 bytes, 30 instructions, max_stack=3
- opcodes: ldfldx8, callx4, callvirtx4, ldarg.0x4, ldarg.1x4, brfalse.sx3, retx2, ldc.i4.0x1
- calls: System.Collections.Generic.EqualityComparer`1.get_Default, System.Collections.Generic.EqualityComparer`1.Equals

### `Deconstruct(r8&, string&, i8&, i8&)` -> `void`
- token `0x0600038d`  flags=`134`  rva=`89497`
- IL size 34 bytes, 17 instructions, max_stack=8
- opcodes: callx4, ldarg.0x4, stind.i8x2, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldarg.sx1, retx1, stind.r8x1, stind.refx1
- calls: SteamProgress.get_Percent, SteamProgress.get_State, SteamProgress.get_Done, SteamProgress.get_Total

