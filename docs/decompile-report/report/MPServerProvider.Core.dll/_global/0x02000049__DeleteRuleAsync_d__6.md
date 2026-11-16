# <DeleteRuleAsync>d__6

Token: `0x02000049`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400015d)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x0400015e)
- `rule` : `class:192`  (flags=6, token=0x0400015f)
- `<>4__this` : `class:44`  (flags=6, token=0x04000160)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x04000161)

## Methods

### `MoveNext()` -> `void`
- token `0x06000287`  flags=`481`  rva=`48076`
- IL size 214 bytes, 75 instructions, max_stack=3
- opcodes: ldarg.0x15, callx7, ldfldx7, callvirtx6, stfldx5, ldfldax4, ldloca.sx3, leave.sx3, stloc.0x3, dupx2, ldc.i4.sx2, ldloc.1x2, ldstrx2, stloc.2x2, brfalse.sx1, brtrue.sx1, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.0x1, ldloc.2x1, ldloc.3x1, popx1, retx1, stloc.1x1, stloc.3x1
- string literals: "netsh advfirewall firewall delete rule name="MPSM - ", """
- calls: MPServerProvider.Core.Models.FirewallRuleInfo.get_Name, System.String.Concat, MPServerProvider.Core.Services.FirewallService.RunElevatedAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Services.SettingsService.get_Settings, MPServerProvider.Core.Models.AppSettings.get_FirewallRules, System.Collections.Generic.List`1.Remove, MPServerProvider.Core.Services.SettingsService.Save, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000288`  flags=`481`  rva=`48320`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

