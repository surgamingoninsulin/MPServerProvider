# MPServerProvider.App.Views.ServerDetailView

Token: `0x02000019`  Flags: `1048577`  Extends: `0x01000074`

## Fields

- `_server` : `class:497`  (flags=1, token=0x040000bf)
- `_filesDir` : `string`  (flags=1, token=0x040000c0)
- `_console` : `class:609`  (flags=33, token=0x040000c1)
- `_timer` : `class:265`  (flags=33, token=0x040000c2)
- `_lastCpuTime` : `valuetype:453`  (flags=1, token=0x040000c3)
- `_lastCpuSample` : `valuetype:473`  (flags=1, token=0x040000c4)
- `_players` : `valuetype:325<valuetype:509<i4,i4>>`  (flags=1, token=0x040000c5)
- `_lastPlayerQuery` : `valuetype:473`  (flags=1, token=0x040000c6)
- `_editorPath` : `string`  (flags=1, token=0x040000c7)
- `NameText` : `class:389`  (flags=3, token=0x040000c8)
- `StatusBadge` : `class:317`  (flags=3, token=0x040000c9)
- `StatusText` : `class:389`  (flags=3, token=0x040000ca)
- `GameText` : `class:389`  (flags=3, token=0x040000cb)
- `StartButton` : `class:305`  (flags=3, token=0x040000cc)
- `StopButton` : `class:305`  (flags=3, token=0x040000cd)
- `TabConsole` : `class:293`  (flags=3, token=0x040000ce)
- `TabFiles` : `class:293`  (flags=3, token=0x040000cf)
- `TabMods` : `class:293`  (flags=3, token=0x040000d0)
- `TabResources` : `class:293`  (flags=3, token=0x040000d1)
- `TabTxAdmin` : `class:293`  (flags=3, token=0x040000d2)
- `TabSettings` : `class:293`  (flags=3, token=0x040000d3)
- `ConsolePanel` : `class:613`  (flags=3, token=0x040000d4)
- `ConsoleScroll` : `class:477`  (flags=3, token=0x040000d5)
- `ConsoleOutput` : `class:301`  (flags=3, token=0x040000d6)
- `CommandBox` : `class:301`  (flags=3, token=0x040000d7)
- `FilesPanel` : `class:613`  (flags=3, token=0x040000d8)
- `FilesPathText` : `class:389`  (flags=3, token=0x040000d9)
- `NewFileButton` : `class:305`  (flags=3, token=0x040000da)
- `FileEditorPanel` : `class:613`  (flags=3, token=0x040000db)
- `EditorFileName` : `class:389`  (flags=3, token=0x040000dc)
- `FileEditorBox` : `class:301`  (flags=3, token=0x040000dd)
- `EditorStatus` : `class:389`  (flags=3, token=0x040000de)
- `FilesListBorder` : `class:317`  (flags=3, token=0x040000df)
- `FilesList` : `class:601`  (flags=3, token=0x040000e0)
- `ModsPanel` : `class:613`  (flags=3, token=0x040000e1)
- `ModLibraryHeader` : `class:389`  (flags=3, token=0x040000e2)
- `LibraryModsList` : `class:601`  (flags=3, token=0x040000e3)
- `InstalledModsHeader` : `class:389`  (flags=3, token=0x040000e4)
- `InstalledModsList` : `class:601`  (flags=3, token=0x040000e5)
- `ResourcesPanel` : `class:613`  (flags=3, token=0x040000e6)
- `CpuText` : `class:389`  (flags=3, token=0x040000e7)
- `CpuBar` : `class:521`  (flags=3, token=0x040000e8)
- `RamText` : `class:389`  (flags=3, token=0x040000e9)
- `RamBar` : `class:521`  (flags=3, token=0x040000ea)
- `UptimeText` : `class:389`  (flags=3, token=0x040000eb)
- `PlayersText` : `class:389`  (flags=3, token=0x040000ec)
- `ConnectionText` : `class:389`  (flags=3, token=0x040000ed)
- `TxAdminPanel` : `class:613`  (flags=3, token=0x040000ee)
- `TxAdminWeb` : `class:617`  (flags=3, token=0x040000ef)
- `TxAdminPlaceholder` : `class:297`  (flags=3, token=0x040000f0)
- `TxAdminStatusText` : `class:389`  (flags=3, token=0x040000f1)
- `SettingsPanel` : `class:477`  (flags=3, token=0x040000f2)
- `SettingDescription` : `class:301`  (flags=3, token=0x040000f3)
- `SettingPort` : `class:301`  (flags=3, token=0x040000f4)
- `SettingRamMin` : `class:301`  (flags=3, token=0x040000f5)
- `SettingRamMax` : `class:301`  (flags=3, token=0x040000f6)
- `SettingStartCommand` : `class:301`  (flags=3, token=0x040000f7)
- `SettingStopCommand` : `class:301`  (flags=3, token=0x040000f8)
- `SettingAutoRestart` : `class:309`  (flags=3, token=0x040000f9)
- `_contentLoaded` : `bool`  (flags=1, token=0x040000fa)

