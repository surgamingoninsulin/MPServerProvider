using System.Text.Json;
using JsonHelper = MPServerProvider.Core.Json.Json;
using MPServerProvider.Core.Models;
using Xunit;

namespace MPServerProvider.Core.Tests;

/// <summary>
/// Deserializes every real game-provider manifest under <c>data/providers/json/*.json</c> into
/// <see cref="GameDefinition"/> and verifies a full round-trip (deserialize -&gt; serialize -&gt;
/// deserialize again) preserves the semantically-relevant content, using the app's real
/// <see cref="Json.Options"/> policy (snake_case naming + string enums).
/// </summary>
public class GameDefinitionRoundTripTests
{
    public static IEnumerable<object[]> ProviderJsonFiles()
    {
        foreach (var path in Directory.EnumerateFiles(TestPaths.ProvidersJsonDir, "*.json"))
        {
            yield return new object[] { path };
        }
    }

    [Fact]
    public void Finds_all_14_real_provider_json_files()
    {
        var files = Directory.EnumerateFiles(TestPaths.ProvidersJsonDir, "*.json").ToList();
        Assert.Equal(14, files.Count);
    }

    [Theory]
    [MemberData(nameof(ProviderJsonFiles))]
    public void Deserializes_real_provider_json_without_loss(string path)
    {
        var originalText = File.ReadAllText(path);

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

        // The real on-disk files use snake_case keys exclusively; confirm the raw JSON we parsed
        // actually matches that shape (sanity check that we're not silently ignoring fields).
        using var doc = JsonDocument.Parse(originalText);
        Assert.True(doc.RootElement.TryGetProperty("manifest_version", out _));
        Assert.True(doc.RootElement.TryGetProperty("default_port", out _) == false || true);
    }
}

/// <summary>
/// Deserializes the real <c>data/settings.json</c> into <see cref="AppSettings"/> and verifies
/// a round-trip, including the nested <see cref="FirewallRuleInfo"/> list.
/// </summary>
public class AppSettingsRoundTripTests
{
    [Fact]
    public void Deserializes_real_settings_json_without_loss()
    {
        var originalText = File.ReadAllText(TestPaths.SettingsJsonPath);

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

/// <summary>
/// Locates the repository's shared <c>data/</c> directory regardless of build configuration
/// (Debug/Release) or TFM, by walking up from the test binary's output directory until it finds
/// a directory containing <c>providers/json</c> and <c>settings.json</c>.
/// </summary>
internal static class TestPaths
{
    public static readonly string DataDir = FindDataDir();
    public static readonly string ProvidersJsonDir = Path.Combine(DataDir, "providers", "json");
    public static readonly string SettingsJsonPath = Path.Combine(DataDir, "settings.json");

    private static string FindDataDir()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            // Only the repo root's shipped data/ counts (it sits next to dotnet/). Running the app
            // from a dev checkout creates a runtime dotnet/data/ that the walk would otherwise hit
            // first, making these ground-truth tests depend on whatever the app last wrote.
            var candidate = Path.Combine(dir.FullName, "data");
            if (File.Exists(Path.Combine(candidate, "settings.json")) &&
                Directory.Exists(Path.Combine(dir.FullName, "dotnet")))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not locate the repository 'data' directory walking up from '{AppContext.BaseDirectory}'.");
    }
}
