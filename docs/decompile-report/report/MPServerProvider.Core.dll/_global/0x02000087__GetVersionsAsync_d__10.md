# <GetVersionsAsync>d__10

Token: `0x02000087`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040002b2)
- `<>t__builder` : `valuetype:145<class:269<string>>`  (flags=6, token=0x040002b3)
- `<>4__this` : `class:120`  (flags=6, token=0x040002b4)
- `ct` : `valuetype:121`  (flags=6, token=0x040002b5)
- `<index>5__2` : `class:297<string,string>`  (flags=1, token=0x040002b6)
- `<>u__1` : `valuetype:137<class:297<string,string>>`  (flags=1, token=0x040002b7)
- `<>u__2` : `valuetype:137<valuetype:305<string,string>>`  (flags=1, token=0x040002b8)

## Methods

### `MoveNext()` -> `void`
- token `0x0600033e`  flags=`481`  rva=`73040`
- IL size 544 bytes, 192 instructions, max_stack=4
- opcodes: ldarg.0x26, callx20, ldloc.sx19, callvirtx11, stfldx11, stloc.sx11, ldfldx9, ldloca.sx9, dupx7, brfalse.sx6, ldfldax6, ldstrx5, stloc.0x5, brtrue.sx4, ldloc.3x4, ldloc.0x3, ldnullx3, leave.sx3, initobjx2, ldc.i4.0x2, ldc.i4.1x2, ldc.i4.m1x2, ldc.i4.sx2, ldloc.1x2, leavex2, newobjx2, beqx1, bge.sx1, br.sx1, constrained.x1, endfinallyx1, ldftnx1, ldloc.2x1, ldsfldx1, popx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, stsfldx1
- string literals: "build ", " (recommended)", "build ", " (latest)", "build "
- calls: MPServerProvider.Core.Providers.CfxProvider.GetIndexAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.Core.Providers.CfxProvider.GetChannelsAsync, System.Collections.Generic.Dictionary`2.get_Keys, System.Int32.Parse, System.Func`2..ctor, System.Linq.Enumerable.OrderByDescending, System.Linq.Enumerable.ToList, System.Collections.Generic.List`1.get_Count, System.Collections.Generic.List`1..ctor, System.Collections.Generic.List`1.Contains, System.String.Concat, System.Collections.Generic.List`1.Add, System.String.op_Inequality, System.Collections.Generic.List`1.GetEnumerator, Enumerator.get_Current, Enumerator.MoveNext, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600033f`  flags=`481`  rva=`73648`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

