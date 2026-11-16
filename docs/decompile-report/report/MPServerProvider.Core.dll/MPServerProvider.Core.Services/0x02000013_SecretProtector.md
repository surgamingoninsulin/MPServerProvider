# MPServerProvider.Core.Services.SecretProtector

Token: `0x02000013`  Flags: `1048961`  Extends: `0x01000015`

## Fields

- `Entropy` : `u1[]`  (flags=49, token=0x0400002f)

## Methods

### `Protect(string)` -> `string`
- token `0x0600007a`  flags=`150`  rva=`24788`
- IL size 48 bytes, 21 instructions, max_stack=3
- opcodes: callx4, ldarg.0x2, ldnullx2, leave.sx2, retx2, stloc.0x2, brfalse.sx1, callvirtx1, ldc.i4.0x1, ldloc.0x1, ldsfldx1, nopx1, popx1
- calls: System.String.IsNullOrEmpty, System.Text.Encoding.get_UTF8, System.Text.Encoding.GetBytes, System.Security.Cryptography.ProtectedData.Protect, System.Convert.ToBase64String

### `Unprotect(string)` -> `string`
- token `0x0600007b`  flags=`150`  rva=`24864`
- IL size 50 bytes, 23 instructions, max_stack=3
- opcodes: callx4, ldarg.0x2, ldnullx2, leave.sx2, retx2, stloc.1x2, brfalse.sx1, callvirtx1, ldc.i4.0x1, ldloc.0x1, ldloc.1x1, ldsfldx1, nopx1, popx1, stloc.0x1
- calls: System.String.IsNullOrEmpty, System.Convert.FromBase64String, System.Security.Cryptography.ProtectedData.Unprotect, System.Text.Encoding.get_UTF8, System.Text.Encoding.GetString

### `.cctor()` -> `void`
- token `0x0600007c`  flags=`6289`  rva=`24944`
- IL size 21 bytes, 5 instructions, max_stack=8
- opcodes: callx1, callvirtx1, ldstrx1, retx1, stsfldx1
- string literals: "MPServerProvider.v1"
- calls: System.Text.Encoding.get_UTF8, System.Text.Encoding.GetBytes

