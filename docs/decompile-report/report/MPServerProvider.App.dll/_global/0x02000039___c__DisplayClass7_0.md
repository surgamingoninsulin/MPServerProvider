# <>c__DisplayClass7_0

Token: `0x02000039`  Flags: `1048835`  Extends: `0x0100001a`

## Fields

- `selectedGame` : `string`  (flags=6, token=0x0400018d)
- `<>4__this` : `class:80`  (flags=6, token=0x0400018e)
- `keyword` : `string`  (flags=6, token=0x0400018f)

## Methods

### `.ctor()` -> `void`
- token `0x0600019a`  flags=`6278`  rva=`48610`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<Refresh>b__3(class:281)` -> `bool`
- token `0x0600019b`  flags=`131`  rva=`48618`
- IL size 18 bytes, 6 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, ldarg.1x1, ldfldx1, retx1
- calls: MPServerProvider.Core.Models.GameDefinition.get_Name, System.String.op_Equality

### `<Refresh>b__0(class:569)` -> `bool`
- token `0x0600019c`  flags=`131`  rva=`48637`
- IL size 26 bytes, 9 instructions, max_stack=8
- opcodes: callvirtx2, ldfldx2, ceqx1, ldarg.0x1, ldarg.1x1, ldc.i4.0x1, retx1
- calls: MPServerProvider.Core.Models.ModItem.get_GameId, System.Collections.Generic.HashSet`1.Contains

### `<Refresh>b__1(class:569)` -> `bool`
- token `0x0600019d`  flags=`131`  rva=`48664`
- IL size 34 bytes, 13 instructions, max_stack=8
- opcodes: callvirtx3, ldarg.0x2, ldfldx2, retx2, brfalse.sx1, ldarg.1x1, ldc.i4.1x1, ldc.i4.5x1
- calls: System.String.get_Length, MPServerProvider.Core.Models.ModItem.get_FileName, System.String.Contains

