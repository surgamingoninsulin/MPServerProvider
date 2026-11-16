# MPServerProvider.App.Views.HomePage

Token: `0x0200000c`  Flags: `1048577`  Extends: `0x01000074`

## Fields

- `_timer` : `class:265`  (flags=33, token=0x04000057)
- `_lastPlayerQuery` : `valuetype:473`  (flags=1, token=0x04000058)
- `_lastPlayerCount` : `i4`  (flags=1, token=0x04000059)
- `SlidePixelsPerSecond` : `r8`  (flags=32849, token=0x0400005a)
- `SlideTickMs` : `i4`  (flags=32849, token=0x0400005b)
- `_news` : `class:289<class:44>`  (flags=1, token=0x0400005c)
- `_slideTimer` : `class:265`  (flags=1, token=0x0400005d)
- `_slidePaused` : `bool`  (flags=1, token=0x0400005e)
- `UptimeStat` : `class:389`  (flags=3, token=0x0400005f)
- `PlayersStat` : `class:389`  (flags=3, token=0x04000060)
- `ModsShortcutText` : `class:389`  (flags=3, token=0x04000061)
- `ServersShortcutText` : `class:389`  (flags=3, token=0x04000062)
- `NewsScroller` : `class:477`  (flags=3, token=0x04000063)
- `NewsHost` : `class:481`  (flags=3, token=0x04000064)
- `_contentLoaded` : `bool`  (flags=1, token=0x04000065)

## Methods

### `.ctor()` -> `void`
- token `0x06000074`  flags=`6278`  rva=`14464`
- IL size 113 bytes, 32 instructions, max_stack=4
- opcodes: ldarg.0x10, callx5, callvirtx3, newobjx3, stfldx3, ldfldx2, dupx1, ldc.r8x1, ldftnx1, ldsfldx1, popx1, retx1
- calls: System.Collections.Generic.List`1..ctor, System.Windows.Controls.UserControl..ctor, MPServerProvider.App.Views.HomePage.InitializeComponent, MPServerProvider.App.Views.HomePage.LoadNews, System.Windows.Threading.DispatcherTimer..ctor, System.TimeSpan.FromSeconds, System.Windows.Threading.DispatcherTimer.set_Interval, MPServerProvider.App.Views.HomePage.<.ctor>b__3_0, System.EventHandler..ctor, System.Windows.Threading.DispatcherTimer.add_Tick, System.Windows.Threading.DispatcherTimer.Start, MPServerProvider.App.Views.HomePage.RefreshStatsSafeAsync

### `OnNavigatedTo()` -> `void`
- token `0x06000075`  flags=`486`  rva=`14589`
- IL size 25 bytes, 9 instructions, max_stack=8
- opcodes: ldarg.0x3, callx2, ldsfldx1, popx1, retx1, stfldx1
- calls: MPServerProvider.App.Views.HomePage.LoadNews, MPServerProvider.App.Views.HomePage.RefreshStatsSafeAsync

### `LoadNews()` -> `void`
- token `0x06000076`  flags=`129`  rva=`14616`
- IL size 169 bytes, 55 instructions, max_stack=4
- opcodes: ldarg.0x9, callx8, callvirtx5, ldfldx5, br.sx2, brfalse.sx2, brtrue.sx2, dupx2, ldloc.0x2, ldsfldx2, leave.sx2, newobjx2, popx2, retx2, stfldx2, cgtx1, ldc.i4.0x1, ldc.i4.1x1, ldftnx1, stloc.0x1, stsfldx1
- calls: MPServerProvider.App.AppServices.get_News, MPServerProvider.Core.Services.NewsService.Load, <>c.<LoadNews>b__10_0, System.Func`2..ctor, System.Linq.Enumerable.Select, System.Linq.Enumerable.ToList, System.Collections.Generic.List`1..ctor, MPServerProvider.App.AppServices.get_Settings, MPServerProvider.Core.Services.SettingsService.get_Settings, MPServerProvider.Core.Models.AppSettings.get_NewsAutoScroll, System.Collections.Generic.List`1.get_Count, System.Linq.Enumerable.Concat, System.Windows.Controls.ItemsControl.set_ItemsSource, MPServerProvider.App.Views.HomePage.StartSliding, MPServerProvider.App.Views.HomePage.StopSliding

