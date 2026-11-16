# <>c__DisplayClass13_1

Token: `0x02000035`  Flags: `1048835`  Extends: `0x0100001a`

## Fields

- `server` : `class:497`  (flags=6, token=0x04000184)
- `CS$<>8__locals1` : `class:208`  (flags=6, token=0x04000185)

## Methods

### `.ctor()` -> `void`
- token `0x06000191`  flags=`6278`  rva=`48035`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<OnAddToServer>b__1(object, class:329)` -> `void`
- token `0x06000192`  flags=`131`  rva=`48044`
- IL size 115 bytes, 29 instructions, max_stack=3
- opcodes: ldfldx8, callx5, callvirtx5, ldarg.0x5, ldstrx3, leave.sx2, retx1
- string literals: " added to ", "", ""
- calls: MPServerProvider.App.AppServices.get_Mods, MPServerProvider.App.Views.ModCardVm.get_Mod, MPServerProvider.Core.Services.ModLibraryService.AssignToServer, MPServerProvider.App.Views.ModCardVm.get_FileName, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.String.Concat, MPServerProvider.App.ToastService.Show, System.Exception.get_Message, MPServerProvider.App.Views.ModsPage.Refresh