## Methods

### `.ctor()` -> `void`
- token `0x06000107`  flags=`6278`  rva=`27196`
- IL size 155 bytes, 40 instructions, max_stack=4
- opcodes: ldarg.0x11, callx5, callvirtx5, newobjx5, stfldx4, ldftnx3, ldfldx2, dupx1, ldc.r8x1, ldsfldx1, ldstrx1, retx1
- string literals: ""
- calls: System.Text.StringBuilder..ctor, System.Windows.Controls.UserControl..ctor, MPServerProvider.App.Views.ServerDetailView.InitializeComponent, MPServerProvider.App.AppServices.get_Processes, MPServerProvider.App.Views.ServerDetailView.OnServerOutput, System.Action`2..ctor, MPServerProvider.Core.Services.ServerProcessService.add_OutputReceived, MPServerProvider.App.Views.ServerDetailView.OnServerStateChanged, MPServerProvider.Core.Services.ServerProcessService.add_StateChanged, System.Windows.Threading.DispatcherTimer..ctor, System.TimeSpan.FromSeconds, System.Windows.Threading.DispatcherTimer.set_Interval, MPServerProvider.App.Views.ServerDetailView.<.ctor>b__8_0, System.EventHandler..ctor, System.Windows.Threading.DispatcherTimer.add_Tick, System.Windows.Threading.DispatcherTimer.Start

### `SetServer(class:497)` -> `void`
- token `0x06000108`  flags=`134`  rva=`27364`
- IL size 453 bytes, 153 instructions, max_stack=4
- opcodes: callvirtx25, callx23, ldarg.0x18, ldarg.1x11, ldfldx10, ldloca.sx10, brtrue.sx6, ldloc.sx6, ldstrx5, stloc.sx5, br.sx4, brfalse.sx3, popx3, ldc.i4.0x2, ldc.i4.1x2, ldloc.0x2, ldloc.1x2, ldloc.3x2, stfldx2, dupx1, endfinallyx1, ldc.i4.2x1, ldc.i4.4x1, ldc.i4.sx1, ldnullx1, leave.sx1, newobjx1, retx1, stloc.0x1, stloc.1x1, stloc.3x1
- string literals: "  •  ", " ", "  •  port ", "fivem", "redm"
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Models.ServerInstance.get_ServerGame, MPServerProvider.Core.Services.GameLibraryService.Find, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, MPServerProvider.Core.Models.GameDefinition.get_Name, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, MPServerProvider.Core.Models.ServerInstance.get_ServerType, MPServerProvider.Core.Models.ServerInstance.get_ServerVersion, MPServerProvider.Core.Models.ServerInstance.get_ServerPort, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.String.op_Equality, System.Windows.UIElement.set_Visibility, System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, System.Nullable`1..ctor, System.Windows.Controls.Primitives.ToggleButton.set_IsChecked, MPServerProvider.App.AppServices.get_Processes, MPServerProvider.Core.Services.ServerProcessService.StateOf, MPServerProvider.App.Views.ServerDetailView.UpdateTxAdminPanel, System.Text.StringBuilder.Clear, MPServerProvider.Core.Services.ServerProcessService.ConsoleOf, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.Text.StringBuilder.AppendLine, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, System.Object.ToString, System.Windows.Controls.TextBox.set_Text, System.Windows.Controls.ScrollViewer.ScrollToEnd, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.RootFor, MPServerProvider.App.Views.ServerDetailView.RefreshFiles, MPServerProvider.App.Views.ServerDetailView.RefreshMods, MPServerProvider.App.Views.ServerDetailView.LoadSettings, MPServerProvider.App.Views.ServerDetailView.RefreshHeader, MPServerProvider.App.Views.ServerDetailView.UpdateLiveStats

### `RefreshHeader()` -> `void`
- token `0x06000109`  flags=`129`  rva=`27848`
- IL size 114 bytes, 52 instructions, max_stack=2
- opcodes: ldarg.0x6, ldfldx6, callvirtx5, ldloc.0x5, ldc.i4.1x4, stloc.2x4, callx3, br.sx2, ldc.i4.0x2, ldloc.1x2, ldloc.2x2, retx2, stloc.1x2, bgt.un.sx1, bne.un.sx1, brfalse.sx1, brtrue.sx1, ldc.i4.4x1, stloc.0x1, subx1
- calls: MPServerProvider.App.AppServices.get_Processes, MPServerProvider.Core.Services.ServerProcessService.StateOf, MPServerProvider.App.Views.ServerListItemVm.StatusLabel, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.App.Views.ServerListItemVm.StatusBrushFor, System.Windows.Controls.Border.set_Background, System.Windows.UIElement.set_IsEnabled

