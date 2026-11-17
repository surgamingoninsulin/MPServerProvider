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
