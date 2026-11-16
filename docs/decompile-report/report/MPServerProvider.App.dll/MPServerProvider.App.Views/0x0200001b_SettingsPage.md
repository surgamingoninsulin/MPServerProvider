# MPServerProvider.App.Views.SettingsPage

Token: `0x0200001b`  Flags: `1048577`  Extends: `0x01000074`

## Fields

- `_timer` : `class:265`  (flags=33, token=0x040000fc)
- `_drivesLoaded` : `bool`  (flags=1, token=0x040000fd)
- `DiskUsageText` : `class:389`  (flags=3, token=0x040000fe)
- `DiskUsageBar` : `class:521`  (flags=3, token=0x040000ff)
- `DiskUsageCaption` : `class:389`  (flags=3, token=0x04000100)
- `GameFilesText` : `class:389`  (flags=3, token=0x04000101)
- `GameFilesBar` : `class:521`  (flags=3, token=0x04000102)
- `GameFilesCaption` : `class:389`  (flags=3, token=0x04000103)
- `DriveCombo` : `class:321`  (flags=3, token=0x04000104)
- `InstallPathText` : `class:389`  (flags=3, token=0x04000105)
- `SettingStartOnBoot` : `class:309`  (flags=3, token=0x04000106)
- `SettingStartMinimized` : `class:309`  (flags=3, token=0x04000107)
- `SettingRestoreServers` : `class:309`  (flags=3, token=0x04000108)
- `SettingNewsAutoScroll` : `class:309`  (flags=3, token=0x04000109)
- `CpuText` : `class:389`  (flags=3, token=0x0400010a)
- `CpuBar` : `class:521`  (flags=3, token=0x0400010b)
- `RamText` : `class:389`  (flags=3, token=0x0400010c)
- `RamBar` : `class:521`  (flags=3, token=0x0400010d)
- `ActiveServersText` : `class:389`  (flags=3, token=0x0400010e)
- `SpecCpu` : `class:389`  (flags=3, token=0x0400010f)
- `SpecRam` : `class:389`  (flags=3, token=0x04000110)
- `SpecGpu` : `class:389`  (flags=3, token=0x04000111)
- `SpecOs` : `class:389`  (flags=3, token=0x04000112)
- `RulesHost` : `class:481`  (flags=3, token=0x04000113)
- `NoRulesText` : `class:389`  (flags=3, token=0x04000114)
- `_contentLoaded` : `bool`  (flags=1, token=0x04000115)

## Methods

### `.ctor()` -> `void`
- token `0x06000133`  flags=`6278`  rva=`34684`
- IL size 187 bytes, 54 instructions, max_stack=4
- opcodes: ldarg.0x15, callvirtx12, callx9, ldfldx6, ldloc.0x4, newobjx2, dupx1, ldc.r8x1, ldftnx1, retx1, stfldx1, stloc.0x1
- calls: System.Windows.Controls.UserControl..ctor, MPServerProvider.App.Views.SettingsPage.InitializeComponent, MPServerProvider.App.AppServices.get_Metrics, MPServerProvider.Core.Services.SystemMetricsService.Specs, MPServerProvider.Core.Services.SystemSpecs.get_Processor, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.Core.Services.SystemSpecs.get_MemoryGb, MPServerProvider.Core.Services.SystemSpecs.get_Graphics, MPServerProvider.Core.Services.SystemSpecs.get_OperatingSystem, MPServerProvider.App.Views.SettingsPage.LoadSettings, MPServerProvider.App.Views.SettingsPage.LoadDrives, MPServerProvider.App.Views.SettingsPage.RefreshRules, MPServerProvider.App.Views.SettingsPage.RefreshStorage, System.Windows.Threading.DispatcherTimer..ctor, System.TimeSpan.FromSeconds, System.Windows.Threading.DispatcherTimer.set_Interval, MPServerProvider.App.Views.SettingsPage.<.ctor>b__2_0, System.EventHandler..ctor, System.Windows.Threading.DispatcherTimer.add_Tick, System.Windows.Threading.DispatcherTimer.Start, MPServerProvider.App.Views.SettingsPage.RefreshLive

### `OnNavigatedTo()` -> `void`
- token `0x06000134`  flags=`486`  rva=`34883`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx2, ldarg.0x2, retx1
- calls: MPServerProvider.App.Views.SettingsPage.RefreshStorage, MPServerProvider.App.Views.SettingsPage.RefreshRules

