# MPServerProvider.App.Views.AddCustomGameDialog

Token: `0x02000008`  Flags: `1048577`  Extends: `0x01000044`

## Fields

- `<CreatedGame>k__BackingField` : `class:281`  (flags=1, token=0x04000021)
- `<ImportFolder>k__BackingField` : `string`  (flags=1, token=0x04000022)
- `_lastBrowseFolder` : `string`  (flags=17, token=0x04000023)
- `SteamHttp` : `class:285`  (flags=49, token=0x04000024)
- `_fetchedDescription` : `string`  (flags=1, token=0x04000025)
- `_fetchedGenres` : `class:289<string>`  (flags=33, token=0x04000026)
- `SourceJson` : `class:293`  (flags=3, token=0x04000027)
- `SourceSteam` : `class:293`  (flags=3, token=0x04000028)
- `SteamPanel` : `class:297`  (flags=3, token=0x04000029)
- `AppIdBox` : `class:301`  (flags=3, token=0x0400002a)
- `FetchButton` : `class:305`  (flags=3, token=0x0400002b)
- `AnonymousCheck` : `class:309`  (flags=3, token=0x0400002c)
- `CredentialsPanel` : `class:297`  (flags=3, token=0x0400002d)
- `SteamUserBox` : `class:301`  (flags=3, token=0x0400002e)
- `SteamPassBox` : `class:313`  (flags=3, token=0x0400002f)
- `WorkshopBox` : `class:301`  (flags=3, token=0x04000030)
- `JsonFieldsPanel` : `class:297`  (flags=3, token=0x04000031)
- `TypeNewBorder` : `class:317`  (flags=3, token=0x04000032)
- `TypeNew` : `class:293`  (flags=3, token=0x04000033)
- `TypeImportBorder` : `class:317`  (flags=3, token=0x04000034)
- `TypeImport` : `class:293`  (flags=3, token=0x04000035)
- `NameBox` : `class:301`  (flags=3, token=0x04000036)
- `IdBox` : `class:301`  (flags=3, token=0x04000037)
- `PortBox` : `class:301`  (flags=3, token=0x04000038)
- `ProtocolCombo` : `class:321`  (flags=3, token=0x04000039)
- `ExecPanel` : `class:297`  (flags=3, token=0x0400003a)
- `ExecBox` : `class:301`  (flags=3, token=0x0400003b)
- `ArgsBox` : `class:301`  (flags=3, token=0x0400003c)
- `StopBox` : `class:301`  (flags=3, token=0x0400003d)
- `ModTypeNone` : `class:293`  (flags=3, token=0x0400003e)
- `ModTypeMods` : `class:293`  (flags=3, token=0x0400003f)
- `ModTypePlugins` : `class:293`  (flags=3, token=0x04000040)
- `ModTypeAddons` : `class:293`  (flags=3, token=0x04000041)
- `ModsBox` : `class:301`  (flags=3, token=0x04000042)
- `FolderPickerPanel` : `class:297`  (flags=3, token=0x04000043)
- `FolderBox` : `class:301`  (flags=3, token=0x04000044)
- `ImageBox` : `class:301`  (flags=3, token=0x04000045)
- `AddButton` : `class:305`  (flags=3, token=0x04000046)
- `_contentLoaded` : `bool`  (flags=1, token=0x04000047)

## Methods

### `get_CreatedGame()` -> `class:281`
- token `0x06000033`  flags=`2182`  rva=`9577`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_CreatedGame(class:281)` -> `void`
- token `0x06000034`  flags=`2177`  rva=`9585`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_ImportFolder()` -> `string`
- token `0x06000035`  flags=`2182`  rva=`9594`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_ImportFolder(string)` -> `void`
- token `0x06000036`  flags=`2177`  rva=`9602`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `.ctor()` -> `void`
- token `0x06000037`  flags=`6278`  rva=`9612`
- IL size 164 bytes, 44 instructions, max_stack=3
- opcodes: ldarg.0x14, callvirtx7, ldfldx7, newobjx5, ldftnx4, ldstrx3, callx2, retx1, stfldx1
- string literals: "My Custom Game", "my-custom-game", "server.exe"
- calls: System.Collections.Generic.List`1..ctor, System.Windows.Window..ctor, MPServerProvider.App.Views.AddCustomGameDialog.InitializeComponent, System.Windows.Controls.TextBox.set_Text, MPServerProvider.App.Views.AddCustomGameDialog.<.ctor>b__9_0, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ToggleButton.add_Checked, MPServerProvider.App.Views.AddCustomGameDialog.<.ctor>b__9_1, MPServerProvider.App.Views.AddCustomGameDialog.<.ctor>b__9_2, MPServerProvider.App.Views.AddCustomGameDialog.<.ctor>b__9_3

