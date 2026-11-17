namespace MPServerProvider.Core.Models;

public sealed record ScenarioListing(
    int Id,
    string Slug,
    string Title,
    string Author,
    string? ThumbnailUrl,
    string ViewUrl);
