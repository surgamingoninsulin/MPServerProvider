using System.Text.Json;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class VsModDbServiceTests
{
    // A trimmed excerpt of a real mods.vintagestory.at/api/mods response (three hits, one
    // client-only).
    private const string SearchResponse = """
        {
          "statuscode": "200",
          "mods": [
            {
              "modid": 12903,
              "name": "Cottage Industry",
              "summary": "Real medieval cottage industry, from rollable barrels to period machines.",
              "modidstrs": ["cottageindustry"],
              "side": "both",
              "downloads": 12345,
              "logo": "https://moddbcdn.vintagestory.at/cottageindustry.png"
            },
            {
              "modid": 13543,
              "name": "VintageVR (VVR)",
              "summary": "Play Vintage Story in VR with SteamVR.",
              "modidstrs": ["vintagevr"],
              "side": "client",
              "downloads": 900
            },
            {
              "modid": 13548,
              "name": "Industrial Story - Fixes",
              "summary": "A patch that fixes two minor but annoying issues.",
              "modidstrs": ["zindustrialstoryfixes"],
              "side": "server",
              "downloads": 6
            }
          ]
        }
        """;

    // A trimmed excerpt of a real mods.vintagestory.at/api/mod/cottageindustry response.
    private const string ModResponse = """
        {
          "mod": {
            "modid": 12903,
            "name": "Cottage Industry",
            "releases": [
              {
                "releaseid": 65940,
                "mainfile": "https://moddbcdn.vintagestory.at/cottageindustry_1.2._hash.zip?dl=cottageindustry_1.2.3.zip",
                "filename": "cottageindustry_1.2.3.zip",
                "modversion": "1.2.3",
                "tags": ["1.22.5", "1.22.6", "1.22.7"]
              },
              {
                "releaseid": 60000,
                "mainfile": "https://moddbcdn.vintagestory.at/cottageindustry_1.1._hash.zip?dl=cottageindustry_1.1.0.zip",
                "filename": "cottageindustry_1.1.0.zip",
                "modversion": "1.1.0",
                "tags": ["1.21.0", "1.21.1"]
              }
            ]
          }
        }
        """;

    [Fact]
    public void Parses_search_hits_and_skips_client_only_mods()
    {
        using var doc = JsonDocument.Parse(SearchResponse);
        var results = VsModDbService.ParseSearchResults(doc.RootElement, limit: 30);

        Assert.Equal(2, results.Count);
        Assert.Equal("cottageindustry", results[0].ModIdStr);
        Assert.Equal("Cottage Industry", results[0].Title);
        Assert.Equal(12345, results[0].Downloads);
        Assert.Equal("zindustrialstoryfixes", results[1].ModIdStr);
        Assert.DoesNotContain(results, r => r.ModIdStr == "vintagevr");
    }

    [Fact]
    public void Respects_the_limit()
    {
        using var doc = JsonDocument.Parse(SearchResponse);
        var results = VsModDbService.ParseSearchResults(doc.RootElement, limit: 1);
        Assert.Single(results);
    }

    [Fact]
    public void Picks_the_newest_release_that_supports_the_server_version()
    {
        using var doc = JsonDocument.Parse(ModResponse);
        var release = VsModDbService.PickRelease(doc.RootElement, "1.21.0");

        Assert.NotNull(release);
        Assert.Equal("1.1.0", release!.ModVersion);
        Assert.Equal("cottageindustry_1.1.0.zip", release.Filename);
    }

    [Fact]
    public void Falls_back_to_the_newest_release_when_no_version_matches()
    {
        using var doc = JsonDocument.Parse(ModResponse);
        var release = VsModDbService.PickRelease(doc.RootElement, "1.19.0");

        Assert.NotNull(release);
        Assert.Equal("1.2.3", release!.ModVersion);
    }

    [Fact]
    public void Ignores_the_version_filter_when_none_is_given()
    {
        using var doc = JsonDocument.Parse(ModResponse);
        var release = VsModDbService.PickRelease(doc.RootElement, null);

        Assert.NotNull(release);
        Assert.Equal("1.2.3", release!.ModVersion);
    }

    [Fact]
    public void No_releases_returns_null()
    {
        using var doc = JsonDocument.Parse("""{ "mod": { "releases": [] } }""");
        Assert.Null(VsModDbService.PickRelease(doc.RootElement, null));
    }
}
