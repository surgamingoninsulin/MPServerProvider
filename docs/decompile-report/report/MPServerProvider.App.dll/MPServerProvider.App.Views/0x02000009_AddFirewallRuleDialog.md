# MPServerProvider.App.Views.AddFirewallRuleDialog

Token: `0x02000009`  Flags: `1048577`  Extends: `0x01000044`

## Fields

- `<RuleName>k__BackingField` : `string`  (flags=1, token=0x04000048)
- `<Port>k__BackingField` : `i4`  (flags=1, token=0x04000049)
- `<Protocol>k__BackingField` : `string`  (flags=1, token=0x0400004a)
- `NameBox` : `class:301`  (flags=3, token=0x0400004b)
- `NameHint` : `class:389`  (flags=3, token=0x0400004c)
- `PortBox` : `class:301`  (flags=3, token=0x0400004d)
- `PortHint` : `class:389`  (flags=3, token=0x0400004e)
- `ProtocolCombo` : `class:321`  (flags=3, token=0x0400004f)
- `_contentLoaded` : `bool`  (flags=1, token=0x04000050)

## Methods

### `get_RuleName()` -> `string`
- token `0x0600004e`  flags=`2182`  rva=`12669`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_RuleName(string)` -> `void`
- token `0x0600004f`  flags=`2177`  rva=`12677`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Port()` -> `i4`
- token `0x06000050`  flags=`2182`  rva=`12686`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Port(i4)` -> `void`
- token `0x06000051`  flags=`2177`  rva=`12694`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Protocol()` -> `string`
- token `0x06000052`  flags=`2182`  rva=`12703`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Protocol(string)` -> `void`
- token `0x06000053`  flags=`2177`  rva=`12711`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `.ctor()` -> `void`
- token `0x06000054`  flags=`6278`  rva=`12720`
- IL size 35 bytes, 11 instructions, max_stack=8
- opcodes: ldarg.0x4, callx2, ldstrx2, stfldx2, retx1
- string literals: "", "both"
- calls: System.Windows.Window..ctor, MPServerProvider.App.Views.AddFirewallRuleDialog.InitializeComponent

### `OnNameChanged(object, class:393)` -> `void`
- token `0x06000055`  flags=`129`  rva=`12756`
- IL size 34 bytes, 12 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x2, ldfldx2, br.sx1, brtrue.sx1, callx1, ldc.i4.0x1, ldc.i4.2x1, retx1
- calls: System.Windows.Controls.TextBox.get_Text, System.String.IsNullOrEmpty, System.Windows.UIElement.set_Visibility

### `OnPortChanged(object, class:393)` -> `void`
- token `0x06000056`  flags=`129`  rva=`12791`
- IL size 34 bytes, 12 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x2, ldfldx2, br.sx1, brtrue.sx1, callx1, ldc.i4.0x1, ldc.i4.2x1, retx1
- calls: System.Windows.Controls.TextBox.get_Text, System.String.IsNullOrEmpty, System.Windows.UIElement.set_Visibility

### `OnAdd(object, class:329)` -> `void`
- token `0x06000057`  flags=`129`  rva=`12828`
- IL size 195 bytes, 75 instructions, max_stack=2
- opcodes: callx7, ldarg.0x7, ldstrx7, callvirtx6, br.sx4, ldc.i4.1x4, stloc.sx4, ldfldx3, ldloc.1x3, ldloc.sx3, retx3, beq.sx2, brtrue.sx2, ldc.i4.0x2, ldloc.0x2, ldloc.2x2, stloc.2x2, stloc.3x2, ble.sx1, blt.sx1, brfalse.sx1, ceqx1, ldc.i4x1, ldc.i4.2x1, ldloc.3x1, ldloca.sx1, newobjx1, stloc.0x1
- string literals: "Enter a rule name", "", "Enter a valid port (1–65535)", "", "tcp", "udp", "both"
- calls: System.Windows.Controls.TextBox.get_Text, System.String.Trim, System.String.get_Length, MPServerProvider.App.ToastService.Show, System.Int32.TryParse, MPServerProvider.App.Views.AddFirewallRuleDialog.set_RuleName, MPServerProvider.App.Views.AddFirewallRuleDialog.set_Port, System.Windows.Controls.Primitives.Selector.get_SelectedIndex, MPServerProvider.App.Views.AddFirewallRuleDialog.set_Protocol, System.Nullable`1..ctor, System.Windows.Window.set_DialogResult

### `OnCancel(object, class:329)` -> `void`
- token `0x06000058`  flags=`129`  rva=`13035`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldc.i4.0x1, newobjx1, retx1
- calls: System.Nullable`1..ctor, System.Windows.Window.set_DialogResult

### `InitializeComponent()` -> `void`
- token `0x06000059`  flags=`486`  rva=`13052`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/addfirewallruledialog.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x0600005a`  flags=`481`  rva=`13100`
- IL size 208 bytes, 60 instructions, max_stack=3
- opcodes: ldarg.0x12, retx8, castclassx7, ldarg.2x7, stfldx6, callvirtx4, ldftnx4, newobjx4, ldc.i4.1x2, ldfldx2, brx1, ldarg.1x1, subx1, switchx1
- calls: MPServerProvider.App.Views.AddFirewallRuleDialog.OnNameChanged, System.Windows.Controls.TextChangedEventHandler..ctor, System.Windows.Controls.Primitives.TextBoxBase.add_TextChanged, MPServerProvider.App.Views.AddFirewallRuleDialog.OnPortChanged, MPServerProvider.App.Views.AddFirewallRuleDialog.OnCancel, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.AddFirewallRuleDialog.OnAdd