### `OnStart(object, class:329)` -> `void`
- token `0x0600010a`  flags=`129`  rva=`27976`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `OnStop(object, class:329)` -> `void`
- token `0x0600010b`  flags=`129`  rva=`28032`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `OnRestart(object, class:329)` -> `void`
- token `0x0600010c`  flags=`129`  rva=`28088`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `OnOpenFolder(object, class:329)` -> `void`
- token `0x0600010d`  flags=`129`  rva=`28144`
- IL size 74 bytes, 24 instructions, max_stack=4
- opcodes: callx4, ldstrx3, callvirtx2, ldarg.0x2, ldfldx2, ldloc.0x2, retx2, brfalse.sx1, brtrue.sx1, dupx1, ldc.i4.1x1, newobjx1, popx1, stloc.0x1
- string literals: "explorer.exe", """, """
- calls: MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.RootFor, System.IO.Directory.Exists, System.String.Concat, System.Diagnostics.ProcessStartInfo..ctor, System.Diagnostics.ProcessStartInfo.set_UseShellExecute, System.Diagnostics.Process.Start

### `OnExport(object, class:329)` -> `void`
- token `0x0600010e`  flags=`129`  rva=`28232`
- IL size 408 bytes, 123 instructions, max_stack=4
- opcodes: callvirtx37, dupx18, ldarg.0x18, ldfldx17, callx7, ldstrx7, retx3, brtrue.sx2, ldloc.0x2, leave.sx2, newobjx2, ldloc.2x1, ldloc.3x1, ldloca.sx1, nopx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "Export Server Config", "Server Config (*.server.json)|*.server.json", ".server.json", "Server config exported", "", "Export failed: ", ""
- calls: Microsoft.Win32.SaveFileDialog..ctor, Microsoft.Win32.CommonItemDialog.set_Title, Microsoft.Win32.FileDialog.set_Filter, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.String.Concat, Microsoft.Win32.FileDialog.set_FileName, System.Windows.Window.GetWindow, Microsoft.Win32.CommonDialog.ShowDialog, System.Nullable`1.GetValueOrDefault, MPServerProvider.Core.Models.ServerExportData..ctor, MPServerProvider.Core.Models.ServerInstance.get_ManifestVersion, MPServerProvider.Core.Models.ServerExportData.set_ManifestVersion, MPServerProvider.Core.Models.ServerExportData.set_ServerName, MPServerProvider.Core.Models.ServerInstance.get_ServerDescription, MPServerProvider.Core.Models.ServerExportData.set_ServerDescription, MPServerProvider.Core.Models.ServerInstance.get_ServerGame, MPServerProvider.Core.Models.ServerExportData.set_ServerGame, MPServerProvider.Core.Models.ServerInstance.get_ServerVersion, MPServerProvider.Core.Models.ServerExportData.set_ServerVersion, MPServerProvider.Core.Models.ServerInstance.get_ServerType, MPServerProvider.Core.Models.ServerExportData.set_ServerType, MPServerProvider.Core.Models.ServerInstance.get_ServerPort, MPServerProvider.Core.Models.ServerExportData.set_ServerPort, MPServerProvider.Core.Models.ServerInstance.get_RamMinMb, MPServerProvider.Core.Models.ServerExportData.set_RamMinMb, MPServerProvider.Core.Models.ServerInstance.get_RamMaxMb, MPServerProvider.Core.Models.ServerExportData.set_RamMaxMb, MPServerProvider.Core.Models.ServerInstance.get_JavaMajor, MPServerProvider.Core.Models.ServerExportData.set_JavaMajor, MPServerProvider.Core.Models.ServerInstance.get_StartCommand, MPServerProvider.Core.Models.ServerExportData.set_StartCommand, MPServerProvider.Core.Models.ServerInstance.get_StopCommand, MPServerProvider.Core.Models.ServerExportData.set_StopCommand, MPServerProvider.Core.Models.ServerInstance.get_ModsFolder, MPServerProvider.Core.Models.ServerExportData.set_ModsFolder, MPServerProvider.Core.Models.ServerInstance.get_AutoRestartOnCrash, MPServerProvider.Core.Models.ServerExportData.set_AutoRestartOnCrash, MPServerProvider.Core.Models.ServerInstance.get_CreateFirewallRule, MPServerProvider.Core.Models.ServerExportData.set_CreateFirewallRule, Microsoft.Win32.FileDialog.get_FileName...

### `OnServerStateChanged(class:497, valuetype:589)` -> `void`
- token `0x0600010f`  flags=`129`  rva=`28680`
- IL size 76 bytes, 28 instructions, max_stack=3
- opcodes: callx3, ldarg.0x3, ldloc.0x3, callvirtx2, newobjx2, retx2, stfldx2, br.sx1, brfalse.sx1, brtrue.sx1, dupx1, ldarg.1x1, ldarg.2x1, ldfldx1, ldftnx1, ldnullx1, popx1, stloc.0x1
- calls: <>c__DisplayClass16_0..ctor, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.String.op_Inequality, System.Windows.Threading.DispatcherObject.get_Dispatcher, <>c__DisplayClass16_0.<OnServerStateChanged>b__0, System.Action..ctor, System.Windows.Threading.Dispatcher.Invoke

### `UpdateTxAdminPanel(valuetype:589)` -> `void`
- token `0x06000110`  flags=`129`  rva=`28768`
- IL size 124 bytes, 49 instructions, max_stack=2
- opcodes: ldarg.0x6, ldfldx5, callx4, callvirtx4, ldstrx4, brtrue.sx3, retx3, br.sx2, ldc.i4.0x2, ldc.i4.2x2, ldloc.0x2, popx2, stloc.1x2, bne.un.sx1, brfalse.sx1, dupx1, ldarg.1x1, ldc.i4.1x1, ldloc.1x1, ldnullx1, stloc.0x1
- string literals: "fivem", "redm", "txAdmin is starting…", "Start the server to open the txAdmin console"
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerGame, System.String.op_Equality, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.App.Views.ServerDetailView.ShowTxAdminAsync, System.Windows.UIElement.set_Visibility

### `ShowTxAdminAsync()` -> `class:433`
- token `0x06000111`  flags=`129`  rva=`28904`
- IL size 55 bytes, 17 instructions, max_stack=2
- opcodes: ldloca.sx6, callx3, stfldx3, ldfldax2, ldarg.0x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `OnServerOutput(class:497, string)` -> `void`
- token `0x06000112`  flags=`129`  rva=`28972`
- IL size 82 bytes, 30 instructions, max_stack=3
- opcodes: callx4, ldarg.0x3, ldloc.0x3, callvirtx2, newobjx2, popx2, retx2, stfldx2, br.sx1, brfalse.sx1, brtrue.sx1, dupx1, ldarg.1x1, ldarg.2x1, ldfldx1, ldftnx1, ldnullx1, stloc.0x1
- calls: <>c__DisplayClass19_0..ctor, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.String.op_Inequality, System.Windows.Threading.DispatcherObject.get_Dispatcher, <>c__DisplayClass19_0.<OnServerOutput>b__0, System.Action..ctor, System.Array.Empty, System.Windows.Threading.Dispatcher.BeginInvoke

### `OnSendCommand(object, class:329)` -> `void`
- token `0x06000113`  flags=`129`  rva=`29066`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.ServerDetailView.SendCommand

### `OnCommandKeyDown(object, class:625)` -> `void`
- token `0x06000114`  flags=`129`  rva=`29074`
- IL size 16 bytes, 7 instructions, max_stack=8
- opcodes: bne.un.sx1, callx1, callvirtx1, ldarg.0x1, ldarg.2x1, ldc.i4.6x1, retx1
- calls: System.Windows.Input.KeyEventArgs.get_Key, MPServerProvider.App.Views.ServerDetailView.SendCommand

### `SendCommand()` -> `void`
- token `0x06000115`  flags=`129`  rva=`29092`
- IL size 106 bytes, 31 instructions, max_stack=3
- opcodes: callvirtx6, ldarg.0x6, ldfldx6, callx4, retx3, brfalse.sx2, ldstrx2, beq.sx1, ldc.i4.2x1
- string literals: "Server is not running", ""
- calls: System.Windows.Controls.TextBox.get_Text, System.String.IsNullOrWhiteSpace, MPServerProvider.App.AppServices.get_Processes, MPServerProvider.Core.Services.ServerProcessService.StateOf, MPServerProvider.App.ToastService.Show, System.String.Trim, MPServerProvider.Core.Services.ServerProcessService.SendCommand, System.Windows.Controls.TextBox.Clear

### `OnTabChanged(object, class:329)` -> `void`
- token `0x06000116`  flags=`129`  rva=`29212`
- IL size 280 bytes, 101 instructions, max_stack=2
- opcodes: ldarg.0x17, ldfldx15, callvirtx14, callx10, ldloca.sx8, stloc.0x8, brtrue.sx7, br.sx6, ldc.i4.0x6, ldc.i4.2x6, brfalse.sx2, retx2
- calls: System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, System.Windows.UIElement.set_Visibility, MPServerProvider.App.Views.ServerDetailView.RefreshFiles, MPServerProvider.App.Views.ServerDetailView.RefreshMods

### `RefreshFiles()` -> `void`
- token `0x06000117`  flags=`129`  rva=`29504`
- IL size 388 bytes, 130 instructions, max_stack=4
- opcodes: callvirtx26, dupx13, callx9, ldarg.0x8, ldfldx8, ldloc.1x8, newobjx6, brtrue.sx5, ldloc.sx4, ldsfldx4, leave.sx4, brfalse.sx3, ldloc.0x3, ldloc.2x3, br.sx2, endfinallyx2, ldftnx2, ldloc.3x2, ldstrx2, popx2, retx2, stloc.1x2, stloc.sx2, stsfldx2, ldc.i4.0x1, ldc.i4.1x1, ldnullx1, stloc.0x1, stloc.2x1, stloc.3x1
- string literals: "Cannot read folder: ", ""
- calls: System.IO.Directory.Exists, System.Windows.Controls.ItemsControl.set_ItemsSource, System.Windows.Controls.TextBlock.set_Text, System.Collections.Generic.List`1..ctor, System.IO.Directory.EnumerateDirectories, <>c.<RefreshFiles>b__24_0, System.Func`2..ctor, System.Linq.Enumerable.OrderBy, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, MPServerProvider.App.Views.FileEntryVm..ctor, System.IO.Path.GetFileName, MPServerProvider.App.Views.FileEntryVm.set_Name, MPServerProvider.App.Views.FileEntryVm.set_FullPath, MPServerProvider.App.Views.FileEntryVm.set_IsDirectory, System.IO.Directory.GetLastWriteTime, MPServerProvider.App.Views.FileEntryVm.set_Modified, System.Collections.Generic.List`1.Add, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, System.IO.Directory.EnumerateFiles, <>c.<RefreshFiles>b__24_1, System.IO.FileInfo..ctor, System.IO.FileSystemInfo.get_Name, System.IO.FileInfo.get_Length, MPServerProvider.App.Views.FileEntryVm.set_Size, System.IO.FileSystemInfo.get_LastWriteTime, System.Exception.get_Message, System.String.Concat, MPServerProvider.App.ToastService.Show

