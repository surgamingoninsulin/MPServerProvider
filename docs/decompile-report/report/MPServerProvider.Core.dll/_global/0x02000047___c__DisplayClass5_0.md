# <>c__DisplayClass5_0

Token: `0x02000047`  Flags: `1048835`  Extends: `0x01000015`

## Fields

- `ruleName` : `string`  (flags=6, token=0x04000152)
- `port` : `i4`  (flags=6, token=0x04000153)
- `name` : `string`  (flags=6, token=0x04000154)

## Methods

### `.ctor()` -> `void`
- token `0x06000282`  flags=`6278`  rva=`47450`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<AddRuleAsync>b__0(string)` -> `string`
- token `0x06000283`  flags=`131`  rva=`47460`
- IL size 88 bytes, 27 instructions, max_stack=3
- opcodes: callx8, ldloca.sx8, ldstrx3, ldarg.0x2, ldfldx2, ldarg.1x1, ldc.i4.3x1, ldc.i4.sx1, retx1
- string literals: "netsh advfirewall firewall add rule name="", "" dir=in action=allow protocol=", " localport="
- calls: System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear

### `<AddRuleAsync>b__1(class:192)` -> `bool`
- token `0x06000284`  flags=`131`  rva=`47560`
- IL size 19 bytes, 7 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x1, ldarg.1x1, ldc.i4.5x1, ldfldx1, retx1
- calls: MPServerProvider.Core.Models.FirewallRuleInfo.get_Name, System.String.Equals