### `StartSliding()` -> `void`
- token `0x06000077`  flags=`129`  rva=`14816`
- IL size 74 bytes, 20 instructions, max_stack=4
- opcodes: ldarg.0x5, callvirtx3, ldfldx3, newobjx2, brtrue.sx1, callx1, dupx1, ldc.r8x1, ldftnx1, retx1, stfldx1
- calls: System.Windows.Threading.DispatcherTimer..ctor, System.TimeSpan.FromMilliseconds, System.Windows.Threading.DispatcherTimer.set_Interval, MPServerProvider.App.Views.HomePage.OnSlideTick, System.EventHandler..ctor, System.Windows.Threading.DispatcherTimer.add_Tick, System.Windows.Threading.DispatcherTimer.Start

### `StopSliding()` -> `void`
- token `0x06000078`  flags=`129`  rva=`14902`
- IL size 43 bytes, 16 instructions, max_stack=8
- opcodes: brtrue.sx2, callx2, dupx2, ldarg.0x2, ldfldx2, popx2, retx2, br.sx1, ldc.r8x1
- calls: System.Windows.Threading.DispatcherTimer.Stop, System.Windows.Controls.ScrollViewer.ScrollToHorizontalOffset

### `OnSlideTick(object, class:269)` -> `void`
- token `0x06000079`  flags=`129`  rva=`14948`
- IL size 95 bytes, 35 instructions, max_stack=2
- opcodes: ldarg.0x5, ldfldx5, callvirtx3, ldc.r8x3, ldloc.0x3, ldloc.1x3, retx3, brtrue.sx2, stloc.1x2, addx1, bgt.un.sx1, blt.un.sx1, divx1, stloc.0x1, subx1
- calls: System.Windows.Controls.ScrollViewer.get_ExtentWidth, System.Windows.Controls.ScrollViewer.get_HorizontalOffset, System.Windows.Controls.ScrollViewer.ScrollToHorizontalOffset

### `OnNewsMouseEnter(object, class:485)` -> `void`
- token `0x0600007a`  flags=`129`  rva=`15055`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldc.i4.1x1, retx1, stfldx1

### `OnNewsMouseLeave(object, class:485)` -> `void`
- token `0x0600007b`  flags=`129`  rva=`15064`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldc.i4.0x1, retx1, stfldx1

### `OnNewsCardClick(object, class:345)` -> `void`
- token `0x0600007c`  flags=`129`  rva=`15076`
- IL size 78 bytes, 30 instructions, max_stack=2
- opcodes: callvirtx6, brfalse.sx3, isinstx3, ldloc.0x3, retx3, callx2, ldloc.1x2, ldloc.2x2, brtrue.sx1, ldarg.0x1, ldarg.1x1, stloc.0x1, stloc.1x1, stloc.2x1
- calls: System.Windows.FrameworkElement.get_DataContext, MPServerProvider.App.Views.NewsCardVm.get_Item, MPServerProvider.Core.Models.NewsItem.get_GameId, System.String.IsNullOrWhiteSpace, System.Windows.Window.GetWindow, MPServerProvider.App.Views.MainWindow.ShowGameInLibrary

### `RefreshNow()` -> `void`
- token `0x0600007d`  flags=`134`  rva=`15166`
- IL size 19 bytes, 7 instructions, max_stack=8
- opcodes: ldarg.0x2, callx1, ldsfldx1, popx1, retx1, stfldx1
- calls: MPServerProvider.App.Views.HomePage.RefreshStatsSafeAsync

