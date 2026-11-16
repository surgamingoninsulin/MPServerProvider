# <OnInstallClick>d__10

Token: `0x02000030`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000170)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x04000171)
- `sender` : `object`  (flags=6, token=0x04000172)
- `<>4__this` : `class:56`  (flags=6, token=0x04000173)
- `e` : `class:329`  (flags=6, token=0x04000174)
- `<>u__1` : `valuetype:181`  (flags=1, token=0x04000175)

## Methods

### `MoveNext()` -> `void`
- token `0x06000182`  flags=`481`  rva=`46800`
- IL size 437 bytes, 141 instructions, max_stack=4
- opcodes: callvirtx17, ldarg.0x16, callx14, ldloc.sx12, ldstrx7, stloc.sx7, ldfldx6, stfldx5, brfalsex4, ldfldax4, ldloc.2x4, ldloca.sx4, leave.sx4, brfalse.sx3, brtrue.sx3, dupx3, isinstx3, ldloc.1x3, leavex3, stloc.0x3, ldc.i4.sx2, ldloc.0x2, ldloc.3x2, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, newobjx1, nopx1, popx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: " installed", "", "Firewall rule added", "", "Firewall rule skipped (needs admin approval)", "", "servers"
- calls: System.Windows.FrameworkElement.get_Tag, MPServerProvider.App.Views.GameCardVm.get_Game, MPServerProvider.App.Views.LibraryPage.OnAddCustomGame, MPServerProvider.App.Views.InstallServerDialog..ctor, System.Windows.Window.GetWindow, System.Windows.Window.set_Owner, System.Windows.Window.ShowDialog, System.Nullable`1.GetValueOrDefault, MPServerProvider.App.Views.InstallServerDialog.get_InstalledServer, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.String.Concat, MPServerProvider.App.ToastService.Show, MPServerProvider.Core.Models.ServerInstance.get_CreateFirewallRule, MPServerProvider.App.AppServices.get_Firewall, MPServerProvider.Core.Models.ServerInstance.get_ServerPort, MPServerProvider.Core.Models.GameDefinition.get_Network, MPServerProvider.Core.Models.NetworkSpec.get_Protocol, MPServerProvider.Core.Services.FirewallService.AddRuleAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.App.Views.MainWindow.RefreshHome, MPServerProvider.App.Views.MainWindow.NavigateTo, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x06000183`  flags=`481`  rva=`47304`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

