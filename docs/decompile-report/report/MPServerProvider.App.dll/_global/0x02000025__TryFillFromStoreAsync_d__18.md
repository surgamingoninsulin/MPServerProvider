# <TryFillFromStoreAsync>d__18

Token: `0x02000025`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000142)
- `<>t__builder` : `valuetype:385<bool>`  (flags=6, token=0x04000143)
- `appId` : `i4`  (flags=6, token=0x04000144)
- `<>4__this` : `class:32`  (flags=6, token=0x04000145)
- `<>u__1` : `valuetype:361<string>`  (flags=1, token=0x04000146)

## Methods

### `MoveNext()` -> `void`
- token `0x06000167`  flags=`481`  rva=`41944`
- IL size 554 bytes, 201 instructions, max_stack=4
- opcodes: ldloca.sx31, callx26, ldarg.0x15, brfalse.sx10, callvirtx10, stloc.sx10, ldloc.sx9, ldc.i4.0x7, ldfldx7, ldstrx7, stfldx6, ldfldax5, brtrue.sx4, ldloc.0x4, ldloc.1x4, leave.sx4, ldc.i4.sx3, ldloc.3x3, stloc.0x3, stloc.2x3, bge.sx2, ble.sx2, br.sx2, dupx2, endfinallyx2, initobjx2, ldc.i4.1x2, leavex2, popx2, bne.un.sx1, constrained.x1, ldc.i4.2x1, ldc.i4.m1x1, ldloc.2x1, ldnullx1, ldsfldx1, newobjx1, nopx1, retx1, stloc.1x1, stloc.3x1
- string literals: "https://store.steampowered.com/api/appdetails?appids=", "success", "data", "header_image", "short_description", "genres", "description"
- calls: System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Net.Http.HttpClient.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Int32.ToString, System.Text.Json.JsonElement.TryGetProperty, System.Text.Json.JsonElement.GetBoolean, System.Text.Json.JsonElement.GetString, System.String.get_Length, System.Windows.Controls.TextBox.set_Text, System.Collections.Generic.List`1.Clear, System.Text.Json.JsonElement.get_ValueKind, System.Text.Json.JsonElement.EnumerateArray, ArrayEnumerator.GetEnumerator, ArrayEnumerator.get_Current, System.Collections.Generic.List`1.Add, ArrayEnumerator.MoveNext, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x06000168`  flags=`481`  rva=`42612`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

