# <VerifyHashAsync>d__7

Token: `0x02000046`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000146)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x04000147)
- `algorithm` : `class:237`  (flags=6, token=0x04000148)
- `file` : `string`  (flags=6, token=0x04000149)
- `ct` : `valuetype:121`  (flags=6, token=0x0400014a)
- `expected` : `string`  (flags=6, token=0x0400014b)
- `<>7__wrap1` : `class:237`  (flags=1, token=0x0400014c)
- `<stream>5__3` : `class:265`  (flags=1, token=0x0400014d)
- `<>7__wrap3` : `object`  (flags=1, token=0x0400014e)
- `<>7__wrap4` : `i4`  (flags=1, token=0x0400014f)
- `<>u__1` : `valuetype:137<u1[]>`  (flags=1, token=0x04000150)
- `<>u__2` : `valuetype:245`  (flags=1, token=0x04000151)

## Methods

### `MoveNext()` -> `void`
- token `0x06000280`  flags=`481`  rva=`46872`
- IL size 473 bytes, 179 instructions, max_stack=3
- opcodes: ldarg.0x39, stfldx16, ldfldx15, callx14, ldloca.sx7, callvirtx6, ldfldax6, brfalse.sx5, dupx5, ldloc.0x5, leave.sx5, stloc.0x5, brtrue.sx4, ldloc.2x4, ldnullx4, ldc.i4.0x3, ldc.i4.1x3, initobjx2, ldc.i4.m1x2, ldc.i4.sx2, ldstrx2, leavex2, nopx2, stloc.1x2, stloc.2x2, stloc.3x2, stloc.sx2, throwx2, beqx1, bge.sx1, ble.un.sx1, endfinallyx1, isinstx1, ldc.i4.5x1, ldloc.1x1, ldloc.3x1, ldloc.sx1, newobjx1, retx1
- string literals: "Checksum mismatch for ", " — download corrupted or tampered."
- calls: System.IO.File.OpenRead, System.Security.Cryptography.HashAlgorithm.ComputeHashAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Convert.ToHexString, System.String.Equals, System.IO.Path.GetFileName, System.String.Concat, System.InvalidOperationException..ctor, System.IO.Stream.DisposeAsync, System.Threading.Tasks.ValueTask.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter.GetResult, System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture, System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000281`  flags=`481`  rva=`47436`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

