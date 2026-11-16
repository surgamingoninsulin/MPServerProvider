# MPServerProvider.Core.Services.DeElevatedLauncher

Token: `0x02000009`  Flags: `1048961`  Extends: `0x01000015`

## Fields

- `PROCESS_QUERY_INFORMATION` : `u4`  (flags=32849, token=0x04000012)
- `TOKEN_DUPLICATE` : `u4`  (flags=32849, token=0x04000013)
- `TOKEN_ALL_ACCESS` : `u4`  (flags=32849, token=0x04000014)
- `CREATE_NO_WINDOW` : `u4`  (flags=32849, token=0x04000015)
- `CREATE_UNICODE_ENVIRONMENT` : `u4`  (flags=32849, token=0x04000016)
- `STARTF_USESTDHANDLES` : `i4`  (flags=32849, token=0x04000017)
- `LOGON_WITH_PROFILE` : `u4`  (flags=32849, token=0x04000018)
- `SecurityImpersonation` : `i4`  (flags=32849, token=0x04000019)
- `TokenPrimary` : `i4`  (flags=32849, token=0x0400001a)

## Methods

### `IsCurrentProcessElevated()` -> `bool`
- token `0x06000024`  flags=`150`  rva=`14052`
- IL size 42 bytes, 19 instructions, max_stack=2
- opcodes: ldloc.0x3, callvirtx2, leave.sx2, stloc.1x2, brfalse.sx1, callx1, endfinallyx1, ldc.i4x1, ldc.i4.0x1, ldloc.1x1, newobjx1, popx1, retx1, stloc.0x1
- calls: System.Security.Principal.WindowsIdentity.GetCurrent, System.Security.Principal.WindowsPrincipal..ctor, System.Security.Principal.WindowsPrincipal.IsInRole, System.IDisposable.Dispose

