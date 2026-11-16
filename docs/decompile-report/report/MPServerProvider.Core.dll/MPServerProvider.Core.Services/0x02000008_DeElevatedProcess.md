# MPServerProvider.Core.Services.DeElevatedProcess

Token: `0x02000008`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `<Process>k__BackingField` : `class:169`  (flags=33, token=0x0400000f)
- `<StandardInput>k__BackingField` : `class:173`  (flags=33, token=0x04000010)
- `<Owned>k__BackingField` : `class:165[]`  (flags=33, token=0x04000011)

## Methods

### `get_Process()` -> `class:169`
- token `0x0600001c`  flags=`2182`  rva=`13923`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Process(class:169)` -> `void modreq:177`
- token `0x0600001d`  flags=`2182`  rva=`13931`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_StandardInput()` -> `class:173`
- token `0x0600001e`  flags=`2182`  rva=`13940`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_StandardInput(class:173)` -> `void modreq:177`
- token `0x0600001f`  flags=`2182`  rva=`13948`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `get_Owned()` -> `class:165[]`
- token `0x06000020`  flags=`2182`  rva=`13957`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Owned(class:165[])` -> `void modreq:177`
- token `0x06000021`  flags=`2182`  rva=`13965`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `Dispose()` -> `void`
- token `0x06000022`  flags=`486`  rva=`13976`
- IL size 37 bytes, 25 instructions, max_stack=2
- opcodes: ldloc.1x3, ldloc.0x2, leave.sx2, stloc.1x2, addx1, blt.sx1, br.sx1, callx1, callvirtx1, conv.i4x1, ldarg.0x1, ldc.i4.0x1, ldc.i4.1x1, ldelem.refx1, ldlenx1, ldloc.2x1, popx1, retx1, stloc.0x1, stloc.2x1
- calls: MPServerProvider.Core.Services.DeElevatedProcess.get_Owned, System.IDisposable.Dispose

### `.ctor()` -> `void`
- token `0x06000023`  flags=`6278`  rva=`14044`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

