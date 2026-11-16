# <QueryFiveMAsync>d__8

Token: `0x0200005b`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001aa)
- `<>t__builder` : `valuetype:145<valuetype:261<valuetype:305<i4,i4>>>`  (flags=6, token=0x040001ab)
- `host` : `string`  (flags=6, token=0x040001ac)
- `port` : `i4`  (flags=6, token=0x040001ad)
- `<cts>5__2` : `class:329`  (flags=1, token=0x040001ae)
- `<playersDoc>5__3` : `class:341`  (flags=1, token=0x040001af)
- `<online>5__4` : `i4`  (flags=1, token=0x040001b0)
- `<max>5__5` : `i4`  (flags=1, token=0x040001b1)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x040001b2)

## Methods

### `MoveNext()` -> `void`
- token `0x060002b2`  flags=`481`  rva=`52844`
- IL size 733 bytes, 258 instructions, max_stack=3
- opcodes: ldarg.0x41, callx27, ldloca.sx27, stfldx17, ldfldx16, callvirtx11, ldloc.0x8, ldstrx8, ldloc.sx7, brfalse.sx6, ldfldax6, stloc.sx6, ldc.i4.1x5, ldc.i4.sx5, leave.sx5, newobjx5, stloc.0x5, dupx4, initobjx4, ldc.i4.0x4, ldnullx4, stloc.3x4, bge.sx3, endfinallyx3, ldsfldx3, nopx3, beqx2, brtrue.sx2, ldc.i4.2x2, ldc.i4.m1x2, ldloc.3x2, leavex2, beq.sx1, ble.un.sx1, brfalsex1, ldloc.1x1, ldloc.2x1, popx1, retx1, stloc.1x1, stloc.2x1
- string literals: "http://", ":", "/players.json", "http://", ":", "/info.json", "vars", "sv_maxClients"
- calls: System.Threading.CancellationTokenSource..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Threading.CancellationTokenSource.get_Token, System.Net.Http.HttpClient.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetArrayLength, System.Text.Json.JsonElement.TryGetProperty, System.Text.Json.JsonElement.GetString, System.Int32.TryParse, System.IDisposable.Dispose, System.ValueTuple`2..ctor, System.Nullable`1..ctor, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002b3`  flags=`481`  rva=`53716`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

