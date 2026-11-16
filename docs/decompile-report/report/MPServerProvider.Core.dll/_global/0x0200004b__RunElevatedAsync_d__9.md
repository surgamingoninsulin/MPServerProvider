# <RunElevatedAsync>d__9

Token: `0x0200004b`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000167)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x04000168)
- `commandLine` : `string`  (flags=6, token=0x04000169)
- `<process>5__2` : `class:169`  (flags=1, token=0x0400016a)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x0400016b)

## Methods

### `MoveNext()` -> `void`
- token `0x0600028b`  flags=`481`  rva=`48684`
- IL size 373 bytes, 132 instructions, max_stack=4
- opcodes: ldarg.0x21, callx11, callvirtx10, dupx8, ldfldx8, ldloca.sx8, stfldx8, ldstrx6, brfalse.sx4, ldfldax4, newobjx4, ldc.i4.1x3, ldc.i4.sx3, ldloc.0x3, stloc.0x3, brtrue.sx2, initobjx2, ldc.i4.0x2, ldnullx2, leave.sx2, stloc.2x2, stloc.sx2, throwx2, bge.sx1, endfinallyx1, ldc.i4.m1x1, ldloc.1x1, ldloc.2x1, ldloc.3x1, ldloc.sx1, leavex1, nopx1, popx1, retx1, stloc.1x1
- string literals: "cmd.exe", "/c ", "runas", "Could not start elevated command.", "Firewall command failed (exit code ", ")."
- calls: System.Diagnostics.ProcessStartInfo..ctor, System.Diagnostics.ProcessStartInfo.set_FileName, System.String.Concat, System.Diagnostics.ProcessStartInfo.set_Arguments, System.Diagnostics.ProcessStartInfo.set_UseShellExecute, System.Diagnostics.ProcessStartInfo.set_Verb, System.Diagnostics.ProcessStartInfo.set_WindowStyle, System.Diagnostics.Process.Start, System.InvalidOperationException..ctor, System.Diagnostics.Process.WaitForExitAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Diagnostics.Process.get_ExitCode, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600028c`  flags=`481`  rva=`49124`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

