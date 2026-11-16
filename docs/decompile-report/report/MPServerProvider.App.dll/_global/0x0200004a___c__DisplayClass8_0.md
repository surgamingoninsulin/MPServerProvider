# <>c__DisplayClass8_0

Token: `0x0200004a`  Flags: `1048835`  Extends: `0x0100001a`

## Fields

- `installDir` : `string`  (flags=6, token=0x040001c8)
- `<>4__this` : `class:108`  (flags=6, token=0x040001c9)
- `drive` : `class:661`  (flags=6, token=0x040001ca)

## Methods

### `.ctor()` -> `void`
- token `0x060001ca`  flags=`6278`  rva=`53340`
- IL size 7 bytes, 3 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, retx1
- calls: System.Object..ctor

### `<RefreshStorage>b__0(class:661)` -> `bool`
- token `0x060001cb`  flags=`131`  rva=`53348`
- IL size 19 bytes, 7 instructions, max_stack=8
- opcodes: callvirtx2, ldarg.0x1, ldarg.1x1, ldc.i4.5x1, ldfldx1, retx1
- calls: MPServerProvider.Core.Services.DriveUsage.get_Root, System.String.StartsWith

### `<RefreshStorage>b__1()` -> `void`
- token `0x060001cc`  flags=`131`  rva=`53368`
- IL size 121 bytes, 34 instructions, max_stack=4
- opcodes: callx6, ldloc.0x4, ldarg.0x3, newobjx3, stfldx3, callvirtx2, dupx2, ldfldx2, ldftnx2, ldsfldx2, brtrue.sx1, popx1, retx1, stloc.0x1, stsfldx1
- calls: <>c__DisplayClass8_1..ctor, MPServerProvider.Core.Services.SystemMetricsService.DirectorySizeBytes, MPServerProvider.App.AppServices.get_Servers, MPServerProvider.Core.Services.ServerRepository.get_Servers, <>c.<RefreshStorage>b__8_2, System.Func`2..ctor, System.Linq.Enumerable.Select, System.Linq.Enumerable.Distinct, System.Linq.Enumerable.Count, System.Windows.Threading.DispatcherObject.get_Dispatcher, <>c__DisplayClass8_1.<RefreshStorage>b__3, System.Action..ctor, System.Windows.Threading.Dispatcher.Invoke

