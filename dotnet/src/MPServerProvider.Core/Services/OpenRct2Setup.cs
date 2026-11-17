namespace MPServerProvider.Core.Services;

/// <summary>
/// First-run setup helpers for OpenRCT2 servers: the default permission groups file (matching the
/// community's standard dedicated-server groups.json) and pointing the server at the player's own
/// legitimate RCT2 / RCT Classic / RCT1 data files through user-data\config.ini.
/// </summary>
public static class OpenRct2Setup
{
    public const string GameId = "openrct2";

    // Mirrors the groups.json commonly used for OpenRCT2 dedicated servers: an Admin group with
    // every permission, a read-only Spectator group, the default "User" group new players join in,
    // and a "Power User" group with most permissions but no moderation/admin tools.
    public const string DefaultGroupsJson = """
        {
            "default_group": 2,
            "groups": [
                {
                    "id": 0,
                    "name": "Admin",
                    "permissions": [
                        "PERMISSION_CHAT",
                        "PERMISSION_TERRAFORM",
                        "PERMISSION_SET_WATER_LEVEL",
                        "PERMISSION_TOGGLE_PAUSE",
                        "PERMISSION_CREATE_RIDE",
                        "PERMISSION_REMOVE_RIDE",
                        "PERMISSION_BUILD_RIDE",
                        "PERMISSION_RIDE_PROPERTIES",
                        "PERMISSION_SCENERY",
                        "PERMISSION_PATH",
                        "PERMISSION_CLEAR_LANDSCAPE",
                        "PERMISSION_GUEST",
                        "PERMISSION_STAFF",
                        "PERMISSION_PARK_PROPERTIES",
                        "PERMISSION_PARK_FUNDING",
                        "PERMISSION_KICK_PLAYER",
                        "PERMISSION_MODIFY_GROUPS",
                        "PERMISSION_SET_PLAYER_GROUP",
                        "PERMISSION_CHEAT",
                        "PERMISSION_TOGGLE_SCENERY_CLUSTER",
                        "PERMISSION_PASSWORDLESS_LOGIN",
                        "PERMISSION_MODIFY_TILE",
                        "PERMISSION_EDIT_SCENARIO_OPTIONS"
                    ]
                },
                {
                    "id": 1,
                    "name": "Spectator",
                    "permissions": [
                        "PERMISSION_CHAT"
                    ]
                },
                {
                    "id": 2,
                    "name": "User",
                    "permissions": [
                        "PERMISSION_CHAT",
                        "PERMISSION_TERRAFORM",
                        "PERMISSION_SET_WATER_LEVEL",
                        "PERMISSION_TOGGLE_PAUSE",
                        "PERMISSION_CREATE_RIDE",
                        "PERMISSION_REMOVE_RIDE",
                        "PERMISSION_BUILD_RIDE",
                        "PERMISSION_RIDE_PROPERTIES",
                        "PERMISSION_SCENERY",
                        "PERMISSION_PATH",
                        "PERMISSION_CLEAR_LANDSCAPE",
                        "PERMISSION_GUEST",
                        "PERMISSION_STAFF",
                        "PERMISSION_PARK_PROPERTIES",
                        "PERMISSION_PARK_FUNDING",
                        "PERMISSION_TOGGLE_SCENERY_CLUSTER"
                    ]
                },
                {
                    "id": 3,
                    "name": "Power User",
                    "permissions": [
                        "PERMISSION_CHAT",
                        "PERMISSION_TERRAFORM",
                        "PERMISSION_SET_WATER_LEVEL",
                        "PERMISSION_TOGGLE_PAUSE",
                        "PERMISSION_CREATE_RIDE",
                        "PERMISSION_REMOVE_RIDE",
                        "PERMISSION_BUILD_RIDE",
                        "PERMISSION_RIDE_PROPERTIES",
                        "PERMISSION_SCENERY",
                        "PERMISSION_PATH",
                        "PERMISSION_CLEAR_LANDSCAPE",
                        "PERMISSION_GUEST",
                        "PERMISSION_STAFF",
                        "PERMISSION_PARK_PROPERTIES",
                        "PERMISSION_PARK_FUNDING",
                        "PERMISSION_CHEAT",
                        "PERMISSION_TOGGLE_SCENERY_CLUSTER"
                    ]
                }
            ]
        }
        """;

    public static string UserDataDir(string serverRoot) => Path.Combine(serverRoot, "user-data");

