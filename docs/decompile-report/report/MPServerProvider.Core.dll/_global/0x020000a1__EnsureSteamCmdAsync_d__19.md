# <EnsureSteamCmdAsync>d__19

Token: `0x020000a1`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000364)
- `<>t__builder` : `valuetype:145<string>`  (flags=6, token=0x04000365)
- `<>4__this` : `class:152`  (flags=6, token=0x04000366)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000367)
- `ct` : `valuetype:121`  (flags=6, token=0x04000368)
- `<dir>5__2` : `string`  (flags=1, token=0x04000369)
- `<zip>5__3` : `string`  (flags=1, token=0x0400036a)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x0400036b)

## Methods

### `MoveNext()` -> `void`
- token `0x06000397`  flags=`481`  rva=`89992`
- IL size 386 bytes, 131 instructions, max_stack=8
- opcodes: ldarg.0x29, ldfldx15, callx13, stfldx11, ldstrx8, ldnullx6, ldfldax4, ldloc.sx4, stloc.sx4, callvirtx3, ldloca.sx3, leavex3, stloc.0x3, stloc.2x3, brfalse.sx2, dupx2, ldc.i4.sx2, ldloc.1x2, ldloc.3x2, leave.sx2, brfalsex1, brtrue.sx1, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.0x1, ldloc.2x1, retx1, stloc.1x1, stloc.3x1
- string literals: "steamcmd", "steamcmd.exe", "steamcmd.exe", "steamcmd.zip", "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip", "Downloading SteamCMD (official Valve tool)", "Extracting SteamCMD…", "steamcmd.exe"
- calls: MPServerProvider.Core.Services.AppPaths.get_ToolsDir, System.IO.Path.Combine, System.IO.File.Exists, MPServerProvider.Core.Providers.SteamCmdProvider.FindOnPath, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Services.DownloadService.ExtractZip, System.IO.File.Delete, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000398`  flags=`481`  rva=`90420`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

