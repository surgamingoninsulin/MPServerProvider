# MPServerProvider.App.Views.ModCardVm

Token: `0x02000013`  Flags: `1048833`  Extends: `0x0100001a`

## Fields

- `<Mod>k__BackingField` : `class:569`  (flags=33, token=0x040000a2)

## Methods

### `get_Mod()` -> `class:569`
- token `0x060000c6`  flags=`2182`  rva=`21791`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Mod(class:569)` -> `void modreq:257`
- token `0x060000c7`  flags=`2182`  rva=`21799`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_FileName()` -> `string`
- token `0x060000c8`  flags=`2182`  rva=`21808`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.ModCardVm.get_Mod, MPServerProvider.Core.Models.ModItem.get_FileName

### `get_GameName()` -> `string`
- token `0x060000c9`  flags=`2182`  rva=`21821`
- IL size 49 bytes, 18 instructions, max_stack=8
- opcodes: callx4, callvirtx3, brtrue.sx2, dupx2, ldarg.0x2, popx2, br.sx1, ldnullx1, retx1
- calls: MPServerProvider.App.AppServices.get_Games, MPServerProvider.App.Views.ModCardVm.get_Mod, MPServerProvider.Core.Models.ModItem.get_GameId, MPServerProvider.Core.Services.GameLibraryService.Find, MPServerProvider.Core.Models.GameDefinition.get_Name

### `get_DetailText()` -> `string`
- token `0x060000ca`  flags=`2182`  rva=`21872`
- IL size 144 bytes, 49 instructions, max_stack=3
- opcodes: callx11, callvirtx5, ldloca.sx5, ldstrx5, br.sx3, ldarg.0x3, ldloc.1x3, stloc.2x3, ldc.i4.1x2, beq.sx1, brfalse.sx1, ldc.i4.0x1, ldc.i4.sx1, ldloc.0x1, ldloc.2x1, retx1, stloc.0x1, stloc.1x1
- string literals: "not assigned", "on ", "on ", " servers", "  •  "
- calls: MPServerProvider.App.Views.ModCardVm.get_Mod, MPServerProvider.Core.Models.ModItem.get_AssignedServers, System.Collections.Generic.List`1.get_Count, System.Collections.Generic.List`1.get_Item, System.String.Concat, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.ModItem.get_SizeBytes, MPServerProvider.Core.Services.DownloadService.FormatBytes

### `.ctor()` -> `void`
- token `0x060000cb`  flags=`6278`  rva=`22028`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

