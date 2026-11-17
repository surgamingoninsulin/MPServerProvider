namespace MPServerProvider.Core.Models;

public sealed record VsModDbProject(
    string ModIdStr,
    string Title,
    string Summary,
    string? LogoUrl,
    long Downloads);

public sealed record VsModDbRelease(
    string Filename,
    string Url,
    string ModVersion,
    IReadOnlyList<string> GameVersions);

/// <summary>The newest release's version, and whether that newest release declares support for
/// the server's Vintage Story version (null when the server's version isn't known).</summary>
public sealed record VsModCompatInfo(string? LatestVersion, bool? ConfirmedForServerVersion);
