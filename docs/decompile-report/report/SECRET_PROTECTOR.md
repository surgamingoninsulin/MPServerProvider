# SecretProtector (MPServerProvider.Core.Services.SecretProtector)

**Algorithm: plain Windows DPAPI, with a static entropy string. No custom crypto.**

```
static byte[] Entropy = Encoding.UTF8.GetBytes("MPServerProvider.v1");  // .cctor, token 0x0600007c

string Protect(string plaintext):
    if string.IsNullOrEmpty(plaintext) -> return plaintext (or empty) unchanged  [brfalse.s guard]
    bytes = Encoding.UTF8.GetBytes(plaintext)
    protected = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser)  // scope inferred default; no explicit LocalMachine literal observed
    return Convert.ToBase64String(protected)

string Unprotect(string base64):
    if string.IsNullOrEmpty(base64) -> return unchanged
    bytes = Convert.FromBase64String(base64)
    plaintext_bytes = ProtectedData.Unprotect(bytes, Entropy, DataProtectionScope.CurrentUser)
    return Encoding.UTF8.GetString(plaintext_bytes)
```

- Used to encrypt the stored license key at rest (paired with `LicenseKeyDialog`/`Views.LicenseKeyDialog`).
- DPAPI-protected blobs are tied to the **Windows user account** (and machine, unless `LocalMachine` scope) that created them — they will NOT decrypt on another machine/user, and will NOT decrypt from a Rust binary using a different crypto scheme.

## Rust port guidance

Two options, in order of recommendation:
1. **Re-key on first run of the Rust rewrite.** Since this only protects a local license key (not a shared secret or network protocol), it's safe and simplest to detect an undecryptable/missing blob and re-prompt the user for their license key once, then re-protect it with the Rust app's own scheme. No need to byte-for-byte match DPAPI.
2. **If exact DPAPI compatibility is wanted** (so an existing encrypted blob from the C# app still decrypts after switching to the Rust build): call the same Win32 DPAPI (`CryptProtectData`/`CryptUnprotectData`) via `windows-rs` (`windows::Win32::Security::Cryptography::{CryptProtectData, CryptUnprotectData}`), passing the same entropy bytes (`"MPServerProvider.v1"` UTF-8) and `CurrentUser` scope (no `CRYPTPROTECT_LOCAL_MACHINE` flag). This reproduces the exact same algorithm since DPAPI is OS-level, not .NET-level — fully portable to Rust with identical on-disk results.
