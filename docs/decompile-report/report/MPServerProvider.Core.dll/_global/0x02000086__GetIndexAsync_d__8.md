# <GetIndexAsync>d__8

Token: `0x02000086`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040002ad)
- `<>t__builder` : `valuetype:145<class:297<string,string>>`  (flags=6, token=0x040002ae)
- `<>4__this` : `class:120`  (flags=6, token=0x040002af)
- `ct` : `valuetype:121`  (flags=6, token=0x040002b0)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x040002b1)

## Methods

### `MoveNext()` -> `void`
- token `0x0600033c`  flags=`481`  rva=`72820`
- IL size 175 bytes, 66 instructions, max_stack=3
- opcodes: ldarg.0x14, callx6, ldfldx5, stfldx5, ldfldax4, ldloca.sx3, leave.sx3, stloc.0x3, callvirtx2, dupx2, ldc.i4.sx2, stloc.3x2, brfalse.sx1, brtrue.sx1, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.0x1, ldloc.1x1, ldloc.2x1, ldloc.3x1, ldloc.sx1, ldstrx1, retx1, stloc.1x1, stloc.2x1, stloc.sx1
- string literals: "https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/"
- calls: MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.Core.Providers.CfxProvider.ParseArtifactIndex, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600033d`  flags=`481`  rva=`73024`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

