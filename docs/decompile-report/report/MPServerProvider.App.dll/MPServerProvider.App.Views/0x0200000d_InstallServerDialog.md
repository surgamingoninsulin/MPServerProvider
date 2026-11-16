# MPServerProvider.App.Views.InstallServerDialog

Token: `0x0200000d`  Flags: `1048577`  Extends: `0x01000044`

## Fields

- `_game` : `class:281`  (flags=33, token=0x04000066)
- `_cts` : `class:517`  (flags=1, token=0x04000067)
- `<InstalledServer>k__BackingField` : `class:497`  (flags=1, token=0x04000068)
- `FormPanel` : `class:297`  (flags=3, token=0x04000069)
- `TitleText` : `class:389`  (flags=3, token=0x0400006a)
- `SubtitleText` : `class:389`  (flags=3, token=0x0400006b)
- `NameBox` : `class:301`  (flags=3, token=0x0400006c)
- `VariantPanel` : `class:297`  (flags=3, token=0x0400006d)
- `VariantCombo` : `class:321`  (flags=3, token=0x0400006e)
- `VersionCombo` : `class:321`  (flags=3, token=0x0400006f)
- `ManualFolderPanel` : `class:297`  (flags=3, token=0x04000070)
- `ManualFolderBox` : `class:301`  (flags=3, token=0x04000071)
- `PortBox` : `class:301`  (flags=3, token=0x04000072)
- `RamMinPanel` : `class:297`  (flags=3, token=0x04000073)
- `RamMinBox` : `class:301`  (flags=3, token=0x04000074)
- `RamMaxPanel` : `class:297`  (flags=3, token=0x04000075)
- `RamMaxBox` : `class:301`  (flags=3, token=0x04000076)
- `EulaCheck` : `class:309`  (flags=3, token=0x04000077)
- `NoteBox` : `class:317`  (flags=3, token=0x04000078)
- `NoteText` : `class:389`  (flags=3, token=0x04000079)
- `InstallButton` : `class:305`  (flags=3, token=0x0400007a)
- `ProgressPanel` : `class:297`  (flags=3, token=0x0400007b)
- `ProgressTitle` : `class:389`  (flags=3, token=0x0400007c)
- `ProgressStatus` : `class:389`  (flags=3, token=0x0400007d)
- `ProgressBarControl` : `class:521`  (flags=3, token=0x0400007e)
- `ProgressPercent` : `class:389`  (flags=3, token=0x0400007f)
- `CancelInstallButton` : `class:305`  (flags=3, token=0x04000080)
- `_contentLoaded` : `bool`  (flags=1, token=0x04000081)

## Methods

