using System.Text.Json;
using JsonHelper = MPServerProvider.Core.Json.Json;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

/// <summary>
/// Tests for the optional <see cref="GameDefinition.Details"/> block (Game Detail page content):
/// JSON shape/round-trip, backwards compatibility with manifests that predate it, consistency
/// between the shipped <c>data/providers/json</c> files and <see cref="BuiltInGames"/>, and
/// <see cref="GameLibraryService"/> back-filling it into existing on-disk built-in copies.
/// </summary>
public class GameDetailsTests
{
    private static AppPaths TempPaths()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"mpsp-details-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        return AppPaths.ForRoot(dir);
    }

    private static GameDetails SampleDetails() => new()
    {
        About = "Para one.\n\nPara two.",
        Developer = "Dev",
        Publisher = "Pub",
        ReleaseDate = "Feb 8, 2018",
        Website = "https://example.com",
        StoreUrl = "https://store.example.com/app/1",
        DocsUrl = "https://docs.example.com/server",
        MaxPlayers = "Up to 10 players",
        Features = new() { "Feature A", "Feature B" },
        ServerRequirements = new ServerRequirementsSpec { MinRamMb = 2048, RecommendedRamMb = 4096, DiskGb = 15, Notes = "SSD preferred" },
        Ports = new()
        {
            new GamePortInfo { Port = "28015", Protocol = "UDP", Purpose = "Game" },
            new GamePortInfo { Port = "26900-26902", Protocol = "TCP+UDP", Purpose = "Range" },
        },
        SetupTips = new() { "Tip 1" },
        Screenshots = new() { "https://cdn.example.com/1.jpg" },
    };

    [Fact]
    public void Details_round_trip_with_snake_case_keys()
    {
        var game = new GameDefinition { Id = "x", Name = "X", Details = SampleDetails() };

        var json = JsonHelper.Serialize(game);
        using (var doc = JsonDocument.Parse(json))
        {
            var details = doc.RootElement.GetProperty("details");
            Assert.True(details.TryGetProperty("release_date", out _));
            Assert.True(details.TryGetProperty("store_url", out _));
            Assert.True(details.TryGetProperty("docs_url", out _));
            Assert.True(details.TryGetProperty("max_players", out _));
            Assert.True(details.TryGetProperty("setup_tips", out _));
            Assert.Equal(2048, details.GetProperty("server_requirements").GetProperty("min_ram_mb").GetInt32());
            Assert.Equal(4096, details.GetProperty("server_requirements").GetProperty("recommended_ram_mb").GetInt32());
            Assert.Equal(15, details.GetProperty("server_requirements").GetProperty("disk_gb").GetInt32());
            Assert.Equal("26900-26902", details.GetProperty("ports")[1].GetProperty("port").GetString());
        }

        var back = JsonHelper.Deserialize<GameDefinition>(json)!;
        Assert.NotNull(back.Details);
        Assert.Equal(JsonHelper.Serialize(game.Details), JsonHelper.Serialize(back.Details));
        Assert.Equal("Para one.\n\nPara two.", back.Details!.About);
        Assert.Equal("TCP+UDP", back.Details.Ports[1].Protocol);
        Assert.Equal("SSD preferred", back.Details.ServerRequirements!.Notes);
    }

    [Fact]
    public void Manifest_without_details_still_loads_and_omits_the_key()
    {
        const string legacy = """
            { "manifest_version": 1, "id": "old", "name": "Old Game", "description": "d", "genres": ["Sandbox"] }
            """;
        var game = JsonHelper.Deserialize<GameDefinition>(legacy)!;
        Assert.Equal("old", game.Id);
        Assert.Null(game.Details);

        using var doc = JsonDocument.Parse(JsonHelper.Serialize(game));
        Assert.False(doc.RootElement.TryGetProperty("details", out _));
    }

    [Fact]
    public void Partial_details_leave_missing_fields_null_or_empty()
    {
        const string partial = """
            { "id": "p", "name": "P", "details": { "about": "Only about", "server_requirements": { "notes": "n" } } }
            """;
        var details = JsonHelper.Deserialize<GameDefinition>(partial)!.Details!;
        Assert.Equal("Only about", details.About);
        Assert.Null(details.Developer);
        Assert.Null(details.ServerRequirements!.MinRamMb);
        Assert.Empty(details.Ports);
        Assert.Empty(details.Screenshots);
    }

    [Fact]
    public void Every_built_in_game_has_details_with_safe_urls()
    {
        foreach (var game in BuiltInGames.All())
        {
            Assert.NotNull(game.Details);
            Assert.False(string.IsNullOrWhiteSpace(game.Details!.About), game.Id);
            Assert.False(game.Description.StartsWith("INFERRED", StringComparison.Ordinal), game.Id);

            var urls = new[] { game.Details.Website, game.Details.StoreUrl, game.Details.DocsUrl }
                .Where(u => u is not null)
                .Concat(game.Details.Screenshots);
            foreach (var url in urls)
            {
                Assert.True(Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps, $"{game.Id}: {url}");
            }

            foreach (var port in game.Details.Ports)
            {
                Assert.Contains(port.Protocol, new[] { "TCP", "UDP", "TCP+UDP" });
                Assert.Matches(@"^\d+(-\d+)?$", port.Port);
            }
        }
    }

    [Fact]
    public void Shipped_json_details_match_BuiltInGames()
    {
        var builtIns = BuiltInGames.All().ToDictionary(g => g.Id);
        var checkedCount = 0;
        foreach (var path in Directory.EnumerateFiles(TestPaths.ProvidersJsonDir, "*.json"))
        {
            var fromJson = JsonHelper.Deserialize<GameDefinition>(File.ReadAllText(path))!;
            Assert.NotNull(fromJson.Details);
            if (!builtIns.TryGetValue(fromJson.Id, out var builtIn))
            {
                continue; // e.g. Cubic Odyssey: shipped as a data file only, no C# built-in.
            }

            Assert.Equal(JsonHelper.Serialize(builtIn.Details), JsonHelper.Serialize(fromJson.Details));
            Assert.Equal(builtIn.Runtime.Arguments, fromJson.Runtime.Arguments);
            Assert.Equal(builtIn.Runtime.Executable, fromJson.Runtime.Executable);
            Assert.Equal(builtIn.Network.DefaultPort, fromJson.Network.DefaultPort);
            checkedCount++;
        }

        Assert.Equal(13, checkedCount);
    }

    [Fact]
    public void Reload_backfills_details_into_existing_built_in_copy_without_clobbering_edits()
    {
        var paths = TempPaths();
        try
        {
            var stale = BuiltInGames.All().Single(g => g.Id == "rust");
            stale.Details = null;
            stale.Name = "My Renamed Rust";
            stale.Description = "My own description";
            stale.Runtime.Arguments = "-batchmode +server.maxplayers 10";
            File.WriteAllText(Path.Combine(paths.GamesDir, "rust.json"), JsonHelper.Serialize(stale));

            var svc = new GameLibraryService(paths);
            svc.Reload();

            var rust = svc.Find("rust")!;
            Assert.NotNull(rust.Details);
            Assert.Equal(
                JsonHelper.Serialize(BuiltInGames.All().Single(g => g.Id == "rust").Details),
                JsonHelper.Serialize(rust.Details));
            Assert.Equal("My Renamed Rust", rust.Name);
            Assert.Equal("My own description", rust.Description);
            Assert.Equal("-batchmode +server.maxplayers 10", rust.Runtime.Arguments);

            // Persisted, so the next reload (or another process) sees it too.
            var onDisk = JsonHelper.Deserialize<GameDefinition>(File.ReadAllText(Path.Combine(paths.GamesDir, "rust.json")))!;
            Assert.NotNull(onDisk.Details);
        }
        finally
        {
            Directory.Delete(paths.DataRoot, recursive: true);
        }
    }

    [Fact]
    public void Reload_keeps_user_edited_details_and_fills_empty_description()
    {
        var paths = TempPaths();
        try
        {
            var edited = BuiltInGames.All().Single(g => g.Id == "valheim");
            edited.Details = new GameDetails { About = "My notes" };
            edited.Description = "";
            File.WriteAllText(Path.Combine(paths.GamesDir, "valheim.json"), JsonHelper.Serialize(edited));

            var svc = new GameLibraryService(paths);
            svc.Reload();

            var valheim = svc.Find("valheim")!;
            Assert.Equal("My notes", valheim.Details!.About);
            Assert.Equal(BuiltInGames.All().Single(g => g.Id == "valheim").Description, valheim.Description);
        }
        finally
        {
            Directory.Delete(paths.DataRoot, recursive: true);
        }
    }

    [Fact]
    public void Reload_repairs_cfx_and_seven_days_definitions()
    {
        var paths = TempPaths();
        try
        {
            var builtIns = BuiltInGames.All().ToDictionary(g => g.Id);

            var fivem = builtIns["fivem"];
            fivem.Runtime.Arguments = "+exec server.cfg";
            File.WriteAllText(Path.Combine(paths.GamesDir, "fivem.json"), JsonHelper.Serialize(fivem));

            var redm = builtIns["redm"];
            redm.Runtime.Arguments = "+exec server.cfg";
            File.WriteAllText(Path.Combine(paths.GamesDir, "redm.json"), JsonHelper.Serialize(redm));

            var sdtd = builtIns["7-days-to-die-dedicated-server"];
            sdtd.Network.DefaultPort = 27015;
            File.WriteAllText(Path.Combine(paths.GamesDir, "7-days-to-die-dedicated-server.json"), JsonHelper.Serialize(sdtd));

            var svc = new GameLibraryService(paths);
            svc.Reload();
            svc.Reload(); // idempotent

            Assert.Equal(string.Empty, svc.Find("fivem")!.Runtime.Arguments);
            Assert.Equal("+set gamename rdr3", svc.Find("redm")!.Runtime.Arguments);
            Assert.Equal(26900, svc.Find("7-days-to-die-dedicated-server")!.Network.DefaultPort);
        }
        finally
        {
            Directory.Delete(paths.DataRoot, recursive: true);
        }
    }

    [Fact]
    public void Reload_replaces_legacy_inferred_openrct2_description()
    {
        var paths = TempPaths();
        try
        {
            var orct = BuiltInGames.All().Single(g => g.Id == "openrct2");
            orct.Description = "INFERRED: OpenRCT2, reconstructed from a single artwork-URL string literal.";
            File.WriteAllText(Path.Combine(paths.GamesDir, "openrct2.json"), JsonHelper.Serialize(orct));

            var svc = new GameLibraryService(paths);
            svc.Reload();

            Assert.Equal(BuiltInGames.All().Single(g => g.Id == "openrct2").Description, svc.Find("openrct2")!.Description);
        }
        finally
        {
            Directory.Delete(paths.DataRoot, recursive: true);
        }
    }
}
