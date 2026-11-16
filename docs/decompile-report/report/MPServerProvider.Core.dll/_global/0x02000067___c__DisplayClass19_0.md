# <>c__DisplayClass19_0

Token: `0x02000067`  Flags: `1048835`  Extends: `0x01000015`

## Fields

- `server` : `class:220`  (flags=6, token=0x04000207)
- `<>4__this` : `class:80`  (flags=6, token=0x04000208)
- `running` : `class:400`  (flags=6, token=0x04000209)

## Methods

### `.ctor()` -> `void`
- token `0x060002ea`  flags=`6278`  rva=`60037`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<StartAsync>b__0(string)` -> `class:400`
- token `0x060002eb`  flags=`131`  rva=`60045`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, newobjx1, retx1
- calls: RunningServer..ctor

### `<StartAsync>b__1(object, class:373)` -> `void`
- token `0x060002ec`  flags=`131`  rva=`60058`
- IL size 18 bytes, 6 instructions, max_stack=8
- opcodes: ldarg.0x2, ldfldx2, callx1, retx1
- calls: MPServerProvider.Core.Services.ServerProcessService.OnExited

### `<StartAsync>b__2(object, class:377)` -> `void`
- token `0x060002ed`  flags=`131`  rva=`60077`
- IL size 32 bytes, 11 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x2, ldarg.2x2, ldfldx2, brfalse.sx1, callx1, retx1
- calls: System.Diagnostics.DataReceivedEventArgs.get_Data, MPServerProvider.Core.Services.ServerProcessService.AppendLine

### `<StartAsync>b__3(object, class:377)` -> `void`
- token `0x060002ee`  flags=`131`  rva=`60110`
- IL size 32 bytes, 11 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x2, ldarg.2x2, ldfldx2, brfalse.sx1, callx1, retx1
- calls: System.Diagnostics.DataReceivedEventArgs.get_Data, MPServerProvider.Core.Services.ServerProcessService.AppendLine

### `<StartAsync>b__4(object, class:373)` -> `void`
- token `0x060002ef`  flags=`131`  rva=`60143`
- IL size 18 bytes, 6 instructions, max_stack=8
- opcodes: ldarg.0x2, ldfldx2, callx1, retx1
- calls: MPServerProvider.Core.Services.ServerProcessService.OnExited

### `<StartAsync>b__5(object, class:377)` -> `void`
- token `0x060002f0`  flags=`131`  rva=`60162`
- IL size 32 bytes, 11 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x2, ldarg.2x2, ldfldx2, brfalse.sx1, callx1, retx1
- calls: System.Diagnostics.DataReceivedEventArgs.get_Data, MPServerProvider.Core.Services.ServerProcessService.AppendLine

### `<StartAsync>b__6(object, class:377)` -> `void`
- token `0x060002f1`  flags=`131`  rva=`60195`
- IL size 32 bytes, 11 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x2, ldarg.2x2, ldfldx2, brfalse.sx1, callx1, retx1
- calls: System.Diagnostics.DataReceivedEventArgs.get_Data, MPServerProvider.Core.Services.ServerProcessService.AppendLine

### `<StartAsync>b__7(object, class:373)` -> `void`
- token `0x060002f2`  flags=`131`  rva=`60228`
- IL size 18 bytes, 6 instructions, max_stack=8
- opcodes: ldarg.0x2, ldfldx2, callx1, retx1
- calls: MPServerProvider.Core.Services.ServerProcessService.OnExited

