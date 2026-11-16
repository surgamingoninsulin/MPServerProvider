# <InstallAsync>d__11

Token: `0x02000088`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040002b9)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x040002ba)
- `progress` : `class:233<class:200>`  (flags=6, token=0x040002bb)
- `request` : `class:208`  (flags=6, token=0x040002bc)
- `<>4__this` : `class:120`  (flags=6, token=0x040002bd)
- `ct` : `valuetype:121`  (flags=6, token=0x040002be)
- `<buildNumber>5__2` : `string`  (flags=1, token=0x040002bf)
- `<index>5__3` : `class:297<string,string>`  (flags=1, token=0x040002c0)
- `<artifactDir>5__4` : `string`  (flags=1, token=0x040002c1)
- `<zip>5__5` : `string`  (flags=1, token=0x040002c2)
- `<>u__1` : `valuetype:137<class:297<string,string>>`  (flags=1, token=0x040002c3)
- `<>u__2` : `valuetype:137<valuetype:305<string,string>>`  (flags=1, token=0x040002c4)
- `<>u__3` : `valuetype:133`  (flags=1, token=0x040002c5)

## Methods

### `MoveNext()` -> `void`
- token `0x06000340`  flags=`481`  rva=`73664`
- IL size 1038 bytes, 340 instructions, max_stack=8
- opcodes: ldarg.0x64, callx37, ldfldx30, stfldx25, ldloca.sx23, callvirtx17, ldstrx16, dupx14, stloc.sx13, ldloc.sx12, ldnullx11, ldfldax8, brtrue.sx7, stloc.0x7, newobjx6, ldc.i4.sx5, popx4, initobjx3, ldc.i4.0x3, ldc.i4.m1x3, ldloc.1x3, ldsfldx3, leavex3, ldc.i4.2x2, ldftnx2, leave.sx2, stsfldx2, ble.sx1, br.sx1, brfalse.sx1, brtruex1, ldc.i4.1x1, ldc.i4.3x1, ldloc.0x1, ldloc.2x1, ldloc.3x1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, switchx1, throwx1
- string literals: "Resolving Cfx.re server artifact…", "", "Build ", " is no longer available — using recommended build ", " instead.", "The Cfx.re artifact index is empty or unreachable.", "https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/", "-", "/server.zip", "artifact", "server.zip", "Downloading FXServer build ", "Extracting FXServer…", "artifact", "FXServer.exe", "build "
- calls: MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Models.InstallRequest.get_Version, System.String.Split, <>c.<InstallAsync>b__11_0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault, MPServerProvider.Core.Providers.CfxProvider.GetIndexAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Collections.Generic.Dictionary`2.TryGetValue, MPServerProvider.Core.Providers.CfxProvider.GetChannelsAsync, System.String.get_Length, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Collections.Generic.Dictionary`2.get_Keys, System.Int32.Parse, System.Linq.Enumerable.OrderByDescending, System.InvalidOperationException..ctor, System.Collections.Generic.Dictionary`2.get_Item, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, System.String.Concat, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Services.DownloadService.ExtractZip, System.IO.File.Delete, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000341`  flags=`481`  rva=`74744`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

