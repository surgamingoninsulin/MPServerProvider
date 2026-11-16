# <RunStartupAsync>d__1

Token: `0x02000055`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001eb)
- `<>t__builder` : `valuetype:437`  (flags=6, token=0x040001ec)
- `<>4__this` : `class:112`  (flags=6, token=0x040001ed)
- `<>u__1` : `valuetype:181`  (flags=1, token=0x040001ee)

## Methods

### `MoveNext()` -> `void`
- token `0x060001e1`  flags=`481`  rva=`55752`
- IL size 556 bytes, 185 instructions, max_stack=3
- opcodes: ldarg.0x34, callx22, stfldx14, ldloca.sx12, dupx10, ldfldax10, stloc.0x9, stloc.2x8, ldfldx6, brtrue.sx5, ldloc.1x5, callvirtx4, initobjx4, ldc.i4.m1x4, ldc.r8x4, ldloc.2x4, ldstrx4, leavex3, leave.sx3, ldc.i4x2, ldc.i4.sx2, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.3x1, ldftnx1, ldloc.0x1, ldloc.3x1, ldnullx1, ldsfldx1, newobjx1, popx1, retx1, stloc.1x1, stloc.3x1, stsfldx1, switchx1
- string literals: "Initializing services…", "Loading game library…", "Fetching official artwork…", "You are up to date. Finalizing..."
- calls: MPServerProvider.App.Views.SplashWindow.Report, MPServerProvider.App.AppServices.Initialize, System.Action..ctor, System.Threading.Tasks.Task.Run, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Threading.Tasks.Task.Delay, MPServerProvider.App.Views.SplashWindow.PrefetchArtworkAsync, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x060001e2`  flags=`481`  rva=`56348`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

