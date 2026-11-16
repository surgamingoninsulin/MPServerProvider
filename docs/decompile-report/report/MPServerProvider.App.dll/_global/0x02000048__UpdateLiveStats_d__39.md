# <UpdateLiveStats>d__39

Token: `0x02000048`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001be)
- `<>t__builder` : `valuetype:185`  (flags=6, token=0x040001bf)
- `<>4__this` : `class:100`  (flags=6, token=0x040001c0)
- `<running>5__2` : `bool`  (flags=1, token=0x040001c1)
- `<>u__1` : `valuetype:361<valuetype:325<valuetype:509<i4,i4>>>`  (flags=1, token=0x040001c2)

## Methods

### `MoveNext()` -> `void`
- token `0x060001c2`  flags=`481`  rva=`51836`
- IL size 1272 bytes, 376 instructions, max_stack=4
- opcodes: callx60, ldloca.sx37, ldfldx33, ldloc.1x33, callvirtx30, stloc.sx22, ldstrx21, ldloc.sx20, ldarg.0x17, ldc.r8x10, stfldx10, brfalsex6, ldfldax6, ldloc.2x5, newobjx5, brtrue.sx4, dupx4, initobjx4, ldc.i4.1x4, ldc.i4.3x4, leave.sx4, br.sx3, conv.r8x3, divx3, ldc.i4.2x3, ldc.i4.sx3, mulx3, stloc.0x3, brfalse.sx2, leavex2, ble.unx1, brtruex1, ceqx1, conv.i4x1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.0x1, popx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1
- string literals: "—", "00", ":", "00", ":", "00", "0", " MB", "0.0", "%", "—", "none", "127.0.0.1", "—", " / ", "Local address: 127.0.0.1:", "
", "LAN address: ", ":", "
", "Internet: use Connect to Tunnel (UPnP or playit.gg) to go public."
- calls: System.Windows.FrameworkElement.get_IsLoaded, MPServerProvider.App.Views.ServerDetailView.RefreshHeader, MPServerProvider.App.AppServices.get_Processes, MPServerProvider.Core.Services.ServerProcessService.ProcessOf, MPServerProvider.Core.Services.ServerProcessService.StateOf, MPServerProvider.Core.Services.ServerProcessService.UptimeOf, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.TimeSpan.get_TotalHours, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.TimeSpan.get_Minutes, System.TimeSpan.get_Seconds, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Windows.Controls.TextBlock.set_Text, System.Diagnostics.Process.get_HasExited, System.Diagnostics.Process.Refresh, System.Diagnostics.Process.get_WorkingSet64, MPServerProvider.Core.Models.ServerInstance.get_RamMaxMb, System.Math.Max, System.Math.Clamp, System.Windows.Controls.Primitives.RangeBase.set_Value, System.DateTime.get_UtcNow, System.Diagnostics.Process.get_TotalProcessorTime, System.DateTime.op_Inequality, System.TimeSpan.op_Subtraction, System.TimeSpan.get_TotalMilliseconds, System.DateTime.op_Subtraction, System.Environment.get_ProcessorCount, System.TimeSpan.get_TotalSeconds, MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Models.ServerInstance.get_ServerGame, MPServerProvider.Core.Services.GameLibraryService.Find, MPServerProvider.Core.Models.GameDefinition.get_Network, MPServerProvider.Core.Models.NetworkSpec.get_Query, System.String.op_Inequality, MPServerProvider.App.AppServices.get_Query, MPServerProvider.Core.Models.ServerInstance.get_ServerPort, MPServerProvider.Core.Services.QueryService.QueryAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted...

### `SetStateMachine(class:177)` -> `void`
- token `0x060001c3`  flags=`481`  rva=`53172`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncVoidMethodBuilder.SetStateMachine

