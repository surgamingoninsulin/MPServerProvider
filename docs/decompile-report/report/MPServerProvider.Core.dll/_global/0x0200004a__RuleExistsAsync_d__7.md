# <RuleExistsAsync>d__7

Token: `0x0200004a`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000162)
- `<>t__builder` : `valuetype:145<bool>`  (flags=6, token=0x04000163)
- `rule` : `class:192`  (flags=6, token=0x04000164)
- `<process>5__2` : `class:169`  (flags=1, token=0x04000165)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x04000166)

## Methods

### `MoveNext()` -> `void`
- token `0x06000289`  flags=`481`  rva=`48336`
- IL size 291 bytes, 108 instructions, max_stack=4
- opcodes: ldarg.0x20, callvirtx8, stfldx8, callx7, ldfldx7, dupx5, ldc.i4.0x4, ldfldax4, ldloca.sx4, brfalse.sx3, ldloc.0x3, ldstrx3, stloc.0x3, initobjx2, ldc.i4.1x2, ldc.i4.sx2, ldloc.sx2, ldnullx2, leave.sx2, stloc.3x2, bge.sx1, brtrue.sx1, ceqx1, endfinallyx1, ldc.i4.m1x1, ldloc.1x1, ldloc.2x1, ldloc.3x1, leavex1, newobjx1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.sx1
- string literals: "netsh", "advfirewall firewall show rule name="MPSM - ", """
- calls: MPServerProvider.Core.Models.FirewallRuleInfo.get_Name, System.String.Concat, System.Diagnostics.ProcessStartInfo..ctor, System.Diagnostics.ProcessStartInfo.set_UseShellExecute, System.Diagnostics.ProcessStartInfo.set_RedirectStandardOutput, System.Diagnostics.ProcessStartInfo.set_CreateNoWindow, System.Diagnostics.Process.Start, System.Diagnostics.Process.WaitForExitAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Diagnostics.Process.get_ExitCode, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600028a`  flags=`481`  rva=`48668`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

