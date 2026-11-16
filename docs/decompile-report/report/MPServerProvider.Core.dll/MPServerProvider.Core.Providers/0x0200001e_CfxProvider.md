# MPServerProvider.Core.Providers.CfxProvider

Token: `0x0200001e`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `ChangelogApi` : `string`  (flags=32849, token=0x0400005b)
- `ArtifactBase` : `string`  (flags=32849, token=0x0400005c)
- `_downloads` : `class:40`  (flags=33, token=0x0400005d)

## Methods

### `.ctor(class:40)` -> `void`
- token `0x06000113`  flags=`6278`  rva=`34595`
- IL size 14 bytes, 6 instructions, max_stack=8
- opcodes: ldarg.0x2, callx1, ldarg.1x1, retx1, stfldx1
- calls: System.Object..ctor

### `get_Id()` -> `string`
- token `0x06000114`  flags=`2534`  rva=`34610`
- IL size 6 bytes, 2 instructions, max_stack=8
- opcodes: ldstrx1, retx1
- string literals: "cfx"

### `ArtifactLink()` -> `class:57`
- token `0x06000115`  flags=`145`  rva=`34617`
- IL size 6 bytes, 2 instructions, max_stack=8
- opcodes: ldsfldx1, retx1

### `ParseArtifactIndex(string)` -> `class:297<string,string>`
- token `0x06000116`  flags=`150`  rva=`34624`
- IL size 107 bytes, 39 instructions, max_stack=4
- opcodes: callvirtx12, ldloc.1x3, ldloc.0x2, ldloc.2x2, ldloc.3x2, br.sx1, brfalse.sx1, brtrue.sx1, callx1, castclassx1, endfinallyx1, isinstx1, ldarg.0x1, ldc.i4.1x1, ldc.i4.2x1, leave.sx1, newobjx1, popx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1
- calls: System.Collections.Generic.Dictionary`2..ctor, MPServerProvider.Core.Providers.CfxProvider.ArtifactLink, System.Text.RegularExpressions.Regex.Matches, System.Text.RegularExpressions.MatchCollection.GetEnumerator, System.Collections.IEnumerator.get_Current, System.Text.RegularExpressions.Match.get_Groups, System.Text.RegularExpressions.GroupCollection.get_Item, System.Text.RegularExpressions.Capture.get_Value, System.Collections.Generic.Dictionary`2.TryAdd, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose

### `GetIndexAsync(valuetype:121)` -> `class:117<class:297<string,string>>`
- token `0x06000117`  flags=`129`  rva=`34760`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `GetChannelsAsync(valuetype:121)` -> `class:117<valuetype:305<string,string>>`
- token `0x06000118`  flags=`129`  rva=`34836`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `GetVersionsAsync(class:160, string, valuetype:121)` -> `class:117<class:269<string>>`
- token `0x06000119`  flags=`486`  rva=`34912`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `InstallAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x0600011a`  flags=`486`  rva=`34988`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