### `get_IsSteamMode()` -> `bool`
- token `0x06000038`  flags=`2177`  rva=`9788`
- IL size 20 bytes, 7 instructions, max_stack=1
- opcodes: callx1, callvirtx1, ldarg.0x1, ldfldx1, ldloca.sx1, retx1, stloc.0x1
- calls: System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault

### `ToggleSource()` -> `void`
- token `0x06000039`  flags=`129`  rva=`9820`
- IL size 214 bytes, 73 instructions, max_stack=2
- opcodes: ldarg.0x17, ldfldx10, callvirtx9, callx7, br.sx6, brtrue.sx6, ldstrx5, ldc.i4.0x4, ldc.i4.2x4, retx2, brfalse.sx1, ldc.i4.1x1, newobjx1
- string literals: "My Steam Game", "27015", "25565", "my-custom-game", "my-steam-game"
- calls: MPServerProvider.App.Views.AddCustomGameDialog.get_IsSteamMode, System.Windows.UIElement.set_Visibility, System.Nullable`1..ctor, System.Windows.Controls.Primitives.ToggleButton.set_IsChecked, System.Windows.Controls.TextBox.set_Text, MPServerProvider.App.Views.AddCustomGameDialog.ToggleMode

### `OnAnonymousChanged(object, class:329)` -> `void`
- token `0x0600003a`  flags=`129`  rva=`10048`
- IL size 46 bytes, 18 instructions, max_stack=2
- opcodes: ldarg.0x3, ldfldx3, brtrue.sx2, callvirtx2, retx2, br.sx1, callx1, ldc.i4.0x1, ldc.i4.2x1, ldloca.sx1, stloc.0x1
- calls: System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, System.Windows.UIElement.set_Visibility

### `OnFetchSteamInfo(object, class:329)` -> `void`
- token `0x0600003b`  flags=`129`  rva=`10108`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `TryFillFromStoreAsync(i4)` -> `class:333<bool>`
- token `0x0600003c`  flags=`129`  rva=`10164`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `Slugify(string)` -> `string`
- token `0x0600003d`  flags=`145`  rva=`10240`
- IL size 94 bytes, 29 instructions, max_stack=3
- opcodes: callvirtx4, ldloc.0x3, ldstrx3, brtrue.sx2, callx2, dupx2, ldsfldx2, newobjx2, stloc.0x2, br.sx1, ldarg.0x1, ldc.i4.sx1, ldftnx1, popx1, retx1, stsfldx1
- string literals: "--", "-", "--"
- calls: System.String.ToLowerInvariant, <>c.<Slugify>b__19_0, System.Func`2..ctor, System.Linq.Enumerable.Select, System.Linq.Enumerable.ToArray, System.String..ctor, System.String.Replace, System.String.Contains, System.String.Trim

### `ToggleMode()` -> `void`
- token `0x0600003e`  flags=`129`  rva=`10348`
- IL size 129 bytes, 49 instructions, max_stack=2
- opcodes: ldarg.0x7, br.sx5, brtrue.sx5, callvirtx5, ldfldx5, ldloc.0x4, ldstrx4, callx3, ldc.i4.0x3, ldc.i4.2x2, retx2, brfalse.sx1, ldloca.sx1, stloc.0x1, stloc.1x1
- string literals: "My Custom Game", "Imported Game", "my-custom-game", "imported-game"
- calls: MPServerProvider.App.Views.AddCustomGameDialog.get_IsSteamMode, System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, System.Windows.UIElement.set_Visibility, System.Windows.Controls.TextBox.set_Text

### `OnModTypeChanged(object, class:329)` -> `void`
- token `0x0600003f`  flags=`129`  rva=`10492`
- IL size 140 bytes, 45 instructions, max_stack=2
- opcodes: ldarg.0x8, ldfldx8, callvirtx7, retx5, ldstrx4, brfalse.sx3, callx3, ldloca.sx3, stloc.0x3, brtrue.sx1
- string literals: "mods", "plugins", "addons", ""
- calls: System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, System.Windows.Controls.TextBox.set_Text

