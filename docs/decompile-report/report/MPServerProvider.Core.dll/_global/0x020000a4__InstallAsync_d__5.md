# <InstallAsync>d__5

Token: `0x020000a4`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000384)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x04000385)
- `request` : `class:208`  (flags=6, token=0x04000386)
- `<>4__this` : `class:156`  (flags=6, token=0x04000387)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000388)
- `ct` : `valuetype:121`  (flags=6, token=0x04000389)
- `<fileName>5__2` : `string`  (flags=1, token=0x0400038a)
- `<downloaded>5__3` : `string`  (flags=1, token=0x0400038b)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x0400038c)
- `<stream>5__4` : `class:265`  (flags=1, token=0x0400038d)
- `<gzip>5__5` : `class:481`  (flags=1, token=0x0400038e)
- `<>7__wrap5` : `object`  (flags=1, token=0x0400038f)
- `<>7__wrap6` : `i4`  (flags=1, token=0x04000390)
- `<>7__wrap7` : `object`  (flags=1, token=0x04000391)
- `<>7__wrap8` : `i4`  (flags=1, token=0x04000392)
- `<>u__2` : `valuetype:245`  (flags=1, token=0x04000393)

## Methods

### `MoveNext()` -> `void`
- token `0x0600039d`  flags=`481`  rva=`93220`
- IL size 1270 bytes, 423 instructions, max_stack=8
- opcodes: ldarg.0x85, ldfldx43, callx38, callvirtx35, stfldx29, ldloc.sx19, stloc.sx18, dupx15, ldloca.sx14, ldstrx13, brtrue.sx11, ldfldax10, ldnullx9, stloc.0x9, brfalse.sx7, leave.sx6, initobjx4, ldc.i4.0x4, ldc.i4.1x4, ldc.i4.m1x4, ldloc.0x4, leavex4, newobjx4, br.sx3, ldc.i4.sx3, popx3, throwx3, beq.sx2, isinstx2, ldc.i4.2x2, ldloc.3x2, nopx2, beqx1, brx1, brfalsex1, ldc.i4.3x1, ldc.i4.5x1, ldloc.1x1, ldloc.2x1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, switchx1
- string literals: "Custom game '", "' has no install.url.", "server-download", "Downloading ", ".zip", "none", "zip", "zip", "Extracting server files…", "targz", "Extracting server files…", "{server_root}", "{server_file}"
- calls: MPServerProvider.Core.Models.InstallRequest.get_Game, MPServerProvider.Core.Models.GameDefinition.get_Install, MPServerProvider.Core.Models.InstallSpec.get_Url, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.Concat, System.InvalidOperationException..ctor, System.Uri..ctor, System.Uri.get_LocalPath, System.IO.Path.GetFileName, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, MPServerProvider.Core.Models.GameDefinition.get_Name, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Models.InstallSpec.get_Archive, System.String.ToLowerInvariant, System.String.EndsWith, System.String.op_Equality, MPServerProvider.Core.Services.DownloadService.ExtractZip, MPServerProvider.Core.Services.DownloadService.FlattenSingleRootFolder, System.IO.File.Delete, MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, System.IO.File.OpenRead, System.IO.Compression.GZipStream..ctor, System.Formats.Tar.TarFile.ExtractToDirectoryAsync, System.IO.Stream.DisposeAsync, System.Threading.Tasks.ValueTask.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter.GetResult, System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture, System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw, MPServerProvider.Core.Models.GameDefinition.get_Runtime, MPServerProvider.Core.Models.RuntimeSpec.get_Executable, System.String.Replace, System.String.Trim...

### `SetStateMachine(class:129)` -> `void`
- token `0x0600039e`  flags=`481`  rva=`94580`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

