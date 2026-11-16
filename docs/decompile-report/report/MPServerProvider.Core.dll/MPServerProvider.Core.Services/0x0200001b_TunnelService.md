# MPServerProvider.Core.Services.TunnelService

Token: `0x0200001b`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `PlayitReleasesApi` : `string`  (flags=32849, token=0x0400004d)
- `_paths` : `class:16`  (flags=33, token=0x0400004e)
- `_downloads` : `class:40`  (flags=33, token=0x0400004f)
- `_mappings` : `class:297<i4,class:433>`  (flags=33, token=0x04000050)
- `_discoveryGate` : `class:101`  (flags=33, token=0x04000051)
- `_device` : `class:437`  (flags=1, token=0x04000052)
- `<PlayitProcess>k__BackingField` : `class:169`  (flags=1, token=0x04000053)
- `DiscoveryTimeout` : `valuetype:89`  (flags=49, token=0x04000054)
- `DiscoveryAttempts` : `i4`  (flags=32849, token=0x04000055)

## Methods

### `get_PlayitProcess()` -> `class:169`
- token `0x060000f8`  flags=`2182`  rva=`32899`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_PlayitProcess(class:169)` -> `void`
- token `0x060000f9`  flags=`2177`  rva=`32907`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `.ctor(class:16, class:40)` -> `void`
- token `0x060000fa`  flags=`6278`  rva=`32916`
- IL size 45 bytes, 17 instructions, max_stack=8
- opcodes: ldarg.0x5, stfldx4, ldc.i4.1x2, newobjx2, callx1, ldarg.1x1, ldarg.2x1, retx1
- calls: System.Collections.Generic.Dictionary`2..ctor, System.Threading.SemaphoreSlim..ctor, System.Object..ctor

### `DiscoverRouterAsync()` -> `class:117<class:437>`
- token `0x060000fb`  flags=`129`  rva=`32964`
- IL size 55 bytes, 17 instructions, max_stack=2
- opcodes: ldloca.sx6, callx3, stfldx3, ldfldax2, ldarg.0x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `MapPortAsync(class:220, string)` -> `class:117<class:104>`
- token `0x060000fc`  flags=`134`  rva=`33032`
- IL size 71 bytes, 23 instructions, max_stack=2
- opcodes: ldloca.sx8, stfldx5, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `UnmapPortAsync(class:220)` -> `class:221`
- token `0x060000fd`  flags=`134`  rva=`33116`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `get_PlayitInstalled()` -> `bool`
- token `0x060000fe`  flags=`2182`  rva=`33191`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx2, ldarg.0x1, retx1
- calls: MPServerProvider.Core.Services.TunnelService.get_PlayitExePath, System.IO.File.Exists

### `get_PlayitExePath()` -> `string`
- token `0x060000ff`  flags=`2177`  rva=`33204`
- IL size 27 bytes, 7 instructions, max_stack=8
- opcodes: ldstrx2, callx1, callvirtx1, ldarg.0x1, ldfldx1, retx1
- string literals: "playit", "playit.exe"
- calls: MPServerProvider.Core.Services.AppPaths.get_ToolsDir, System.IO.Path.Combine

### `EnsurePlayitAsync(class:233<class:200>, valuetype:121)` -> `class:117<string>`
- token `0x06000100`  flags=`134`  rva=`33232`
- IL size 71 bytes, 23 instructions, max_stack=2
- opcodes: ldloca.sx8, stfldx5, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `StartPlayitAsync(class:193<string>, valuetype:121)` -> `class:221`
- token `0x06000101`  flags=`134`  rva=`33316`
- IL size 71 bytes, 23 instructions, max_stack=2
- opcodes: ldloca.sx8, stfldx5, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `StopPlayit()` -> `void`
- token `0x06000102`  flags=`134`  rva=`33400`
- IL size 43 bytes, 19 instructions, max_stack=2
- opcodes: callx3, ldarg.0x3, callvirtx2, ldloc.0x2, leave.sx2, brfalse.sx1, brtrue.sx1, ldc.i4.1x1, ldnullx1, popx1, retx1, stloc.0x1
- calls: MPServerProvider.Core.Services.TunnelService.get_PlayitProcess, System.Diagnostics.Process.get_HasExited, System.Diagnostics.Process.Kill, MPServerProvider.Core.Services.TunnelService.set_PlayitProcess

### `.cctor()` -> `void`
- token `0x06000103`  flags=`6289`  rva=`33472`
- IL size 20 bytes, 4 instructions, max_stack=8
- opcodes: callx1, ldc.r8x1, retx1, stsfldx1
- calls: System.TimeSpan.FromSeconds

