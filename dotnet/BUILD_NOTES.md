# Build notes

This is a from-scratch C#/WPF (.NET 8) reconstruction of MP Server Provider,
whose original source was lost. It's rebuilt from a decompile of the shipped
binaries plus the original XAML recovered from their compiled BAML resources,
cross-checked against real screenshots of the running original app for
anything the decompile couldn't recover (colors, background art, the sliding
news carousel's behavior).

An earlier Rust/Slint attempt existed alongside this one but was removed once
this C# version (the native platform, much more faithful) replaced it as the
only implementation.

## Build

```
cd dotnet
dotnet build MPServerProvider.sln
```

Plain `dotnet` CLI, no special setup. SDKs installed: 8.0.428 and 10.0.300;
the projects explicitly target `net8.0`/`net8.0-windows` to match the
original.

## Layout

- `src/MPServerProvider.Core` — class library: models, services, providers,
  JSON helpers. Mirrors the original's real namespaces
  (`MPServerProvider.Core.Models`, `.Services`, `.Providers`, `.Json`) and,
  where decompile coverage allows, the original's real class/method names.
- `src/MPServerProvider.Core.Tests` — xUnit tests, the main correctness
  signal for the Core port (there's no original source to diff against).
- `src/MPServerProvider.App` — WPF app: `Views/` (XAML + code-behind, one
  pair per original screen, seeded from the recovered XAML under
  `../docs/decompile-report/report/baml_xaml/`), `ViewModels/`
  (CommunityToolkit.Mvvm `[ObservableProperty]`/`[RelayCommand]`, matching
  the original's real MVVM framework choice), `Themes/Theme.xaml` (the real
  sampled color palette).

## Source of truth, in priority order

1. `../docs/reference-screenshots/*.png` + `INDEX.md` — real screenshots of
   the original app running; THE authority for colors, layout proportions,
   and interaction details (e.g. the news carousel actually sliding).
2. `../docs/decompile-report/report/baml_xaml/*.xaml` — fully recovered real
   XAML element trees (names, control types, structure) for all 16 screens.
3. `../docs/decompile-report/report/MPServerProvider.{App,Core}.dll/**/*.md`
   — per-type decompiled method evidence (call edges, string literals,
   opcode-level bodies) for business logic.

## Data files

`../data/providers/json/*.json` and `../data/settings.json` are the real
game catalog / settings shape — reuse them as-is, don't re-derive. Every
`start_command`/`arguments` template in these files uses exactly these
placeholders: `{java}`, `{ram_min_mb}`, `{ram_max_mb}`, `{server_file}`,
`{server_name}`, `{server_port}`, `{server_root}` — `ServerProcessService
.BuildCommandAsync` is the single place responsible for substituting all of
them; if a server won't launch, check there first.
