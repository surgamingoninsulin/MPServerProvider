namespace MPServerProvider.Core.Models;

public sealed record ModrinthProject(
    string ProjectId,
    string Slug,
    string Title,
    string Description,
    string? IconUrl,
    long Downloads);

public sealed record ModrinthVersionFile(
    string VersionId,
    string VersionNumber,
    string Filename,
    string Url,
    string? Sha1);
