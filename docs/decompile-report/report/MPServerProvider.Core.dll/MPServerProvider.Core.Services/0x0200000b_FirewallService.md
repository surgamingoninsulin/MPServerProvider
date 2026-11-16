# MPServerProvider.Core.Services.FirewallService

Token: `0x0200000b`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `Prefix` : `string`  (flags=32849, token=0x0400001c)
- `_settings` : `class:88`  (flags=33, token=0x0400001d)

## Methods

### `.ctor(class:88)` -> `void`
- token `0x0600003a`  flags=`6278`  rva=`15861`
- IL size 14 bytes, 6 instructions, max_stack=8
- opcodes: ldarg.0x2, callx1, ldarg.1x1, retx1, stfldx1
- calls: System.Object..ctor

### `get_Rules()` -> `class:269<class:192>`
- token `0x0600003b`  flags=`2182`  rva=`15876`
- IL size 17 bytes, 5 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x1, ldfldx1, retx1
- calls: MPServerProvider.Core.Services.SettingsService.get_Settings, MPServerProvider.Core.Models.AppSettings.get_FirewallRules

### `AddRuleAsync(string, i4, string)` -> `class:221`
- token `0x0600003c`  flags=`134`  rva=`15896`
- IL size 79 bytes, 26 instructions, max_stack=2
- opcodes: ldloca.sx9, stfldx6, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldarg.2x1, ldarg.3x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `DeleteRuleAsync(class:192)` -> `class:221`
- token `0x0600003d`  flags=`134`  rva=`15988`
- IL size 63 bytes, 20 instructions, max_stack=2
- opcodes: ldloca.sx7, stfldx4, callx3, ldfldax2, ldarg.0x1, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `RuleExistsAsync(class:192)` -> `class:117<bool>`
- token `0x0600003e`  flags=`134`  rva=`16064`
- IL size 55 bytes, 17 instructions, max_stack=2
- opcodes: ldloca.sx6, callx3, stfldx3, ldfldax2, ldarg.1x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1.get_Task

### `ProtocolsFor(string)` -> `class:273<string>`
- token `0x0600003f`  flags=`145`  rva=`16132`
- IL size 94 bytes, 41 instructions, max_stack=4
- opcodes: ldstrx6, dupx4, stelem.refx4, br.sx3, ldc.i4.0x3, ldc.i4.1x3, newarrx3, stloc.0x3, brtrue.sx2, callx2, ldloc.1x2, callvirtx1, ldarg.0x1, ldc.i4.2x1, ldloc.0x1, retx1, stloc.1x1
- string literals: "tcp", "udp", "TCP", "UDP", "TCP", "UDP"
- calls: System.String.ToLowerInvariant, System.String.op_Equality

### `RunElevatedAsync(string)` -> `class:221`
- token `0x06000040`  flags=`145`  rva=`16240`
- IL size 55 bytes, 17 instructions, max_stack=2
- opcodes: ldloca.sx6, callx3, stfldx3, ldfldax2, ldarg.0x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

