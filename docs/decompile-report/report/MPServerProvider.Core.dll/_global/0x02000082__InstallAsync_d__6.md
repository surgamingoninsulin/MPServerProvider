# <InstallAsync>d__6

Token: `0x02000082`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000299)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x0400029a)
- `progress` : `class:233<class:200>`  (flags=6, token=0x0400029b)
- `<>4__this` : `class:116`  (flags=6, token=0x0400029c)
- `ct` : `valuetype:121`  (flags=6, token=0x0400029d)
- `request` : `class:208`  (flags=6, token=0x0400029e)
- `<doc>5__2` : `class:341`  (flags=1, token=0x0400029f)
- `<version>5__3` : `string`  (flags=1, token=0x040002a0)
- `<zip>5__4` : `string`  (flags=1, token=0x040002a1)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x040002a2)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x040002a3)

## Methods

### `MoveNext()` -> `void`
- token `0x06000335`  flags=`481`  rva=`71460`
- IL size 831 bytes, 272 instructions, max_stack=8
- opcodes: ldarg.0x47, callx29, ldfldx24, callvirtx17, ldstrx17, stfldx17, ldloca.sx15, stloc.sx14, dupx9, ldloc.sx9, ldnullx8, ldfldax6, stloc.0x5, brtrue.sx4, ldc.i4.1x4, ldloc.0x4, newobjx4, brfalse.sx3, initobjx3, ldc.i4.sx3, beqx2, ldc.i4.0x2, ldc.i4.m1x2, ldloc.1x2, ldloc.3x2, ldsfldx2, leavex2, leave.sx2, popx2, bge.sx1, boxx1, endfinallyx1, ldftnx1, ldloc.2x1, nopx1, retx1, stloc.1x1, stloc.2x1, stloc.3x1, stsfldx1, throwx1
- string literals: "Resolving Bedrock server from Mojang…", "https://net-secondary.web.minecraft-services.net/api/v1.0/download/links", "result", "links", "downloadUrl", "Mojang's API returned no Windows Bedrock server download.", "bedrock-server-", "", "bedrock-server.zip", "Downloading Bedrock server ", "Extracting Bedrock server…", "server.properties", "server-port=19132", "server-port=", "server-name=Dedicated Server", "server-name=", "bedrock_server.exe"
- calls: MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, MPServerProvider.Core.Services.DownloadService.GetStringAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Text.Json.JsonDocument.Parse, System.Text.Json.JsonDocument.get_RootElement, System.Text.Json.JsonElement.GetProperty, System.Text.Json.JsonElement.EnumerateArray, <>c.<InstallAsync>b__6_0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault, System.Text.Json.JsonElement.GetString, System.InvalidOperationException..ctor, System.IO.Path.GetFileNameWithoutExtension, System.String.Replace, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.IO.Path.Combine, System.String.Concat, MPServerProvider.Core.Services.DownloadService.DownloadFileAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, MPServerProvider.Core.Services.DownloadService.ExtractZip, System.IO.File.Delete, System.IO.File.Exists, System.IO.File.ReadAllText, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, MPServerProvider.Core.Models.InstallRequest.get_Port, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallRequest.get_ServerName, System.IO.File.WriteAllText, MPServerProvider.Core.Models.InstallResult..ctor, MPServerProvider.Core.Models.InstallResult.set_ServerFile, MPServerProvider.Core.Models.InstallResult.set_Version, System.IDisposable.Dispose...

### `SetStateMachine(class:129)` -> `void`
- token `0x06000336`  flags=`481`  rva=`72356`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

