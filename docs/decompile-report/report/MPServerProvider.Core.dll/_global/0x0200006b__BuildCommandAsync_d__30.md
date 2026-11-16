# <BuildCommandAsync>d__30

Token: `0x0200006b`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000211)
- `<>t__builder` : `valuetype:145<valuetype:365<string,string,string>>`  (flags=6, token=0x04000212)
- `<>4__this` : `class:80`  (flags=6, token=0x04000213)
- `server` : `class:220`  (flags=6, token=0x04000214)
- `<root>5__2` : `string`  (flags=1, token=0x04000215)
- `<>7__wrap2` : `string`  (flags=1, token=0x04000216)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000217)

## Methods

### `MoveNext()` -> `void`
- token `0x060002fa`  flags=`481`  rva=`60448`
- IL size 704 bytes, 227 instructions, max_stack=4
- opcodes: ldarg.0x36, ldfldx26, callvirtx25, callx18, ldloc.sx14, stloc.sx13, ldstrx11, ldloca.sx10, stfldx10, brtrue.sx5, ldloc.3x5, dupx4, initobjx4, ldfldax4, ldnullx4, br.sx3, ldc.i4.sx3, ldloc.1x3, newobjx3, stloc.0x3, stloc.3x3, brfalsex2, brfalse.sx2, ldc.i4.0x2, leave.sx2, throwx2, bgt.sx1, ldc.i4.m1x1, ldloc.0x1, ldloc.2x1, ldsfldx1, leavex1, popx1, retx1, stloc.1x1, stloc.2x1
- string literals: "Server folder not found: ", "This server has no start_command configured.", "{java}", "{java}", "{server_root}", "{server_file}", "{server_ip}", "{server_port}", "{server_name}", "{ram_min_mb}", "{ram_max_mb}"
- calls: MPServerProvider.Core.Services.ServerRepository.RootFor, System.IO.Directory.Exists, System.String.Concat, System.InvalidOperationException..ctor, MPServerProvider.Core.Models.ServerInstance.get_StartCommand, System.String.IsNullOrWhiteSpace, System.String.Contains, MPServerProvider.Core.Models.ServerInstance.get_ServerGame, MPServerProvider.Core.Services.GameLibraryService.Find, MPServerProvider.Core.Models.ServerInstance.get_JavaMajor, MPServerProvider.Core.Models.GameDefinition.get_Runtime, MPServerProvider.Core.Models.RuntimeSpec.get_Requires, MPServerProvider.Core.Models.RequiresSpec.get_Java, System.Nullable`1.GetValueOrDefault, MPServerProvider.Core.Providers.JavaRuntimeProvider.ResolveAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.String.Replace, MPServerProvider.Core.Models.ServerInstance.get_ServerFile, System.IO.Path.Combine, MPServerProvider.Core.Models.ServerInstance.get_ServerIp, MPServerProvider.Core.Models.ServerInstance.get_ServerPort, System.Int32.ToString, MPServerProvider.Core.Models.ServerInstance.get_ServerName, MPServerProvider.Core.Models.ServerInstance.get_RamMinMb, MPServerProvider.Core.Models.ServerInstance.get_RamMaxMb, MPServerProvider.Core.Services.ServerProcessService.SplitCommandLine, System.IO.Path.IsPathRooted, System.ValueTuple`3..ctor, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x060002fb`  flags=`481`  rva=`61192`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

