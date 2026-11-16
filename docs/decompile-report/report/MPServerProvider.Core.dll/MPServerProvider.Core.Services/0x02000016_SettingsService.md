# MPServerProvider.Core.Services.SettingsService

Token: `0x02000016`  Flags: `1048833`  Extends: `0x01000015`

## Fields

- `_paths` : `class:16`  (flags=33, token=0x0400003d)
- `<Settings>k__BackingField` : `class:216`  (flags=1, token=0x0400003e)

## Methods

### `get_Settings()` -> `class:216`
- token `0x060000ad`  flags=`2182`  rva=`29236`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: ldarg.0x1, ldfldx1, retx1

### `set_Settings(class:216)` -> `void`
- token `0x060000ae`  flags=`2177`  rva=`29244`
- IL size 8 bytes, 4 instructions, max_stack=8
- opcodes: ldarg.0x1, ldarg.1x1, retx1, stfldx1

### `.ctor(class:16)` -> `void`
- token `0x060000af`  flags=`6278`  rva=`29253`
- IL size 31 bytes, 11 instructions, max_stack=8
- opcodes: ldarg.0x4, callx2, stfldx2, ldarg.1x1, newobjx1, retx1
- calls: MPServerProvider.Core.Models.AppSettings..ctor, System.Object..ctor, MPServerProvider.Core.Services.SettingsService.Load

### `Load()` -> `void`
- token `0x060000b0`  flags=`134`  rva=`29288`
- IL size 72 bytes, 22 instructions, max_stack=3
- opcodes: callx5, ldarg.0x5, callvirtx4, brtrue.sx2, ldfldx2, dupx1, newobjx1, popx1, retx1
- calls: MPServerProvider.Core.Services.AppPaths.get_SettingsFile, MPServerProvider.Core.Json.Json.ReadFile, MPServerProvider.Core.Models.AppSettings..ctor, MPServerProvider.Core.Services.SettingsService.set_Settings, MPServerProvider.Core.Services.SettingsService.get_Settings, MPServerProvider.Core.Models.AppSettings.get_InstallRoot, System.String.IsNullOrWhiteSpace, MPServerProvider.Core.Services.AppPaths.OverrideServersDir

### `Save()` -> `void`
- token `0x060000b1`  flags=`134`  rva=`29372`
- IL size 29 bytes, 9 instructions, max_stack=8
- opcodes: callx3, ldarg.0x3, callvirtx1, ldfldx1, retx1
- calls: MPServerProvider.Core.Services.AppPaths.get_SettingsFile, MPServerProvider.Core.Services.SettingsService.get_Settings, MPServerProvider.Core.Json.Json.WriteFile, MPServerProvider.Core.Services.SettingsService.ApplyStartOnBoot

### `SetInstallRoot(string)` -> `void`
- token `0x060000b2`  flags=`134`  rva=`29402`
- IL size 31 bytes, 11 instructions, max_stack=8
- opcodes: ldarg.0x3, callx2, callvirtx2, ldarg.1x2, ldfldx1, retx1
- calls: MPServerProvider.Core.Services.SettingsService.get_Settings, MPServerProvider.Core.Models.AppSettings.set_InstallRoot, MPServerProvider.Core.Services.AppPaths.OverrideServersDir, MPServerProvider.Core.Services.SettingsService.Save

### `ApplyStartOnBoot()` -> `void`
- token `0x060000b3`  flags=`129`  rva=`29436`
- IL size 139 bytes, 48 instructions, max_stack=6
- opcodes: ldstrx8, callvirtx7, ldloc.0x6, callx4, leave.sx4, brfalse.sx3, br.sx2, brtrue.sx2, ldarg.0x2, endfinallyx1, ldc.i4.1x1, ldloc.1x1, ldloc.2x1, ldsfldx1, popx1, retx1, stloc.0x1, stloc.1x1, stloc.2x1
- string literals: "Software\Microsoft\Windows\CurrentVersion\Run", "", " --minimized", "MPServerProvider", """, """, "MPServerProvider", "MPServerProvider"
- calls: Microsoft.Win32.RegistryKey.OpenSubKey, MPServerProvider.Core.Services.SettingsService.get_Settings, MPServerProvider.Core.Models.AppSettings.get_StartOnBoot, System.Environment.get_ProcessPath, MPServerProvider.Core.Models.AppSettings.get_StartMinimizedToTray, System.String.Concat, Microsoft.Win32.RegistryKey.SetValue, Microsoft.Win32.RegistryKey.GetValue, Microsoft.Win32.RegistryKey.DeleteValue, System.IDisposable.Dispose

