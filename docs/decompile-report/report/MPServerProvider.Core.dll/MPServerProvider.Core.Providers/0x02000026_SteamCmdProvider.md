# MPServerProvider.Core.Providers.SteamCmdProvider

Token: `0x02000026`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `SteamCmdZip` : `string`  (flags=32849, token=0x04000074)
- `_downloads` : `class:40`  (flags=33, token=0x04000075)
- `_paths` : `class:16`  (flags=33, token=0x04000076)
- `_workshop` : `class:112`  (flags=33, token=0x04000077)
- `ProgressLine` : `class:57`  (flags=49, token=0x04000078)
- `ExeBlocklist` : `string[]`  (flags=49, token=0x04000079)

## Methods

### `.ctor(class:40, class:16)` -> `void`
- token `0x0600014d`  flags=`6278`  rva=`38413`
- IL size 34 bytes, 14 instructions, max_stack=8
- opcodes: ldarg.0x4, stfldx3, ldarg.1x2, ldarg.2x2, callx1, newobjx1, retx1
- calls: System.Object..ctor, MPServerProvider.Core.Services.WorkshopService..ctor

### `get_Id()` -> `string`
- token `0x0600014e`  flags=`2534`  rva=`38448`
- IL size 6 bytes, 2 instructions, max_stack=8
- opcodes: ldstrx1, retx1
- string literals: "steamcmd"

### `GetVersionsAsync(class:160, string, valuetype:121)` -> `class:117<class:269<string>>`
- token `0x0600014f`  flags=`486`  rva=`38455`
- IL size 20 bytes, 8 instructions, max_stack=8
- opcodes: callx1, dupx1, ldc.i4.0x1, ldc.i4.1x1, ldstrx1, newarrx1, retx1, stelem.refx1
- string literals: "latest"
- calls: System.Threading.Tasks.Task.FromResult

### `GetAppInfoAsync(i4, class:233<class:200>, valuetype:121)` -> `class:117<class:144>`
- token `0x06000150`  flags=`134`  rva=`38476`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `InstallAsync(class:208, class:233<class:200>, valuetype:121)` -> `class:117<class:212>`
- token `0x06000151`  flags=`486`  rva=`38568`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `ParseProgress(string)` -> `valuetype:261<valuetype:628>`
- token `0x06000152`  flags=`150`  rva=`38660`
- IL size 183 bytes, 44 instructions, max_stack=5
- opcodes: callvirtx15, callx7, ldloc.0x5, ldstrx4, ldc.r8x2, newobjx2, retx2, brtrue.sx1, initobjx1, ldarg.0x1, ldloc.1x1, ldloca.sx1, ldsfldx1, stloc.0x1
- string literals: "pct", "state", "done", "total"
- calls: System.Text.RegularExpressions.Regex.Match, System.Text.RegularExpressions.Group.get_Success, System.Text.RegularExpressions.Match.get_Groups, System.Text.RegularExpressions.GroupCollection.get_Item, System.Text.RegularExpressions.Capture.get_Value, System.Globalization.CultureInfo.get_InvariantCulture, System.Double.Parse, System.Math.Clamp, System.String.Trim, System.Int64.Parse, SteamProgress..ctor, System.Nullable`1..ctor

### `Megabytes(i8)` -> `string`
- token `0x06000153`  flags=`145`  rva=`38856`
- IL size 36 bytes, 11 instructions, max_stack=2
- opcodes: divx2, ldc.r8x2, callx1, conv.r8x1, ldarg.0x1, ldloca.sx1, ldstrx1, retx1, stloc.0x1
- string literals: "0"
- calls: System.Double.ToString

### `LoginArguments(class:168)` -> `string`
- token `0x06000154`  flags=`150`  rva=`38904`
- IL size 144 bytes, 45 instructions, max_stack=3
- opcodes: callx11, ldloca.sx7, ldstrx6, callvirtx5, ldarg.0x5, retx3, brtrue.sx2, ldloc.0x2, brfalse.sx1, ldc.i4.2x1, ldc.i4.sx1, stloc.0x1
- string literals: "+login anonymous", "+login "", "" "", """, "+login "", """
- calls: MPServerProvider.Core.Models.InstallSpec.get_Anonymous, MPServerProvider.Core.Models.InstallSpec.get_Username, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Models.InstallSpec.get_Password, MPServerProvider.Core.Services.SecretProtector.Unprotect, System.String.IsNullOrEmpty, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.String.Concat

### `DefinitionNamesExecutable(string)` -> `bool`
- token `0x06000155`  flags=`145`  rva=`39060`
- IL size 25 bytes, 11 instructions, max_stack=8
- opcodes: ldarg.0x2, ldc.i4.0x2, retx2, brtrue.sx1, callx1, callvirtx1, ceqx1, ldstrx1
- string literals: "{server_file}"
- calls: System.String.IsNullOrWhiteSpace, System.String.Contains

### `ExeFromDefinition(class:208)` -> `string`
- token `0x06000156`  flags=`145`  rva=`39088`
- IL size 98 bytes, 34 instructions, max_stack=3
- opcodes: callvirtx10, callx4, ldarg.0x4, ldloc.0x4, brtrue.sx3, retx3, ldnullx2, stloc.0x2, ldc.i4.sx1, ldstrx1
- string literals: "{server_root}"
- calls: MPServerProvider.Core.Models.InstallRequest.get_Game, MPServerProvider.Core.Models.GameDefinition.get_Runtime, MPServerProvider.Core.Models.RuntimeSpec.get_Executable, MPServerProvider.Core.Providers.SteamCmdProvider.DefinitionNamesExecutable, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, System.String.Replace, System.String.Trim, System.IO.Path.IsPathRooted, System.IO.Path.Combine, System.IO.File.Exists

### `AutoDetectServerExe(string)` -> `string`
- token `0x06000157`  flags=`150`  rva=`39200`
- IL size 388 bytes, 135 instructions, max_stack=4
- opcodes: ldloc.sx23, callvirtx11, stloc.sx11, callx9, ldstrx6, newobjx5, addx4, brfalse.sx4, brtruex4, ldc.i4.sx4, ldsfldx4, leave.sx4, ldfldx3, ldloca.sx3, popx3, conv.i8x2, dupx2, ldc.i4x2, ldc.i4.1x2, ldftnx2, ldnullx2, retx2, stloc.0x2, stloc.1x2, ble.sx1, brx1, brtrue.sx1, conv.i4x1, divx1, endfinallyx1, ldarg.0x1, ldc.i4.0x1, ldc.i4.6x1, ldloc.0x1, ldloc.1x1, ldloc.2x1, ldloc.3x1, leavex1, stfldx1, stloc.2x1, stloc.3x1, stsfldx1, subx1
- string literals: "*.exe", "_commonredist", "redist", "server", "srcds", "dedicated"
- calls: System.IO.Directory.EnumerateFiles, System.Collections.Generic.IEnumerable`1.GetEnumerator, System.Collections.Generic.IEnumerator`1.get_Current, <>c__DisplayClass18_0..ctor, System.IO.Path.GetFileNameWithoutExtension, System.String.ToLowerInvariant, <>c__DisplayClass18_0.<AutoDetectServerExe>b__0, System.Func`2..ctor, System.Linq.Enumerable.Any, System.String.Contains, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, <>c.<AutoDetectServerExe>b__18_1, System.Linq.Enumerable.Count, System.IO.FileInfo..ctor, System.IO.FileInfo.get_Length, System.Math.Min, System.Collections.IEnumerator.MoveNext, System.IDisposable.Dispose

