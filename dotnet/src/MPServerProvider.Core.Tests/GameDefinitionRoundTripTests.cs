using System.Text.Json;
using JsonHelper = MPServerProvider.Core.Json.Json;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class GameDefinitionRoundTripTests
{
    public static IEnumerable<object[]> BuiltInIds()
        => BuiltInGames.All().Select(g => new object[] { g.Id });

    [Theory]
    [MemberData(nameof(BuiltInIds))]
    public void Built_in_definition_round_trips_through_manifest_json(string id)
    {
        var originalText = JsonHelper.Serialize(BuiltInGames.All().Single(g => g.Id == id));

        var definition = JsonHelper.Deserialize<GameDefinition>(originalText);
        Assert.NotNull(definition);
        Assert.False(string.IsNullOrEmpty(definition!.Id));
        Assert.False(string.IsNullOrEmpty(definition.Name));
        Assert.Equal(1, definition.ManifestVersion);

        // Round-trip: serialize back out and re-parse; the resulting object graph must be
        // equivalent to the first parse (this exercises every property name/type mapping).
        var reserialized = JsonHelper.Serialize(definition);
        var roundTripped = JsonHelper.Deserialize<GameDefinition>(reserialized);
        Assert.NotNull(roundTripped);

        Assert.Equal(definition.Id, roundTripped!.Id);
        Assert.Equal(definition.Name, roundTripped.Name);
        Assert.Equal(definition.Description, roundTripped.Description);
        Assert.Equal(definition.Genres, roundTripped.Genres);
        Assert.Equal(definition.Artwork.Cover, roundTripped.Artwork.Cover);
        Assert.Equal(definition.Artwork.Hero, roundTripped.Artwork.Hero);
        Assert.Equal(definition.Artwork.Accent, roundTripped.Artwork.Accent);
        Assert.Equal(definition.Install.Provider, roundTripped.Install.Provider);
        Assert.Equal(definition.Install.Variants, roundTripped.Install.Variants);
        Assert.Equal(definition.Install.AppId, roundTripped.Install.AppId);
        Assert.Equal(definition.Install.Branch, roundTripped.Install.Branch);
        Assert.Equal(definition.Install.Anonymous, roundTripped.Install.Anonymous);
        Assert.Equal(definition.Install.Product, roundTripped.Install.Product);
        Assert.Equal(definition.Runtime.Executable, roundTripped.Runtime.Executable);
        Assert.Equal(definition.Runtime.Arguments, roundTripped.Runtime.Arguments);
        Assert.Equal(definition.Runtime.WorkingDir, roundTripped.Runtime.WorkingDir);
        Assert.Equal(definition.Runtime.StopCommand, roundTripped.Runtime.StopCommand);
        Assert.Equal(definition.Runtime.Requires?.Java, roundTripped.Runtime.Requires?.Java);
        Assert.Equal(definition.Network.DefaultPort, roundTripped.Network.DefaultPort);
        Assert.Equal(definition.Network.Protocol, roundTripped.Network.Protocol);
        Assert.Equal(definition.Network.Query, roundTripped.Network.Query);
        Assert.Equal(definition.Network.ConsoleMethod, roundTripped.Network.ConsoleMethod);
        Assert.Equal(definition.Mods.Supported, roundTripped.Mods.Supported);
        Assert.Equal(definition.Mods.Folder, roundTripped.Mods.Folder);
        Assert.Equal(definition.Mods.Extensions, roundTripped.Mods.Extensions);
        Assert.Equal(definition.Mods.Kind, roundTripped.Mods.Kind);

        // Manifests use snake_case keys exclusively.
        using var doc = JsonDocument.Parse(originalText);
        Assert.True(doc.RootElement.TryGetProperty("manifest_version", out _));
        Assert.True(doc.RootElement.GetProperty("network").TryGetProperty("default_port", out _));
    }
}

public class AppSettingsRoundTripTests
{
    private const string SettingsJson = """
        {
          "start_on_boot": false,
          "start_minimized_to_tray": false,
          "restore_running_servers": false,
          "news_auto_scroll": true,
          "firewall_rules": [
            {
              "name": "minecraft",
              "port": 25565,
              "protocol": "both",
              "active": true,
              "created": "2026-07-15T12:43:16.9806051+02:00"
            }
          ],
          "assumed_hosting_price_per_hour": 0.02
        }
        """;

    [Fact]
    public void Deserializes_settings_json_without_loss()
    {
        var originalText = SettingsJson;
        var settings = JsonHelper.Deserialize<AppSettings>(originalText);
        Assert.NotNull(settings);
        Assert.False(settings!.StartOnBoot);
        Assert.True(settings.NewsAutoScroll);
        Assert.Equal(0.02, settings.AssumedHostingPricePerHour);
        Assert.Single(settings.FirewallRules);
        Assert.Equal("minecraft", settings.FirewallRules[0].Name);
        Assert.Equal(25565, settings.FirewallRules[0].Port);
        Assert.Equal("both", settings.FirewallRules[0].Protocol);
        Assert.True(settings.FirewallRules[0].Active);

        var reserialized = JsonHelper.Serialize(settings);
        var roundTripped = JsonHelper.Deserialize<AppSettings>(reserialized);
        Assert.NotNull(roundTripped);
        Assert.Equal(settings.StartOnBoot, roundTripped!.StartOnBoot);
        Assert.Equal(settings.StartMinimizedToTray, roundTripped.StartMinimizedToTray);
        Assert.Equal(settings.RestoreRunningServers, roundTripped.RestoreRunningServers);
        Assert.Equal(settings.NewsAutoScroll, roundTripped.NewsAutoScroll);
        Assert.Equal(settings.AssumedHostingPricePerHour, roundTripped.AssumedHostingPricePerHour);
        Assert.Equal(settings.FirewallRules.Count, roundTripped.FirewallRules.Count);
        Assert.Equal(settings.FirewallRules[0].Name, roundTripped.FirewallRules[0].Name);
        Assert.Equal(settings.FirewallRules[0].Port, roundTripped.FirewallRules[0].Port);
        Assert.Equal(settings.FirewallRules[0].Protocol, roundTripped.FirewallRules[0].Protocol);
        Assert.Equal(settings.FirewallRules[0].Active, roundTripped.FirewallRules[0].Active);
        Assert.Equal(settings.FirewallRules[0].Created, roundTripped.FirewallRules[0].Created);
    }
}
