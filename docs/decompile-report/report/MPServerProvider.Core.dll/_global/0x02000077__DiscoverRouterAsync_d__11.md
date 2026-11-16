# <DiscoverRouterAsync>d__11

Token: `0x02000077`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400024d)
- `<>t__builder` : `valuetype:145<class:437>`  (flags=6, token=0x0400024e)
- `<>4__this` : `class:108`  (flags=6, token=0x0400024f)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x04000250)
- `<attempt>5__2` : `i4`  (flags=1, token=0x04000251)
- `<cts>5__3` : `class:329`  (flags=1, token=0x04000252)
- `<>u__2` : `valuetype:137<class:437>`  (flags=1, token=0x04000253)

## Methods

### `MoveNext()` -> `void`
- token `0x0600031b`  flags=`481`  rva=`66268`
- IL size 451 bytes, 178 instructions, max_stack=3
- opcodes: ldarg.0x28, ldfldx16, stfldx12, callx9, ldloc.1x8, ldc.i4.1x7, ldloc.0x7, stloc.sx7, ldfldax6, ldloca.sx6, callvirtx5, ldloc.sx5, stloc.0x5, beq.sx4, brfalse.sx4, dupx4, leavex4, ldc.i4.0x3, leave.sx3, nopx3, stloc.2x3, bge.sx2, brtrue.sx2, endfinallyx2, initobjx2, ldc.i4.m1x2, ldc.i4.sx2, newobjx2, stloc.3x2, addx1, blex1, brx1, ldc.i4.2x1, ldc.i4.3x1, ldloc.2x1, ldloc.3x1, ldnullx1, ldsfldx1, popx1, retx1, stloc.1x1, throwx1
- calls: System.Threading.SemaphoreSlim.WaitAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Threading.CancellationTokenSource..ctor, Open.Nat.NatDiscoverer..ctor, Open.Nat.NatDiscoverer.DiscoverDeviceAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.IDisposable.Dispose, System.Threading.SemaphoreSlim.Release, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600031c`  flags=`481`  rva=`66832`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

