# <GetLocalPathAsync>d__5

Token: `0x0200003e`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000103)
- `<>t__builder` : `valuetype:145<string>`  (flags=6, token=0x04000104)
- `url` : `string`  (flags=6, token=0x04000105)
- `<>4__this` : `class:20`  (flags=6, token=0x04000106)
- `ct` : `valuetype:121`  (flags=6, token=0x04000107)
- `<cached>5__2` : `string`  (flags=1, token=0x04000108)
- `<>u__1` : `valuetype:133`  (flags=1, token=0x04000109)
- `<>u__2` : `valuetype:137<u1[]>`  (flags=1, token=0x0400010a)

## Methods

### `MoveNext()` -> `void`
- token `0x06000275`  flags=`481`  rva=`43544`
- IL size 832 bytes, 290 instructions, max_stack=4
- opcodes: ldarg.0x52, callx33, ldfldx31, stfldx14, callvirtx12, stloc.sx10, ldloc.sx9, ldloca.sx9, leavex9, ldfldax8, stloc.2x8, brtrue.sx7, ldnullx7, stloc.0x7, dupx6, brfalse.sx5, ldc.i4.1x5, ldloc.0x5, ldstrx5, ldloc.1x4, br.sx3, initobjx3, ldc.i4.5x3, ldc.i4.m1x3, ldloc.3x3, leave.sx3, ldc.i4.0x2, ldc.i4.2x2, ldc.i4.sx2, newobjx2, popx2, stloc.3x2, beqx1, beq.sx1, bge.sx1, ble.sx1, ble.unx1, brfalsex1, brtruex1, endfinallyx1, ldloc.2x1, ldsfldx1, nopx1, retx1, stloc.1x1, subx1
- string literals: "http", "file:", ".img", ".tmp", ".tmp"
- calls: System.String.IsNullOrWhiteSpace, System.String.StartsWith, System.Uri..ctor, System.Uri.get_LocalPath, System.IO.File.Exists, System.IO.Path.IsPathRooted, MPServerProvider.Core.Services.AppPaths.get_DataRoot, System.IO.Path.Combine, System.IO.Path.GetFullPath, System.IO.Path.GetExtension, System.String.IsNullOrEmpty, System.String.get_Length, MPServerProvider.Core.Services.AppPaths.get_ArtworkCacheDir, System.Text.Encoding.get_UTF8, System.Text.Encoding.GetBytes, System.Security.Cryptography.MD5.HashData, System.Convert.ToHexString, System.String.Concat, System.Threading.SemaphoreSlim.WaitAsync, System.Threading.Tasks.Task.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter.GetResult, System.Net.Http.HttpClient.GetByteArrayAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.IO.File.WriteAllBytesAsync, System.IO.File.Move, System.Threading.SemaphoreSlim.Release, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetException, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetResult

### `SetStateMachine(class:129)` -> `void`
- token `0x06000276`  flags=`481`  rva=`44464`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

