# <UnmapPortAsync>d__15

Token: `0x0200007b`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400026f)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x04000270)
- `<>4__this` : `class:108`  (flags=6, token=0x04000271)
- `server` : `class:220`  (flags=6, token=0x04000272)
- `<>7__wrap1` : `valuetype:445[]`  (flags=1, token=0x04000273)
- `<>7__wrap2` : `i4`  (flags=1, token=0x04000274)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x04000275)

## Methods

### `MoveNext()` -> `void`
- token `0x06000323`  flags=`481`  rva=`68828`
- IL size 297 bytes, 115 instructions, max_stack=5
- opcodes: ldarg.0x23, ldfldx12, stfldx9, callx6, leave.sx5, callvirtx4, ldfldax4, ldloca.sx4, brfalse.sx3, dupx3, ldc.i4.1x3, ldloc.1x3, stloc.0x3, stloc.sx3, brtrue.sx2, ldc.i4.0x2, ldc.i4.sx2, ldloc.0x2, ldloc.sx2, addx1, bltx1, brx1, conv.i4x1, initobjx1, ldc.i4.2x1, ldc.i4.m1x1, ldelem.i4x1, ldlenx1, ldloc.2x1, ldloc.3x1, ldnullx1, leavex1, newarrx1, nopx1, popx1, retx1, stelem.i4x1, stloc.1x1, stloc.2x1
- calls: MPServerProvider.Core.Models.ServerInstance.get_ServerPort, System.HashCode.Combine, System.Collections.Generic.Dictionary`2.Remove, Open.Nat.NatDevice.DeletePortMapAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000324`  flags=`481`  rva=`69168`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