### `EnsureSteamCmdAsync(class:233<class:200>, valuetype:121)` -> `class:117<string>`
- token `0x06000158`  flags=`129`  rva=`39676`
- IL size 71 bytes, 23 instructions, max_stack=2
- opcodes: ldloca.sx8, stfldx5, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `FindOnPath(string)` -> `string`
- token `0x06000159`  flags=`145`  rva=`39760`
- IL size 88 bytes, 47 instructions, max_stack=3
- opcodes: callx5, ldloc.1x3, leave.sx3, popx3, br.sx2, brtrue.sx2, dupx2, ldc.i4.0x2, ldloc.0x2, ldloc.2x2, ldnullx2, retx2, stloc.1x2, addx1, blt.sx1, brfalse.sx1, callvirtx1, conv.i4x1, ldarg.0x1, ldc.i4.1x1, ldelem.refx1, ldlenx1, ldloc.3x1, ldsfldx1, ldstrx1, stloc.0x1, stloc.2x1, stloc.3x1
- string literals: "PATH"
- calls: System.Environment.GetEnvironmentVariable, System.String.Split, System.Array.Empty, System.String.Trim, System.IO.Path.Combine, System.IO.File.Exists

### `.cctor()` -> `void`
- token `0x0600015a`  flags=`6289`  rva=`39876`
- IL size 114 bytes, 48 instructions, max_stack=4
- opcodes: ldstrx11, dupx10, stelem.refx10, ldc.i4.sx2, stsfldx2, ldc.i4x1, ldc.i4.0x1, ldc.i4.1x1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.4x1, ldc.i4.5x1, ldc.i4.6x1, ldc.i4.7x1, ldc.i4.8x1, newarrx1, newobjx1, retx1
- string literals: "Update state \(0x[0-9a-fA-F]+\)\s*(?<state>[\w ]+?),\s*progress:\s*(?<pct>[0-9.]+)\s*\(\s*(?<done>\d+)\s*/\s*(?<total>\d+)\s*\)", "steamerrorreporter", "steamservice", "crashhandler", "unitycrashhandler", "vcredist", "dxsetup", "dxwebsetup", "uninstall", "steamcmd", "installscript"
- calls: System.Text.RegularExpressions.Regex..ctor

