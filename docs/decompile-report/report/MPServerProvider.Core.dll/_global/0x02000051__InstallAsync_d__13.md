# <InstallAsync>d__13

Token: `0x02000051`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000176)
- `<>t__builder` : `valuetype:145<class:220>`  (flags=6, token=0x04000177)
- `<>4__this` : `class:56`  (flags=6, token=0x04000178)
- `serverName` : `string`  (flags=6, token=0x04000179)
- `game` : `class:160`  (flags=6, token=0x0400017a)
- `variant` : `string`  (flags=6, token=0x0400017b)
- `version` : `string`  (flags=6, token=0x0400017c)
- `port` : `i4`  (flags=6, token=0x0400017d)
- `ramMinMb` : `i4`  (flags=6, token=0x0400017e)
- `ramMaxMb` : `i4`  (flags=6, token=0x0400017f)
- `description` : `string`  (flags=6, token=0x04000180)
- `eulaAccepted` : `bool`  (flags=6, token=0x04000181)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000182)
- `ct` : `valuetype:121`  (flags=6, token=0x04000183)
- `<targetDir>5__2` : `string`  (flags=1, token=0x04000184)
- `<request>5__3` : `class:208`  (flags=1, token=0x04000185)
- `<result>5__4` : `class:212`  (flags=1, token=0x04000186)
- `<>u__1` : `valuetype:137<class:212>`  (flags=1, token=0x04000187)
- `<>u__2` : `valuetype:137<string>`  (flags=1, token=0x04000188)

## Methods

### `MoveNext()` -> `void`
- token `0x0600029a`  flags=`481`  rva=`49268`
- IL size 752 bytes, 258 instructions, max_stack=5
- opcodes: ldarg.0x56, ldfldx35, callx22, callvirtx20, dupx15, stfldx15, ldloca.sx9, stloc.sx8, ldloc.sx7, ldfldax6, ldloc.1x5, popx5, stloc.0x5, brfalse.sx4, ldnullx4, ldstrx4, leave.sx4, brtrue.sx3, initobjx3, ldc.i4.1x3, ldloc.0x3, ldc.i4.m1x2, ldc.i4.sx2, leavex2, newobjx2, beqx1, ble.unx1, br.sx1, ldc.i4.0x1, ldc.r8x1, ldloc.2x1, ldloc.3x1, nopx1, retx1, rethrowx1, stloc.1x1, stloc.2x1, stloc.3x1, throwx1
- string literals: "Folder ", " already exists and is not empty.", "Finishing up…", "Stopping and removing partial files…"
- calls: MPServerProvider.Core.Services.ServerRepository.NewServerDir, System.IO.Directory.Exists, System.IO.Directory.EnumerateFileSystemEntries, System.Linq.Enumerable.Any, System.String.Concat, System.InvalidOperationException..ctor, System.IO.Directory.CreateDirectory, MPServerProvider.Core.Models.InstallRequest..ctor, MPServerProvider.Core.Models.InstallRequest.set_Game, MPServerProvider.Core.Models.InstallRequest.set_ServerName, MPServerProvider.Core.Models.InstallRequest.set_Variant, MPServerProvider.Core.Models.InstallRequest.set_Version, MPServerProvider.Core.Models.InstallRequest.set_TargetDir, MPServerProvider.Core.Models.InstallRequest.set_Port, MPServerProvider.Core.Models.InstallRequest.set_RamMinMb, MPServerProvider.Core.Models.InstallRequest.set_RamMaxMb, MPServerProvider.Core.Models.InstallRequest.set_Description, MPServerProvider.Core.Models.InstallRequest.set_EulaAccepted, MPServerProvider.Core.Services.InstallService.ProviderFor, MPServerProvider.Core.Providers.IInstallProvider.InstallAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.Core.Models.GameDefinition.get_Runtime, MPServerProvider.Core.Models.RuntimeSpec.get_Requires, MPServerProvider.Core.Models.RequiresSpec.get_Java, System.Nullable`1.get_HasValue, System.Nullable`1.GetValueOrDefault, MPServerProvider.Core.Services.InstallService.get_JavaRuntime, MPServerProvider.Core.Providers.JavaRuntimeProvider.ResolveAsync, MPServerProvider.Core.Services.InstallService.BuildInstance, MPServerProvider.Core.Models.InstallProgress.At, System.IProgress`1.Report, MPServerProvider.Core.Services.ServerRepository.Save, MPServerProvider.Core.Models.InstallProgress.Indeterminate, MPServerProvider.Core.Services.ServerRepository.ForceDeleteDirectory, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600029b`  flags=`481`  rva=`50108`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

