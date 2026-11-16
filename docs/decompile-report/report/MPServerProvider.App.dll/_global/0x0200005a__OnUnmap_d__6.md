# <OnUnmap>d__6

Token: `0x0200005a`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001fc)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x040001fd)
- `<>4__this` : `class:116`  (flags=6, token=0x040001fe)
- `<server>5__2` : `class:497`  (flags=1, token=0x040001ff)
- `<>u__1` : `valuetype:181`  (flags=1, token=0x04000200)

## Methods

### `MoveNext()` -> `void`
- token `0x060001ec`  flags=`481`  rva=`57372`
- IL size 315 bytes, 105 instructions, max_stack=3
- opcodes: ldarg.0x19, callx13, ldfldx8, stfldx8, ldloca.sx7, callvirtx5, ldstrx5, ldfldax4, ldloc.1x4, ldc.i4.sx3, stloc.0x3, brtrue.sx2, dupx2, ldnullx2, leavex2, leave.sx2, stloc.2x2, brfalse.sx1, castclassx1, initobjx1, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.m1x1, ldloc.0x1, ldloc.2x1, ldloc.sx1, newobjx1, retx1, stloc.1x1, stloc.3x1, stloc.sx1
- string literals: "Mapping for port ", " removed.", "TextSecondary", "UPnP mapping removed", ""
- calls: MPServerProvider.App.Views.TunnelDialog.get_SelectedServer, MPServerProvider.App.AppServices.get_Tunnel, MPServerProvider.Core.Services.TunnelService.UnmapPortAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, MPServerProvider.Core.Models.ServerInstance.get_ServerPort, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Windows.Controls.TextBlock.set_Text, System.Windows.FrameworkElement.FindResource, System.Windows.Controls.TextBlock.set_Foreground, MPServerProvider.App.ToastService.Show, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x060001ed`  flags=`481`  rva=`57716`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

