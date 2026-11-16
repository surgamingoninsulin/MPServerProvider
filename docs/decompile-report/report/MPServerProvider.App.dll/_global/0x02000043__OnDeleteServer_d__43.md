# <OnDeleteServer>d__43

Token: `0x02000043`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001a9)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x040001aa)
- `<>4__this` : `class:100`  (flags=6, token=0x040001ab)
- `<>8__1` : `class:264`  (flags=1, token=0x040001ac)
- `<>u__1` : `valuetype:181`  (flags=1, token=0x040001ad)

## Methods

### `MoveNext()` -> `void`
- token `0x060001b8`  flags=`481`  rva=`49944`
- IL size 529 bytes, 175 instructions, max_stack=5
- opcodes: ldarg.0x29, callx18, ldfldx16, stfldx13, ldstrx10, ldloca.sx7, callvirtx6, ldfldax6, stloc.0x5, dupx4, ldloc.1x4, leavex4, leave.sx4, stloc.3x4, brtrue.sx3, initobjx3, ldc.i4.1x3, ldc.i4.sx3, ldloc.0x3, ldloc.sx3, ldc.i4.m1x2, ldloc.2x2, ldloc.3x2, ldnullx2, newobjx2, stloc.sx2, beqx1, beq.sx1, bne.un.sx1, brfalsex1, brfalse.sx1, ceqx1, ldc.i4.0x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.6x1, ldftnx1, nopx1, retx1, stloc.1x1, stloc.2x1
- string literals: "Delete ", "?

Yes — delete the server AND all its files
No — remove it from the panel but keep the files", "MP Server Provider", "Deleting ", " and its files…", "", "Server deleted", "", "Removed from the panel, but the folder couldn't be fully deleted: ", ""
- calls: <>c__DisplayClass43_0..ctor, System.Windows.Window.GetWindow, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.String.Concat, System.Windows.MessageBox.Show, MPServerProvider.App.AppServices.get_Processes, MPServerProvider.Core.Services.ServerProcessService.StopAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.App.ToastService.Show, <>c__DisplayClass43_0.<OnDeleteServer>b__0, System.Action..ctor, System.Threading.Tasks.Task.Run, System.Exception.get_Message, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x060001b9`  flags=`481`  rva=`50540`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

