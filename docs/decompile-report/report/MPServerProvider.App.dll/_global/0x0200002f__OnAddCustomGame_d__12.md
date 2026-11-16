# <OnAddCustomGame>d__12

Token: `0x0200002f`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400016c)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x0400016d)
- `<>4__this` : `class:56`  (flags=6, token=0x0400016e)
- `<>u__1` : `valuetype:181`  (flags=1, token=0x0400016f)

## Methods

### `MoveNext()` -> `void`
- token `0x06000180`  flags=`481`  rva=`46204`
- IL size 515 bytes, 162 instructions, max_stack=4
- opcodes: callx24, callvirtx22, ldloc.sx14, ldarg.0x13, ldstrx9, stloc.sx9, ldloc.3x6, brfalsex5, ldloca.sx5, stfldx5, dupx4, ldfldax4, ldloc.1x4, leave.sx4, brfalse.sx3, ldfldx3, ldloc.2x3, stloc.0x3, brtrue.sx2, ldc.i4.sx2, ldloc.0x2, leavex2, newobjx2, popx2, br.sx1, initobjx1, isinstx1, ldc.i4.0x1, ldc.i4.m1x1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: ".json", " installed", "", "Firewall rule skipped (needs admin approval)", "", "servers", "'", "' added to library", ""
- calls: MPServerProvider.App.Views.AddCustomGameDialog..ctor, System.Windows.Window.GetWindow, System.Windows.Window.set_Owner, System.Windows.Window.ShowDialog, System.Nullable`1.GetValueOrDefault, MPServerProvider.App.Views.AddCustomGameDialog.get_CreatedGame, MPServerProvider.App.AppServices.get_Paths, MPServerProvider.Core.Services.AppPaths.get_GamesDir, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.Concat, System.IO.Path.Combine, System.IO.Directory.CreateDirectory, MPServerProvider.Core.Json.Json.WriteFile, MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Services.GameLibraryService.Reload, MPServerProvider.App.Views.LibraryPage.Reload, MPServerProvider.App.Views.AddCustomGameDialog.get_ImportFolder, MPServerProvider.App.Views.ManualInstallDialog..ctor, MPServerProvider.App.Views.ManualInstallDialog.get_InstalledServer, MPServerProvider.Core.Models.ServerInstance.get_ServerName, MPServerProvider.App.ToastService.Show, MPServerProvider.Core.Models.ServerInstance.get_CreateFirewallRule, MPServerProvider.App.AppServices.get_Firewall, MPServerProvider.Core.Models.ServerInstance.get_ServerPort, MPServerProvider.Core.Models.GameDefinition.get_Network, MPServerProvider.Core.Models.NetworkSpec.get_Protocol, MPServerProvider.Core.Services.FirewallService.AddRuleAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.App.Views.MainWindow.RefreshHome, MPServerProvider.App.Views.MainWindow.NavigateTo, MPServerProvider.Core.Models.GameDefinition.get_Name, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x06000181`  flags=`481`  rva=`46784`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

