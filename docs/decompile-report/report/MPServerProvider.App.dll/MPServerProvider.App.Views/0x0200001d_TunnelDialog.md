# MPServerProvider.App.Views.TunnelDialog

Token: `0x0200001d`  Flags: `1048577`  Extends: `0x01000044`

## Fields

- `ServerCombo` : `class:321`  (flags=3, token=0x0400011a)
- `TabUpnp` : `class:293`  (flags=3, token=0x0400011b)
- `TabPlayit` : `class:293`  (flags=3, token=0x0400011c)
- `UpnpPanel` : `class:297`  (flags=3, token=0x0400011d)
- `UpnpResult` : `class:389`  (flags=3, token=0x0400011e)
- `UpnpAddressBox` : `class:317`  (flags=3, token=0x0400011f)
- `UpnpAddressText` : `class:389`  (flags=3, token=0x04000120)
- `UnmapButton` : `class:305`  (flags=3, token=0x04000121)
- `MapButton` : `class:305`  (flags=3, token=0x04000122)
- `PlayitPanel` : `class:297`  (flags=3, token=0x04000123)
- `PlayitScroll` : `class:477`  (flags=3, token=0x04000124)
- `PlayitOutput` : `class:301`  (flags=3, token=0x04000125)
- `StopAgentButton` : `class:305`  (flags=3, token=0x04000126)
- `StartAgentButton` : `class:305`  (flags=3, token=0x04000127)
- `_contentLoaded` : `bool`  (flags=1, token=0x04000128)

## Methods

