# MPServerProvider.Core.Services.ModLibraryService

Token: `0x0200000f`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_paths` : `class:16`  (flags=33, token=0x04000026)
- `_servers` : `class:84`  (flags=33, token=0x04000027)
- `_games` : `class:52`  (flags=33, token=0x04000028)

## Methods

### `.ctor(class:16, class:84, class:52)` -> `void`
- token `0x06000059`  flags=`6278`  rva=`20805`
- IL size 28 bytes, 12 instructions, max_stack=8
- opcodes: ldarg.0x4, stfldx3, callx1, ldarg.1x1, ldarg.2x1, ldarg.3x1, retx1
- calls: System.Object..ctor

### `Scan(string)` -> `class:269<class:188>`
- token `0x0600005a`  flags=`134`  rva=`20836`
- IL size 590 bytes, 184 instructions, max_stack=6
- opcodes: callvirtx38, ldloc.sx25, callx19, stloc.sx9, dupx8, newobjx8, brfalse.sx5, ldloca.sx5, brfalsex4, brtruex4, brtrue.sx4, endfinallyx4, ldarg.0x4, ldarg.1x4, ldfldx4, ldloc.0x4, ldloc.1x4, leave.sx4, br.sx3, ldloc.3x3, brx2, ldc.i4.5x2, ldftnx2, ldloc.2x2, ldsfldx2, constrained.x1, conv.i8x1, ldstrx1, popx1, retx1, stfldx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1, stsfldx1
- string literals: "steamcmd"
- calls: System.Collections.Generic.List`1..ctor, MPServerProvider.Core.Services.AppPaths.get_ModsDir, System.IO.Directory.EnumerateDirectories, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.IO.Path.GetFileName, System.String.Equals, System.IO.Directory.EnumerateFileSystemEntries, System.IO.Directory.Exists, MPServerProvider.Core.Models.ModItem..ctor, MPServerProvider.Core.Models.ModItem.set_FileName, MPServerProvider.Core.Models.ModItem.set_GameId, MPServerProvider.Core.Models.ModItem.set_LibraryPath, System.IO.FileInfo..ctor, System.IO.FileInfo.get_Length, MPServerProvider.Core.Services.ModLibraryService.DirSize, MPServerProvider.Core.Models.ModItem.set_SizeBytes, System.IO.File.GetCreationTime, System.DateTimeOffset.op_Implicit, MPServerProvider.Core.Models.ModItem.set_Imported, MPServerProvider.Core.Services.ModLibraryService.FindAssignments, MPServerProvider.Core.Models.ModItem.set_AssignedServers, System.Collections.Generic.List`1.Add, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, MPServerProvider.Core.Services.GameLibraryService.get_Games, MPServerProvider.Core.Models.GameDefinition.get_Install, MPServerProvider.Core.Models.InstallSpec.get_Provider, System.String.op_Inequality, MPServerProvider.Core.Models.InstallSpec.get_AppId, System.Nullable`1.get_HasValue, MPServerProvider.Core.Models.GameDefinition.get_Id, MPServerProvider.Core.Models.InstallSpec.get_WorkshopIds, System.Collections.Generic.List`1.get_Count, MPServerProvider.Core.Services.DownloadService..ctor, MPServerProvider.Core.Services.WorkshopService..ctor, System.Nullable`1.get_Value, MPServerProvider.Core.Services.WorkshopService.ScanItemsForApp, System.Collections.Generic.List`1.GetEnumerator, <>c__DisplayClass4_0..ctor...

### `ImportFromFolder(string, string)` -> `i4`
- token `0x0600005b`  flags=`134`  rva=`21540`
- IL size 238 bytes, 97 instructions, max_stack=3
- opcodes: callx14, callvirtx13, ldloc.3x8, brtrue.sx5, ldloc.sx5, br.sx3, brfalse.sx3, ldc.i4.1x3, ldloc.0x3, ldloc.1x3, ldloc.2x3, popx3, stloc.2x3, addx2, dupx2, endfinallyx2, ldarg.0x2, ldarg.1x2, ldarg.2x2, ldc.i4.0x2, ldfldx2, leave.sx2, stloc.3x2, stloc.sx2, ble.sx1, ldnullx1, newobjx1, retx1, stloc.0x1, stloc.1x1
- calls: MPServerProvider.Core.Services.GameLibraryService.Find, MPServerProvider.Core.Models.GameDefinition.get_Mods, MPServerProvider.Core.Models.ModsSpec.get_Extensions, System.Collections.Generic.List`1..ctor, MPServerProvider.Core.Services.AppPaths.get_ModsDir, System.IO.Path.Combine, System.IO.Directory.CreateDirectory, System.IO.Directory.EnumerateFiles, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.Collections.Generic.List`1.get_Count, System.IO.Path.GetExtension, System.StringComparer.get_OrdinalIgnoreCase, System.Linq.Enumerable.Contains, System.IO.Path.GetFileName, System.IO.File.Copy, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, System.IO.Directory.EnumerateDirectories, MPServerProvider.Core.Services.ModLibraryService.CopyDirectory

### `AssignToServer(class:188, class:220)` -> `void`
- token `0x0600005c`  flags=`134`  rva=`21820`
- IL size 464 bytes, 155 instructions, max_stack=3
- opcodes: callx32, ldloc.sx21, callvirtx12, ldarg.1x9, ldstrx8, stloc.sx7, brfalse.sx6, brtrue.sx6, dupx6, popx6, ldc.i4.1x5, newobjx4, br.sx3, ldarg.2x3, brfalsex2, ldarg.0x2, ldfldx2, ldftnx2, ldloc.0x2, ldloc.1x2, ldloc.3x2, ldsfldx2, stloc.2x2, ldc.i4.0x1, ldloc.2x1, retx1, stfldx1, stloc.0x1, stloc.1x1, stloc.3x1, stsfldx1, throwx1
- string literals: "This server does not support mods.", "fivem", "redm", "txData", "resources", "server.cfg", "server.cfg", "ensure "
- calls: MPServerProvider.Core.Services.ModLibraryService.ServerModsDir, System.InvalidOperationException..ctor, System.IO.Directory.CreateDirectory, MPServerProvider.Core.Models.ModItem.get_FileName, System.IO.Path.Combine, MPServerProvider.Core.Models.ModItem.get_LibraryPath, System.IO.Directory.Exists, MPServerProvider.Core.Services.ModLibraryService.CopyDirectory, System.IO.File.Copy, MPServerProvider.Core.Models.ServerInstance.get_ServerGame, System.String.op_Equality, MPServerProvider.Core.Services.ServerRepository.RootFor, System.IO.Directory.EnumerateDirectories, <>c.<AssignToServer>b__6_0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault, System.IO.Directory.Delete, System.IO.Path.GetDirectoryName, System.IO.File.Exists, System.IO.Directory.EnumerateFiles, <>c__DisplayClass6_0..ctor, System.IO.File.ReadAllLines, System.Linq.Enumerable.ToList, System.String.Concat, <>c__DisplayClass6_0.<AssignToServer>b__1, System.Linq.Enumerable.Any, System.Collections.Generic.List`1.Add, System.IO.File.WriteAllLines

