# <ResolveAsync>d__4

Token: `0x0200008d`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040002e0)
- `<>t__builder` : `valuetype:145<string>`  (flags=6, token=0x040002e1)
- `<>4__this` : `class:132`  (flags=6, token=0x040002e2)
- `majorVersion` : `i4`  (flags=6, token=0x040002e3)
- `progress` : `class:233<class:200>`  (flags=6, token=0x040002e4)
- `ct` : `valuetype:121`  (flags=6, token=0x040002e5)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x040002e6)

## Methods

### `MoveNext()` -> `void`
- token `0x0600034c`  flags=`481`  rva=`76640`
- IL size 325 bytes, 121 instructions, max_stack=4
- opcodes: ldarg.0x22, ldfldx16, ldloc.sx9, callx8, stloc.sx8, ldloc.1x6, callvirtx5, dupx5, ldloca.sx5, stfldx5, ldfldax4, stloc.2x4, brfalse.sx3, leavex3, leave.sx3, stloc.0x3, ldc.i4.sx2, brfalsex1, brtrue.sx1, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.0x1, ldloc.2x1, ldloc.3x1, retx1, stloc.1x1
- calls: System.Collections.Generic.Dictionary`2.TryGetValue, MPServerProvider.Core.Providers.JavaRuntimeProvider.FindProvisioned, System.Collections.Generic.Dictionary`2.set_Item, MPServerProvider.Core.Providers.JavaRuntimeProvider.SystemJavaMatches, MPServerProvider.Core.Providers.JavaRuntimeProvider.ProvisionTemurinAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600034d`  flags=`481`  rva=`77008`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