### `OnFilesUp(object, class:329)` -> `void`
- token `0x06000118`  flags=`129`  rva=`29980`
- IL size 86 bytes, 34 instructions, max_stack=3
- opcodes: ldarg.0x7, ldfldx5, brfalse.sx3, callx3, callvirtx3, retx3, ldc.i4.5x2, ldloc.0x2, ldloc.1x2, brtrue.sx1, stfldx1, stloc.0x1, stloc.1x1
- calls: MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.RootFor, System.IO.Path.GetDirectoryName, System.String.StartsWith, System.String.Equals, MPServerProvider.App.Views.ServerDetailView.RefreshFiles

### `OnFilesRefresh(object, class:329)` -> `void`
- token `0x06000119`  flags=`129`  rva=`30078`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.ServerDetailView.RefreshFiles

### `OnFileOpen(object, class:345)` -> `void`
- token `0x0600011a`  flags=`129`  rva=`30088`
- IL size 105 bytes, 37 instructions, max_stack=3
- opcodes: callvirtx6, callx4, ldloc.0x4, ldarg.0x3, retx3, ldstrx2, leave.sx2, brfalse.sx1, brtrue.sx1, dupx1, isinstx1, ldc.i4.1x1, ldfldx1, ldloc.1x1, newobjx1, nopx1, popx1, stfldx1, stloc.0x1, stloc.1x1
- string literals: "Cannot open file: ", ""
- calls: System.Windows.Controls.Primitives.Selector.get_SelectedItem, MPServerProvider.App.Views.FileEntryVm.get_IsDirectory, MPServerProvider.App.Views.FileEntryVm.get_FullPath, MPServerProvider.App.Views.ServerDetailView.RefreshFiles, System.Diagnostics.ProcessStartInfo..ctor, System.Diagnostics.ProcessStartInfo.set_UseShellExecute, System.Diagnostics.Process.Start, System.Exception.get_Message, System.String.Concat, MPServerProvider.App.ToastService.Show

