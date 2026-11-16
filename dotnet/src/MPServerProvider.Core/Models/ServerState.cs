namespace MPServerProvider.Core.Models;

/// <summary>
/// Lifecycle state of a running (or not-yet-running) server process.
/// Ported from <c>MPServerProvider.Core.Models.ServerState</c> (decompile token 0x0200003a).
/// Has no on-disk representation (it's an in-memory/IPC status enum, not persisted in
/// <c>server.json</c>), so variant names/order are taken directly from the decompiled enum
/// fields: Stopped, Starting, Running, Stopping, Crashed, Installing. Rendered via
/// <see cref="System.Text.Json.Serialization.JsonStringEnumConverter"/> registered globally in
/// <c>MPServerProvider.Core.Json.Json</c> — note that converter is constructed with no naming
/// policy argument there (confirmed by the decompile's IL), so on net8.0 it serializes using the
/// literal PascalCase member names below, not snake_case (verified empirically by this port's
/// test suite; the earlier Rust port's snake_case assumption for this never-persisted enum was
/// unverified speculation).
/// </summary>
public enum ServerState
{
    Stopped,
    Starting,
    Running,
    Stopping,
    Crashed,
    Installing,
}
