# <>c__DisplayClass9_0

Token: `0x020000a0`  Flags: `1048835`  Extends: `0x01000015`

## Fields

- `progress` : `class:233<class:200>`  (flags=6, token=0x04000362)
- `recentOutput` : `class:369<string>`  (flags=6, token=0x04000363)

## Methods

### `.ctor()` -> `void`
- token `0x06000393`  flags=`6278`  rva=`89584`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<InstallAsync>g__Capture|0(string)` -> `void`
- token `0x06000394`  flags=`131`  rva=`89592`
- IL size 344 bytes, 109 instructions, max_stack=6
- opcodes: callx26, ldloca.sx19, callvirtx10, ldarg.0x6, ldfldx6, ldstrx5, dupx4, ldloc.0x3, ldloc.sx3, br.sx2, brfalse.sx2, ldarg.1x2, ldc.i4.0x2, newobjx2, retx2, stloc.sx2, bgt.sx1, blex1, brfalsex1, conv.i8x1, endfinallyx1, ldc.i4.3x1, ldc.i4.8x1, ldc.i4.sx1, leave.sx1, popx1, stloc.0x1, stloc.1x1, stloc.2x1
- string literals: "SteamCMD: ", " ", " / ", " MB", "SteamCMD: "
- calls: System.String.IsNullOrWhiteSpace, System.String.Trim, MPServerProvider.Core.Providers.SteamCmdProvider.ParseProgress, System.Nullable`1.get_HasValue, System.Nullable`1.GetValueOrDefault, SteamProgress.get_Total, MPServerProvider.Core.Models.InstallProgress..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, SteamProgress.get_State, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, SteamProgress.get_Done, MPServerProvider.Core.Providers.SteamCmdProvider.Megabytes, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallProgress.set_Status, SteamProgress.get_Percent, MPServerProvider.Core.Models.InstallProgress.set_Percent, MPServerProvider.Core.Models.InstallProgress.set_BytesReceived, System.Nullable`1..ctor, MPServerProvider.Core.Models.InstallProgress.set_TotalBytes, System.IProgress`1.Report, System.String.Concat, MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.Threading.Monitor.Enter, System.Collections.Generic.Queue`1.Enqueue, System.Collections.Generic.Queue`1.Dequeue, System.Collections.Generic.Queue`1.get_Count, System.Threading.Monitor.Exit

### `<InstallAsync>b__1(object, class:377)` -> `void`
- token `0x06000395`  flags=`131`  rva=`89964`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, ldarg.2x1, retx1
- calls: System.Diagnostics.DataReceivedEventArgs.get_Data, <>c__DisplayClass9_0.<InstallAsync>g__Capture|0

### `<InstallAsync>b__2(object, class:377)` -> `void`
- token `0x06000396`  flags=`131`  rva=`89978`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldarg.0x1, ldarg.2x1, retx1
- calls: System.Diagnostics.DataReceivedEventArgs.get_Data, <>c__DisplayClass9_0.<InstallAsync>g__Capture|0

