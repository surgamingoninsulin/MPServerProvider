namespace MPServerProvider.Core.Models;

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
