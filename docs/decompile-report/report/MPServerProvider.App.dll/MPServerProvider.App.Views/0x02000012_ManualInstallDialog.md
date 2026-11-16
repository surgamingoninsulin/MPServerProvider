# MPServerProvider.App.Views.ManualInstallDialog

Token: `0x02000012`  Flags: `1048577`  Extends: `0x01000044`

## Fields

- `_game` : `class:281`  (flags=33, token=0x04000098)
- `_sourceFolder` : `string`  (flags=33, token=0x04000099)
- `<InstalledServer>k__BackingField` : `class:497`  (flags=1, token=0x0400009a)
- `TitleText` : `class:389`  (flags=3, token=0x0400009b)
- `SubtitleText` : `class:389`  (flags=3, token=0x0400009c)
- `NameBox` : `class:301`  (flags=3, token=0x0400009d)
- `PortBox` : `class:301`  (flags=3, token=0x0400009e)
- `FolderBox` : `class:301`  (flags=3, token=0x0400009f)
- `InstallButton` : `class:305`  (flags=3, token=0x040000a0)
- `_contentLoaded` : `bool`  (flags=1, token=0x040000a1)

## Methods

### `get_InstalledServer()` -> `class:497`
- token `0x060000be`  flags=`2182`  rva=`21116`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_InstalledServer(class:497)` -> `void`
- token `0x060000bf`  flags=`2177`  rva=`21124`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `.ctor(class:281, string)` -> `void`
- token `0x060000c0`  flags=`6278`  rva=`21136`
- IL size 140 bytes, 43 instructions, max_stack=4
- opcodes: ldarg.0x10, callvirtx8, callx6, ldfldx5, ldarg.1x3, ldarg.2x3, ldstrx3, stfldx2, ldloca.sx1, retx1, stloc.0x1
- string literals: "Install ", " from Folder", "Create a server using existing files from:
"
- calls: System.Windows.Window..ctor, MPServerProvider.App.Views.ManualInstallDialog.InitializeComponent, MPServerProvider.Core.Models.GameDefinition.get_Name, System.String.Concat, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.App.Views.ManualInstallDialog.UniqueDefaultName, System.Windows.Controls.TextBox.set_Text, MPServerProvider.Core.Models.GameDefinition.get_Network, MPServerProvider.Core.Models.NetworkSpec.get_DefaultPort, System.Int32.ToString

### `UniqueDefaultName()` -> `string`
- token `0x060000c1`  flags=`129`  rva=`21288`
- IL size 187 bytes, 65 instructions, max_stack=3
- opcodes: ldstrx11, callx8, callvirtx8, ldloca.sx5, ldloc.1x4, brtrue.sx3, ldloc.0x3, br.sx2, ldc.i4.1x2, ldc.i4.2x2, ldloc.2x2, ldloc.3x2, stloc.0x2, stloc.1x2, stloc.2x2, stloc.3x2, addx1, ldarg.0x1, ldc.i4.5x1, ldfldx1, retx1
- string literals: ":", "", "Edition", "", "  ", " ", "  ", "My ", "Server", " Server", " "
- calls: MPServerProvider.Core.Models.GameDefinition.get_Name, System.String.Replace, System.String.Trim, System.String.Contains, System.String.Concat, System.String.EndsWith, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.NameTaken

### `OnCancel(object, class:329)` -> `void`
- token `0x060000c2`  flags=`129`  rva=`21487`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldc.i4.0x1, newobjx1, retx1
- calls: System.Nullable`1..ctor, System.Windows.Window.set_DialogResult

### `OnInstall(object, class:329)` -> `void`
- token `0x060000c3`  flags=`129`  rva=`21504`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `InitializeComponent()` -> `void`
- token `0x060000c4`  flags=`486`  rva=`21560`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/manualinstalldialog.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x060000c5`  flags=`481`  rva=`21608`
- IL size 171 bytes, 52 instructions, max_stack=3
- opcodes: ldarg.0x10, retx8, castclassx7, ldarg.2x7, stfldx7, callvirtx2, ldc.i4.1x2, ldftnx2, newobjx2, br.sx1, ldarg.1x1, ldfldx1, subx1, switchx1
- calls: MPServerProvider.App.Views.ManualInstallDialog.OnCancel, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.ManualInstallDialog.OnInstall

