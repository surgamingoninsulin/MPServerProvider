using System.Net;
using System.Net.Http;
using MPServerProvider.Core.Services.Mods;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class GameModSupportTests
{
    [Theory]
    [InlineData("minecraft-java")]
    [InlineData("hytale")]
    public void CurseForge_is_available_for_wired_up_games(string gameId)
        => Assert.Contains(Core.Models.ModSourceKind.CurseForge, GameModSupport.SourcesFor(gameId));

    [Fact]
    public void Unknown_games_have_no_mod_sources()
        => Assert.Empty(GameModSupport.SourcesFor("some-unmapped-game"));
}

public class CurseForgeModSourceTests
{
    private const string GamesJson = """
        { "data": [ { "id": 432, "name": "Minecraft", "slug": "minecraft" }, { "id": 9999, "name": "Hytale", "slug": "hytale" } ] }
        """;

    private const string SearchJson = """
        { "data": [ { "id": 111, "name": "Sample Mod", "summary": "Does things.", "downloadCount": 4200,
            "logo": { "thumbnailUrl": "https://example.test/logo.png" },
            "links": { "websiteUrl": "https://www.curseforge.com/minecraft/mc-mods/sample-mod" },
            "authors": [ { "name": "SomeDev" } ] } ] }
        """;

    private const string FilesJson = """
        { "data": [
            { "id": 2, "displayName": "Sample Mod 2.0.jar", "fileName": "sample-mod-2.0.jar", "fileDate": "2026-02-01T00:00:00Z", "fileLength": 2048, "downloadUrl": "https://example.test/files/2", "gameVersions": ["1.21"] },
            { "id": 1, "displayName": "Sample Mod 1.0.jar", "fileName": "sample-mod-1.0.jar", "fileDate": "2026-01-01T00:00:00Z", "fileLength": 1024, "downloadUrl": "https://example.test/files/1", "gameVersions": ["1.20"] }
        ] }
        """;

    private const string NoDistributionFileJson = """
        { "data": { "id": 3, "displayName": "Locked.jar", "fileName": "locked.jar", "fileDate": "2026-01-01T00:00:00Z", "fileLength": 10, "downloadUrl": null } }
        """;

    [Fact]
    public async Task Search_resolves_the_game_id_then_returns_listings()
    {
        var handler = new FakeHandler();
        handler.On("https://api.curseforge.com/v1/games?index=0&pageSize=50", GamesJson);
        handler.OnPrefix("https://api.curseforge.com/v1/mods/search", SearchJson);
        var source = new CurseForgeModSource(new HttpClient(handler), () => "test-key");

        var results = await source.SearchAsync("minecraft-java", "sample", 0, CancellationToken.None);

        Assert.Single(results);
        Assert.Equal("111", results[0].Id);
        Assert.Equal("Sample Mod", results[0].Name);
        Assert.Equal("SomeDev", results[0].Author);
        Assert.Equal(4200, results[0].Downloads);
        Assert.Contains("x-api-key", handler.LastRequestHeaders);
    }

    [Fact]
    public async Task Game_id_lookup_pages_past_the_first_50_games_to_find_a_newer_addition()
    {
        // Regression test: CurseForge caps pageSize at 50 and has thousands of games, so a
        // recently added one (like Hytale) can sit on a later page. A lookup that only ever
        // checked page one would silently find nothing and "search" would look broken.
        var firstPage = string.Join(",", Enumerable.Range(1, 50).Select(i => $$"""{ "id": {{i}}, "name": "Filler {{i}}", "slug": "filler-{{i}}" }"""));
        var firstPageJson = $$"""{ "data": [ {{firstPage}} ] }""";
        var secondPageJson = """{ "data": [ { "id": 9999, "name": "Hytale", "slug": "hytale" } ] }""";

        var handler = new FakeHandler();
        handler.On("https://api.curseforge.com/v1/games?index=0&pageSize=50", firstPageJson);
        handler.On("https://api.curseforge.com/v1/games?index=50&pageSize=50", secondPageJson);
        handler.OnPrefix("https://api.curseforge.com/v1/mods/search", SearchJson);
        var source = new CurseForgeModSource(new HttpClient(handler), () => "test-key");

        var results = await source.SearchAsync("hytale", "sample", 0, CancellationToken.None);

        Assert.Single(results);
    }

