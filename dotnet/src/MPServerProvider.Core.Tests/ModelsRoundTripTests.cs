using JsonHelper = MPServerProvider.Core.Json.Json;
using MPServerProvider.Core.Models;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class ModelsRoundTripTests
{
    [Fact]
    public void ModItem_round_trips()
    {
        var item = new ModItem
        {
            FileName = "some-mod.jar",
            GameId = "minecraft-java",
            SizeBytes = 123456,
            Imported = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc),
            LibraryPath = "mods/some-mod.jar",
            AssignedServers = new List<string> { "server-a", "server-b" },
        };

        var json = JsonHelper.Serialize(item);
        Assert.Contains("\"file_name\"", json);
        Assert.Contains("\"game_id\"", json);
        Assert.Contains("\"size_bytes\"", json);
        Assert.Contains("\"library_path\"", json);
        Assert.Contains("\"assigned_servers\"", json);

        var roundTripped = JsonHelper.Deserialize<ModItem>(json);
        Assert.NotNull(roundTripped);
        Assert.Equal(item.FileName, roundTripped!.FileName);
        Assert.Equal(item.GameId, roundTripped.GameId);
        Assert.Equal(item.SizeBytes, roundTripped.SizeBytes);
        Assert.Equal(item.Imported, roundTripped.Imported);
        Assert.Equal(item.LibraryPath, roundTripped.LibraryPath);
        Assert.Equal(item.AssignedServers, roundTripped.AssignedServers);
    }

    [Fact]
    public void NewsItem_round_trips_and_defaults_tag_to_changelog()
    {
        var item = new NewsItem
        {
            Title = "New release",
            Summary = "Fixed bugs.",
            Date = new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Utc),
            GameId = "valheim",
            Link = "https://example.com/news/1",
        };

        Assert.Equal("changelog", item.Tag);

        var json = JsonHelper.Serialize(item);
        var roundTripped = JsonHelper.Deserialize<NewsItem>(json);
        Assert.NotNull(roundTripped);
        Assert.Equal(item.Tag, roundTripped!.Tag);
        Assert.Equal(item.Title, roundTripped.Title);
        Assert.Equal(item.Summary, roundTripped.Summary);
        Assert.Equal(item.Date, roundTripped.Date);
        Assert.Equal(item.GameId, roundTripped.GameId);
        Assert.Equal(item.Link, roundTripped.Link);
    }

    [Fact]
    public void InstallProgress_factory_helpers_and_round_trip()
    {
        var indeterminate = InstallProgress.Indeterminate("Downloading...");
        Assert.Equal("Downloading...", indeterminate.Status);
        Assert.Equal(0.0, indeterminate.Percent);

        var at = InstallProgress.At("Extracting...", 42.5);
        Assert.Equal("Extracting...", at.Status);
        Assert.Equal(42.5, at.Percent);

        at.BytesReceived = 1024;
        at.TotalBytes = 2048;

        var json = JsonHelper.Serialize(at);
        var roundTripped = JsonHelper.Deserialize<InstallProgress>(json);
        Assert.NotNull(roundTripped);
        Assert.Equal(at.Status, roundTripped!.Status);
        Assert.Equal(at.Percent, roundTripped.Percent);
        Assert.Equal(at.BytesReceived, roundTripped.BytesReceived);
        Assert.Equal(at.TotalBytes, roundTripped.TotalBytes);
    }

    [Fact]
    public void InstallRequest_round_trips_with_embedded_GameDefinition()
    {
        var request = new InstallRequest
        {
            Game = new GameDefinition { Id = "minecraft-java", Name = "Minecraft: Java Edition" },
            ServerName = "My Server",
            Variant = "paper",
            Version = "1.21",
            TargetDir = "C:\\Servers\\My Server",
            Port = 25565,
            RamMinMb = 1024,
            RamMaxMb = 4096,
            Description = "A test server",
            EulaAccepted = true,
        };

        var json = JsonHelper.Serialize(request);
        var roundTripped = JsonHelper.Deserialize<InstallRequest>(json);
        Assert.NotNull(roundTripped);
        Assert.Equal(request.Game.Id, roundTripped!.Game.Id);
        Assert.Equal(request.ServerName, roundTripped.ServerName);
        Assert.Equal(request.Variant, roundTripped.Variant);
        Assert.Equal(request.Version, roundTripped.Version);
        Assert.Equal(request.TargetDir, roundTripped.TargetDir);
        Assert.Equal(request.Port, roundTripped.Port);
        Assert.Equal(request.RamMinMb, roundTripped.RamMinMb);
        Assert.Equal(request.RamMaxMb, roundTripped.RamMaxMb);
        Assert.Equal(request.Description, roundTripped.Description);
        Assert.Equal(request.EulaAccepted, roundTripped.EulaAccepted);
    }

    [Fact]
    public void InstallResult_round_trips()
    {
        var result = new InstallResult
        {
            ServerFile = "server.jar",
            Version = "1.21",
            StartCommand = "java -jar server.jar",
            JavaMajor = 21,
        };

        var json = JsonHelper.Serialize(result);
        var roundTripped = JsonHelper.Deserialize<InstallResult>(json);
        Assert.NotNull(roundTripped);
        Assert.Equal(result.ServerFile, roundTripped!.ServerFile);
        Assert.Equal(result.Version, roundTripped.Version);
        Assert.Equal(result.StartCommand, roundTripped.StartCommand);
        Assert.Equal(result.JavaMajor, roundTripped.JavaMajor);
    }

    [Fact]
    public void ServerExportData_round_trips()
    {
        var export = new ServerExportData
        {
            ServerName = "My Server",
            ServerDescription = "desc",
            ServerGame = "minecraft-java",
            ServerVersion = "1.21",
            ServerType = "paper",
            ServerPort = 25565,
            RamMinMb = 1024,
            RamMaxMb = 4096,
            JavaMajor = 21,
            StartCommand = "java -jar server.jar",
            StopCommand = "stop",
            ModsFolder = "mods",
            AutoRestartOnCrash = true,
            CreateFirewallRule = true,
        };

        var json = JsonHelper.Serialize(export);
        var roundTripped = JsonHelper.Deserialize<ServerExportData>(json);
        Assert.NotNull(roundTripped);
        Assert.Equal(export.ServerName, roundTripped!.ServerName);
        Assert.Equal(export.ServerGame, roundTripped.ServerGame);
        Assert.Equal(export.ServerPort, roundTripped.ServerPort);
        Assert.Equal(export.AutoRestartOnCrash, roundTripped.AutoRestartOnCrash);
        Assert.Equal(export.CreateFirewallRule, roundTripped.CreateFirewallRule);
    }

    [Fact]
    public void ServerInstance_round_trips_with_tunnel_and_stats()
    {
        var instance = new ServerInstance
        {
            ServerGame = "minecraft-java",
            ServerGenres = new List<string> { "Sandbox", "Survival" },
            ServerVersion = "1.21",
            ServerType = "paper",
            ServerName = "My Server",
            ServerDescription = "desc",
            ServerRoot = "C:\\Servers\\My Server",
            ServerFile = "server.jar",
            ServerPort = 25565,
            RamMinMb = 1024,
            RamMaxMb = 4096,
            JavaMajor = 21,
            StartCommand = "java -jar server.jar",
            StopCommand = "stop",
            Tunnel = new TunnelSpec { Mode = "playit" },
            Stats = new ServerStats
            {
                TotalUptimeSeconds = 3600,
                MonthUptimeSeconds = 1800,
                MonthKey = "2026-03",
                CrashCount = 1,
                LastStarted = new DateTime(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc),
            },
        };

        Assert.Equal("0.0.0.0", instance.ServerIp);
        Assert.Equal(instance.ServerRoot, instance.ResolveRoot(""));
        Assert.Equal("C:\\Servers\\My Server\\logs\\latest.log", instance.ResolveRoot("logs\\latest.log"));

        var json = JsonHelper.Serialize(instance);
        var roundTripped = JsonHelper.Deserialize<ServerInstance>(json);
        Assert.NotNull(roundTripped);
        Assert.Equal(instance.ServerGame, roundTripped!.ServerGame);
        Assert.Equal(instance.ServerGenres, roundTripped.ServerGenres);
        Assert.Equal(instance.ServerRoot, roundTripped.ServerRoot);
        Assert.Equal(instance.Tunnel.Mode, roundTripped.Tunnel.Mode);
        Assert.Equal(instance.Stats.TotalUptimeSeconds, roundTripped.Stats.TotalUptimeSeconds);
        Assert.Equal(instance.Stats.MonthKey, roundTripped.Stats.MonthKey);
        Assert.Equal(instance.Stats.LastStarted, roundTripped.Stats.LastStarted);
    }

    [Theory]
    // NB: `Json` constructs `JsonStringEnumConverter()` with no naming policy argument, and on
    // net8.0 `JsonStringEnumConverter`'s global naming policy
    // fallback does not extend to enum *member* names unless a naming policy is passed to the
    // converter explicitly. So enum values round-trip using their literal C# member names
    // (PascalCase), not snake_case.
    [InlineData(ServerState.Stopped, "\"Stopped\"")]
    [InlineData(ServerState.Starting, "\"Starting\"")]
    [InlineData(ServerState.Running, "\"Running\"")]
    [InlineData(ServerState.Stopping, "\"Stopping\"")]
    [InlineData(ServerState.Crashed, "\"Crashed\"")]
    [InlineData(ServerState.Installing, "\"Installing\"")]
    public void ServerState_serializes_using_literal_member_names(ServerState state, string expectedJson)
    {
        var json = JsonHelper.Serialize(state);
        Assert.Equal(expectedJson, json);

        var roundTripped = JsonHelper.Deserialize<ServerState>(json);
        Assert.Equal(state, roundTripped);
    }
}
