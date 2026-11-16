# <>c__DisplayClass8_1

Token: `0x0200004b`  Flags: `1048835`  Extends: `0x0100001a`

## Fields

- `bytes` : `i8`  (flags=6, token=0x040001cb)
- `installedGames` : `i4`  (flags=6, token=0x040001cc)
- `CS$<>8__locals1` : `class:296`  (flags=6, token=0x040001cd)

## Methods

### `.ctor()` -> `void`
- token `0x060001cd`  flags=`6278`  rva=`53501`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<RefreshStorage>b__3()` -> `void`
- token `0x060001ce`  flags=`131`  rva=`53512`
- IL size 285 bytes, 78 instructions, max_stack=4
- opcodes: ldfldx17, callx11, ldloca.sx10, ldarg.0x9, ldstrx6, ldc.r8x5, callvirtx4, conv.r8x3, br.sx2, divx2, ldc.i4.1x2, beq.sx1, brfalse.sx1, ldc.i4.2x1, ldc.i4.3x1, ldc.i4.sx1, mulx1, retx1
- string literals: "0.0", " GB", " game", "s", "", " installed"
- calls: System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Windows.Controls.TextBlock.set_Text, MPServerProvider.Core.Services.DriveUsage.get_TotalBytes, System.Math.Clamp, System.Windows.Controls.Primitives.RangeBase.set_Value