### `LoadSettings()` -> `void`
- token `0x06000135`  flags=`129`  rva=`34900`
- IL size 100 bytes, 28 instructions, max_stack=2
- opcodes: callvirtx9, ldarg.0x4, ldfldx4, ldloc.0x4, newobjx4, callx1, retx1, stloc.0x1
- calls: MPServerProvider.App.AppServices.get_Settings, MPServerProvider.Core.Services.SettingsService.get_Settings, MPServerProvider.Core.Models.AppSettings.get_StartOnBoot, System.Nullable`1..ctor, System.Windows.Controls.Primitives.ToggleButton.set_IsChecked, MPServerProvider.Core.Models.AppSettings.get_StartMinimizedToTray, MPServerProvider.Core.Models.AppSettings.get_RestoreRunningServers, MPServerProvider.Core.Models.AppSettings.get_NewsAutoScroll

### `OnStartOnBootChanged(object, class:329)` -> `void`
- token `0x06000136`  flags=`129`  rva=`35012`
- IL size 127 bytes, 32 instructions, max_stack=2
- opcodes: callvirtx9, callx8, ldstrx4, ldarg.0x2, ldfldx2, ldloca.sx2, retx2, stloc.0x2, brfalse.sx1
- string literals: "Panel will launch at boot", "", "Boot launch disabled", ""
- calls: MPServerProvider.App.AppServices.get_Settings, MPServerProvider.Core.Services.SettingsService.get_Settings, System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, MPServerProvider.Core.Models.AppSettings.set_StartOnBoot, MPServerProvider.Core.Models.AppSettings.set_StartMinimizedToTray, MPServerProvider.Core.Services.SettingsService.Save, MPServerProvider.Core.Models.AppSettings.get_StartOnBoot, MPServerProvider.App.ToastService.Show

### `OnBehaviourChanged(object, class:329)` -> `void`
- token `0x06000137`  flags=`129`  rva=`35152`
- IL size 104 bytes, 32 instructions, max_stack=3
- opcodes: callvirtx8, callx6, ldarg.0x4, ldfldx3, ldloca.sx3, stloc.0x3, dupx2, retx2, brtrue.sx1
- calls: System.Windows.FrameworkElement.get_IsLoaded, MPServerProvider.App.AppServices.get_Settings, MPServerProvider.Core.Services.SettingsService.get_Settings, System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, MPServerProvider.Core.Models.AppSettings.set_StartMinimizedToTray, MPServerProvider.Core.Models.AppSettings.set_RestoreRunningServers, MPServerProvider.Core.Models.AppSettings.set_NewsAutoScroll, MPServerProvider.Core.Services.SettingsService.Save

### `RefreshLive()` -> `void`
- token `0x06000138`  flags=`129`  rva=`35268`
- IL size 195 bytes, 60 instructions, max_stack=4
- opcodes: callx13, ldloca.sx9, callvirtx8, ldarg.0x6, ldfldx6, ldc.i4.1x4, ldstrx4, ldloc.0x2, ldloc.1x2, retx2, brtrue.sx1, stloc.0x1, stloc.1x1, stloc.sx1
- string literals: "0.0", "%", "0.0", "%"
- calls: System.Windows.FrameworkElement.get_IsLoaded, MPServerProvider.App.AppServices.get_Metrics, MPServerProvider.Core.Services.SystemMetricsService.CpuPercent, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Windows.Controls.TextBlock.set_Text, System.Windows.Controls.Primitives.RangeBase.set_Value, MPServerProvider.Core.Services.SystemMetricsService.Ram, MPServerProvider.App.AppServices.get_Processes, MPServerProvider.Core.Services.ServerProcessService.get_RunningCount, System.Int32.ToString

### `RefreshStorage()` -> `void`
- token `0x06000139`  flags=`129`  rva=`35476`
- IL size 317 bytes, 92 instructions, max_stack=4
- opcodes: callx15, ldloc.0x10, ldloca.sx10, callvirtx9, ldfldx9, ldstrx7, ldarg.0x5, newobjx3, stfldx3, conv.r8x2, divx2, ldc.r8x2, ldftnx2, ldloc.1x2, brfalsex1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.sx1, ldloc.2x1, popx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1
- string literals: "0.0", " GB", "0.0", " GB of ", "0", " GB used", "Current path: "
- calls: <>c__DisplayClass8_0..ctor, MPServerProvider.App.AppServices.get_Paths, MPServerProvider.Core.Services.AppPaths.get_ServersDir, MPServerProvider.App.AppServices.get_Metrics, MPServerProvider.Core.Services.SystemMetricsService.Drives, <>c__DisplayClass8_0.<RefreshStorage>b__0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault, MPServerProvider.Core.Services.DriveUsage.get_UsedBytes, MPServerProvider.Core.Services.DriveUsage.get_TotalBytes, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.Core.Services.DriveUsage.get_UsedPercent, System.Windows.Controls.Primitives.RangeBase.set_Value, <>c__DisplayClass8_0.<RefreshStorage>b__1, System.Action..ctor, System.Threading.Tasks.Task.Run, System.String.Concat

### `LoadDrives()` -> `void`
- token `0x0600013a`  flags=`129`  rva=`35808`
- IL size 137 bytes, 44 instructions, max_stack=4
- opcodes: callx5, callvirtx5, dupx3, ldarg.0x3, newobjx3, ldc.i4.0x2, ldfldx2, ldftnx2, ldloc.0x2, ldloc.2x2, ldsfldx2, stfldx2, bge.sx1, br.sx1, brtrue.sx1, ldc.i4.1x1, ldloc.1x1, popx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1, stsfldx1
- calls: <>c__DisplayClass9_0..ctor, MPServerProvider.App.AppServices.get_Metrics, MPServerProvider.Core.Services.SystemMetricsService.Drives, System.Linq.Enumerable.ToList, <>c.<LoadDrives>b__9_0, System.Func`2..ctor, System.Linq.Enumerable.Select, System.Windows.Controls.ItemsControl.set_ItemsSource, MPServerProvider.App.AppServices.get_Paths, MPServerProvider.Core.Services.AppPaths.get_ServersDir, <>c__DisplayClass9_0.<LoadDrives>b__1, System.Predicate`1..ctor, System.Collections.Generic.List`1.FindIndex, System.Windows.Controls.Primitives.Selector.set_SelectedIndex

