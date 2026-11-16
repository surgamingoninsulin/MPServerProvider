# <StartPlayitAsync>d__21

Token: `0x0200007a`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000268)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x04000269)
- `onOutput` : `class:193<string>`  (flags=6, token=0x0400026a)
- `<>4__this` : `class:108`  (flags=6, token=0x0400026b)
- `ct` : `valuetype:121`  (flags=6, token=0x0400026c)
- `<>8__1` : `class:472`  (flags=1, token=0x0400026d)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x0400026e)

## Methods

### `MoveNext()` -> `void`
- token `0x06000321`  flags=`481`  rva=`68400`
- IL size 371 bytes, 129 instructions, max_stack=3
- opcodes: ldarg.0x21, callx14, callvirtx11, stfldx9, ldfldx8, dupx7, ldloc.1x7, ldfldax4, ldloc.sx4, newobjx4, stloc.sx4, ldc.i4.1x3, ldloca.sx3, ldnullx3, stloc.0x3, brfalse.sx2, brtrue.sx2, ldc.i4.0x2, ldc.i4.sx2, ldftnx2, ldloc.2x2, leavex2, leave.sx2, initobjx1, ldc.i4.m1x1, ldloc.0x1, ldloc.3x1, retx1, stloc.1x1, stloc.2x1, stloc.3x1
- calls: <>c__DisplayClass21_0..ctor, MPServerProvider.Core.Services.TunnelService.get_PlayitProcess, System.Diagnostics.Process.get_HasExited, MPServerProvider.Core.Services.TunnelService.EnsurePlayitAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Diagnostics.ProcessStartInfo..ctor, System.Diagnostics.ProcessStartInfo.set_UseShellExecute, System.Diagnostics.ProcessStartInfo.set_CreateNoWindow, System.Diagnostics.ProcessStartInfo.set_RedirectStandardOutput, System.Diagnostics.ProcessStartInfo.set_RedirectStandardError, System.IO.Path.GetDirectoryName, System.Diagnostics.ProcessStartInfo.set_WorkingDirectory, System.Diagnostics.Process.Start, MPServerProvider.Core.Services.TunnelService.set_PlayitProcess, <>c__DisplayClass21_0.<StartPlayitAsync>b__0, System.Diagnostics.DataReceivedEventHandler..ctor, System.Diagnostics.Process.add_OutputDataReceived, <>c__DisplayClass21_0.<StartPlayitAsync>b__1, System.Diagnostics.Process.add_ErrorDataReceived, System.Diagnostics.Process.BeginOutputReadLine, System.Diagnostics.Process.BeginErrorReadLine, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000322`  flags=`481`  rva=`68812`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

