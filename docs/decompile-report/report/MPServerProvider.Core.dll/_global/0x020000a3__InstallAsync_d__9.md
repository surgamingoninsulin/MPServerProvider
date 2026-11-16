# <InstallAsync>d__9

Token: `0x020000a3`  Flags: `1048835`  Extends: `0x0100001f`

## Fields

- `<>1__state` : `i4`  (flags=6, token=0x04000376)
- `<>t__builder` : `valuetype:145<class:212>`  (flags=6, token=0x04000377)
- `progress` : `class:233<class:200>`  (flags=6, token=0x04000378)
- `request` : `class:208`  (flags=6, token=0x04000379)
- `<>4__this` : `class:152`  (flags=6, token=0x0400037a)
- `ct` : `valuetype:121`  (flags=6, token=0x0400037b)
- `<>8__1` : `class:640`  (flags=1, token=0x0400037c)
- `<appId>5__2` : `i4`  (flags=1, token=0x0400037d)
- `<process>5__3` : `class:169`  (flags=1, token=0x0400037e)
- `<definitionExe>5__4` : `string`  (flags=1, token=0x0400037f)
- `<serverExe>5__5` : `string`  (flags=1, token=0x04000380)
- `<>u__1` : `valuetype:137<string>`  (flags=1, token=0x04000381)
- `<>u__2` : `valuetype:133`  (flags=1, token=0x04000382)
- `<>u__3` : `valuetype:137<class:144>`  (flags=1, token=0x04000383)

## Methods

### `MoveNext()` -> `void`
- token `0x0600039b`  flags=`481`  rva=`91316`
- IL size 1752 bytes, 568 instructions, max_stack=4
- opcodes: ldarg.0x98, ldfldx63, callx60, callvirtx49, ldloca.sx38, stfldx31, stloc.sx25, ldloc.sx23, ldstrx20, dupx17, brtrue.sx12, newobjx12, ldc.i4.1x10, ldfldax10, ldnullx10, stloc.0x9, ldc.i4.sx8, ldc.i4.0x6, ldloc.0x6, leave.sx6, initobjx4, ldc.i4.m1x4, ldloc.1x4, leavex4, br.sx3, brfalse.sx3, ldc.i4.3x3, bge.sx2, brtruex2, endfinallyx2, ldc.i4.2x2, ldftnx2, nopx2, popx2, subx2, throwx2, beq.sx1, blex1, ble.unx1, brfalsex1, ldloc.2x1, ldloc.3x1, retx1, rethrowx1, stloc.1x1, stloc.2x1, stloc.3x1, switchx1
- string literals: "Game '", "' has no steamcmd app_id.", "SteamCMD: downloading app ", " (this can take a while)…", "+force_install_dir "", "" ", " +app_update ", " validate +quit", " | ", " SteamCMD said: ", "", "SteamCMD exited with code ", " and no server executable was found in ", ".", ""{server_root}\", "" ", ""{server_root}\", """, "latest (app ", ")"
- calls: <>c__DisplayClass9_0..ctor, MPServerProvider.Core.Models.InstallRequest.get_Game, MPServerProvider.Core.Models.GameDefinition.get_Install, MPServerProvider.Core.Models.InstallSpec.get_AppId, System.Nullable`1.get_HasValue, MPServerProvider.Core.Models.GameDefinition.get_Id, System.String.Concat, System.InvalidOperationException..ctor, System.Nullable`1.GetValueOrDefault, MPServerProvider.Core.Providers.SteamCmdProvider.EnsureSteamCmdAsync, System.Threading.Tasks.Task`1.GetAwaiter, System.Runtime.CompilerServices.TaskAwaiter`1.get_IsCompleted, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.AwaitUnsafeOnCompleted, System.Runtime.CompilerServices.TaskAwaiter`1.GetResult, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallProgress.Indeterminate, System.IProgress`1.Report, System.Diagnostics.ProcessStartInfo..ctor, System.Diagnostics.ProcessStartInfo.set_FileName, MPServerProvider.Core.Models.InstallRequest.get_TargetDir, MPServerProvider.Core.Providers.SteamCmdProvider.LoginArguments, System.Diagnostics.ProcessStartInfo.set_Arguments, System.Diagnostics.ProcessStartInfo.set_UseShellExecute, System.Diagnostics.ProcessStartInfo.set_CreateNoWindow, System.Diagnostics.ProcessStartInfo.set_RedirectStandardOutput, System.Diagnostics.ProcessStartInfo.set_RedirectStandardError, System.Collections.Generic.Queue`1..ctor, System.Diagnostics.Process.Start, <>c__DisplayClass9_0.<InstallAsync>b__1, System.Diagnostics.DataReceivedEventHandler..ctor, System.Diagnostics.Process.add_OutputDataReceived, <>c__DisplayClass9_0.<InstallAsync>b__2, System.Diagnostics.Process.add_ErrorDataReceived, System.Diagnostics.Process.BeginOutputReadLine, System.Diagnostics.Process.BeginErrorReadLine, System.Diagnostics.Process.WaitForExitAsync, System.Threading.Tasks.Task.GetAwaiter...

### `SetStateMachine(class:129)` -> `void`
- token `0x0600039c`  flags=`481`  rva=`93204`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldarg.1x1, ldfldax1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.SetStateMachine

