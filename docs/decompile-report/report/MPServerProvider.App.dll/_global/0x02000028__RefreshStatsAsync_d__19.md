# <RefreshStatsAsync>d__19

Token: `0x02000028`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000152)
- `<>t__builder` : `valuetype:437`  (flags=6, token=0x04000153)
- `<>4__this` : `class:48`  (flags=6, token=0x04000154)
- `<total>5__2` : `i4`  (flags=1, token=0x04000155)
- `<>7__wrap2` : `class:505<class:497>`  (flags=1, token=0x04000156)
- `<>u__1` : `valuetype:361<valuetype:325<valuetype:509<i4,i4>>>`  (flags=1, token=0x04000157)

## Methods

### `MoveNext()` -> `void`
- token `0x06000170`  flags=`481`  rva=`43208`
- IL size 1036 bytes, 328 instructions, max_stack=8
- opcodes: callx46, callvirtx33, ldloca.sx25, ldarg.0x23, stloc.sx23, ldloc.sx21, ldfldx16, ldstrx14, stfldx11, ldloc.1x9, ldc.i4.0x7, ldloc.2x6, ldc.i4.1x5, ldc.r8x5, ldfldax5, br.sx4, brfalse.sx4, brtrue.sx4, dupx4, ldc.i4.sx4, ldloc.0x4, leave.sx4, newobjx4, addx3, brfalsex3, ldc.i4.2x3, ldloc.3x3, stloc.0x3, beq.sx2, bge.sx2, brx2, brtruex2, conv.r8x2, divx2, endfinallyx2, ldnullx2, ldsfldx2, ble.unx1, bne.un.sx1, initobjx1, ldc.i4.m1x1, ldftnx1, leavex1, mulx1, nopx1, popx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, stsfldx1
- string literals: "—", "0.0", "%", " server", "s", "", " ready", " mod", "s", "", " in library", "Import mods & plugins", "none", "127.0.0.1"
- calls: MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.get_Servers, System.Collections.Generic.IReadOnlyCollection`1.get_Count, System.Windows.Controls.TextBlock.set_Text, System.DateTimeOffset.get_Now, System.DateTimeOffset.get_Year, System.DateTimeOffset.get_Month, System.DateTimeOffset.get_Offset, System.DateTimeOffset..ctor, System.DateTimeOffset.op_Subtraction, System.TimeSpan.get_TotalSeconds, System.Math.Max, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, MPServerProvider.Core.Models.ServerInstance.get_Stats, MPServerProvider.Core.Models.ServerStats.get_MonthUptimeSeconds, MPServerProvider.App.AppServices.get_Processes, MPServerProvider.Core.Services.ServerProcessService.StateOf, MPServerProvider.Core.Services.ServerProcessService.UptimeOf, System.Math.Min, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.App.AppServices.get_Mods, MPServerProvider.Core.Services.ModLibraryService.Scan, System.DateTime.get_UtcNow, System.DateTime.op_Subtraction, <>c.<RefreshStatsAsync>b__19_0, System.Func`2..ctor, System.Linq.Enumerable.Where, MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Models.ServerInstance.get_ServerGame, MPServerProvider.Core.Services.GameLibraryService.Find, MPServerProvider.Core.Models.GameDefinition.get_Network, MPServerProvider.Core.Models.NetworkSpec.get_Query, System.String.op_Equality, MPServerProvider.App.AppServices.get_Query...

### `SetStateMachine(class:177)` -> `void`
- token `0x06000171`  flags=`481`  rva=`44332`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

