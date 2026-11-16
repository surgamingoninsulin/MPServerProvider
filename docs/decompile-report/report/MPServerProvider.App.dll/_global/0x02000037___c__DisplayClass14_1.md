# <>c__DisplayClass14_1

Token: `0x02000037`  Flags: `1048835`  Extends: `0x0100001a`

## Fields

- `serverName` : `string`  (flags=6, token=0x04000188)
- `server` : `class:497`  (flags=6, token=0x04000189)
- `CS$<>8__locals1` : `class:216`  (flags=6, token=0x0400018a)

## Methods

### `.ctor()` -> `void`
- token `0x06000195`  flags=`6278`  rva=`48360`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<OnDeleteMod>b__1(object, class:329)` -> `void`
- token `0x06000196`  flags=`131`  rva=`48368`
- IL size 122 bytes, 32 instructions, max_stack=3
- opcodes: ldfldx8, callx6, ldarg.0x5, callvirtx4, ldstrx4, leave.sx2, ldloc.0x1, retx1, stloc.0x1
- string literals: " removed from ", "", "Remove failed: ", ""
- calls: MPServerProvider.App.AppServices.get_Mods, MPServerProvider.App.Views.ModCardVm.get_Mod, MPServerProvider.Core.Services.ModLibraryService.RemoveFromServer, MPServerProvider.App.Views.ModCardVm.get_FileName, System.String.Concat, MPServerProvider.App.ToastService.Show, System.Exception.get_Message, MPServerProvider.App.Views.ModsPage.Refresh

