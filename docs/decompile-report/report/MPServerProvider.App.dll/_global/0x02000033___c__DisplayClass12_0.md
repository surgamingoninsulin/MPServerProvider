# <>c__DisplayClass12_0

Token: `0x02000033`  Flags: `1048835`  Extends: `0x0100001a`

## Fields

- `name` : `string`  (flags=6, token=0x0400017f)
- `gameId` : `string`  (flags=6, token=0x04000180)
- `folderName` : `string`  (flags=6, token=0x04000181)

## Methods

### `.ctor()` -> `void`
- token `0x0600018c`  flags=`6278`  rva=`47893`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<OnImport>b__0(class:281)` -> `bool`
- token `0x0600018d`  flags=`131`  rva=`47901`
- IL size 18 bytes, 6 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, ldarg.1x1, ldfldx1, retx1
- calls: MPServerProvider.Core.Models.GameDefinition.get_Name, System.String.op_Equality

### `<OnImport>b__1()` -> `i4`
- token `0x0600018e`  flags=`131`  rva=`47920`
- IL size 32 bytes, 14 instructions, max_stack=3
- opcodes: ldarg.0x2, ldfldx2, leave.sx2, stloc.0x2, callx1, callvirtx1, ldc.i4.m1x1, ldloc.0x1, popx1, retx1
- calls: MPServerProvider.App.AppServices.get_Mods, MPServerProvider.Core.Services.ModLibraryService.ImportFromFolder

