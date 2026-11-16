# MPServerProvider.App.Views.SplashWindow

Token: `0x0200001c`  Flags: `1048577`  Extends: `0x01000044`

## Fields

- `StatusText` : `class:389`  (flags=3, token=0x04000116)
- `PercentText` : `class:389`  (flags=3, token=0x04000117)
- `Progress` : `class:521`  (flags=3, token=0x04000118)
- `_contentLoaded` : `bool`  (flags=1, token=0x04000119)

## Methods

### `.ctor()` -> `void`
- token `0x06000143`  flags=`6278`  rva=`37117`
- IL size 13 bytes, 5 instructions, max_stack=8
- opcodes: callx2, ldarg.0x2, retx1
- calls: System.Windows.Window..ctor, MPServerProvider.App.Views.SplashWindow.InitializeComponent

### `RunStartupAsync()` -> `class:433`
- token `0x06000144`  flags=`134`  rva=`37132`
- IL size 55 bytes, 17 instructions, max_stack=2
- opcodes: ldloca.sx6, callx3, stfldx3, ldfldax2, ldarg.0x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `PrefetchArtworkAsync()` -> `class:433`
- token `0x06000145`  flags=`129`  rva=`37200`
- IL size 55 bytes, 17 instructions, max_stack=2
- opcodes: ldloca.sx6, callx3, stfldx3, ldfldax2, ldarg.0x1, ldc.i4.m1x1, retx1
- calls: System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start, System.Runtime.CompilerServices.AsyncTaskMethodBuilder.get_Task

### `Report(string, r8)` -> `void`
- token `0x06000146`  flags=`129`  rva=`37268`
- IL size 77 bytes, 25 instructions, max_stack=4
- opcodes: callx4, ldloca.sx4, callvirtx3, ldarg.0x3, ldfldx3, ldarg.2x2, ldc.i4.1x2, ldstrx2, ldarg.1x1, retx1
- string literals: "0", "%"
- calls: System.Windows.Controls.TextBlock.set_Text, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler..ctor, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendFormatted, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.AppendLiteral, System.Runtime.CompilerServices.DefaultInterpolatedStringHandler.ToStringAndClear, System.Windows.Controls.Primitives.RangeBase.set_Value

### `InitializeComponent()` -> `void`
- token `0x06000147`  flags=`486`  rva=`37360`
- IL size 36 bytes, 15 instructions, max_stack=2
- opcodes: ldarg.0x3, retx2, brfalse.sx1, callx1, ldc.i4.1x1, ldc.i4.2x1, ldfldx1, ldloc.0x1, ldstrx1, newobjx1, stfldx1, stloc.0x1
- string literals: "/MPServerProvider.App;component/views/splashwindow.xaml"
- calls: System.Uri..ctor, System.Windows.Application.LoadComponent

### `System.Windows.Markup.IComponentConnector.Connect(i4, object)` -> `void`
- token `0x06000148`  flags=`481`  rva=`37408`
- IL size 69 bytes, 24 instructions, max_stack=2
- opcodes: ldarg.0x4, retx4, stfldx4, castclassx3, ldarg.2x3, ldc.i4.1x2, br.sx1, ldarg.1x1, subx1, switchx1