### `RefreshStatsSafeAsync()` -> `class:433`
- token `0x0600007e`  flags=`129`  rva=`15186`
- IL size 48 bytes, 14 instructions, max_stack=8
- opcodes: dupx2, ldsfldx2, brtrue.sx1, callx1, callvirtx1, ldarg.0x1, ldc.i4x1, ldftnx1, newobjx1, popx1, retx1, stsfldx1
- calls: MPServerProvider.App.Views.HomePage.RefreshStatsAsync, <>c.<RefreshStatsSafeAsync>b__18_0, System.Action`1..ctor, System.Threading.Tasks.Task.ContinueWith

### `RefreshStatsAsync()` -> `class:433`
- token `0x0600007f`  flags=`129`  rva=`15236`
- IL size 55 bytes, 17 instructions, max_stack=2
- opcodes: ldloca.sx6, callx3, stfldx3, ldfldax2, ldarg.0x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `OnManageServers(object, class:329)` -> `void`
- token `0x06000080`  flags=`129`  rva=`15303`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldstrx1, retx1
- string literals: "servers"
- calls: MPServerProvider.App.Views.HomePage.Navigate

### `OnBrowseGames(object, class:329)` -> `void`
- token `0x06000081`  flags=`129`  rva=`15316`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldstrx1, retx1
- string literals: "library"
- calls: MPServerProvider.App.Views.HomePage.Navigate

### `OnShortcutMods(object, class:345)` -> `void`
- token `0x06000082`  flags=`129`  rva=`15329`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldstrx1, retx1
- string literals: "mods"
- calls: MPServerProvider.App.Views.HomePage.Navigate

### `OnShortcutServers(object, class:345)` -> `void`
- token `0x06000083`  flags=`129`  rva=`15342`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldstrx1, retx1
- string literals: "servers"
- calls: MPServerProvider.App.Views.HomePage.Navigate

### `OnShortcutGetStarted(object, class:345)` -> `void`
- token `0x06000084`  flags=`129`  rva=`15355`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldstrx1, retx1
- string literals: "library"
- calls: MPServerProvider.App.Views.HomePage.Navigate

### `Navigate(string)` -> `void`
- token `0x06000085`  flags=`129`  rva=`15368`
- IL size 23 bytes, 10 instructions, max_stack=2
- opcodes: ldloc.0x2, brfalse.sx1, callx1, callvirtx1, isinstx1, ldarg.0x1, ldarg.1x1, retx1, stloc.0x1
- calls: System.Windows.Window.GetWindow, MPServerProvider.App.Views.MainWindow.NavigateTo

### `InitializeComponent()` -> `void`
- token `0x06000086`  flags=`486`  rva=`15404`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/homepage.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x06000087`  flags=`481`  rva=`15452`
- IL size 281 bytes, 79 instructions, max_stack=3
- opcodes: ldarg.0x15, retx11, castclassx10, ldarg.2x10, stfldx7, callvirtx6, ldftnx6, newobjx6, ldc.i4.1x2, ldfldx2, brx1, ldarg.1x1, subx1, switchx1
- calls: MPServerProvider.App.Views.HomePage.OnManageServers, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.HomePage.OnBrowseGames, MPServerProvider.App.Views.HomePage.OnShortcutMods, System.Windows.Input.MouseButtonEventHandler..ctor, System.Windows.UIElement.add_MouseLeftButtonUp, MPServerProvider.App.Views.HomePage.OnShortcutServers, MPServerProvider.App.Views.HomePage.OnNewsMouseEnter, System.Windows.Input.MouseEventHandler..ctor, System.Windows.UIElement.add_MouseEnter, MPServerProvider.App.Views.HomePage.OnNewsMouseLeave, System.Windows.UIElement.add_MouseLeave

### `System.Windows.Markup.IStyleConnector.Connect(i4, object)` -> `void`
- token `0x06000088`  flags=`481`  rva=`15745`
- IL size 29 bytes, 10 instructions, max_stack=8
- opcodes: bne.un.sx1, callvirtx1, castclassx1, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.sx1, ldftnx1, newobjx1, retx1
- calls: MPServerProvider.App.Views.HomePage.OnNewsCardClick, System.Windows.Input.MouseButtonEventHandler..ctor, System.Windows.UIElement.add_MouseLeftButtonUp

### `<.ctor>b__3_0(object, class:269)` -> `void`
- token `0x06000089`  flags=`129`  rva=`15775`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, popx1, retx1
- calls: MPServerProvider.App.Views.HomePage.RefreshStatsSafeAsync

