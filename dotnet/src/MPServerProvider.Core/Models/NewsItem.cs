namespace MPServerProvider.Core.Models;

/// <summary>
/// An in-app news/changelog feed entry.
/// Ported from <c>MPServerProvider.Core.Models.NewsItem</c> (decompile token 0x02000031).
/// No real on-disk sample was available for this type (news items are fetched remotely, not
/// stored under <c>data/</c>); field names are reconstructed from the decompiled property names
/// via the same <c>snake_case</c> convention used throughout <c>MPServerProvider.Core.Json.Json</c>.
/// The ctor's string literal default for <see cref="Tag"/> (<c>"changelog"</c>) is preserved,
/// matching the already-verified Rust port's judgment call.
/// </summary>
public class NewsItem
{
    public string Tag { get; set; } = "changelog";

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string? Image { get; set; }

    public string? GameId { get; set; }

    public string? Link { get; set; }
}
