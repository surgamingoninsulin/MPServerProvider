# MPServerProvider.App.Views.MyServersPage

Token: `0x02000016`  Flags: `1048577`  Extends: `0x01000074`

## Fields

- `_allItems` : `class:289<class:84>`  (flags=33, token=0x040000b0)
- `_detail` : `class:100`  (flags=33, token=0x040000b1)
- `ActiveCountText` : `class:389`  (flags=3, token=0x040000b2)
- `SearchBox` : `class:301`  (flags=3, token=0x040000b3)
- `SearchHint` : `class:389`  (flags=3, token=0x040000b4)
- `ServerList` : `class:601`  (flags=3, token=0x040000b5)
- `EmptyState` : `class:297`  (flags=3, token=0x040000b6)
- `DetailHost` : `class:561`  (flags=3, token=0x040000b7)
- `_contentLoaded` : `bool`  (flags=1, token=0x040000b8)

## Methods

### `.ctor()` -> `void`
- token `0x060000e5`  flags=`6278`  rva=`24752`
- IL size 102 bytes, 28 instructions, max_stack=3
- opcodes: ldarg.0x9, callx5, newobjx4, callvirtx3, ldfldx2, ldftnx2, stfldx2, retx1
- calls: System.Collections.Generic.List`1..ctor, MPServerProvider.App.Views.ServerDetailView..ctor, System.Windows.Controls.UserControl..ctor, MPServerProvider.App.Views.MyServersPage.InitializeComponent, System.Windows.Controls.ContentControl.set_Content, MPServerProvider.App.AppServices.get_Processes, MPServerProvider.App.Views.MyServersPage.<.ctor>b__2_0, System.Action`2..ctor, MPServerProvider.Core.Services.ServerProcessService.add_StateChanged, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.App.Views.MyServersPage.<.ctor>b__2_1, System.Action..ctor, MPServerProvider.Core.Services.ServerRepository.add_Changed, MPServerProvider.App.Views.MyServersPage.ReloadList

### `OnNavigatedTo()` -> `void`
- token `0x060000e6`  flags=`486`  rva=`24866`
- IL size 58 bytes, 20 instructions, max_stack=8
- opcodes: ldarg.0x5, callvirtx4, ldfldx4, ble.sx1, brtrue.sx1, callx1, ldc.i4.0x1, ldc.i4.1x1, retx1, subx1
- calls: MPServerProvider.App.Views.MyServersPage.ReloadList, System.Windows.Controls.Primitives.Selector.get_SelectedItem, System.Collections.Generic.List`1.get_Count, System.Windows.Controls.Primitives.Selector.set_SelectedIndex

### `ReloadList()` -> `void`
- token `0x060000e7`  flags=`129`  rva=`24928`
- IL size 209 bytes, 67 instructions, max_stack=4
- opcodes: callvirtx10, ldarg.0x7, callx6, ldfldx6, ldloc.1x4, newobjx4, brtrue.sx3, dupx3, ldloc.0x3, br.sx2, brfalse.sx2, ldftnx2, ldsfldx2, popx2, endfinallyx1, isinstx1, ldloc.2x1, ldnullx1, leave.sx1, retx1, stfldx1, stloc.0x1, stloc.1x1, stloc.2x1, stsfldx1
- calls: <>c__DisplayClass4_0..ctor, System.Windows.Controls.Primitives.Selector.get_SelectedItem, MPServerProvider.App.Views.ServerListItemVm.get_Server, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.Collections.Generic.List`1.Clear, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.get_Servers, <>c.<ReloadList>b__4_1, System.Func`2..ctor, System.Linq.Enumerable.OrderBy, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, MPServerProvider.App.Views.ServerListItemVm..ctor, System.Collections.Generic.List`1.Add, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, MPServerProvider.App.Views.MyServersPage.ApplyFilter, <>c__DisplayClass4_0.<ReloadList>b__0, System.Linq.Enumerable.FirstOrDefault, System.Windows.Controls.Primitives.Selector.set_SelectedItem, MPServerProvider.App.Views.MyServersPage.RefreshStates

