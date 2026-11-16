# <RefreshRules>d__11

Token: `0x0200004f`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001d8)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x040001d9)
- `<>4__this` : `class:108`  (flags=6, token=0x040001da)
- `<>7__wrap1` : `valuetype:585<class:649>`  (flags=1, token=0x040001db)
- `<rule>5__3` : `class:649`  (flags=1, token=0x040001dc)
- `<>u__1` : `valuetype:361<bool>`  (flags=1, token=0x040001dd)

## Methods

### `MoveNext()` -> `void`
- token `0x060001d5`  flags=`481`  rva=`54496`
- IL size 455 bytes, 147 instructions, max_stack=4
- opcodes: ldarg.0x22, callx17, callvirtx14, ldfldx8, ldfldax8, stfldx8, dupx6, brtrue.sx4, ldsfldx4, brfalse.sx3, ldc.i4.0x3, ldloc.0x3, ldloc.1x3, ldloc.2x3, ldloca.sx3, leave.sx3, popx3, stloc.0x3, initobjx2, ldc.i4.sx2, ldftnx2, newobjx2, stloc.3x2, stsfldx2, bge.sx1, brx1, br.sx1, brtruex1, constrained.x1, endfinallyx1, ldc.i4.2x1, ldc.i4.m1x1, ldloc.3x1, ldloc.sx1, ldnullx1, leavex1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.sx1
- calls: MPServerProvider.App.AppServices.get_Firewall, MPServerProvider.Core.Services.FirewallService.get_Rules, System.Linq.Enumerable.ToList, <>c.<RefreshRules>b__11_0, System.Func`2..ctor, System.Linq.Enumerable.Select, System.Windows.Controls.ItemsControl.set_ItemsSource, System.Collections.Generic.List`1.get_Count, System.Windows.UIElement.set_Visibility, System.Collections.Generic.List`1.GetEnumerator, Enumerator.get_Current, MPServerProvider.Core.Services.FirewallService.RuleExistsAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.App.AppServices.get_Settings, MPServerProvider.Core.Services.SettingsService.get_Settings, MPServerProvider.Core.Models.AppSettings.get_FirewallRules, System.Collections.Generic.List`1.Remove, MPServerProvider.Core.Services.SettingsService.Save, <>c.<RefreshRules>b__11_1, Enumerator.MoveNext, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x060001d6`  flags=`481`  rva=`55016`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

