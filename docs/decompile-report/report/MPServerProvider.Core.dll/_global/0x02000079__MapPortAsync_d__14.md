# <MapPortAsync>d__14

Token: `0x02000079`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x0400025c)
- `<>t__builder` : `valuetype:145<class:104>`  (flags=6, token=0x0400025d)
- `<>4__this` : `class:108`  (flags=6, token=0x0400025e)
- `protocol` : `string`  (flags=6, token=0x0400025f)
- `server` : `class:220`  (flags=6, token=0x04000260)
- `<>u__1` : `valuetype:137<class:437>`  (flags=1, token=0x04000261)
- `<>7__wrap1` : `valuetype:445[]`  (flags=1, token=0x04000262)
- `<>7__wrap2` : `i4`  (flags=1, token=0x04000263)
- `<proto>5__4` : `valuetype:445`  (flags=1, token=0x04000264)
- `<mapping>5__5` : `class:433`  (flags=1, token=0x04000265)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x04000266)
- `<>u__3` : `valuetype:137<class:429>`  (flags=1, token=0x04000267)

## Methods

### `MoveNext()` -> `void`
- token `0x0600031f`  flags=`481`  rva=`67480`
- IL size 815 bytes, 286 instructions, max_stack=6
- opcodes: ldarg.0x50, callx24, ldfldx24, stfldx20, ldloca.sx16, callvirtx14, stloc.sx14, ldloc.sx9, dupx8, ldc.i4.1x8, ldfldax8, ldstrx8, stloc.0x7, ldloc.1x6, brtrue.sx5, ldc.i4.0x5, ldnullx5, newobjx5, leave.sx4, br.sx3, initobjx3, ldc.i4.2x3, ldc.i4.m1x3, ldc.i4.sx3, leavex3, newarrx3, popx3, stloc.2x3, ldloc.0x2, ldloc.3x2, stelem.i4x2, addx1, bltx1, brx1, conv.i4x1, ldc.i4.3x1, ldelem.i4x1, ldlenx1, ldloc.2x1, nopx1, retx1, stloc.1x1, stloc.3x1, switchx1
- string literals: "tcp", "udp", "MP Server Provider — ", "Port ", " forwarded. Players can join at ", ":", "No UPnP router found. Enable UPnP on your router or use the playit.gg tunnel instead.", "UPnP mapping failed: "
- calls: MPServerProvider.Core.Services.TunnelService.DiscoverRouterAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.String.ToLowerInvariant, System.String.op_Equality, MPServerProvider.Core.Models.ServerInstance.get_ServerPort, MPServerProvider.Core.Models.ServerInstance.get_ServerName, System.String.Concat, Open.Nat.Mapping..ctor, Open.Nat.NatDevice.CreatePortMapAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.HashCode.Combine, System.Collections.Generic.Dictionary`2.set_Item, Open.Nat.NatDevice.GetExternalIPAsync, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Services.UpnpResult..ctor, System.Exception.get_Message, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000320`  flags=`481`  rva=`68384`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

