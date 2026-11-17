using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public static partial class BuiltInGames
{
    private static IEnumerable<GameDefinition> SurvivalDGames() => new[]
    {
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "starbound",
            Name = "Starbound Dedicated Server",
            Description = "Starbound's starbound_server.exe from Chucklefish, installed through Valve's SteamCMD. The app asks for your Steam login when you install, because only accounts that own Starbound can download it. No password is set by default.",
            Genres = new() { "Sandbox", "Adventure", "Space" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/211820/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/211820/library_hero.jpg",
                Accent = "#1E2A55",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                // "unstable" picks Starbound's public Steam beta branch of the same name instead
                // of the default stable branch; SteamCmdProvider applies it via -beta.
                Variants = new() { "stable", "unstable" },
                AppId = 211820,
                Anonymous = false,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\win64\\starbound_server.exe",
                Arguments = "",
                WorkingDir = "{server_root}\\win64",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 21025,
                Protocol = "tcp",
                Query = "none",
                ConsoleMethod = "stdin",
            },
            Mods = new ModsSpec
            {
                Supported = true,
                Folder = "mods",
                Extensions = new() { ".pak" },
                Kind = "mod",
            },
            Details = new GameDetails
            {
                About = "Starbound is a 2D sandbox adventure from Chucklefish. Players explore a procedurally generated universe, travel between planets in their own ship, build bases, farm, craft gear and follow a story campaign.\n\nThe game ships with its own dedicated server, starbound_server.exe, so a group can share one persistent universe without anyone hosting from the game client. Planets, ships and characters stay on the server between sessions.\n\nThe server reads its settings from starbound_server.config in the storage folder. Accounts with passwords, admin rights, the player limit and optional RCON and query servers are all set there.",
                Developer = "Chucklefish",
                Publisher = "Chucklefish",
                ReleaseDate = "Jul 22, 2016",
                Website = "https://www.playstarbound.com",
                StoreUrl = "https://store.steampowered.com/app/211820/",
                DocsUrl = "https://starbounder.org/Guide:Setting_Up_Multiplayer",
                MaxPlayers = "8 players by default (maxPlayers in the config)",
                Features = new()
                {
                    "Official starbound_server.exe from the Starbound Steam depot (app 211820)",
                    "Persistent shared universe with planets, ships and characters saved on the server",
                    "Server accounts with passwords and admin rights (serverUsers)",
                    "Optional RCON server for remote commands (runRconServer)",
                    "Mods load from .pak files in the mods folder",
                    "Player limit and anonymous access set in starbound_server.config",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "21025", Protocol = "TCP", Purpose = "Game traffic (gameServerPort)" },
                    new GamePortInfo { Port = "21026", Protocol = "TCP", Purpose = "RCON, only when runRconServer is true (rconServerPort)" },
                },
                SetupTips = new()
                {
                    "The app launches win64\\starbound_server.exe with no arguments from the win64 folder. The port, server name and player limit come from storage\\starbound_server.config, which is created on first start.",
                    "Installing asks for a Steam login on an account that owns Starbound. The password only goes to SteamCMD and is never saved; if Steam asks for a Steam Guard code, enter it in the same window.",
                    "Pick \"unstable\" in the Variant list to install Starbound's public beta branch instead of the stable release. Switching branches reinstalls the server on that branch.",
                    "Open TCP 21025 in Firewall Rules or with Connect to Tunnel. Players connect with your IP and that port.",
                    "To require a password or make an admin, stop the server and add an entry under serverUsers in storage\\starbound_server.config, for example \"name\": { \"admin\": true, \"password\": \"secret\" }. Set allowAnonymousConnections to false to block guests.",
                    "Universe saves live in storage\\universe and the log is storage\\starbound_server.log. Back up the storage folder before updates.",
                    "Drop mod .pak files into the mods folder in the server root. Players need the same mods.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/211820/ss_2dcc1e0c07f7597edc6556b31b8df9902fddac1a.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/211820/ss_c8bb844f59b490bbfee6d179879e47272044ca34.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/211820/ss_af82dfaaa6521c92cb311f7ba54ecf5257bf880e.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/211820/ss_a911c976aeb84fcd25c8866eba08e1ca642d25d4.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/211820/ss_974323d818a04ed7657aa87eef74f8cf0811548a.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/211820/ss_d825ae99fa02dca4c770fef9a9429f6f3fc4598d.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "survive-the-nights",
            Name = "Survive the Nights Dedicated Server",
            Description = "Official Survive the Nights dedicated server from a2z Interactive, installed through Valve's SteamCMD (anonymous). No password is set by default, add one in ServerConfig.txt.",
            Genres = new() { "Survival", "Horror", "Open World" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/541300/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/541300/library_hero.jpg",
                Accent = "#5A1414",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 1502300,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\STN_Dedicated_Server.exe",
                Arguments = "-batchmode -nographics",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 7950,
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
                About = "Survive the Nights is an open world survival game from a2z Interactive. Players scavenge for food and gear, build and fortify bases, drive vehicles and try to last through nights when the infected become far more dangerous.\n\nA dedicated server keeps the world, bases and player progress online for your group. It runs on its own without a player hosting from the game client.\n\nThe server reads ServerConfig.txt for its name, password, port, day length, loot and horde settings. Owner and admin rights are kept in ServerUsers.txt and UserPermissions.json.",
                Developer = "a2z Interactive",
                Publisher = "a2z Interactive",
                ReleaseDate = "Dec 5, 2025",
                Website = "https://www.survivethenights.net",
                StoreUrl = "https://store.steampowered.com/app/541300/",
                DocsUrl = "https://linuxgsm.com/servers/stnserver/",
                MaxPlayers = "Set in the server config",
                Features = new()
                {
                    "Official dedicated server tool, installed anonymously via SteamCMD (app 1502300)",
                    "Persistent world with bases, vehicles and player progress",
                    "Day length, time persistence and weather settings",
                    "Loot, zombie, horde and vehicle spawn rates in ServerConfig.txt",
                    "Optional PvP and public lobby listing",
                    "Owner and admin permissions through ServerUsers.txt and UserPermissions.json",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7950", Protocol = "UDP", Purpose = "Game traffic (ServerPort)" },
                    new GamePortInfo { Port = "7951", Protocol = "UDP", Purpose = "Steam query (game port + 1)" },
                },
                SetupTips = new()
                {
                    "The app launches STN_Dedicated_Server.exe with -batchmode -nographics. The port, name and password come from ServerConfig.txt, not from launch arguments.",
                    "Start the server once so it writes its config files, then stop it and edit ServerConfig.txt (LinuxGSM keeps it in the Config folder of the server root).",
                    "Set ServerName, ServerPassword (blank means public) and ServerPort (default 7950) in ServerConfig.txt.",
                    "Put your SteamID64 in ServerOwner before the first start. The server writes it to ServerUsers.txt and gives you owner rights.",
                    "Open UDP 7950-7951 in Firewall Rules or with Connect to Tunnel.",
                    "Set ShowInPublicLobby to false to keep the server off the public list.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/541300/ss_7c95eafcbde15d2f75228da12e36024af321151c.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/541300/ss_e5af7f185039c4a60cc1310f0fc007e2171ca47e.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/541300/ss_d9433bd561495f74174e10629e4bdd24a8d90277.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/541300/ss_f681e099f8fed3c65c29369596aef16a947ede5e.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/541300/ss_7b16b7598bb2b1e3832fce26971dc5e82665606a.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/541300/ss_59bd79621f917c7163a938ca31170fbdd227508a.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "vein",
            Name = "VEIN Dedicated Server",
            Description = "Official VEIN dedicated server from Ramjet Studios, installed through Valve's SteamCMD (anonymous). No password is set by default, add one in Game.ini.",
            Genres = new() { "Survival", "Horror", "Open World" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/1857950/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/1857950/library_hero.jpg",
                Accent = "#4A4F3A",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 2131400,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\Vein\\Binaries\\Win64\\VeinServer-Win64-Test.exe",
                Arguments = "-log -Port={server_port} -QueryPort=27015",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 7777,
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
                About = "VEIN is a post-apocalyptic zombie survival game from Ramjet Studios. Players scavenge a large open world, manage hunger, thirst and mental state, repair vehicles and fortify buildings against the infected.\n\nThe free VEIN Dedicated Server tool keeps the world running for your group around the clock. Bases, loot and player progress are saved on the server.\n\nServer settings live in Game.ini, and gameplay rules such as PvP, decay, hordes and zombie damage are console variables in Engine.ini. The game is in Early Access, so settings may change between updates.",
                Developer = "Ramjet Studios",
                Publisher = "Ramjet Studios",
                ReleaseDate = "Oct 24, 2025",
                Website = "https://vein.gg",
                StoreUrl = "https://store.steampowered.com/app/1857950/",
                DocsUrl = "https://ramjet.notion.site/Config-279f9ec29f178011a909f8ea9525936d",
                MaxPlayers = "16 players by default (MaxPlayers in Game.ini)",
                Features = new()
                {
                    "Official VEIN Dedicated Server, installed anonymously via SteamCMD (app 2131400)",
                    "Persistent world with saved bases, loot and characters",
                    "Admin and super admin lists by SteamID64 in Game.ini",
                    "Optional whitelist of allowed players",
                    "Gameplay console variables for PvP, decay, hordes and zombie strength (Engine.ini)",
                    "Steam server browser listing with an optional password",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "UDP", Purpose = "Game traffic (-Port)" },
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Steam query and server list (-QueryPort / GameServerQueryPort)" },
                },
                SetupTips = new()
                {
                    "The app launches Vein\\Binaries\\Win64\\VeinServer-Win64-Test.exe with -log -Port=7777 -QueryPort=27015. Keep the = signs if you edit the arguments.",
                    "Open UDP 7777 and UDP 27015 in Firewall Rules or with Connect to Tunnel.",
                    "Server settings are in Vein\\Saved\\Config\\WindowsServer\\Game.ini, created on first start. Set ServerName, Password and bPublic under [/Script/Vein.VeinGameSession] and MaxPlayers under [/Script/Engine.GameSession].",
                    "Add admins with AdminSteamIDs or SuperAdminSteamIDs (SteamID64) under [/Script/Vein.VeinGameSession]. Extra entries go on new lines that start with +.",
                    "Saves are in Vein\\Saved\\SaveGames (server.save and manifest.save). Stop the server before editing configs or copying saves.",
                    "Gameplay rules such as vein.PvP and vein.BuildObjectDecay go under [ConsoleVariables] in Engine.ini in the same folder.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1857950/4cf2ffb8af7825c85d05e8a23487d931c1cb43a7/ss_4cf2ffb8af7825c85d05e8a23487d931c1cb43a7.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1857950/85a5497dd8a19f3f8c8def786914299382d8ebb6/ss_85a5497dd8a19f3f8c8def786914299382d8ebb6.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1857950/2769f4bc3a669f24d6a445de16a2a6f4515d0cef/ss_2769f4bc3a669f24d6a445de16a2a6f4515d0cef.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1857950/e2c2e1f3a9e6e50f4f20bb7d1563aa0d113e557c/ss_e2c2e1f3a9e6e50f4f20bb7d1563aa0d113e557c.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1857950/05d559ad45f0ace5f6b7ec382ea1259ce0db960a/ss_05d559ad45f0ace5f6b7ec382ea1259ce0db960a.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1857950/c127178b04634c260eafa544d6b5ded43bca7eb6/ss_c127178b04634c260eafa544d6b5ded43bca7eb6.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "scum",
            Name = "SCUM Dedicated Server",
            Description = "Official SCUM Server Tool from Gamepires, installed through Valve's SteamCMD (anonymous). No password is set by default, add one in ServerSettings.ini. Players connect on the game port + 2.",
            Genres = new() { "Survival", "Open World", "Shooter" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/513710/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/513710/library_hero.jpg",
                Accent = "#7A6A2E",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 3792580,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\SCUM\\Binaries\\Win64\\SCUMServer.exe",
                Arguments = "-log -Port={server_port} -QueryPort=27015 -MaxPlayers=64",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 7777,
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
                About = "SCUM is an open world survival game from Gamepires. Prisoners are dropped on a large island where they manage detailed body metabolism, scavenge, craft, build bases and fight puppets, mechs and other players.\n\nThe SCUM Server Tool lets you host your own persistent island. Bases, vehicles, squads and characters are kept on the server, and you control loot, PvP and the rest of the rules.\n\nMost settings are in ServerSettings.ini, which the server writes on first start. Admin, whitelist and ban lists are separate .ini files in the same folder.",
                Developer = "Gamepires",
                Publisher = "Gamepires",
                ReleaseDate = "Jun 17, 2025",
                Website = "https://scum.game",
                StoreUrl = "https://store.steampowered.com/app/513710/",
                DocsUrl = "https://scum.wiki.gg/wiki/Server_Settings",
                MaxPlayers = "Up to 64 players (-MaxPlayers)",
                Features = new()
                {
                    "Official SCUM Server Tool, installed anonymously via SteamCMD (app 3792580)",
                    "Persistent island with bases, vehicles and squads",
                    "Detailed rule settings in ServerSettings.ini",
                    "Admin, whitelist, ban and exclusive user lists as .ini files",
                    "Optional join password and PvP or PvE rules",
                    "Listed in the in-game server browser through the Steam query port",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    RecommendedRamMb = 12288,
                    Notes = "Hosting guides suggest 8 to 12 GB RAM for 20 or more players. Usage grows with population, bases and vehicles.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777-7779", Protocol = "UDP", Purpose = "Game traffic. -Port sets the base, players connect on the base port + 2 (7779)" },
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Steam query and server list (-QueryPort)" },
                },
                SetupTips = new()
                {
                    "The app launches SCUM\\Binaries\\Win64\\SCUMServer.exe with -log -Port=7777 -QueryPort=27015 -MaxPlayers=64.",
                    "Players connect with your IP and the game port + 2, so 7779 by default. Open UDP 7777-7779 and UDP 27015 in Firewall Rules or with Connect to Tunnel.",
                    "Settings are in SCUM\\Saved\\Config\\WindowsServer\\ServerSettings.ini, written on first start. Set scum.ServerName and scum.ServerPassword there, then restart.",
                    "Add admin SteamID64s, one per line, to AdminUsers.ini in the same folder. ServerSettingsAdminUsers.ini separately grants access to the in-game settings panel.",
                    "Saves are in SCUM\\Saved\\SaveGames. Stop the server before editing configs or copying saves.",
                    "If SteamCMD reports state 0x6 after an update, run the update again.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/513710/ss_0b75f5f5aa10f00a0d3e9776b15477fe3f9b6eb5.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/513710/ss_62f56143b1c35cb843a4f3e29290a09176532ee5.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/513710/ss_ecfffcc4a177083e0ed0727319f77019624f076a.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/513710/ef7f226e863f6e82d77550875f8296803d914f4b/ss_ef7f226e863f6e82d77550875f8296803d914f4b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/513710/ss_facfc7ebc5dfd485cd269888d996ec51cdaae434.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/513710/b8347f1c3a78f04c66fde7a794c1c11baf3e9e57/ss_b8347f1c3a78f04c66fde7a794c1c11baf3e9e57.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "palworld",
            Name = "Palworld Dedicated Server",
            Description = "Official Palworld Dedicated Server Tool from Pocketpair, installed through Valve's SteamCMD (anonymous). No password is set by default, add ServerPassword and AdminPassword in PalWorldSettings.ini.",
            Genres = new() { "Survival", "Crafting", "Open World" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/1623730/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/1623730/library_hero.jpg",
                Accent = "#2C7BB6",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 2394010,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\Pal\\Binaries\\Win64\\PalServer-Win64-Shipping-Cmd.exe",
                Arguments = "-port={server_port} -players=32",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 8211,
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
                About = "Palworld is a survival crafting game from Pocketpair. Players catch creatures called Pals, put them to work at their bases, build and defend settlements, and fight or explore across a large open world.\n\nThe free Palworld Dedicated Server tool keeps a shared world running for up to 32 players. Bases, Pals and guilds stay on the server, so nobody needs to host from their own game.\n\nAll world rules, from experience rates to PvP and death penalties, are set in PalWorldSettings.ini. The server can also be listed in the in-game community server list and managed through RCON or a REST API.",
                Developer = "Pocketpair",
                Publisher = "Pocketpair",
                ReleaseDate = "Jul 9, 2026",
                Website = "https://www.pocketpair.jp/palworld",
                StoreUrl = "https://store.steampowered.com/app/1623730/",
                DocsUrl = "https://docs.palworldgame.com/getting-started/deploy-dedicated-server",
                MaxPlayers = "Up to 32 players",
                Features = new()
                {
                    "Official Palworld Dedicated Server Tool, installed anonymously via SteamCMD (app 2394010)",
                    "Shared world for up to 32 players with bases, Pals and guilds",
                    "Detailed world rules in PalWorldSettings.ini",
                    "Optional community server listing (-publiclobby)",
                    "Remote admin through RCON and a REST API",
                    "Automatic backups of save data (bIsUseBackupSaveData)",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 8192,
                    RecommendedRamMb = 16384,
                    Notes = "Pocketpair docs: 4+ CPU cores and 16 GB RAM recommended, 32 GB or more is better. 8 GB can boot but risks out-of-memory crashes. A fast SSD is recommended, slow storage can corrupt saves.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "8211", Protocol = "UDP", Purpose = "Game traffic (-port)" },
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Steam query for the community server list (only with -publiclobby)" },
                    new GamePortInfo { Port = "25575", Protocol = "TCP", Purpose = "RCON, only when RCONEnabled=True (RCONPort)" },
                },
                SetupTips = new()
                {
                    "The app launches Pal\\Binaries\\Win64\\PalServer-Win64-Shipping-Cmd.exe with -port=8211 -players=32. Add -publiclobby to list the server as a community server.",
                    "Open UDP 8211 in Firewall Rules or with Connect to Tunnel. Players join with your IP and port.",
                    "Start the server once, stop it, then copy DefaultPalWorldSettings.ini from the server root to Pal\\Saved\\Config\\WindowsServer\\PalWorldSettings.ini and edit the copy. Editing the default file has no effect.",
                    "Set ServerName, ServerPassword and AdminPassword in PalWorldSettings.ini. Players use /AdminPassword in chat to gain admin rights.",
                    "World saves are in Pal\\Saved\\SaveGames. Back them up before updates.",
                    "The REST API (RESTAPIEnabled, port 8212) has no protection beyond the admin password. Do not expose it to the internet.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1623730/ss_f81b7c4f20be3b99f76a1415c4cdb9b444c99b97.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1623730/648ed4266fc18f413292292741304ef648421c55/ss_648ed4266fc18f413292292741304ef648421c55.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1623730/ss_b3cea7c9f04a67d784d4c6a0c157a11d6268b189.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1623730/cd7b46028d10c21db8fc8217192094d309efcebc/ss_cd7b46028d10c21db8fc8217192094d309efcebc.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1623730/ss_06e27c15c7b4b10233c937b887cf6a6925c83009.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1623730/ss_a99fba5536acde781bd863cb3555c10b5b96c0ae.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "enshrouded",
            Name = "Enshrouded Dedicated Server",
            Description = "Official Enshrouded Dedicated Server from Keen Games, installed through Valve's SteamCMD (anonymous). Group passwords are random on first start, check enshrouded_server.json.",
            Genres = new() { "Survival", "Crafting", "RPG" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/1203620/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/1203620/library_hero.jpg",
                Accent = "#5B3A7A",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 2278520,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\enshrouded_server.exe",
                Arguments = "",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 15637,
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
                About = "Enshrouded is a co-op survival action RPG from Keen Games. Players explore a world covered by a deadly fog called the Shroud, fight its creatures, craft gear and build bases with a voxel building system.\n\nThe free Enshrouded Dedicated Server keeps the world online for up to 16 players. Bases and progress stay on the server, so the group can play without the original host.\n\nThe server is configured through enshrouded_server.json, which is written on first start. Access is controlled by user groups, each with its own password and permissions.",
                Developer = "Keen Games GmbH",
                Publisher = "Keen Games GmbH",
                ReleaseDate = "Oct 15, 2026",
                Website = "https://www.enshrouded.com",
                StoreUrl = "https://store.steampowered.com/app/1203620/",
                DocsUrl = "https://enshrouded.zendesk.com/hc/en-us",
                MaxPlayers = "Up to 16 players",
                Features = new()
                {
                    "Official Enshrouded Dedicated Server, installed anonymously via SteamCMD (app 2278520)",
                    "Persistent world for up to 16 players",
                    "User groups with separate passwords and permissions",
                    "Difficulty presets (Default, Relaxed, Hard, Survival, Custom)",
                    "Optional voice and text chat",
                    "Single UDP port for game and query traffic",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    RecommendedRamMb = 16384,
                    DiskGb = 30,
                    Notes = "Keen Games figures: 6 cores / 12 threads for 4 to 6 players, 8 cores / 16 threads for 16 players, 16 GB RAM, SSD with 30 GB free and about 2 Mbit/s upload per player. Windows only.",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "15637", Protocol = "UDP", Purpose = "Game traffic and Steam query (queryPort)" },
                },
                SetupTips = new()
                {
                    "The app launches enshrouded_server.exe with no arguments. The name, port, slot count and passwords all come from enshrouded_server.json in the server root.",
                    "Start the server once so it writes enshrouded_server.json, then stop it before editing. Changes made while it runs are lost.",
                    "Passwords are set per group in userGroups. Each group has a random password on a new file, and the password a player enters decides their permissions. Give the admin group password only to admins.",
                    "Open UDP 15637 in Firewall Rules or with Connect to Tunnel. Players can add the server in Steam favourites with your IP and 15637. Older guides also mention 15636, current builds use one port.",
                    "Saves are in the savegame folder and logs in the logs folder next to the server exe.",
                    "Set slotCount (1 to 16) and gameSettingsPreset to change the player limit and difficulty.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1203620/09a1e99c47ba7ffb2413c4708a9f5fbef1397652/ss_09a1e99c47ba7ffb2413c4708a9f5fbef1397652.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1203620/5d3de290f5b9fc2b850260c73483e15f501087f7/ss_5d3de290f5b9fc2b850260c73483e15f501087f7.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1203620/962286f68343e1d42d6868079b617b7e7e384fca/ss_962286f68343e1d42d6868079b617b7e7e384fca.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1203620/507e6719722e1094417e953a1274110deecc3397/ss_507e6719722e1094417e953a1274110deecc3397.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1203620/f214fd4869c74e1f99c18b03f771e9c624814188/ss_f214fd4869c74e1f99c18b03f771e9c624814188.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1203620/c591ae8cfbb033f7930eb22d1482691da648d324/ss_c591ae8cfbb033f7930eb22d1482691da648d324.1920x1080.jpg",
                },
            },
        },
    };
}
