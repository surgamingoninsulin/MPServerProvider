using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public class NewsService
{
    public List<NewsItem> Load()
        => BuiltInNews.All()
            .OrderByDescending(item => item.Date)
            .ToList();
}
