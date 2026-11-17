using System.Net;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Browses and downloads RCT2 / RCT Classic scenarios from rctgo.com. The listing page is plain
/// server-rendered HTML (no API), so entries are pulled out with a small regex per item rather
/// than a full HTML parser. https://rctgo.com/downloads/scenarios/rct2 is already filtered to the
/// "RCT2 & RCT Classic" game category shown in the site's filter bar.
/// </summary>
public sealed partial class RctgoScenarioService
{
    private const string BaseUrl = "https://rctgo.com";
    private const string ListingPath = "/downloads/scenarios/rct2";

    private static readonly HttpClient Http = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MPServerProvider/1.0 (+https://github.com/surgamingoninsulin/MPServerProvider)");
        return client;
    }

    public async Task<List<ScenarioListing>> SearchAsync(int page, CancellationToken ct = default)
    {
        var url = page <= 1 ? $"{BaseUrl}{ListingPath}" : $"{BaseUrl}{ListingPath}/{page}";
        var html = await Http.GetStringAsync(url, ct).ConfigureAwait(false);
        return Parse(html);
    }

    internal static List<ScenarioListing> Parse(string html)
    {
        var results = new List<ScenarioListing>();
        foreach (Match entry in EntryRegex().Matches(html))
        {
            if (!int.TryParse(entry.Groups["id"].Value, out var id))
            {
                continue;
            }

            // Everything up to the next entry (or end of page) belongs to this card.
            var start = entry.Index;
            var nextStart = EntryRegex().Match(html, start + 1);
            var end = nextStart.Success ? nextStart.Index : html.Length;
            var block = html[start..end];

            var title = TitleRegex().Match(block) is { Success: true } t ? WebUtility.HtmlDecode(t.Groups["title"].Value).Trim() : "Untitled scenario";
            var author = AuthorRegex().Match(block) is { Success: true } a ? WebUtility.HtmlDecode(a.Groups["author"].Value).Trim() : "Unknown";
            var thumb = ThumbRegex().Match(block) is { Success: true } th ? th.Groups["url"].Value : null;

            results.Add(new ScenarioListing(id, entry.Groups["slug"].Value, title, author, thumb, $"{BaseUrl}/downloads/view/{entry.Groups["slug"].Value}.{id}"));
        }

        return results;
    }

    /// <summary>Downloads a scenario's save file, following rctgo.com's redirect to its CDN.</summary>
    public async Task<(byte[] Data, string FileName)> DownloadAsync(int id, CancellationToken ct = default)
    {
        using var response = await Http.GetAsync($"{BaseUrl}/downloads/download/{id}", HttpCompletionOption.ResponseHeadersRead, ct)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var fileName = Path.GetFileName(Uri.UnescapeDataString(response.RequestMessage?.RequestUri?.AbsolutePath ?? string.Empty));
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"scenario-{id}.sc6";
        }

        var data = await response.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);
        return (data, fileName);
    }

    [GeneratedRegex(@"<a href=""/downloads/view/(?<slug>[^""/.]+)\.(?<id>\d+)""")]
    private static partial Regex EntryRegex();

    [GeneratedRegex(@"<span class=""browse__item__title"">(?<title>.*?)</span>", RegexOptions.Singleline)]
    private static partial Regex TitleRegex();

    [GeneratedRegex(@"<span class=""browse__item__author"">by (?<author>.*?)</span>", RegexOptions.Singleline)]
    private static partial Regex AuthorRegex();

    [GeneratedRegex(@"data-src=""(?<url>https://thumb\.rctgo\.com/[^""]+)""")]
    private static partial Regex ThumbRegex();
}
