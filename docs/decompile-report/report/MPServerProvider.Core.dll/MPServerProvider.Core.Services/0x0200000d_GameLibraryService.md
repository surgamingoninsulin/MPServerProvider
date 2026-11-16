# MPServerProvider.Core.Services.GameLibraryService

Token: `0x0200000d`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_paths` : `class:16`  (flags=33, token=0x0400001e)
- `_games` : `class:153<class:160>`  (flags=33, token=0x0400001f)
- `StaleArtworkUrls` : `class:289<string>`  (flags=49, token=0x04000020)

## Methods

### `get_Games()` -> `class:269<class:160>`
- token `0x06000048`  flags=`2182`  rva=`18520`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `.ctor(class:16)` -> `void`
- token `0x06000049`  flags=`6278`  rva=`18528`
- IL size 25 bytes, 9 instructions, max_stack=8
- opcodes: ldarg.0x3, stfldx2, callx1, ldarg.1x1, newobjx1, retx1
- calls: System.Collections.Generic.List`1..ctor, System.Object..ctor

### `Reload()` -> `void`
- token `0x0600004a`  flags=`134`  rva=`18556`
- IL size 302 bytes, 93 instructions, max_stack=3
- opcodes: callvirtx12, callx10, ldfldx10, ldloc.3x8, ldarg.0x6, brfalse.sx4, dupx4, ldloc.1x4, ldsfldx4, leave.sx4, newobjx4, ldftnx3, popx3, brtrue.sx2, ldloc.2x2, stsfldx2, brx1, brtruex1, endfinallyx1, ldloc.0x1, ldstrx1, retx1, stfldx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "*.json"
- calls: MPServerProvider.Core.Services.GameLibraryService.MigrateLegacyLayout, MPServerProvider.Core.Services.GameLibraryService.SeedBuiltIns, System.Collections.Generic.List`1.Clear, MPServerProvider.Core.Services.BuiltInGames.All, <>c.<Reload>b__5_0, System.Func`2..ctor, System.Linq.Enumerable.Select, System.Linq.Enumerable.ToHashSet, MPServerProvider.Core.Services.AppPaths.get_GamesDir, System.IO.Directory.EnumerateFiles, <>c.<Reload>b__5_1, System.Linq.Enumerable.OrderBy, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, <>c__DisplayClass5_0..ctor, MPServerProvider.Core.Json.Json.ReadFile, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Models.GameDefinition.set_SourceFile, System.Collections.Generic.HashSet`1.Contains, MPServerProvider.Core.Models.GameDefinition.set_IsBuiltIn, <>c__DisplayClass5_0.<Reload>b__2, System.Linq.Enumerable.All, System.Collections.Generic.List`1.Add, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose

### `Find(string)` -> `class:160`
- token `0x0600004b`  flags=`134`  rva=`18900`
- IL size 37 bytes, 12 instructions, max_stack=3
- opcodes: ldloc.0x2, newobjx2, callx1, ldarg.0x1, ldarg.1x1, ldfldx1, ldftnx1, retx1, stfldx1, stloc.0x1
- calls: <>c__DisplayClass6_0..ctor, <>c__DisplayClass6_0.<Find>b__0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault

