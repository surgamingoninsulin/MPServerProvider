using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public static partial class CreativerseWorld
{
    public const string GameId = "creativerse";
    public const string DefaultWorldKey = "mpserver_world";

    public static string DataDir(string? localAppData = null)
        => Path.Combine(localAppData ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PlayfulCorp", "CreativerseServer");

    public static string WorldKey(ServerInstance server, GameDefinition game)
    {
        var args = string.IsNullOrEmpty(server.StartCommand) ? game.Runtime.Arguments : server.StartCommand;
        var match = WorldIdRegex().Match(args ?? string.Empty);
        return match.Success ? match.Groups["key"].Value : DefaultWorldKey;
    }

    public static string ConfigPath(string worldKey, string? localAppData = null)
        => Path.Combine(DataDir(localAppData), "worlddata", "worlds", worldKey, "config_world.json");

    public static bool NeedsSetup(string worldKey, string? localAppData = null)
    {
        var path = ConfigPath(worldKey, localAppData);
        if (!File.Exists(path))
        {
            return true;
        }

        try
        {
            var owner = JsonNode.Parse(File.ReadAllText(path))?["WorldOwnerSteamId"];
            return owner is null || string.IsNullOrWhiteSpace(owner.ToString()) || owner.ToString() == "0";
        }
        catch
        {
            return true;
        }
    }

    public static bool IsSteamId64(string? value)
        => value is not null && SteamId64Regex().IsMatch(value.Trim());

    public static List<string> KnownTemplateKeys(string? localAppData = null)
    {
        var data = DataDir(localAppData);
        var keys = new List<string>();
        foreach (var dir in new[] { "dltemplates", "templates" })
        {
            var path = Path.Combine(data, dir);
            if (Directory.Exists(path))
            {
                keys.AddRange(Directory.GetDirectories(path).Select(Path.GetFileName).OfType<string>());
            }
        }

        var worlds = Path.Combine(data, "worlddata", "worlds");
        if (Directory.Exists(worlds))
        {
            foreach (var config in Directory.GetFiles(worlds, "config_world.json", SearchOption.AllDirectories))
            {
                try
                {
                    if (JsonNode.Parse(File.ReadAllText(config))?["TemplateKey"]?.ToString() is { Length: > 0 } key)
                    {
                        keys.Insert(0, key);
                    }
                }
                catch
                {
                    // Unreadable world config: skip it.
                }
            }
        }

        return keys.Where(k => !string.IsNullOrWhiteSpace(k)).Distinct(StringComparer.Ordinal).ToList();
    }

    public static void Write(string worldKey, string ownerSteamId64, int gamePort, string worldName, string? templateKey, string? localAppData = null)
    {
        if (!IsSteamId64(ownerSteamId64))
        {
            throw new ArgumentException("That isn't a SteamID64 (17 digits starting with 7656119).", nameof(ownerSteamId64));
        }

        var path = ConfigPath(worldKey, localAppData);
        JsonObject config;
        try
        {
            config = File.Exists(path) ? JsonNode.Parse(File.ReadAllText(path)) as JsonObject ?? new JsonObject() : new JsonObject();
        }
        catch (JsonException)
        {
            config = new JsonObject();
        }

        config["GamePort"] = gamePort;
        config["PrivateServer"] = false;
        config["UsePlayerAccessFile"] ??= false;
        config["WorldOwnerSteamId"] = ownerSteamId64.Trim();
        config["WorldKey"] = worldKey;
        if (!string.IsNullOrWhiteSpace(templateKey))
        {
            config["TemplateKey"] = templateKey.Trim();
        }

        config["WorldName"] ??= string.IsNullOrWhiteSpace(worldName) ? "My World" : worldName.Trim();
        config["WorldDescription"] ??= string.Empty;
        config["Tags"] ??= new JsonArray();

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, config.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    [GeneratedRegex(@"-worldId=(?<key>[A-Za-z0-9_\-]+)")]
    private static partial Regex WorldIdRegex();

    [GeneratedRegex(@"^7656119\d{10}$")]
    private static partial Regex SteamId64Regex();
}
