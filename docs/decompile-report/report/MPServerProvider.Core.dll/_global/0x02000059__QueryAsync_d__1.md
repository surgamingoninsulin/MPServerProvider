# <QueryAsync>d__1

Token: `0x02000059`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400019c)
- `<>t__builder` : `valuetype:145<valuetype:261<valuetype:305<i4,i4>>>`  (flags=6, token=0x0400019d)
- `protocol` : `string`  (flags=6, token=0x0400019e)
- `host` : `string`  (flags=6, token=0x0400019f)
- `port` : `i4`  (flags=6, token=0x040001a0)
- `<>u__1` : `valuetype:137<valuetype:261<valuetype:305<i4,i4>>>`  (flags=1, token=0x040001a1)

## Methods

### `MoveNext()` -> `void`
- token `0x060002ae`  flags=`481`  rva=`51336`
- IL size 614 bytes, 214 instructions, max_stack=3
- opcodes: ldarg.0x42, callx22, ldfldx14, ldloca.sx14, stfldx14, ldfldax10, stloc.0x9, stloc.sx9, dupx8, initobjx6, ldloc.sx6, brtrue.sx5, stloc.2x5, callvirtx4, ldc.i4.m1x4, ldloc.3x4, ldstrx4, leave.sx4, brx3, brtruex3, leavex3, popx3, br.sx2, ldc.i4.3x2, ldc.i4.sx2, ldloc.0x2, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.2x1, ldloc.1x1, ldloc.2x1, nopx1, retx1, stloc.1x1, stloc.3x1, switchx1
- string literals: "minecraft-java", "minecraft-bedrock", "fivem", "a2s"
- calls: System.String.op_Equality, MPServerProvider.Core.Services.QueryService.QueryMinecraftJavaAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, MPServerProvider.Core.Services.QueryService.QueryBedrockAsync, MPServerProvider.Core.Services.QueryService.QueryFiveMAsync, MPServerProvider.Core.Services.QueryService.QueryA2SAsync, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002af`  flags=`481`  rva=`52016`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