### `OnBrowseFolder(object, class:329)` -> `void`
- token `0x06000040`  flags=`129`  rva=`10644`
- IL size 74 bytes, 24 instructions, max_stack=3
- opcodes: callvirtx6, ldloc.0x3, dupx2, ldarg.0x2, brfalse.sx1, callx1, ldfldx1, ldloca.sx1, ldsfldx1, ldstrx1, newobjx1, retx1, stloc.0x1, stloc.1x1, stsfldx1
- string literals: "Select server folder"
- calls: Microsoft.Win32.OpenFolderDialog..ctor, Microsoft.Win32.CommonItemDialog.set_Title, Microsoft.Win32.CommonItemDialog.set_DefaultDirectory, Microsoft.Win32.CommonDialog.ShowDialog, System.Nullable`1.GetValueOrDefault, Microsoft.Win32.OpenFolderDialog.get_FolderName, System.Windows.Controls.TextBox.set_Text

### `OnBrowseImage(object, class:329)` -> `void`
- token `0x06000041`  flags=`129`  rva=`10732`
- IL size 63 bytes, 21 instructions, max_stack=3
- opcodes: callvirtx5, dupx2, ldarg.0x2, ldloc.0x2, ldstrx2, brfalse.sx1, callx1, ldfldx1, ldloca.sx1, newobjx1, retx1, stloc.0x1, stloc.1x1
- string literals: "Select header image", "Images (*.png;*.jpg;*.jpeg;*.webp;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.webp;*.bmp;*.gif"
- calls: Microsoft.Win32.OpenFileDialog..ctor, Microsoft.Win32.CommonItemDialog.set_Title, Microsoft.Win32.FileDialog.set_Filter, Microsoft.Win32.CommonDialog.ShowDialog, System.Nullable`1.GetValueOrDefault, Microsoft.Win32.FileDialog.get_FileName, System.Windows.Controls.TextBox.set_Text

### `ApplyHeaderImage(class:281)` -> `void`
- token `0x06000042`  flags=`129`  rva=`10808`
- IL size 233 bytes, 84 instructions, max_stack=4
- opcodes: callx13, callvirtx11, brtrue.sx8, ldstrx8, ldloc.0x7, ldloc.1x7, ldarg.1x3, retx3, brfalse.sx2, ldc.i4.1x2, ldloc.2x2, leave.sx2, popx2, stloc.1x2, stloc.3x2, br.sx1, ldarg.0x1, ldc.i4.0x1, ldc.i4.5x1, ldfldx1, ldloc.3x1, leavex1, nopx1, stloc.0x1, stloc.2x1
- string literals: "http", ".png", ".jpg", ".jpeg", ".webp", ".bmp", ".gif", ".png"
- calls: System.Windows.Controls.TextBox.get_Text, System.String.Trim, System.String.get_Length, System.String.StartsWith, MPServerProvider.Core.Models.GameDefinition.get_Artwork, MPServerProvider.Core.Models.ArtworkSpec.set_Cover, MPServerProvider.Core.Models.ArtworkSpec.set_Hero, System.IO.File.Exists, System.IO.Path.GetExtension, System.String.ToLowerInvariant, System.String.op_Equality, MPServerProvider.App.AppServices.get_Paths, MPServerProvider.Core.Services.AppPaths.get_GameArtOverrideDir, System.IO.Directory.CreateDirectory, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.Concat, System.IO.Path.Combine, System.IO.File.Copy