### `OnFileDelete(object, class:329)` -> `void`
- token `0x0600011b`  flags=`129`  rva=`30224`
- IL size 163 bytes, 52 instructions, max_stack=5
- opcodes: callx10, callvirtx7, ldstrx7, ldloc.0x6, ldarg.0x3, retx3, leave.sx2, beq.sx1, br.sx1, brfalse.sx1, brtrue.sx1, isinstx1, ldc.i4.1x1, ldc.i4.4x1, ldc.i4.6x1, ldc.i4.sx1, ldfldx1, ldloc.1x1, nopx1, stloc.0x1, stloc.1x1
- string literals: "Delete ", "?", "MP Server Provider", " deleted", "", "Delete failed: ", ""
- calls: System.Windows.Controls.Primitives.Selector.get_SelectedItem, System.Windows.Window.GetWindow, MPServerProvider.App.Views.FileEntryVm.get_Name, System.String.Concat, System.Windows.MessageBox.Show, MPServerProvider.App.Views.FileEntryVm.get_IsDirectory, MPServerProvider.App.Views.FileEntryVm.get_FullPath, System.IO.Directory.Delete, System.IO.File.Delete, MPServerProvider.App.ToastService.Show, System.Exception.get_Message, MPServerProvider.App.Views.ServerDetailView.RefreshFiles

