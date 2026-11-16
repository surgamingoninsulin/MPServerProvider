# Native / P-Invoke Boundary Scan

Ran `inspect_managed_native_boundaries` on both assemblies.

## MPServerProvider.App.dll
**Zero P/Invoke.** `module_ref_count=0`, `pinvoke_import_count=0`. 100% managed, no native imports.

## MPServerProvider.Core.dll
**8 P/Invoke imports across 3 native DLLs** — all concentrated in the de-elevation launcher
(`DeElevatedLauncher`/`DeElevatedProcess`, per the earlier report). This is the classic
"find the unelevated shell's token and spawn a process with it" pattern, used so the app can
run elevated (needed for firewall-rule edits) while still launching game-server child processes
unelevated.

| Import | From | Purpose in this pattern |
|---|---|---|
| `GetShellWindow` | user32.dll | Find the desktop shell (Explorer) window |
| `GetWindowThreadProcessId` | user32.dll | Get Explorer's PID from that window |
| `OpenProcess` | kernel32.dll | Open a handle to the Explorer process |
| `OpenProcessToken` | advapi32.dll | Get Explorer's (unelevated) access token |
| `DuplicateTokenEx` | advapi32.dll | Duplicate that token for use in process creation |
| `CreateProcessWithTokenW` | advapi32.dll | Primary: launch a new process using the duplicated unelevated token |
| `CreateProcessAsUser` | advapi32.dll | Fallback/alternate path to the same end using a user token |
| `CloseHandle` | kernel32.dll | Cleanup |

All are declared with standard `winapi` calling convention, `SetLastError=true` on the kernel32/advapi32 ones. No custom native DLLs, no unmanaged code bundled with the app — these are all stock Win32 APIs.

## Rust port guidance

Map 1:1 onto `windows-rs`:
- `windows::Win32::UI::WindowsAndMessaging::GetShellWindow`
- `windows::Win32::UI::WindowsAndMessaging::GetWindowThreadProcessId`
- `windows::Win32::System::Threading::OpenProcess`
- `windows::Win32::Security::OpenProcessToken`
- `windows::Win32::Security::DuplicateTokenEx`
- `windows::Win32::System::Threading::CreateProcessWithTokenW`
- `windows::Win32::System::Threading::CreateProcessAsUser`
- `windows::Win32::Foundation::CloseHandle`

This is a well-known, straightforward Rust port (same functions, same arguments); no behavioral guesswork needed.
