# UI Technology — VERDICT

**The app's UI is 100% native WPF, built from compiled XAML (BAML). It is NOT a web-hosted UI.**

Evidence:
- `MPServerProvider.App.dll` carries one `ManifestResource`: `MPServerProvider.App.g.resources`, the standard WPF resource blob. Loaded via reflection (`System.Resources.ResourceReader`), it contains exactly 16 BAML entries + 1 image:
  - `app.baml` (App.xaml — Application entry)
  - `themes/theme.baml` (shared ResourceDictionary — styles/brushes/templates)
  - `views/mainwindow.baml`
  - `views/splashwindow.baml`
  - `views/homepage.baml`
  - `views/librarypage.baml`
  - `views/myserverspage.baml`
  - `views/serverdetailview.baml`
  - `views/modspage.baml`
  - `views/settingspage.baml`
  - `views/addcustomgamedialog.baml`
  - `views/addfirewallruledialog.baml`
  - `views/installserverdialog.baml`
  - `views/manualinstalldialog.baml`
  - `views/licensekeydialog.baml`
  - `views/tunneldialog.baml`
  - `assets/logo.png`
- All 16 BAML entries were **fully decompiled back to readable XAML** in this pass (see `BAML_STRUCTURE.md` / `baml_xaml/*.xaml`) using the real WPF runtime's internal `System.Windows.Baml2006.Baml2006Reader` (via reflection) piped into `System.Xaml.XamlXmlWriter`. This is not approximation — it is the actual BAML node stream re-serialized to XML, so element tree, `x:Name`s, resource keys, bindings, styles and markup extensions are recovered with high fidelity (some `StaticResource`/converter markup extensions round-trip as their CLR type name rather than the original `{StaticResource Foo}` shorthand text — cosmetic only, the resource key is still present as a child node/attribute in the raw XML).
- `data\` on disk contains **no HTML/JS/CSS UI bundle belonging to this app.** The only HTML/JS/CSS tree found (`data\tools\steamcmd\siteserverui\`) belongs to the bundled third-party **SteamCMD** tool (its own optional web login UI), not to MP Server Provider's shell.
- **WebView2 is used in exactly one place**: `views/serverdetailview.baml` declares `<wv2:WebView2 x:Name="TxAdminWeb" .../>` inside `ServerDetailView`. This is almost certainly used to embed FiveM/RedM's **txAdmin** web admin panel for a running Cfx-based server, not to render the app's own UI. Every other BAML file has zero WebView2/CoreWebView2 references.

## Rewrite implication

A Rust rewrite should target a native Rust GUI toolkit (e.g. `egui`/`iced`/`slint` or a `windows-rs` + XAML-equivalent layer) and re-implement each of the 16 screens/dialogs from the recovered XAML structure in `baml_xaml/`, not build a web app. The single WebView2 control can be replicated with Rust's `webview2-com`/`wry` for the txAdmin panel embed only.