### `.ctor(class:497)` -> `void`
- token `0x06000149`  flags=`6278`  rva=`37492`
- IL size 147 bytes, 51 instructions, max_stack=3
- opcodes: callx8, callvirtx6, ldarg.0x5, brtrue.sx3, dupx3, ldfldx3, br.sx2, ldarg.1x2, ldc.i4.0x2, ldloc.0x2, ldloc.1x2, ldsfldx2, popx2, brfalse.sx1, ceqx1, ldftnx1, ldnullx1, newobjx1, retx1, stloc.0x1, stloc.1x1, stsfldx1
- calls: System.Windows.Window..ctor, MPServerProvider.App.Views.TunnelDialog.InitializeComponent, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.get_Servers, <>c.<.ctor>b__0_0, System.Func`2..ctor, System.Linq.Enumerable.Select, System.Linq.Enumerable.ToList, System.Windows.Controls.ItemsControl.set_ItemsSource, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.Linq.Enumerable.FirstOrDefault, System.Windows.Controls.Primitives.Selector.set_SelectedItem, MPServerProvider.App.AppServices.get_Tunnel, MPServerProvider.Core.Services.TunnelService.get_PlayitProcess, System.Diagnostics.Process.get_HasExited, System.Windows.UIElement.set_IsEnabled

### `get_SelectedServer()` -> `class:497`
- token `0x0600014a`  flags=`2177`  rva=`37652`
- IL size 34 bytes, 13 instructions, max_stack=2
- opcodes: callvirtx2, ldloc.0x2, retx2, brtrue.sx1, callx1, isinstx1, ldarg.0x1, ldfldx1, ldnullx1, stloc.0x1
- calls: System.Windows.Controls.Primitives.Selector.get_SelectedItem, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.Find

### `OnTabChanged(object, class:329)` -> `void`
- token `0x0600014b`  flags=`129`  rva=`37700`
- IL size 82 bytes, 31 instructions, max_stack=2
- opcodes: ldarg.0x5, ldfldx5, callvirtx4, brtrue.sx3, br.sx2, callx2, ldc.i4.0x2, ldc.i4.2x2, ldloca.sx2, retx2, stloc.0x2
- calls: System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, System.Windows.UIElement.set_Visibility

### `OnMap(object, class:329)` -> `void`
- token `0x0600014c`  flags=`129`  rva=`37796`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `OnCopyUpnpAddress(object, class:329)` -> `void`
- token `0x0600014d`  flags=`129`  rva=`37852`
- IL size 67 bytes, 17 instructions, max_stack=3
- opcodes: ldstrx6, callx3, callvirtx2, leave.sx2, ldarg.0x1, ldfldx1, popx1, retx1
- string literals: "Connect: ", "", "Address copied to clipboard", "", "Could not copy to clipboard", ""
- calls: System.Windows.Controls.TextBlock.get_Text, System.String.Replace, System.Windows.Clipboard.SetText, MPServerProvider.App.ToastService.Show

### `OnUnmap(object, class:329)` -> `void`
- token `0x0600014e`  flags=`129`  rva=`37948`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `OnStartAgent(object, class:329)` -> `void`
- token `0x0600014f`  flags=`129`  rva=`38004`
- IL size 43 bytes, 14 instructions, max_stack=2
- opcodes: ldloca.sx5, stfldx3, callx2, ldarg.0x1, ldc.i4.m1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start

### `OnStopAgent(object, class:329)` -> `void`
- token `0x06000150`  flags=`129`  rva=`38059`
- IL size 51 bytes, 15 instructions, max_stack=8
- opcodes: callvirtx4, ldarg.0x3, ldfldx3, callx1, ldc.i4.0x1, ldc.i4.1x1, ldstrx1, retx1
- string literals: "Agent stopped.
"
- calls: MPServerProvider.App.AppServices.get_Tunnel, MPServerProvider.Core.Services.TunnelService.StopPlayit, System.Windows.Controls.Primitives.TextBoxBase.AppendText, System.Windows.UIElement.set_IsEnabled

### `OnCopyPlayitAddress(object, class:329)` -> `void`
- token `0x06000151`  flags=`129`  rva=`38112`
- IL size 130 bytes, 51 instructions, max_stack=3
- opcodes: ldstrx7, callx6, callvirtx4, ldloc.2x4, ldloc.1x3, leave.sx3, ldc.i4.1x2, ldelem.refx2, ldloc.0x2, stloc.2x2, subx2, bge.sx1, br.sx1, brfalse.sx1, conv.i4x1, ldarg.0x1, ldc.i4.0x1, ldc.i4.3x1, ldc.i4.sx1, ldfldx1, ldlenx1, popx1, retx1, stloc.0x1, stloc.1x1
- string literals: "[\w.-]+\.\w+:\d+", "Address copied to clipboard", "", "Playit output copied to clipboard", "", "Could not copy to clipboard", ""
- calls: System.Windows.Controls.TextBox.get_Text, System.String.Split, System.Text.RegularExpressions.Regex.IsMatch, System.String.Trim, System.Windows.Clipboard.SetText, MPServerProvider.App.ToastService.Show

### `OnClose(object, class:329)` -> `void`
- token `0x06000152`  flags=`129`  rva=`38272`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Windows.Window.Close

### `InitializeComponent()` -> `void`
- token `0x06000153`  flags=`486`  rva=`38280`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/tunneldialog.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x06000154`  flags=`481`  rva=`38328`
- IL size 481 bytes, 136 instructions, max_stack=3
- opcodes: ldarg.0x30, retx18, castclassx17, ldarg.2x17, stfldx15, callvirtx9, ldftnx9, newobjx9, ldfldx6, ldc.i4.1x2, brx1, ldarg.1x1, subx1, switchx1
- calls: MPServerProvider.App.Views.TunnelDialog.OnTabChanged, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ToggleButton.add_Checked, MPServerProvider.App.Views.TunnelDialog.OnCopyUpnpAddress, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.TunnelDialog.OnUnmap, MPServerProvider.App.Views.TunnelDialog.OnMap, MPServerProvider.App.Views.TunnelDialog.OnCopyPlayitAddress, MPServerProvider.App.Views.TunnelDialog.OnStopAgent, MPServerProvider.App.Views.TunnelDialog.OnStartAgent, MPServerProvider.App.Views.TunnelDialog.OnClose

### `<OnStartAgent>b__7_0(string)` -> `void`
- token `0x06000155`  flags=`129`  rva=`38824`
- IL size 50 bytes, 17 instructions, max_stack=3
- opcodes: ldloc.0x3, callx2, ldarg.0x2, newobjx2, stfldx2, callvirtx1, ldarg.1x1, ldftnx1, popx1, retx1, stloc.0x1
- calls: <>c__DisplayClass7_0..ctor, System.Windows.Threading.DispatcherObject.get_Dispatcher, <>c__DisplayClass7_0.<OnStartAgent>b__1, System.Action..ctor, System.Array.Empty, System.Windows.Threading.Dispatcher.BeginInvoke

