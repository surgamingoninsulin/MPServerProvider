# <OnAppStartup>d__1

Token: `0x0200001f`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400012c)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x0400012d)
- `<>4__this` : `class:16`  (flags=6, token=0x0400012e)
- `<splash>5__2` : `class:112`  (flags=1, token=0x0400012f)
- `<>u__1` : `valuetype:181`  (flags=1, token=0x04000130)

## Methods

### `MoveNext()` -> `void`
- token `0x0600015a`  flags=`481`  rva=`39060`
- IL size 302 bytes, 111 instructions, max_stack=4
- opcodes: ldarg.0x19, callx12, stfldx8, ldfldx6, ldloca.sx6, callvirtx5, ldc.i4.sx4, ldfldax4, leave.sx4, ldc.i4.1x3, ldloc.1x3, newobjx3, stloc.0x3, stloc.sx3, brfalse.sx2, dupx2, ldc.i4.0x2, ldloc.0x2, ldloc.2x2, ldloc.sx2, ldnullx2, ldstrx2, stloc.3x2, brtrue.sx1, initobjx1, ldc.i4.m1x1, ldloc.3x1, leavex1, nopx1, popx1, retx1, stloc.1x1, stloc.2x1
- string literals: "MPServerProvider failed to start:

", "MPServerProvider"
- calls: MPServerProvider.App.Views.SplashWindow..ctor, System.Windows.Window.Show, MPServerProvider.App.Views.SplashWindow.RunStartupAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Windows.MessageBox.Show, System.Windows.Application.Shutdown, MPServerProvider.App.Views.MainWindow..ctor, System.Windows.Application.set_MainWindow, System.Windows.Application.set_ShutdownMode, System.Windows.Window.Close, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x0600015b`  flags=`481`  rva=`39404`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