### `OnFileEdit(object, class:329)` -> `void`
- token `0x0600011c`  flags=`129`  rva=`30416`
- IL size 47 bytes, 20 instructions, max_stack=2
- opcodes: brfalse.sx3, callvirtx3, ldloc.0x3, isinstx2, ldloc.1x2, retx2, callx1, ldarg.0x1, ldarg.1x1, stloc.0x1, stloc.1x1
- calls: System.Windows.FrameworkElement.get_Tag, MPServerProvider.App.Views.FileEntryVm.get_IsDirectory, MPServerProvider.App.Views.FileEntryVm.get_FullPath, MPServerProvider.App.Views.ServerDetailView.OpenEditor

### `OnNewFile(object, class:329)` -> `void`
- token `0x0600011d`  flags=`129`  rva=`30476`
- IL size 113 bytes, 41 instructions, max_stack=4
- opcodes: callx10, ldloca.sx5, ldstrx4, ldarg.0x3, ldloc.0x3, ldc.i4.1x2, ldfldx2, ldloc.1x2, stloc.0x2, stloc.1x2, addx1, br.sx1, brtrue.sx1, ldc.i4.2x1, ldc.i4.sx1, retx1
- string literals: "newfile.txt", "newfile", ".txt", ""
- calls: System.IO.Path.Combine, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.IO.File.Exists, System.IO.File.WriteAllText, MPServerProvider.App.Views.ServerDetailView.OpenEditor

### `OpenEditor(string)` -> `void`
- token `0x0600011e`  flags=`129`  rva=`30604`
- IL size 132 bytes, 42 instructions, max_stack=3
- opcodes: callvirtx8, ldarg.0x8, ldfldx7, callx3, ldarg.1x3, ldstrx3, leave.sx2, ldc.i4.0x1, ldc.i4.2x1, ldloc.0x1, ldloc.1x1, retx1, stfldx1, stloc.0x1, stloc.1x1
- string literals: "", "", "Could not read file: "
- calls: System.IO.Path.GetFileName, System.Windows.Controls.TextBlock.set_Text, System.Windows.UIElement.set_Visibility, System.IO.File.ReadAllText, System.Windows.Controls.TextBox.set_Text, System.Exception.get_Message, System.String.Concat

### `OnFileSave(object, class:329)` -> `void`
- token `0x0600011f`  flags=`129`  rva=`30764`
- IL size 154 bytes, 43 instructions, max_stack=3
- opcodes: callx9, ldarg.0x7, ldfldx7, ldstrx6, callvirtx5, ldloc.0x2, leave.sx2, retx2, brtrue.sx1, nopx1, stloc.0x1
- string literals: "Saved ", " saved", "", "Save failed: ", "Save failed: ", ""
- calls: System.Windows.Controls.TextBox.get_Text, System.IO.File.WriteAllText, System.IO.Path.GetFileName, System.String.Concat, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.App.ToastService.Show, System.Exception.get_Message

### `OnFileDiscard(object, class:329)` -> `void`
- token `0x06000120`  flags=`129`  rva=`30948`
- IL size 38 bytes, 13 instructions, max_stack=8
- opcodes: ldarg.0x4, ldfldx3, retx2, brtrue.sx1, callx1, callvirtx1, ldstrx1
- string literals: "Changes discarded — reverted to last saved version"
- calls: MPServerProvider.App.Views.ServerDetailView.OpenEditor, System.Windows.Controls.TextBlock.set_Text

### `OnFileEditorClose(object, class:329)` -> `void`
- token `0x06000121`  flags=`129`  rva=`30987`
- IL size 38 bytes, 14 instructions, max_stack=8
- opcodes: ldarg.0x4, callvirtx2, ldfldx2, callx1, ldc.i4.0x1, ldc.i4.2x1, ldnullx1, retx1, stfldx1
- calls: System.Windows.UIElement.set_Visibility, MPServerProvider.App.Views.ServerDetailView.RefreshFiles

### `RefreshMods()` -> `void`
- token `0x06000122`  flags=`129`  rva=`31028`
- IL size 574 bytes, 183 instructions, max_stack=4
- opcodes: callx25, callvirtx25, ldloc.sx14, ldarg.0x13, ldfldx13, brtrue.sx9, dupx8, stloc.sx7, brfalse.sx6, ldsfldx6, newobjx6, br.sx5, ldstrx5, popx5, ldloc.0x4, ldnullx4, ldc.i4.0x3, ldftnx3, ldloc.3x3, retx3, stsfldx3, ldloc.1x2, ldloc.2x2, bgt.sx1, endfinallyx1, ldc.i4.1x1, ldloca.sx1, leave.sx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "mod", " Library", "This game does not support mods", "Installed on this server (", ")"
- calls: MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Models.ServerInstance.get_ServerGame, MPServerProvider.Core.Services.GameLibraryService.Find, MPServerProvider.Core.Models.GameDefinition.get_Mods, MPServerProvider.Core.Models.ModsSpec.get_Kind, System.String.get_Chars, System.Char.ToUpper, System.ReadOnlySpan`1..ctor, System.String.Substring, System.String.op_Implicit, System.String.Concat, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.Core.Models.ServerInstance.get_ModsFolder, System.String.IsNullOrWhiteSpace, System.Windows.Controls.ItemsControl.set_ItemsSource, MPServerProvider.App.AppServices.get_Mods, MPServerProvider.Core.Services.ModLibraryService.Scan, <>c.<RefreshMods>b__36_0, System.Func`2..ctor, System.Linq.Enumerable.Select, System.Linq.Enumerable.ToList, MPServerProvider.Core.Services.ModLibraryService.ServerModsDir, MPServerProvider.Core.Models.ModsSpec.get_Extensions, System.Collections.Generic.List`1..ctor, <>c.<RefreshMods>b__36_1, System.StringComparer.get_OrdinalIgnoreCase, System.Linq.Enumerable.ToHashSet, System.IO.Directory.Exists, System.IO.Directory.EnumerateFileSystemEntries, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.IO.Path.GetFileName, System.Collections.Generic.List`1.get_Count, System.Collections.Generic.HashSet`1.Contains, System.IO.File.Exists, System.IO.Path.GetExtension, System.Linq.Enumerable.Contains, System.Collections.Generic.List`1.Add, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose...

