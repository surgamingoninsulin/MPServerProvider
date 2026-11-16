# <AddRuleAsync>d__5

Token: `0x02000048`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000155)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x04000156)
- `port` : `i4`  (flags=6, token=0x04000157)
- `name` : `string`  (flags=6, token=0x04000158)
- `protocol` : `string`  (flags=6, token=0x04000159)
- `<>4__this` : `class:44`  (flags=6, token=0x0400015a)
- `<>8__1` : `class:284`  (flags=1, token=0x0400015b)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x0400015c)

## Methods

### `MoveNext()` -> `void`
- token `0x06000285`  flags=`481`  rva=`47580`
- IL size 438 bytes, 137 instructions, max_stack=4
- opcodes: ldarg.0x28, ldfldx21, callvirtx14, callx11, stfldx11, dupx7, ldfldax4, newobjx4, ldloc.1x3, ldloca.sx3, stloc.0x3, ldc.i4.sx2, ldftnx2, ldnullx2, ldstrx2, leave.sx2, stloc.3x2, brfalsex1, brtrue.sx1, initobjx1, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.m1x1, ldloc.0x1, ldloc.2x1, ldloc.3x1, ldloc.sx1, leavex1, popx1, retx1, stloc.1x1, stloc.2x1, stloc.sx1
- string literals: "MPSM - ", " & "
- calls: <>c__DisplayClass5_0..ctor, System.String.Concat, MPServerProvider.Core.Services.FirewallService.ProtocolsFor, <>c__DisplayClass5_0.<AddRuleAsync>b__0, System.Func`2..ctor, System.Linq.Enumerable.Select, System.String.Join, MPServerProvider.Core.Services.FirewallService.RunElevatedAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Services.SettingsService.get_Settings, MPServerProvider.Core.Models.AppSettings.get_FirewallRules, <>c__DisplayClass5_0.<AddRuleAsync>b__1, System.Predicate`1..ctor, System.Collections.Generic.List`1.RemoveAll, MPServerProvider.Core.Models.FirewallRuleInfo..ctor, MPServerProvider.Core.Models.FirewallRuleInfo.set_Name, MPServerProvider.Core.Models.FirewallRuleInfo.set_Port, System.String.ToLowerInvariant, MPServerProvider.Core.Models.FirewallRuleInfo.set_Protocol, MPServerProvider.Core.Models.FirewallRuleInfo.set_Active, System.DateTimeOffset.get_Now, MPServerProvider.Core.Models.FirewallRuleInfo.set_Created, System.Collections.Generic.List`1.Add, MPServerProvider.Core.Services.SettingsService.Save, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000286`  flags=`481`  rva=`48060`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

