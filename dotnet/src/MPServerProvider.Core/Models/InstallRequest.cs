namespace MPServerProvider.Core.Models;

/// <summary>
/// Parameters for installing a new server from a <see cref="GameDefinition"/>.
/// Ported from <c>MPServerProvider.Core.Models.InstallRequest</c> (decompile token 0x02000034).
/// The decompiled setters carry an <c>IsExternalInit</c> modreq (<c>void modreq(177)</c>),
/// indicating the original properties are <c>init</c>-only — this is an immutable payload built
/// once via an object initializer, not mutated afterward.
/// This is an in-process/IPC payload rather than an on-disk manifest, so there is no
/// ground-truth JSON sample; field names follow the same <c>snake_case</c> convention used by
/// every type that *is* ground truth.
/// </summary>
public class InstallRequest
{
    public required GameDefinition Game { get; init; }

    public string ServerName { get; init; } = string.Empty;

    public string? Variant { get; init; }

    public string? Version { get; init; }

    public string TargetDir { get; init; } = string.Empty;

    public int Port { get; init; }

    public int RamMinMb { get; init; }

    public int RamMaxMb { get; init; }

    public string? Description { get; init; }

    public bool EulaAccepted { get; init; }
}