### `get_InstalledServer()` -> `class:497`
- token `0x0600008a`  flags=`2182`  rva=`15784`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_InstalledServer(class:497)` -> `void`
- token `0x0600008b`  flags=`2177`  rva=`15792`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `.ctor(class:281)` -> `void`
- token `0x0600008c`  flags=`6278`  rva=`15804`
- IL size 500 bytes, 169 instructions, max_stack=4
- opcodes: callvirtx40, ldarg.0x20, ldfldx15, ldarg.1x14, callx10, ldstrx9, brtrue.sx8, br.sx7, ldc.i4.0x7, ldc.i4.2x6, ldc.i4.5x4, ldloca.sx3, ceqx2, dupx2, ldloc.2x2, ldloc.sx2, popx2, retx2, stloc.sx2, ble.sx1, brfalse.sx1, initobjx1, ldc.i4.1x1, ldloc.0x1, ldloc.1x1, ldnullx1, stfldx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "Install ", "{ram_", "minecraft", "manual", "", "REQUIRED", "key", "Needs", "💡 "
- calls: System.Windows.Window..ctor, MPServerProvider.App.Views.InstallServerDialog.InitializeComponent, MPServerProvider.Core.Models.GameDefinition.get_Name, System.String.Concat, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.Core.Models.GameDefinition.get_Description, MPServerProvider.App.Views.InstallServerDialog.UniqueDefaultName, System.Windows.Controls.TextBox.set_Text, MPServerProvider.Core.Models.GameDefinition.get_Network, MPServerProvider.Core.Models.NetworkSpec.get_DefaultPort, System.Int32.ToString, MPServerProvider.Core.Models.GameDefinition.get_Runtime, MPServerProvider.Core.Models.RuntimeSpec.get_Arguments, System.String.Contains, MPServerProvider.Core.Models.RuntimeSpec.get_Requires, MPServerProvider.Core.Models.RequiresSpec.get_Java, System.Nullable`1.get_HasValue, System.Windows.UIElement.set_Visibility, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.StartsWith, MPServerProvider.Core.Models.GameDefinition.get_Install, MPServerProvider.Core.Models.InstallSpec.get_Provider, System.String.op_Equality, MPServerProvider.Core.Models.InstallSpec.get_Variants, System.Collections.Generic.List`1.get_Count, System.Windows.Controls.ItemsControl.set_ItemsSource, System.Windows.Controls.Primitives.Selector.set_SelectedIndex, MPServerProvider.App.Views.InstallServerDialog.LoadVersionsAsync

### `UniqueDefaultName()` -> `string`
- token `0x0600008d`  flags=`129`  rva=`16316`
- IL size 187 bytes, 65 instructions, max_stack=3
- opcodes: ldstrx11, callx8, callvirtx8, ldloca.sx5, ldloc.1x4, brtrue.sx3, ldloc.0x3, br.sx2, ldc.i4.1x2, ldc.i4.2x2, ldloc.2x2, ldloc.3x2, stloc.0x2, stloc.1x2, stloc.2x2, stloc.3x2, addx1, ldarg.0x1, ldc.i4.5x1, ldfldx1, retx1
- string literals: ":", "", "Edition", "", "  ", " ", "  ", "My ", "Server", " Server", " "
- calls: MPServerProvider.Core.Models.GameDefinition.get_Name, System.String.Replace, System.String.Trim, System.String.Contains, System.String.Concat, System.String.EndsWith, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.NameTaken

### `OnVariantChanged(object, class:529)` -> `void`
- token `0x0600008e`  flags=`129`  rva=`16516`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `LoadVersionsAsync(string)` -> `class:433`
- token `0x0600008f`  flags=`129`  rva=`16572`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `OnBrowseManualFolder(object, class:329)` -> `void`
- token `0x06000090`  flags=`129`  rva=`16648`
- IL size 52 bytes, 18 instructions, max_stack=3
- opcodes: callvirtx4, ldarg.0x2, ldloc.0x2, brfalse.sx1, callx1, dupx1, ldfldx1, ldloca.sx1, ldstrx1, newobjx1, retx1, stloc.0x1, stloc.1x1
- string literals: "Select server files folder"
- calls: Microsoft.Win32.OpenFolderDialog..ctor, Microsoft.Win32.CommonItemDialog.set_Title, Microsoft.Win32.CommonDialog.ShowDialog, System.Nullable`1.GetValueOrDefault, Microsoft.Win32.OpenFolderDialog.get_FolderName, System.Windows.Controls.TextBox.set_Text

### `OnInstall(object, class:329)` -> `void`
- token `0x06000091`  flags=`129`  rva=`16712`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `Humanize(string)` -> `string`
- token `0x06000092`  flags=`145`  rva=`16767`
- IL size 22 bytes, 9 instructions, max_stack=8
- opcodes: ldarg.0x2, ldstrx2, retx2, brtrue.sx1, callvirtx1, ldc.i4.5x1
- string literals: "type 'quit' to exit", "SteamCMD: press Cancel to exit"
- calls: System.String.Contains

