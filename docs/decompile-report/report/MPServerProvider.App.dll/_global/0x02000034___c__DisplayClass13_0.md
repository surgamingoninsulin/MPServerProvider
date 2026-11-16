# <>c__DisplayClass13_0

Token: `0x02000034`  Flags: `1048835`  Extends: `0x0100001a`

## Fields

- `vm` : `class:76`  (flags=6, token=0x04000182)
- `<>4__this` : `class:80`  (flags=6, token=0x04000183)

## Methods

### `.ctor()` -> `void`
- token `0x0600018f`  flags=`6278`  rva=`47980`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<OnAddToServer>b__0(class:497)` -> `bool`
- token `0x06000190`  flags=`131`  rva=`47988`
- IL size 46 bytes, 16 instructions, max_stack=8
- opcodes: callvirtx4, callx2, ldarg.1x2, ldc.i4.0x2, retx2, brfalse.sx1, ceqx1, ldarg.0x1, ldfldx1
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerGame, MPServerProvider.App.Views.ModCardVm.get_Mod, MPServerProvider.Core.Models.ModItem.get_GameId, System.String.op_Equality, MPServerProvider.Core.Models.ServerInstance.get_ModsFolder, System.String.IsNullOrWhiteSpace

