# MPServerProvider.Core.Services.InstallService

Token: `0x0200000e`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_providers` : `class:297<string,class:128>`  (flags=33, token=0x04000021)
- `_servers` : `class:84`  (flags=33, token=0x04000022)
- `_paths` : `class:16`  (flags=33, token=0x04000023)
- `<JavaRuntime>k__BackingField` : `class:132`  (flags=33, token=0x04000024)
- `<Workshop>k__BackingField` : `class:112`  (flags=33, token=0x04000025)

## Methods

### `get_JavaRuntime()` -> `class:132`
- token `0x06000050`  flags=`2182`  rva=`19853`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `get_Workshop()` -> `class:112`
- token `0x06000051`  flags=`2182`  rva=`19861`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `.ctor(class:16, class:84, class:40)` -> `void`
- token `0x06000052`  flags=`6278`  rva=`19872`
- IL size 159 bytes, 69 instructions, max_stack=6
- opcodes: newobjx10, dupx9, ldarg.3x9, stelem.refx7, ldarg.0x6, stfldx5, ldarg.1x4, callx2, ldsfldx2, brtrue.sx1, ldarg.2x1, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.4x1, ldc.i4.5x1, ldc.i4.6x1, ldc.i4.7x1, ldftnx1, newarrx1, popx1, retx1, stsfldx1
- calls: System.Object..ctor, MPServerProvider.Core.Providers.JavaRuntimeProvider..ctor, MPServerProvider.Core.Services.WorkshopService..ctor, MPServerProvider.Core.Providers.MinecraftJavaProvider..ctor, MPServerProvider.Core.Providers.BedrockProvider..ctor, MPServerProvider.Core.Providers.CfxProvider..ctor, MPServerProvider.Core.Providers.BeamMpProvider..ctor, MPServerProvider.Core.Providers.SteamCmdProvider..ctor, MPServerProvider.Core.Providers.UrlProvider..ctor, MPServerProvider.Core.Providers.ManualProvider..ctor, <>c.<.ctor>b__9_0, System.Func`2..ctor, System.Linq.Enumerable.ToDictionary

