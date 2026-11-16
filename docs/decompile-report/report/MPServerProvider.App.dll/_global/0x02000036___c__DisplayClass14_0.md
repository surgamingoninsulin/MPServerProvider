# <>c__DisplayClass14_0

Token: `0x02000036`  Flags: `1048835`  Extends: `0x0100001a`

## Fields

- `vm` : `class:76`  (flags=6, token=0x04000186)
- `<>4__this` : `class:80`  (flags=6, token=0x04000187)

## Methods

### `.ctor()` -> `void`
- token `0x06000193`  flags=`6278`  rva=`48188`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<OnDeleteMod>b__0(object, class:329)` -> `void`
- token `0x06000194`  flags=`131`  rva=`48196`
- IL size 134 bytes, 38 instructions, max_stack=5
- opcodes: callx8, ldstrx7, callvirtx4, ldarg.0x4, ldfldx4, leave.sx2, retx2, beq.sx1, ldc.i4.4x1, ldc.i4.6x1, ldc.i4.sx1, ldloc.0x1, nopx1, stloc.0x1
- string literals: "Delete ", " from the mod library?
(Copies already on servers are kept — remove those per server.)", "MP Server Provider", "Mod deleted from library", "", "Delete failed: ", ""
- calls: System.Windows.Window.GetWindow, MPServerProvider.App.Views.ModCardVm.get_FileName, System.String.Concat, System.Windows.MessageBox.Show, MPServerProvider.App.AppServices.get_Mods, MPServerProvider.App.Views.ModCardVm.get_Mod, MPServerProvider.Core.Services.ModLibraryService.DeleteFromLibrary, MPServerProvider.App.ToastService.Show, System.Exception.get_Message, MPServerProvider.App.Views.ModsPage.Refresh

