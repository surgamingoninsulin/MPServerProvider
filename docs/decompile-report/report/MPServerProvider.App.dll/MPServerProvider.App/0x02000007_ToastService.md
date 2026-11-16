# MPServerProvider.App.ToastService

Token: `0x02000007`  Flags: `1048961`  Extends: `0x0100001a`

## Fields

- `<Toasts>k__BackingField` : `class:261<class:24>`  (flags=49, token=0x04000020)

## Methods

### `get_Toasts()` -> `class:261<class:24>`
- token `0x0600002f`  flags=`2198`  rva=`9448`
- IL size 6 bytes, 2 instructions, max_stack=8
- opcodes: ldsfldx1, retx1

### `Show(string, string)` -> `void`
- token `0x06000030`  flags=`150`  rva=`9456`
- IL size 84 bytes, 31 instructions, max_stack=3
- opcodes: callx4, ldloc.0x4, callvirtx3, retx3, brtrue.sx2, dupx2, newobjx2, popx2, stfldx2, br.sx1, brfalse.sx1, ldarg.0x1, ldarg.1x1, ldc.i4.0x1, ldftnx1, stloc.0x1
- calls: <>c__DisplayClass3_0..ctor, System.Windows.Application.get_Current, System.Windows.Threading.DispatcherObject.get_Dispatcher, System.Windows.Threading.Dispatcher.CheckAccess, <>c__DisplayClass3_0.<Show>g__Add|0, System.Action..ctor, System.Windows.Threading.Dispatcher.Invoke

### `Error(string)` -> `void`
- token `0x06000031`  flags=`150`  rva=`9552`
- IL size 12 bytes, 4 instructions, max_stack=8
- opcodes: callx1, ldarg.0x1, ldstrx1, retx1
- string literals: ""
- calls: MPServerProvider.App.ToastService.Show

### `.cctor()` -> `void`
- token `0x06000032`  flags=`6289`  rva=`9565`
- IL size 11 bytes, 3 instructions, max_stack=8
- opcodes: newobjx1, retx1, stsfldx1
- calls: System.Collections.ObjectModel.ObservableCollection`1..ctor

