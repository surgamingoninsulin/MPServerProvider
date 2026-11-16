# <LoadVersionsAsync>d__9

Token: `0x02000029`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000158)
- `<>t__builder` : `valuetype:437`  (flags=6, token=0x04000159)
- `<>4__this` : `class:52`  (flags=6, token=0x0400015a)
- `variant` : `string`  (flags=6, token=0x0400015b)
- `<>u__1` : `valuetype:361<class:501<string>>`  (flags=1, token=0x0400015c)

## Methods

### `MoveNext()` -> `void`
- token `0x06000172`  flags=`481`  rva=`44348`
- IL size 349 bytes, 124 instructions, max_stack=5
- opcodes: ldarg.0x14, ldfldx13, callvirtx12, ldloc.1x9, callx8, ldc.i4.0x7, stfldx5, dupx4, ldfldax4, ldloca.sx4, ldstrx4, leave.sx4, ldc.i4.1x3, ldloc.sx3, stloc.0x3, brfalse.sx2, initobjx2, ldc.i4.sx2, ldloc.0x2, ldloc.2x2, newarrx2, stelem.refx2, stloc.3x2, stloc.sx2, brtrue.sx1, cgtx1, ldc.i4.m1x1, ldloc.3x1, leavex1, nopx1, retx1, stloc.1x1, stloc.2x1
- string literals: "Loading…", "unavailable (offline?)", "Could not fetch versions: ", ""
- calls: System.Windows.Controls.ItemsControl.set_ItemsSource, System.Windows.Controls.Primitives.Selector.set_SelectedIndex, System.Windows.UIElement.set_IsEnabled, MPServerProvider.App.AppServices.get_Installer, MPServerProvider.Core.Services.InstallService.GetVersionsAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Collections.Generic.IReadOnlyCollection`1.get_Count, System.Exception.get_Message, System.String.Concat, MPServerProvider.App.ToastService.Show, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x06000173`  flags=`481`  rva=`44764`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

