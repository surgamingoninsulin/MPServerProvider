# <OnStartAgent>d__7

Token: `0x02000059`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001f8)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x040001f9)
- `<>4__this` : `class:116`  (flags=6, token=0x040001fa)
- `<>u__1` : `valuetype:181`  (flags=1, token=0x040001fb)

## Methods

### `MoveNext()` -> `void`
- token `0x060001ea`  flags=`481`  rva=`57040`
- IL size 275 bytes, 95 instructions, max_stack=4
- opcodes: ldarg.0x13, callvirtx8, ldfldx8, callx7, ldloc.1x6, stfldx5, ldfldax4, ldloca.sx4, leave.sx4, ldstrx3, stloc.0x3, brfalse.sx2, dupx2, initobjx2, ldc.i4.0x2, ldc.i4.1x2, ldc.i4.sx2, ldloc.0x2, ldloc.sx2, stloc.2x2, stloc.sx2, brtrue.sx1, ldc.i4.m1x1, ldftnx1, ldloc.2x1, ldloc.3x1, leavex1, newobjx1, nopx1, retx1, stloc.1x1
- string literals: "Downloading/starting the official playit agent…
", "Failed to start agent: ", "
"
- calls: System.Windows.UIElement.set_IsEnabled, System.Windows.Controls.TextBox.set_Text, MPServerProvider.App.AppServices.get_Tunnel, MPServerProvider.App.Views.TunnelDialog.<OnStartAgent>b__7_0, System.Action`1..ctor, MPServerProvider.Core.Services.TunnelService.StartPlayitAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Exception.get_Message, System.String.Concat, System.Windows.Controls.Primitives.TextBoxBase.AppendText, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x060001eb`  flags=`481`  rva=`57356`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