### `OnDriveChanged(object, class:529)` -> `void`
- token `0x0600013b`  flags=`129`  rva=`35960`
- IL size 202 bytes, 63 instructions, max_stack=3
- opcodes: callx12, callvirtx11, ldstrx7, ldarg.0x4, ldfldx3, ldloc.2x3, retx3, brfalse.sx2, ldc.i4.5x2, ldloc.0x2, ldloc.1x2, leave.sx2, bge.sx1, br.sx1, brtrue.sx1, ldc.i4.0x1, ldloc.3x1, nopx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "servers", "MPServerProvider", "servers", "New servers will install to ", "", "Cannot use that drive: ", ""
- calls: System.Windows.Controls.Primitives.Selector.get_SelectedIndex, MPServerProvider.App.AppServices.get_Metrics, MPServerProvider.Core.Services.SystemMetricsService.Drives, System.Collections.Generic.IReadOnlyList`1.get_Item, MPServerProvider.App.AppServices.get_Paths, MPServerProvider.Core.Services.AppPaths.get_DataRoot, System.IO.Path.Combine, MPServerProvider.Core.Services.DriveUsage.get_Root, System.String.StartsWith, MPServerProvider.Core.Services.AppPaths.get_ServersDir, System.String.Equals, MPServerProvider.App.AppServices.get_Settings, MPServerProvider.Core.Services.SettingsService.SetInstallRoot, System.String.Concat, MPServerProvider.App.ToastService.Show, System.Exception.get_Message, MPServerProvider.App.Views.SettingsPage.RefreshStorage

### `RefreshRules()` -> `void`
- token `0x0600013c`  flags=`129`  rva=`36192`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `OnAddRule(object, class:329)` -> `void`
- token `0x0600013d`  flags=`129`  rva=`36248`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `OnDeleteRule(object, class:329)` -> `void`
- token `0x0600013e`  flags=`129`  rva=`36304`
- IL size 51 bytes, 17 instructions, max_stack=2
- opcodes: ldloca.sx6, stfldx4, callx2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `InitializeComponent()` -> `void`
- token `0x0600013f`  flags=`486`  rva=`36368`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/settingspage.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x06000140`  flags=`481`  rva=`36416`
- IL size 651 bytes, 185 instructions, max_stack=3
- opcodes: ldarg.0x43, retx25, castclassx24, ldarg.2x24, stfldx24, callvirtx10, ldftnx10, newobjx10, ldfldx9, ldc.i4.1x2, brx1, ldarg.1x1, subx1, switchx1
- calls: MPServerProvider.App.Views.SettingsPage.OnDriveChanged, System.Windows.Controls.SelectionChangedEventHandler..ctor, System.Windows.Controls.Primitives.Selector.add_SelectionChanged, MPServerProvider.App.Views.SettingsPage.OnStartOnBootChanged, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ToggleButton.add_Checked, System.Windows.Controls.Primitives.ToggleButton.add_Unchecked, MPServerProvider.App.Views.SettingsPage.OnBehaviourChanged, MPServerProvider.App.Views.SettingsPage.OnAddRule, System.Windows.Controls.Primitives.ButtonBase.add_Click

### `System.Windows.Markup.IStyleConnector.Connect(i4, object)` -> `void`
- token `0x06000141`  flags=`481`  rva=`37079`
- IL size 29 bytes, 10 instructions, max_stack=8
- opcodes: bne.un.sx1, callvirtx1, castclassx1, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.sx1, ldftnx1, newobjx1, retx1
- calls: MPServerProvider.App.Views.SettingsPage.OnDeleteRule, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ButtonBase.add_Click

### `<.ctor>b__2_0(object, class:269)` -> `void`
- token `0x06000142`  flags=`129`  rva=`37109`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.Views.SettingsPage.RefreshLive