### `RemoveFromServer(class:188, class:220)` -> `void`
- token `0x0600005d`  flags=`134`  rva=`22296`
- IL size 363 bytes, 124 instructions, max_stack=3
- opcodes: callx25, ldloc.sx20, stloc.sx9, brfalse.sx8, callvirtx8, ldstrx6, br.sx4, brtrue.sx4, ldc.i4.1x4, ldloc.0x4, ldloc.2x4, ldarg.2x3, ldfldx3, brfalsex2, ldarg.0x2, ldloc.1x2, ldloc.3x2, newobjx2, retx2, endfinallyx1, ldarg.1x1, ldc.i4.0x1, ldftnx1, leave.sx1, stfldx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "fivem", "redm", "txData", "resources", "server.cfg", "server.cfg"
- calls: <>c__DisplayClass7_0..ctor, MPServerProvider.Core.Services.ModLibraryService.ServerModsDir, MPServerProvider.Core.Models.ModItem.get_FileName, System.IO.Path.Combine, System.IO.Directory.Exists, System.IO.Directory.Delete, System.IO.File.Exists, System.IO.File.Delete, MPServerProvider.Core.Models.ServerInstance.get_ServerGame, System.String.op_Equality, MPServerProvider.Core.Services.ServerRepository.RootFor, System.IO.Directory.EnumerateDirectories, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, System.IO.Directory.EnumerateFiles, System.Linq.Enumerable.FirstOrDefault, System.IO.File.ReadAllLines, <>c__DisplayClass7_0.<RemoveFromServer>b__0, System.Func`2..ctor, System.Linq.Enumerable.Where, System.Linq.Enumerable.ToList, System.IO.File.WriteAllLines

### `DeleteFromLibrary(class:188)` -> `void`
- token `0x0600005e`  flags=`134`  rva=`22688`
- IL size 51 bytes, 17 instructions, max_stack=8
- opcodes: callx4, callvirtx4, ldarg.1x4, brfalse.sx2, retx2, ldc.i4.1x1
- calls: MPServerProvider.Core.Models.ModItem.get_LibraryPath, System.IO.Directory.Exists, System.IO.Directory.Delete, System.IO.File.Exists, System.IO.File.Delete

### `ServerModsDir(class:220)` -> `string`
- token `0x0600005f`  flags=`134`  rva=`22740`
- IL size 39 bytes, 14 instructions, max_stack=8
- opcodes: callvirtx3, ldarg.1x3, callx2, retx2, brfalse.sx1, ldarg.0x1, ldfldx1, ldnullx1
- calls: MPServerProvider.Core.Models.ServerInstance.get_ModsFolder, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Services.ServerRepository.RootFor, System.IO.Path.Combine

### `FindAssignments(string, string)` -> `class:153<string>`
- token `0x06000060`  flags=`129`  rva=`22780`
- IL size 347 bytes, 121 instructions, max_stack=4
- opcodes: ldloc.sx19, callvirtx14, callx13, stloc.sx9, brfalse.sx7, brtrue.sx5, ldloc.0x5, ldfldx4, ldloc.2x4, ldstrx4, ldarg.0x3, ldloc.1x3, leave.sx3, newobjx3, brx2, br.sx2, dupx2, endfinallyx2, ldarg.2x2, stfldx2, brfalsex1, brtruex1, ldarg.1x1, ldc.i4.0x1, ldc.i4.1x1, ldftnx1, ldloc.3x1, popx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "fivem", "redm", "txData", "resources"
- calls: <>c__DisplayClass10_0..ctor, System.Collections.Generic.List`1..ctor, MPServerProvider.Core.Services.ServerRepository.get_Servers, <>c__DisplayClass10_0.<FindAssignments>b__0, System.Func`2..ctor, System.Linq.Enumerable.Where, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, MPServerProvider.Core.Services.ModLibraryService.ServerModsDir, System.IO.Path.Combine, System.IO.File.Exists, System.IO.Directory.Exists, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.Collections.Generic.List`1.Add, System.String.op_Equality, MPServerProvider.Core.Services.ServerRepository.RootFor, System.IO.Directory.EnumerateDirectories, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose

