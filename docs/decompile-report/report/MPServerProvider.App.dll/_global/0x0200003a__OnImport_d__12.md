# <OnImport>d__12

Token: `0x0200003a`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000190)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x04000191)
- `<>4__this` : `class:80`  (flags=6, token=0x04000192)
- `<kind>5__2` : `string`  (flags=1, token=0x04000193)
- `<extensions>5__3` : `class:289<string>`  (flags=1, token=0x04000194)
- `<>u__1` : `valuetype:361<i4>`  (flags=1, token=0x04000195)

## Methods

### `MoveNext()` -> `void`
- token `0x0600019e`  flags=`481`  rva=`48700`
- IL size 670 bytes, 213 instructions, max_stack=4
- opcodes: callx30, ldarg.0x21, ldstrx17, ldloca.sx16, stfldx14, callvirtx13, ldloc.sx9, stloc.sx9, ldfldx8, ldloc.2x7, ldloc.3x6, newobjx6, ldnullx5, ldc.i4.sx4, ldfldax4, ldloc.1x4, brtrue.sx3, dupx3, ldc.i4.0x3, leavex3, stloc.0x3, br.sx2, brfalse.sx2, ldftnx2, leave.sx2, stloc.3x2, beq.sx1, bge.sx1, ble.sx1, brx1, brfalsex1, initobjx1, isinstx1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.m1x1, ldloc.0x1, retx1, stloc.1x1, stloc.2x1
- string literals: "All Games", "Pick a game in the Show filter first, then import", "", "Import ", "s for ", " — pick the folder that contains them", "Import failed — see logs for details", "", "Imported ", " ", "s", "", "", "No matching files (", ", ", ") found in that folder", ""
- calls: <>c__DisplayClass12_0..ctor, System.Windows.Controls.Primitives.Selector.get_SelectedItem, System.String.op_Inequality, MPServerProvider.App.Views.ModsPage.get_ModdableGames, <>c__DisplayClass12_0.<OnImport>b__0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault, MPServerProvider.App.ToastService.Show, Microsoft.Win32.OpenFolderDialog..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, MPServerProvider.Core.Models.GameDefinition.get_Mods, MPServerProvider.Core.Models.ModsSpec.get_Kind, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, MPServerProvider.Core.Models.GameDefinition.get_Name, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, Microsoft.Win32.CommonItemDialog.set_Title, System.Windows.Window.GetWindow, Microsoft.Win32.CommonDialog.ShowDialog, System.Nullable`1.GetValueOrDefault, Microsoft.Win32.OpenFolderDialog.get_FolderName, MPServerProvider.Core.Models.GameDefinition.get_Id, MPServerProvider.Core.Models.ModsSpec.get_Extensions, <>c__DisplayClass12_0.<OnImport>b__1, System.Func`1..ctor, System.Threading.Tasks.Task.Run, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.String.Join, System.String.Concat, MPServerProvider.App.Views.ModsPage.Refresh, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x0600019f`  flags=`481`  rva=`49412`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

