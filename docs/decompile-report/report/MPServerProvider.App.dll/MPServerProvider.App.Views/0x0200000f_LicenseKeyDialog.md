# MPServerProvider.App.Views.LicenseKeyDialog

Token: `0x0200000f`  Flags: `1048577`  Extends: `0x01000044`

## Fields

- `_serverCfgPath` : `string`  (flags=33, token=0x0400008a)
- `KeyBox` : `class:301`  (flags=3, token=0x0400008b)
- `KeyHint` : `class:389`  (flags=3, token=0x0400008c)
- `_contentLoaded` : `bool`  (flags=1, token=0x0400008d)

## Methods

### `.ctor(string)` -> `void`
- token `0x060000a8`  flags=`6278`  rva=`19188`
- IL size 20 bytes, 8 instructions, max_stack=8
- opcodes: ldarg.0x3, callx2, ldarg.1x1, retx1, stfldx1
- calls: System.Windows.Window..ctor, MPServerProvider.App.Views.LicenseKeyDialog.InitializeComponent

### `ReadKey(string)` -> `string`
- token `0x060000a9`  flags=`150`  rva=`19212`
- IL size 91 bytes, 34 instructions, max_stack=3
- opcodes: callvirtx7, brtrue.sx3, callx3, ldloc.1x3, ldstrx3, retx3, ldarg.0x2, ldloc.0x2, ldnullx2, br.sx1, brfalse.sx1, ldc.i4.1x1, ldc.i4.5x1, stloc.0x1, stloc.1x1
- string literals: "sv_licenseKey\s+"?([^"\r\n]*)"?", "", "changeme"
- calls: System.IO.File.Exists, System.IO.File.ReadAllText, System.Text.RegularExpressions.Regex.Match, System.Text.RegularExpressions.Group.get_Success, System.Text.RegularExpressions.Match.get_Groups, System.Text.RegularExpressions.GroupCollection.get_Item, System.Text.RegularExpressions.Capture.get_Value, System.String.Trim, System.String.get_Length, System.String.Equals

### `WriteKey(string, string)` -> `void`
- token `0x060000aa`  flags=`150`  rva=`19316`
- IL size 153 bytes, 49 instructions, max_stack=5
- opcodes: callx16, ldloca.sx7, ldstrx7, ldloc.0x4, ldarg.0x3, stloc.0x3, br.sx2, ldarg.1x2, brfalse.sx1, brtrue.sx1, ldc.i4.3x1, ldc.i4.sx1, retx1
- string literals: "", "sv_licenseKey", "sv_licenseKey\s+"?[^"\r\n]*"?", "sv_licenseKey "", """, "sv_licenseKey "", """
- calls: System.IO.File.Exists, System.IO.File.ReadAllText, System.Text.RegularExpressions.Regex.IsMatch, System.String.Concat, System.Text.RegularExpressions.Regex.Replace, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Environment.get_NewLine, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.IO.File.WriteAllText

### `OnKeyChanged(object, class:393)` -> `void`
- token `0x060000ab`  flags=`129`  rva=`19481`
- IL size 34 bytes, 12 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x2, ldfldx2, br.sx1, brtrue.sx1, callx1, ldc.i4.0x1, ldc.i4.2x1, retx1
- calls: System.Windows.Controls.TextBox.get_Text, System.String.IsNullOrEmpty, System.Windows.UIElement.set_Visibility

### `OnSave(object, class:329)` -> `void`
- token `0x060000ac`  flags=`129`  rva=`19516`
- IL size 99 bytes, 32 instructions, max_stack=2
- opcodes: callx5, callvirtx4, ldstrx4, ldarg.0x3, ldfldx2, ldloc.0x2, leave.sx2, retx2, bge.sx1, ldc.i4.1x1, ldc.i4.8x1, ldloc.1x1, newobjx1, nopx1, stloc.0x1, stloc.1x1
- string literals: "That doesn't look like a valid licence key", "", "Could not save the key: ", ""
- calls: System.Windows.Controls.TextBox.get_Text, System.String.Trim, System.String.get_Length, MPServerProvider.App.ToastService.Show, MPServerProvider.App.Views.LicenseKeyDialog.WriteKey, System.Nullable`1..ctor, System.Windows.Window.set_DialogResult, System.Exception.get_Message, System.String.Concat

### `OnCancel(object, class:329)` -> `void`
- token `0x060000ad`  flags=`129`  rva=`19644`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldc.i4.0x1, newobjx1, retx1
- calls: System.Nullable`1..ctor, System.Windows.Window.set_DialogResult

### `InitializeComponent()` -> `void`
- token `0x060000ae`  flags=`486`  rva=`19660`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/licensekeydialog.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x060000af`  flags=`481`  rva=`19708`
- IL size 131 bytes, 39 instructions, max_stack=3
- opcodes: ldarg.0x7, retx5, castclassx4, ldarg.2x4, callvirtx3, ldftnx3, newobjx3, stfldx3, ldc.i4.1x2, br.sx1, ldarg.1x1, ldfldx1, subx1, switchx1
- calls: MPServerProvider.App.Views.LicenseKeyDialog.OnKeyChanged, System.Windows.Controls.TextChangedEventHandler..ctor, System.Windows.Controls.Primitives.TextBoxBase.add_TextChanged, MPServerProvider.App.Views.LicenseKeyDialog.OnCancel, System.Windows.RoutedEventHandler..ctor, System.Windows.Controls.Primitives.ButtonBase.add_Click, MPServerProvider.App.Views.LicenseKeyDialog.OnSave

