using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public static partial class BuiltInGames
{
    private static IEnumerable<GameDefinition> SurvivalBGames() => new[]
    {
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "craftopia",
            Name = "Craftopia Dedicated Server",
            Description = "Official Craftopia dedicated server from Pocketpair, installed through Valve's SteamCMD (anonymous). Port, world name and password live in ServerSetting.ini, which is created on first launch.",
            Genres = new() { "Survival", "Crafting", "Open World" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/1307550/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/1307550/library_hero.jpg",
                Accent = "#3B9AD9",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 1670340,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\Craftopia.exe",
                Arguments = "-batchmode -showlogs -logFile server.log",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 6587,
                Protocol = "both",
                Query = "none",
                ConsoleMethod = "logfile",
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
                About = "Craftopia is a multiplayer survival action game from Pocketpair. It mixes hunting, farming, hack-and-slash combat, building and factory automation on a set of islands that players explore and develop.\n\nThe Craftopia Dedicated Server is a free tool that keeps a world online without a player hosting it from the game client. It runs from Craftopia.exe in batch mode and reads every option from a ServerSetting.ini file next to the executable.\n\nThe server is light compared to most survival games. A small group can play on a machine with a few gigabytes of free RAM.",
                Developer = "Pocketpair",
                Publisher = "Pocketpair",
                ReleaseDate = "Sep 4, 2020",
                Website = "https://www.pocketpair.jp/craftopia",
                StoreUrl = "https://store.steampowered.com/app/1307550/",
                DocsUrl = "https://www.ionos.com/digitalguide/server/know-how/craftopia-dedicated-server/",
                MaxPlayers = "Set in the [Host] section of ServerSetting.ini (the default file allows 7)",
                Features = new()
                {
                    "Official Craftopia Dedicated Server, installed anonymously via SteamCMD (app 1670340)",
                    "Persistent world that stays online without a player hosting",
                    "All options in one ServerSetting.ini file (world, difficulty, port, password)",
                    "Autosaves on a timer plus hourly autosaves",
                    "Optional join password",
                    "Creative-mode rule overrides for building servers",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 3072,
                    DiskGb = 10,
                    Notes = "IONOS guide minimum: 2 cores at 2.4 GHz, 3 GB RAM and 10 GB of storage. More players need more RAM.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "6587", Protocol = "UDP", Purpose = "Game traffic ([Host] port in ServerSetting.ini)" },
                    new GamePortInfo { Port = "6587", Protocol = "TCP", Purpose = "Game traffic ([Host] port in ServerSetting.ini)" },
                },
                SetupTips = new()
                {
                    "The app starts Craftopia.exe with -batchmode -showlogs -logFile server.log and shows server.log in the console. Craftopia takes no port or name arguments, everything is set in ServerSetting.ini.",
                    "ServerSetting.ini is created next to Craftopia.exe the first time the server runs. Stop the server, edit it, then start it again.",
                    "Set the world name under [GameWorld] and the port, player limit and password under [Host]. The password is only used when password use is switched on in the same section.",
                    "Saves go to the DedicatedServerSave folder next to ServerSetting.ini. Back up that folder before updates.",
                    "Open port 6587 (UDP and TCP) in Firewall Rules or with Connect to Tunnel. Players join with the server IP and port 6587.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1307550/ss_616173811e76c0a36e30521c9bbeb175b51cb852.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1307550/ss_03cfe4af015afbc580f6a04723ec18df00986985.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1307550/ss_2c53935e707daf7be8ae04487a6e6751780799f9.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1307550/ss_78292945425e58643ff5baeb148d8a9dafbb7b18.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1307550/ss_72bbe65038c3977bf1e0ad3ef0d44e242fbebc43.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1307550/ss_e0ef3064da80985c54dd2d6f3ab5e821cecff829.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "dayz",
            Name = "DayZ Server",
            Description = "Official DayZ dedicated server from Bohemia Interactive, installed through Valve's SteamCMD. The app asks for your Steam login when you install, because only accounts that own DayZ can download it.",
            Genres = new() { "Survival", "Open World", "Shooter" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/221100/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/221100/library_hero.jpg",
                Accent = "#5B6B4E",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 223350,
                Anonymous = false,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\DayZServer_x64.exe",
                Arguments = "-config=serverDZ.cfg -port={server_port} -profiles=profiles -dologs -adminlog -netlog -freezecheck",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 2302,
                Protocol = "udp",
                Query = "a2s",
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
                About = "DayZ is a hardcore open-world survival game from Bohemia Interactive. Players spawn on the coast of a large post-Soviet map with almost nothing, then scavenge for food, water, weapons and medical supplies while avoiding infected and other survivors.\n\nA dedicated DayZ server lets you run your own persistent world with your own rules, mission, time acceleration, loot economy and admin team. Server options live in serverDZ.cfg and the loot economy is configured in the mission folder under mpmissions.\n\nThe DayZ Server tool is free, but Bohemia only allows it to be downloaded by a Steam account that owns DayZ. Community mods are loaded with the -mod= launch argument.",
                Developer = "Bohemia Interactive",
                Publisher = "Bohemia Interactive",
                ReleaseDate = "Dec 13, 2018",
                Website = "https://www.dayz.com/",
                StoreUrl = "https://store.steampowered.com/app/221100/",
                DocsUrl = "https://community.bistudio.com/wiki/DayZ:Server_Configuration",
                MaxPlayers = "Set by maxPlayers in serverDZ.cfg (the default file uses 60)",
                Features = new()
                {
                    "Official DayZ Server, installed via SteamCMD (app 223350, needs an account that owns DayZ)",
                    "Persistent world with hive storage per mission and instance",
                    "Configurable loot economy, events and spawns in the mission files",
                    "Server name, passwords, time acceleration and rules in serverDZ.cfg",
                    "Workshop and community mods loaded with -mod= and -serverMod=",
                    "BattlEye anti-cheat",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "2302", Protocol = "UDP", Purpose = "Game traffic (-port)" },
                    new GamePortInfo { Port = "2303-2305", Protocol = "UDP", Purpose = "Additional game and BattlEye traffic used above the game port" },
                    new GamePortInfo { Port = "27016", Protocol = "UDP", Purpose = "Steam query (steamQueryPort in serverDZ.cfg), needed for the server browser" },
                },
                SetupTips = new()
                {
                    "The app starts DayZServer_x64.exe with -config=serverDZ.cfg -port=2302 -profiles=profiles -dologs -adminlog -netlog -freezecheck. The server name is not passed on the command line.",
                    "Installing asks for a Steam login on an account that owns DayZ, because Bohemia does not allow anonymous downloads of app 223350. The password only goes to SteamCMD and is never saved; if Steam asks for a Steam Guard code, enter it in the same window.",
                    "Set hostname, password (join password), passwordAdmin and maxPlayers in serverDZ.cfg in the server folder. The mission is chosen in the Missions block (template = \"dayzOffline.chernarusplus\" by default).",
                    "Logs and crash reports (.RPT, .ADM) are written to the profiles folder. The server does not print to its console window, so check those files when something goes wrong.",
                    "Open UDP 2302-2305 and the Steam query port (steamQueryPort, 27016 in common configs) in Firewall Rules or with Connect to Tunnel.",
                    "Persistence is stored in mpmissions\\<mission>\\storage_<instanceId>. Delete that folder to wipe the world.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/221100/ss_bf0e5d5538d95a1cb943a7963489ba4df2782942.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/221100/ss_4b71f0ac633c88e8af8de1c4eec0b5b263f989cc.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/221100/ss_0487147f80e9350d9c655deb7f3d25b3876a1958.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/221100/ss_06f0e97dd31b322d38dfd137b268d793ff62f86f.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/221100/ss_9517bb26b6811f70b5b19ece4bce3e90452378d3.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/221100/ss_54729794426ffcfb74cb90176edd80b2225eb4fc.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "dayz-experimental",
            Name = "DayZ Experimental Server",
            Description = "DayZ Experimental dedicated server from Bohemia Interactive, installed through Valve's SteamCMD (anonymous). Only players running the DayZ Experimental client can join.",
            Genres = new() { "Survival", "Open World", "Shooter" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/221100/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/221100/library_hero.jpg",
                Accent = "#B45309",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 1042420,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\DayZServer_x64.exe",
                Arguments = "-config=serverDZ.cfg -port={server_port} -profiles=profiles -dologs -adminlog -netlog -freezecheck",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 2302,
                Protocol = "udp",
                Query = "a2s",
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
                About = "DayZ Experimental is Bohemia Interactive's public test build of DayZ. New updates, fixes and balance changes land here before they reach the stable game, so the community can test them and report problems.\n\nThe DayZ Experimental Server is a separate Steam tool (app 1042420) with its own files. It is set up the same way as the stable DayZ server, using serverDZ.cfg and the mission folders under mpmissions.\n\nPlayers need the DayZ Experimental client to join, and the build changes often. Expect wipes and breaking changes, and do not mix its saves with a stable server.",
                Developer = "Bohemia Interactive",
                Publisher = "Bohemia Interactive",
                ReleaseDate = "Dec 13, 2018",
                Website = "https://www.dayz.com/",
                StoreUrl = "https://store.steampowered.com/app/221100/",
                DocsUrl = "https://community.bistudio.com/wiki/DayZ:Server_Configuration",
                MaxPlayers = "Set by maxPlayers in serverDZ.cfg",
                Features = new()
                {
                    "DayZ Experimental Server, installed anonymously via SteamCMD (app 1042420)",
                    "Test upcoming DayZ updates before they reach stable",
                    "Same serverDZ.cfg and mission setup as the stable server",
                    "Separate install, so it does not touch a stable DayZ server",
                    "BattlEye anti-cheat",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "2302", Protocol = "UDP", Purpose = "Game traffic (-port)" },
                    new GamePortInfo { Port = "2303-2305", Protocol = "UDP", Purpose = "Additional game and BattlEye traffic used above the game port" },
                    new GamePortInfo { Port = "27016", Protocol = "UDP", Purpose = "Steam query (steamQueryPort in serverDZ.cfg), needed for the server browser" },
                },
                SetupTips = new()
                {
                    "The app starts DayZServer_x64.exe with -config=serverDZ.cfg -port=2302 -profiles=profiles -dologs -adminlog -netlog -freezecheck. The server name is not passed on the command line.",
                    "This is Steam app 1042420, not a beta branch of the stable server. It downloads anonymously, but a Steam account that owns DayZ is still needed to download Workshop mods.",
                    "Set hostname, password (join password), passwordAdmin and maxPlayers in serverDZ.cfg in the server folder.",
                    "Logs (.RPT, .ADM) are written to the profiles folder. The server does not print to its console window.",
                    "Open UDP 2302-2305 and the Steam query port (steamQueryPort, 27016 in common configs) in Firewall Rules or with Connect to Tunnel. If a stable DayZ server runs on the same PC, give this one different ports.",
                    "Players must switch to DayZ Experimental in their Steam library to join.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/221100/ss_bf0e5d5538d95a1cb943a7963489ba4df2782942.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/221100/ss_4b71f0ac633c88e8af8de1c4eec0b5b263f989cc.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/221100/ss_0487147f80e9350d9c655deb7f3d25b3876a1958.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/221100/ss_06f0e97dd31b322d38dfd137b268d793ff62f86f.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "deadpoly",
            Name = "DeadPoly Dedicated Server",
            Description = "Official DeadPoly dedicated server from TFL Games, installed through Valve's SteamCMD (anonymous). Server name and player slots are set in Game.ini.",
            Genres = new() { "Survival", "Shooter", "Building" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/1621070/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/1621070/library_hero.jpg",
                Accent = "#C2410C",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 2208380,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\DeadPoly\\Binaries\\Win64\\DeadPolyServer.exe",
                Arguments = "-log -stdout -FullStdOutLogOutput -unattended -port={server_port} -queryport=7778",
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
                About = "DeadPoly is a low-poly zombie survival looter shooter from TFL Games. Players scavenge for gear, build bases, craft, level up and take on quests, but the only real goal is to stay alive.\n\nThe DeadPoly Dedicated Server is a free Unreal Engine server tool. It keeps a world online for your group and stores its settings in Game.ini under the DeadPoly\\Saved folder.\n\nPlayers can find the server in the in-game browser or join directly by entering the IP and port in the Direct Connect field.",
                Developer = "TFL Games",
                Publisher = "TFL Games",
                ReleaseDate = "Jan 12, 2022",
                Website = "https://deadpoly.com/",
                StoreUrl = "https://store.steampowered.com/app/1621070/",
                DocsUrl = "https://deadpoly.fandom.com/wiki/Setting_up_a_Server_(Locally)",
                MaxPlayers = "Set by PlayerSlots in Game.ini",
                Features = new()
                {
                    "Official DeadPoly Dedicated Server, installed anonymously via SteamCMD (app 2208380)",
                    "Persistent world that stays online without a player hosting",
                    "Server name, player slots, private mode and admin IDs in Game.ini",
                    "Allow lists and bans for private servers",
                    "Direct connect by IP and port",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "UDP", Purpose = "Game traffic (-port)" },
                    new GamePortInfo { Port = "7778", Protocol = "UDP", Purpose = "Query port (-queryport), may not need forwarding" },
                },
                SetupTips = new()
                {
                    "The app starts DeadPoly\\Binaries\\Win64\\DeadPolyServer.exe with -log -stdout -FullStdOutLogOutput -unattended -port=7777 -queryport=7778.",
                    "Server settings live in DeadPoly\\Saved\\Config\\WindowsServer\\Game.ini, created after the first start. Set ServerName and PlayerSlots there, then restart.",
                    "To make the server private, enable the private setting in Game.ini and add the allowed player IDs. Admin player IDs are also added in Game.ini. Find a player's ID in the in-game Trust Menu or the server log.",
                    "Back up the DeadPoly\\Saved folder before updates. It holds the world and the config.",
                    "Open UDP 7777 in Firewall Rules or with Connect to Tunnel. Players can join with Direct Connect using IP:7777.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621070/ss_04e983a0cb2feff2b286bcc7277824140631d056.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621070/ss_cb437af137361fb55df4f09fff0ea05105c0128d.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621070/b81a815ea2627b1eef9f1b54e72b74d5a7cc3a74/ss_b81a815ea2627b1eef9f1b54e72b74d5a7cc3a74.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621070/b5efbd0cf206e825e8779523cabc118128b0bc27/ss_b5efbd0cf206e825e8779523cabc118128b0bc27.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621070/ss_a2d9483ba9d02f821fd0fafe7120f4ecb5562a53.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621070/ss_4da92dddd840d7c4de2249a3067c3dfa30aac067.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "eco",
            Name = "Eco Server",
            Description = "Official Eco dedicated server from Strange Loop Games, installed through Valve's SteamCMD (anonymous). Name, password and ports are set in Configs\\Network.eco.",
            Genres = new() { "Simulation", "Building", "Crafting" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/382310/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/382310/library_hero.jpg",
                Accent = "#4D7C0F",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 739590,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\EcoServer.exe",
                Arguments = "-nogui",
                WorkingDir = "{server_root}",
                StopCommand = "exit",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 3000,
                Protocol = "both",
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
                About = "Eco is a multiplayer ecosystem and civilization game from Strange Loop Games. Players work together to build a society, research technology and stop an incoming meteor, while every tree cut and every factory built affects a fully simulated ecosystem.\n\nAn Eco server runs one shared world. Players can set up laws, elections, currencies and a player-driven economy, and the world keeps evolving while they are offline.\n\nThe Eco Server tool is free through SteamCMD. It also runs a built-in web server with a map, statistics and an economy viewer on a second port.",
                Developer = "Strange Loop Games",
                Publisher = "Strange Loop Games",
                ReleaseDate = "Feb 6, 2018",
                Website = "https://www.play.eco/",
                StoreUrl = "https://store.steampowered.com/app/382310/",
                DocsUrl = "https://wiki.play.eco/en/Setting_Up_a_Server",
                MaxPlayers = "Set by MaxConnections in Configs\\Network.eco (-1 means no limit)",
                Features = new()
                {
                    "Official Eco Server, installed anonymously via SteamCMD (app 739590)",
                    "Simulated ecosystem shared by every player on the server",
                    "Player-made laws, governments, currencies and economy",
                    "Built-in web server with map, stats and economy viewer",
                    "Server name, password and ports in Configs\\Network.eco",
                    "Server-side C# mods in the Mods folder",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "3000", Protocol = "UDP", Purpose = "Game traffic (GameServerPort in Network.eco)" },
                    new GamePortInfo { Port = "3001", Protocol = "TCP", Purpose = "Web server and web map (WebServerPort in Network.eco)" },
                },
                SetupTips = new()
                {
                    "The app starts EcoServer.exe with -nogui, so it runs as a console without the server window. The ports are not passed on the command line.",
                    "Set Description (the server name), Password, PublicServer, GameServerPort and WebServerPort in Configs\\Network.eco. Stop the server before editing.",
                    "Add admins by Steam ID or SLG ID in Configs\\Users.eco. World generation and difficulty settings are in the other files in the Configs folder.",
                    "To list the server publicly, add a server user token from your play.eco account with -userToken=<token> in the launch settings.",
                    "The world is saved in the Storage folder. Back it up before updates.",
                    "Open UDP 3000 and TCP 3001 in Firewall Rules or with Connect to Tunnel.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/382310/ss_1274926dada7ac4f001d973fa7db6028c06c6b70.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/382310/ss_51131190262e47a1ad9160a3a727bc0957984875.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/382310/ss_a55f17232e3ed1b7735a1bdf00d370187e9991e3.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/382310/ss_87af3c40b53fd392989044d7ab15dcc6484c23d1.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/382310/ss_c5e44486fdf04dbfe1cd575960aaa0acd0ec82cb.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/382310/ss_fa99d3e2fcd43bfb60de4a9fec2bf3ddd221fcd9.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "empyrion-galactic-survival",
            Name = "Empyrion - Galactic Survival Dedicated Server",
            Description = "Official Empyrion dedicated server from Eleon Game Studios, installed through Valve's SteamCMD (anonymous). Name, password and player limit are set in dedicated.yaml.",
            Genres = new() { "Space", "Survival", "Building" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/383120/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/383120/library_hero.jpg",
                Accent = "#1E3A5F",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 530870,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\DedicatedServer\\EmpyrionDedicated.exe",
                Arguments = "-batchmode -nographics -dedicated dedicated.yaml -logFile server.log",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 30000,
                Protocol = "both",
                Query = "none",
                ConsoleMethod = "logfile",
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
                About = "Empyrion - Galactic Survival is a space sandbox survival game from Eleon Game Studios. Players build ships, bases and space stations block by block, explore planets and moons, and fight aliens and other players across a star system.\n\nThe free dedicated server keeps a galaxy online for your community. It starts extra playfield server processes as players visit planets and space sectors, so load grows with how spread out players are.\n\nAll settings live in dedicated.yaml in the server folder. Remote administration is done over a Telnet console that can be enabled in the same file.",
                Developer = "Eleon Game Studios",
                Publisher = "Eleon Game Studios",
                ReleaseDate = "Aug 5, 2020",
                Website = "https://empyriongame.com/",
                StoreUrl = "https://store.steampowered.com/app/383120/",
                DocsUrl = "https://empyrion.fandom.com/wiki/Dedicated_Server_Setup",
                MaxPlayers = "Set by Srv_MaxPlayers in dedicated.yaml",
                Features = new()
                {
                    "Official Empyrion Dedicated Server, installed anonymously via SteamCMD (app 530870)",
                    "Persistent galaxy with planets, moons and space sectors",
                    "Separate playfield server processes spread the load",
                    "Scenario and world seed support in dedicated.yaml",
                    "Telnet remote console with saveandexit for clean shutdowns",
                    "Admin and ban lists in adminconfig.yaml",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "30000", Protocol = "UDP", Purpose = "Game traffic (Srv_Port in dedicated.yaml)" },
                    new GamePortInfo { Port = "30001-30002", Protocol = "UDP", Purpose = "Game port + 1 and + 2, fixed offsets (30001 is needed for the server browser)" },
                    new GamePortInfo { Port = "30004", Protocol = "TCP", Purpose = "Telnet console (Tel_Port, only when Tel_Enabled is true; keep it closed to the internet)" },
                },
                SetupTips = new()
                {
                    "The app starts DedicatedServer\\EmpyrionDedicated.exe with -batchmode -nographics -dedicated dedicated.yaml -logFile server.log and shows server.log in the console.",
                    "Set Srv_Name, Srv_Password, Srv_MaxPlayers and Srv_Port under ServerConfig in dedicated.yaml. The server only reads this file at startup.",
                    "The server does not read commands from its console. Enable Tel_Enabled with a Tel_Pwd in dedicated.yaml and send saveandexit 0 over Telnet to save and stop cleanly.",
                    "Saves are written to the Saves folder in the server directory unless SaveDirectory in dedicated.yaml says otherwise. Admins are set in adminconfig.yaml.",
                    "Open UDP 30000-30002 in Firewall Rules or with Connect to Tunnel. Without 30001 the server may not show in the in-game browser.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/383120/ss_a709efe21c2d29f9c872539fcde3a47c35887ea8.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/383120/ss_a3b80f8f29ae2ad22e1a0dc2bf8c15e29f3918bb.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/383120/ss_ee3666fdcf68905dffe1a0a2f0b669d7b4c9232e.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/383120/ss_8906ee8a4e6eededc2392a9b5edfc6795ff34059.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/383120/ss_c018494c67298ae83fd2ad029b8534c07d886803.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/383120/ss_73b6075c42318a5b0e4d7f9b7de3ee31bb91193c.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "the-forest",
            Name = "The Forest Dedicated Server",
            Description = "Official The Forest dedicated server from Endnight Games, installed through Valve's SteamCMD (anonymous). No join password by default, set one in the launch settings or Server.cfg.",
            Genres = new() { "Survival", "Horror", "Co-op" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/242760/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/242760/library_hero.jpg",
                Accent = "#2F4F2F",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 556450,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\TheForestDedicatedServer.exe",
                Arguments = "-batchmode -nographics -nosteamclient -servergameport {server_port} -serverqueryport 27016 -serversteamport 8766 -servername \"{server_name}\" -serverplayers 8 -difficulty Normal -inittype Continue -slot 1 -logFile server.log",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 27015,
                Protocol = "udp",
                Query = "a2s",
                ConsoleMethod = "logfile",
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
                About = "The Forest is a first-person survival horror game from Endnight Games. After a plane crash on a remote peninsula, players build shelters, craft weapons and search caves for a missing son, while a society of cannibal mutants watches and attacks.\n\nThe free dedicated server keeps a co-op world running so friends can join and leave without one player hosting. It supports save slots, difficulty settings, autosaves and an admin password for in-game admin commands.\n\nSettings can be passed on the command line or stored in a Server.cfg file that the server creates on first launch.",
                Developer = "Endnight Games Ltd",
                Publisher = "Endnight Games Ltd",
                ReleaseDate = "Apr 30, 2018",
                Website = "https://survivetheforest.com/",
                StoreUrl = "https://store.steampowered.com/app/242760/",
                DocsUrl = "https://theforest.fandom.com/wiki/Dedicated_Servers",
                MaxPlayers = "Configurable via -serverplayers (this app launches with 8)",
                Features = new()
                {
                    "Official The Forest Dedicated Server, installed anonymously via SteamCMD (app 556450)",
                    "Persistent co-op world that stays online without a player hosting",
                    "Save slots 1-5 with new or continue start modes",
                    "Peaceful, Normal and Hard difficulty",
                    "Admin password for in-game admin commands",
                    "Optional VAC and join password",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Game traffic (-servergameport)" },
                    new GamePortInfo { Port = "27016", Protocol = "UDP", Purpose = "Steam query (-serverqueryport)" },
                    new GamePortInfo { Port = "8766", Protocol = "UDP", Purpose = "Steam communication (-serversteamport)" },
                },
                SetupTips = new()
                {
                    "The app starts TheForestDedicatedServer.exe with -batchmode -nographics -nosteamclient -servergameport 27015 -serverqueryport 27016 -serversteamport 8766 -servername, -serverplayers 8, -difficulty Normal, -inittype Continue, -slot 1 and -logFile server.log.",
                    "Add -serverpassword <password> for a join password and -serverpassword_admin <password> to allow in-game admin commands.",
                    "The server writes its Server.cfg and saves under %USERPROFILE%\\AppData\\LocalLow\\SKS\\TheForestDedicatedServer\\ds. Use -configfilepath and -savefolderpath to move them.",
                    "-inittype Continue loads the save in the chosen -slot. Use -inittype New to start a fresh world in that slot.",
                    "Open UDP 27015, 27016 and 8766 in Firewall Rules or with Connect to Tunnel.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/242760/ss_8ccb821c4df3fafdf4161d77f38635441a8157f2.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/242760/ss_53c615d49c4777144ed7359e4bf7c9eb6838cc8e.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/242760/ss_772eebf0ce7bdb51546055a36185e8ee46e8acac.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/242760/ss_d03a261fecab226a0ecac5746225c2a50d65c670.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/242760/ss_d77d402c78451a04b5c370e81ff7767c4008343c.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/242760/ss_a37e6873baf869be91010b20c30a7e61e4b82cc1.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "conan-exiles",
            Name = "Conan Exiles Dedicated Server",
            Description = "Official Conan Exiles Enhanced dedicated server from Funcom, installed through Valve's SteamCMD (anonymous). No join password by default, set one in the launch settings or Engine.ini.",
            Genres = new() { "Survival", "Open World", "Building" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/440900/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/440900/library_hero.jpg",
                Accent = "#9A6A33",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 443030,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\ConanSandbox\\Binaries\\Win64\\ConanSandboxServer-Win64-Shipping.exe",
                Arguments = "-log -Port={server_port} -QueryPort=27015 -ServerName=\"{server_name}\" -MaxPlayers=40",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 7777,
                Protocol = "both",
                Query = "a2s",
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
                About = "Conan Exiles is an open-world survival game from Funcom set in the lands of Hyboria. Players start as exiles in the desert, gather resources, build fortresses, tame thralls and pets, and fight monsters, gods and other players.\n\nIn 2026 the game was upgraded to Unreal Engine 5 as Conan Exiles Enhanced. The dedicated server keeps the same Steam app (443030), and the old Unreal Engine 4 build is still available on the conan-exiles-legacy branch.\n\nA dedicated server gives you control over PvP or PvE rules, harvest and XP rates, building limits, raid windows and admin access. Funcom recommends more RAM for Enhanced servers than for the older build.",
                Developer = "Funcom, Inflexion Games",
                Publisher = "Funcom",
                ReleaseDate = "May 8, 2018",
                Website = "https://www.conanexiles.com/",
                StoreUrl = "https://store.steampowered.com/app/440900/",
                DocsUrl = "https://www.conanexiles.com/dedicated-servers/",
                MaxPlayers = "Configurable via -MaxPlayers (this app launches with 40)",
                Features = new()
                {
                    "Official Conan Exiles Dedicated Server, installed anonymously via SteamCMD (app 443030)",
                    "Unreal Engine 5 Enhanced build, with the UE4 build on the conan-exiles-legacy branch",
                    "PvP, PvE and PvE-Conflict rules with raid time windows",
                    "Harvest, XP, crafting and decay rates in ServerSettings.ini",
                    "RCON remote administration",
                    "Steam Workshop mods with a modlist",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    RecommendedRamMb = 16384,
                    Notes = "Hosting guides citing Funcom recommend at least 16 GB RAM for Enhanced (UE5) servers, and more for 40+ players or heavy mod lists.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "UDP", Purpose = "Game traffic (-Port)" },
                    new GamePortInfo { Port = "7778", Protocol = "UDP", Purpose = "Pinger for the server browser (always game port + 1)" },
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Steam query (-QueryPort)" },
                    new GamePortInfo { Port = "7777", Protocol = "TCP", Purpose = "Mod downloads (game port plus an offset, opens on demand)" },
                    new GamePortInfo { Port = "25575", Protocol = "TCP", Purpose = "RCON (-RconPort, only if enabled)" },
                },
                SetupTips = new()
                {
                    "The app starts ConanSandbox\\Binaries\\Win64\\ConanSandboxServer-Win64-Shipping.exe with -log -Port=7777 -QueryPort=27015 -ServerName and -MaxPlayers=40.",
                    "Config files are in ConanSandbox\\Saved\\Config\\WindowsServer. Set ServerPassword under [OnlineSubsystem] in Engine.ini (or add -ServerPassword=\"...\") and AdminPassword under [ServerSettings] in ServerSettings.ini.",
                    "The world is saved to ConanSandbox\\Saved\\game_0.db on Enhanced (game.db on the legacy build). Stop the server before copying or replacing it.",
                    "To run the old UE4 version, install the conan-exiles-legacy branch. Players must be on the same version.",
                    "Open UDP 7777, 7778 and 27015 in Firewall Rules or with Connect to Tunnel. Open TCP 25575 only if you use RCON.",
                    "The server is ready when the log shows Started SourceServerQueries.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/440900/a37146febb3cdca83efbacec992da4aa9da74c33/ss_a37146febb3cdca83efbacec992da4aa9da74c33.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/440900/929bf96124bc0da976f65db3feb6efc23ad16aab/ss_929bf96124bc0da976f65db3feb6efc23ad16aab.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/440900/f607a98ef3454a45f1d33d0bf19de184911f131d/ss_f607a98ef3454a45f1d33d0bf19de184911f131d.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/440900/745df54d4f217268f0f83603983d61fbb4bb0d78/ss_745df54d4f217268f0f83603983d61fbb4bb0d78.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/440900/768e9fc630413e617b5524c3863f470ead538e6b/ss_768e9fc630413e617b5524c3863f470ead538e6b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/440900/212cb7c435803db08c35f875d8f166ed5327c21e/ss_212cb7c435803db08c35f875d8f166ed5327c21e.1920x1080.jpg",
                },
            },
        },
    };
}
