# <InstallAsync>d__5

Token: `0x0200008e`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040002e7)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x040002e8)
- `request` : `class:208`  (flags=6, token=0x040002e9)
- `progress` : `class:233<class:200>`  (flags=6, token=0x040002ea)
- `ct` : `valuetype:121`  (flags=6, token=0x040002eb)

## Methods

### `MoveNext()` -> `void`
- token `0x0600034e`  flags=`481`  rva=`77024`
- IL size 515 bytes, 191 instructions, max_stack=4
- opcodes: callvirtx20, ldloc.sx18, callx16, ldstrx15, ldarg.0x14, dupx10, ldfldx9, stloc.sx9, stelem.refx7, ldc.i4.1x6, ldloc.2x6, brfalse.sx5, br.sx4, ldc.i4.0x4, leave.sx4, brtrue.sx3, ldfldax3, newobjx3, stloc.3x3, bge.sx2, endfinallyx2, ldc.i4.sx2, ldloc.0x2, ldloc.3x2, stfldx2, throwx2, addx1, blt.sx1, conv.i4x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.4x1, ldc.i4.5x1, ldc.i4.6x1, ldc.i4.7x1, ldelem.refx1, ldlenx1, ldloc.1x1, newarrx1, popx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1
- string literals: "Manual install requires a source folder path.", "Source folder not found: ", "Copying server files…", "*", "*", "", "server.exe", "srcds.exe", "FXServer.exe", "valheim_server.exe", "RustDedicated.exe", "eurotrucks2_server.exe", "amtrucks_server.exe", "server.exe", "manual"
- calls: MPServerProvider.Core.Models.InstallRequest.get_Version, System.String.IsNullOrWhiteSpace, System.InvalidOperationException..ctor, System.IO.Directory.Exists, System.String.Concat, System.IO.DirectoryNotFoundException..ctor, MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, System.IO.Directory.EnumerateDirectories, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.String.Replace, System.IO.Directory.CreateDirectory, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, System.IO.Directory.EnumerateFiles, System.Threading.CancellationToken.ThrowIfCancellationRequested, System.IO.File.Copy, MPServerProvider.Core.Services.DownloadService.FlattenSingleRootFolder, System.Linq.Enumerable.FirstOrDefault, System.IO.Path.GetRelativePath, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600034f`  flags=`481`  rva=`77628`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

