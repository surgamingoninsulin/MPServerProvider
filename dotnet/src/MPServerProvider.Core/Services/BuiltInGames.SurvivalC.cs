using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public static partial class BuiltInGames
{
    private static IEnumerable<GameDefinition> SurvivalCGames() => new[]
    {
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "left-4-dead",
            Name = "Left 4 Dead Dedicated Server",
            Description = "Valve's Source Dedicated Server for the original Left 4 Dead, installed through Valve's SteamCMD (anonymous). Starts on No Mercy with no password.",
            Genres = new() { "Shooter", "Co-op", "Horror" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/500/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/500/library_hero.jpg",
                Accent = "#B91C1C",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 222840,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\srcds.exe",
                Arguments = "-console -game left4dead +hostname \"{server_name}\" +map l4d_hospital01_apartment -port {server_port} -norestart",
                WorkingDir = "{server_root}",
                StopCommand = "quit",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 27015,
                Protocol = "both",
                Query = "a2s",
                ConsoleMethod = "stdin",
            },
            Mods = new ModsSpec
            {
                Supported = true,
                Folder = "left4dead\\addons",
                Extensions = new() { ".vpk" },
                Kind = "addon",
            },
            Details = new GameDetails
            {
                About = "Left 4 Dead is Valve's co-op zombie shooter. Four survivors fight through five campaigns against hordes of infected, while the AI Director changes enemy placement, items and pacing on every run.\n\nA dedicated server runs Valve's Source Dedicated Server (SRCDS) for Left 4 Dead. It hosts Co-op, Versus and Survival games without one player hosting from the game client, and gives better connection quality than a listen server.\n\nThe server is free to download through SteamCMD. Custom campaigns and add-ons are .vpk files placed in the addons folder. Every player needs the same add-ons installed.",
                Developer = "Valve",
                Publisher = "Valve",
                ReleaseDate = "Nov 17, 2008",
                Website = "https://www.l4d.com/",
                StoreUrl = "https://store.steampowered.com/app/500/",
                DocsUrl = "https://developer.valvesoftware.com/wiki/Source_Dedicated_Server",
                MaxPlayers = "4 survivors in Co-op, 8 players in Versus",
                Features = new()
                {
                    "Official SRCDS dedicated server, installed anonymously via SteamCMD (app 222840)",
                    "Co-op, Versus and Survival game modes",
                    "AI Director keeps every run different",
                    "Drop-in .vpk add-ons and custom campaigns in left4dead\\addons",
                    "server.cfg for cvars such as hostname, rcon_password and sv_password",
                    "Valve Anti-Cheat enabled",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Game traffic and server queries (-port)" },
                    new GamePortInfo { Port = "27015", Protocol = "TCP", Purpose = "RCON remote console" },
                },
                SetupTips = new()
                {
                    "The app starts srcds.exe with -console -game left4dead +hostname \"<server name>\" +map l4d_hospital01_apartment -port 27015 -norestart.",
                    "Put cvars in left4dead\\cfg\\server.cfg. Set sv_password to require a password and rcon_password for remote admin.",
                    "Players join through the lobby or by typing connect <ip>:27015 in the developer console. Add +sv_allow_lobby_connect_only 0 if direct connects are refused.",
                    "Change the starting campaign with +map, for example l4d_vs_hospital01_apartment for Versus.",
                    "Place add-on .vpk files in left4dead\\addons. Players must have the same add-ons.",
                    "Allow 27015 (UDP for players, TCP for RCON) in Firewall Rules or with Connect to Tunnel.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/500/0000006028.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/500/0000006029.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/500/0000006030.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/500/0000006031.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/500/0000006032.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/500/0000006033.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "left-4-dead-2",
            Name = "Left 4 Dead 2 Dedicated Server",
            Description = "Valve's Source Dedicated Server for Left 4 Dead 2, installed through Valve's SteamCMD (anonymous). Starts on Dead Center with no password.",
            Genres = new() { "Shooter", "Co-op", "Horror" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/550/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/550/library_hero.jpg",
                Accent = "#DC2626",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 222860,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\srcds.exe",
                Arguments = "-console -game left4dead2 +hostname \"{server_name}\" +map c1m1_hotel -port {server_port} -norestart",
                WorkingDir = "{server_root}",
                StopCommand = "quit",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 27015,
                Protocol = "both",
                Query = "a2s",
                ConsoleMethod = "stdin",
            },
            Mods = new ModsSpec
            {
                Supported = true,
                Folder = "left4dead2\\addons",
                Extensions = new() { ".vpk" },
                Kind = "addon",
            },
            Details = new GameDetails
            {
                About = "Left 4 Dead 2 is Valve's co-op zombie shooter set in the American South. Four survivors fight through campaigns with melee weapons, new special infected and changing weather, while the AI Director adjusts the challenge on every run.\n\nA dedicated server runs Valve's Source Dedicated Server (SRCDS) for Left 4 Dead 2. It hosts Co-op, Versus, Survival, Scavenge and Realism games, plus Mutations, without one player hosting from the game client.\n\nThe server is free to download through SteamCMD. Custom campaigns and Workshop add-ons are .vpk files placed in the addons folder. Every player needs the same add-ons installed.",
                Developer = "Valve",
                Publisher = "Valve",
                ReleaseDate = "Nov 16, 2009",
                Website = "https://www.l4d.com/",
                StoreUrl = "https://store.steampowered.com/app/550/",
                DocsUrl = "https://developer.valvesoftware.com/wiki/Source_Dedicated_Server",
                MaxPlayers = "4 survivors in Co-op, 8 players in Versus and Scavenge",
                Features = new()
                {
                    "Official SRCDS dedicated server, installed anonymously via SteamCMD (app 222860)",
                    "Co-op, Versus, Survival, Scavenge, Realism and Mutation modes",
                    "All official campaigns, including The Passing and The Sacrifice",
                    "Drop-in .vpk add-ons and custom campaigns in left4dead2\\addons",
                    "server.cfg for cvars such as hostname, rcon_password and sv_password",
                    "Valve Anti-Cheat enabled",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Game traffic and server queries (-port)" },
                    new GamePortInfo { Port = "27015", Protocol = "TCP", Purpose = "RCON remote console" },
                },
                SetupTips = new()
                {
                    "The app starts srcds.exe with -console -game left4dead2 +hostname \"<server name>\" +map c1m1_hotel -port 27015 -norestart.",
                    "Put cvars in left4dead2\\cfg\\server.cfg. Set sv_password to require a password and rcon_password for remote admin.",
                    "Players join through the lobby or by typing connect <ip>:27015 in the developer console. Add +sv_allow_lobby_connect_only 0 if direct connects are refused.",
                    "Pick the mode with +mp_gamemode (coop, versus, survival, scavenge or realism) and the start map with +map.",
                    "Place add-on .vpk files in left4dead2\\addons. Players must have the same add-ons.",
                    "Allow 27015 (UDP for players, TCP for RCON) in Firewall Rules or with Connect to Tunnel.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/550/ss_2eae29fbdfe8e5e8999b96d8bb28c5db70507968.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/550/ss_29b3b4f2a3994c889f6fc12e0781d9d4726ef33f.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/550/ss_9488e329bb42d792a059fb44cb7135d25b6262f5.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/550/ss_6ec4ee04d4924b099e25ce79f3d6571c3b623b3b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/550/ss_73ad69168a263ce585e4823d9607d901719ca3c5.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/550/ss_a9c1227ffe8a1968e234b3ebda08e773fc619e37.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "memories-of-mars",
            Name = "Memories of Mars Dedicated Server",
            Description = "Official Memories of Mars private server from Limbic Entertainment, installed through Valve's SteamCMD (anonymous). Name, password and admins are set in DedicatedServerConfig.cfg.",
            Genres = new() { "Survival", "Crafting", "Open World" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/644290/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/644290/library_hero.jpg",
                Accent = "#C2410C",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 897590,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\Game\\Binaries\\Win64\\MemoriesOfMarsServer.exe",
                Arguments = "-log -port={server_port}",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 7777,
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
                About = "Memories of Mars is a multiplayer survival game from Limbic Entertainment set on a colonised Mars. Players wake up as clones, scavenge abandoned facilities, craft gear, build bases and fight other players and rogue machines.\n\nThe free dedicated server tool lets you run a private PvE or PvP world with your own rules. Server settings live in DedicatedServerConfig.cfg, and CustomRuleSet.cfg changes item spawns, AI, meteors, building and player rules.\n\nThe server keeps its world in a file-based database. It restarts itself twice a day at the hour set in DailyRestartUTCHour, which the developers say is required for stability.",
                Developer = "Limbic Entertainment GmbH",
                Publisher = "505 Games",
                ReleaseDate = "Mar 12, 2020",
                Website = "https://www.memoriesofmars.com",
                StoreUrl = "https://store.steampowered.com/app/644290/",
                DocsUrl = "https://505games.com/wp-content/uploads/Handbook-Dedicated-Servers-Memories-of-Mars-V1.12.pdf",
                MaxPlayers = "Set by MaxPlayers in DedicatedServerConfig.cfg (default 64)",
                Features = new()
                {
                    "Official dedicated server, installed anonymously via SteamCMD (app 897590)",
                    "PvE or PvP worlds, with optional PvP areas around events",
                    "CustomRuleSet.cfg for loot, AI, meteor, building and player rules",
                    "Whitelist, blacklist and admin lists using Mars account IDs",
                    "In-game admin tools window (F8) for admins",
                    "EasyAntiCheat support (EnableEAC)",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "TCP+UDP", Purpose = "Game port players connect to (-port)" },
                    new GamePortInfo { Port = "15000", Protocol = "TCP+UDP", Purpose = "Beacon port for the initial connection (-beaconport)" },
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Steam query port for the server list (-queryport)" },
                },
                SetupTips = new()
                {
                    "The app starts Game\\Binaries\\Win64\\MemoriesOfMarsServer.exe with -log -port=7777. Other ports can be changed with -queryport and -beaconport.",
                    "Edit DedicatedServerConfig.cfg in the server folder before the first start. ServerName and ServerID are required, ServerPassword is optional.",
                    "Add your Mars account ID (shown in the top right of the game's main menu) to Admins, then press F8 in game to open the admin tools.",
                    "The server shuts down twice a day at DailyRestartUTCHour and DailyRestartUTCHour + 12. Pick an hour outside your play time.",
                    "Logs and saves are written to Game\\Saved. The main log is Game\\Saved\\Logs\\Game.log.",
                    "Open 7777, 15000 and 27015 in Firewall Rules or with Connect to Tunnel for a public server.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/644290/ss_1f28b02237135cf8eb73c2f2339bfc8a6cf6efd3.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/644290/ss_462407cb4d99a083d9f136735977e80c1abdbcf8.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/644290/ss_e5d1ce677c99a96da8a097e94b6aa9b56f4a8e04.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/644290/ss_ca03b552d0cc0fba0db76088905aecb6532ddf0e.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/644290/ss_e63375273166ce69349bf70621fb6002da4f6b6b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/644290/ss_fe59d1a74cc629d07fa20cce1876264ad945fbe8.1920x1080.jpg",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 5120,
                    DiskGb = 3,
                    Notes = "The handbook asks for 2 CPU cores or threads, about 5 GB of RAM in normal operation and about 3 GB of disk, which grows with logs and the database.",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "medieval-engineers",
            Name = "Medieval Engineers Dedicated Server",
            Description = "Official Medieval Engineers dedicated server from Keen Software House, installed through Valve's SteamCMD (anonymous). Runs in console mode; settings live in MedievalEngineers-Dedicated.cfg.",
            Genres = new() { "Sandbox", "Building", "Survival" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/333950/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/333950/library_hero.jpg",
                Accent = "#92400E",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 367970,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\DedicatedServer64\\MedievalEngineersDedicated.exe",
                Arguments = "-console -port {server_port}",
                WorkingDir = "{server_root}\\DedicatedServer64",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 27016,
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
                About = "Medieval Engineers is a sandbox game from Keen Software House about engineering, construction and maintenance of medieval buildings and machines. Structures use realistic physics, so walls, bridges and siege engines can collapse under their own weight.\n\nThe dedicated server hosts a persistent survival or creative world for a group of players. Admins, mods, world settings and the optional Remote API are set in MedievalEngineers-Dedicated.cfg.\n\nThe server tool is free to download through SteamCMD and runs on Windows. Mods are added by Steam Workshop ID in the server config.",
                Developer = "Keen Software House",
                Publisher = "Keen Software House",
                ReleaseDate = "Feb 19, 2015",
                Website = "https://www.medievalengineers.com/",
                StoreUrl = "https://store.steampowered.com/app/333950/",
                DocsUrl = "https://medievalengineers.com/dedicated-servers",
                MaxPlayers = "Configurable in the server config or with -maxPlayers",
                Features = new()
                {
                    "Official dedicated server, installed anonymously via SteamCMD (app 367970)",
                    "Console mode that skips the configuration dialogs (-console)",
                    "Workshop mods listed by ID in the server config",
                    "Admin list by 64-bit Steam ID, with kick and ban rights",
                    "Optional Steam group restriction and pause-when-empty",
                    "Optional HTTP Remote API for remote management",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "27016", Protocol = "UDP", Purpose = "Game traffic (-port)" },
                    new GamePortInfo { Port = "8080", Protocol = "TCP", Purpose = "Remote API (only if enabled)" },
                },
                SetupTips = new()
                {
                    "The app starts DedicatedServer64\\MedievalEngineersDedicated.exe with -console -port 27016.",
                    "The config file MedievalEngineers-Dedicated.cfg is in C:\\ProgramData\\MedievalEngineersDedicated by default. Add -path \"<folder>\" to keep config and saves in another folder.",
                    "To configure the world and admins with a window, run MedievalEngineersDedicated.exe once without -console, save the settings, then start the server from the app.",
                    "Add server admins as 64-bit Steam IDs and mods as Workshop IDs in the config file.",
                    "To host a world made in the game, copy it from %AppData%\\MedievalEngineers into the dedicated server saves folder and set LoadWorld in the config.",
                    "Open UDP 27016 in Firewall Rules or with Connect to Tunnel. Stop the server before updating it.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/333950/ss_6d5b5ab7822280b356a5919cc0b2e4cb97cdf716.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/333950/ss_32f14ca8ad0b86da0960e5db6ed8f00849173c58.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/333950/ss_92973312a930b9cc201b588bac38f56ff581fdfe.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/333950/ss_cc18889a81ab8a4de58ae7850a83c4eafea68e90.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/333950/ss_3b9b97c8a4e141ecd84429ebf78aaa5b7186e7c2.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/333950/ss_0ad7d502fddc20cbdf09e2b16d8685e22202b792.1920x1080.jpg",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    DiskGb = 3,
                    Notes = "Windows with .NET Framework 4.6.1 or newer. A static IP is recommended for internet play.",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "project-zomboid",
            Name = "Project Zomboid Dedicated Server",
            Description = "Official Project Zomboid dedicated server from The Indie Stone, installed through Valve's SteamCMD (anonymous). The admin password is 'mpserver' by default, change it after the first start.",
            Genres = new() { "Survival", "Horror", "Open World" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/108600/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/108600/library_hero.jpg",
                Accent = "#7F1D1D",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 380870,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\StartServer64.bat",
                Arguments = "-adminpassword mpserver",
                WorkingDir = "{server_root}",
                StopCommand = "quit",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 16261,
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
                About = "Project Zomboid is an open-world zombie survival game from The Indie Stone. Players loot, farm, craft, fortify buildings and try to survive as long as possible in Knox Country, a large map based on rural Kentucky.\n\nA dedicated server keeps a shared world running for a group. Server rules live in servertest.ini and world rules in servertest_SandboxVars.lua, both in the Zomboid\\Server folder of the user running the server.\n\nThe server is free to download through SteamCMD. Workshop mods are added by listing their Workshop and mod IDs in the server .ini file.",
                Developer = "The Indie Stone",
                Publisher = "The Indie Stone",
                ReleaseDate = "Nov 8, 2013",
                Website = "https://www.projectzomboid.com",
                StoreUrl = "https://store.steampowered.com/app/108600/",
                DocsUrl = "https://pzwiki.net/wiki/Dedicated_server",
                MaxPlayers = "Set by MaxPlayers in servertest.ini",
                Features = new()
                {
                    "Official dedicated server, installed anonymously via SteamCMD (app 380870)",
                    "Persistent shared world with PvP on or off",
                    "Sandbox settings for zombie count, speed, loot and world decay",
                    "Steam Workshop mods through WorkshopItems and Mods in the .ini",
                    "Admin commands from the server console or in-game chat",
                    "Optional RCON for remote admin",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "16261", Protocol = "UDP", Purpose = "Game traffic (DefaultPort in servertest.ini)" },
                    new GamePortInfo { Port = "16262", Protocol = "UDP", Purpose = "Direct connection port (UDPPort in servertest.ini)" },
                    new GamePortInfo { Port = "27015", Protocol = "TCP", Purpose = "RCON (only if RCONPort and RCONPassword are set)" },
                },
                SetupTips = new()
                {
                    "The app runs StartServer64.bat with -adminpassword mpserver. The script only forwards two arguments, so set ports and the server name in the .ini instead of on the command line.",
                    "Settings are in %USERPROFILE%\\Zomboid\\Server\\servertest.ini and saves in %USERPROFILE%\\Zomboid\\Saves\\Multiplayer\\servertest. Set Password= in the .ini to make the server private.",
                    "Log in as admin with the password above and change it, or remove -adminpassword after the first start.",
                    "StartServer64.bat sets the Java heap with -Xms and -Xmx. Lower them if the PC has less RAM than the script asks for.",
                    "Add mods with WorkshopItems= (Workshop IDs) and Mods= (mod IDs) in servertest.ini.",
                    "Open UDP 16261 and 16262 in Firewall Rules or with Connect to Tunnel.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/108600/e0dd37d69051f7da85896d11027ab018d23893fa/ss_e0dd37d69051f7da85896d11027ab018d23893fa.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/108600/d9df2102dd12526700b15660bc5ec8f039dc8742/ss_d9df2102dd12526700b15660bc5ec8f039dc8742.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/108600/4c8860818ac52c685bbe0988aa137d52bad4aac5/ss_4c8860818ac52c685bbe0988aa137d52bad4aac5.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/108600/ss_eb1862af5109e4658e2538d897cbd16b87ad1818.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/108600/ss_507e952789822b69868d768e1e4476946f37b1fb.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/108600/ss_eca8be032b3f5508bf5bea74cfbc823a4df047ce.1920x1080.jpg",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    DiskGb = 8,
                    Notes = "Memory use depends on the -Xmx value in StartServer64.bat and the number of players and mods.",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "smalland",
            Name = "Smalland Dedicated Server",
            Description = "Official Smalland: Survive the Wilds dedicated server from Max Ent Games, installed through Valve's SteamCMD (anonymous). No password is set by default.",
            Genres = new() { "Survival", "Crafting", "Co-op" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/768200/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/768200/library_hero.jpg",
                Accent = "#4D7C0F",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 808040,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\SMALLAND\\Binaries\\Win64\\SMALLANDServer-Win64-Shipping.exe",
                Arguments = "SMALLAND /Game/Maps/WorldGame/WorldGame_Smalland?SERVERNAME=\"{server_name}\"?WORLDNAME=\"World\"?lengthofdayseconds=1800?lengthofseasonseconds=10800 -ini:Engine:[EpicOnlineServices]:DeploymentId=50f2b148496e4cbbbdeefbecc2ccd6a3 -ini:Engine:[EpicOnlineServices]:DedicatedServerClientId=xyza78918KT08TkA6emolUay8yhvAAy2 -ini:Engine:[EpicOnlineServices]:DedicatedServerClientSecret=aN2GtVw7aHb6hx66HwohNM+qktFaO3vtrLSbGdTzZWk -ini:Engine:[EpicOnlineServices]:DedicatedServerPrivateKey= -port={server_port} -NOSTEAM -log",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 7777,
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
                About = "Smalland: Survive the Wilds is a multiplayer survival game where players are tiny people in a world of giant insects and plants. They gather resources, craft armour from creature parts, build hidden bases and tame and ride creatures.\n\nThe free dedicated server keeps a world online for up to ten players. World name, password, friendly fire, keep inventory, day length, season length and creature modifiers are set as options on the server's start line.\n\nThe server uses Epic Online Services for its server list, so the start line includes the EOS settings from the official start-server.bat.",
                Developer = "Max Ent Games Ltd",
                Publisher = "Max Ent Games Ltd",
                ReleaseDate = "Feb 15, 2024",
                Website = "https://maximument.com/games/smalland-survive-the-wilds/",
                StoreUrl = "https://store.steampowered.com/app/768200/",
                DocsUrl = "https://www.ionos.com/digitalguide/server/know-how/smalland-server/",
                MaxPlayers = "Up to 10 players",
                Features = new()
                {
                    "Official dedicated server, installed anonymously via SteamCMD (app 808040)",
                    "Persistent world that stays online without a player hosting",
                    "Start-line options for password, friendly fire, keep inventory and private mode",
                    "Adjustable day and season length",
                    "Creature health, creature damage, hunger and fall damage modifiers",
                    "Worlds saved as .wld files that can be moved between servers",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "TCP+UDP", Purpose = "Game traffic (-port)" },
                    new GamePortInfo { Port = "7778", Protocol = "TCP+UDP", Purpose = "Query port" },
                },
                SetupTips = new()
                {
                    "The app starts SMALLAND\\Binaries\\Win64\\SMALLANDServer-Win64-Shipping.exe with the world map, ?SERVERNAME=\"<server name>\"?WORLDNAME=\"World\", day length 1800 s, season length 10800 s, the EOS settings from start-server.bat, -port=7777, -NOSTEAM and -log.",
                    "Add ?PASSWORD=\"yourpassword\" after the WORLDNAME option to make the server private. Add ?FRIENDLYFIRE, ?KEEPINVENTORY, ?PEACEFULMODE or ?PRIVATE to turn those on.",
                    "Worlds are saved in SMALLAND\\Saved\\SaveGames\\Worlds as .wld files. To load an existing world, copy its .wld file there and set WORLDNAME to its name without .wld.",
                    "start-server.bat in the server folder shows the official start line. Compare it after updates in case the EOS settings change.",
                    "Open 7777 and 7778 in Firewall Rules or with Connect to Tunnel.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/768200/ss_a8181478ebe43df02ff321e19a7061da510a0644.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/768200/ss_8367f4da58a7200a4b3bc7ea8a1da92cfe5b455d.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/768200/ss_f0d99c1761214634ff988d58ac51fc81aa6b0d79.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/768200/ss_a9a574425cbf492f9a56f68a6c5bf2aa7a9d63d5.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/768200/ss_cc4ee74c7c9e4f3db422a23cfbab15f2ac0e11cf.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/768200/ss_1631e81472ca4a096db003bebe8554386ea89f62.1920x1080.jpg",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    DiskGb = 3,
                    Notes = "The Windows server download is about 2.2 GB.",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "sons-of-the-forest",
            Name = "Sons of the Forest Dedicated Server",
            Description = "Official Sons of the Forest dedicated server from Endnight Games, installed through Valve's SteamCMD (anonymous). No password is set by default.",
            Genres = new() { "Survival", "Horror", "Co-op" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/1326470/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/1326470/library_hero.jpg",
                Accent = "#3F6212",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 2465200,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\SonsOfTheForestDS.exe",
                Arguments = "-dedicatedserver.ServerName \"{server_name}\" -dedicatedserver.GamePort \"{server_port}\"",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 8766,
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
                About = "Sons of the Forest is a survival horror game from Endnight Games. Players are sent to find a missing billionaire on a remote island full of cannibals and mutants, and must build shelter, craft weapons and survive the seasons.\n\nThe free dedicated server tool keeps a co-op world online for up to eight players. It reads its settings from dedicatedserver.cfg, a JSON file, and every setting can be overridden on the command line with -dedicatedserver.<Key>.\n\nAt every start the server checks its config, gets its public IP from Steam and tests whether its ports are reachable. If a check fails it stops and prints the reason.",
                Developer = "Endnight Games Ltd",
                Publisher = "Newnight",
                ReleaseDate = "Feb 22, 2024",
                Website = "https://endnightgames.com/",
                StoreUrl = "https://store.steampowered.com/app/1326470/",
                DocsUrl = "https://steamcommunity.com/sharedfiles/filedetails/?id=2992700419",
                MaxPlayers = "Up to 8 players",
                Features = new()
                {
                    "Official dedicated server, installed anonymously via SteamCMD (app 2465200)",
                    "JSON config (dedicatedserver.cfg) with command-line overrides",
                    "Normal, hard, hard survival, peaceful, creative and custom game modes",
                    "Save slots with continue or new save modes and timed autosaves",
                    "Server owners listed by Steam ID in ownerswhitelist.txt",
                    "Startup self-test that reports closed ports",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "8766", Protocol = "UDP", Purpose = "Game traffic (GamePort)" },
                    new GamePortInfo { Port = "27016", Protocol = "UDP", Purpose = "Steam query (QueryPort)" },
                    new GamePortInfo { Port = "9700", Protocol = "UDP", Purpose = "Save and world data sync (BlobSyncPort)" },
                },
                SetupTips = new()
                {
                    "The app starts SonsOfTheForestDS.exe with -dedicatedserver.ServerName \"<server name>\" -dedicatedserver.GamePort \"8766\". Any other key can be added the same way.",
                    "The first start creates dedicatedserver.cfg and ownerswhitelist.txt in %USERPROFILE%\\AppData\\LocalLow\\Endnight\\SonsOfTheForestDS. Add -userdatapath \"<folder>\" to keep them and the Saves folder elsewhere.",
                    "Set Password in dedicatedserver.cfg or add -dedicatedserver.Password \"yourpassword\" to make the server private.",
                    "Add your 64-bit Steam ID to ownerswhitelist.txt to get owner rights in game.",
                    "Open UDP 8766, 27016 and 9700 in Firewall Rules or with Connect to Tunnel. The server will not start if its self-test finds a closed port, unless SkipNetworkAccessibilityTest is true.",
                    "GameMode only applies when a new save is created. Change SaveSlot or use SaveMode new to start a fresh world.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1326470/ss_e6e3ab1badfb287a77fb6eef7b1589a35371496b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1326470/ss_4fa5d260318f0aa37754b00c5dc10d1b7b9b9b1d.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1326470/ss_2b67491ca913adea6953429e1a567824aaa0ed54.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1326470/ss_ba2c1f6e69c6c7d69f7965cbd4b78da59ec59d55.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1326470/ss_2c537a8c689dd6b21643b8054a64646334ef7391.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1326470/ss_b05310f659a67129e9089fbb22e445b0998d6d1d.1920x1080.jpg",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    DiskGb = 4,
                    Notes = "Windows-only server. The download is about 3.3 GB.",
                },
            },
        },
    };
}
