# <ConnectRconAsync>d__22

Token: `0x0200006c`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000218)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x04000219)
- `running` : `class:400`  (flags=6, token=0x0400021a)
- `game` : `class:160`  (flags=6, token=0x0400021b)
- `<>4__this` : `class:80`  (flags=6, token=0x0400021c)
- `server` : `class:220`  (flags=6, token=0x0400021d)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x0400021e)

## Methods

### `MoveNext()` -> `void`
- token `0x060002fc`  flags=`481`  rva=`61208`
- IL size 478 bytes, 173 instructions, max_stack=5
- opcodes: ldarg.0x27, callx16, ldfldx15, callvirtx8, ldloc.1x8, stfldx8, dupx7, ldloca.sx7, ldstrx7, brtrue.sx6, ldfldax6, popx6, stloc.0x5, leave.sx4, stloc.3x4, br.sx3, initobjx3, ldc.i4.1x3, ldloc.0x3, ldloc.sx3, leavex3, brfalse.sx2, ldc.i4.m1x2, ldc.i4.sx2, ldloc.3x2, stloc.sx2, beqx1, beq.sx1, ldc.i4x1, ldc.i4.0x1, ldc.i4.2x1, ldloc.2x1, ldnullx1, nopx1, retx1, stloc.1x1, stloc.2x1
- string literals: "source", "", "", "127.0.0.1", "[MP Server Provider] RCON connected.", "[MP Server Provider] RCON connection failed.", "[MP Server Provider] RCON error: "
- calls: System.Threading.Tasks.Task.Delay, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, RunningServer.get_State, MPServerProvider.Core.Models.GameDefinition.get_Network, MPServerProvider.Core.Models.NetworkSpec.get_Query, System.String.op_Equality, MPServerProvider.Core.Models.ServerInstance.get_ServerPort, MPServerProvider.Core.Services.RconService.ConnectAsync, MPServerProvider.Core.Services.RconService.get_Connected, MPServerProvider.Core.Services.ServerProcessService.AppendLine, MPServerProvider.Core.Services.ServerProcessService.PollRconAsync, System.Exception.get_Message, System.String.Concat, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002fd`  flags=`481`  rva=`61752`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

