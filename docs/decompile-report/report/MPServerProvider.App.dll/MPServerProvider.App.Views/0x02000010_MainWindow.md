# MPServerProvider.App.Views.MainWindow

Token: `0x02000010`  Flags: `1048577`  Extends: `0x01000044`

## Fields

- `_pages` : `class:557<string,class:465>`  (flags=33, token=0x0400008e)
- `_fullscreen` : `bool`  (flags=1, token=0x0400008f)
- `NavHome` : `class:293`  (flags=3, token=0x04000090)
- `NavLibrary` : `class:293`  (flags=3, token=0x04000091)
- `NavMods` : `class:293`  (flags=3, token=0x04000092)
- `NavServers` : `class:293`  (flags=3, token=0x04000093)
- `NavSettings` : `class:293`  (flags=3, token=0x04000094)
- `MaxRestoreButton` : `class:305`  (flags=3, token=0x04000095)
- `PageHost` : `class:561`  (flags=3, token=0x04000096)
- `_contentLoaded` : `bool`  (flags=1, token=0x04000097)

## Methods

### `.ctor()` -> `void`
- token `0x060000b0`  flags=`6278`  rva=`19851`
- IL size 53 bytes, 16 instructions, max_stack=8
- opcodes: ldarg.0x6, callx4, newobjx2, ldftnx1, ldstrx1, retx1, stfldx1
- string literals: "home"
- calls: System.Collections.Generic.Dictionary`2..ctor, System.Windows.Window..ctor, MPServerProvider.App.Views.MainWindow.InitializeComponent, MPServerProvider.App.Views.MainWindow.<.ctor>b__2_0, System.EventHandler..ctor, System.Windows.Window.add_StateChanged, MPServerProvider.App.Views.MainWindow.ShowPage

### `NavigateTo(string)` -> `void`
- token `0x060000b1`  flags=`134`  rva=`19908`
- IL size 136 bytes, 53 instructions, max_stack=2
- opcodes: callx6, ldarg.0x6, br.sx5, ldarg.1x5, ldfldx5, stloc.1x5, brtrue.sx4, ldstrx4, callvirtx2, ldloc.0x2, retx2, brfalse.sx1, ldc.i4.1x1, ldloc.1x1, ldloca.sx1, newobjx1, stloc.0x1, stloc.2x1
- string literals: "library", "mods", "servers", "settings"
- calls: System.String.op_Equality, System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, MPServerProvider.App.Views.MainWindow.ShowPage, System.Nullable`1..ctor, System.Windows.Controls.Primitives.ToggleButton.set_IsChecked

### `RefreshHome()` -> `void`
- token `0x060000b2`  flags=`134`  rva=`20056`
- IL size 37 bytes, 14 instructions, max_stack=3
- opcodes: brfalse.sx2, callvirtx2, ldloc.1x2, isinstx1, ldarg.0x1, ldfldx1, ldloc.0x1, ldloca.sx1, ldstrx1, retx1, stloc.1x1
- string literals: "home"
- calls: System.Collections.Generic.Dictionary`2.TryGetValue, MPServerProvider.App.Views.HomePage.RefreshNow

### `ShowGameInLibrary(string)` -> `void`
- token `0x060000b3`  flags=`134`  rva=`20108`
- IL size 49 bytes, 18 instructions, max_stack=3
- opcodes: brfalse.sx2, callvirtx2, ldarg.0x2, ldloc.1x2, ldstrx2, callx1, isinstx1, ldarg.1x1, ldfldx1, ldloc.0x1, ldloca.sx1, retx1, stloc.1x1
- string literals: "library", "library"
- calls: MPServerProvider.App.Views.MainWindow.NavigateTo, System.Collections.Generic.Dictionary`2.TryGetValue, MPServerProvider.App.Views.LibraryPage.ShowGame

### `OnNavChecked(object, class:329)` -> `void`
- token `0x060000b4`  flags=`129`  rva=`20172`
- IL size 33 bytes, 15 instructions, max_stack=2
- opcodes: brfalse.sx2, isinstx2, ldloc.0x2, ldloc.1x2, callx1, callvirtx1, ldarg.0x1, ldarg.1x1, retx1, stloc.0x1, stloc.1x1
- calls: System.Windows.FrameworkElement.get_Tag, MPServerProvider.App.Views.MainWindow.ShowPage