    [Fact]
    public async Task Search_for_an_unmapped_game_returns_nothing_without_calling_the_api()
    {
        var handler = new FakeHandler();
        var source = new CurseForgeModSource(new HttpClient(handler), () => "test-key");

        var results = await source.SearchAsync("some-unmapped-game", "sample", 0, CancellationToken.None);

        Assert.Empty(results);
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task Files_are_ordered_newest_first()
    {
        var handler = new FakeHandler();
        handler.OnPrefix("https://api.curseforge.com/v1/mods/111/files", FilesJson);
        var source = new CurseForgeModSource(new HttpClient(handler), () => "test-key");

        var files = await source.GetFilesAsync("minecraft-java", "111", CancellationToken.None);

        Assert.Equal(2, files.Count);
        Assert.Equal("2", files[0].Id);
        Assert.Equal("1", files[1].Id);
    }

    [Fact]
    public async Task Download_rejects_files_with_distribution_disabled()
    {
        var handler = new FakeHandler();
        handler.OnPrefix("https://api.curseforge.com/v1/mods/111/files/3", NoDistributionFileJson);
        var source = new CurseForgeModSource(new HttpClient(handler), () => "test-key");

        var dir = Directory.CreateTempSubdirectory().FullName;
        try
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => source.DownloadAsync("minecraft-java", "111", "3", dir, CancellationToken.None));
            Assert.Contains("distribution", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task Download_writes_the_file_to_the_target_directory()
    {
        const string fileJson = """
            { "data": { "id": 1, "displayName": "Sample Mod 1.0.jar", "fileName": "sample-mod-1.0.jar", "fileDate": "2026-01-01T00:00:00Z", "fileLength": 5, "downloadUrl": "https://example.test/files/1" } }
            """;
        var handler = new FakeHandler();
        handler.OnPrefix("https://api.curseforge.com/v1/mods/111/files/1", fileJson);
        handler.OnBytes("https://example.test/files/1", new byte[] { 1, 2, 3, 4, 5 });
        var source = new CurseForgeModSource(new HttpClient(handler), () => "test-key");

        var dir = Directory.CreateTempSubdirectory().FullName;
        try
        {
            var written = await source.DownloadAsync("minecraft-java", "111", "1", dir, CancellationToken.None);
            Assert.Single(written);
            Assert.Equal("sample-mod-1.0.jar", Path.GetFileName(written[0]));
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, await File.ReadAllBytesAsync(written[0]));
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task Unauthorized_response_reports_a_clear_api_key_error()
    {
        var handler = new FakeHandler();
        handler.OnStatus("https://api.curseforge.com/v1/games?index=0&pageSize=50", HttpStatusCode.Unauthorized);
        var source = new CurseForgeModSource(new HttpClient(handler), () => "bad-key");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => source.SearchAsync("minecraft-java", "sample", 0, CancellationToken.None));
        Assert.Contains("API key", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}

public class ThunderstoreModSourceTests
{
    private const string PackagesJson = """
        [
          { "name": "CoolMod", "full_name": "SomeAuthor-CoolMod", "owner": "SomeAuthor", "package_url": "https://thunderstore.io/c/test/p/SomeAuthor/CoolMod/", "is_deprecated": false,
            "versions": [ { "version_number": "1.2.0", "description": "A cool mod.", "icon": "https://example.test/icon.png", "download_url": "https://example.test/dl/1.2.0.zip", "downloads": 500, "file_size": 1024, "date_created": "2026-01-01T00:00:00Z" } ] },
          { "name": "OldMod", "full_name": "SomeAuthor-OldMod", "owner": "SomeAuthor", "package_url": "https://thunderstore.io/c/test/p/SomeAuthor/OldMod/", "is_deprecated": true,
            "versions": [ { "version_number": "0.1.0", "description": "Deprecated.", "download_url": "https://example.test/dl/old.zip", "downloads": 10, "file_size": 10, "date_created": "2025-01-01T00:00:00Z" } ] }
        ]
        """;

    [Fact]
    public async Task Search_excludes_deprecated_packages_and_matches_by_name()
    {
        var handler = new FakeHandler();
        handler.On("https://thunderstore.io/c/test/api/v1/package/", PackagesJson);
        var source = new ThunderstoreModSource(new HttpClient(handler));

        var results = await source.SearchCommunityAsync("test", "cool", 0, CancellationToken.None);

        Assert.Single(results);
        Assert.Equal("SomeAuthor-CoolMod", results[0].Id);
        Assert.Equal("CoolMod", results[0].Name);
        Assert.Equal(500, results[0].Downloads);
    }

    [Fact]
    public async Task Search_with_an_empty_query_returns_all_non_deprecated_packages()
    {
        var handler = new FakeHandler();
        handler.On("https://thunderstore.io/c/test/api/v1/package/", PackagesJson);
        var source = new ThunderstoreModSource(new HttpClient(handler));

        var results = await source.SearchCommunityAsync("test", "", 0, CancellationToken.None);

        Assert.Single(results);
        Assert.DoesNotContain(results, r => r.Id == "SomeAuthor-OldMod");
    }

    [Fact]
    public async Task Download_extracts_the_zip_into_its_own_folder_and_strips_thunderstore_metadata()
    {
        var zipBytes = BuildZip(new Dictionary<string, string>
        {
            ["manifest.json"] = "{}",
            ["icon.png"] = "not really a png",
            ["plugins/CoolMod.dll"] = "fake-dll-bytes",
        });

        var handler = new FakeHandler();
        handler.On("https://thunderstore.io/c/test/api/v1/package/", PackagesJson);
        handler.OnBytes("https://example.test/dl/1.2.0.zip", zipBytes);
        var source = new ThunderstoreModSource(new HttpClient(handler));

        var dir = Directory.CreateTempSubdirectory().FullName;
        try
        {
            var written = await source.DownloadInCommunityAsync("test", "SomeAuthor-CoolMod", "1.2.0", dir, CancellationToken.None);
            var extractDir = Assert.Single(written);
            Assert.True(Directory.Exists(extractDir));
            Assert.False(File.Exists(Path.Combine(extractDir, "manifest.json")));
            Assert.False(File.Exists(Path.Combine(extractDir, "icon.png")));
            Assert.True(File.Exists(Path.Combine(extractDir, "plugins", "CoolMod.dll")));
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    private static byte[] BuildZip(Dictionary<string, string> entries)
    {
        using var stream = new MemoryStream();
        using (var zip = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (name, content) in entries)
            {
                var entry = zip.CreateEntry(name);
                using var writer = new StreamWriter(entry.Open());
                writer.Write(content);
            }
        }
        return stream.ToArray();
    }
}

/// <summary>Routes requests by exact URL or prefix to canned JSON/byte responses; records headers/counts for assertions.</summary>
internal sealed class FakeHandler : HttpMessageHandler
{
    private readonly Dictionary<string, string> _exactJson = new();
    private readonly List<(string Prefix, string Json)> _prefixJson = new();
    private readonly Dictionary<string, byte[]> _bytes = new();
    private readonly Dictionary<string, HttpStatusCode> _status = new();

    public int RequestCount { get; private set; }

    public IEnumerable<string> LastRequestHeaders { get; private set; } = Array.Empty<string>();

    public void On(string url, string json) => _exactJson[url] = json;

    public void OnPrefix(string prefix, string json) => _prefixJson.Add((prefix, json));

    public void OnBytes(string url, byte[] bytes) => _bytes[url] = bytes;

    public void OnStatus(string url, HttpStatusCode status) => _status[url] = status;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestCount++;
        LastRequestHeaders = request.Headers.Select(h => h.Key).ToList();
        var url = request.RequestUri!.ToString();

        if (_status.TryGetValue(url, out var status))
        {
            return Task.FromResult(new HttpResponseMessage(status));
        }
        if (_bytes.TryGetValue(url, out var bytes))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(bytes) });
        }
        if (_exactJson.TryGetValue(url, out var json))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
        }
        var prefixMatch = _prefixJson.FirstOrDefault(p => url.StartsWith(p.Prefix, StringComparison.Ordinal));
        if (prefixMatch.Json is not null)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(prefixMatch.Json) });
        }

        throw new InvalidOperationException($"FakeHandler has no route for {url}");
    }
}
