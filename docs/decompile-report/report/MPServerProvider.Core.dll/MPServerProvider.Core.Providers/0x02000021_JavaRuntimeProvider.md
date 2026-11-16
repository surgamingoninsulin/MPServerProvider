# MPServerProvider.Core.Providers.JavaRuntimeProvider

Token: `0x02000021`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_downloads` : `class:40`  (flags=33, token=0x04000060)
- `_paths` : `class:16`  (flags=33, token=0x04000061)
- `_resolved` : `class:297<i4,string>`  (flags=33, token=0x04000062)

## Methods

### `.ctor(class:40, class:16)` -> `void`
- token `0x06000123`  flags=`6278`  rva=`35368`
- IL size 32 bytes, 12 instructions, max_stack=8
- opcodes: ldarg.0x4, stfldx3, callx1, ldarg.1x1, ldarg.2x1, newobjx1, retx1
- calls: System.Collections.Generic.Dictionary`2..ctor, System.Object..ctor

### `ResolveAsync(i4, class:233<class:200>, valuetype:121)` -> `class:117<string>`
- token `0x06000124`  flags=`134`  rva=`35404`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `FindProvisioned(i4)` -> `string`
- token `0x06000125`  flags=`129`  rva=`35496`
- IL size 88 bytes, 28 instructions, max_stack=3
- opcodes: callx5, brtrue.sx2, dupx2, ldloc.0x2, ldsfldx2, ldstrx2, retx2, callvirtx1, ldarg.0x1, ldarga.sx1, ldc.i4.1x1, ldfldx1, ldftnx1, ldnullx1, newobjx1, popx1, stloc.0x1, stsfldx1
- string literals: "java", "java.exe"
- calls: MPServerProvider.Core.Services.AppPaths.get_ToolsDir, System.Int32.ToString, System.IO.Path.Combine, System.IO.Directory.Exists, System.IO.Directory.EnumerateFiles, <>c.<FindProvisioned>b__5_0, System.Func`2..ctor, System.Linq.Enumerable.FirstOrDefault

### `SystemJavaMatches(i4, string&)` -> `bool`
- token `0x06000126`  flags=`145`  rva=`35596`
- IL size 230 bytes, 91 instructions, max_stack=3
- opcodes: callvirtx20, ldc.i4.1x6, ldloc.0x6, callx5, ldc.i4.0x5, stloc.3x5, dupx4, ldloc.1x4, ldstrx4, leave.sx4, brtrue.sx3, ldarg.1x2, ldc.i4.2x2, ldloc.2x2, popx2, stind.refx2, beq.sx1, bne.un.sx1, br.sx1, brfalse.sx1, endfinallyx1, ldarg.0x1, ldc.i4x1, ldloc.3x1, ldnullx1, leavex1, newobjx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1
- string literals: "java", "-version", "version "(\d+)(?:\.(\d+))?", "java"
- calls: System.Diagnostics.ProcessStartInfo..ctor, System.Diagnostics.ProcessStartInfo.set_UseShellExecute, System.Diagnostics.ProcessStartInfo.set_RedirectStandardError, System.Diagnostics.ProcessStartInfo.set_RedirectStandardOutput, System.Diagnostics.ProcessStartInfo.set_CreateNoWindow, System.Diagnostics.Process.Start, System.Diagnostics.Process.get_StandardError, System.IO.TextReader.ReadToEnd, System.Diagnostics.Process.get_StandardOutput, System.String.Concat, System.Diagnostics.Process.WaitForExit, System.Text.RegularExpressions.Regex.Match, System.Text.RegularExpressions.Group.get_Success, System.Text.RegularExpressions.Match.get_Groups, System.Text.RegularExpressions.GroupCollection.get_Item, System.Text.RegularExpressions.Capture.get_Value, System.Int32.Parse, System.IDisposable.Dispose

### `ProvisionTemurinAsync(i4, class:233<class:200>, valuetype:121)` -> `class:117<string>`
- token `0x06000127`  flags=`129`  rva=`35868`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

