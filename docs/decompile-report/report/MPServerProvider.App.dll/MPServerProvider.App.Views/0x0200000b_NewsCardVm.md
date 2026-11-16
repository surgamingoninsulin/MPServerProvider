# MPServerProvider.App.Views.NewsCardVm

Token: `0x0200000b`  Flags: `1048833`  Extends: `0x0100001a`

## Fields

- `<Item>k__BackingField` : `class:445`  (flags=33, token=0x04000056)

## Methods

### `get_Item()` -> `class:445`
- token `0x0600006a`  flags=`2182`  rva=`14143`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Item(class:445)` -> `void modreq:257`
- token `0x0600006b`  flags=`2182`  rva=`14151`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Tag()` -> `string`
- token `0x0600006c`  flags=`2182`  rva=`14160`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.NewsCardVm.get_Item, MPServerProvider.Core.Models.NewsItem.get_Tag

### `get_Title()` -> `string`
- token `0x0600006d`  flags=`2182`  rva=`14173`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.NewsCardVm.get_Item, MPServerProvider.Core.Models.NewsItem.get_Title

### `get_Summary()` -> `string`
- token `0x0600006e`  flags=`2182`  rva=`14186`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.NewsCardVm.get_Item, MPServerProvider.Core.Models.NewsItem.get_Summary

### `get_Image()` -> `string`
- token `0x0600006f`  flags=`2182`  rva=`14199`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.NewsCardVm.get_Item, MPServerProvider.Core.Models.NewsItem.get_Image

### `get_HasImage()` -> `bool`
- token `0x06000070`  flags=`2182`  rva=`14212`
- IL size 20 bytes, 7 instructions, max_stack=8
- opcodes: callx2, callvirtx1, ceqx1, ldarg.0x1, ldc.i4.0x1, retx1
- calls: MPServerProvider.App.Views.NewsCardVm.get_Item, MPServerProvider.Core.Models.NewsItem.get_Image, System.String.IsNullOrWhiteSpace

### `get_CardCursor()` -> `class:449`
- token `0x06000071`  flags=`2182`  rva=`14233`
- IL size 30 bytes, 9 instructions, max_stack=8
- opcodes: callx4, retx2, brtrue.sx1, callvirtx1, ldarg.0x1
- calls: MPServerProvider.App.Views.NewsCardVm.get_Item, MPServerProvider.Core.Models.NewsItem.get_GameId, System.String.IsNullOrWhiteSpace, System.Windows.Input.Cursors.get_Hand, System.Windows.Input.Cursors.get_Arrow

### `get_AgeText()` -> `string`
- token `0x06000072`  flags=`2182`  rva=`14264`
- IL size 179 bytes, 70 instructions, max_stack=3
- opcodes: callx14, ldloca.sx10, br.sx6, ldloc.0x6, ldstrx5, stloc.2x5, ldc.i4.sx4, ldc.i4.1x3, callvirtx2, ldarg.0x2, beq.sx1, bge.sx1, ble.sx1, blt.sx1, conv.i4x1, divx1, ldc.i4.0x1, ldc.i4.7x1, ldloc.2x1, retx1, stloc.0x1, stloc.1x1, stloc.sx1
- string literals: "today", "1 day ago", " days ago", " weeks ago", "MMM yyyy"
- calls: System.DateTimeOffset.get_Now, MPServerProvider.App.Views.NewsCardVm.get_Item, MPServerProvider.Core.Models.NewsItem.get_Date, System.DateTimeOffset.op_Subtraction, System.TimeSpan.get_TotalDays, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.DateTimeOffset.ToString

### `.ctor()` -> `void`
- token `0x06000073`  flags=`6278`  rva=`14455`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