### `SteamAppInfoAsync(i4, valuetype:121)` -> `class:117<class:144>`
- token `0x06000053`  flags=`134`  rva=`20043`
- IL size 30 bytes, 10 instructions, max_stack=8
- opcodes: callvirtx2, castclassx1, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldfldx1, ldnullx1, ldstrx1, retx1
- string literals: "steamcmd"
- calls: System.Collections.Generic.Dictionary`2.get_Item, MPServerProvider.Core.Providers.SteamCmdProvider.GetAppInfoAsync

### `ProviderFor(class:160)` -> `class:128`
- token `0x06000054`  flags=`134`  rva=`20076`
- IL size 45 bytes, 15 instructions, max_stack=3
- opcodes: callvirtx4, ldarg.0x2, ldfldx2, retx2, brtrue.sx1, ldarg.1x1, ldloc.0x1, ldloca.sx1, ldstrx1
- string literals: "url"
- calls: MPServerProvider.Core.Models.GameDefinition.get_Install, MPServerProvider.Core.Models.InstallSpec.get_Provider, System.Collections.Generic.Dictionary`2.TryGetValue, System.Collections.Generic.Dictionary`2.get_Item

### `GetVersionsAsync(class:160, string, valuetype:121)` -> `class:117<class:269<string>>`
- token `0x06000055`  flags=`134`  rva=`20133`
- IL size 16 bytes, 8 instructions, max_stack=8
- opcodes: ldarg.1x2, callx1, callvirtx1, ldarg.0x1, ldarg.2x1, ldarg.3x1, retx1
- calls: MPServerProvider.Core.Services.InstallService.ProviderFor, MPServerProvider.Core.Providers.IInstallProvider.GetVersionsAsync

### `InstallAsync(class:160, string, string, string, i4, i4, i4, string, bool, class:233<class:200>, valuetype:121)` -> `class:117<class:220>`
- token `0x06000056`  flags=`134`  rva=`20152`
- IL size 151 bytes, 50 instructions, max_stack=2
- opcodes: ldloca.sx17, stfldx14, ldarg.sx8, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `BuildInstance(class:160, class:208, class:212, string)` -> `class:220`
- token `0x06000057`  flags=`129`  rva=`20316`
- IL size 296 bytes, 95 instructions, max_stack=4
- opcodes: callvirtx39, dupx18, ldarg.2x8, ldarg.1x7, callx4, ldarg.3x4, ldstrx4, brtrue.sx2, ldarg.sx2, newobjx2, br.sx1, ldloc.0x1, popx1, retx1, stloc.0x1
- string literals: """, "" ", "0.0.0.0", "server.json"
- calls: MPServerProvider.Core.Models.InstallResult.get_StartCommand, MPServerProvider.Core.Models.GameDefinition.get_Runtime, MPServerProvider.Core.Models.RuntimeSpec.get_Executable, MPServerProvider.Core.Models.RuntimeSpec.get_Arguments, System.String.Concat, System.String.Trim, MPServerProvider.Core.Models.ServerInstance..ctor, MPServerProvider.Core.Models.GameDefinition.get_Id, MPServerProvider.Core.Models.ServerInstance.set_ServerGame, MPServerProvider.Core.Models.GameDefinition.get_Genres, System.Collections.Generic.List`1..ctor, MPServerProvider.Core.Models.ServerInstance.set_ServerGenres, MPServerProvider.Core.Models.InstallResult.get_Version, MPServerProvider.Core.Models.ServerInstance.set_ServerVersion, MPServerProvider.Core.Models.InstallRequest.get_Variant, MPServerProvider.Core.Models.ServerInstance.set_ServerType, MPServerProvider.Core.Models.InstallRequest.get_ServerName, MPServerProvider.Core.Models.ServerInstance.set_ServerName, MPServerProvider.Core.Models.InstallRequest.get_Description, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Models.GameDefinition.get_Name, MPServerProvider.Core.Models.ServerInstance.set_ServerDescription, MPServerProvider.Core.Models.ServerInstance.set_ServerRoot, MPServerProvider.Core.Models.InstallResult.get_ServerFile, MPServerProvider.Core.Models.ServerInstance.set_ServerFile, MPServerProvider.Core.Models.ServerInstance.set_ServerIp, MPServerProvider.Core.Models.InstallRequest.get_Port, MPServerProvider.Core.Models.ServerInstance.set_ServerPort, MPServerProvider.Core.Models.InstallRequest.get_RamMinMb, MPServerProvider.Core.Models.ServerInstance.set_RamMinMb, MPServerProvider.Core.Models.InstallRequest.get_RamMaxMb, MPServerProvider.Core.Models.ServerInstance.set_RamMaxMb, MPServerProvider.Core.Models.InstallResult.get_JavaMajor, MPServerProvider.Core.Models.ServerInstance.set_JavaMajor, MPServerProvider.Core.Models.ServerInstance.set_StartCommand, MPServerProvider.Core.Models.RuntimeSpec.get_StopCommand, MPServerProvider.Core.Models.ServerInstance.set_StopCommand, MPServerProvider.Core.Services.InstallService.ModsFolderFor, MPServerProvider.Core.Models.ServerInstance.set_ModsFolder, System.IO.Path.Combine...

### `ModsFolderFor(class:160, string)` -> `string`
- token `0x06000058`  flags=`145`  rva=`20624`
- IL size 169 bytes, 62 instructions, max_stack=2
- opcodes: ldstrx11, br.sx7, brtrue.sx7, callx7, stloc.0x7, ldarg.1x6, callvirtx5, ldnullx4, ldarg.0x3, retx3, brfalse.sx1, ldloc.0x1
- string literals: "minecraft-java", "paper", "purpur", "fabric", "forge", "velocity", "bungeecord", "plugins", "plugins", "mods", "mods"
- calls: MPServerProvider.Core.Models.GameDefinition.get_Mods, MPServerProvider.Core.Models.ModsSpec.get_Supported, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.op_Equality, MPServerProvider.Core.Models.ModsSpec.get_Folder

