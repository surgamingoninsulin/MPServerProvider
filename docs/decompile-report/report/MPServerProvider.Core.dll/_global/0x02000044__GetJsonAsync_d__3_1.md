# <GetJsonAsync>d__3`1

Token: `0x02000044`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400013c)
- `<>t__builder` : `valuetype:145<var:0>`  (flags=6, token=0x0400013d)
- `url` : `string`  (flags=6, token=0x0400013e)
- `ct` : `valuetype:121`  (flags=6, token=0x0400013f)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000140)

## Methods

### `MoveNext()` -> `void`
- token `0x0600027c`  flags=`481`  rva=`46452`
- IL size 166 bytes, 63 instructions, max_stack=3
- opcodes: ldarg.0x14, callx6, stfldx5, ldfldx4, ldfldax4, ldloca.sx3, leave.sx3, stloc.0x3, callvirtx2, dupx2, ldc.i4.sx2, stloc.2x2, brfalse.sx1, brtrue.sx1, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.0x1, ldloc.1x1, ldloc.2x1, ldloc.3x1, ldsfldx1, retx1, stloc.1x1, stloc.3x1
- calls: System.Net.Http.HttpClient.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.Core.Json.Json.Deserialize, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600027d`  flags=`481`  rva=`46648`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

