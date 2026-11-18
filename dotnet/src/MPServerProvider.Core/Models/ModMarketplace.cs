namespace MPServerProvider.Core.Models;

public enum ModSourceKind
{
    CurseForge,
    Thunderstore,
    ModIo,
}

/// <summary>One mod/project returned by a marketplace search.</summary>
public sealed record ModListing(
    ModSourceKind Source,
    string Id,
    string Name,
    string? Author,
    string? Summary,
    string? ThumbnailUrl,
    string? WebsiteUrl,
    long? Downloads);

/// <summary>One downloadable file/version of a <see cref="ModListing"/>.</summary>
public sealed record ModFileOption(
    string Id,
    string DisplayName,
    string? GameVersion,
    DateTimeOffset? Published,
    long? SizeBytes);

/// <summary>The games and marketplaces an install can come from.</summary>
public sealed record GameModSource(ModSourceKind Kind, bool Available, string? UnavailableReason = null);
