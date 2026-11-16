using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.NewsService</c> (decompile token 0x02000010).
/// <para>
/// The decompiled type is strikingly small: <c>Load()</c> just calls <c>BuiltInNews.All()</c> and
/// orders it <c>OrderByDescending</c> (by <c>Date</c>, confirmed by the lambda consuming
/// <c>NewsItem.get_Date</c> via the call graph). There is <b>no</b> HTTP/<c>DownloadService</c>
/// call anywhere in this type's call edges — contrary to what one might assume ("fetches news,
/// falls back to BuiltInNews"), the decompiled <c>NewsService</c> never fetches anything remote
/// at all; it's purely a thin, always-available wrapper over the embedded built-in feed
/// (<c>.ctor()</c> is a no-op base-class call). This restores the already-verified Rust port's
/// same finding rather than re-deriving it: there is no "real endpoint" to recover from string
/// evidence, because none exists in this type.
/// </para>
/// </summary>
public class NewsService
{
    /// <summary>Port of <c>Load() -&gt; List&lt;NewsItem&gt;</c>: the built-in feed, newest first.</summary>
    public List<NewsItem> Load()
        => BuiltInNews.All()
            .OrderByDescending(item => item.Date)
            .ToList();
}