### `ApplyFilter()` -> `void`
- token `0x060000e8`  flags=`129`  rva=`25168`
- IL size 153 bytes, 53 instructions, max_stack=3
- opcodes: callvirtx7, ldarg.0x7, ldfldx7, brtrue.sx4, callx4, ldc.i4.0x3, br.sx2, dupx2, ldloc.0x2, ldloc.1x2, newobjx2, popx2, ble.sx1, ldc.i4.2x1, ldftnx1, ldnullx1, ldstrx1, retx1, stfldx1, stloc.0x1, stloc.1x1
- string literals: ""
- calls: <>c__DisplayClass5_0..ctor, System.Windows.Controls.TextBox.get_Text, System.String.Trim, <>c__DisplayClass5_0.<ApplyFilter>b__0, System.Func`2..ctor, System.Linq.Enumerable.Where, System.Linq.Enumerable.ToList, System.Windows.Controls.ItemsControl.set_ItemsSource, System.String.IsNullOrEmpty, System.Windows.UIElement.set_Visibility, System.Windows.Controls.Primitives.Selector.get_SelectedItem, System.Collections.Generic.List`1.get_Count, System.Windows.Controls.Primitives.Selector.set_SelectedIndex

### `OnSearchChanged(object, class:393)` -> `void`
- token `0x060000e9`  flags=`129`  rva=`25333`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.MyServersPage.ApplyFilter

### `RefreshStates()` -> `void`
- token `0x060000ea`  flags=`129`  rva=`25344`
- IL size 166 bytes, 53 instructions, max_stack=4
- opcodes: callx8, ldloca.sx8, callvirtx4, ldarg.0x3, ldfldx3, ldstrx3, br.sx2, brtrue.sx2, dupx2, ldloc.0x2, ldsfldx2, beq.sx1, constrained.x1, endfinallyx1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.sx1, ldftnx1, leave.sx1, newobjx1, popx1, retx1, stloc.0x1, stloc.1x1, stsfldx1
- string literals: " active server", "s", ""
- calls: System.Collections.Generic.List`1.GetEnumerator, Enumerator.get_Current, MPServerProvider.App.Views.ServerListItemVm.Refresh, Enumerator.MoveNext, System.IDisposable.Dispose, <>c.<RefreshStates>b__7_0, System.Func`2..ctor, System.Linq.Enumerable.Count, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Windows.Controls.TextBlock.set_Text

### `OnServerSelected(object, class:529)` -> `void`
- token `0x060000eb`  flags=`129`  rva=`25540`
- IL size 87 bytes, 30 instructions, max_stack=2
- opcodes: callvirtx7, ldarg.0x6, ldfldx6, ldc.i4.0x2, ldc.i4.2x2, ldloc.0x2, retx2, brfalse.sx1, isinstx1, stloc.0x1
- calls: System.Windows.Controls.Primitives.Selector.get_SelectedItem, System.Windows.UIElement.set_Visibility, MPServerProvider.App.Views.ServerListItemVm.get_Server, MPServerProvider.App.Views.ServerDetailView.SetServer

### `OnGoToLibrary(object, class:329)` -> `void`
- token `0x060000ec`  flags=`129`  rva=`25640`
- IL size 27 bytes, 10 instructions, max_stack=2
- opcodes: ldloc.0x2, brfalse.sx1, callx1, callvirtx1, isinstx1, ldarg.0x1, ldstrx1, retx1, stloc.0x1
- string literals: "library"
- calls: System.Windows.Window.GetWindow, MPServerProvider.App.Views.MainWindow.NavigateTo

### `OnConnectTunnel(object, class:329)` -> `void`
- token `0x060000ed`  flags=`129`  rva=`25679`
- IL size 52 bytes, 18 instructions, max_stack=8
- opcodes: callvirtx3, callx2, dupx2, ldarg.0x2, popx2, br.sx1, brtrue.sx1, isinstx1, ldfldx1, ldnullx1, newobjx1, retx1
- calls: System.Windows.Controls.Primitives.Selector.get_SelectedItem, MPServerProvider.App.Views.ServerListItemVm.get_Server, MPServerProvider.App.Views.TunnelDialog..ctor, System.Windows.Window.GetWindow, System.Windows.Window.set_Owner, System.Windows.Window.ShowDialog

