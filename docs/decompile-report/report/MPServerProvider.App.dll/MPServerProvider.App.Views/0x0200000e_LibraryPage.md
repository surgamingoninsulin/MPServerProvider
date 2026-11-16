# MPServerProvider.App.Views.LibraryPage

Token: `0x0200000e`  Flags: `1048577`  Extends: `0x01000074`

## Fields

- `_allCards` : `class:289<class:40>`  (flags=33, token=0x04000082)
- `_activeGenre` : `string`  (flags=1, token=0x04000083)
- `SearchBox` : `class:301`  (flags=3, token=0x04000084)
- `SearchHint` : `class:389`  (flags=3, token=0x04000085)
- `CountText` : `class:389`  (flags=3, token=0x04000086)
- `GenreChips` : `class:297`  (flags=3, token=0x04000087)
- `CardsHost` : `class:481`  (flags=3, token=0x04000088)
- `_contentLoaded` : `bool`  (flags=1, token=0x04000089)

## Methods

### `.ctor()` -> `void`
- token `0x0600009a`  flags=`6278`  rva=`17739`
- IL size 41 bytes, 13 instructions, max_stack=8
- opcodes: ldarg.0x5, callx3, stfldx2, ldstrx1, newobjx1, retx1
- string literals: ""
- calls: System.Collections.Generic.List`1..ctor, System.Windows.Controls.UserControl..ctor, MPServerProvider.App.Views.LibraryPage.InitializeComponent, MPServerProvider.App.Views.LibraryPage.Reload

### `OnNavigatedTo()` -> `void`
- token `0x0600009b`  flags=`486`  rva=`17781`
- IL size 17 bytes, 5 instructions, max_stack=8
- opcodes: callx2, callvirtx1, ldarg.0x1, retx1
- calls: MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Services.GameLibraryService.Reload, MPServerProvider.App.Views.LibraryPage.Reload

### `Reload()` -> `void`
- token `0x0600009c`  flags=`129`  rva=`17800`
- IL size 139 bytes, 45 instructions, max_stack=3
- opcodes: callvirtx8, ldarg.0x5, callx4, ldloc.0x4, ldfldx3, newobjx3, brtrue.sx2, dupx2, ldsfldx2, br.sx1, brfalse.sx1, endfinallyx1, ldftnx1, ldloc.1x1, ldnullx1, leave.sx1, popx1, retx1, stloc.0x1, stloc.1x1, stsfldx1
- calls: System.Collections.Generic.List`1.Clear, MPServerProvider.App.Views.GameCardVm..ctor, System.Collections.Generic.List`1.Add, MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Services.GameLibraryService.get_Games, <>c.<Reload>b__4_0, System.Func`2..ctor, System.Linq.Enumerable.OrderBy, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, MPServerProvider.App.Views.LibraryPage.BuildGenreChips, MPServerProvider.App.Views.LibraryPage.ApplyFilter

### `ShowGame(string)` -> `void`
- token `0x0600009d`  flags=`134`  rva=`17968`
- IL size 142 bytes, 45 instructions, max_stack=3
- opcodes: callvirtx11, callx5, ldarg.0x4, ldloc.1x4, brtrue.sx2, ldfldx2, ldloc.0x2, ldstrx2, retx2, br.sx1, brfalse.sx1, dupx1, endfinallyx1, isinstx1, ldarg.1x1, leave.sx1, newobjx1, stfldx1, stloc.0x1, stloc.1x1
- string literals: "", ""
- calls: MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Services.GameLibraryService.Reload, MPServerProvider.App.Views.LibraryPage.Reload, MPServerProvider.Core.Services.GameLibraryService.Find, System.Windows.Controls.Panel.get_Children, System.Linq.Enumerable.OfType, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.Windows.FrameworkElement.get_Tag, System.String.op_Equality, System.Nullable`1..ctor, System.Windows.Controls.Primitives.ToggleButton.set_IsChecked, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, MPServerProvider.Core.Models.GameDefinition.get_Name, System.Windows.Controls.TextBox.set_Text

### `BuildGenreChips()` -> `void`
- token `0x0600009e`  flags=`129`  rva=`18140`
- IL size 88 bytes, 32 instructions, max_stack=4
- opcodes: callvirtx7, ldloc.0x4, callx3, ldarg.0x3, ldloc.1x2, ldstrx2, br.sx1, brfalse.sx1, brtrue.sx1, endfinallyx1, ldc.i4.0x1, ldc.i4.1x1, ldfldx1, leave.sx1, retx1, stloc.0x1, stloc.1x1
- string literals: "All Games", ""
- calls: System.Windows.Controls.Panel.get_Children, System.Windows.Controls.UIElementCollection.Clear, MPServerProvider.App.Views.LibraryPage.AddChip, MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Services.GameLibraryService.AllGenres, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose

### `AddChip(string, string, bool)` -> `void`
- token `0x0600009f`  flags=`129`  rva=`18256`
- IL size 127 bytes, 42 instructions, max_stack=4
- opcodes: callvirtx8, dupx5, ldloc.0x4, newobjx4, ldarg.0x3, ldfldx2, ldloc.1x2, ldstrx2, stfldx2, callx1, castclassx1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldftnx1, popx1, retx1, stloc.0x1, stloc.1x1
- string literals: "libraryGenre", "FilterChip"
- calls: <>c__DisplayClass7_0..ctor, System.Windows.Controls.RadioButton..ctor, System.Windows.Controls.ContentControl.set_Content, System.Windows.Controls.RadioButton.set_GroupName, System.Windows.FrameworkElement.FindResource, System.Windows.FrameworkElement.set_Style, System.Nullable`1..ctor, System.Windows.Controls.Primitives.ToggleButton.set_IsChecked, System.Windows.FrameworkElement.set_Tag, <>c__DisplayClass7_0.<AddChip>b__0, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ToggleButton.add_Checked, System.Windows.Controls.Panel.get_Children, System.Windows.Controls.UIElementCollection.Add

