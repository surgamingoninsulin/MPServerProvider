using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public static partial class BuiltInGames
{
    private static IEnumerable<GameDefinition> ClassicGames() => new[]
    {
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "half-life",
            Name = "Half-Life Dedicated Server",
            Description = "Valve's original GoldSrc Half-Life Dedicated Server (HLDS), installed through SteamCMD (anonymous, app 90). Starts on Crossfire with no password.",
            Genres = new() { "Shooter", "Classic" },
            Artwork = new ArtworkSpec
            {
                Cover = SteamCdn + "70/header.jpg",
                Hero = SteamCdn + "70/library_hero.jpg",
                Accent = "#C8372D",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 90,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\hlds.exe",
                Arguments = "-console -game valve +maxplayers 16 +map crossfire -port {server_port}",
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
                Folder = "valve\\maps",
                Extensions = new() { ".bsp" },
                Kind = "map",
            },
            Details = new GameDetails
            {
                About = "Half-Life is Valve's 1998 GoldSrc shooter. The dedicated server (HLDS) hosts deathmatch and the many mods built on the same engine without a player hosting from the game client.\n\nHLDS is free to download through SteamCMD, no Half-Life purchase required. The -game flag picks which mod it runs: valve for base Half-Life Deathmatch, or a mod folder such as cstrike for Counter-Strike 1.6.",
                Developer = "Valve",
                Publisher = "Valve",
                ReleaseDate = "Nov 8, 1998",
                Website = "https://www.half-life.com/",
                StoreUrl = "https://store.steampowered.com/app/70/",
                DocsUrl = "https://developer.valvesoftware.com/wiki/Half-Life_Dedicated_Server",
                MaxPlayers = "Set with +maxplayers (default 16)",
                Features = new()
                {
                    "Official HLDS dedicated server, installed anonymously via SteamCMD (app 90)",
                    "Runs Half-Life Deathmatch or any GoldSrc mod installed in its own folder",
                    "Classic console commands over stdin (map, kick, say, quit)",
                    "Custom maps drop into valve\\maps as .bsp files",
                    "Lightweight: runs comfortably on very old hardware",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 256,
                    DiskGb = 1,
                    Notes = "HLDS is a 1998-era engine; almost any modern machine, including small VPS instances, runs it with room to spare.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Game traffic and server queries (-port)" },
                },
                SetupTips = new()
                {
                    "The app starts hlds.exe with -console -game valve +maxplayers 16 +map crossfire -port 27015. Change +map to any Half-Life Deathmatch map you have installed.",
                    "To run a GoldSrc mod instead (Counter-Strike 1.6, Team Fortress Classic, Day of Defeat...), add this app's matching game entry, or edit -game in the launch arguments after install.",
                    "Type console commands straight into the Console tab: they go to the server over stdin, same as typing at the HLDS window.",
                    "Open UDP 27015 in Firewall Rules or with Connect to Tunnel so players outside your network can join.",
                    "Server cvars go in valve\\server.cfg (hostname, sv_password, rcon_password). Create it if it doesn't exist.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/70/ss_b674f0a307aa961d7a0892ac8ba736714d1c3f37.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "counter-strike-16",
            Name = "Counter-Strike 1.6 Dedicated Server",
            Description = "Classic Counter-Strike 1.6, hosted on Valve's Half-Life Dedicated Server (HLDS) engine with -game cstrike, installed through SteamCMD (anonymous, app 90). Starts on de_dust2 with no password.",
            Genres = new() { "Shooter", "Classic", "Competitive" },
            Artwork = new ArtworkSpec
            {
                Cover = SteamCdn + "10/header.jpg",
                Hero = SteamCdn + "10/library_hero.jpg",
                Accent = "#D4A017",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 90,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\hlds.exe",
                Arguments = "-console -game cstrike +maxplayers 16 +map de_dust2 -port {server_port}",
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
                Folder = "cstrike\\maps",
                Extensions = new() { ".bsp" },
                Kind = "map",
            },
            Details = new GameDetails
            {
                About = "Counter-Strike 1.6 is the classic team-based shooter built on Valve's GoldSrc engine, pitting Terrorists against Counter-Terrorists across bomb-defusal and hostage-rescue maps.\n\nIts dedicated server is the same free HLDS download as Half-Life, pointed at the cstrike mod folder with -game cstrike. No Counter-Strike purchase is required to run the server.",
                Developer = "Valve / Minh \"Gooseman\" Le & Jess Cliffe",
                Publisher = "Valve",
                ReleaseDate = "Nov 9, 2000",
                Website = "https://store.steampowered.com/app/10/",
                StoreUrl = "https://store.steampowered.com/app/10/",
                DocsUrl = "https://developer.valvesoftware.com/wiki/Half-Life_Dedicated_Server",
                MaxPlayers = "Set with +maxplayers (default 16)",
                Features = new()
                {
                    "Official HLDS dedicated server running -game cstrike, installed anonymously via SteamCMD (app 90)",
                    "Classic bomb-defusal and hostage-rescue game modes",
                    "Console commands over stdin (map, kick, say, quit)",
                    "Custom maps drop into cstrike\\maps as .bsp files",
                    "Runs comfortably on minimal hardware",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 256,
                    DiskGb = 1,
                    Notes = "A 2000-era engine; any modern machine, including small VPS instances, runs it easily.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Game traffic and server queries (-port)" },
                },
                SetupTips = new()
                {
                    "The app starts hlds.exe with -console -game cstrike +maxplayers 16 +map de_dust2 -port 27015. Change +map to switch the starting map.",
                    "Type console commands straight into the Console tab: they go to the server over stdin, same as typing at the HLDS window.",
                    "Open UDP 27015 in Firewall Rules or with Connect to Tunnel so players outside your network can join.",
                    "Server cvars go in cstrike\\server.cfg (hostname, sv_password, rcon_password). Create it if it doesn't exist.",
                    "Custom maps need matching .res files alongside the .bsp so players download the right textures and models.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/10/ss_57dca044e8ff17c91b6c756f5b24f446c2211c6c.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "rising-world",
            Name = "Rising World Dedicated Server",
            Description = "Official Rising World dedicated server from brainarts, installed through SteamCMD (anonymous, app 339010). Port and world settings live in server.properties, created on first launch.",
            Genres = new() { "Sandbox", "Survival", "Voxel" },
            Artwork = new ArtworkSpec
            {
                Cover = SteamCdn + "324080/header.jpg",
                Hero = SteamCdn + "324080/library_hero.jpg",
                Accent = "#5C8A3A",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 339010,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\RisingWorldDedicatedServer.exe",
                Arguments = "",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 4255,
                Protocol = "both",
                Query = "none",
                ConsoleMethod = "stdin",
            },
            Mods = new ModsSpec
            {
                Supported = true,
                Folder = "plugins",
                Extensions = new() { ".jar" },
                Kind = "plugin",
            },
            Details = new GameDetails
            {
                About = "Rising World is a voxel-based sandbox from brainarts where players mine, build, farm and explore a fully destructible world, alone or with friends.\n\nThe free dedicated server app keeps a world online without a player hosting it from the game client. Settings, including the port, world name and player limit, live in server.properties next to the server executable.",
                Developer = "brainarts",
                Publisher = "brainarts",
                Website = "https://rising-world.net",
                StoreUrl = "https://store.steampowered.com/app/324080/",
                DocsUrl = "https://forum.rising-world.net/thread/11490",
                MaxPlayers = "Set with maxPlayers in server.properties",
                Features = new()
                {
                    "Official Rising World dedicated server, installed anonymously via SteamCMD (app 339010)",
                    "Fully destructible voxel world, persistent between sessions",
                    "Java/C# plugin support for custom server behavior",
                    "Optional RCON for remote administration",
                    "Everything configured in one server.properties file",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 2048,
                    DiskGb = 2,
                    Notes = "Disk use grows with explored/edited terrain chunks; budget extra space for long-running worlds.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "4255", Protocol = "TCP", Purpose = "Game traffic" },
                    new GamePortInfo { Port = "4255", Protocol = "UDP", Purpose = "Game traffic" },
                },
                SetupTips = new()
                {
                    "server.properties is created next to RisingWorldDedicatedServer.exe the first time the server runs. Stop the server, edit it, then start it again.",
                    "Set the port, world name, player limit and password under their matching keys in server.properties.",
                    "Open port 4255 (TCP and UDP) in Firewall Rules or with Connect to Tunnel. Players join with the server IP and port.",
                    "Plugins (.jar files) go in the plugins folder next to the server executable.",
                    "If the server executable name ever changes in a future Rising World update, point the launch arguments at the new file under the server's root folder.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/324080/ss_1e277cf3ce2e51e6b37a04f45a73cda71a49da2a.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "stationeers",
            Name = "Stationeers Dedicated Server",
            Description = "Official Stationeers dedicated server from Rocketwerkz, installed through SteamCMD (anonymous, app 600760). Settings live in rocketstation_DedicatedServer_Data\\settings.xml, created on first launch.",
            Genres = new() { "Survival", "Space", "Simulation" },
            Artwork = new ArtworkSpec
            {
                Cover = SteamCdn + "544550/header.jpg",
                Hero = SteamCdn + "544550/library_hero.jpg",
                Accent = "#E67E22",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 600760,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\rocketstation_DedicatedServer.exe",
                Arguments = "-batchmode -nographics -settings StartLocalHost true ServerName \"{server_name}\" ServerPort {server_port} -logfile server.log",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 27016,
                Protocol = "udp",
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
                About = "Stationeers is a space-station survival and engineering game from Rocketwerkz, with deep atmospherics, power and plumbing simulation.\n\nThe free dedicated server keeps a station online without a player hosting from the game client. It runs headless in batch mode and writes its console output to a log file instead of an interactive console.",
                Developer = "Rocketwerkz",
                Publisher = "Rocketwerkz",
                Website = "https://stationeers.com",
                StoreUrl = "https://store.steampowered.com/app/544550/",
                DocsUrl = "https://stationeers-wiki.com/Dedicated_Server_Guide",
                MaxPlayers = "Set in the server settings (MaxPlayers)",
                Features = new()
                {
                    "Official Stationeers dedicated server, installed anonymously via SteamCMD (app 600760)",
                    "Deep atmospherics, power and plumbing simulation keeps running while players are offline",
                    "Headless batch-mode server with log-file console output",
                    "Optional Steam beta branch for testing upcoming builds",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 4096,
                    DiskGb = 5,
                    Notes = "Stationeers' simulation is CPU-heavy; a faster single-thread CPU helps more than extra RAM beyond the minimum.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "27016", Protocol = "UDP", Purpose = "Game traffic (ServerPort in settings)" },
                },
                SetupTips = new()
                {
                    "The app starts rocketstation_DedicatedServer.exe in batch mode and follows server.log for console output; there's no interactive stdin console.",
                    "Settings live under rocketstation_DedicatedServer_Data in the server folder once the server has started at least once.",
                    "Open UDP 27016 in Firewall Rules or with Connect to Tunnel. If players can't connect, check server.log for the port the server actually bound to; some builds differ from the default.",
                    "A Steam beta branch is available for testing upcoming builds; most groups should stay on the default (stable) branch for a public server.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/544550/ss_7d0bc55f5a02fd2f00c33a8d59f4ab93c37e6f9a.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "stormworks",
            Name = "Stormworks Dedicated Server",
            Description = "Official Stormworks: Build and Rescue dedicated server from Geometa, installed through SteamCMD (anonymous, app 1247090). Settings live in server_config.xml, created on first launch.",
            Genres = new() { "Simulation", "Building", "Rescue" },
            Artwork = new ArtworkSpec
            {
                Cover = SteamCdn + "573090/header.jpg",
                Hero = SteamCdn + "573090/library_hero.jpg",
                Accent = "#3E6680",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 1247090,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\server64.exe",
                Arguments = "",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 25564,
                Protocol = "udp",
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
                About = "Stormworks: Build and Rescue is a vehicle-building and disaster-response simulation from Geometa, where players design their own boats, planes and rescue vehicles piece by piece.\n\nThe free dedicated server keeps a multiplayer world, including shared vehicles and missions, online without a player hosting from the game client.",
                Developer = "Geometa",
                Publisher = "Geometa",
                Website = "https://store.steampowered.com/app/573090/",
                StoreUrl = "https://store.steampowered.com/app/573090/",
                DocsUrl = "https://discourse.cubecoders.com/t/stormworks-setup-guide/5782",
                MaxPlayers = "Set in server_config.xml",
                Features = new()
                {
                    "Official Stormworks dedicated server, installed anonymously via SteamCMD (app 1247090)",
                    "Shared vehicles, missions and world state between sessions",
                    "Windows-only dedicated server binary (server64.exe)",
                    "Admin Steam IDs can be preset in server_config.xml",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 4096,
                    DiskGb = 5,
                    Notes = "Vehicle-heavy worlds with several players need more RAM; Stormworks' simulation also leans on CPU single-thread speed.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "25564-25566", Protocol = "UDP", Purpose = "Game traffic" },
                },
                SetupTips = new()
                {
                    "The app starts server64.exe with no arguments; console output is followed from the server's log file rather than an interactive console.",
                    "server_config.xml is created in the server folder after the first run. Stop the server, edit it (server name, port, save name, admin Steam IDs), then start it again.",
                    "Open UDP 25564-25566 in Firewall Rules or with Connect to Tunnel so players outside your network can join.",
                    "The dedicated server binary is Windows-only, which matches this app's own Windows-only hosting.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/573090/ss_7e622f36880d6d10a4fdbf8df9c76e1ed5222d21.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "satisfactory",
            Name = "Satisfactory Dedicated Server",
            Description = "Official Satisfactory dedicated server from Coffee Stain Studios, installed through SteamCMD (anonymous, app 1690800). No password by default; manage it through the in-game Server Manager.",
            Genres = new() { "Factory", "Building", "Survival" },
            Artwork = new ArtworkSpec
            {
                Cover = SteamCdn + "526870/header.jpg",
                Hero = SteamCdn + "526870/library_hero.jpg",
                Accent = "#F39C12",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 1690800,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\FactoryServer.exe",
                Arguments = "-Port={server_port} -ReliablePort=8888 -log -unattended",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 7777,
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
                About = "Satisfactory is Coffee Stain Studios' first-person factory-building game, set on an alien planet players industrialize from the ground up, alone or with friends.\n\nThe free dedicated server keeps a save running without a player hosting from the game client. It's administered entirely from the in-game Server Manager once you connect, rather than a text config file.",
                Developer = "Coffee Stain Studios",
                Publisher = "Coffee Stain Studios",
                ReleaseDate = "Sep 10, 2024",
                Website = "https://www.satisfactorygame.com",
                StoreUrl = "https://store.steampowered.com/app/526870/",
                DocsUrl = "https://satisfactory.wiki.gg/wiki/Dedicated_servers",
                MaxPlayers = "Configurable in the Server Manager (no hard cap)",
                Features = new()
                {
                    "Official Satisfactory dedicated server, installed anonymously via SteamCMD (app 1690800)",
                    "Administered in-game through the Server Manager, no config file editing needed",
                    "Runs unattended with its own crash-safe autosave",
                    "Supports the same mods as the client when added through the Server Manager",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 8192,
                    RecommendedRamMb = 16384,
                    DiskGb = 15,
                    Notes = "Large, heavily-built factories need significantly more RAM than the minimum; Coffee Stain recommends 16 GB or more for bigger saves.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "UDP", Purpose = "Game traffic (-Port)" },
                    new GamePortInfo { Port = "7777", Protocol = "TCP", Purpose = "Server Manager API" },
                    new GamePortInfo { Port = "8888", Protocol = "TCP", Purpose = "Reliable messaging (-ReliablePort)" },
                },
                SetupTips = new()
                {
                    "The app starts FactoryServer.exe with -Port=7777 -ReliablePort=8888 -log -unattended. Open both ports, not just 7777, or clients can connect but desync.",
                    "First connect to the server from the in-game main menu's Server Manager (its address is the server's IP and port 7777) to create or load a save and set a password.",
                    "Open UDP+TCP 7777 and TCP 8888 in Firewall Rules or with Connect to Tunnel so players outside your network can join.",
                    "Saves live under the server's %LOCALAPPDATA%\\FactoryGame\\Saved\\SaveGames\\server folder; back that up before updates.",
                    "Mods installed through the Server Manager need the same mods on every connecting client.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/526870/ss_273d5d8f5e60a41fda4c25c69d9b69c6fbb79e50.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "terraria",
            Name = "Terraria Dedicated Server",
            Description = "Re-Logic's official standalone Terraria server (TerrariaServer.exe), set up from the files you download yourself from terraria.org — no Steam account or ownership check needed to run a server.",
            Genres = new() { "Sandbox", "Adventure", "2D" },
            Artwork = new ArtworkSpec
            {
                Cover = SteamCdn + "105600/header.jpg",
                Hero = SteamCdn + "105600/library_hero.jpg",
                Accent = "#6FA86F",
            },
            Install = new InstallSpec
            {
                Provider = "manual",
                Variants = new(),
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\TerrariaServer.exe",
                Arguments = "-port {server_port} -players 8 -world \"{server_root}\\world.wld\" -autocreate 2 -worldname World",
                WorkingDir = "{server_root}",
                StopCommand = "exit",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 7777,
                Protocol = "tcp",
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
                About = "Terraria is Re-Logic's 2D sandbox adventure, mixing mining, building, crafting and boss fights across a procedurally generated world.\n\nRe-Logic ships TerrariaServer.exe as a free standalone download, separate from the paid game, so anyone can run a dedicated server without owning Terraria themselves. It's a console app that reads typed commands and autosaves the world on a timer.",
                Developer = "Re-Logic",
                Publisher = "Re-Logic",
                ReleaseDate = "May 16, 2011",
                Website = "https://terraria.org",
                StoreUrl = "https://store.steampowered.com/app/105600/",
                DocsUrl = "https://terraria.wiki.gg/wiki/Dedicated_Server",
                MaxPlayers = "Set with -players (default 8, max 255)",
                Features = new()
                {
                    "Official standalone TerrariaServer.exe, free and separate from the paid game",
                    "No Steam account or game ownership needed to host",
                    "Console commands over stdin (save, exit, kick, ban, time, settle...)",
                    "Autosaves the world on a timer, and on a clean exit",
                    "Autocreates a new world on first start if none is found",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 512,
                    RecommendedRamMb = 1024,
                    DiskGb = 1,
                    Notes = "Very light; large worlds with many NPCs and players benefit from the recommended RAM, but Terraria runs comfortably on modest hardware.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "TCP", Purpose = "Game traffic (-port)" },
                },
                SetupTips = new()
                {
                    "This game installs from files you download yourself: get the \"Dedicated Server\" package from terraria.org (bottom of the page), unzip it, then pick that folder when installing here.",
                    "The app starts TerrariaServer.exe with -port 7777 -players 8 -world \"<server folder>\\world.wld\" -autocreate 2 -worldname World. It creates a medium world automatically if world.wld doesn't exist yet.",
                    "Type console commands straight into the Console tab (save, exit, kick <name>, ban <name>, time day/night, password <pw>...): they go to the server over stdin.",
                    "Open TCP 7777 in Firewall Rules or with Connect to Tunnel so players outside your network can join.",
                    "Keep the server's TerrariaServer.exe version matched to what Re-Logic currently ships at terraria.org — mismatched client/server versions refuse to connect.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/105600/ss_9c0db1480909d59d8e4c05ccea38a6fd4d5301f2.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "vintage-story",
            Name = "Vintage Story Dedicated Server",
            Description = "Vintage Story's own VintagestoryServer.exe, downloaded straight from Anego Studios' public CDN — no Steam, account login or manual download needed.",
            Genres = new() { "Survival", "Sandbox", "Crafting" },
            Artwork = new ArtworkSpec
            {
                // Official game logo banner and an in-game screenshot from vintagestory.at's own
                // press kit / media page (not Steam — Vintage Story isn't sold there).
                Cover = "https://media.vintagestory.at/monthly_2018_02/gamelogo-vintagestory-banner.png.05569bcb37e8009e6c751d354a4dd033.png",
                Hero = "https://media.vintagestory.at/monthly_2024_12/2024-12-04_18-33-17.jpg.1d9a95995179fbf21292b3184ca44f63.jpg",
                Accent = "#8B5A2B",
            },
            Install = new InstallSpec
            {
                Provider = "url",
                Variants = new(),
                // The account/download site gates its download *page* behind a login (and
                // actively blocks non-browser requests with a 403), but the actual CDN files it
                // links to are plain public GETs. The wiki's Version history page is a public,
                // unprotected table of every released version (newest first), so the Version
                // dropdown's list and the chosen download both come from scraping that instead.
                Url = "https://cdn.vintagestory.at/gamefiles/stable/vs_server_win-x64_{version}.zip",
                Archive = "zip",
                VersionListUrl = "https://wiki.vintagestory.at/Version_history",
                VersionListPattern = @"<a href=""/(?<version>\d+\.\d+\.\d+)""",
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\VintagestoryServer.exe",
                Arguments = "--dataPath \"{server_root}\\data\" --port {server_port}",
                WorkingDir = "{server_root}",
                StopCommand = "/stop",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 42420,
                Protocol = "both",
                Query = "none",
                ConsoleMethod = "stdin",
            },
            Mods = new ModsSpec
            {
                Supported = true,
                Folder = "data\\Mods",
                Extensions = new() { ".zip", ".cs", ".dll" },
                Kind = "mod",
            },
            Details = new GameDetails
            {
                About = "Vintage Story is a wilderness-survival sandbox from Anego Studios, built around realistic geology, seasons and a deep crafting and building system, with no paid-publisher storefront.\n\nThe free standalone VsServer package (VintagestoryServer.exe) keeps a persistent world online for a group to build in together, without needing the full game client installed on the machine that hosts it.",
                Developer = "Anego Studios",
                Publisher = "Anego Studios",
                Website = "https://www.vintagestory.at",
                DocsUrl = "https://wiki.vintagestory.at/Guide:Dedicated_Server",
                MaxPlayers = "Set with --maxclients",
                Features = new()
                {
                    "Downloaded automatically from Anego Studios' public CDN, always the current stable version",
                    "Realistic geology, seasons and temperature simulation",
                    "Server-side mods (.zip, .cs, .dll) load from data\\Mods",
                    "Console commands over stdin, same as the server's own console window",
                    "Per-server data path keeps settings and saves separate from any client install",
                    "Built-in UPnP router setup (/serverconfig upnp 1) as a free alternative to manual port forwarding",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 1024,
                    RecommendedRamMb = 2048,
                    DiskGb = 3,
                    Notes = "Official recommendation: 1 GB RAM plus ~300 MB per player, and a CPU with 4 threads (1 GHz plus ~150 MHz per player). Needs the .NET runtime version Vintage Story currently targets installed on the machine.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "42420", Protocol = "TCP", Purpose = "Game traffic" },
                    new GamePortInfo { Port = "42420", Protocol = "UDP", Purpose = "Game traffic" },
                },
                SetupTips = new()
                {
                    "The app downloads the current stable VsServer release straight from Anego Studios' CDN — no account login or manual file picking needed.",
                    "The app starts VintagestoryServer.exe with --dataPath \"<server folder>\\data\" --port 42420. Settings, worlds and mods all live under that data folder, separate from any client data.",
                    "Type console commands straight into the Console tab (/stop, /whitelist, /time, /worldconfig, /serverconfig upnp 1...): they go to the server over stdin.",
                    "Try /serverconfig upnp 1 in the console first to open the port automatically; if that fails, open TCP+UDP 42420 in Firewall Rules or use Connect to Tunnel so players outside your network can join.",
                    "Server-side mods (.zip, .cs or .dll) go in data\\Mods. Players need matching client-side mods where a mod affects them.",
                },
                Screenshots = new()
                {
                    "https://media.vintagestory.at/monthly_2024_12/2024-12-04_18-33-17.jpg.1d9a95995179fbf21292b3184ca44f63.jpg",
                    "https://media.vintagestory.at/monthly_2024_12/2024-01-16_14-08-41.jpg.3c279fb15643896c948f3f40b74619ad.jpg",
                    "https://media.vintagestory.at/monthly_2024_12/2024-10-08_09-52-05.jpg.31906ccffc26126ad65a55d24be4116a.jpg",
                    "https://media.vintagestory.at/monthly_2024_12/2023-11-29_00-16-18.jpg.e8f2d72fde39c3780189fecf6aea7048.jpg",
                },
            },
        },
    };
}
