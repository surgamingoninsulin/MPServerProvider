# MPServerProvider.App.Views.LibraryModVm

Token: `0x02000018`  Flags: `1048833`  Extends: `0x0100001a`

## Fields

- `<Mod>k__BackingField` : `class:569`  (flags=33, token=0x040000be)

## Methods

### `get_Mod()` -> `class:569`
- token `0x06000102`  flags=`2182`  rva=`27140`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Mod(class:569)` -> `void modreq:257`
- token `0x06000103`  flags=`2182`  rva=`27148`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_FileName()` -> `string`
- token `0x06000104`  flags=`2182`  rva=`27157`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.LibraryModVm.get_Mod, MPServerProvider.Core.Models.ModItem.get_FileName

### `get_SizeText()` -> `string`
- token `0x06000105`  flags=`2182`  rva=`27170`
- IL size 17 bytes, 5 instructions, max_stack=8
- opcodes: callx2, callvirtx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.LibraryModVm.get_Mod, MPServerProvider.Core.Models.ModItem.get_SizeBytes, MPServerProvider.Core.Services.DownloadService.FormatBytes

### `.ctor()` -> `void`
- token `0x06000106`  flags=`6278`  rva=`27188`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

