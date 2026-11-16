# <EnsureSteamCmdAsync>d__11

Token: `0x02000080`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400028e)
- `<>t__builder` : `valuetype:145<string>`  (flags=6, token=0x0400028f)
- `<>4__this` : `class:112`  (flags=6, token=0x04000290)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000291)
- `ct` : `valuetype:121`  (flags=6, token=0x04000292)
- `<dir>5__2` : `string`  (flags=1, token=0x04000293)
- `<exe>5__3` : `string`  (flags=1, token=0x04000294)
- `<zip>5__4` : `string`  (flags=1, token=0x04000295)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x04000296)

## Methods

### `MoveNext()` -> `void`
- token `0x06000330`  flags=`481`  rva=`70960`
- IL size 378 bytes, 130 instructions, max_stack=8
- opcodes: ldarg.0x34, ldfldx17, stfldx14, callx11, ldnullx8, ldstrx6, ldfldax4, callvirtx3, ldloca.sx3, stloc.0x3, dupx2, ldc.i4.sx2, ldloc.1x2, leavex2, leave.sx2, stloc.2x2, stloc.3x2, brfalsex1, brfalse.sx1, brtrue.sx1, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.0x1, ldloc.2x1, ldloc.3x1, ldloc.sx1, retx1, stloc.1x1, stloc.sx1
- string literals: "steamcmd", "steamcmd.exe", "steamcmd.zip", "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip", "Downloading SteamCMD (official Valve tool)", "Extracting SteamCMD…"
- calls: MPServerProvider.Core.Services.AppPaths.get_ToolsDir, System.IO.Path.Combine, System.IO.File.Exists, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Services.DownloadService.ExtractZip, System.IO.File.Delete, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000331`  flags=`481`  rva=`71380`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

