using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public static partial class BuiltInGames
{
    private static IEnumerable<GameDefinition> SurvivalAGames() => new[]
    {
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "abiotic-factor",
            Name = "Abiotic Factor Dedicated Server",
            Description = "Official Abiotic Factor dedicated server from Deep Field Games, installed through Valve's SteamCMD (anonymous). No join password is set by default.",
            Genres = new() { "Survival", "Crafting", "Co-op" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/427410/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/427410/library_hero.jpg",
                Accent = "#F59E0B",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 2857200,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\AbioticFactor\\Binaries\\Win64\\AbioticFactorServer-Win64-Shipping.exe",
                Arguments = "-log -useperfthreads -PORT={server_port} -QueryPort=27015 -MaxServerPlayers=6 -SteamServerName=\"{server_name}\" -WorldSaveName=Cascade",
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
                About = "Abiotic Factor is a survival crafting game from Deep Field Games for 1 to 6 players. You play as a scientist trapped in GATE Cascade, an underground research facility overrun by paranormal entities, soldiers and creatures from other worlds.\n\nPlayers scavenge office supplies and lab equipment, craft weapons and armour, build bases inside the facility and work through its sectors together. The game left early access with its 1.0 release in July 2025.\n\nThe free Abiotic Factor Dedicated Server tool keeps a world online without a player hosting. It supports a server password, an admin password, sandbox settings per world and mod.io mods.",
                Developer = "Deep Field Games",
                Publisher = "Playstack",
                ReleaseDate = "Jul 22, 2025",
                StoreUrl = "https://store.steampowered.com/app/427410/",
                DocsUrl = "https://github.com/DFJacob/AbioticFactorDedicatedServer/wiki",
                MaxPlayers = "1 to 24 via -MaxServerPlayers (6 recommended; this app launches with 6)",
                Features = new()
                {
                    "Official dedicated server tool, installed anonymously via SteamCMD (app 2857200)",
                    "Persistent world that stays online without a player hosting",
                    "Server password and in-game admin password options",
                    "Per-world SandboxSettings.ini for difficulty and rules",
                    "Admin.ini moderator list by Steam ID",
                    "mod.io mod support through -Mods and -ModioToken",
                    "Crossplay with console players unless -PlatformLimited is set",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "UDP", Purpose = "Game traffic (-PORT)" },
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Steam query and server browser listing (-QueryPort)" },
                },
                SetupTips = new()
                {
                    "The app launches AbioticFactor\\Binaries\\Win64\\AbioticFactorServer-Win64-Shipping.exe with -log -useperfthreads -PORT=<port> -QueryPort=27015 -MaxServerPlayers=6 -SteamServerName=\"<name>\" -WorldSaveName=Cascade.",
                    "Open UDP 7777 and UDP 27015 on the Firewall Rules page or through Connect to Tunnel. The developers say to forward both if the server does not show in the server browser.",
                    "Add -ServerPassword=<password> to require a join password and -AdminPassword=<password> to let players become admins in game. Without an admin password, add Moderator=<SteamID> under [Moderators] in AbioticFactor\\Saved\\SaveGames\\Server\\Admin.ini while the server is stopped.",
                    "World saves and each world's SandboxSettings.ini live under AbioticFactor\\Saved\\SaveGames\\Server. Change -WorldSaveName to run a different world.",
                    "For mods, add -Mods=<id;id> with mod.io mod IDs and -ModioToken=<token> so mods are not downloaded again on every start.",
                    "Do not add -NOSTEAM. The developers warn against it for dedicated servers.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/427410/ss_f57e6a13592c1dee5d903218e31d4add71dbaa83.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/427410/ss_a0858ca74b521e17bc923f94d6e77c6ff224da2f.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/427410/57a1ab194abcbb53d007e2a467b6a3a0118a8e35/ss_57a1ab194abcbb53d007e2a467b6a3a0118a8e35.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/427410/702ec91ec4e414f11204895bf661ad3e0d84d69a/ss_702ec91ec4e414f11204895bf661ad3e0d84d69a.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/427410/ss_d8b3b99a793ebcbac59b7cca8bd14310d2fd0b06.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/427410/7dc96fef7bd039d4c36d7d60f83f9bc908f5754c/ss_7dc96fef7bd039d4c36d7d60f83f9bc908f5754c.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "ark-survival-evolved",
            Name = "ARK: Survival Evolved Dedicated Server",
            Description = "Official ARK: Survival Evolved dedicated server from Studio Wildcard, installed through Valve's SteamCMD (anonymous). No join or admin password is set by default.",
            Genres = new() { "Survival", "Open World", "Crafting" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/346110/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/346110/library_hero.jpg",
                Accent = "#2F855A",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 376030,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\ShooterGame\\Binaries\\Win64\\ShooterGameServer.exe",
                Arguments = "\"TheIsland?listen?SessionName={server_name}?Port={server_port}?QueryPort=27015?MaxPlayers=70?RCONEnabled=False?RCONPort=27020\" -server -log",
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
                About = "ARK: Survival Evolved is a dinosaur survival game from Studio Wildcard. Players wake up on an island with nothing, then gather resources, craft tools, build bases and tame more than a hundred creatures to ride, fight and farm with.\n\nThe game has many maps, including The Island, The Center, Scorched Earth, Ragnarok, Aberration and Extinction. Tribes can cooperate on PvE servers or raid each other on PvP servers.\n\nThe free ARK Dedicated Server tool gives you full control of rates, difficulty, map choice and passwords through GameUserSettings.ini and Game.ini. Steam Workshop mods are supported through the ActiveMods setting.",
                Developer = "Studio Wildcard",
                Publisher = "Studio Wildcard",
                ReleaseDate = "Aug 27, 2017",
                Website = "https://www.playark.com/",
                StoreUrl = "https://store.steampowered.com/app/346110/",
                DocsUrl = "https://ark.wiki.gg/wiki/Dedicated_server_setup",
                MaxPlayers = "Configurable via ?MaxPlayers (this app launches with 70)",
                Features = new()
                {
                    "Official dedicated server tool, installed anonymously via SteamCMD (app 376030)",
                    "Many official maps selected by the map name in the launch URL",
                    "PvE or PvP rules, rates and difficulty in GameUserSettings.ini and Game.ini",
                    "Server password and admin password for in-game cheat commands",
                    "Optional RCON remote console",
                    "Steam Workshop mods through ActiveMods",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777-7778", Protocol = "UDP", Purpose = "Game traffic (Port) and raw UDP peer port (Port + 1)" },
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Steam query for the server browser (QueryPort)" },
                    new GamePortInfo { Port = "27020", Protocol = "TCP", Purpose = "RCON (only when RCONEnabled=True)" },
                },
                SetupTips = new()
                {
                    "The app launches ShooterGame\\Binaries\\Win64\\ShooterGameServer.exe with \"TheIsland?listen?SessionName=<name>?Port=<port>?QueryPort=27015?MaxPlayers=70?RCONEnabled=False?RCONPort=27020\" -server -log. Replace TheIsland with another map name to change maps.",
                    "Open UDP 7777-7778 and UDP 27015 on the Firewall Rules page or through Connect to Tunnel. The server also uses the port right after the game port.",
                    "Set ServerPassword and ServerAdminPassword under [ServerSettings] in ShooterGame\\Saved\\Config\\WindowsServer\\GameUserSettings.ini, or add ?ServerPassword=<password>?ServerAdminPassword=<password> inside the quoted map URL. Admins use enablecheats <admin password> in game.",
                    "World, tribe and player saves are in ShooterGame\\Saved\\SavedArks. Logs are in ShooterGame\\Saved\\Logs.",
                    "The Windows server needs the Visual C++ 2013 x64 redistributable and the DirectX End-User Runtime (June 2010).",
                    "For RCON, set RCONEnabled=True, set an admin password and open TCP 27020.",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 6144,
                    RecommendedRamMb = 8192,
                    DiskGb = 20,
                    Notes = "The ARK wiki lists 3.5 to 4.5 GiB of RAM for an empty The Island, plus 50 to 150 MiB per player, and about 18 GiB of disk.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/346110/ss_2fd997a2f7151cb2187043a1f41589cc6a9ebf3a.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/346110/ss_01cbef83fe28d64ee5a3d39a86043fb1e49abd31.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/346110/ss_164a92a53f9bcbb728b391fc0719f9769c2e1249.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/346110/ss_46778c08a1a5ac5bdbaf8a5bf844fa666f66a14b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/346110/ss_ffe9f0e2e23892f3bb6188e5a3eed0f60a08baf4.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/346110/ss_cada382a940c062a1a5244db8da1326de55ddeae.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "ark-survival-ascended",
            Name = "ARK: Survival Ascended Dedicated Server",
            Description = "Official ARK: Survival Ascended dedicated server from Studio Wildcard, installed through Valve's SteamCMD (anonymous). No join or admin password is set by default.",
            Genres = new() { "Survival", "Open World", "Crafting" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/2399830/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/2399830/library_hero.jpg",
                Accent = "#0EA5E9",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 2430930,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\ShooterGame\\Binaries\\Win64\\ArkAscendedServer.exe",
                Arguments = "\"TheIsland_WP?listen?SessionName={server_name}?RCONEnabled=False?RCONPort=27020\" -port={server_port} -WinLiveMaxPlayers=70 -log",
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
                About = "ARK: Survival Ascended is the Unreal Engine 5 remake of ARK: Survival Evolved from Studio Wildcard. It rebuilds the original maps and creatures with new graphics, building and taming changes, and cross-platform play between PC and consoles.\n\nPlayers gather resources, craft gear, build bases and tame dinosaurs and other creatures, alone or in tribes, on PvE or PvP servers.\n\nThe free ARK: Survival Ascended Dedicated Server tool is separate from the ARK: Survival Evolved server. It uses CurseForge mods instead of the Steam Workshop and lists servers through Epic Online Services instead of the Steam server browser.",
                Developer = "Studio Wildcard",
                Publisher = "Studio Wildcard",
                ReleaseDate = "Oct 25, 2023",
                Website = "https://www.playark.com/",
                StoreUrl = "https://store.steampowered.com/app/2399830/",
                DocsUrl = "https://ark.wiki.gg/wiki/Dedicated_server_setup",
                MaxPlayers = "Configurable via -WinLiveMaxPlayers (default and this app's value: 70)",
                Features = new()
                {
                    "Official dedicated server tool, installed anonymously via SteamCMD (app 2430930)",
                    "Unreal Engine 5 remake of the ARK maps, starting with The Island",
                    "Cross-platform play with console players",
                    "CurseForge mods installed automatically with -mods=<ids>",
                    "Rates, difficulty and rules in GameUserSettings.ini and Game.ini",
                    "Optional RCON remote console",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "UDP", Purpose = "Game traffic (-port)" },
                    new GamePortInfo { Port = "27020", Protocol = "TCP", Purpose = "RCON (only when RCONEnabled=True)" },
                },
                SetupTips = new()
                {
                    "The app launches ShooterGame\\Binaries\\Win64\\ArkAscendedServer.exe with \"TheIsland_WP?listen?SessionName=<name>?RCONEnabled=False?RCONPort=27020\" -port=<port> -WinLiveMaxPlayers=70 -log.",
                    "Open UDP 7777 on the Firewall Rules page or through Connect to Tunnel. ASA does not use a Steam query port, so the app does not query it.",
                    "Set ServerPassword and ServerAdminPassword under [ServerSettings] in ShooterGame\\Saved\\Config\\WindowsServer\\GameUserSettings.ini. If you add ?ServerAdminPassword=<password> to the map URL instead, it must be the last option in the URL.",
                    "-WinLiveMaxPlayers replaces the MaxPlayers ini setting. Change it in the launch arguments to raise or lower the player cap.",
                    "Add -mods=<id>,<id> with CurseForge project IDs to install mods at start. Add -NoBattlEye to run without BattlEye.",
                    "Saves are in ShooterGame\\Saved\\SavedArks and logs in ShooterGame\\Saved\\Logs. Expect 8 to 10 GiB of RAM on The Island.",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 10240,
                    RecommendedRamMb = 16384,
                    DiskGb = 15,
                    Notes = "The ARK wiki lists 8 to 10 GiB of RAM for The Island and up to 12 GiB for The Center, about 11 GiB of disk and 4 logical CPU cores per instance.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/2399830/ss_44f74e86ebd6620a4f0a1f3f8ea97f73dc215e69.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/2399830/ss_65c0de5ced28281764990a299eb3926629b2863e.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/2399830/ss_0ce49e629369dbf9e8fba324667167568c05c66f.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/2399830/ss_63a6ea13596a948b7873ed6de4fd4d01f9cbe57b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/2399830/ss_37b5cb9485658f91b856a0714a8cbdc69f1802b1.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/2399830/ss_f9e156d2b818d808c65476997283b5da8be8324f.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "astroneer",
            Name = "ASTRONEER Dedicated Server",
            Description = "Official ASTRONEER dedicated server from System Era, installed through Valve's SteamCMD (anonymous). Set PublicIP and the port in the server's ini files before players join.",
            Genres = new() { "Sandbox", "Adventure", "Space" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/361420/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/361420/library_hero.jpg",
                Accent = "#8B5CF6",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 728470,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\Astro\\Binaries\\Win64\\AstroServer-Win64-Shipping.exe",
                Arguments = "-log",
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
                About = "ASTRONEER is a space exploration and sandbox game from System Era Softworks. Players land on colourful, fully deformable planets, reshape the terrain with a terrain tool, gather resources and build bases, vehicles and rockets to reach other worlds in the solar system.\n\nThe game supports co-op for up to four players in a hosted session. The free ASTRONEER Dedicated Server tool keeps a save running on its own so players can join whenever they like.\n\nServer settings live in two ini files. The server needs your public IP address and an open UDP port before players outside your network can connect.",
                Developer = "System Era Softworks",
                Publisher = "System Era Softworks",
                ReleaseDate = "Feb 5, 2019",
                Website = "https://astroneer.space/",
                StoreUrl = "https://store.steampowered.com/app/361420/",
                DocsUrl = "https://blog.astroneer.space/p/astroneer-dedicated-server-details/",
                MaxPlayers = "Set with MaximumPlayerCount in AstroServerSettings.ini",
                Features = new()
                {
                    "Official dedicated server tool, installed anonymously via SteamCMD (app 728470)",
                    "Persistent save that stays online without a player hosting",
                    "Owner set by Steam name in AstroServerSettings.ini",
                    "Optional server password",
                    "Fully deformable planets and shared base building",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "7777", Protocol = "UDP", Purpose = "Game traffic (Port under [URL] in Engine.ini)" },
                },
                SetupTips = new()
                {
                    "The app launches Astro\\Binaries\\Win64\\AstroServer-Win64-Shipping.exe with -log. The port is not a launch argument: set Port=7777 under [URL] in Astro\\Saved\\Config\\WindowsServer\\Engine.ini so it matches the port in the app.",
                    "Start the server once, then stop it, so Engine.ini and AstroServerSettings.ini are created in Astro\\Saved\\Config\\WindowsServer. System Era says not to edit these files while the server is running.",
                    "In AstroServerSettings.ini set PublicIP to your public IP address, ServerName, and OwnerName to your Steam name with OwnerGuid=0. The first player with that name becomes the owner.",
                    "Set ServerPassword in AstroServerSettings.ini to require a password to join.",
                    "Open UDP 7777 on the Firewall Rules page or through Connect to Tunnel. Saves are stored in Astro\\Saved\\SaveGames.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/361420/ss_539bc5211ccdad2bc6cc70e4af40194d74eb0256.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/361420/ss_858b8bece04b753a6b35a009776a4de9dd6e0df7.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/361420/ss_436f3ea0934a6ecd3fdd2f260ef535d80b8c185d.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/361420/ss_13682f989b3f0d68afb92d5d50185b0258dd4767.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/361420/ss_c7c54244add5ac921f2ae5b28f6f3efbc8470823.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/361420/ss_63402300a9d8f1eff80a3034b872995db03158ab.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "core-keeper",
            Name = "Core Keeper Dedicated Server",
            Description = "Official Core Keeper dedicated server from Pugstorm, installed through Valve's SteamCMD (anonymous). No join password is set by default.",
            Genres = new() { "Survival", "Crafting", "Co-op" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/1621690/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/1621690/library_hero.jpg",
                Accent = "#D97706",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 1963720,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\CoreKeeperServer.exe",
                Arguments = "-batchmode -logfile server.log -worldname \"{server_name}\" -maxplayers 8 -port {server_port}",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 27015,
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
                About = "Core Keeper is a mining sandbox adventure from Pugstorm for 1 to 8 players. You wake up in an underground cavern next to an ancient core, then dig outward to gather resources, farm, fish, cook, craft gear and fight large bosses.\n\nThe world is procedurally generated and expands into biomes such as the Azeos' Wilderness and the Sunken Sea. Players build bases, automate crafting and unlock new areas together.\n\nThe free Core Keeper Dedicated Server tool keeps a world running without a player hosting. Players join with a Game ID through Steam's relay, or by IP and port when the server is started with -port.",
                Developer = "Pugstorm",
                Publisher = "Fireshine Games",
                ReleaseDate = "Aug 27, 2024",
                StoreUrl = "https://store.steampowered.com/app/1621690/",
                DocsUrl = "https://core-keeper.fandom.com/wiki/Dedicated_Servers",
                MaxPlayers = "Configurable via -maxplayers (this app launches with 8)",
                Features = new()
                {
                    "Official dedicated server tool, installed anonymously via SteamCMD (app 1963720)",
                    "Join by Game ID through Steam's relay, no port forwarding needed",
                    "Direct IP connections with -port for Steam, Epic, GOG and Microsoft Store players",
                    "World name, seed, mode and season options",
                    "Optional join password",
                    "Admin tools in the in-game pause menu",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Direct connections (-port). Not needed for Game ID joins through Steam's relay" },
                },
                SetupTips = new()
                {
                    "The app launches CoreKeeperServer.exe with -batchmode -logfile server.log -worldname \"<name>\" -maxplayers 8 -port <port>. The console shown in the app is read from server.log.",
                    "Each start writes GameInfo.txt next to the server exe. It holds the Game ID, the password and an IP;PORT;;PASSWORD line for Join Game Via IP.",
                    "Add -password <password> to require a password. Most options can also be set in ServerConfig.json, but -port and -ip only work on the command line.",
                    "Saves and ServerConfig.json are in %USERPROFILE%\\AppData\\LocalLow\\Pugstorm\\Core Keeper\\DedicatedServer unless you set -datapath.",
                    "Open UDP 27015 on the Firewall Rules page or through Connect to Tunnel for direct connections. Players who join by Game ID do not need it.",
                    "Admins can make other players admins, ban players and regenerate the Game ID from the in-game pause menu.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621690/632a42b91e005b2d2daf99fee946732c95a9737c/ss_632a42b91e005b2d2daf99fee946732c95a9737c.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621690/bca1feb5ec1bf64543f80c74effdcaed4216033c/ss_bca1feb5ec1bf64543f80c74effdcaed4216033c.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621690/804585b7772d3ae5c7d56fc80401469d5bf6fd27/ss_804585b7772d3ae5c7d56fc80401469d5bf6fd27.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621690/ss_1e162c49418038f8042c92810fe3c753449e204b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621690/ss_2a6cdb26200932593b4edd5ab3d1cc2da5bc15ee.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1621690/ss_8f3fd22bc5c385450d36309755323ca18efda4be.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "cubic-odyssey",
            Name = "Cubic Odyssey Dedicated Server",
            Description = "Official Cubic Odyssey dedicated server (beta) from Atypical Games, installed through Valve's SteamCMD (anonymous). No join password is set by default.",
            Genres = new() { "Adventure", "Open World", "Crafting" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/3400000/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/3400000/library_hero.jpg",
                Accent = "#4F6BFF",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 3858450,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\server\\CubicOdysseyServer.exe",
                Arguments = "-Port={server_port}",
                WorkingDir = "{server_root}",
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
                Supported = false,
                Folder = "mods",
                Extensions = new(),
                Kind = "mod",
            },
            Details = new GameDetails
            {
                About = "Cubic Odyssey is a voxel open-world adventure from Atypical Games, published by Gaijin. You travel between colourful planets, gather resources, craft tools, build vehicles and bases, and fight the spreading Red Darkness while uncovering ancient mysteries.\n\nThe dedicated server is a separate free tool for PC owners and is currently in beta. It lets the world keep running and evolving when the host is offline. Lobbies support up to 20 players, more than the peer-to-peer co-op mode. Each server has its own characters, so progress made there stays tied to that server.",
                Developer = "Atypical Games",
                Publisher = "Gaijin Network Ltd",
                ReleaseDate = "May 14, 2025",
                Website = "https://cubicodyssey.game/",
                StoreUrl = "https://store.steampowered.com/app/3400000/",
                DocsUrl = "https://store.steampowered.com/app/3858450/",
                MaxPlayers = "Up to 20 players",
                Features = new()
                {
                    "Voxel open-world adventure across many planets",
                    "Crafting, vehicle building and base building",
                    "Free dedicated server tool for PC owners (beta), installed anonymously via SteamCMD (app 3858450)",
                    "Up to 20 players per server lobby",
                    "World persists while the host is offline",
                    "Characters and progress are separate for each server",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "27015", Protocol = "TCP+UDP", Purpose = "Game traffic (-Port, the app's default; no official port list is published)" },
                },
                SetupTips = new()
                {
                    "The server is a separate free Steam tool (app 3858450). The app downloads it with an anonymous SteamCMD login (no Steam account needed to install it) and launches server\\CubicOdysseyServer.exe -Port=<port> from the server folder.",
                    "Settings live in config/server_config.txt in the server folder (the download ships config/server_config.txt.sample as a template): serverName, serverPassword, maxPlayers, galaxySeed, privateServer, allowRelaying and the difficulty scales. help/server_manual.txt lists the launch options: -Port, -MaxPort, -MaxNumPlayers, -Password and -Gamemode=adventure|creative.",
                    "Players join from Join Game > Dedicated Servers in the game. Each server uses its own character, so tell players that progress does not carry over from their other worlds.",
                    "No official port list is published. The app passes your port (default 27015) as -Port, so open it on the Firewall Rules page or through Connect to Tunnel, and check the server console for the port it actually binds to.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/3400000/2b09f6346b32c2a8b6ffecd3e7e5dfd49608b191/ss_2b09f6346b32c2a8b6ffecd3e7e5dfd49608b191.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/3400000/ss_42dfb80d95b5fe58171d4dde92ed663cb505db66.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/3400000/7c0f479fc14dab3c582d5e54f0f80194a6a90987/ss_7c0f479fc14dab3c582d5e54f0f80194a6a90987.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/3400000/ss_6ec6404e0695a325eca5472beb8ff30d9494dc3b.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/3400000/0e6755afae8125468e187e2f9464626d605c8bd4/ss_0e6755afae8125468e187e2f9464626d605c8bd4.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/3400000/45104b133ebf4ef107c61f450df5fb02578254dc/ss_45104b133ebf4ef107c61f450df5fb02578254dc.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "creativerse",
            Name = "Creativerse Dedicated Server",
            Description = "Official Creativerse dedicated server from Playful Studios, installed through Valve's SteamCMD (anonymous). On the first start the app asks for your SteamID64 and writes the world's config_world.json for you.",
            Genres = new() { "Sandbox", "Building", "Survival" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/280790/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/280790/library_hero.jpg",
                Accent = "#22C55E",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 1098260,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{server_root}\\CreativerseServer.exe",
                Arguments = "-worldId=mpserver_world -forceIp=0.0.0.0",
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = 26900,
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
                About = "Creativerse is a free-to-play voxel sandbox from Playful Studios. Players explore procedurally generated worlds, mine and gather resources, craft tools, tame creatures and build anything from small huts to large machines.\n\nThe Definitive Edition moved all worlds to player hosting after the official servers were shut down. Every world now runs on a Creativerse server, either started in the background by the game client or run on its own as a dedicated server.\n\nThe free Creativerse Dedicated Server tool runs a world by its world key. While a world is running it also serves a WebAdmin page with owner tools, backups and access requests.",
                Developer = "Playful Corp.",
                Publisher = "Playful Corp.",
                ReleaseDate = "May 8, 2017",
                Website = "https://www.creativersegame.com/",
                StoreUrl = "https://store.steampowered.com/app/280790/",
                DocsUrl = "https://docs.google.com/document/d/1rAeuk7qFinUyPRJGk4x6Dim3rmQN2zoLaR3PNR0roU4",
                MaxPlayers = "Set with the Max Players world setting",
                Features = new()
                {
                    "Official dedicated server tool, installed anonymously via SteamCMD (app 1098260)",
                    "Runs worlds created in the game client or written by hand",
                    "WebAdmin page with owner tools, backups and access requests",
                    "Whitelisting and private worlds",
                    "Automatic saves every minute and configurable backups",
                    "Several worlds on one machine with different base ports",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "26900", Protocol = "UDP", Purpose = "Game traffic (GamePort in config_world.json)" },
                    new GamePortInfo { Port = "26901", Protocol = "UDP", Purpose = "Steam query and world lists (GamePort + 1)" },
                    new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Alternative Steam query port some setups use" },
                    new GamePortInfo { Port = "26902", Protocol = "TCP", Purpose = "WebAdmin and extra client data (GamePort + 2)" },
                },
                SetupTips = new()
                {
                    "The app launches CreativerseServer.exe with -worldId=mpserver_world -forceIp=0.0.0.0. The worldId value is case sensitive (capital I) and must match a folder in %LOCALAPPDATA%\\PlayfulCorp\\CreativerseServer\\worlddata\\worlds. On the very first start the server only creates its data folders and reports that the world is missing.",
                    "The dedicated server cannot create worlds by itself. Before the first start the app asks for your SteamID64, a world name and a template, then writes worlddata\\worlds\\mpserver_world\\config_world.json with PrivateServer false and you as WorldOwnerSteamId. Templates are found from worlds you made in the game on this PC.",
                    "To write a world by hand, create worlddata\\worlds\\mpserver_world\\config_world.json containing {\"GamePort\": 26900, \"PrivateServer\": false, \"UsePlayerAccessFile\": false, \"WorldOwnerSteamId\": \"<your SteamID64>\", \"WorldKey\": \"mpserver_world\", \"TemplateKey\": \"<template key>\", \"WorldName\": \"My World\", \"WorldDescription\": \"\", \"Tags\": []}. TemplateKey must name a template in the dltemplates or templates folder (templates come from mod.io/g/creativerse). Copying the config_world.json of a world made in the game is the safest source of a valid key.",
                    "The port is not a launch argument. Set GamePort in config_world.json (default 26900) and match it in the app. The server also uses GamePort + 1 (UDP, query) and GamePort + 2 (TCP, WebAdmin). WorldOwnerSteamId always gets the Owner role. Edit the file only while the server is stopped.",
                    "Open UDP 26900 (game), UDP 26901 or 27015 (query) and TCP 26902 (WebAdmin) on the Firewall Rules page or through Connect to Tunnel. Worlds only show in the LAN tab when the base port is in 26900-26905 or 27015-27020. Set PrivateServer to true to hide the world from public lists, or UsePlayerAccessFile to require whitelisting.",
                    "Logs are written to logs\\mpserver_world in the server folder. The WebAdmin (full owner tools and backups) is at http://localhost:26902 on the server machine only.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/280790/ss_661ff812336102c15878df9b65f74ac30a89a1ab.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/280790/ss_62425db637f19693b30cb823c8f05949dff7d869.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/280790/ss_bbe2d11c348ffc4b6a2fec882c360b243a4cba1d.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/280790/ss_cc9e426a44001000c29f866a60bafd099dc76558.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/280790/ss_0805cc2364a56d88a989bcc1a28be2f09f514bea.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/280790/ss_9f31d7b5732833d96aa12881083b04feb3d5edab.1920x1080.jpg",
                },
            },
        },
        new GameDefinition
        {
            ManifestVersion = 1,
            Id = "cryofall",
            Name = "CryoFall Dedicated Server",
            Description = "Official CryoFall dedicated server from AtomicTorch Studio, installed through Valve's SteamCMD (anonymous). It runs on .NET 6, which the app downloads for you if it isn't installed.",
            Genres = new() { "Survival", "RPG", "Crafting" },
            Artwork = new ArtworkSpec
            {
                Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/829590/header.jpg",
                Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/829590/library_hero.jpg",
                Accent = "#38BDF8",
            },
            Install = new InstallSpec
            {
                Provider = "steamcmd",
                Variants = new(),
                AppId = 1061710,
                Anonymous = true,
            },
            Runtime = new RuntimeSpec
            {
                Executable = "{dotnet}",
                Arguments = "\"{server_root}\\Binaries\\Server\\CryoFall_Server.dll\" loadOrNew",
                WorkingDir = "{server_root}\\Binaries\\Server",
                StopCommand = "stop 0",
                Requires = new RequiresSpec { DotNet = 6 },
            },
            Network = new NetworkSpec
            {
                DefaultPort = 6000,
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
                About = "CryoFall is a sci-fi multiplayer survival RPG from AtomicTorch Studio, played from a top-down view on a persistent server. Players gather resources, farm, craft, build and defend bases, and progress through a technology tree from primitive tools to power armour and vehicles.\n\nServers can run PvP, PvE or mixed rules, and a single server can hold hundreds of players. The game has a player-driven economy, land claims and faction systems.\n\nThe free CryoFall Dedicated Server runs on .NET and is light on hardware. Settings, server rates and mods are configured through files in its Data folder.",
                Developer = "AtomicTorch Studio",
                Publisher = "Daedalic Entertainment",
                ReleaseDate = "Apr 29, 2021",
                Website = "https://cryofall.com/",
                StoreUrl = "https://store.steampowered.com/app/829590/",
                DocsUrl = "https://wiki.atomictorch.com/CryoFall/Server/Setup",
                MaxPlayers = "Hundreds of players (AtomicTorch's sizing guide covers up to about 300 online)",
                Features = new()
                {
                    "Official dedicated server tool, installed anonymously via SteamCMD (app 1061710)",
                    "PvP, PvE or mixed rules on a persistent world",
                    "Server rates tuned in Data\\ServerRates.config",
                    "Server operators list in SettingsServer.xml",
                    "Whitelist, blacklist, kick and mute moderation commands",
                    "Clean shutdown with the stop console command",
                },
                Ports = new()
                {
                    new GamePortInfo { Port = "6000", Protocol = "UDP", Purpose = "Game traffic (set in Data\\SettingsServer.xml)" },
                },
                SetupTips = new()
                {
                    "The app runs dotnet \"Binaries\\Server\\CryoFall_Server.dll\" loadOrNew from the Binaries\\Server folder. loadOrNew loads the last save or creates a new world. If .NET 6 isn't installed, the app downloads Microsoft's .NET 6 runtime into its tools folder on the first start.",
                    "The port is not a launch argument. Change it in Data\\SettingsServer.xml (default UDP 6000) and match it in the app. Allow dotnet.exe through Windows Firewall if Windows asks, then open the port on the Firewall Rules page or through Connect to Tunnel.",
                    "Add your nickname inside <server_operators_list> in Data\\SettingsServer.xml to become an operator. Operators open the console in game with the ~ key and prefix commands with /.",
                    "CryoFall has no join password setting. Use /mod.whiteList.enabled 1 and /mod.whiteList.add <name> to limit who can join.",
                    "The app stops the server with stop 0, which saves and quits. Saves are kept in Data\\Saves, and rates are in Data\\ServerRates.config.",
                },
                ServerRequirements = new ServerRequirementsSpec
                {
                    MinRamMb = 1536,
                    RecommendedRamMb = 2048,
                    Notes = "AtomicTorch lists 1.5 GB of RAM and one 1.6 to 2 GHz core for about 30 players, and 2 to 2.5 GB with two cores for 100 to 300 players. Runs on .NET 6, downloaded by the app when missing.",
                },
                Screenshots = new()
                {
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/829590/ss_a5fd00758eabcff045da06eb90d8e08295b15c48.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/829590/ss_0aec5d448a24f8aaf68cf556408427cd40513ec1.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/829590/ss_c46f000b1bdc6cce19f8cea32a7a24a7f6043f65.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/829590/ss_3dcf06794505e18541bc0c3d98fccf1bc12cb63f.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/829590/ss_5eab4c675b783aaed52d97fb9910c6ab171a77b6.1920x1080.jpg",
                    "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/829590/ss_5b42d84fd15d47abbf4b3929fded3d5558fc7a21.1920x1080.jpg",
                },
            },
        },
    };
}
