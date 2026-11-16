# <OnDeleteRule>d__13

Token: `0x0200004e`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001d3)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x040001d4)
- `sender` : `object`  (flags=6, token=0x040001d5)
- `<>4__this` : `class:108`  (flags=6, token=0x040001d6)
- `<>u__1` : `valuetype:181`  (flags=1, token=0x040001d7)

## Methods

### `MoveNext()` -> `void`
- token `0x060001d3`  flags=`481`  rva=`54176`
- IL size 261 bytes, 91 instructions, max_stack=3
- opcodes: ldarg.0x14, callx10, callvirtx5, stfldx5, ldfldx4, ldfldax4, ldstrx4, leave.sx4, stloc.sx4, brfalse.sx3, ldloc.sx3, ldloca.sx3, stloc.0x3, brtrue.sx2, dupx2, isinstx2, ldc.i4.sx2, ldloc.0x2, ldloc.2x2, ldloc.3x2, leavex2, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.1x1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "Firewall rule deleted", "", "Delete failed: ", ""
- calls: System.Windows.FrameworkElement.get_Tag, MPServerProvider.App.AppServices.get_Firewall, MPServerProvider.App.Views.RuleVm.get_Rule, MPServerProvider.Core.Services.FirewallService.DeleteRuleAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.App.ToastService.Show, System.Exception.get_Message, System.String.Concat, MPServerProvider.App.Views.SettingsPage.RefreshRules, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x060001d4`  flags=`481`  rva=`54480`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

