# MPServerProvider.Core.Services.DownloadService

Token: `0x0200000a`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `Http` : `class:105`  (flags=54, token=0x0400001b)

## Methods

### `CreateClient()` -> `class:105`
- token `0x06000030`  flags=`145`  rva=`14984`
- IL size 76 bytes, 20 instructions, max_stack=4
- opcodes: callvirtx6, dupx4, newobjx3, ldstrx2, callx1, ldc.i4.1x1, ldc.i4.m1x1, ldc.r8x1, retx1
- string literals: "MPServerProvider", "1.0"
- calls: System.Net.Http.SocketsHttpHandler..ctor, System.Net.Http.SocketsHttpHandler.set_AllowAutoRedirect, System.Net.Http.SocketsHttpHandler.set_AutomaticDecompression, System.Net.Http.HttpClient..ctor, System.Net.Http.HttpClient.get_DefaultRequestHeaders, System.Net.Http.Headers.HttpRequestHeaders.get_UserAgent, System.Net.Http.Headers.ProductInfoHeaderValue..ctor, System.Net.Http.Headers.HttpHeaderValueCollection`1.Add, System.TimeSpan.FromMinutes, System.Net.Http.HttpClient.set_Timeout

### `GetStringAsync(string, valuetype:121)` -> `class:117<string>`
- token `0x06000031`  flags=`134`  rva=`15072`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `GetJsonAsync(string, valuetype:121)` -> `class:117<mvar:0>`
- token `0x06000032`  flags=`134`  rva=`15148`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `DownloadFileAsync(string, string, class:233<class:200>, string, string, string, valuetype:121)` -> `class:221`
- token `0x06000033`  flags=`134`  rva=`15224`
- IL size 107 bytes, 35 instructions, max_stack=2
- opcodes: ldloca.sx12, stfldx9, ldarg.sx4, callx3, ldfldax2, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `ExtractZip(string, string, class:233<class:200>, string)` -> `void`
- token `0x06000034`  flags=`150`  rva=`15343`
- IL size 31 bytes, 14 instructions, max_stack=8
- opcodes: callx3, ldarg.1x2, ldarg.2x2, brfalse.sx1, callvirtx1, ldarg.0x1, ldarg.3x1, ldc.i4.1x1, popx1, retx1
- calls: MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, System.IO.Directory.CreateDirectory, System.IO.Compression.ZipFile.ExtractToDirectory

### `FlattenSingleRootFolder(string)` -> `void`
- token `0x06000035`  flags=`150`  rva=`15376`
- IL size 105 bytes, 55 instructions, max_stack=2
- opcodes: callx9, ldloc.sx6, ldc.i4.0x3, ldelem.refx3, ldloc.0x3, ldloc.3x3, br.sx2, conv.i4x2, ldarg.0x2, ldc.i4.1x2, ldlenx2, ldloc.1x2, ldloc.2x2, retx2, stloc.3x2, stloc.sx2, addx1, blt.sx1, bne.un.sx1, brfalse.sx1, brtrue.sx1, stloc.0x1, stloc.1x1, stloc.2x1
- calls: System.IO.Directory.GetFileSystemEntries, System.IO.Directory.Exists, System.IO.Path.GetFileName, System.IO.Path.Combine, System.IO.Directory.Move, System.IO.File.Move, System.IO.Directory.Delete

### `VerifyHashAsync(string, string, class:237, valuetype:121)` -> `class:221`
- token `0x06000036`  flags=`145`  rva=`15496`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `FormatBytes(i8)` -> `string`
- token `0x06000037`  flags=`150`  rva=`15588`
- IL size 241 bytes, 83 instructions, max_stack=3
- opcodes: callx16, ldloca.sx16, ldarg.0x7, ldstrx7, ldc.i4.1x4, stloc.0x4, br.sx3, conv.i8x3, conv.r8x3, divx3, ldc.i4x3, ldc.i4.3x3, ldc.r8x3, bge.sx2, brx2, blt.sx1, ldc.i4.2x1, ldloc.0x1, retx1
- string literals: "0.0", " GB", "0.0", " MB", "0.0", " KB", " B"
- calls: System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear

### `.ctor()` -> `void`
- token `0x06000038`  flags=`6278`  rva=`15841`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `.cctor()` -> `void`
- token `0x06000039`  flags=`6289`  rva=`15849`
- IL size 11 bytes, 3 instructions, max_stack=8
- opcodes: callx1, retx1, stsfldx1
- calls: MPServerProvider.Core.Services.DownloadService.CreateClient