### `OnAssignMod(object, class:329)` -> `void`
- token `0x06000123`  flags=`129`  rva=`31632`
- IL size 120 bytes, 39 instructions, max_stack=3
- opcodes: callvirtx6, callx5, ldarg.0x4, ldfldx3, ldloc.0x3, ldstrx3, brfalse.sx2, isinstx2, ldloc.1x2, leave.sx2, retx2, brtrue.sx1, ldarg.1x1, nopx1, stloc.0x1, stloc.1x1
- string literals: " added to ", "", ""
- calls: System.Windows.FrameworkElement.get_Tag, MPServerProvider.App.AppServices.get_Mods, MPServerProvider.App.Views.LibraryModVm.get_Mod, MPServerProvider.Core.Services.ModLibraryService.AssignToServer, MPServerProvider.App.Views.LibraryModVm.get_FileName, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.String.Concat, MPServerProvider.App.ToastService.Show, System.Exception.get_Message, MPServerProvider.App.Views.ServerDetailView.RefreshMods

### `OnRemoveMod(object, class:329)` -> `void`
- token `0x06000124`  flags=`129`  rva=`31780`
- IL size 231 bytes, 78 instructions, max_stack=3
- opcodes: callx16, ldstrx6, callvirtx5, ldloc.2x5, brfalse.sx4, retx4, brtrue.sx3, ldarg.0x3, ldloc.0x3, ldloc.1x3, ldloc.sx3, stloc.sx3, isinstx2, ldfldx2, ldsfldx2, leave.sx2, br.sx1, ldarg.1x1, ldc.i4.1x1, ldc.i4.5x1, ldloc.3x1, ldloca.sx1, newobjx1, nopx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "Blocked: target is outside the mods folder", "", " removed", "", "Remove failed: ", ""
- calls: System.Windows.FrameworkElement.get_Tag, MPServerProvider.App.AppServices.get_Mods, MPServerProvider.Core.Services.ModLibraryService.ServerModsDir, System.IO.Path.Combine, System.IO.Path.GetFullPath, System.String.TrimEnd, System.String.op_Implicit, System.ReadOnlySpan`1..ctor, System.String.Concat, System.String.StartsWith, MPServerProvider.App.ToastService.Show, System.IO.Directory.Exists, System.IO.Directory.Delete, System.IO.File.Exists, System.IO.File.Delete, System.Exception.get_Message, MPServerProvider.App.Views.ServerDetailView.RefreshMods

### `UpdateLiveStats()` -> `void`
- token `0x06000125`  flags=`129`  rva=`32040`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `GetLanAddress()` -> `string`
- token `0x06000126`  flags=`145`  rva=`32096`
- IL size 81 bytes, 30 instructions, max_stack=3
- opcodes: dupx4, popx4, brtrue.sx3, callx3, ldsfldx2, ldstrx2, leave.sx2, stloc.0x2, br.sx1, callvirtx1, ldftnx1, ldloc.0x1, ldnullx1, newobjx1, retx1, stsfldx1
- string literals: "0.0.0.0", "0.0.0.0"
- calls: System.Net.Dns.GetHostName, System.Net.Dns.GetHostAddresses, <>c.<GetLanAddress>b__40_0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault, System.Object.ToString

### `LoadSettings()` -> `void`
- token `0x06000127`  flags=`129`  rva=`32208`
- IL size 202 bytes, 61 instructions, max_stack=3
- opcodes: ldarg.0x15, ldfldx15, callvirtx14, callx3, ldloca.sx3, stloc.0x3, brtrue.sx2, retx2, dupx1, ldstrx1, newobjx1, popx1
- string literals: ""
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerDescription, System.Windows.Controls.TextBox.set_Text, MPServerProvider.Core.Models.ServerInstance.get_ServerPort, System.Int32.ToString, MPServerProvider.Core.Models.ServerInstance.get_RamMinMb, MPServerProvider.Core.Models.ServerInstance.get_RamMaxMb, MPServerProvider.Core.Models.ServerInstance.get_StartCommand, MPServerProvider.Core.Models.ServerInstance.get_StopCommand, MPServerProvider.Core.Models.ServerInstance.get_AutoRestartOnCrash, System.Nullable`1..ctor, System.Windows.Controls.Primitives.ToggleButton.set_IsChecked

