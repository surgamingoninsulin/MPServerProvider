# <OnAddRule>d__12

Token: `0x0200004d`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001cf)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x040001d0)
- `<>4__this` : `class:108`  (flags=6, token=0x040001d1)
- `<>u__1` : `valuetype:181`  (flags=1, token=0x040001d2)

## Methods

### `MoveNext()` -> `void`
- token `0x060001d1`  flags=`481`  rva=`53840`
- IL size 277 bytes, 95 instructions, max_stack=4
- opcodes: ldarg.0x13, callx12, callvirtx8, stfldx5, ldfldax4, ldloc.2x4, ldloca.sx4, ldstrx4, leave.sx4, stloc.sx4, dupx3, ldfldx3, ldloc.sx3, stloc.0x3, brfalse.sx2, brtrue.sx2, ldc.i4.sx2, ldloc.0x2, ldloc.1x2, leavex2, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, newobjx1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "Firewall rule added", "", "Rule not added: ", ""
- calls: MPServerProvider.App.Views.AddFirewallRuleDialog..ctor, System.Windows.Window.GetWindow, System.Windows.Window.set_Owner, System.Windows.Window.ShowDialog, System.Nullable`1.GetValueOrDefault, MPServerProvider.App.AppServices.get_Firewall, MPServerProvider.App.Views.AddFirewallRuleDialog.get_RuleName, MPServerProvider.App.Views.AddFirewallRuleDialog.get_Port, MPServerProvider.App.Views.AddFirewallRuleDialog.get_Protocol, MPServerProvider.Core.Services.FirewallService.AddRuleAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.App.ToastService.Show, System.Exception.get_Message, System.String.Concat, MPServerProvider.App.Views.SettingsPage.RefreshRules, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x060001d2`  flags=`481`  rva=`54160`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

