# RunningServer

Token: `0x02000064`  Flags: `1048835`  Extends: `0x01000015`

## Fields

- `_console` : `class:369<string>`  (flags=33, token=0x040001fb)
- `_lock` : `object`  (flags=33, token=0x040001fc)
- `_recentCrashes` : `class:369<valuetype:157>`  (flags=33, token=0x040001fd)
- `<Server>k__BackingField` : `class:220`  (flags=1, token=0x040001fe)
- `<Process>k__BackingField` : `class:169`  (flags=1, token=0x040001ff)
- `<State>k__BackingField` : `valuetype:232`  (flags=1, token=0x04000200)
- `<StartedAt>k__BackingField` : `valuetype:261<valuetype:157>`  (flags=1, token=0x04000201)
- `<DeElevated>k__BackingField` : `class:32`  (flags=1, token=0x04000202)

## Methods

### `.ctor(class:220)` -> `void`
- token `0x060002d6`  flags=`6278`  rva=`59410`
- IL size 47 bytes, 15 instructions, max_stack=8
- opcodes: ldarg.0x5, newobjx3, stfldx3, callx2, ldarg.1x1, retx1
- calls: System.Collections.Generic.Queue`1..ctor, System.Object..ctor, RunningServer.set_Server

### `get_Server()` -> `class:220`
- token `0x060002d7`  flags=`2182`  rva=`59458`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Server(class:220)` -> `void`
- token `0x060002d8`  flags=`2182`  rva=`59466`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Process()` -> `class:169`
- token `0x060002d9`  flags=`2182`  rva=`59475`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Process(class:169)` -> `void`
- token `0x060002da`  flags=`2182`  rva=`59483`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_State()` -> `valuetype:232`
- token `0x060002db`  flags=`2182`  rva=`59492`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_State(valuetype:232)` -> `void`
- token `0x060002dc`  flags=`2182`  rva=`59500`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_StartedAt()` -> `valuetype:261<valuetype:157>`
- token `0x060002dd`  flags=`2182`  rva=`59509`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_StartedAt(valuetype:261<valuetype:157>)` -> `void`
- token `0x060002de`  flags=`2182`  rva=`59517`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_DeElevated()` -> `class:32`
- token `0x060002df`  flags=`2182`  rva=`59526`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_DeElevated(class:32)` -> `void`
- token `0x060002e0`  flags=`2182`  rva=`59534`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_StdIn()` -> `class:173`
- token `0x060002e1`  flags=`2182`  rva=`59543`
- IL size 40 bytes, 20 instructions, max_stack=8
- opcodes: callx4, brtrue.sx3, dupx3, popx3, ldarg.0x2, ldnullx2, retx2, br.sx1
- calls: RunningServer.get_DeElevated, MPServerProvider.Core.Services.DeElevatedProcess.get_StandardInput, RunningServer.get_Process, System.Diagnostics.Process.get_StandardInput

### `Append(string)` -> `void`
- token `0x060002e2`  flags=`134`  rva=`59584`
- IL size 74 bytes, 29 instructions, max_stack=2
- opcodes: ldarg.0x4, ldfldx4, callvirtx3, callx2, ldloc.0x2, bgt.sx1, br.sx1, brfalse.sx1, endfinallyx1, ldarg.1x1, ldc.i4x1, ldc.i4.0x1, ldloc.1x1, ldloca.sx1, leave.sx1, popx1, retx1, stloc.0x1, stloc.1x1
- calls: System.Threading.Monitor.Enter, System.Collections.Generic.Queue`1.Enqueue, System.Collections.Generic.Queue`1.Dequeue, System.Collections.Generic.Queue`1.get_Count, System.Threading.Monitor.Exit

### `ClearConsole()` -> `void`
- token `0x060002e3`  flags=`134`  rva=`59688`
- IL size 41 bytes, 18 instructions, max_stack=2
- opcodes: callx2, ldarg.0x2, ldfldx2, ldloc.0x2, brfalse.sx1, callvirtx1, endfinallyx1, ldc.i4.0x1, ldloc.1x1, ldloca.sx1, leave.sx1, retx1, stloc.0x1, stloc.1x1
- calls: System.Threading.Monitor.Enter, System.Collections.Generic.Queue`1.Clear, System.Threading.Monitor.Exit

### `ConsoleSnapshot()` -> `class:269<string>`
- token `0x060002e4`  flags=`134`  rva=`59760`
- IL size 43 bytes, 20 instructions, max_stack=2
- opcodes: callx2, ldarg.0x2, ldfldx2, ldloc.0x2, brfalse.sx1, callvirtx1, endfinallyx1, ldc.i4.0x1, ldloc.1x1, ldloc.2x1, ldloca.sx1, leave.sx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1
- calls: System.Threading.Monitor.Enter, System.Collections.Generic.Queue`1.ToArray, System.Threading.Monitor.Exit

### `RegisterCrashForRestart()` -> `bool`
- token `0x060002e5`  flags=`134`  rva=`59832`
- IL size 102 bytes, 33 instructions, max_stack=2
- opcodes: callvirtx5, ldarg.0x5, ldfldx5, callx4, ldc.i4.0x2, ldloc.0x2, ble.sx1, br.sx1, brtrue.sx1, ceqx1, cgtx1, ldc.i4.3x1, ldc.r8x1, popx1, retx1, stloc.0x1
- calls: System.DateTimeOffset.get_Now, System.Collections.Generic.Queue`1.Dequeue, System.Collections.Generic.Queue`1.get_Count, System.Collections.Generic.Queue`1.Peek, System.DateTimeOffset.op_Subtraction, System.TimeSpan.FromMinutes, System.TimeSpan.op_GreaterThan, System.Collections.Generic.Queue`1.Enqueue

