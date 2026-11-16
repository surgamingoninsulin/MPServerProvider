# <OnInstall>d__11

Token: `0x0200002a`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400015d)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x0400015e)
- `<>4__this` : `class:52`  (flags=6, token=0x0400015f)
- `<>u__1` : `valuetype:361<class:497>`  (flags=1, token=0x04000160)

## Methods

### `MoveNext()` -> `void`
- token `0x06000174`  flags=`481`  rva=`44780`
- IL size 905 bytes, 290 instructions, max_stack=12
- opcodes: callvirtx32, callx27, ldloc.1x24, ldloc.sx23, ldstrx23, ldfldx21, stloc.sx20, ldarg.0x13, brfalse.sx8, ldc.i4.0x8, ldloca.sx8, brtrue.sx7, leavex7, stfldx6, leave.sx5, newobjx5, popx5, ldfldax4, ldloc.2x4, dupx3, ldc.i4x3, ldc.i4.1x3, ldc.i4.sx3, ldloc.3x3, stloc.0x3, br.sx2, isinstx2, ldc.i4.2x2, ldloc.0x2, bge.sx1, bgt.sx1, ble.sx1, blt.sx1, brfalsex1, ceqx1, initobjx1, ldc.i4.5x1, ldc.i4.m1x1, ldftnx1, nopx1, retx1, stloc.1x1, stloc.2x1
- string literals: "Enter a server name", "", "A server with this name already exists", "", "Enter a valid port (1–65535)", "", "minecraft", "The Minecraft EULA must be accepted to install a server", "", "manual", "Select a folder with server files", "", "", "", "Loading", "unavailable", "Pick a version first", "", "Installing ", "…", "", "Install failed:

", "MPServerProvider"
- calls: System.Windows.Controls.TextBox.get_Text, System.String.Trim, System.String.get_Length, MPServerProvider.App.ToastService.Show, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.NameTaken, System.Int32.TryParse, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.StartsWith, System.Windows.Controls.Primitives.ToggleButton.get_IsChecked, System.Nullable`1.GetValueOrDefault, MPServerProvider.Core.Models.GameDefinition.get_Install, MPServerProvider.Core.Models.InstallSpec.get_Provider, System.String.op_Equality, System.IO.Directory.Exists, System.Windows.Controls.Primitives.Selector.get_SelectedItem, System.Math.Max, System.Windows.UIElement.set_Visibility, System.String.Concat, System.Windows.Controls.TextBlock.set_Text, System.Threading.CancellationTokenSource..ctor, MPServerProvider.App.Views.InstallServerDialog.<OnInstall>b__11_0, System.Action`1..ctor, System.Progress`1..ctor, MPServerProvider.App.AppServices.get_Installer, System.Threading.CancellationTokenSource.get_Token, MPServerProvider.Core.Services.InstallService.InstallAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.App.Views.InstallServerDialog.set_InstalledServer, System.Nullable`1..ctor, System.Windows.Window.set_DialogResult, System.Exception.get_Message, System.Windows.MessageBox.Show, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x06000175`  flags=`481`  rva=`45776`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

