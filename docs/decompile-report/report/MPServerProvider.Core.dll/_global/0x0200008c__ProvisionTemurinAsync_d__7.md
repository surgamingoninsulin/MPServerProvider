# <ProvisionTemurinAsync>d__7

Token: `0x0200008c`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x040002d5)
- `<>t__builder` : `valuetype:145<string>`  (flags=6, token=0x040002d6)
- `progress` : `class:233<class:200>`  (flags=6, token=0x040002d7)
- `major` : `i4`  (flags=6, token=0x040002d8)
- `<>4__this` : `class:132`  (flags=6, token=0x040002d9)
- `ct` : `valuetype:121`  (flags=6, token=0x040002da)
- `<doc>5__2` : `class:341`  (flags=1, token=0x040002db)
- `<dir>5__3` : `string`  (flags=1, token=0x040002dc)
- `<zip>5__4` : `string`  (flags=1, token=0x040002dd)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x040002de)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x040002df)

## Methods

### `MoveNext()` -> `void`
- token `0x0600034a`  flags=`481`  rva=`75700`
- IL size 857 bytes, 289 instructions, max_stack=8
- opcodes: ldarg.0x48, callx39, ldloca.sx29, ldfldx25, stloc.sx18, stfldx17, ldstrx14, callvirtx8, ldc.i4.1x7, ldfldax7, ldloc.sx7, ldnullx7, dupx6, ldc.i4.sx6, newobjx5, stloc.0x5, brtrue.sx4, ldloc.0x4, ldloc.1x4, initobjx3, beqx2, ldc.i4.0x2, ldc.i4.m1x2, leavex2, leave.sx2, popx2, bge.sx1, boxx1, br.sx1, brfalsex1, brfalse.sx1, endfinallyx1, ldloc.2x1, ldloc.3x1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, throwx1
- string literals: "Resolving Temurin JRE ", " from Adoptium…", "https://api.adoptium.net/v3/assets/latest/", "/hotspot?os=windows&architecture=x64&image_type=jre", "binary", "package", "link", "checksum", "java", "temurin.zip", "Downloading Temurin JRE ", "Extracting Java runtime…", "Temurin JRE ", " was downloaded but java.exe was not found."
- calls: System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.EnumerateArray, System.Linq.Enumerable.First, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.GetString, MPServerProvider.Core.Services.AppPaths.get_ToolsDir, System.Int32.ToString, System.IO.Path.Combine, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Services.DownloadService.ExtractZip, System.IO.File.Delete, MPServerProvider.Core.Providers.JavaRuntimeProvider.FindProvisioned, System.InvalidOperationException..ctor, System.IDisposable.Dispose, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x0600034b`  flags=`481`  rva=`76624`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

