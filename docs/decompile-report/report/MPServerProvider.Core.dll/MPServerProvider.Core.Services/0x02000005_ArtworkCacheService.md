# MPServerProvider.Core.Services.ArtworkCacheService

Token: `0x02000005`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_paths` : `class:16`  (flags=33, token=0x04000007)
- `_gate` : `class:101`  (flags=33, token=0x04000008)
- `ArtworkHttp` : `class:105`  (flags=49, token=0x04000009)

## Methods

### `.ctor(class:16)` -> `void`
- token `0x06000015`  flags=`6278`  rva=`9006`
- IL size 26 bytes, 10 instructions, max_stack=8
- opcodes: ldarg.0x3, stfldx2, callx1, ldarg.1x1, ldc.i4.4x1, newobjx1, retx1
- calls: System.Threading.SemaphoreSlim..ctor, System.Object..ctor

### `CreateArtworkClient()` -> `class:105`
- token `0x06000016`  flags=`145`  rva=`9036`
- IL size 67 bytes, 19 instructions, max_stack=4
- opcodes: callvirtx5, dupx4, ldstrx2, newobjx2, callx1, ldc.i4.1x1, ldc.i4.m1x1, ldc.r8x1, popx1, retx1
- string literals: "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0 Safari/537.36"
- calls: System.Net.Http.SocketsHttpHandler..ctor, System.Net.Http.SocketsHttpHandler.set_AllowAutoRedirect, System.Net.Http.SocketsHttpHandler.set_AutomaticDecompression, System.Net.Http.HttpClient..ctor, System.Net.Http.HttpClient.get_DefaultRequestHeaders, System.Net.Http.Headers.HttpHeaders.TryAddWithoutValidation, System.TimeSpan.FromSeconds, System.Net.Http.HttpClient.set_Timeout

### `GetLocalPathAsync(string, valuetype:121)` -> `class:117<string>`
- token `0x06000017`  flags=`134`  rva=`9116`
- IL size 71 bytes, 23 instructions, max_stack=2
- opcodes: ldloca.sx8, stfldx5, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `.cctor()` -> `void`
- token `0x06000018`  flags=`6289`  rva=`9199`
- IL size 11 bytes, 3 instructions, max_stack=8
- opcodes: callx1, retx1, stsfldx1
- calls: MPServerProvider.Core.Services.ArtworkCacheService.CreateArtworkClient

