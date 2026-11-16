# MPServerProvider.Core.Providers.UrlProvider

Token: `0x02000027`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_downloads` : `class:40`  (flags=33, token=0x0400007a)

## Methods

### `.ctor(class:40)` -> `void`
- token `0x0600015b`  flags=`6278`  rva=`40002`
- IL size 14 bytes, 6 instructions, max_stack=8
- opcodes: ldarg.0x2, callx1, ldarg.1x1, retx1, stfldx1
- calls: System.Object..ctor

### `get_Id()` -> `string`
- token `0x0600015c`  flags=`2534`  rva=`40017`
- IL size 6 bytes, 2 instructions, max_stack=8
- opcodes: ldstrx1, retx1
- string literals: "url"

### `GetVersionsAsync(class:160, string, valuetype:121)` -> `class:117<class:269<string>>`
- token `0x0600015d`  flags=`486`  rva=`40024`
- IL size 20 bytes, 8 instructions, max_stack=8
- opcodes: callx1, dupx1, ldc.i4.0x1, ldc.i4.1x1, ldstrx1, newarrx1, retx1, stelem.refx1
- string literals: "latest"
- calls: System.Threading.Tasks.Task.FromResult

### `InstallAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x0600015e`  flags=`486`  rva=`40048`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

