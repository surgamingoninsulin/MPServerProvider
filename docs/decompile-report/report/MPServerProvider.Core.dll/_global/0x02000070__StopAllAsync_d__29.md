# <StopAllAsync>d__29

Token: `0x02000070`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000232)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x04000233)
- `<>4__this` : `class:80`  (flags=6, token=0x04000234)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x04000235)

## Methods

### `MoveNext()` -> `void`
- token `0x06000304`  flags=`481`  rva=`64544`
- IL size 218 bytes, 76 instructions, max_stack=3
- opcodes: ldarg.0x13, callx9, stfldx5, dupx4, ldfldx4, ldfldax4, ldloca.sx3, leave.sx3, stloc.0x3, brtrue.sx2, callvirtx2, ldc.i4.sx2, ldftnx2, ldloc.1x2, ldsfldx2, newobjx2, stloc.2x2, brfalse.sx1, initobjx1, ldc.i4.0x1, ldc.i4.m1x1, ldloc.0x1, ldloc.2x1, ldloc.3x1, popx1, retx1, stloc.1x1, stloc.3x1, stsfldx1
- calls: System.Collections.Concurrent.ConcurrentDictionary`2.get_Values, <>c.<StopAllAsync>b__29_0, System.Func`2..ctor, System.Linq.Enumerable.Where, MPServerProvider.Core.Services.ServerProcessService.<StopAllAsync>b__29_1, System.Linq.Enumerable.Select, System.Linq.Enumerable.ToList, System.Threading.Tasks.Task.WhenAll, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000305`  flags=`481`  rva=`64792`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

