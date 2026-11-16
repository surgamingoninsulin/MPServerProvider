# MPServerProvider.Core.Providers.MinecraftJavaProvider

Token: `0x02000023`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `MojangManifest` : `string`  (flags=32849, token=0x04000064)
- `PaperApi` : `string`  (flags=32849, token=0x04000065)
- `FabricMeta` : `string`  (flags=32849, token=0x04000066)
- `PurpurApi` : `string`  (flags=32849, token=0x04000067)
- `VelocityApi` : `string`  (flags=32849, token=0x04000068)
- `ForgePromos` : `string`  (flags=32849, token=0x04000069)
- `ForgeMavenMeta` : `string`  (flags=32849, token=0x0400006a)
- `BungeeCordUrl` : `string`  (flags=32849, token=0x0400006b)
- `_downloads` : `class:40`  (flags=33, token=0x0400006c)

## Methods

### `.ctor(class:40)` -> `void`
- token `0x0600012c`  flags=`6278`  rva=`36087`
- IL size 14 bytes, 6 instructions, max_stack=8
- opcodes: ldarg.0x2, callx1, ldarg.1x1, retx1, stfldx1
- calls: System.Object..ctor

### `get_Id()` -> `string`
- token `0x0600012d`  flags=`2534`  rva=`36102`
- IL size 6 bytes, 2 instructions, max_stack=8
- opcodes: ldstrx1, retx1
- string literals: "minecraft-java"

### `GetVersionsAsync(class:160, string, valuetype:121)` -> `class:117<class:269<string>>`
- token `0x0600012e`  flags=`486`  rva=`36112`
- IL size 71 bytes, 23 instructions, max_stack=2
- opcodes: ldloca.sx8, stfldx5, callx3, ldfldax2, ldarg.0x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `InstallAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x0600012f`  flags=`486`  rva=`36196`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `InstallVanillaAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x06000130`  flags=`129`  rva=`36288`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `InstallPaperAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x06000131`  flags=`129`  rva=`36380`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `InstallPurpurAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x06000132`  flags=`129`  rva=`36472`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `InstallVelocityAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x06000133`  flags=`129`  rva=`36564`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `InstallBungeeCordAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x06000134`  flags=`129`  rva=`36656`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `InstallForgeAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x06000135`  flags=`129`  rva=`36748`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `GuessJavaMajor(string)` -> `i4`
- token `0x06000136`  flags=`150`  rva=`36840`
- IL size 153 bytes, 82 instructions, max_stack=4
- opcodes: ldc.i4.sx10, br.sx7, ldloc.sx6, stloc.sx6, ldloc.0x5, callx4, blt.sx3, ldc.i4.2x3, ldelem.refx3, ldloca.sx3, retx3, beq.sx2, ble.sx2, brtrue.sx2, conv.i4x2, ldc.i4.0x2, ldc.i4.1x2, ldlenx2, bge.sx1, brfalse.sx1, callvirtx1, dupx1, ldarg.0x1, ldc.i4.3x1, ldc.i4.5x1, ldc.i4.8x1, ldloc.1x1, ldloc.2x1, ldloc.3x1, ldtokenx1, newarrx1, stloc.0x1, stloc.3x1
- calls: System.Runtime.CompilerServices.RuntimeHelpers.InitializeArray, System.String.Split, System.Int32.TryParse

### `InstallFabricAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x06000137`  flags=`129`  rva=`37008`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `WriteCommonFiles(class:208)` -> `void`
- token `0x06000138`  flags=`145`  rva=`37100`
- IL size 145 bytes, 43 instructions, max_stack=4
- opcodes: callx12, ldloca.sx7, ldstrx6, callvirtx5, ldarg.0x5, ldloc.0x2, brfalse.sx1, brtrue.sx1, ldc.i4.2x1, ldc.i4.sx1, retx1, stloc.0x1
- string literals: "eula.txt", "# Accepted via MP Server Provider install dialog (https://aka.ms/MinecraftEULA)
eula=true
", "server.properties", "server-port=", "
motd=", "
"
- calls: MPServerProvider.Core.Models.InstallRequest.get_EulaAccepted, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, System.IO.File.WriteAllText, System.IO.File.Exists, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, MPServerProvider.Core.Models.InstallRequest.get_Port, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, MPServerProvider.Core.Models.InstallRequest.get_ServerName, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear

