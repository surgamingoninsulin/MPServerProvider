# MPServerProvider.App.Views.ServerListItemVm

Token: `0x02000015`  Flags: `1048577`  Extends: `0x01000063`

## Fields

- `<Server>k__BackingField` : `class:497`  (flags=33, token=0x040000ad)
- `_statusBrush` : `class:405`  (flags=1, token=0x040000ae)
- `_subtitle` : `string`  (flags=1, token=0x040000af)

## Methods

### `get_Server()` -> `class:497`
- token `0x060000db`  flags=`2182`  rva=`24212`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `.ctor(class:497)` -> `void`
- token `0x060000dc`  flags=`6278`  rva=`24220`
- IL size 42 bytes, 14 instructions, max_stack=8
- opcodes: ldarg.0x5, callx3, stfldx3, ldarg.1x1, ldstrx1, retx1
- string literals: ""
- calls: System.Windows.Media.Brushes.get_Gray, CommunityToolkit.Mvvm.ComponentModel.ObservableObject..ctor, MPServerProvider.App.Views.ServerListItemVm.Refresh

### `get_Name()` -> `string`
- token `0x060000dd`  flags=`2182`  rva=`24263`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.ServerListItemVm.get_Server, MPServerProvider.Core.Models.ServerInstance.get_ServerName

### `Refresh()` -> `void`
- token `0x060000de`  flags=`134`  rva=`24276`
- IL size 102 bytes, 35 instructions, max_stack=4
- opcodes: callx11, ldarg.0x5, callvirtx4, brtrue.sx2, dupx2, ldloc.0x2, popx2, br.sx1, ldloc.1x1, ldnullx1, ldstrx1, retx1, stloc.0x1, stloc.1x1
- string literals: " — "
- calls: MPServerProvider.App.AppServices.get_Processes, MPServerProvider.App.Views.ServerListItemVm.get_Server, MPServerProvider.Core.Services.ServerProcessService.StateOf, MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Models.ServerInstance.get_ServerGame, MPServerProvider.Core.Services.GameLibraryService.Find, MPServerProvider.Core.Models.GameDefinition.get_Name, MPServerProvider.App.Views.ServerListItemVm.StatusLabel, System.String.Concat, MPServerProvider.App.Views.ServerListItemVm.set_Subtitle, MPServerProvider.App.Views.ServerListItemVm.StatusBrushFor, MPServerProvider.App.Views.ServerListItemVm.set_StatusBrush

### `StatusLabel(valuetype:589)` -> `string`
- token `0x060000df`  flags=`150`  rva=`24392`
- IL size 78 bytes, 24 instructions, max_stack=2
- opcodes: br.sx6, ldstrx6, stloc.0x6, ldarg.0x1, ldc.i4.1x1, ldloc.0x1, retx1, subx1, switchx1
- string literals: "online", "starting", "stopping", "crashed", "installing", "offline"

### `StatusBrushFor(valuetype:589)` -> `class:405`
- token `0x060000e0`  flags=`150`  rva=`24484`
- IL size 140 bytes, 41 instructions, max_stack=3
- opcodes: ldc.i4.sx10, br.sx5, callx5, ldc.i4x5, newobjx5, stloc.0x5, ldarg.0x1, ldc.i4.1x1, ldloc.0x1, retx1, subx1, switchx1
- calls: System.Windows.Media.Color.FromRgb, System.Windows.Media.SolidColorBrush..ctor

### `get_StatusBrush()` -> `class:405`
- token `0x060000e1`  flags=`2182`  rva=`24636`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_StatusBrush(class:405)` -> `void`
- token `0x060000e2`  flags=`2182`  rva=`24644`
- IL size 49 bytes, 16 instructions, max_stack=8
- opcodes: ldarg.0x4, callvirtx3, ldarg.1x2, ldsfldx2, brtrue.sx1, callx1, ldfldx1, retx1, stfldx1
- calls: System.Collections.Generic.EqualityComparer`1.get_Default, System.Collections.Generic.EqualityComparer`1.Equals, CommunityToolkit.Mvvm.ComponentModel.ObservableObject.OnPropertyChanging, CommunityToolkit.Mvvm.ComponentModel.ObservableObject.OnPropertyChanged

### `get_Subtitle()` -> `string`
- token `0x060000e3`  flags=`2182`  rva=`24694`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Subtitle(string)` -> `void`
- token `0x060000e4`  flags=`2182`  rva=`24702`
- IL size 49 bytes, 16 instructions, max_stack=8
- opcodes: ldarg.0x4, callvirtx3, ldarg.1x2, ldsfldx2, brtrue.sx1, callx1, ldfldx1, retx1, stfldx1
- calls: System.Collections.Generic.EqualityComparer`1.get_Default, System.Collections.Generic.EqualityComparer`1.Equals, CommunityToolkit.Mvvm.ComponentModel.ObservableObject.OnPropertyChanging, CommunityToolkit.Mvvm.ComponentModel.ObservableObject.OnPropertyChanged

