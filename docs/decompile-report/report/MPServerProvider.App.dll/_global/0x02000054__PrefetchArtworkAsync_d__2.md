# <PrefetchArtworkAsync>d__2

Token: `0x02000054`  Flags: `1048835`  Extends: `0x0100002b`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040001e7)
- `<>t__builder` : `valuetype:437`  (flags=6, token=0x040001e8)
- `<>4__this` : `class:112`  (flags=6, token=0x040001e9)
- `<>u__1` : `valuetype:361<class:433>`  (flags=1, token=0x040001ea)

## Methods

### `MoveNext()` -> `void`
- token `0x060001df`  flags=`481`  rva=`55356`
- IL size 339 bytes, 107 instructions, max_stack=4
- opcodes: callx16, ldarg.0x13, stfldx8, dupx6, ldloc.1x6, ldfldx5, brtrue.sx4, ldfldax4, ldsfldx4, newobjx4, callvirtx3, ldftnx3, ldloca.sx3, leave.sx3, popx3, stloc.0x3, ldc.i4.0x2, ldc.i4.sx2, stloc.2x2, stsfldx2, brfalsex1, initobjx1, ldc.i4.m1x1, ldc.r8x1, ldloc.0x1, ldloc.2x1, ldloc.3x1, leavex1, retx1, stloc.1x1, stloc.3x1
- calls: <>c__DisplayClass2_0..ctor, MPServerProvider.App.AppServices.get_Games, MPServerProvider.Core.Services.GameLibraryService.get_Games, <>c.<PrefetchArtworkAsync>b__2_0, System.Func`2..ctor, System.Linq.Enumerable.SelectMany, <>c.<PrefetchArtworkAsync>b__2_1, System.Linq.Enumerable.Where, System.Linq.Enumerable.Distinct, System.Linq.Enumerable.ToList, System.Collections.Generic.List`1.get_Count, <>c__DisplayClass2_0.<PrefetchArtworkAsync>b__2, System.Linq.Enumerable.Select, System.Threading.Tasks.Task.WhenAll, System.TimeSpan.FromSeconds, System.Threading.Tasks.Task.Delay, System.Threading.Tasks.Task.WhenAny, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:177)` -> `void`
- token `0x060001e0`  flags=`481`  rva=`55736`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

