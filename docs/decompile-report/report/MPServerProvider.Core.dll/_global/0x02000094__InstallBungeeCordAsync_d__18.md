# <InstallBungeeCordAsync>d__18

Token: `0x02000094`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000306)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x04000307)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000308)
- `<>4__this` : `class:140`  (flags=6, token=0x04000309)
- `request` : `class:208`  (flags=6, token=0x0400030a)
- `ct` : `valuetype:121`  (flags=6, token=0x0400030b)
- `<jar>5__2` : `string`  (flags=1, token=0x0400030c)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x0400030d)

## Methods

### `MoveNext()` -> `void`
- token `0x06000364`  flags=`481`  rva=`81308`
- IL size 293 bytes, 101 instructions, max_stack=8
- opcodes: ldarg.0x22, ldfldx10, stfldx8, callx7, callvirtx7, dupx5, ldstrx5, ldfldax4, ldnullx4, ldc.i4.sx3, ldloca.sx3, stloc.0x3, leave.sx2, stloc.3x2, brfalsex1, brtrue.sx1, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.0x1, ldloc.1x1, ldloc.2x1, ldloc.3x1, ldloc.sx1, leavex1, newobjx1, retx1, stloc.1x1, stloc.2x1, stloc.sx1
- string literals: "Downloading BungeeCord…", "BungeeCord.jar", "https://ci.md-5.net/job/BungeeCord/lastStableBuild/artifact/bootstrap/target/BungeeCord.jar", "Downloading BungeeCord latest", "latest"
- calls: MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version, MPServerProvider.Core.Models.InstallResult.set_JavaMajor, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000365`  flags=`481`  rva=`81632`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

