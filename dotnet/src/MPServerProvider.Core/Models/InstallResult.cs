namespace MPServerProvider.Core.Models;

/// <summary>
/// Outcome of a successful install, used to seed a new <see cref="ServerInstance"/>.
/// Ported from <c>MPServerProvider.Core.Models.InstallResult</c> (decompile token 0x02000035).
/// Like <see cref="InstallRequest"/>, the decompiled setters carry an <c>IsExternalInit</c>
/// modreq, so this is modeled with <c>init</c>-only properties.
/// </summary>
public class InstallResult
{
    public string ServerFile { get; init; } = string.Empty;

    public string? Version { get; init; }

    public string? StartCommand { get; init; }

    public int JavaMajor { get; init; }
}