### `OnFilterChanged(object, class:393)` -> `void`
- token `0x060000a0`  flags=`129`  rva=`18395`
- IL size 40 bytes, 14 instructions, max_stack=8
- opcodes: ldarg.0x3, callx2, callvirtx2, ldfldx2, br.sx1, brtrue.sx1, ldc.i4.0x1, ldc.i4.2x1, retx1
- calls: System.Windows.Controls.TextBox.get_Text, System.String.IsNullOrEmpty, System.Windows.UIElement.set_Visibility, MPServerProvider.App.Views.LibraryPage.ApplyFilter

### `ApplyFilter()` -> `void`
- token `0x060000a1`  flags=`129`  rva=`18436`
- IL size 214 bytes, 71 instructions, max_stack=4
- opcodes: callx10, ldloca.sx6, ldarg.0x5, ldstrx5, dupx4, ldfldx4, brtrue.sx3, callvirtx3, ldloc.0x3, newobjx3, popx3, br.sx2, ldftnx2, ldloc.1x2, ldloc.2x2, ldsfldx2, stfldx2, beq.sx1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.sx1, ldnullx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1, stsfldx1
- string literals: "", " game", "s", "", " available"
- calls: <>c__DisplayClass9_0..ctor, System.Windows.Controls.TextBox.get_Text, System.String.Trim, <>c__DisplayClass9_0.<ApplyFilter>b__0, System.Func`2..ctor, System.Linq.Enumerable.Where, System.Linq.Enumerable.ToList, System.Windows.Controls.ItemsControl.set_ItemsSource, <>c.<ApplyFilter>b__9_1, System.Linq.Enumerable.Count, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Windows.Controls.TextBlock.set_Text

### `OnInstallClick(object, class:329)` -> `void`
- token `0x060000a2`  flags=`129`  rva=`18664`
- IL size 59 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx5, callx2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `OnCopyBakeInCode(object, class:329)` -> `void`
- token `0x060000a3`  flags=`129`  rva=`18736`
- IL size 123 bytes, 38 instructions, max_stack=3
- opcodes: ldstrx7, callx6, callvirtx5, ldloc.0x4, brfalse.sx2, isinstx2, ldloc.1x2, leave.sx2, retx2, brtrue.sx1, ldarg.1x1, nopx1, popx1, stloc.0x1, stloc.1x1
- string literals: "Pick a real game — the Custom Game Server tile can't be exported", "", "Copied C# for '", "' — paste it into BuiltInGames.cs", "", "Couldn't access the clipboard", ""
- calls: System.Windows.FrameworkElement.get_DataContext, MPServerProvider.App.Views.GameCardVm.get_Game, MPServerProvider.App.ToastService.Show, MPServerProvider.Core.Services.GameDefinitionCSharp.Render, System.Windows.Clipboard.SetText, MPServerProvider.Core.Models.GameDefinition.get_Name, System.String.Concat

### `OnAddCustomGame(object, class:329)` -> `void`
- token `0x060000a4`  flags=`129`  rva=`18888`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `InitializeComponent()` -> `void`
- token `0x060000a5`  flags=`486`  rva=`18944`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/librarypage.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x060000a6`  flags=`481`  rva=`18992`
- IL size 126 bytes, 40 instructions, max_stack=3
- opcodes: ldarg.0x8, retx6, stfldx6, castclassx5, ldarg.2x5, ldc.i4.1x2, br.sx1, callvirtx1, ldarg.1x1, ldfldx1, ldftnx1, newobjx1, subx1, switchx1
- calls: MPServerProvider.App.Views.LibraryPage.OnFilterChanged, System.Windows.Controls.TextChangedEventHandler..ctor, System.Windows.Controls.Primitives.TextBoxBase.add_TextChanged

### `System.Windows.Markup.IStyleConnector.Connect(i4, object)` -> `void`
- token `0x060000a7`  flags=`481`  rva=`19130`
- IL size 57 bytes, 21 instructions, max_stack=8
- opcodes: retx3, beq.sx2, callvirtx2, castclassx2, ldarg.0x2, ldarg.1x2, ldarg.2x2, ldftnx2, newobjx2, ldc.i4.6x1, ldc.i4.7x1
- calls: MPServerProvider.App.Views.LibraryPage.OnCopyBakeInCode, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.MenuItem.add_Click, MPServerProvider.App.Views.LibraryPage.OnInstallClick, System.Windows.Controls.Primitives.ButtonBase.add_Click

