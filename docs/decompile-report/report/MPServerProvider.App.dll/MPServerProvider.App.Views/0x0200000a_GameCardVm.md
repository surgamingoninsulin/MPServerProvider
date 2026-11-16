# MPServerProvider.App.Views.GameCardVm

Token: `0x0200000a`  Flags: `1048577`  Extends: `0x01000063`

## Fields

- `OverrideExtensions` : `string[]`  (flags=49, token=0x04000051)
- `AppLogo` : `class:401`  (flags=49, token=0x04000052)
- `<Game>k__BackingField` : `class:281`  (flags=33, token=0x04000053)
- `LogoTileBrush` : `class:405`  (flags=49, token=0x04000054)
- `_cover` : `class:417`  (flags=1, token=0x04000055)

## Methods

### `LoadAppLogo()` -> `class:401`
- token `0x0600005b`  flags=`145`  rva=`13320`
- IL size 47 bytes, 15 instructions, max_stack=8
- opcodes: callvirtx5, dupx5, newobjx2, ldc.i4.1x1, ldstrx1, retx1
- string literals: "pack://application:,,,/Assets/logo.png"
- calls: System.Windows.Media.Imaging.BitmapImage..ctor, System.Windows.Media.Imaging.BitmapImage.BeginInit, System.Uri..ctor, System.Windows.Media.Imaging.BitmapImage.set_UriSource, System.Windows.Media.Imaging.BitmapImage.set_CacheOption, System.Windows.Media.Imaging.BitmapImage.EndInit, System.Windows.Freezable.Freeze

### `get_Game()` -> `class:281`
- token `0x0600005c`  flags=`2182`  rva=`13368`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `Freeze(class:405)` -> `class:405`
- token `0x0600005d`  flags=`145`  rva=`13376`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x2, callvirtx1, retx1
- calls: System.Windows.Freezable.Freeze

### `.ctor(class:281)` -> `void`
- token `0x0600005e`  flags=`6278`  rva=`13385`
- IL size 21 bytes, 9 instructions, max_stack=8
- opcodes: ldarg.0x3, callx2, ldarg.1x1, popx1, retx1, stfldx1
- calls: CommunityToolkit.Mvvm.ComponentModel.ObservableObject..ctor, MPServerProvider.App.Views.GameCardVm.LoadCoverAsync

### `get_Name()` -> `string`
- token `0x0600005f`  flags=`2182`  rva=`13407`
- IL size 28 bytes, 13 instructions, max_stack=8
- opcodes: brtrue.sx2, callx2, dupx2, popx2, br.sx1, ldarg.0x1, ldnullx1, ldstrx1, retx1
- string literals: "Custom Game Server"
- calls: MPServerProvider.App.Views.GameCardVm.get_Game, MPServerProvider.Core.Models.GameDefinition.get_Name

### `get_GenreText()` -> `string`
- token `0x06000060`  flags=`2182`  rva=`13436`
- IL size 36 bytes, 11 instructions, max_stack=8
- opcodes: callx3, ldarg.0x2, ldstrx2, retx2, brfalse.sx1, callvirtx1
- string literals: "  •  ", ""
- calls: MPServerProvider.App.Views.GameCardVm.get_Game, MPServerProvider.Core.Models.GameDefinition.get_Genres, System.String.Join

### `get_HasCover()` -> `bool`
- token `0x06000061`  flags=`2182`  rva=`13473`
- IL size 13 bytes, 7 instructions, max_stack=8
- opcodes: ceqx2, callx1, ldarg.0x1, ldc.i4.0x1, ldnullx1, retx1
- calls: MPServerProvider.App.Views.GameCardVm.get_Cover

### `get_CoverStretch()` -> `valuetype:421`
- token `0x06000062`  flags=`2182`  rva=`13488`
- IL size 68 bytes, 28 instructions, max_stack=2
- opcodes: ldloc.0x4, callvirtx3, conv.r8x2, ldc.r8x2, ldloc.1x2, retx2, bge.un.sx1, ble.sx1, ble.un.sx1, brfalse.sx1, callx1, divx1, isinstx1, ldarg.0x1, ldc.i4.0x1, ldc.i4.2x1, ldc.i4.3x1, stloc.0x1, stloc.1x1
- calls: MPServerProvider.App.Views.GameCardVm.get_Cover, System.Windows.Media.Imaging.BitmapSource.get_PixelHeight, System.Windows.Media.Imaging.BitmapSource.get_PixelWidth

