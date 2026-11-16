using System.Text.Json.Nodes;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Providers;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public sealed class GameSetupHelperTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), $"mpsp-setup-test-{Guid.NewGuid():N}");

    public GameSetupHelperTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch { }
    }

    // ---- Hytale Downloader output --------------------------------------------------------

    [Theory]
    [InlineData("Please visit https://accounts.hytale.com/device?user_code=ABCD-EFGH to log in", "https://accounts.hytale.com/device?user_code=ABCD-EFGH")]
    [InlineData("Open https://oauth.accounts.hytale.com/oauth2/device/verify.", "https://oauth.accounts.hytale.com/oauth2/device/verify")]
    [InlineData("Downloading from https://downloader.hytale.com/x.zip", null)]
    [InlineData("Visit https://evil.example.com/hytale.com/device", null)]
    [InlineData("Visit http://accounts.hytale.com/device", null)]
    public void Only_https_hytale_sign_in_links_are_passed_on(string line, string? expected)
        => Assert.Equal(expected, HytaleProvider.SignInLink(line));

    [Theory]
    [InlineData("Enter code: ABCD-EFGH", "ABCD-EFGH")]
    [InlineData("https://accounts.hytale.com/device?user_code=WXYZ1234", "WXYZ1234")]
    [InlineData("exit code 1", null)]
    public void Device_code_is_read_from_the_downloader(string line, string? expected)
        => Assert.Equal(expected, HytaleProvider.DeviceCode(line));

    [Fact]
    public void Download_percent_is_read()
    {
        Assert.Equal(42.5, HytaleProvider.Percent("[=====     ] 42.5% (1.2 GB / 2.8 GB)"));
        Assert.Null(HytaleProvider.Percent("no progress here"));
    }

    [Fact]
    public void Finds_the_windows_downloader_exe()
    {
        File.WriteAllText(Path.Combine(_dir, "hytale-downloader-linux-amd64"), "");
        File.WriteAllText(Path.Combine(_dir, "hytale-downloader-windows-amd64.exe"), "");
        Assert.EndsWith("hytale-downloader-windows-amd64.exe", HytaleProvider.FindDownloaderExe(_dir));
    }

    // ---- .NET runtime ---------------------------------------------------------------------

    [Fact]
    public void Picks_the_latest_windows_x64_runtime_zip_from_microsofts_release_list()
    {
        const string json = """
            {
              "latest-runtime": "6.0.36",
              "releases": [
                { "runtime": { "version": "6.0.36", "files": [
                    { "name": "dotnet-runtime-linux-x64.tar.gz", "url": "https://builds.dotnet.microsoft.com/a.tar.gz", "hash": "aa" },
                    { "name": "dotnet-runtime-win-x64.zip", "url": "https://builds.dotnet.microsoft.com/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-win-x64.zip", "hash": "ABCDEF" } ] } },
                { "runtime": { "version": "6.0.35", "files": [
                    { "name": "dotnet-runtime-win-x64.zip", "url": "https://builds.dotnet.microsoft.com/old.zip", "hash": "11" } ] } }
              ]
            }
            """;
        var latest = DotNetRuntimeProvider.ParseLatestRuntime(json)!.Value;
        Assert.Equal("6.0.36", latest.Version);
        Assert.EndsWith("dotnet-runtime-6.0.36-win-x64.zip", latest.Url);
        Assert.Equal("ABCDEF", latest.Sha512);
    }

    [Fact]
    public void Refuses_a_runtime_url_off_microsoft_hosts()
    {
        const string json = """
            { "latest-runtime": "6.0.36", "releases": [ { "runtime": { "version": "6.0.36", "files": [
              { "name": "dotnet-runtime-win-x64.zip", "url": "https://example.com/dotnet.zip", "hash": "AB" } ] } } ] }
            """;
        Assert.Null(DotNetRuntimeProvider.ParseLatestRuntime(json));
    }

    [Fact]
    public void Reads_installed_runtimes()
    {
        const string output = "Microsoft.AspNetCore.App 8.0.10 [C:\\x]\r\nMicrosoft.NETCore.App 6.0.36 [C:\\x]\r\nMicrosoft.NETCore.App 8.0.10 [C:\\x]\r\n";
        Assert.True(DotNetRuntimeProvider.ListsRuntime(output, 6));
        Assert.False(DotNetRuntimeProvider.ListsRuntime(output, 7));
    }

    [Fact]
    public void Dotnet_placeholder_is_substituted()
    {
        var server = new ServerInstance { ServerRoot = "C:\\srv" };
        Assert.Equal("C:\\tools\\dotnet.exe \"C:\\srv\\a.dll\"",
            ServerProcessService.SubstitutePlaceholders("{dotnet} \"{server_root}\\a.dll\"", server, null, "C:\\tools\\dotnet.exe"));
    }

    // ---- Creativerse world ----------------------------------------------------------------

    [Fact]
    public void Creativerse_world_is_written_public_with_owner_and_port()
    {
        Assert.True(CreativerseWorld.NeedsSetup("mpserver_world", _dir));

        CreativerseWorld.Write("mpserver_world", "76561198000000001", 26910, "Our World", "tpl_islands", _dir);

        Assert.False(CreativerseWorld.NeedsSetup("mpserver_world", _dir));
        var config = JsonNode.Parse(File.ReadAllText(CreativerseWorld.ConfigPath("mpserver_world", _dir)))!;
        Assert.Equal(26910, config["GamePort"]!.GetValue<int>());
        Assert.False(config["PrivateServer"]!.GetValue<bool>());
        Assert.Equal("76561198000000001", config["WorldOwnerSteamId"]!.ToString());
        Assert.Equal("tpl_islands", config["TemplateKey"]!.ToString());
        Assert.Equal("Our World", config["WorldName"]!.ToString());
    }

    [Fact]
    public void Existing_creativerse_world_keeps_its_other_settings()
    {
        var path = CreativerseWorld.ConfigPath("mpserver_world", _dir);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, """{ "PrivateServer": true, "WorldOwnerSteamId": null, "WorldName": "Old", "TemplateKey": "tpl_a", "UsePlayerAccessFile": true }""");
        Assert.True(CreativerseWorld.NeedsSetup("mpserver_world", _dir));
        Assert.Contains("tpl_a", CreativerseWorld.KnownTemplateKeys(_dir));

        CreativerseWorld.Write("mpserver_world", "76561198000000001", 26900, "New", null, _dir);

        var config = JsonNode.Parse(File.ReadAllText(path))!;
        Assert.False(config["PrivateServer"]!.GetValue<bool>());
        Assert.Equal("Old", config["WorldName"]!.ToString());
        Assert.Equal("tpl_a", config["TemplateKey"]!.ToString());
        Assert.True(config["UsePlayerAccessFile"]!.GetValue<bool>());
    }

    [Theory]
    [InlineData("76561198000000001", true)]
    [InlineData("7656119800000000", false)]
    [InlineData("12345678901234567", false)]
    [InlineData("STEAM_0:1:123", false)]
    public void SteamId64_is_validated(string value, bool valid)
        => Assert.Equal(valid, CreativerseWorld.IsSteamId64(value));

    [Fact]
    public void Creativerse_world_key_comes_from_the_launch_arguments()
    {
        var game = BuiltInGames.All().Single(g => g.Id == CreativerseWorld.GameId);
        Assert.Equal("mpserver_world", CreativerseWorld.WorldKey(new ServerInstance(), game));
        Assert.Equal("my_world", CreativerseWorld.WorldKey(new ServerInstance { StartCommand = "-worldId=my_world -forceIp=0.0.0.0" }, game));
    }

    // ---- Don't Starve Together ------------------------------------------------------------

    [Fact]
    public void Dst_token_save_writes_token_and_default_cluster_files_once()
    {
        Assert.Null(DstCluster.ReadToken(_dir));
        const string token = "pds-g^KU_abcdefgh^abcdefghijklmnopqrstuvwxyz0123456789=";

        DstCluster.SaveToken(_dir, "  " + token + "\r\n", "My \"DST\" Server", 10999);

        Assert.Equal(token, DstCluster.ReadToken(_dir));
        var cluster = DstCluster.ClusterDir(_dir);
        Assert.Contains("cluster_name = My \"DST\" Server", File.ReadAllText(Path.Combine(cluster, "cluster.ini")));
        Assert.Contains("server_port = 10999", File.ReadAllText(Path.Combine(cluster, "Master", "server.ini")));

        File.WriteAllText(Path.Combine(cluster, "cluster.ini"), "edited");
        DstCluster.SaveToken(_dir, token, "Other", 11000);
        Assert.Equal("edited", File.ReadAllText(Path.Combine(cluster, "cluster.ini")));
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("pds-g^KU_abc def^with spaces in the middle")]
    public void Dst_rejects_things_that_arent_tokens(string token)
        => Assert.Throws<ArgumentException>(() => DstCluster.SaveToken(_dir, token, "x", 10999));

    [Fact]
    public void Dst_launch_keeps_the_cluster_inside_the_server_folder()
    {
        var dst = BuiltInGames.All().Single(g => g.Id == DstCluster.GameId);
        Assert.Contains("-persistent_storage_root \"{server_root}\\data\"", dst.Runtime.Arguments);
        Assert.Contains("-conf_dir DoNotStarveTogether -cluster Cluster_1", dst.Runtime.Arguments);
    }
}