### `ShowPage(string)` -> `void`
- token `0x060000b5`  flags=`129`  rva=`20220`
- IL size 152 bytes, 56 instructions, max_stack=3
- opcodes: brtrue.sx6, ldarg.1x6, br.sx5, newobjx5, stloc.1x5, callx4, callvirtx4, ldstrx4, ldarg.0x3, ldfldx3, ldloc.0x3, retx2, dupx1, isinstx1, ldloc.1x1, ldloca.sx1, popx1, stloc.0x1
- string literals: "library", "mods", "servers", "settings"
- calls: System.Collections.Generic.Dictionary`2.TryGetValue, System.String.op_Equality, MPServerProvider.App.Views.LibraryPage..ctor, MPServerProvider.App.Views.ModsPage..ctor, MPServerProvider.App.Views.MyServersPage..ctor, MPServerProvider.App.Views.SettingsPage..ctor, MPServerProvider.App.Views.HomePage..ctor, System.Collections.Generic.Dictionary`2.set_Item, System.Windows.Controls.ContentControl.set_Content, MPServerProvider.App.Views.INavigationAware.OnNavigatedTo

### `OnTitleBarMouseDown(object, class:345)` -> `void`
- token `0x060000b6`  flags=`129`  rva=`20384`
- IL size 34 bytes, 16 instructions, max_stack=8
- opcodes: ldarg.2x3, bne.un.sx2, callx2, callvirtx2, ldarg.0x2, retx2, ldarg.1x1, ldc.i4.1x1, ldc.i4.2x1
- calls: System.Windows.Input.MouseButtonEventArgs.get_ClickCount, MPServerProvider.App.Views.MainWindow.OnMaximizeRestore, System.Windows.Input.MouseEventArgs.get_LeftButton, System.Windows.Window.DragMove

### `OnMaximizeRestore(object, class:329)` -> `void`
- token `0x060000b7`  flags=`129`  rva=`20419`
- IL size 20 bytes, 10 instructions, max_stack=8
- opcodes: callx2, ldarg.0x2, ldc.i4.2x2, beq.sx1, br.sx1, ldc.i4.0x1, retx1
- calls: System.Windows.Window.get_WindowState, System.Windows.Window.set_WindowState

### `OnToggleFullscreen(object, class:329)` -> `void`
- token `0x060000b8`  flags=`129`  rva=`20440`
- IL size 199 bytes, 65 instructions, max_stack=3
- opcodes: callx21, ldarg.0x19, ldc.i4.0x7, ldc.r8x4, ldloca.sx4, ldfldx2, retx2, brfalse.sx1, ceqx1, ldc.i4.1x1, ldc.i4.2x1, stfldx1, stloc.0x1
- calls: System.Windows.SystemParameters.get_WorkArea, System.Windows.Window.set_WindowState, System.Windows.Window.set_WindowStyle, System.Windows.Window.set_ResizeMode, System.Windows.Rect.get_Left, System.Windows.Window.set_Left, System.Windows.Rect.get_Top, System.Windows.Window.set_Top, System.Windows.Rect.get_Width, System.Windows.FrameworkElement.set_Width, System.Windows.Rect.get_Height, System.Windows.FrameworkElement.set_Height, System.Windows.Window.set_Topmost

### `OnClose(object, class:329)` -> `void`
- token `0x060000b9`  flags=`129`  rva=`20651`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Windows.Window.Close

### `InitializeComponent()` -> `void`
- token `0x060000ba`  flags=`486`  rva=`20660`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/mainwindow.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x060000bb`  flags=`481`  rva=`20708`
- IL size 362 bytes, 101 instructions, max_stack=3
- opcodes: ldarg.0x23, retx11, castclassx10, ldarg.2x10, callvirtx9, ldftnx9, newobjx9, stfldx8, ldfldx6, ldc.i4.1x2, brx1, ldarg.1x1, subx1, switchx1
- calls: MPServerProvider.App.Views.MainWindow.OnTitleBarMouseDown, System.Windows.Input.MouseButtonEventHandler..ctor, System.Windows.UIElement.add_MouseLeftButtonDown, MPServerProvider.App.Views.MainWindow.OnNavChecked, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ToggleButton.add_Checked, MPServerProvider.App.Views.MainWindow.OnToggleFullscreen, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.MainWindow.OnMaximizeRestore, MPServerProvider.App.Views.MainWindow.OnClose

### `<.ctor>b__2_0(object, class:269)` -> `void`
- token `0x060000bc`  flags=`129`  rva=`21082`
- IL size 33 bytes, 11 instructions, max_stack=8
- opcodes: ldarg.0x2, ldstrx2, beq.sx1, br.sx1, callx1, callvirtx1, ldc.i4.2x1, ldfldx1, retx1
- string literals: "", ""
- calls: System.Windows.Window.get_WindowState, System.Windows.Controls.ContentControl.set_Content