### `OnImport(object, class:329)` -> `void`
- token `0x060000ee`  flags=`129`  rva=`25732`
- IL size 771 bytes, 221 instructions, max_stack=4
- opcodes: callvirtx63, ldfldx24, ldloc.2x24, callx20, dupx18, ldloc.sx14, ldstrx12, brfalse.sx5, ldarg.0x4, ldloc.3x4, newobjx4, stloc.sx3, bgt.sx2, brtrue.sx2, ldc.i4x2, ldc.i4.0x2, ldloc.0x2, leavex2, leave.sx2, retx2, brfalsex1, ldftnx1, ldloca.sx1, nopx1, popx1, stfldx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "Import Server Config", "Server Config (*.server.json)|*.server.json", "Invalid server config file", "", "A server named '", "' already exists", "", "'", "' imported", "", "Import failed: ", ""
- calls: Microsoft.Win32.OpenFileDialog..ctor, Microsoft.Win32.CommonItemDialog.set_Title, Microsoft.Win32.FileDialog.set_Filter, System.Windows.Window.GetWindow, Microsoft.Win32.CommonDialog.ShowDialog, System.Nullable`1.GetValueOrDefault, <>c__DisplayClass11_0..ctor, Microsoft.Win32.FileDialog.get_FileName, MPServerProvider.Core.Json.Json.ReadFile, MPServerProvider.Core.Models.ServerExportData.get_ServerName, System.String.IsNullOrWhiteSpace, MPServerProvider.App.ToastService.Show, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.NameTaken, System.String.Concat, MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Models.ServerExportData.get_ServerGame, MPServerProvider.Core.Services.GameLibraryService.Find, MPServerProvider.Core.Services.ServerRepository.NewServerDir, System.IO.Directory.CreateDirectory, MPServerProvider.Core.Models.ServerInstance..ctor, MPServerProvider.Core.Models.ServerExportData.get_ManifestVersion, MPServerProvider.Core.Models.ServerInstance.set_ManifestVersion, MPServerProvider.Core.Models.ServerInstance.set_ServerName, MPServerProvider.Core.Models.ServerExportData.get_ServerDescription, MPServerProvider.Core.Models.ServerInstance.set_ServerDescription, MPServerProvider.Core.Models.ServerInstance.set_ServerGame, MPServerProvider.Core.Models.ServerExportData.get_ServerVersion, MPServerProvider.Core.Models.ServerInstance.set_ServerVersion, MPServerProvider.Core.Models.ServerExportData.get_ServerType, MPServerProvider.Core.Models.ServerInstance.set_ServerType, MPServerProvider.Core.Models.ServerInstance.set_ServerRoot, MPServerProvider.Core.Models.ServerExportData.get_ServerPort, MPServerProvider.Core.Models.ServerInstance.set_ServerPort, MPServerProvider.Core.Models.ServerExportData.get_RamMinMb, MPServerProvider.Core.Models.ServerInstance.set_RamMinMb, MPServerProvider.Core.Models.ServerExportData.get_RamMaxMb, MPServerProvider.Core.Models.ServerInstance.set_RamMaxMb, MPServerProvider.Core.Models.ServerExportData.get_JavaMajor, MPServerProvider.Core.Models.ServerInstance.set_JavaMajor...

### `InitializeComponent()` -> `void`
- token `0x060000ef`  flags=`486`  rva=`26544`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/myserverspage.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x060000f0`  flags=`481`  rva=`26592`
- IL size 253 bytes, 72 instructions, max_stack=3
- opcodes: ldarg.0x14, retx10, castclassx9, ldarg.2x9, stfldx7, callvirtx5, ldftnx5, newobjx5, ldc.i4.1x2, ldfldx2, brx1, ldarg.1x1, subx1, switchx1
- calls: MPServerProvider.App.Views.MyServersPage.OnSearchChanged, System.Windows.Controls.TextChangedEventHandler..ctor, System.Windows.Controls.Primitives.TextBoxBase.add_TextChanged, MPServerProvider.App.Views.MyServersPage.OnServerSelected, System.Windows.Controls.SelectionChangedEventHandler..ctor, System.Windows.Controls.Primitives.Selector.add_SelectionChanged, MPServerProvider.App.Views.MyServersPage.OnConnectTunnel, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.MyServersPage.OnImport, MPServerProvider.App.Views.MyServersPage.OnGoToLibrary, System.Windows.Documents.Hyperlink.add_Click

### `<.ctor>b__2_0(class:497, valuetype:589)` -> `void`
- token `0x060000f1`  flags=`129`  rva=`26857`
- IL size 24 bytes, 7 instructions, max_stack=8
- opcodes: ldarg.0x2, callx1, callvirtx1, ldftnx1, newobjx1, retx1
- calls: System.Windows.Threading.DispatcherObject.get_Dispatcher, MPServerProvider.App.Views.MyServersPage.RefreshStates, System.Action..ctor, System.Windows.Threading.Dispatcher.Invoke

### `<.ctor>b__2_1()` -> `void`
- token `0x060000f2`  flags=`129`  rva=`26882`
- IL size 24 bytes, 7 instructions, max_stack=8
- opcodes: ldarg.0x2, callx1, callvirtx1, ldftnx1, newobjx1, retx1
- calls: System.Windows.Threading.DispatcherObject.get_Dispatcher, MPServerProvider.App.Views.MyServersPage.ReloadList, System.Action..ctor, System.Windows.Threading.Dispatcher.Invoke

