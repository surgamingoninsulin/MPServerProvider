# MPServerProvider.Core.Services.WorkshopService

Token: `0x0200001c`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `SteamCmdZip` : `string`  (flags=32849, token=0x04000056)
- `_downloads` : `class:40`  (flags=33, token=0x04000057)
- `_paths` : `class:16`  (flags=33, token=0x04000058)

## Methods

### `.ctor(class:40, class:16)` -> `void`
- token `0x06000104`  flags=`6278`  rva=`33493`
- IL size 21 bytes, 9 instructions, max_stack=8
- opcodes: ldarg.0x3, stfldx2, callx1, ldarg.1x1, ldarg.2x1, retx1
- calls: System.Object..ctor

### `CacheDirFor(i8)` -> `string`
- token `0x06000105`  flags=`134`  rva=`33515`
- IL size 24 bytes, 7 instructions, max_stack=8
- opcodes: callx2, callvirtx1, ldarg.0x1, ldarga.sx1, ldfldx1, retx1
- calls: MPServerProvider.Core.Services.AppPaths.get_WorkshopDir, System.Int64.ToString, System.IO.Path.Combine

### `ItemDir(i8, i8)` -> `string`
- token `0x06000106`  flags=`134`  rva=`33540`
- IL size 20 bytes, 7 instructions, max_stack=8
- opcodes: callx3, ldarg.0x1, ldarg.1x1, ldarga.sx1, retx1
- calls: MPServerProvider.Core.Services.WorkshopService.CacheDirFor, System.Int64.ToString, System.IO.Path.Combine

### `IsCached(i8, i8)` -> `bool`
- token `0x06000107`  flags=`134`  rva=`33561`
- IL size 14 bytes, 6 instructions, max_stack=8
- opcodes: callx2, ldarg.0x1, ldarg.1x1, ldarg.2x1, retx1
- calls: MPServerProvider.Core.Services.WorkshopService.ItemDir, System.IO.Directory.Exists

### `CachedItems(i8)` -> `class:269<i8>`
- token `0x06000108`  flags=`134`  rva=`33576`
- IL size 142 bytes, 43 instructions, max_stack=3
- opcodes: callx8, dupx6, ldsfldx6, brtrue.sx4, ldftnx3, newobjx3, popx3, stsfldx3, ldloc.0x2, retx2, ldarg.0x1, ldarg.1x1, stloc.0x1
- calls: MPServerProvider.Core.Services.WorkshopService.CacheDirFor, System.IO.Directory.Exists, System.Array.Empty, System.IO.Directory.EnumerateDirectories, <>c.<CachedItems>b__7_0, System.Func`2..ctor, System.Linq.Enumerable.Select, <>c.<CachedItems>b__7_1, System.Linq.Enumerable.Where, <>c.<CachedItems>b__7_2, System.Linq.Enumerable.ToList

### `DownloadItemsAsync(class:160, class:233<class:200>, valuetype:121)` -> `class:221`
- token `0x06000109`  flags=`134`  rva=`33732`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `DeployToServer(class:160, string)` -> `void`
- token `0x0600010a`  flags=`134`  rva=`33824`
- IL size 167 bytes, 70 instructions, max_stack=4
- opcodes: callx9, callvirtx6, dupx6, ldloca.sx6, stelem.refx6, ldloc.sx3, ldstrx3, brtrue.sx2, ldarg.1x2, ldloc.0x2, retx2, stloc.sx2, br.sx1, brfalse.sx1, constrained.x1, conv.i8x1, endfinallyx1, ldarg.0x1, ldarg.2x1, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.4x1, ldc.i4.5x1, ldc.i4.6x1, ldloc.3x1, leave.sx1, newarrx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "steamapps", "workshop", "content"
- calls: MPServerProvider.Core.Models.GameDefinition.get_Install, MPServerProvider.Core.Models.InstallSpec.get_AppId, System.Nullable`1.GetValueOrDefault, MPServerProvider.Core.Models.InstallSpec.get_WorkshopIds, System.Collections.Generic.List`1.GetEnumerator, Enumerator.get_Current, MPServerProvider.Core.Services.WorkshopService.ItemDir, System.IO.Directory.Exists, System.Int32.ToString, System.Int64.ToString, System.IO.Path.Combine, MPServerProvider.Core.Services.WorkshopService.CopyDirectory, Enumerator.MoveNext, System.IDisposable.Dispose

### `ScanItemsForApp(i8, string)` -> `class:153<class:188>`
- token `0x0600010b`  flags=`134`  rva=`34020`
- IL size 156 bytes, 59 instructions, max_stack=5
- opcodes: callvirtx10, callx8, dupx5, ldloc.1x4, ldloc.3x3, ldloca.sx3, newobjx3, ldarg.0x2, ldarg.1x2, ldloc.0x2, ldloc.2x2, br.sx1, brfalse.sx1, brtrue.sx1, endfinallyx1, ldarg.2x1, ldc.i4.1x1, ldc.i4.sx1, ldstrx1, leave.sx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1, stloc.sx1
- string literals: "workshop_"
- calls: System.Collections.Generic.List`1..ctor, MPServerProvider.Core.Services.WorkshopService.CachedItems, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, MPServerProvider.Core.Services.WorkshopService.ItemDir, MPServerProvider.Core.Models.ModItem..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.ModItem.set_FileName, MPServerProvider.Core.Models.ModItem.set_GameId, MPServerProvider.Core.Models.ModItem.set_LibraryPath, MPServerProvider.Core.Services.WorkshopService.DirSize, MPServerProvider.Core.Models.ModItem.set_SizeBytes, System.IO.Directory.GetCreationTime, System.DateTimeOffset.op_Implicit, MPServerProvider.Core.Models.ModItem.set_Imported, System.Collections.Generic.List`1.Add, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose

### `EnsureSteamCmdAsync(class:233<class:200>, valuetype:121)` -> `class:117<string>`
- token `0x0600010c`  flags=`129`  rva=`34204`
- IL size 71 bytes, 23 instructions, max_stack=2
- opcodes: ldloca.sx8, stfldx5, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `CopyDirectory(string, string)` -> `void`
- token `0x0600010d`  flags=`145`  rva=`34288`
- IL size 91 bytes, 39 instructions, max_stack=3
- opcodes: callx7, callvirtx4, ldloc.0x4, ldarg.0x2, ldarg.1x2, ldc.i4.1x2, ldloc.1x2, ldloc.3x2, popx2, br.sx1, brfalse.sx1, brtrue.sx1, endfinallyx1, ldloc.2x1, ldstrx1, leave.sx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "*"
- calls: System.IO.Directory.CreateDirectory, System.IO.Directory.EnumerateFiles, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.IO.Path.GetRelativePath, System.IO.Path.Combine, System.IO.Path.GetDirectoryName, System.IO.File.Copy, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose

### `DirSize(string)` -> `i8`
- token `0x0600010e`  flags=`145`  rva=`34408`
- IL size 49 bytes, 15 instructions, max_stack=8
- opcodes: callx2, dupx2, ldsfldx2, brtrue.sx1, ldarg.0x1, ldc.i4.1x1, ldftnx1, ldstrx1, newobjx1, popx1, retx1, stsfldx1
- string literals: "*"
- calls: System.IO.Directory.EnumerateFiles, <>c.<DirSize>b__13_0, System.Func`2..ctor, System.Linq.Enumerable.Sum

