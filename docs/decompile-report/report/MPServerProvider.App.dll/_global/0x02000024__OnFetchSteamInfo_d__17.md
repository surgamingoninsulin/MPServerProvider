# <OnFetchSteamInfo>d__17

Token: `0x02000024`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400013a)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x0400013b)
- `<>4__this` : `class:32`  (flags=6, token=0x0400013c)
- `<appId>5__2` : `i4`  (flags=1, token=0x0400013d)
- `<appInfo>5__3` : `class:357`  (flags=1, token=0x0400013e)
- `<storeId>5__4` : `i4`  (flags=1, token=0x0400013f)
- `<>u__1` : `valuetype:361<class:357>`  (flags=1, token=0x04000140)
- `<>u__2` : `valuetype:361<bool>`  (flags=1, token=0x04000141)

## Methods

### `MoveNext()` -> `void`
- token `0x06000165`  flags=`481`  rva=`40924`
- IL size 915 bytes, 303 instructions, max_stack=5
- opcodes: ldarg.0x48, ldfldx32, callx30, callvirtx22, ldstrx15, stfldx14, ldloc.1x12, ldloca.sx12, brtrue.sx10, stloc.sx10, ldfldax9, ldloc.sx9, dupx8, stloc.0x7, ldc.i4.0x6, initobjx5, leavex5, leave.sx5, br.sx4, ldc.i4.sx4, bgt.sx3, brfalse.sx3, ldc.i4.m1x3, ldloc.0x3, popx3, ldc.i4.1x2, ldc.i4.2x2, ldloc.2x2, ldloc.3x2, ldnullx2, stloc.2x2, beq.sx1, bge.sx1, ble.un.sx1, endfinallyx1, nopx1, retx1, stloc.1x1, stloc.3x1, switchx1
- string literals: "Enter a valid Steam app id first", "", "Fetching…", "{server_root}\", "Steam doesn't know this app id — check it on steamdb.info", "", "", " — tags: ", ", ", "Fetched game info from Steam", "Fetched from Steam — server exe: ", "", "Couldn't reach Steam — check your connection or enter details manually", "", "Fetch info"
- calls: System.Windows.Controls.TextBox.get_Text, System.String.Trim, System.Int32.TryParse, MPServerProvider.App.ToastService.Show, System.Windows.UIElement.set_IsEnabled, System.Windows.Controls.ContentControl.set_Content, MPServerProvider.App.AppServices.get_Installer, MPServerProvider.Core.Services.InstallService.SteamAppInfoAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.Core.Providers.SteamAppInfo.get_Name, System.String.IsNullOrWhiteSpace, System.Windows.Controls.TextBox.set_Text, MPServerProvider.App.Views.AddCustomGameDialog.Slugify, MPServerProvider.Core.Providers.SteamAppInfo.get_Executable, System.String.Replace, System.String.Concat, MPServerProvider.Core.Providers.SteamAppInfo.get_ParentAppId, System.Nullable`1.GetValueOrDefault, MPServerProvider.App.Views.AddCustomGameDialog.TryFillFromStoreAsync, System.Collections.Generic.List`1.get_Count, System.String.Join, System.String.get_Length, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x06000166`  flags=`481`  rva=`41928`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

