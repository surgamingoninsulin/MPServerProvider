# <DownloadFileAsync>d__4

Token: `0x02000043`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000123)
- `<>t__builder` : `valuetype:225`  (flags=6, token=0x04000124)
- `destination` : `string`  (flags=6, token=0x04000125)
- `url` : `string`  (flags=6, token=0x04000126)
- `ct` : `valuetype:121`  (flags=6, token=0x04000127)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000128)
- `statusLabel` : `string`  (flags=6, token=0x04000129)
- `sha1` : `string`  (flags=6, token=0x0400012a)
- `sha256` : `string`  (flags=6, token=0x0400012b)
- `<response>5__2` : `class:241`  (flags=1, token=0x0400012c)
- `<total>5__3` : `valuetype:261<i8>`  (flags=1, token=0x0400012d)
- `<>u__1` : `valuetype:137<class:241>`  (flags=1, token=0x0400012e)
- `<source>5__4` : `class:209`  (flags=1, token=0x0400012f)
- `<>u__2` : `valuetype:137<class:209>`  (flags=1, token=0x04000130)
- `<>7__wrap4` : `object`  (flags=1, token=0x04000131)
- `<>7__wrap5` : `i4`  (flags=1, token=0x04000132)
- `<target>5__7` : `class:265`  (flags=1, token=0x04000133)
- `<>7__wrap7` : `object`  (flags=1, token=0x04000134)
- `<>7__wrap8` : `i4`  (flags=1, token=0x04000135)
- `<buffer>5__10` : `u1[]`  (flags=1, token=0x04000136)
- `<received>5__11` : `i8`  (flags=1, token=0x04000137)
- `<read>5__12` : `i4`  (flags=1, token=0x04000138)
- `<>u__3` : `valuetype:245`  (flags=1, token=0x04000139)
- `<>u__4` : `valuetype:253<i4>`  (flags=1, token=0x0400013a)
- `<>u__5` : `valuetype:133`  (flags=1, token=0x0400013b)

## Methods

### `MoveNext()` -> `void`
- token `0x0600027a`  flags=`481`  rva=`44640`
- IL size 1682 bytes, 592 instructions, max_stack=6
- opcodes: ldarg.0x125, callx57, ldfldx48, stfldx45, ldloca.sx34, stloc.sx26, dupx24, ldfldax24, callvirtx23, ldloc.sx18, stloc.0x17, brtrue.sx11, brfalse.sx10, ldc.i4.0x9, initobjx8, ldc.i4.m1x8, ldloc.0x8, ldnullx7, leavex7, leave.sx7, ldc.i4.1x5, conv.i8x4, ldc.i4.3x4, ldstrx4, beqx3, ldc.i4.2x3, nopx3, popx3, subx3, bgt.sx2, brx2, br.sx2, conv.r8x2, isinstx2, ldc.i4.4x2, ldc.i4.6x2, ldc.i4.sx2, ldc.r8x2, newobjx2, stloc.2x2, throwx2, addx1, bge.sx1, bgtx1, ble.unx1, ble.un.sx1, divx1, endfinallyx1, ldc.i4x1, ldc.i4.5x1, ldc.i4.7x1, ldloc.1x1, ldloc.2x1, ldloc.3x1, ldsfldx1, mulx1, newarrx1, retx1, stloc.1x1, stloc.3x1, switchx1
- string literals: " (", "", " / ", ")"
- calls: System.IO.Path.GetDirectoryName, System.IO.Directory.CreateDirectory, System.Net.Http.HttpClient.GetAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode, System.Net.Http.HttpResponseMessage.get_Content, System.Net.Http.HttpContent.get_Headers, System.Net.Http.Headers.HttpContentHeaders.get_ContentLength, System.Net.Http.HttpContent.ReadAsStreamAsync, System.IO.File.Create, System.MemoryExtensions.AsMemory, System.Memory`1.op_Implicit, System.IO.Stream.WriteAsync, System.Threading.Tasks.ValueTask.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter.GetResult, MPServerProvider.Core.Models.InstallProgress..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, MPServerProvider.Core.Services.DownloadService.FormatBytes, System.Nullable`1.get_HasValue, System.Nullable`1.GetValueOrDefault, System.Nullable`1.get_Value, System.String.Concat, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallProgress.set_Status, MPServerProvider.Core.Models.InstallProgress.set_Percent, MPServerProvider.Core.Models.InstallProgress.set_BytesReceived, MPServerProvider.Core.Models.InstallProgress.set_TotalBytes, System.IProgress`1.Report, System.IO.Stream.ReadAsync, System.Threading.Tasks.ValueTask`1.GetAwaiter, System.Runtime.CompilerServices.ValueTaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult, System.IO.Stream.DisposeAsync, System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture...

### `SetStateMachine(class:129)` -> `void`
- token `0x0600027b`  flags=`481`  rva=`46436`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.SetStateMachine