### `OnSaveSettings(object, class:329)` -> `void`
- token `0x06000128`  flags=`129`  rva=`32424`
- IL size 362 bytes, 117 instructions, max_stack=2
- opcodes: callvirtx23, ldarg.0x20, ldfldx19, callx9, ldloca.sx4, ldstrx4, brfalse.sx3, brtrue.sx3, ldc.i4.0x3, ldloc.0x3, retx3, stloc.sx3, ble.sx2, blt.sx2, br.sx2, ldc.i4.1x2, ldloc.1x2, ldloc.2x2, ldloc.3x2, stloc.3x2, ceqx1, ldc.i4x1, ldloc.sx1, ldnullx1
- string literals: "Enter a valid port", "", "Server settings saved", ""
- calls: System.Windows.Controls.TextBox.get_Text, System.String.Trim, System.Int32.TryParse, MPServerProvider.App.ToastService.Show, MPServerProvider.Core.Models.ServerInstance.set_ServerDescription, MPServerProvider.Core.Models.ServerInstance.set_ServerPort, MPServerProvider.Core.Models.ServerInstance.set_RamMinMb, MPServerProvider.Core.Models.ServerInstance.get_RamMinMb, MPServerProvider.Core.Models.ServerInstance.set_RamMaxMb, MPServerProvider.Core.Models.ServerInstance.set_StartCommand, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Models.ServerInstance.set_StopCommand, System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, MPServerProvider.Core.Models.ServerInstance.set_AutoRestartOnCrash, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.Save, MPServerProvider.App.Views.ServerDetailView.SetServer

### `OnDeleteServer(object, class:329)` -> `void`
- token `0x06000129`  flags=`129`  rva=`32800`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `InitializeComponent()` -> `void`
- token `0x0600012a`  flags=`486`  rva=`32856`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/serverdetailview.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x0600012b`  flags=`481`  rva=`32904`
- IL size 1500 bytes, 416 instructions, max_stack=3
- opcodes: ldarg.0x86, retx64, castclassx63, ldarg.2x63, stfldx51, callvirtx24, ldftnx24, newobjx24, ldfldx11, ldc.i4.1x2, brx1, ldarg.1x1, subx1, switchx1
- calls: MPServerProvider.App.Views.ServerDetailView.OnStart, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.ServerDetailView.OnStop, MPServerProvider.App.Views.ServerDetailView.OnRestart, MPServerProvider.App.Views.ServerDetailView.OnOpenFolder, MPServerProvider.App.Views.ServerDetailView.OnExport, MPServerProvider.App.Views.ServerDetailView.OnTabChanged, System.Windows.Controls.Primitives.ToggleButton.add_Checked, MPServerProvider.App.Views.ServerDetailView.OnCommandKeyDown, System.Windows.Input.KeyEventHandler..ctor, System.Windows.UIElement.add_KeyDown, MPServerProvider.App.Views.ServerDetailView.OnSendCommand, MPServerProvider.App.Views.ServerDetailView.OnFilesUp, MPServerProvider.App.Views.ServerDetailView.OnFilesRefresh, MPServerProvider.App.Views.ServerDetailView.OnFileDelete, MPServerProvider.App.Views.ServerDetailView.OnNewFile, MPServerProvider.App.Views.ServerDetailView.OnFileSave, MPServerProvider.App.Views.ServerDetailView.OnFileDiscard, MPServerProvider.App.Views.ServerDetailView.OnFileEditorClose, MPServerProvider.App.Views.ServerDetailView.OnFileOpen, System.Windows.Input.MouseButtonEventHandler..ctor, System.Windows.Controls.Control.add_MouseDoubleClick, MPServerProvider.App.Views.ServerDetailView.OnSaveSettings, MPServerProvider.App.Views.ServerDetailView.OnDeleteServer

### `System.Windows.Markup.IStyleConnector.Connect(i4, object)` -> `void`
- token `0x0600012c`  flags=`481`  rva=`34416`
- IL size 88 bytes, 31 instructions, max_stack=3
- opcodes: retx4, beq.sx3, callvirtx3, castclassx3, ldarg.0x3, ldarg.1x3, ldarg.2x3, ldc.i4.sx3, ldftnx3, newobjx3
- calls: MPServerProvider.App.Views.ServerDetailView.OnFileEdit, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.ServerDetailView.OnAssignMod, MPServerProvider.App.Views.ServerDetailView.OnRemoveMod

### `<.ctor>b__8_0(object, class:269)` -> `void`
- token `0x0600012d`  flags=`129`  rva=`34516`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.ServerDetailView.UpdateLiveStats

