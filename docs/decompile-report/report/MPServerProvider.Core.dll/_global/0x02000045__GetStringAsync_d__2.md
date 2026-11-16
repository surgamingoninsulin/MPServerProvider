# <GetStringAsync>d__2

Token: `0x02000045`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000141)
- `<>t__builder` : `valuetype:145<string>`  (flags=6, token=0x04000142)
- `url` : `string`  (flags=6, token=0x04000143)
- `ct` : `valuetype:121`  (flags=6, token=0x04000144)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000145)

## Methods

### `MoveNext()` -> `void`
- token `0x0600027e`  flags=`481`  rva=`46664`
- IL size 161 bytes, 62 instructions, max_stack=3
- opcodes: ldarg.0x14, callx5, stfldx5, ldfldx4, ldfldax4, ldloca.sx3, leave.sx3, stloc.0x3, callvirtx2, dupx2, ldc.i4.sx2, stloc.2x2, brfalse.sx1, brtrue.sx1, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.0x1, ldloc.1x1, ldloc.2x1, ldloc.3x1, ldsfldx1, retx1, stloc.1x1, stloc.3x1
- calls: System.Net.Http.HttpClient.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600027f`  flags=`481`  rva=`46856`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

