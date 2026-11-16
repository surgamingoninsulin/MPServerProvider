# MPServerProvider.Core.Models.InstallProgress

Token: `0x02000032`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `<Status>k__BackingField` : `string`  (flags=1, token=0x040000b4)
- `<Percent>k__BackingField` : `r8`  (flags=1, token=0x040000b5)
- `<BytesReceived>k__BackingField` : `i8`  (flags=1, token=0x040000b6)
- `<TotalBytes>k__BackingField` : `valuetype:261<i8>`  (flags=1, token=0x040000b7)

## Methods

### `get_Status()` -> `string`
- token `0x060001db`  flags=`2182`  rva=`41562`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Status(string)` -> `void`
- token `0x060001dc`  flags=`2182`  rva=`41570`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Percent()` -> `r8`
- token `0x060001dd`  flags=`2182`  rva=`41579`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Percent(r8)` -> `void`
- token `0x060001de`  flags=`2182`  rva=`41587`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_BytesReceived()` -> `i8`
- token `0x060001df`  flags=`2182`  rva=`41596`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_BytesReceived(i8)` -> `void`
- token `0x060001e0`  flags=`2182`  rva=`41604`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_TotalBytes()` -> `valuetype:261<i8>`
- token `0x060001e1`  flags=`2182`  rva=`41613`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_TotalBytes(valuetype:261<i8>)` -> `void`
- token `0x060001e2`  flags=`2182`  rva=`41621`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `Indeterminate(string)` -> `class:200`
- token `0x060001e3`  flags=`150`  rva=`41630`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callvirtx1, dupx1, ldarg.0x1, newobjx1, retx1
- calls: MPServerProvider.Core.Models.InstallProgress..ctor, MPServerProvider.Core.Models.InstallProgress.set_Status

### `At(string, r8)` -> `class:200`
- token `0x060001e4`  flags=`150`  rva=`41644`
- IL size 20 bytes, 8 instructions, max_stack=8
- opcodes: callvirtx2, dupx2, ldarg.0x1, ldarg.1x1, newobjx1, retx1
- calls: MPServerProvider.Core.Models.InstallProgress..ctor, MPServerProvider.Core.Models.InstallProgress.set_Status, MPServerProvider.Core.Models.InstallProgress.set_Percent

### `.ctor()` -> `void`
- token `0x060001e5`  flags=`6278`  rva=`41665`
- IL size 33 bytes, 9 instructions, max_stack=8
- opcodes: ldarg.0x3, stfldx2, callx1, ldc.r8x1, ldstrx1, retx1
- string literals: ""
- calls: System.Object..ctor

