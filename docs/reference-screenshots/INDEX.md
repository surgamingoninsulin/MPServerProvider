# Reference screenshots (real original app, user-supplied)

These are screenshots of the ORIGINAL MP Server Provider app actually
running, supplied by the user on 2026-11-14 after reviewing the first
(Rust/Slint) rewrite attempt and finding it didn't match the real visual
design. They are the ground truth for colors, layout, and interaction
patterns that the BAML-recovered XAML (`docs/decompile-report/report/baml_xaml/`)
could not fully capture (deferred binary resource blocks).

| File | Screen | Notes |
|------|--------|-------|
| `1.png` | Home | Hero banner ("MP Server Provider" logo text, gradient white→accent-blue), Manage Servers / Browse Games buttons, Uptime/Total Players stat tiles, Mods/My Servers shortcut tiles, **sliding news carousel** at the bottom (horizontal scrollbar visible — auto-slides per `AppSettings.news_auto_scroll`, see `SettingsPage`'s "Slide the Home news cards automatically" toggle) |
| `2.png` | Library | Search box, genre filter pills ("All Games" pill selected in accent blue), game card grid with REAL cover art per card (not flat tiles), "+ Add Custom Game" button |
| `3.png` | Mods | Game filter dropdown + Filters sidebar (checkboxes per game), "Import from Folder" button, mod list area |
| `4.png` | My Servers → Server Detail (Cubic Odyssey) | Console tab, Start/Stop/Restart/Open Folder/Export buttons, "Connect to Tunnel" / "Import Server Config" buttons in left sidebar |
| `5.png` | My Servers → Server Detail, Mods tab | "This game does not support mods" empty state |
| `6.png` | Settings (top) | Storage Management (disk usage bar, game files, installation drive picker), Startup & Behavior checkboxes |
| `7.png` | Settings (scrolled) | System Resources (CPU/RAM live tiles), System Specifications (CPU/GPU/RAM/OS), Firewall Rules list with green wifi icon per rule + "Add Rule" button |
| `8.png` | My Servers → Tunnel dialog | "Connect to Tunnel" modal: UPnP Port Forward / playit.gg Agent tabs, explanatory text, Remove Mapping / Forward Port buttons |
| `9.png` | Splash screen | Full background art (dark pirate/adventure-themed photo — likely a random/rotating background image, not a fixed brand asset), glowing blue ring around a game-controller glyph, "Getting things ready…" status text + percentage progress bar |
| `10.png` | Splash screen, 100% | Same as above, "You are up to date. Finalizing..." at 100% |

## Sampled colors (see `dotnet/src/MPServerProvider.App/Themes/Theme.xaml`)

Pixel-sampled directly from these PNGs via a PowerShell/System.Drawing probe
(coordinates documented in the session transcript, not repeated here):

- **Brand accent**: `#4F6BFF` — the "PROVIDER" word in the logo, the
  "Manage Servers" button fill, the splash ring's glow.
- **Nav-selected pill**: `#1B2B66` — darker navy, used for the active
  sidebar nav item's background.
- **Panel/card background**: `#0D1220` — stat tiles, settings cards, the
  tunnel dialog's tab strip.
- **Root window background**: `#05070D` — near-black.

Success/warning/danger accent colors were NOT clearly sampled from these
images (no unambiguous reference pixel found) and use placeholder
Fluent-dark-theme conventions in `Theme.xaml` — revisit if a clearer
reference turns up (e.g. an error toast or a stopped-server status dot).
