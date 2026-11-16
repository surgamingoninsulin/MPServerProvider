# <>c__DisplayClass43_0

Token: `0x02000042`  Flags: `1048835`  Extends: `0x0100001a`

## Fields

- `server` : `class:497`  (flags=6, token=0x040001a7)
- `deleteFiles` : `bool`  (flags=6, token=0x040001a8)

## Methods

### `.ctor()` -> `void`
- token `0x060001b6`  flags=`6278`  rva=`49910`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<OnDeleteServer>b__0()` -> `void`
- token `0x060001b7`  flags=`131`  rva=`49918`
- IL size 23 bytes, 7 instructions, max_stack=8
- opcodes: ldarg.0x2, ldfldx2, callx1, callvirtx1, retx1
- calls: MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.Delete

