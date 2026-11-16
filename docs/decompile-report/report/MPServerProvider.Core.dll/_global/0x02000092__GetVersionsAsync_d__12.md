# <GetVersionsAsync>d__12

Token: `0x02000092`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040002f9)
- `<>t__builder` : `valuetype:145<class:269<string>>`  (flags=6, token=0x040002fa)
- `variant` : `string`  (flags=6, token=0x040002fb)
- `<>4__this` : `class:140`  (flags=6, token=0x040002fc)
- `ct` : `valuetype:121`  (flags=6, token=0x040002fd)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x040002fe)

## Methods

### `MoveNext()` -> `void`
- token `0x06000360`  flags=`481`  rva=`78036`
- IL size 1881 bytes, 616 instructions, max_stack=4
- opcodes: callx70, ldarg.0x55, ldloca.sx53, stloc.sx45, ldloc.sx39, callvirtx33, dupx30, ldfldx21, stfldx20, brtrue.sx18, ldstrx18, ldsfldx16, ldfldax14, ldc.i4.0x13, stloc.0x13, initobjx12, leavex12, newobjx11, ldloc.0x10, bge.sx9, endfinallyx9, popx9, ldftnx8, stsfldx8, stloc.2x7, brfalse.sx6, ldc.i4.m1x6, ldloc.1x6, ldloc.3x6, boxx5, brtruex5, leave.sx5, br.sx3, constrained.x3, ldc.i4.sx3, ldc.i4.1x2, ble.sx1, brx1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.4x1, ldc.i4.5x1, ldloc.2x1, newarrx1, retx1, stelem.refx1, stloc.1x1, stloc.3x1, switchx1
- string literals: "paper", "fabric", "purpur", "velocity", "bungeecord", "forge", "https://fill.papermc.io/v3/projects/paper", "versions", "https://meta.fabricmc.net/v2/versions/game", "https://api.purpurmc.org/v2/purpur", "versions", "https://fill.papermc.io/v3/projects/velocity", "versions", "latest", "https://files.minecraftforge.net/net/minecraftforge/forge/promotions_slim.json", "promos", "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json", "versions"
- calls: System.String.op_Equality, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Collections.Generic.List`1..ctor, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.EnumerateObject, ObjectEnumerator.GetEnumerator, ObjectEnumerator.get_Current, System.Text.Json.JsonProperty.get_Value, System.Text.Json.JsonElement.EnumerateArray, <>c.<GetVersionsAsync>b__12_0, System.Func`2..ctor, System.Linq.Enumerable.Select, System.Collections.Generic.List`1.AddRange, ObjectEnumerator.MoveNext, System.IDisposable.Dispose, <>c.<GetVersionsAsync>b__12_1, System.Linq.Enumerable.Where, <>c.<GetVersionsAsync>b__12_2, System.Linq.Enumerable.ToList, <>c.<GetVersionsAsync>b__12_3, System.Collections.Generic.List`1.Reverse, <>c.<GetVersionsAsync>b__12_4, System.Collections.Generic.HashSet`1..ctor, System.Text.Json.JsonProperty.get_Name, System.String.IndexOf, System.String.Substring, System.Collections.Generic.HashSet`1.Add, <>c.<GetVersionsAsync>b__12_5, System.Linq.Enumerable.OrderByDescending, <>c.<GetVersionsAsync>b__12_6, <>c.<GetVersionsAsync>b__12_7, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000361`  flags=`481`  rva=`80176`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