### `get_Installing()` -> `bool`
- token `0x06000093`  flags=`2177`  rva=`16792`
- IL size 35 bytes, 16 instructions, max_stack=2
- opcodes: callvirtx2, ldarg.0x2, ldc.i4.0x2, ldfldx2, ldloc.0x2, retx2, brfalse.sx1, brtrue.sx1, ceqx1, stloc.0x1
- calls: System.Threading.CancellationTokenSource.get_IsCancellationRequested, System.Windows.UIElement.get_Visibility

### `OnCancel(object, class:329)` -> `void`
- token `0x06000094`  flags=`129`  rva=`16839`
- IL size 30 bytes, 13 instructions, max_stack=8
- opcodes: callx3, ldarg.0x3, retx2, brfalse.sx1, ldarg.1x1, ldarg.2x1, ldc.i4.0x1, newobjx1
- calls: MPServerProvider.App.Views.InstallServerDialog.get_Installing, MPServerProvider.App.Views.InstallServerDialog.OnCancelInstall, System.Nullable`1..ctor, System.Windows.Window.set_DialogResult

### `OnClosing(class:533)` -> `void`
- token `0x06000095`  flags=`196`  rva=`16870`
- IL size 36 bytes, 15 instructions, max_stack=8
- opcodes: ldarg.0x4, callx3, ldarg.1x2, retx2, brfalse.sx1, callvirtx1, ldc.i4.1x1, newobjx1
- calls: MPServerProvider.App.Views.InstallServerDialog.get_Installing, System.ComponentModel.CancelEventArgs.set_Cancel, System.Windows.RoutedEventArgs..ctor, MPServerProvider.App.Views.InstallServerDialog.OnCancelInstall, System.Windows.Window.OnClosing

### `OnCancelInstall(object, class:329)` -> `void`
- token `0x06000096`  flags=`129`  rva=`16907`
- IL size 45 bytes, 16 instructions, max_stack=8
- opcodes: ldarg.0x3, ldfldx3, callvirtx2, retx2, brtrue.sx1, callx1, dupx1, ldc.i4.0x1, ldstrx1, popx1
- string literals: "Exiting SteamCMD…"
- calls: System.Windows.UIElement.set_IsEnabled, System.Windows.Controls.ContentControl.set_Content, System.Threading.CancellationTokenSource.Cancel

### `InitializeComponent()` -> `void`
- token `0x06000097`  flags=`486`  rva=`16956`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/installserverdialog.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x06000098`  flags=`481`  rva=`17004`
- IL size 554 bytes, 161 instructions, max_stack=3
- opcodes: ldarg.0x33, retx27, castclassx26, ldarg.2x26, stfldx25, callvirtx5, ldftnx5, newobjx5, ldfldx3, ldc.i4.1x2, brx1, ldarg.1x1, subx1, switchx1
- calls: MPServerProvider.App.Views.InstallServerDialog.OnVariantChanged, System.Windows.Controls.SelectionChangedEventHandler..ctor, System.Windows.Controls.Primitives.Selector.add_SelectionChanged, MPServerProvider.App.Views.InstallServerDialog.OnBrowseManualFolder, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.InstallServerDialog.OnCancel, MPServerProvider.App.Views.InstallServerDialog.OnInstall, MPServerProvider.App.Views.InstallServerDialog.OnCancelInstall

### `<OnInstall>b__11_0(class:537)` -> `void`
- token `0x06000099`  flags=`129`  rva=`17572`
- IL size 155 bytes, 46 instructions, max_stack=4
- opcodes: callvirtx10, ldarg.0x6, ldfldx6, callx5, ldarg.1x4, ldloca.sx4, ldc.i4.1x3, ldstrx3, retx2, blt.un.sx1, ldc.i4.0x1, ldc.r8x1
- string literals: "0", "%", ""
- calls: MPServerProvider.Core.Models.InstallProgress.get_Status, MPServerProvider.App.Views.InstallServerDialog.Humanize, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.Core.Models.InstallProgress.get_Percent, System.Windows.Controls.ProgressBar.set_IsIndeterminate, System.Windows.Controls.Primitives.RangeBase.set_Value, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear

