# MPServerProvider.Core.Services.GameDefinitionCSharp

Token: `0x0200000c`  Flags: `1048961`  Extends: `0x01000015`

## Fields

_none_

## Methods

### `Render(class:160)` -> `string`
- token `0x06000041`  flags=`150`  rva=`16308`
- IL size 651 bytes, 220 instructions, max_stack=6
- opcodes: callx42, ldloca.sx39, callvirtx27, ldstrx21, ldloc.1x16, ldloc.0x13, popx13, ldarg.0x11, ldc.i4.1x8, ldc.i4.sx8, stloc.1x8, brtrue.sx2, dupx2, newobjx2, ble.sx1, ldc.i4.0x1, ldftnx1, ldnullx1, ldsfldx1, retx1, stloc.0x1, stsfldx1
- string literals: "new GameDefinition", "{", "    Id = ", ",", "    Name = ", ",", "    Description = ", ",", "    Genres = { ", ", ", " },", "    Artwork = ", ",", "    Install = ", ",", "    Runtime = ", ",", "    Network = ", ",", "    Mods = ", "},"
- calls: System.Text.StringBuilder..ctor, System.Text.StringBuilder.AppendLine, AppendInterpolatedStringHandler..ctor, AppendInterpolatedStringHandler.AppendLiteral, MPServerProvider.Core.Models.GameDefinition.get_Id, MPServerProvider.Core.Services.GameDefinitionCSharp.Str, AppendInterpolatedStringHandler.AppendFormatted, MPServerProvider.Core.Models.GameDefinition.get_Name, MPServerProvider.Core.Models.GameDefinition.get_Description, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Models.GameDefinition.get_Genres, System.Collections.Generic.List`1.get_Count, System.Func`2..ctor, System.Linq.Enumerable.Select, System.String.Join, MPServerProvider.Core.Models.GameDefinition.get_Artwork, MPServerProvider.Core.Services.GameDefinitionCSharp.Artwork, MPServerProvider.Core.Models.GameDefinition.get_Install, MPServerProvider.Core.Services.GameDefinitionCSharp.Install, System.Text.StringBuilder.Append, MPServerProvider.Core.Models.GameDefinition.get_Runtime, MPServerProvider.Core.Services.GameDefinitionCSharp.Runtime, MPServerProvider.Core.Models.GameDefinition.get_Network, MPServerProvider.Core.Services.GameDefinitionCSharp.Network, MPServerProvider.Core.Models.GameDefinition.get_Mods, MPServerProvider.Core.Services.GameDefinitionCSharp.Mods, System.Object.ToString

### `Artwork(class:164)` -> `string`
- token `0x06000042`  flags=`145`  rva=`16972`
- IL size 153 bytes, 42 instructions, max_stack=3
- opcodes: callx11, callvirtx9, ldarg.0x6, ldstrx6, ldloc.0x4, brtrue.sx3, newobjx1, retx1, stloc.0x1
- string literals: "Cover = ", "Hero = ", "Accent = ", "new ArtworkSpec { ", ", ", " }"
- calls: System.Collections.Generic.List`1..ctor, MPServerProvider.Core.Models.ArtworkSpec.get_Cover, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Services.GameDefinitionCSharp.Str, System.String.Concat, System.Collections.Generic.List`1.Add, MPServerProvider.Core.Models.ArtworkSpec.get_Hero, MPServerProvider.Core.Models.ArtworkSpec.get_Accent, System.String.Join

### `Install(class:168)` -> `string`
- token `0x06000043`  flags=`145`  rva=`17140`
- IL size 399 bytes, 116 instructions, max_stack=6
- opcodes: callx24, callvirtx23, ldstrx15, ldarg.0x13, ldloc.0x8, ldloca.sx6, brtrue.sx5, dupx3, ble.sx2, ldc.i4.0x2, newobjx2, brfalse.sx1, ldc.i4.1x1, ldc.i4.8x1, ldftnx1, ldloc.1x1, ldnullx1, ldsfldx1, popx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1, stsfldx1
- string literals: "Provider = ", "Variants = { ", ", ", " }", "Url = ", "Archive = ", "AppId = ", "Anonymous = false", "Product = ", "WorkshopIds = { ", ", ", " }", "new InstallSpec { ", ", ", " }"
- calls: System.Collections.Generic.List`1..ctor, MPServerProvider.Core.Models.InstallSpec.get_Provider, MPServerProvider.Core.Services.GameDefinitionCSharp.Str, System.String.Concat, System.Collections.Generic.List`1.Add, MPServerProvider.Core.Models.InstallSpec.get_Variants, System.Collections.Generic.List`1.get_Count, System.Func`2..ctor, System.Linq.Enumerable.Select, System.String.Join, MPServerProvider.Core.Models.InstallSpec.get_Url, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Models.InstallSpec.get_Archive, MPServerProvider.Core.Models.InstallSpec.get_AppId, System.Nullable`1.get_HasValue, System.Nullable`1.GetValueOrDefault, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, MPServerProvider.Core.Models.InstallSpec.get_Anonymous, MPServerProvider.Core.Models.InstallSpec.get_Product, MPServerProvider.Core.Models.InstallSpec.get_WorkshopIds

### `Runtime(class:172)` -> `string`
- token `0x06000044`  flags=`145`  rva=`17552`
- IL size 453 bytes, 154 instructions, max_stack=5
- opcodes: callx26, ldloca.sx24, callvirtx17, ldstrx16, popx10, ldloc.0x9, ldarg.0x8, ldloc.2x8, br.sx4, ldc.i4.1x4, ldc.i4.sx4, stloc.2x4, brfalse.sx3, brtrue.sx3, dupx2, initobjx2, ldloc.sx2, boxx1, isinstx1, ldloc.1x1, newobjx1, retx1, stloc.0x1, stloc.1x1, stloc.sx1
- string literals: "new RuntimeSpec", "    {", "        Executable = ", ",", "        Arguments = ", ",", "{server_root}", "        WorkingDir = ", ",", "        StopCommand = ", "null", "", ",", "        Requires = new RequiresSpec { Java = ", " }", "    }"
- calls: System.Text.StringBuilder..ctor, System.Text.StringBuilder.AppendLine, AppendInterpolatedStringHandler..ctor, AppendInterpolatedStringHandler.AppendLiteral, MPServerProvider.Core.Models.RuntimeSpec.get_Executable, MPServerProvider.Core.Services.GameDefinitionCSharp.Str, AppendInterpolatedStringHandler.AppendFormatted, MPServerProvider.Core.Models.RuntimeSpec.get_Arguments, MPServerProvider.Core.Models.RuntimeSpec.get_WorkingDir, System.String.op_Inequality, MPServerProvider.Core.Models.RuntimeSpec.get_StopCommand, MPServerProvider.Core.Models.RuntimeSpec.get_Requires, MPServerProvider.Core.Models.RequiresSpec.get_Java, System.String.Concat, System.Nullable`1.get_HasValue, System.Nullable`1.GetValueOrDefault, System.Text.StringBuilder.Append, System.Object.ToString

### `Network(class:180)` -> `string`
- token `0x06000045`  flags=`145`  rva=`18020`
- IL size 180 bytes, 50 instructions, max_stack=5
- opcodes: callx13, callvirtx9, ldstrx8, ldarg.0x5, ldloca.sx4, dupx3, ldloc.0x2, brfalse.sx1, ldc.i4.1x1, ldc.i4.sx1, newobjx1, retx1, stloc.0x1
- string literals: "DefaultPort = ", "Protocol = ", "Query = ", "stdin", "ConsoleMethod = ", "new NetworkSpec { ", ", ", " }"
- calls: System.Collections.Generic.List`1..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, MPServerProvider.Core.Models.NetworkSpec.get_DefaultPort, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Collections.Generic.List`1.Add, MPServerProvider.Core.Models.NetworkSpec.get_Protocol, MPServerProvider.Core.Services.GameDefinitionCSharp.Str, System.String.Concat, MPServerProvider.Core.Models.NetworkSpec.get_Query, MPServerProvider.Core.Models.NetworkSpec.get_ConsoleMethod, System.String.op_Inequality, System.String.Join

### `Mods(class:184)` -> `string`
- token `0x06000046`  flags=`145`  rva=`18212`
- IL size 195 bytes, 55 instructions, max_stack=6
- opcodes: callvirtx10, ldstrx10, callx9, ldarg.0x5, dupx4, ldloc.0x3, brtrue.sx2, newobjx2, retx2, ble.sx1, ldc.i4.0x1, ldftnx1, ldnullx1, ldsfldx1, popx1, stloc.0x1, stsfldx1
- string literals: "new ModsSpec { Supported = false }", "Supported = true", "Folder = ", "Extensions = { ", ", ", " }", "Kind = ", "new ModsSpec { ", ", ", " }"
- calls: MPServerProvider.Core.Models.ModsSpec.get_Supported, System.Collections.Generic.List`1..ctor, System.Collections.Generic.List`1.Add, MPServerProvider.Core.Models.ModsSpec.get_Folder, MPServerProvider.Core.Services.GameDefinitionCSharp.Str, System.String.Concat, MPServerProvider.Core.Models.ModsSpec.get_Extensions, System.Collections.Generic.List`1.get_Count, System.Func`2..ctor, System.Linq.Enumerable.Select, System.String.Join, MPServerProvider.Core.Models.ModsSpec.get_Kind

### `Str(string)` -> `string`
- token `0x06000047`  flags=`145`  rva=`18420`
- IL size 88 bytes, 23 instructions, max_stack=3
- opcodes: ldstrx11, callvirtx4, ldarg.0x2, retx2, brtrue.sx1, callx1, ldloc.0x1, stloc.0x1
- string literals: "null", "\", "\\", """, "\"", "", "\r", "
", "\n", """, """
- calls: System.String.Replace, System.String.Concat