    public static string GroupsFile(string serverRoot) => Path.Combine(UserDataDir(serverRoot), "groups.json");

    public static string ConfigFile(string serverRoot) => Path.Combine(UserDataDir(serverRoot), "config.ini");

    /// <summary>Writes the default groups.json once, on first install; never overwrites an existing one.</summary>
    public static void EnsureGroupsFile(string serverRoot)
    {
        var path = GroupsFile(serverRoot);
        if (File.Exists(path))
        {
            return;
        }

        Directory.CreateDirectory(UserDataDir(serverRoot));
        File.WriteAllText(path, DefaultGroupsJson);
    }

    /// <summary>
    /// Reads game_path / rct1_path out of user-data\config.ini, if the file and section exist.
    /// </summary>
    public static (string? Rct2Path, string? Rct1Path) ReadGameDataPaths(string serverRoot)
    {
        var path = ConfigFile(serverRoot);
        if (!File.Exists(path))
        {
            return (null, null);
        }

        var lines = File.ReadAllLines(path);
        return (ReadIniValue(lines, "general", "game_path"), ReadIniValue(lines, "general", "rct1_path"));
    }

    /// <summary>
    /// Points the server at the player's own RCT2/RCT Classic install (required) and, optionally,
    /// their RCT1 (or RCT1 Deluxe) install for its scenery and scenarios. OpenRCT2 reads both from
    /// the [general] section of user-data\config.ini, which this creates if it doesn't exist yet.
    /// </summary>
    public static void SetGameDataPaths(string serverRoot, string rct2Path, string? rct1Path)
    {
        Directory.CreateDirectory(UserDataDir(serverRoot));
        var path = ConfigFile(serverRoot);
        var lines = (File.Exists(path) ? File.ReadAllLines(path) : Array.Empty<string>()).ToList();

        SetIniValue(lines, "general", "game_path", rct2Path);
        if (!string.IsNullOrWhiteSpace(rct1Path))
        {
            SetIniValue(lines, "general", "rct1_path", rct1Path!);
        }

        File.WriteAllLines(path, lines);
    }

    private static string? ReadIniValue(IReadOnlyList<string> lines, string section, string key)
    {
        var (start, end) = FindSection(lines, section);
        if (start < 0)
        {
            return null;
        }

        for (var i = start + 1; i < end; i++)
        {
            if (TryParseKeyLine(lines[i], key, out var value))
            {
                return value;
            }
        }

        return null;
    }

    private static void SetIniValue(List<string> lines, string section, string key, string value)
    {
        var (start, end) = FindSection(lines, section);
        if (start < 0)
        {
            lines.Add($"[{section}]");
            start = lines.Count - 1;
            end = lines.Count;
        }

        var line = $"{key} = \"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
        for (var i = start + 1; i < end; i++)
        {
            if (TryParseKeyLine(lines[i], key, out _))
            {
                lines[i] = line;
                return;
            }
        }

        lines.Insert(end, line);
    }

    // Returns the section header's line index and the index just past its last line (the next
    // "[section]" header or end of file), or (-1, -1) if the section isn't there yet.
    private static (int Start, int End) FindSection(IReadOnlyList<string> lines, string section)
    {
        var header = $"[{section}]";
        var start = -1;
        for (var i = 0; i < lines.Count; i++)
        {
            if (lines[i].Trim().Equals(header, StringComparison.OrdinalIgnoreCase))
            {
                start = i;
                break;
            }
        }

        if (start < 0)
        {
            return (-1, -1);
        }

        var end = lines.Count;
        for (var i = start + 1; i < lines.Count; i++)
        {
            if (lines[i].TrimStart().StartsWith('['))
            {
                end = i;
                break;
            }
        }

        return (start, end);
    }

    private static bool TryParseKeyLine(string line, string key, out string value)
    {
        value = string.Empty;
        var trimmed = line.TrimStart();
        if (!trimmed.StartsWith(key, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var rest = trimmed[key.Length..].TrimStart();
        if (!rest.StartsWith('='))
        {
            return false;
        }

        rest = rest[1..].Trim();
        if (rest.Length >= 2 && rest[0] == '"' && rest[^1] == '"')
        {
            rest = rest[1..^1].Replace("\\\"", "\"").Replace("\\\\", "\\");
        }

        value = rest;
        return true;
    }
}