### `OnCancel(object, class:329)` -> `void`
- token `0x06000043`  flags=`129`  rva=`11072`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldc.i4.0x1, newobjx1, retx1
- calls: System.Nullable`1..ctor, System.Windows.Window.set_DialogResult

### `OnTitleBarMouseDown(object, class:345)` -> `void`
- token `0x06000044`  flags=`129`  rva=`11086`
- IL size 16 bytes, 7 instructions, max_stack=8
- opcodes: bne.un.sx1, callx1, callvirtx1, ldarg.0x1, ldarg.2x1, ldc.i4.1x1, retx1
- calls: System.Windows.Input.MouseEventArgs.get_LeftButton, System.Windows.Window.DragMove

### `OnAdd(object, class:329)` -> `void`
- token `0x06000045`  flags=`129`  rva=`11104`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `CreateDefinition(string, string, i4, string, string, string, string, string)` -> `class:281`
- token `0x06000046`  flags=`129`  rva=`11160`
- IL size 483 bytes, 156 instructions, max_stack=5
- opcodes: callvirtx34, dupx31, ldstrx20, newobjx11, br.sx8, ldarg.sx7, brtrue.sx6, callx6, stloc.sx5, ldloc.1x3, ldarg.0x2, ldc.i4.0x2, ldfldx2, ldloc.0x2, ldloc.sx2, ldloca.sx2, stloc.3x2, brfalsex1, ceqx1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.1x1, ldloc.2x1, retx1, stloc.0x1, stloc.1x1, stloc.2x1
- string literals: "mod", "addon", "plugin", "plugin", "addon", ".jar", ".zip", ".zip", ".gma", ".jar", ".dll", ".zip", ".jar", ".dll", "Custom game server added by the user.", "Custom", "#4F6BFF", "manual", "{server_file}", "none"
- calls: System.String.IsNullOrWhiteSpace, System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, System.String.op_Equality, System.Collections.Generic.List`1..ctor, System.Collections.Generic.List`1.Add, MPServerProvider.Core.Models.GameDefinition..ctor, MPServerProvider.Core.Models.GameDefinition.set_Id, MPServerProvider.Core.Models.GameDefinition.set_Name, MPServerProvider.Core.Models.GameDefinition.set_Description, MPServerProvider.Core.Models.GameDefinition.get_Genres, MPServerProvider.Core.Models.ArtworkSpec..ctor, MPServerProvider.Core.Models.ArtworkSpec.set_Accent, MPServerProvider.Core.Models.GameDefinition.set_Artwork, MPServerProvider.Core.Models.InstallSpec..ctor, MPServerProvider.Core.Models.InstallSpec.set_Provider, MPServerProvider.Core.Models.GameDefinition.set_Install, MPServerProvider.Core.Models.RuntimeSpec..ctor, MPServerProvider.Core.Models.RuntimeSpec.set_Executable, MPServerProvider.Core.Models.RuntimeSpec.set_Arguments, MPServerProvider.Core.Models.RuntimeSpec.set_StopCommand, MPServerProvider.Core.Models.GameDefinition.set_Runtime, MPServerProvider.Core.Models.NetworkSpec..ctor, MPServerProvider.Core.Models.NetworkSpec.set_DefaultPort, MPServerProvider.Core.Models.NetworkSpec.set_Protocol, MPServerProvider.Core.Models.NetworkSpec.set_Query, MPServerProvider.Core.Models.GameDefinition.set_Network, MPServerProvider.Core.Models.ModsSpec..ctor, MPServerProvider.Core.Models.ModsSpec.set_Supported, MPServerProvider.Core.Models.ModsSpec.set_Folder, MPServerProvider.Core.Models.ModsSpec.set_Extensions, MPServerProvider.Core.Models.ModsSpec.set_Kind, MPServerProvider.Core.Models.GameDefinition.set_Mods

### `InitializeComponent()` -> `void`
- token `0x06000047`  flags=`486`  rva=`11656`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/addcustomgamedialog.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x06000048`  flags=`481`  rva=`11704`
- IL size 889 bytes, 252 instructions, max_stack=3
- opcodes: ldarg.0x54, retx38, castclassx37, ldarg.2x37, stfldx33, callvirtx13, ldftnx13, newobjx13, ldfldx8, ldc.i4.1x2, brx1, ldarg.1x1, subx1, switchx1
- calls: MPServerProvider.App.Views.AddCustomGameDialog.OnTitleBarMouseDown, System.Windows.Input.MouseButtonEventHandler..ctor, System.Windows.UIElement.add_MouseLeftButtonDown, MPServerProvider.App.Views.AddCustomGameDialog.OnCancel, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.AddCustomGameDialog.OnFetchSteamInfo, MPServerProvider.App.Views.AddCustomGameDialog.OnAnonymousChanged, System.Windows.Controls.Primitives.ToggleButton.add_Checked, System.Windows.Controls.Primitives.ToggleButton.add_Unchecked, MPServerProvider.App.Views.AddCustomGameDialog.OnModTypeChanged, MPServerProvider.App.Views.AddCustomGameDialog.OnBrowseFolder, MPServerProvider.App.Views.AddCustomGameDialog.OnBrowseImage, MPServerProvider.App.Views.AddCustomGameDialog.OnAdd

### `.cctor()` -> `void`
- token `0x06000049`  flags=`6289`  rva=`12605`
- IL size 31 bytes, 7 instructions, max_stack=8
- opcodes: callx1, callvirtx1, dupx1, ldc.r8x1, newobjx1, retx1, stsfldx1
- calls: System.Net.Http.HttpClient..ctor, System.TimeSpan.FromSeconds, System.Net.Http.HttpClient.set_Timeout

### `<.ctor>b__9_0(object, class:329)` -> `void`
- token `0x0600004a`  flags=`129`  rva=`12637`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.AddCustomGameDialog.ToggleMode

### `<.ctor>b__9_1(object, class:329)` -> `void`
- token `0x0600004b`  flags=`129`  rva=`12645`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.AddCustomGameDialog.ToggleMode

### `<.ctor>b__9_2(object, class:329)` -> `void`
- token `0x0600004c`  flags=`129`  rva=`12653`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.AddCustomGameDialog.ToggleSource

### `<.ctor>b__9_3(object, class:329)` -> `void`
- token `0x0600004d`  flags=`129`  rva=`12661`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.AddCustomGameDialog.ToggleSource

