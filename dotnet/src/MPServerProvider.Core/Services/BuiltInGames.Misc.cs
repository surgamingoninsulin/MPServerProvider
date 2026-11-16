using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public static partial class BuiltInGames
{
    private static IEnumerable<GameDefinition> MiscGames() => new[]
    {
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "dont-starve-together",
            Name = "Don't Starve Together Dedicated Server",
            Description = "Official Don't Starve Together dedicated server from Klei, installed through Valve's SteamCMD (anonymous). Runs a single overworld shard. Paste a free cluster token from your Klei account in the Klei Token tab before the first start.",
            Genres = new() { "Survival", "Co-op", "Crafting" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/322330/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/322330/library_hero.jpg",
                Accent = "#8B5A2B",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 343050,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\bin64\\dontstarve_dedicated_server_nullrenderer_x64.exe",
                Arguments = "-console -persistent_storage_root \"{server_root}\\data\" -conf_dir DoNotStarveTogether -cluster Cluster_1 -shard Master -port {server_port}",
                WorkingDir = "{server_root}\\bin64",
                StopCommand = "c_shutdown(true)",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 10999,
                Protocol = "udp",
                Query = "none",
                ConsoleMethod = "stdin",
            },
            Mods = new ModsSpec
            {
                Supported = false,
                Folder = "mods",
                Extensions = new(),
                Kind = "mod",
            },
            Details = new GameDetails
            {
                About = "Don't Starve Together is the multiplayer version of Klei's wilderness survival game. Players gather food and resources, craft tools and shelter, and try to survive the seasons and the strange creatures of the Constant together.\n\nA dedicated server keeps your world running without one player hosting from the game client. Each server is a cluster that can hold an overworld shard and an optional caves shard. This entry runs one overworld shard, which is enough for most groups.\n\nKlei's free dedicated server tool needs a cluster token tied to a Klei account that owns the game. World settings, the player limit and the server password live in plain text files inside the cluster folder.",
                Developer = "Klei Entertainment",
                Publisher = "Klei Entertainment",
                ReleaseDate = "Apr 21, 2016",
                Website = "https://www.klei.com/games/dont-starve-together",
                StoreUrl = "https://store.steampowered.com/app/322330/",
                DocsUrl = "https://dontstarve.wiki.gg/wiki/Dedicated_Server",
                MaxPlayers = "Configurable (max_players in cluster.ini, up to 64)",
                Features = new()
                {
                    "Official Klei dedicated server, installed anonymously via SteamCMD (app 343050)",
                    "Persistent world that stays online without a player hosting",
                    "Single overworld shard by default, with optional caves shard as a second process",
                    "Workshop mods through dedicated_server_mods_setup.lua and modoverrides.lua",
                    "Admin list, password and player limit set in plain text files",
                    "Lua console commands such as c_save() and c_shutdown()",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 1024,
                    RecommendedRamMb = 2048,
                    DiskGb = 4,
                    Notes = "A single shard with a few players uses well under 1 GB of RAM. Adding a caves shard runs a second process and roughly doubles memory use. Mods raise all of these figures.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "10999", Protocol = "UDP", Purpose = "Game traffic (the -port value, server_port in server.ini)" },
                    new GamePortInfo { Port = "27016", Protocol = "UDP", Purpose = "Steam master server port (steam_master_server_port default)" },
                    new GamePortInfo { Port = "8766", Protocol = "UDP", Purpose = "Steam authentication port (steam_authentication_port default)" },
                },
                SetupTips = new()
                {
                    "The app launches bin64\\dontstarve_dedicated_server_nullrenderer_x64.exe from the bin64 folder with -console -persistent_storage_root data -conf_dir DoNotStarveTogether -cluster Cluster_1 -shard Master -port 10999. Each server keeps its own cluster in data\\DoNotStarveTogether\\Cluster_1 inside the server folder. This runs a single overworld shard with no caves.",
                    "The server will not go online without a cluster token. Open the server's Klei Token tab, sign in to your Klei account there, open Games, Don't Starve Together, Game Servers, add a new server, copy the token and paste it in the box at the top. The app saves it as cluster_token.txt and writes a default cluster.ini.",
                    "Set the server name, password and player limit in data\\DoNotStarveTogether\\Cluster_1\\cluster.ini in the server folder (cluster_name, cluster_password and max_players). Shard settings such as server_port are in Cluster_1\\Master\\server.ini.",
                    "Add admins by putting their Klei user IDs (KU_XXXXXXXX), one per line, in adminlist.txt in the cluster folder. Without that file, the token owner is the only admin.",
                    "Workshop mods are listed with ServerModSetup(\"ID\") in mods\\dedicated_server_mods_setup.lua under the server folder, then enabled in Cluster_1\\Master\\modoverrides.lua.",
                    "Open UDP 10999 in Firewall Rules or with Connect to Tunnel. Use the c_shutdown(true) console command to save and stop cleanly.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/322330/ss_d053a7220a03ab689da9312c6d15fe8211401f55.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/322330/ss_37e30c4cf8c945f1958890c8fa7346af6bea547d.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/322330/ss_f863729856b94b3485ee80e6fd11289595f6634c.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/322330/ss_92f4145bd510f66d63028e1cdf27eb30bd44cea1.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/322330/ss_9a7730946f2e060457b5c19ac2d870a78ab0f20b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/322330/ss_7f2ad9be835040a8995cd3653cd521553ef776cf.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "aska",
            Name = "ASKA Dedicated Server",
            Description = "Official ASKA dedicated server from Sand Sailor Studio, installed through Valve's SteamCMD (anonymous). Needs a Steam Game Server Login Token in server properties.txt to be listed.",
            Genres = new() { "Survival", "Building", "Co-op" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/1898300/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/1898300/library_hero.jpg",
                Accent = "#4A6B3A",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 3246670,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\AskaServer.exe",
                Arguments = "-propertiesPath \"{server_root}\\server properties.txt\"",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 27015,
                Protocol = "udp",
                Query = "none",
                ConsoleMethod = "stdin",
            },
            Mods = new ModsSpec
            {
                Supported = false,
                Folder = "mods",
                Extensions = new(),
                Kind = "mod",
            },
            Details = new GameDetails
            {
                About = "ASKA is a Viking survival and settlement-building game from Sand Sailor Studio. Players explore a harsh northern land, gather resources and grow a village of villagers who take on jobs, while defending it from raids and the cold.\n\nThe free ASKA Dedicated Server tool keeps a world online so your group can play without one person hosting from the game client. It reads its settings from a plain text file called server properties.txt next to the server executable.\n\nConnections go through a relay, so a dedicated server can be reached without much network setup. A Steam Game Server Login Token is still needed for the server to show up in the in-game list.",
                Developer = "Sand Sailor Studio",
                Publisher = "Thunderful Publishing",
                ReleaseDate = "Jun 20, 2024",
                Website = "https://www.playaska.com/",
                StoreUrl = "https://store.steampowered.com/app/1898300/",
                DocsUrl = "https://eggs.pterodactyl.io/egg/games-aska",
                MaxPlayers = "Up to 4 players",
                Features = new()
                {
                    "Official ASKA Dedicated Server, installed anonymously via SteamCMD (app 3246670)",
                    "Persistent village that stays online without a player hosting",
                    "Settings in a single plain text file (server properties.txt)",
                    "Optional join password",
                    "Relay connections that need little or no port forwarding",
                    "Option to keep the world running while no players are online",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Steam game port (steam game port in server properties.txt)" },
                    new GamePortInfo { Port = "27016", Protocol = "UDP", Purpose = "Steam query port (steam query port in server properties.txt)" },
                },
                SetupTips = new()
                {
                    "The app launches AskaServer.exe with -propertiesPath pointing at server properties.txt in the server folder. All server settings come from that file, not from launch arguments.",
                    "Create a Game Server Login Token at steamcommunity.com/dev/managegameservers using app ID 1898300 (the game, not the server tool). Paste it into the authentication token line of server properties.txt, then restart.",
                    "Set display name, server name, password and region in server properties.txt. Leave password empty for an open server.",
                    "Set keep server world alive to true if the world should keep running while nobody is online.",
                    "ASKA connects players through a relay, so forwarding ports is usually not needed. If players cannot see the server, open the steam game port and steam query port from the file in Firewall Rules.",
                    "Stop the server before editing server properties.txt, and back up the save folder before major game updates.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1898300/fd81504299949d63c0b70cc5bb85a39ddb448f1b/ss_fd81504299949d63c0b70cc5bb85a39ddb448f1b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1898300/8acd82606fa09a6cff9c8f03681c994dc0d4fe19/ss_8acd82606fa09a6cff9c8f03681c994dc0d4fe19.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1898300/8c2767489ff8fddbd5fe87fb5f160f7d25944e84/ss_8c2767489ff8fddbd5fe87fb5f160f7d25944e84.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1898300/f433cdf25db35a8c62071ea08aeaae028af0c4ce/ss_f433cdf25db35a8c62071ea08aeaae028af0c4ce.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1898300/039b9bc448fd93288514fbb631ad106fb706e500/ss_039b9bc448fd93288514fbb631ad106fb706e500.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1898300/e09e0913e257e95f3497f0063a7b96634a219117/ss_e09e0913e257e95f3497f0063a7b96634a219117.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "totally-accurate-battlegrounds",
            Name = "Totally Accurate Battlegrounds Dedicated Server",
            Description = "Official Totally Accurate Battlegrounds dedicated server from Landfall, installed through Valve's SteamCMD (anonymous). Settings, including the password, are in game_settings.txt.",
            Genres = new() { "Battle Royale", "Shooter" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/823130/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/823130/library_hero.jpg",
                Accent = "#E8B931",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 2311970,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\TABG.exe",
                Arguments = "",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 7777,
                Protocol = "udp",
                Query = "none",
                ConsoleMethod = "stdin",
            },
            Mods = new ModsSpec
            {
                Supported = false,
                Folder = "mods",
                Extensions = new(),
                Kind = "mod",
            },
            Details = new GameDetails
            {
                About = "Totally Accurate Battlegrounds is a free-to-play battle royale from Landfall built around physics-driven, wobbly combat. Players drop onto an island, loot weapons and gear, and fight to be the last one standing.\n\nThe free Totally Accurate Battlegrounds Dedicated Server tool lets you run your own custom match server that appears in the in-game server browser. You choose the server name, password, player limit and match rules.\n\nServers can connect through a Unity-hosted relay, so most hosts do not need a public IP or port forwarding. With the relay on, the server also gets a join code that players can enter.",
                Developer = "Landfall",
                Publisher = "Landfall",
                ReleaseDate = "Jun 5, 2018",
                Website = "https://landfall.se/totally-accurate-battlegrounds",
                StoreUrl = "https://store.steampowered.com/app/823130/",
                DocsUrl = "https://landfall.se/tabg-dedicated-server-guide",
                MaxPlayers = "Configurable (MaxPlayers in game_settings.txt, default 70)",
                Features = new()
                {
                    "Official Landfall dedicated server, installed anonymously via SteamCMD (app 2311970)",
                    "Custom servers listed in the in-game server browser",
                    "Relay mode with a join code, no port forwarding needed",
                    "Optional join password",
                    "Match rules such as souls, spectating and ring sizes in game_settings.txt",
                    "VerifyAllowedWords.exe tool to check the server name against Landfall's word list",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "UDP", Purpose = "Game traffic (Port in game_settings.txt, only needed when Relay is false)" },
                },
                SetupTips = new()
                {
                    "The app launches TABG.exe from the server folder with no extra arguments. All settings come from game_settings.txt in the same folder, so the port, name and password set in the app are not passed on the command line.",
                    "Edit game_settings.txt before the first start. ServerName is shown in the server browser, Password sets the join password, and MaxPlayers sets the player limit.",
                    "The server name may only use words from Landfall's allowed list. Run VerifyAllowedWords.exe in the server folder to check it before starting.",
                    "Set Relay to true unless you have a public IP. With the relay on, no ports need opening and the console shows a join code.",
                    "With Relay set to false, open UDP 7777 (or the Port value) in Firewall Rules or with Connect to Tunnel.",
                    "A healthy server prints heartbeat lines in the console. Errors at the end of the log mean it failed to start.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/823130/ss_d0a03d875702350c11cbf458a7dbd5a4b27a1363.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/823130/ss_6941f0c427a1a936f7a5f4c19937adb415e1564d.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/823130/ss_0fbd4650d36d5406eac0d587ce4837b0727fbb59.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/823130/ss_68fde72ce171a11893df5a2edff817a96dd5195f.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/823130/ss_e592a9c844a687a7355df4ce7fbe238aeb134017.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/823130/ss_02434928193dd8c9a60bac5bb3d31b9380154b20.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "hytale",
            Name = "Hytale Server",
            Description = "Official Hytale server from Hypixel Studios, downloaded with Hypixel's Hytale Downloader. The first install asks you to sign in with your Hytale account in the browser. Runs on Java 25, which the app downloads for you.",
            Genres = new() { "Sandbox", "Adventure", "RPG" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.hytale.com/4fa9acf6-59a8-41bd-b450-8fd4f08c3709.png",
                Hero = "https://cdn.hytale.com/c2cfce93-4518-4215-b2bc-82dc99974479.png",
                Accent = "#2F7FC1",
            },
            Install = new InstallSpec
            {
                Provider = "hytale",
                Variants = new(),
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{java}",
                Arguments = "-Xms{ram_min_mb}M -Xmx{ram_max_mb}M -jar \"{server_root}\\Server\\HytaleServer.jar\" --assets \"{server_root}\\Assets.zip\" --bind 0.0.0.0:{server_port}",
                WorkingDir = "{server_root}",
                StopCommand = "stop",
                Requires = new RequiresSpec { Java = 25 },
            },
            Network = new NetworkSpec
            {
                DefaultPort = 5520,
                Protocol = "udp",
                Query = "none",
                ConsoleMethod = "stdin",
            },
            Mods = new ModsSpec
            {
                Supported = true,
                Folder = "mods",
                Extensions = new() { ".jar", ".zip" },
                Kind = "mod",
            },
            Details = new GameDetails
            {
                About = "Hytale is a block-based sandbox and adventure game from Hypixel Studios. Players explore a procedurally generated world full of creatures and dungeons, build with blocks and craft gear, alone or with friends.\n\nThe official Hytale server is a Java application that ships with the game. You can copy it from the launcher install or fetch it with Hypixel's Hytale Downloader, which signs in with a Hytale account. The server keeps worlds online in the universe folder and reads its settings from config.json.\n\nServers support plugins and asset packs dropped into the mods folder. Each server must be signed in to a Hytale account once with a device code before players can join.",
                Developer = "Hypixel Studios",
                Publisher = "Hypixel Studios",
                ReleaseDate = "Jan 13, 2026",
                Website = "https://hytale.com/",
                StoreUrl = "https://store.hytale.com/",
                DocsUrl = "https://support.hytale.com/hc/en-us/articles/45326769420827",
                MaxPlayers = "Configurable (MaxPlayers in config.json)",
                Features = new()
                {
                    "Official Hytale server (HytaleServer.jar) running on Java 25",
                    "QUIC networking over UDP on port 5520",
                    "Plugins (.jar) and asset packs (.zip) loaded from the mods folder",
                    "Operator, permission group, whitelist and ban files",
                    "Worlds and player data kept in the universe folder",
                    "Optional server password and view radius limits in config.json",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 4096,
                    RecommendedRamMb = 6144,
                    Notes = "Hypixel lists at least 4 GB of memory and Java 25 on x64 or arm64. View distance drives memory use the most. Lower MaxViewRadius in config.json (12 chunks or less is recommended) if the server runs short of memory.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "5520", Protocol = "UDP", Purpose = "Game traffic over QUIC (the --bind port)" },
                },
                SetupTips = new()
                {
                    "The app runs Java 25 with -Xms and -Xmx from your RAM sliders, then -jar Server\\HytaleServer.jar --assets Assets.zip --bind 0.0.0.0:5520 from the server folder.",
                    "Installing runs Hypixel's Hytale Downloader. The first time, a Hytale sign-in page opens in your browser: enter the code shown in the install window and sign in. The login is kept, so updates don't ask again.",
                    "On first start, type /auth login device in the console, open accounts.hytale.com/device and enter the code. Run /auth persistence Encrypted so the login survives restarts.",
                    "Server name, password, MaxPlayers and MaxViewRadius are in config.json in the server folder. Stop the server before editing it. Operators and groups are in permissions.json, and whitelist.json and bans.json hold the access lists.",
                    "Put plugin .jar files and asset pack .zip files in the mods folder. Worlds and player data are in the universe folder, which is the one to back up.",
                    "Open UDP 5520 in Firewall Rules or with Connect to Tunnel. TCP is not used. Players must run the same game version as the server, so update the files after each Hytale patch.",
                },
                Screenshots = new()
                {
                    "https://cdn.hytale.com/5e7a961d5e334000189a2a76_3.jpg",
                    "https://cdn.hytale.com/5e7a961d5e334000189a2a77_5.jpg",
                    "https://cdn.hytale.com/5e7a961d5e334000189a2a78_6.jpg",
                    "https://cdn.hytale.com/5e7a961e5e334000189a2a79_4.jpg",
                    "https://cdn.hytale.com/5e7a961e5e334000189a2a7b_2.jpg",
                },
            },
        },
    };
}
