# MPServerProvider.App.Views.FileEntryVm

Token: `0x02000017`  Flags: `1048833`  Extends: `0x0100001a`

## Fields

- `<Name>k__BackingField` : `string`  (flags=33, token=0x040000b9)
- `<FullPath>k__BackingField` : `string`  (flags=33, token=0x040000ba)
- `<IsDirectory>k__BackingField` : `bool`  (flags=33, token=0x040000bb)
- `<Size>k__BackingField` : `i8`  (flags=33, token=0x040000bc)
- `<Modified>k__BackingField` : `valuetype:473`  (flags=33, token=0x040000bd)

## Methods

### `get_Name()` -> `string`
- token `0x060000f3`  flags=`2182`  rva=`26907`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Name(string)` -> `void modreq:257`
- token `0x060000f4`  flags=`2182`  rva=`26915`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_FullPath()` -> `string`
- token `0x060000f5`  flags=`2182`  rva=`26924`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_FullPath(string)` -> `void modreq:257`
- token `0x060000f6`  flags=`2182`  rva=`26932`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_IsDirectory()` -> `bool`
- token `0x060000f7`  flags=`2182`  rva=`26941`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_IsDirectory(bool)` -> `void modreq:257`
- token `0x060000f8`  flags=`2182`  rva=`26949`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Size()` -> `i8`
- token `0x060000f9`  flags=`2182`  rva=`26958`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Size(i8)` -> `void modreq:257`
- token `0x060000fa`  flags=`2182`  rva=`26966`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Modified()` -> `valuetype:473`
- token `0x060000fb`  flags=`2182`  rva=`26975`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Modified(valuetype:473)` -> `void modreq:257`
- token `0x060000fc`  flags=`2182`  rva=`26983`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Icon()` -> `string`
- token `0x060000fd`  flags=`2182`  rva=`26992`
- IL size 20 bytes, 7 instructions, max_stack=8
- opcodes: ldstrx2, retx2, brtrue.sx1, callx1, ldarg.0x1
- string literals: "", ""
- calls: MPServerProvider.App.Views.FileEntryVm.get_IsDirectory

### `get_IconBrush()` -> `class:405`
- token `0x060000fe`  flags=`2182`  rva=`27013`
- IL size 57 bytes, 15 instructions, max_stack=8
- opcodes: ldc.i4x5, callx3, newobjx2, retx2, brtrue.sx1, ldarg.0x1, ldc.i4.sx1
- calls: MPServerProvider.App.Views.FileEntryVm.get_IsDirectory, System.Windows.Media.Color.FromRgb, System.Windows.Media.SolidColorBrush..ctor

### `get_SizeText()` -> `string`
- token `0x060000ff`  flags=`2182`  rva=`27071`
- IL size 26 bytes, 9 instructions, max_stack=8
- opcodes: callx3, ldarg.0x2, retx2, brtrue.sx1, ldstrx1
- string literals: ""
- calls: MPServerProvider.App.Views.FileEntryVm.get_IsDirectory, MPServerProvider.App.Views.FileEntryVm.get_Size, MPServerProvider.Core.Services.DownloadService.FormatBytes

### `get_ModifiedText()` -> `string`
- token `0x06000100`  flags=`2182`  rva=`27100`
- IL size 20 bytes, 7 instructions, max_stack=2
- opcodes: callx2, ldarg.0x1, ldloca.sx1, ldstrx1, retx1, stloc.0x1
- string literals: "yyyy-MM-dd HH:mm"
- calls: MPServerProvider.App.Views.FileEntryVm.get_Modified, System.DateTime.ToString

### `.ctor()` -> `void`
- token `0x06000101`  flags=`6278`  rva=`27132`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

