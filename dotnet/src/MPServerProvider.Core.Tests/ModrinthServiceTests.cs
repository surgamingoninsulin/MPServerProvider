using System.Text.Json;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class ModrinthServiceTests
{
    // A trimmed excerpt of a real api.modrinth.com/v2/search response (two hits).
    private const string SearchResponse = """
        {
          "hits": [
            {
              "project_id": "1u6JkXh5",
              "slug": "worldedit",
              "title": "WorldEdit",
              "description": "A Minecraft Map Editor that runs in-game!",
              "categories": ["bukkit", "fabric", "paper", "spigot"],
              "downloads": 11938782,
              "icon_url": "https://cdn.modrinth.com/data/1u6JkXh5/icon.webp"
            },
            {
              "project_id": "NSLJJooQ",
              "slug": "worldedit-cui",
              "title": "WorldEdit CUI",
              "description": "A graphical user interface for WorldEdit.",
              "categories": ["fabric", "quilt"],
              "downloads": 500000
            }
          ]
        }
        """;

    // A trimmed excerpt of a real api.modrinth.com/v2/project/worldedit/version response.
    private const string VersionsResponse = """
        [
          {
            "id": "YFnqiUlL",
            "version_number": "7.4.6",
            "loaders": ["bukkit", "folia", "paper", "spigot"],
            "files": [
              {
                "url": "https://cdn.modrinth.com/data/1u6JkXh5/versions/YFnqiUlL/worldedit-bukkit-7.4.6.jar",
                "filename": "worldedit-bukkit-7.4.6.jar",
                "primary": true,
                "hashes": { "sha1": "4de34cb32f338a7727f0c1cc4a0f7ebff3fcc3a3" }
              }
            ]
          }
        ]
        """;

    [Fact]
    public void Parses_search_hits()
    {
        using var doc = JsonDocument.Parse(SearchResponse);
        var results = ModrinthService.ParseSearchResults(doc.RootElement);

        Assert.Equal(2, results.Count);
        Assert.Equal("WorldEdit", results[0].Title);
        Assert.Equal("worldedit", results[0].Slug);
        Assert.Equal(11938782, results[0].Downloads);
        Assert.Equal("https://cdn.modrinth.com/data/1u6JkXh5/icon.webp", results[0].IconUrl);
        Assert.Null(results[1].IconUrl);
    }

    [Fact]
    public void Picks_the_primary_file_from_the_newest_matching_version()
    {
        using var doc = JsonDocument.Parse(VersionsResponse);
        var file = ModrinthService.PickVersionFile(doc.RootElement);

        Assert.NotNull(file);
        Assert.Equal("worldedit-bukkit-7.4.6.jar", file!.Filename);
        Assert.Equal("https://cdn.modrinth.com/data/1u6JkXh5/versions/YFnqiUlL/worldedit-bukkit-7.4.6.jar", file.Url);
        Assert.Equal("4de34cb32f338a7727f0c1cc4a0f7ebff3fcc3a3", file.Sha1);
    }

    [Fact]
    public void No_versions_returns_null()
    {
        using var doc = JsonDocument.Parse("[]");
        Assert.Null(ModrinthService.PickVersionFile(doc.RootElement));
    }

    [Theory]
    [InlineData(new string[0], null, "[[\"project_type:mod\"]]")]
    [InlineData(new[] { "paper", "spigot" }, null, "[[\"project_type:mod\"],[\"categories:paper\",\"categories:spigot\"]]")]
    [InlineData(new[] { "fabric" }, "1.21.1", "[[\"project_type:mod\"],[\"categories:fabric\"],[\"versions:1.21.1\"]]")]
    public void Builds_facets(string[] loaders, string? gameVersion, string expected)
        => Assert.Equal(expected, ModrinthService.BuildFacets(loaders, gameVersion));

    [Theory]
    [InlineData(MinecraftPlatform.Fabric, new[] { "fabric" })]
    [InlineData(MinecraftPlatform.Bukkit | MinecraftPlatform.Paper, new[] { "bukkit", "spigot", "paper", "folia" })]
    [InlineData(MinecraftPlatform.Velocity, new[] { "velocity" })]
    [InlineData(MinecraftPlatform.None, new string[0])]
    public void Maps_accepted_platforms_to_modrinth_loaders(MinecraftPlatform accepted, string[] expected)
        => Assert.Equal(expected, ModCompatibility.ModrinthLoaders(accepted));
}