### `get_TileBackground()` -> `class:405`
- token `0x06000063`  flags=`2182`  rva=`13568`
- IL size 26 bytes, 9 instructions, max_stack=8
- opcodes: callx2, ldarg.0x2, ldsfldx2, retx2, beq.sx1
- calls: MPServerProvider.App.Views.GameCardVm.get_Cover, MPServerProvider.App.Views.GameCardVm.get_AccentBrush

### `get_AccentBrush()` -> `class:405`
- token `0x06000064`  flags=`2182`  rva=`13596`
- IL size 106 bytes, 38 instructions, max_stack=4
- opcodes: callx8, ldc.i4.sx4, ldloca.sx3, popx3, brtrue.sx2, dupx2, leave.sx2, newobjx2, stloc.1x2, br.sx1, callvirtx1, ldarg.0x1, ldc.i4x1, ldloc.1x1, ldnullx1, ldstrx1, retx1, stloc.0x1, unbox.anyx1
- string literals: "#4F6BFF"
- calls: MPServerProvider.App.Views.GameCardVm.get_Game, MPServerProvider.Core.Models.GameDefinition.get_Artwork, MPServerProvider.Core.Models.ArtworkSpec.get_Accent, System.Windows.Media.ColorConverter.ConvertFromString, System.Windows.Media.Color.get_R, System.Windows.Media.Color.get_G, System.Windows.Media.Color.get_B, System.Windows.Media.Color.FromArgb, System.Windows.Media.SolidColorBrush..ctor

### `LoadCoverAsync()` -> `class:433`
- token `0x06000065`  flags=`129`  rva=`13732`
- IL size 55 bytes, 17 instructions, max_stack=2
- opcodes: ldloca.sx6, callx3, stfldx3, ldfldax2, ldarg.0x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `TrySetCover(string)` -> `bool`
- token `0x06000066`  flags=`129`  rva=`13800`
- IL size 72 bytes, 30 instructions, max_stack=2
- opcodes: ldloc.0x7, callvirtx6, ldc.i4.1x2, leave.sx2, newobjx2, stloc.1x2, callx1, ldarg.0x1, ldarg.1x1, ldc.i4x1, ldc.i4.0x1, ldloc.1x1, popx1, retx1, stloc.0x1
- calls: System.Windows.Media.Imaging.BitmapImage..ctor, System.Windows.Media.Imaging.BitmapImage.BeginInit, System.Uri..ctor, System.Windows.Media.Imaging.BitmapImage.set_UriSource, System.Windows.Media.Imaging.BitmapImage.set_CacheOption, System.Windows.Media.Imaging.BitmapImage.set_DecodePixelWidth, System.Windows.Media.Imaging.BitmapImage.EndInit, System.Windows.Freezable.Freeze, MPServerProvider.App.Views.GameCardVm.set_Cover

### `get_Cover()` -> `class:417`
- token `0x06000067`  flags=`2182`  rva=`13900`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Cover(class:417)` -> `void`
- token `0x06000068`  flags=`2182`  rva=`13908`
- IL size 115 bytes, 34 instructions, max_stack=3
- opcodes: ldarg.0x10, callvirtx9, ldsfldx8, ldarg.1x2, brtrue.sx1, callx1, ldfldx1, retx1, stfldx1
- calls: System.Collections.Generic.EqualityComparer`1.get_Default, System.Collections.Generic.EqualityComparer`1.Equals, CommunityToolkit.Mvvm.ComponentModel.ObservableObject.OnPropertyChanging, CommunityToolkit.Mvvm.ComponentModel.ObservableObject.OnPropertyChanged

### `.cctor()` -> `void`
- token `0x06000069`  flags=`6289`  rva=`14036`
- IL size 95 bytes, 37 instructions, max_stack=4
- opcodes: dupx6, ldstrx6, stelem.refx6, callx3, stsfldx3, ldc.i4.sx2, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.4x1, ldc.i4.5x1, ldc.i4.6x1, ldc.i4.7x1, newarrx1, newobjx1, retx1
- string literals: ".png", ".jpg", ".jpeg", ".webp", ".bmp", ".gif"
- calls: MPServerProvider.App.Views.GameCardVm.LoadAppLogo, System.Windows.Media.Color.FromRgb, System.Windows.Media.SolidColorBrush..ctor, MPServerProvider.App.Views.GameCardVm.Freeze

