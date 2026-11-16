# MPServerProvider.Core.Services.RconService

Token: `0x02000012`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_client` : `class:345`  (flags=1, token=0x0400002b)
- `_stream` : `class:309`  (flags=1, token=0x0400002c)
- `_lock` : `object`  (flags=33, token=0x0400002d)
- `<Connected>k__BackingField` : `bool`  (flags=1, token=0x0400002e)

## Methods

### `get_Connected()` -> `bool`
- token `0x06000070`  flags=`2182`  rva=`24151`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Connected(bool)` -> `void`
- token `0x06000071`  flags=`2177`  rva=`24159`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `ConnectAsync(string, i4, string, valuetype:121)` -> `class:221`
- token `0x06000072`  flags=`134`  rva=`24168`
- IL size 88 bytes, 29 instructions, max_stack=2
- opcodes: ldloca.sx10, stfldx7, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldarg.sx1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `SendCommandAsync(string, valuetype:121)` -> `class:117<string>`
- token `0x06000073`  flags=`134`  rva=`24268`
- IL size 71 bytes, 23 instructions, max_stack=2
- opcodes: ldloca.sx8, stfldx5, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `SendCommandMultiAsync(string, valuetype:261<valuetype:89>, valuetype:121)` -> `class:117<string>`
- token `0x06000074`  flags=`134`  rva=`24352`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `Disconnect()` -> `void`
- token `0x06000075`  flags=`134`  rva=`24444`
- IL size 85 bytes, 38 instructions, max_stack=2
- opcodes: ldarg.0x6, callx5, ldfldx3, br.sx2, brtrue.sx2, dupx2, ldc.i4.0x2, ldloc.0x2, ldnullx2, popx2, stfldx2, brfalse.sx1, endfinallyx1, ldloc.1x1, ldloca.sx1, leave.sx1, retx1, stloc.0x1, stloc.1x1
- calls: System.Threading.Monitor.Enter, MPServerProvider.Core.Services.RconService.set_Connected, System.IO.Stream.Dispose, System.Net.Sockets.TcpClient.Dispose, System.Threading.Monitor.Exit

### `Dispose()` -> `void`
- token `0x06000076`  flags=`486`  rva=`24560`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: MPServerProvider.Core.Services.RconService.Disconnect

### `BuildPacket(i4, string)` -> `u1[]`
- token `0x06000077`  flags=`145`  rva=`24568`
- IL size 96 bytes, 45 instructions, max_stack=4
- opcodes: callvirtx9, dupx5, ldloc.2x4, ldc.i4.0x3, ldloc.0x2, ldloc.1x2, newobjx2, addx1, brfalse.sx1, callx1, conv.i4x1, endfinallyx1, ldarg.0x1, ldarg.1x1, ldc.i4.4x1, ldc.i4.sx1, ldlenx1, ldloc.3x1, leave.sx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1, stloc.3x1, subx1
- calls: System.Text.Encoding.get_UTF8, System.Text.Encoding.GetBytes, System.IO.MemoryStream..ctor, System.IO.BinaryWriter..ctor, System.IO.BinaryWriter.Write, System.IO.MemoryStream.ToArray, System.IDisposable.Dispose

### `ReadPacketAsync(valuetype:121)` -> `class:117<class:380>`
- token `0x06000078`  flags=`129`  rva=`24692`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `.ctor()` -> `void`
- token `0x06000079`  flags=`6278`  rva=`24767`
- IL size 18 bytes, 6 instructions, max_stack=8
- opcodes: ldarg.0x2, callx1, newobjx1, retx1, stfldx1
- calls: System.Object..ctor

