using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class RctgoScenarioServiceTests
{
    // A trimmed excerpt of rctgo.com/downloads/scenarios/rct2's real markup (two entries back to
    // back), so the regex parser is checked against the site's actual structure.
    private const string SampleHtml = """
        </div><div class="browse browse--downloads"><a href="/downloads/view/spookfest-26.26783" class="browse__item" id="download_26783">
        	<span class="browse__item__image image image--fit image--bottom-adjacent">
        		<img
        			src="/assets/images/placeholders/350x240.gif"
        			data-src="https://thumb.rctgo.com/img/350x240/2026-10/26783/6acf679d744bd.png"
        			data-src-retina="https://thumb.rctgo.com/img/700x480/2026-10/26783/6acf679d744bd.png"
        			alt="Spookfest '26"
        			class="unveil" />
        	</span>
        		<span class="browse__item__info">
        		<span class="browse__item__title">Spookfest '26</span>
        		<span class="browse__item__author">by Shazbo</span>
        	</span>
        </a><a href="/downloads/view/fallout-falls.26772" class="browse__item" id="download_26772">
        	<span class="browse__item__image image image--fit image--bottom-adjacent">
        		<img
        			src="/assets/images/placeholders/350x240.gif"
        			data-src="https://thumb.rctgo.com/img/350x240/2026-10/26772/6ac9c8a3ba4dd.png"
        			alt="Fallout Falls"
        			class="unveil" />
        	</span>
        		<span class="browse__item__info">
        		<span class="browse__item__title">Fallout Falls</span>
        		<span class="browse__item__author">by BusinessMilk</span>
        	</span>
        </a>
        """;

    [Fact]
    public void Parses_each_card_from_the_listing_markup()
    {
        var scenarios = RctgoScenarioService.Parse(SampleHtml);

        Assert.Equal(2, scenarios.Count);

        Assert.Equal(26783, scenarios[0].Id);
        Assert.Equal("spookfest-26", scenarios[0].Slug);
        Assert.Equal("Spookfest '26", scenarios[0].Title);
        Assert.Equal("Shazbo", scenarios[0].Author);
        Assert.Equal("https://thumb.rctgo.com/img/350x240/2026-10/26783/6acf679d744bd.png", scenarios[0].ThumbnailUrl);
        Assert.Equal("https://rctgo.com/downloads/view/spookfest-26.26783", scenarios[0].ViewUrl);

        Assert.Equal(26772, scenarios[1].Id);
        Assert.Equal("Fallout Falls", scenarios[1].Title);
        Assert.Equal("BusinessMilk", scenarios[1].Author);
    }

    [Fact]
    public void Returns_nothing_for_a_page_with_no_entries()
        => Assert.Empty(RctgoScenarioService.Parse("<html><body>No results</body></html>"));
}