### `AllGenres()` -> `class:269<string>`
- token `0x0600004c`  flags=`134`  rva=`18952`
- IL size 89 bytes, 25 instructions, max_stack=3
- opcodes: callx4, dupx4, ldsfldx4, brtrue.sx2, ldftnx2, newobjx2, popx2, stsfldx2, ldarg.0x1, ldfldx1, retx1
- calls: <>c.<AllGenres>b__7_0, System.Func`2..ctor, System.Linq.Enumerable.SelectMany, System.Linq.Enumerable.Distinct, <>c.<AllGenres>b__7_1, System.Linq.Enumerable.OrderBy, System.Linq.Enumerable.ToList

### `MigrateLegacyLayout()` -> `void`
- token `0x0600004d`  flags=`129`  rva=`19056`
- IL size 353 bytes, 118 instructions, max_stack=2
- opcodes: callx26, callvirtx14, ldloc.sx10, ldloc.3x8, brtrue.sx6, ldarg.0x6, ldfldx6, ldloc.0x5, br.sx4, ldloc.2x4, leave.sx4, stloc.sx4, brfalse.sx3, ldstrx3, popx3, brfalsex2, endfinallyx2, ldloc.1x2, stloc.3x2, retx1, stloc.0x1, stloc.1x1, stloc.2x1
- string literals: "*.json", "art", "openrct2.json"
- calls: MPServerProvider.Core.Services.AppPaths.get_LegacyGamesDir, System.IO.Directory.Exists, MPServerProvider.Core.Services.AppPaths.get_GamesDir, System.IO.Directory.CreateDirectory, System.IO.Directory.EnumerateFiles, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, System.IO.Path.GetFileName, System.IO.Path.Combine, System.IO.File.Exists, System.IO.File.Move, System.IO.File.Delete, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, MPServerProvider.Core.Services.AppPaths.get_GameArtOverrideDir, System.IO.Directory.EnumerateFileSystemEntries, System.Linq.Enumerable.Any, System.IO.Directory.Delete

### `SeedBuiltIns()` -> `void`
- token `0x0600004e`  flags=`129`  rva=`19500`
- IL size 258 bytes, 84 instructions, max_stack=3
- opcodes: callvirtx18, callx11, ldloc.3x7, ldloc.1x5, brfalse.sx4, ldloc.2x4, brtrue.sx3, ldloca.sx3, ldstrx3, leave.sx3, stloc.sx3, brx2, ldc.i4.1x2, brtruex1, constrained.x1, endfinallyx1, ldarg.0x1, ldc.i4.0x1, ldfldx1, ldloc.sx1, ldsfldx1, leavex1, nopx1, popx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: ".json", "cfx", "+exec server.cfg"
- calls: MPServerProvider.Core.Services.BuiltInGames.All, System.Collections.Generic.List`1.GetEnumerator, Enumerator.get_Current, MPServerProvider.Core.Services.AppPaths.get_GamesDir, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.Concat, System.IO.Path.Combine, System.IO.File.Exists, MPServerProvider.Core.Json.Json.WriteFile, MPServerProvider.Core.Json.Json.ReadFile, MPServerProvider.Core.Models.GameDefinition.get_Artwork, MPServerProvider.Core.Models.ArtworkSpec.get_Cover, System.String.IsNullOrWhiteSpace, System.Collections.Generic.HashSet`1.Contains, MPServerProvider.Core.Models.GameDefinition.set_Artwork, MPServerProvider.Core.Models.GameDefinition.get_Install, MPServerProvider.Core.Models.InstallSpec.get_Provider, System.String.op_Equality, MPServerProvider.Core.Models.GameDefinition.get_Runtime, MPServerProvider.Core.Models.RuntimeSpec.get_Arguments, System.String.Contains, MPServerProvider.Core.Models.GameDefinition.set_Runtime, Enumerator.MoveNext, System.IDisposable.Dispose

### `.cctor()` -> `void`
- token `0x0600004f`  flags=`6289`  rva=`19800`
- IL size 52 bytes, 16 instructions, max_stack=8
- opcodes: callvirtx3, dupx3, ldstrx3, popx3, callx1, newobjx1, retx1, stsfldx1
- string literals: "https://forum.cfx.re/uploads/default/original/4X/7/5/e/75ef9fcabc1abea8fce0ebd0236a4132710fcb2e.png", "https://openrct2.org/uploads/images/logo.png", "https://rsg-fivem-forum-assets.s3.dualstack.us-west-2.amazonaws.com/original/5X/e/d/8/1/ed819c68cedac6e7bb496bb7b8423f7949e51c79.png"
- calls: System.StringComparer.get_OrdinalIgnoreCase, System.Collections.Generic.HashSet`1..ctor, System.Collections.Generic.HashSet`1.Add

