# <ShowTxAdminAsync>d__18

Token: `0x02000047`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001ba)
- `<>t__builder` : `valuetype:437`  (flags=6, token=0x040001bb)
- `<>4__this` : `class:100`  (flags=6, token=0x040001bc)
- `<>u__1` : `valuetype:181`  (flags=1, token=0x040001bd)

## Methods

### `MoveNext()` -> `void`
- token `0x060001c0`  flags=`481`  rva=`51320`
- IL size 435 bytes, 154 instructions, max_stack=5
- opcodes: ldarg.0x20, callx13, callvirtx13, ldfldx12, ldloc.1x8, stfldx8, dupx6, ldfldax6, ldloca.sx6, stloc.0x5, leave.sx4, stloc.2x4, brtrue.sx3, ldc.i4.1x3, ldloc.0x3, ldstrx3, leavex3, stloc.sx3, brfalse.sx2, initobjx2, ldc.i4.0x2, ldc.i4.2x2, ldc.i4.m1x2, ldc.i4.sx2, ldloc.2x2, ldloc.3x2, ldloc.sx2, ldnullx2, newobjx2, popx2, beqx1, beq.sx1, ldc.i4x1, nopx1, retx1, stloc.1x1, stloc.3x1
- string literals: "webview2", "http://127.0.0.1:40120/", "Could not open txAdmin: "
- calls: System.Threading.Tasks.Task.Delay, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.App.AppServices.get_Processes, MPServerProvider.Core.Services.ServerProcessService.StateOf, Microsoft.Web.WebView2.Wpf.WebView2.get_CreationProperties, Microsoft.Web.WebView2.Wpf.CoreWebView2CreationProperties..ctor, MPServerProvider.App.AppServices.get_Paths, MPServerProvider.Core.Services.AppPaths.get_CacheDir, System.IO.Path.Combine, Microsoft.Web.WebView2.Wpf.CoreWebView2CreationProperties.set_UserDataFolder, Microsoft.Web.WebView2.Wpf.WebView2.set_CreationProperties, Microsoft.Web.WebView2.Wpf.WebView2.EnsureCoreWebView2Async, System.Uri..ctor, Microsoft.Web.WebView2.Wpf.WebView2.set_Source, System.Windows.UIElement.set_Visibility, System.Exception.get_Message, System.String.Concat, System.Windows.Controls.TextBlock.set_Text, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x060001c1`  flags=`481`  rva=`51820`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

