# <OnInstall>d__9

Token: `0x02000031`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000176)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x04000177)
- `<>4__this` : `class:72`  (flags=6, token=0x04000178)
- `<>u__1` : `valuetype:361<class:497>`  (flags=1, token=0x04000179)

## Methods

### `MoveNext()` -> `void`
- token `0x06000184`  flags=`481`  rva=`47320`
- IL size 432 bytes, 146 instructions, max_stack=12
- opcodes: callx16, ldarg.0x13, ldstrx10, callvirtx9, stloc.sx9, ldfldx7, ldloc.1x7, ldloc.sx7, ldc.i4.0x5, stfldx5, ldfldax4, ldloca.sx4, leavex4, leave.sx4, brfalse.sx3, brtrue.sx3, ldc.i4x3, ldc.i4.1x3, ldc.i4.sx3, ldloc.2x3, ldloc.3x3, stloc.0x3, dupx2, ldloc.0x2, newobjx2, ble.sx1, blt.sx1, br.sx1, brfalsex1, ceqx1, initobjx1, ldc.i4.m1x1, nopx1, popx1, retx1, stloc.1x1, stloc.2x1
- string literals: "Enter a server name", "", "A server with this name already exists", "", "Enter a valid port (1–65535)", "", "", "", "Install failed:

", "MPServerProvider"
- calls: System.Windows.Controls.TextBox.get_Text, System.String.Trim, System.String.get_Length, MPServerProvider.App.ToastService.Show, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.NameTaken, System.Int32.TryParse, MPServerProvider.App.AppServices.get_Installer, System.Progress`1..ctor, System.Threading.CancellationToken.get_None, MPServerProvider.Core.Services.InstallService.InstallAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.App.Views.ManualInstallDialog.set_InstalledServer, System.Nullable`1..ctor, System.Windows.Window.set_DialogResult, System.Exception.get_Message, System.String.Concat, System.Windows.MessageBox.Show, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x06000185`  flags=`481`  rva=`47816`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