### `Start(string, string, string, class:193<string>)` -> `class:32`
- token `0x06000025`  flags=`150`  rva=`14136`
- IL size 480 bytes, 168 instructions, max_stack=11
- opcodes: callvirtx21, ldloc.sx17, callx15, ldloca.sx13, ldc.i4.1x9, newobjx9, stloc.sx9, ldloc.1x5, popx5, stfldx5, brtrue.sx4, dupx4, ldloc.2x4, ldloc.3x4, ldsfldx4, leave.sx4, ldc.i4x3, ldfldx3, ldloc.0x3, ldnullx3, ldstrx3, br.sx2, endfinallyx2, ldarg.2x2, ldarg.3x2, constrained.x1, initobjx1, ldarg.0x1, ldarg.1x1, ldc.i4.0x1, ldc.i4.2x1, retx1, rethrowx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1, throwx1
- string literals: """, "" ", "Could not start the server as the desktop user"
- calls: MPServerProvider.Core.Services.DeElevatedLauncher.OpenShellUserToken, System.Collections.Generic.List`1..ctor, System.IO.Pipes.AnonymousPipeServerStream..ctor, System.Collections.Generic.List`1.Add, System.Runtime.InteropServices.Marshal.SizeOf, System.IO.Pipes.AnonymousPipeServerStream.get_ClientSafePipeHandle, System.Runtime.InteropServices.SafeHandle.DangerousGetHandle, System.String.Concat, System.Text.StringBuilder..ctor, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Services.DeElevatedLauncher.CreateProcessWithTokenW, MPServerProvider.Core.Services.DeElevatedLauncher.CreateProcessAsUser, System.Runtime.InteropServices.Marshal.GetLastWin32Error, Win32Exception_..ctor, MPServerProvider.Core.Services.DeElevatedLauncher.CloseHandle, System.IO.Pipes.AnonymousPipeServerStream.DisposeLocalCopyOfClientHandle, System.Diagnostics.Process.GetProcessById, System.Diagnostics.Process.set_EnableRaisingEvents, MPServerProvider.Core.Services.DeElevatedLauncher.Pump, System.Text.UTF8Encoding..ctor, System.IO.StreamWriter..ctor, System.IO.StreamWriter.set_AutoFlush, MPServerProvider.Core.Services.DeElevatedProcess..ctor, MPServerProvider.Core.Services.DeElevatedProcess.set_Process, MPServerProvider.Core.Services.DeElevatedProcess.set_StandardInput, System.Collections.Generic.List`1.ToArray, MPServerProvider.Core.Services.DeElevatedProcess.set_Owned, System.Collections.Generic.List`1.GetEnumerator, Enumerator.get_Current, System.IDisposable.Dispose, Enumerator.MoveNext

### `Pump(class:209, class:193<string>)` -> `void`
- token `0x06000026`  flags=`145`  rva=`14728`
- IL size 37 bytes, 12 instructions, max_stack=8
- opcodes: dupx2, newobjx2, stfldx2, callx1, ldarg.0x1, ldarg.1x1, ldftnx1, popx1, retx1
- calls: <>c__DisplayClass2_0..ctor, <>c__DisplayClass2_0.<Pump>b__0, System.Func`1..ctor, System.Threading.Tasks.Task.Run

### `OpenShellUserToken()` -> `native-int`
- token `0x06000027`  flags=`145`  rva=`14768`
- IL size 174 bytes, 63 instructions, max_stack=6
- opcodes: callx10, ldstrx5, newobjx5, throwx5, brtrue.sx3, ldloc.1x3, ldloca.sx3, ldsfldx3, popx3, bne.un.sx2, endfinallyx2, ldc.i4x2, ldc.i4.2x2, ldloc.0x2, ldloc.2x2, nopx2, dupx1, ldc.i4.0x1, ldc.i4.1x1, ldloc.3x1, ldloc.sx1, leave.sx1, retx1, stloc.1x1, stloc.sx1
- string literals: "No shell window — cannot obtain the desktop user's token.", "Could not identify the shell process.", "OpenProcess(explorer) failed", "OpenProcessToken(explorer) failed", "DuplicateTokenEx failed"
- calls: MPServerProvider.Core.Services.DeElevatedLauncher.GetShellWindow, System.InvalidOperationException..ctor, MPServerProvider.Core.Services.DeElevatedLauncher.GetWindowThreadProcessId, MPServerProvider.Core.Services.DeElevatedLauncher.OpenProcess, System.Runtime.InteropServices.Marshal.GetLastWin32Error, Win32Exception_..ctor, MPServerProvider.Core.Services.DeElevatedLauncher.OpenProcessToken, MPServerProvider.Core.Services.DeElevatedLauncher.DuplicateTokenEx, MPServerProvider.Core.Services.DeElevatedLauncher.CloseHandle

### `GetShellWindow()` -> `native-int`
- token `0x06000028`  flags=`8337`  rva=`0`
- body status: absent

### `GetWindowThreadProcessId(native-int, u4&)` -> `i4`
- token `0x06000029`  flags=`8337`  rva=`0`
- body status: absent

### `OpenProcess(u4, bool, u4)` -> `native-int`
- token `0x0600002a`  flags=`8337`  rva=`0`
- body status: absent

### `CloseHandle(native-int)` -> `bool`
- token `0x0600002b`  flags=`8337`  rva=`0`
- body status: absent

### `OpenProcessToken(native-int, u4, native-int&)` -> `bool`
- token `0x0600002c`  flags=`8337`  rva=`0`
- body status: absent

### `DuplicateTokenEx(native-int, u4, native-int, i4, i4, native-int&)` -> `bool`
- token `0x0600002d`  flags=`8337`  rva=`0`
- body status: absent

### `CreateProcessWithTokenW(native-int, u4, string, class:201, u4, native-int, string, valuetype:260&, valuetype:256&)` -> `bool`
- token `0x0600002e`  flags=`8337`  rva=`0`
- body status: absent

### `CreateProcessAsUser(native-int, string, class:201, native-int, native-int, bool, u4, native-int, string, valuetype:260&, valuetype:256&)` -> `bool`
- token `0x0600002f`  flags=`8337`  rva=`0`
- body status: absent