### `CopyDirectory(string, string)` -> `void`
- token `0x06000061`  flags=`145`  rva=`23192`
- IL size 91 bytes, 39 instructions, max_stack=3
- opcodes: callx7, callvirtx4, ldloc.0x4, ldarg.0x2, ldarg.1x2, ldc.i4.1x2, ldloc.1x2, ldloc.3x2, popx2, br.sx1, brfalse.sx1, brtrue.sx1, endfinallyx1, ldloc.2x1, ldstrx1, leave.sx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "*"
- calls: System.IO.Directory.CreateDirectory, System.IO.Directory.EnumerateFiles, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.IO.Path.GetRelativePath, System.IO.Path.Combine, System.IO.Path.GetDirectoryName, System.IO.File.Copy, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose

### `DirSize(string)` -> `i8`
- token `0x06000062`  flags=`145`  rva=`23312`
- IL size 49 bytes, 15 instructions, max_stack=8
- opcodes: callx2, dupx2, ldsfldx2, brtrue.sx1, ldarg.0x1, ldc.i4.1x1, ldftnx1, ldstrx1, newobjx1, popx1, retx1, stsfldx1
- string literals: "*"
- calls: System.IO.Directory.EnumerateFiles, <>c.<DirSize>b__12_0, System.Func`2..ctor, System.Linq.Enumerable.Sum

