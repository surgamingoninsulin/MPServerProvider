# <GetChannelsAsync>d__9

Token: `0x02000085`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040002a8)
- `<>t__builder` : `valuetype:145<valuetype:305<string,string>>`  (flags=6, token=0x040002a9)
- `<>4__this` : `class:120`  (flags=6, token=0x040002aa)
- `ct` : `valuetype:121`  (flags=6, token=0x040002ab)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x040002ac)

## Methods

### `MoveNext()` -> `void`
- token `0x0600033a`  flags=`481`  rva=`72440`
- IL size 309 bytes, 114 instructions, max_stack=3
- opcodes: ldarg.0x14, callx10, ldloca.sx8, ldstrx7, stloc.sx7, callvirtx5, ldfldx5, stfldx5, dupx4, ldfldax4, ldloc.3x4, popx4, brtrue.sx3, ldloc.0x3, ldloc.sx3, leave.sx3, stloc.0x3, brfalse.sx2, initobjx2, ldc.i4.0x2, ldc.i4.sx2, newobjx2, stloc.2x2, bge.sx1, endfinallyx1, ldc.i4.m1x1, ldloc.1x1, ldloc.2x1, leavex1, nopx1, retx1, stloc.1x1, stloc.3x1
- string literals: "https://changelogs-live.fivem.net/api/changelog/versions/win32/server", "recommended", "", "latest", "", "", ""
- calls: MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.GetString, System.ValueTuple`2..ctor, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600033b`  flags=`481`  rva=`72804`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

