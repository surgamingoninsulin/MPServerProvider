using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public static partial class BuiltInGames
{
    public const string SteamCdn = "https://cdn.cloudflare.steamstatic.com/steam/apps/";

    public static List<GameDefinition> All()
    {
        var all = CoreGames();
        all.AddRange(AddedGames());
        return all;
    }

    internal static List<GameDefinition> AddedGames()
    {
        var added = new List<GameDefinition>();
        added.AddRange(SurvivalAGames());
        added.AddRange(SurvivalBGames());
        added.AddRange(SurvivalCGames());
        added.AddRange(SurvivalDGames());
        added.AddRange(MiscGames());
        added.AddRange(ClassicGames());
        return added;
    }

    private static List<GameDefinition> CoreGames() => new()
    {
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "minecraft-java",
                Name = "Minecraft: Java Edition",
                Description = "The official Minecraft Java server. Choose Vanilla (Mojang), Paper (high-performance, plugins), Fabric (mods), Purpur (Paper fork), Forge (mods), Velocity (proxy) or BungeeCord (proxy).",
                Genres = new() { "Sandbox", "Survival" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn2.steamgriddb.com/thumb/5810ad26c4f298db46ff65a4b8016dea.jpg",
                    Hero = "https://www.minecraft.net/content/dam/minecraftnet/franchise/logos/minecraft-creeper-face.jpg",
                    Accent = "#3FB950",
                },
                Install = new InstallSpec
                {
                    Provider = "minecraft-java",
                    Variants = new() { "vanilla", "paper", "fabric", "purpur", "forge", "velocity", "bungeecord" },
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{java}",
                    Arguments = "-Xms{ram_min_mb}M -Xmx{ram_max_mb}M -jar \"{server_file}\" nogui",
                    WorkingDir = "{server_root}",
                    StopCommand = "stop",
                    Requires = new RequiresSpec { Java = 25 },
                },
                Network = new NetworkSpec
                {
                    DefaultPort = 25565,
                    Protocol = "both",
                    Query = "minecraft-java",
                    ConsoleMethod = "stdin",
                },
                Mods = new ModsSpec
                {
                    Supported = true,
                    Folder = "mods",
                    Extensions = new() { ".jar" },
                    Kind = "mod",
                },
                Details = new GameDetails
                {
                    About = "Minecraft: Java Edition is the original PC version of Mojang's block-building sandbox. You can gather resources, build, explore and survive in an endless world, and it runs on Windows, macOS and Linux.\n\nRunning your own Java Edition server keeps a world online that everyone can join, even when you are not playing. You decide the rules in server.properties, including difficulty, game mode, player limit and whitelist. You can stay on Mojang's vanilla server or switch to Paper, Purpur, Fabric or Forge for plugins and mods, or to Velocity or BungeeCord to link several servers together.",
                    Developer = "Mojang Studios",
                    ReleaseDate = "Nov 18, 2011",
                    Website = "https://www.minecraft.net/en-us/about-minecraft",
                    StoreUrl = "https://www.minecraft.net/en-us/store/minecraft-java-bedrock-edition-pc",
                    DocsUrl = "https://minecraft.wiki/w/Server.properties",
                    MaxPlayers = "Configurable (default 20)",
                    Features = new()
                    {
                        "Official server software is free to download from minecraft.net",
                        "Vanilla server or plugin and mod platforms (Paper, Purpur, Fabric, Forge)",
                        "Proxy support (Velocity, BungeeCord) for linking multiple servers",
                        "Whitelist, operator and online-mode (account authentication) controls",
                        "Optional RCON and Query interfaces for remote admin and status tools",
                        "World and gameplay rules set in server.properties",
                    },
                    ServerRequirements = new ServerRequirementsSpec
                    {
                        MinRamMb = 1024,
                        RecommendedRamMb = 2048,
                        Notes = "From minecraft.wiki: 1 GB of RAM for the server app with 1-4 players, 2 GB for 5-10 players and 4 GB for 10+ players. The wiki also says at least 4 GB of RAM total for the server and OS combined. A CPU with strong single-core performance helps most, since the server does most of its work on one core. World saves need at least 150-200 MB. Plugins, mods and large worlds raise all of these figures.",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "25565", Protocol = "TCP", Purpose = "Game traffic (server-port)" },
                        new GamePortInfo { Port = "25565", Protocol = "UDP", Purpose = "Query (query.port, only when enable-query=true)" },
                        new GamePortInfo { Port = "25575", Protocol = "TCP", Purpose = "RCON remote console (rcon.port, only when enable-rcon=true)" },
                    },
                    SetupTips = new()
                    {
                        "On first start the server writes eula.txt and exits. Set eula=true in that file and start the server again.",
                        "Minecraft 26.1 and newer need Java 25 (1.20.5 through 1.21.x use Java 21). The app reads each version's Java requirement from Mojang's version metadata and downloads the matching Eclipse Temurin runtime on first start. If a server stops at launch with an UnsupportedClassVersionError, it needs a newer Java runtime.",
                        "The app sets -Xms and -Xmx from your RAM sliders. Plan on about 2 GB for 5-10 players, and more for modpacks.",
                        "In recent versions (26.3+), white-list defaults to true in server.properties. Add players with 'whitelist add <name>' in the console, or set white-list=false for an open server.",
                        "Open TCP 25565 on the Firewall Rules page, or use Connect to Tunnel, so players outside your network can join. Only open 25575 if you enable RCON, and set a strong rcon.password.",
                        "Use the 'stop' console command to shut down cleanly so the world saves. Put Fabric or Forge mod .jar files in the mods folder.",
                    },
                    Screenshots = new()
                    {
                        "https://www.minecraft.net/content/dam/minecraftnet/games/minecraft/screenshots/PDP-Gallery_OV_1_16x9.jpg",
                        "https://www.minecraft.net/content/dam/minecraftnet/games/minecraft/screenshots/PDP-Gallery_OV_2_16x9.jpg",
                        "https://www.minecraft.net/content/dam/minecraftnet/games/minecraft/screenshots/PDP-Gallery_OV_5_16x9.jpg",
                        "https://www.minecraft.net/content/dam/minecraftnet/games/minecraft/screenshots/PDP-Gallery_OV_6_16x9.jpg",
                        "https://www.minecraft.net/content/dam/minecraftnet/games/minecraft/key-art/Hero-Image_Vanilla_Standard_1200x675.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "minecraft-bedrock",
                Name = "Minecraft: Bedrock Edition",
                Description = "Official Bedrock Dedicated Server from Mojang — for phones, consoles and Windows 10/11 clients.",
                Genres = new() { "Sandbox", "Survival" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn2.steamgriddb.com/thumb/131df8dfa437f57f24aa804fc542d653.jpg",
                    Hero = "https://www.minecraft.net/content/dam/minecraftnet/franchise/logos/minecraft-creeper-face.jpg",
                    Accent = "#10B981",
                },
                Install = new InstallSpec
                {
                    Provider = "minecraft-bedrock",
                    Variants = new(),
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_file}",
                    Arguments = "",
                    WorkingDir = "{server_root}",
                    StopCommand = "stop",
                },
                Network = new NetworkSpec
                {
                    DefaultPort = 19132,
                    Protocol = "udp",
                    Query = "minecraft-bedrock",
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
                    About = "Minecraft: Bedrock Edition is the cross-platform version of Minecraft. It runs on Windows, Xbox, PlayStation, Nintendo Switch, iOS and Android, and players on all of them can play together.\n\nThe Bedrock Dedicated Server (BDS) is Mojang's free, official server software for this edition, available for Windows and Ubuntu Linux. Hosting your own BDS keeps a world online around the clock. You control the settings through server.properties and decide who can join with the allow-list and permission files.",
                    Developer = "Mojang Studios",
                    ReleaseDate = "Aug 16, 2011",
                    Website = "https://www.minecraft.net/en-us/about-minecraft",
                    StoreUrl = "https://www.minecraft.net/en-us/store/minecraft-java-bedrock-edition-pc",
                    DocsUrl = "https://minecraft.wiki/w/Bedrock_Dedicated_Server",
                    MaxPlayers = "Configurable (default 10)",
                    Features = new()
                    {
                        "Official, free server software from Mojang for Windows and Ubuntu Linux",
                        "Cross-platform play for Bedrock clients on PC, mobile and consoles",
                        "Allow-list and per-player permission levels",
                        "World, difficulty, game mode and cheat settings in server.properties",
                        "Lightweight next to Java Edition, with no Java runtime needed",
                    },
                    ServerRequirements = new ServerRequirementsSpec
                    {
                        MinRamMb = 4096,
                        DiskGb = 1,
                        Notes = "From the minecraft.net BDS download page: Intel Core i3 3210 / AMD A8 7600 APU or equivalent, 4 GB RAM, 180 MB to 1 GB of disk space, and Windows 10 (10.0.15063+), Windows Server 2016+ or Ubuntu 22.04+. The FAQ on the same page says small servers need at least 1 GB of RAM. minecraft.wiki notes that more than 10 players may need more memory.",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "19132", Protocol = "UDP", Purpose = "Game traffic, IPv4 (server-port)" },
                        new GamePortInfo { Port = "19133", Protocol = "UDP", Purpose = "Game traffic, IPv6 (server-portv6)" },
                    },
                    SetupTips = new()
                    {
                        "Bedrock uses UDP, not TCP. Open UDP 19132 on the Firewall Rules page, or use Connect to Tunnel, for players outside your network. Open 19133 only if you need IPv6.",
                        "allow-list defaults to true in server.properties, so only players listed in allowlist.json can join. Add players with 'allowlist add <name>' in the console, or set allow-list=false for an open server.",
                        "Unlike Java Edition, there is no memory setting. BDS uses what it needs, so leave headroom beyond the 4 GB minimum for larger groups.",
                        "Players connect with the server's IP and port 19132 from the Servers tab. minecraft.wiki notes that Xbox, PlayStation and Switch players may need a LAN or custom-DNS workaround to add a custom server.",
                        "Use the 'stop' console command to shut down cleanly so the world saves. BDS does not support RCON.",
                    },
                    Screenshots = new()
                    {
                        "https://www.minecraft.net/content/dam/minecraftnet/games/minecraft/screenshots/PDP-Gallery_OV_1_16x9.jpg",
                        "https://www.minecraft.net/content/dam/minecraftnet/games/minecraft/screenshots/PDP-Gallery_OV_2_16x9.jpg",
                        "https://www.minecraft.net/content/dam/minecraftnet/games/minecraft/screenshots/PDP-Gallery_OV_5_16x9.jpg",
                        "https://www.minecraft.net/content/dam/minecraftnet/games/minecraft/screenshots/PDP-Gallery_OV_6_16x9.jpg",
                        "https://www.minecraft.net/content/dam/minecraftnet/games/minecraft/key-art/Hero-Image_Vanilla_Standard_1200x675.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "fivem",
                Name = "FiveM Server",
                Description = "Cfx.re FXServer for GTA V multiplayer. txAdmin's setup wizard handles the licence key on first start — visit http://127.0.0.1:40120 after booting.",
                Genres = new() { "Adventure", "Sandbox" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/271590/header.jpg",
                    Hero = null,
                    Accent = "#F97316",
                },
                Install = new InstallSpec
                {
                    Provider = "cfx",
                    Variants = new(),
                    Anonymous = true,
                    Product = "fivem",
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_root}\\artifact\\FXServer.exe",
                    Arguments = "",
                    WorkingDir = "{server_root}",
                    StopCommand = "quit",
                },
                Network = new NetworkSpec
                {
                    DefaultPort = 30120,
                    Protocol = "both",
                    Query = "fivem",
                    ConsoleMethod = "stdin",
                },
                Mods = new ModsSpec
                {
                    Supported = true,
                    Folder = "resources",
                    Extensions = new(),
                    Kind = "resource",
                },
                Details = new GameDetails
                {
                    About = "Grand Theft Auto V is Rockstar's open-world crime game set in Los Santos and Blaine County. FiveM is Cfx.re's multiplayer modification for the PC version. Players connect to community-run servers that can change almost everything about the game through scripts and resources.\n\nHosting your own FiveM server (FXServer) gives you full control over game modes, maps, vehicles and scripts, from serious roleplay to racing or casual freeroam. The bundled txAdmin web panel guides you through first-time setup, deploys a starter recipe, and handles restarts, players and bans from your browser. A free license key from the Cfx.re Portal is required.",
                    Developer = "Rockstar North",
                    Publisher = "Rockstar Games",
                    ReleaseDate = "Apr 13, 2015",
                    Website = "https://fivem.net",
                    StoreUrl = "https://store.steampowered.com/app/271590/",
                    DocsUrl = "https://docs.fivem.net/docs/server-manual/setting-up-a-server/",
                    MaxPlayers = "Up to 48 by default (OneSync); more with Cfx.re Element Club tiers",
                    Features = new()
                    {
                        "Custom game modes, scripts, maps and vehicles through server resources",
                        "txAdmin web panel for setup, restarts, player management and bans",
                        "Recipe deployer that installs a ready-made starter server",
                        "OneSync for larger player counts",
                        "Free license key from the Cfx.re Portal",
                    },
                    ServerRequirements = new ServerRequirementsSpec
                    {
                        Notes = "Cfx.re gives no specific numbers. It recommends an x86-64 system running Windows or Linux, a multi-core processor and decent upload bandwidth.",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "30120", Protocol = "TCP+UDP", Purpose = "Game traffic (endpoint_add_tcp / endpoint_add_udp)" },
                        new GamePortInfo { Port = "40120", Protocol = "TCP", Purpose = "txAdmin web panel" },
                    },
                    SetupTips = new()
                    {
                        "Register a free server license key on the Cfx.re Portal (portal.cfx.re) before the first start.",
                        "On first boot, open http://127.0.0.1:40120. Enter the PIN shown in the console, link your Cfx.re account, set an admin password, then pick the 'CFX Default FiveM' recipe and enter your license key.",
                        "Open 30120 for both TCP and UDP on the Firewall Rules page, or use Connect to Tunnel, so players can connect. Port 40120 is the admin panel and only needs to be reachable by you.",
                        "Slot counts are set with sv_maxclients. Values of 32 and above need OneSync enabled, and free servers are limited to 48 players.",
                        "Add custom resources to the resources folder and ensure them in server.cfg (txAdmin can edit server.cfg). Stop the server with the 'quit' console command.",
                        "Some txAdmin recipes need a database server. Start with 'CFX Default FiveM' if you do not have one.",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/271590/ss_32aa18ab3175e3002217862dd5917646d298ab6b.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/271590/ss_2744f112fa060320d191a50e8b3a92441a648a56.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/271590/ss_da39c16db175f6973770bae6b91d411251763152.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/271590/ss_bd5db78286be0a7c6b2c62519099a9e27e6b06f3.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/271590/ss_b1a1cb7959d6a0e6fcb2d06ebf97a66c9055cef3.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/271590/ss_bc5fc79d3366c837372327717249a4887aa46d63.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "redm",
                Name = "RedM Server",
                Description = "Cfx.re FXServer for Red Dead Redemption 2 multiplayer. Uses the same official artifacts as FiveM with the RDR3 game profile.",
                Genres = new() { "Adventure" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/header.jpg",
                    Hero = null,
                    Accent = "#B91C1C",
                },
                Install = new InstallSpec
                {
                    Provider = "cfx",
                    Variants = new(),
                    Anonymous = true,
                    Product = "redm",
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_root}\\artifact\\FXServer.exe",
                    // RedM runs on the same FXServer artifact as FiveM; without the rdr3 game
                    // profile it would boot as a GTA V (FiveM) server.
                    Arguments = "+set gamename rdr3",
                    WorkingDir = "{server_root}",
                    StopCommand = "quit",
                },
                Network = new NetworkSpec
                {
                    DefaultPort = 30120,
                    Protocol = "both",
                    Query = "fivem",
                    ConsoleMethod = "stdin",
                },
                Mods = new ModsSpec
                {
                    Supported = true,
                    Folder = "resources",
                    Extensions = new(),
                    Kind = "resource",
                },
                Details = new GameDetails
                {
                    About = "RedM is a multiplayer modification for Red Dead Redemption 2 on PC, built on the same Cfx.re framework that powers FiveM. Instead of Rockstar's official online mode, players join community-run dedicated servers that can run their own rules, scripts and game modes, from Wild West roleplay to custom PvP.\n\nRunning your own RedM server uses the official FXServer artifacts with the RDR3 game profile. You control the resource list, player slots, permissions and game build, and the bundled txAdmin web panel gives you a browser-based dashboard for setup, restarts, player management and logs.\n\nPlayers need their own PC copy of Red Dead Redemption 2 plus the RedM client to connect. Your server needs a free Cfx.re license key from the Cfx.re Portal.",
                    Developer = "Cfx.re (RedM); Rockstar Games (Red Dead Redemption 2)",
                    Publisher = "Rockstar Games",
                    ReleaseDate = "Dec 5, 2019",
                    Website = "https://redm.net/",
                    StoreUrl = "https://store.steampowered.com/app/1174180/",
                    DocsUrl = "https://docs.fivem.net/docs/server-manual/setting-up-a-server/",
                    MaxPlayers = "Configurable via sv_maxclients (1-2048; 32+ requires OneSync)",
                    Features = new()
                    {
                        "Community-run Red Dead Redemption 2 multiplayer on dedicated servers",
                        "Same FXServer artifacts as FiveM, using the rdr3 game profile",
                        "Custom scripts and game modes installed as server resources",
                        "txAdmin web panel for setup, restarts, bans and logs",
                        "Choose the enforced RDR2 game build with sv_enforceGameBuild",
                        "Free Cfx.re license key from the Cfx.re Portal",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "30120", Protocol = "TCP+UDP", Purpose = "Game traffic (endpoint_add_tcp / endpoint_add_udp)" },
                        new GamePortInfo { Port = "40120", Protocol = "TCP", Purpose = "txAdmin web panel (keep private; do not expose publicly)" },
                    },
                    SetupTips = new()
                    {
                        "On first start, open txAdmin at http://127.0.0.1:40120. The setup PIN is printed in the Console tab.",
                        "Create a free server license key on the Cfx.re Portal (portal.cfx.re) and enter it in txAdmin's setup, or set sv_licenseKey in server.cfg.",
                        "The app launches FXServer with +set gamename rdr3 so the server runs the RedM (RDR2) profile. Keep it if you change the launch arguments.",
                        "Forward or allow port 30120 on both TCP and UDP using the Firewall Rules page or Connect to Tunnel. txAdmin's 40120 only needs local access.",
                        "Add scripts to the resources folder and enable them with ensure lines in server.cfg. Player slots above 32 need OneSync enabled.",
                        "Use the 'quit' console command (the app's stop command) to shut the server down cleanly.",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1174180/ss_66b553f4c209476d3e4ce25fa4714002cc914c4f.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1174180/ss_bac60bacbf5da8945103648c08d27d5e202444ca.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1174180/ss_668dafe477743f8b50b818d5bbfcec669e9ba93e.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1174180/ss_4ce07ae360b166f0f650e9a895a3b4b7bf15e34f.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/1174180/ss_d1a8f5a69155c3186c65d1da90491fcfd43663d9.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "beammp",
                Name = "BeamMP Server",
                Description = "Official BeamMP multiplayer server for BeamNG.drive. An AuthKey from keymaster.beammp.com is required in ServerConfig.toml.",
                Genres = new() { "Simulation", "Sandbox" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/284160/header.jpg",
                    Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/284160/library_hero.jpg",
                    Accent = "#3B82F6",
                },
                Install = new InstallSpec
                {
                    Provider = "beammp",
                    Variants = new(),
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_file}",
                    Arguments = "",
                    WorkingDir = "{server_root}",
                    StopCommand = "exit",
                },
                Network = new NetworkSpec
                {
                    DefaultPort = 30814,
                    Protocol = "both",
                    Query = "none",
                    ConsoleMethod = "stdin",
                },
                Mods = new ModsSpec
                {
                    Supported = true,
                    Folder = "Resources\\Client",
                    Extensions = new() { ".zip" },
                    Kind = "mod",
                },
                Details = new GameDetails
                {
                    About = "BeamNG.drive is a vehicle simulator built around soft-body physics, so crashes, suspension and handling behave realistically. BeamMP is a community multiplayer mod for it that lets several players drive in the same map at once through the BeamMP Launcher.\n\nRunning your own BeamMP server lets you choose the map, player limit, vehicles per player and which mods everyone downloads when they join. The server is configured through ServerConfig.toml. It needs an AuthKey from BeamMP's Keymaster to appear on the public server list, and a key is recommended even for private servers.",
                    Developer = "BeamNG",
                    Publisher = "BeamNG",
                    ReleaseDate = "May 29, 2015",
                    Website = "https://beammp.com",
                    StoreUrl = "https://store.steampowered.com/app/284160/",
                    DocsUrl = "https://docs.beammp.com/server/create-a-server/",
                    MaxPlayers = "Configurable (default 8)",
                    Features = new()
                    {
                        "Shared soft-body physics driving on any BeamNG.drive map",
                        "Map, player limit and cars-per-player set in ServerConfig.toml",
                        "Vehicle and map mods sent to joining players from Resources/Client",
                        "Public listing with an AuthKey, or Private = true to hide the server",
                        "Optional guest access (AllowGuests) and chat logging",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "30814", Protocol = "TCP+UDP", Purpose = "Game traffic" },
                    },
                    SetupTips = new()
                    {
                        "Create an AuthKey at keymaster.beammp.com (Discord login, then Keys > +) and paste it into AuthKey in ServerConfig.toml. Never share this key. One key runs only one server at a time.",
                        "Open port 30814 for both TCP and UDP on the Firewall Rules page, or use Connect to Tunnel. The server supports IPv4 only.",
                        "Set Map to a path ending in /info.json, for example /levels/ks_nord/info.json. For a modded map, use the folder name inside the mod's levels folder.",
                        "Put vehicle and map mod .zip files in Resources/Client. They are sent to players when they join.",
                        "Set Private = false if you want the server on the public list. While Private = true, players must connect directly.",
                        "On Windows, install the Microsoft Visual C++ Redistributables if the server will not start. Stop it with the 'exit' console command.",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/284160/ss_ae4c258e6f13ddf00ee8e72be8bd0b0fc5e67801.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/284160/ss_50b8ba7ff80d50af3bd6b1b4e326849555a952a2.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/284160/ss_e55bbb37fa7208d720d15e69c23c8d932283b4fe.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/284160/ss_ac6c91223f4f12662a9ae7c3a06e2c951a2551f0.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/284160/ss_1b129249ad662d242a81a34ee2fdcdcbe064eb32.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/284160/ss_9cff66c2f780d440c123c7f6307c3eb72d8aacc0.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "rust",
                Name = "Rust Server",
                Description = "Official Rust dedicated server from Facepunch, installed through Valve's SteamCMD (anonymous).",
                Genres = new() { "Survival" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/252490/header.jpg",
                    Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/252490/library_hero.jpg",
                    Accent = "#CE422B",
                },
                Install = new InstallSpec
                {
                    Provider = "steamcmd",
                    Variants = new(),
                    AppId = 258550,
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_root}\\RustDedicated.exe",
                    Arguments = "-batchmode +server.port {server_port} +server.hostname \"{server_name}\" +server.identity \"main\" +server.maxplayers 50",
                    WorkingDir = "{server_root}",
                    StopCommand = "quit",
                },
                Network = new NetworkSpec
                {
                    DefaultPort = 28015,
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
                    About = "Rust is a multiplayer survival game from Facepunch Studios. Players start with almost nothing on a large island, gather resources, craft tools and weapons, build bases and deal with wildlife, the environment and other players, all of whom may be friendly or hostile.\n\nA dedicated Rust server gives you full control over the world: map seed and size, server name and description, player limit, wipe schedule and admin team. The official RustDedicated server is free to install through SteamCMD and supports remote administration over RCON and the Rust+ companion app.\n\nRust servers are fairly heavy. Facepunch's wiki lists about 12 GB of free RAM as a starting point, with more needed for larger maps and higher populations.",
                    Developer = "Facepunch Studios",
                    Publisher = "Facepunch Studios",
                    ReleaseDate = "Feb 8, 2018",
                    Website = "https://rust.facepunch.com/",
                    StoreUrl = "https://store.steampowered.com/app/252490/",
                    DocsUrl = "https://wiki.facepunch.com/rust/Creating-a-server",
                    MaxPlayers = "Configurable via +server.maxplayers (this app launches with 50)",
                    Features = new()
                    {
                        "Official RustDedicated server, installed anonymously via SteamCMD (app 258550)",
                        "Procedurally generated maps controlled by server.seed and server.worldsize",
                        "Remote administration over RCON (WebSocket mode with rcon.web 1)",
                        "Rust+ companion app support for players and admins",
                        "Separate server identities keep multiple worlds and configs apart",
                        "Valve Anti-Cheat enabled",
                    },
                    ServerRequirements = new ServerRequirementsSpec
                    {
                        MinRamMb = 12288,
                        DiskGb = 15,
                        Notes = "Facepunch wiki: 12 GB free RAM (larger maps such as 6k use more) and 15 GB free disk, SSD/NVMe strongly preferred. Needs grow with population and usage.",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "28015", Protocol = "UDP", Purpose = "Game traffic (server.port)" },
                        new GamePortInfo { Port = "28016", Protocol = "UDP", Purpose = "Server query (defaults to 1 + the higher of server.port and rcon.port)" },
                        new GamePortInfo { Port = "28015", Protocol = "TCP", Purpose = "RCON (rcon.port defaults to server.port; often set to 28016)" },
                        new GamePortInfo { Port = "28082", Protocol = "TCP", Purpose = "Rust+ companion app (app.port, defaults to game/RCON port + 67)" },
                    },
                    SetupTips = new()
                    {
                        "The app starts RustDedicated with -batchmode, +server.port 28015, +server.identity \"main\" and +server.maxplayers 50. Change these in the server's launch settings.",
                        "Open UDP 28015 (game) and UDP 28016 (query) in Firewall Rules or with Connect to Tunnel. Without the query port, the server will not show in the in-game browser.",
                        "For remote RCON, add +rcon.port 28016 +rcon.password \"<strong password>\" +rcon.web 1. The query port then moves to 28017, so open that instead.",
                        "Open TCP 28082 if you want Rust+ to work. To turn Rust+ off, launch with +app.port 1- (the wiki's workaround for -1).",
                        "Add +server.seed and +server.worldsize to fix the map. Changing either one generates a new world.",
                        "The first boot generates the map and can take several minutes. Wait for it to finish before connecting.",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/252490/ss_271feae67943bdc141c1249aba116349397e9ba9.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/252490/ss_e825b087b95e51c3534383cfd75ad6e8038147c3.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/252490/ss_0e646f1a70e5cb8eed00efef8adb9579d40d5b2e.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/252490/69dc23d53163b755e809a562a377bd276154c70e/ss_69dc23d53163b755e809a562a377bd276154c70e.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/252490/ss_1c2d0d1eefee54f0c67626c74eb21699bbb0ef52.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/252490/ss_d0fdacaeef5a28a7cee525fd73376adfe083c964.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "valheim",
                Name = "Valheim Server",
                Description = "Official Valheim dedicated server from Iron Gate, installed through Valve's SteamCMD (anonymous). Password is 'mpserver' by default — change it in the server settings.",
                Genres = new() { "Survival", "Adventure" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/892970/header.jpg",
                    Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/892970/library_hero.jpg",
                    Accent = "#7F1D1D",
                },
                Install = new InstallSpec
                {
                    Provider = "steamcmd",
                    Variants = new(),
                    AppId = 896660,
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_root}\\valheim_server.exe",
                    Arguments = "-nographics -batchmode -name \"{server_name}\" -port {server_port} -world \"Dedicated\" -password \"mpserver\" -public 1",
                    WorkingDir = "{server_root}",
                },
                Network = new NetworkSpec
                {
                    DefaultPort = 2456,
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
                    About = "Valheim is a co-op exploration and survival game from Iron Gate, set in a procedurally generated purgatory inspired by Norse mythology. Players gather resources, build longhouses and ships, craft gear and take on increasingly dangerous biomes and bosses together.\n\nA dedicated server keeps your world running around the clock, so your group can drop in whenever they like without one player hosting from their game client. Iron Gate's free Valheim Dedicated Server tool handles world saves, automatic backups, world modifiers and admin, ban and permitted-player lists.\n\nServers can be listed publicly or kept private with a password. An optional crossplay backend relays traffic so Xbox and Game Pass players can join without port forwarding.",
                    Developer = "Iron Gate AB",
                    Publisher = "Coffee Stain Publishing",
                    ReleaseDate = "Sep 9, 2026",
                    Website = "https://www.valheim.com/",
                    StoreUrl = "https://store.steampowered.com/app/892970/",
                    DocsUrl = "https://www.valheim.com/support/a-guide-to-dedicated-servers/",
                    MaxPlayers = "Up to 10 players",
                    Features = new()
                    {
                        "Official Valheim Dedicated Server, installed anonymously via SteamCMD (app 896660)",
                        "Persistent world that stays online without a player hosting",
                        "Automatic saves and rotating backups (-saveinterval, -backups)",
                        "World modifier presets such as Casual, Hard and Hardcore (-preset)",
                        "Admin, ban and permitted-player lists in the save folder",
                        "Optional crossplay backend that needs no port forwarding (-crossplay)",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "2456-2457", Protocol = "UDP", Purpose = "Game traffic and Steam query (the -port value and port + 1)" },
                    },
                    SetupTips = new()
                    {
                        "The app launches valheim_server.exe with -name, -port 2456, -world \"Dedicated\", -password \"mpserver\" and -public 1. Change the default password before inviting anyone.",
                        "Open UDP 2456-2457 in Firewall Rules or with Connect to Tunnel. Valheim uses the -port value and the port after it.",
                        "Worlds and the adminlist.txt, bannedlist.txt and permittedlist.txt files are stored in %USERPROFILE%\\AppData\\LocalLow\\IronGate\\Valheim unless you add -savedir.",
                        "Add -crossplay to let Xbox and Game Pass players join through Iron Gate's relay. Those players connect using a join code or the server list instead of a forwarded port.",
                        "Set -public 0 to hide the server from the community list. Friends can still join by IP and port.",
                        "Add -saveinterval (seconds, default 1800) and -backups to tune autosaves and backup retention.",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/892970/5a31353178f635b67f0c8720ebb8a1a95b637276/ss_5a31353178f635b67f0c8720ebb8a1a95b637276.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/892970/bc7c361dab1780543a3ceca50c8fba831e5bc8db/ss_bc7c361dab1780543a3ceca50c8fba831e5bc8db.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/892970/ss_a600a7d4ca954543e22f571a9629521a13f82143.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/892970/f7a47b4bd150ce1a2c6ec88f7d79fd57e7c538c8/ss_f7a47b4bd150ce1a2c6ec88f7d79fd57e7c538c8.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/892970/ss_cd0262c5abf8a90ee5e1059acafc5a92b6be0e73.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/892970/ss_3db385fc1223914dadb199ac8682683a8c59454e.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "gmod",
                Name = "Garry's Mod Server",
                Description = "Official SRCDS dedicated server for Garry's Mod, installed through Valve's SteamCMD (anonymous).",
                Genres = new() { "Sandbox" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/4000/header.jpg",
                    Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/4000/library_hero.jpg",
                    Accent = "#60A5FA",
                },
                Install = new InstallSpec
                {
                    Provider = "steamcmd",
                    Variants = new(),
                    AppId = 4020,
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_root}\\srcds.exe",
                    Arguments = "-console -game garrysmod +map gm_construct +maxplayers 16 -port {server_port} -norestart",
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
                    Folder = "garrysmod\\addons",
                    Extensions = new() { ".gma" },
                    Kind = "addon",
                },
                Details = new GameDetails
                {
                    About = "Garry's Mod is Facepunch's physics sandbox built on Valve's Source engine. It has no fixed goals. Players spawn props, ragdolls, NPCs and vehicles, weld contraptions together and play a huge range of community gamemodes, from Trouble in Terrorist Town and Prop Hunt to DarkRP.\n\nA dedicated Garry's Mod server runs Valve's Source Dedicated Server (SRCDS). It lets you choose the gamemode, map rotation, player slots and admin tools, and install addons from the Steam Workshop or drop .gma files into the addons folder.\n\nThe server is free to download through SteamCMD. Facepunch's wiki recommends a Game Server Login Token so your server is ranked properly in the server browser.",
                    Developer = "Facepunch Studios",
                    Publisher = "Valve",
                    ReleaseDate = "Nov 29, 2006",
                    Website = "https://gmod.facepunch.com/",
                    StoreUrl = "https://store.steampowered.com/app/4000/",
                    DocsUrl = "https://wiki.facepunch.com/gmod/Downloading_a_Dedicated_Server",
                    MaxPlayers = "Configurable via +maxplayers (this app launches with 16)",
                    Features = new()
                    {
                        "Official SRCDS dedicated server, installed anonymously via SteamCMD (app 4020)",
                        "Run sandbox or any installed gamemode (+gamemode)",
                        "Auto-install Workshop addons with +host_workshop_collection",
                        "Drop-in .gma addons in garrysmod\\addons",
                        "Lua scripting for custom gamemodes and server-side tools",
                        "Valve Anti-Cheat enabled",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "27015", Protocol = "UDP", Purpose = "Game traffic and server queries (-port)" },
                        new GamePortInfo { Port = "27015", Protocol = "TCP", Purpose = "RCON remote console" },
                        new GamePortInfo { Port = "27020", Protocol = "UDP", Purpose = "SourceTV (only if enabled; +tv_port)" },
                    },
                    SetupTips = new()
                    {
                        "The app starts srcds.exe with -console -game garrysmod +map gm_construct +maxplayers 16 -port 27015. Edit these to change the starting map or slot count.",
                        "Generate a Game Server Login Token for app 4000 on Steam and add +sv_setsteamaccount <token>. Facepunch says servers without one are penalized in the browser ranking.",
                        "Add +host_workshop_collection <collectionID> to download a Workshop collection automatically, or place .gma files in garrysmod\\addons.",
                        "Pick a gamemode with +gamemode (for example sandbox or terrortown) and make sure its files are installed.",
                        "Allow 27015 (UDP for players, TCP for RCON) in Firewall Rules or with Connect to Tunnel. Set rcon_password in garrysmod\\cfg\\server.cfg only if you need remote RCON.",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/4000/ss_af060d0a9aeb6598e154c9704ce250b0e63b2266.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/4000/ss_d314d7dba0987b3e7f49154964bda0ae74ee5161.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/4000/ss_3d320f333fb74413911cbc2bffae9bdda683e2ec.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/4000/ss_12741a4344b5cfee33726ae219476c8634517f7d.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/4000/ss_74a181b4584a04c88ee316df31a850ab2810fb79.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/4000/ss_a6133770663ff34d535adf050f2af64ab47a6974.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "pixark",
                Name = "PixARK Dedicated Server",
                Description = "Official PixARK dedicated server (Snail Games), installed through Valve's SteamCMD (anonymous). A voxel sandbox mashup of ARK and block worlds — tame dinosaurs, build bases and explore a procedurally generated cube world.",
                Genres = new() { "Survival", "Sandbox" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/593600/header.jpg",
                    Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/593600/library_hero.jpg",
                    Accent = "#38BDF8",
                },
                Install = new InstallSpec
                {
                    Provider = "steamcmd",
                    Variants = new(),
                    AppId = 824360,
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_root}\\ShooterGame\\Binaries\\Win64\\PixARKServer.exe",
                    // Per Snail's pinned "Settings Needed for Private Servers" guide: CubePort (TCP
                    // terrain data) must be set, -NoBattlEye is required and the server supports
                    // English only (-CULTUREFORCOOKING=en). Port/QueryPort go both in the map URL
                    // and as dash flags (the guide uses the flags, hosts commonly the URL form).
                    Arguments = "\"CubeWorld_Light?listen?SessionName={server_name}?MaxPlayers=20?Port={server_port}?QueryPort=27015?RCONEnabled=False?RCONPort=27020\" -Port={server_port} -QueryPort=27015 -CubePort=15000 -cubeworld=world -CULTUREFORCOOKING=en -NoBattlEye -NoHangDetection -server -log",
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
                    About = "PixARK is a voxel survival sandbox from Snail Games that takes ARK-style dinosaur taming and puts it in a procedurally generated cube world. You gather materials, craft tools and weapons, tame creatures and build bases alone or with a tribe.\n\nRunning your own dedicated server gives your group a persistent world that keeps going between sessions. You choose the server type (Pioneering PvE, or the Fury and Chaos PvP rulesets), set passwords and player limits, and run several worlds side by side by giving each its own ports.",
                    Developer = "Snail Games USA",
                    Publisher = "Snail Games USA",
                    ReleaseDate = "May 31, 2019",
                    StoreUrl = "https://store.steampowered.com/app/593600/",
                    DocsUrl = "https://steamcommunity.com/app/593600/discussions/0/1697168437881708051",
                    MaxPlayers = "Configurable (MaxPlayers launch option; this app launches with 20)",
                    Features = new()
                    {
                        "Voxel survival sandbox with ARK-style creature taming",
                        "Procedurally generated cube worlds",
                        "Pioneering (PvE), Fury and Chaos (PvP) server types",
                        "Tribes, base building and crafting",
                        "Server password, admin password and RCON options",
                        "Several servers can run on one machine with separate ports",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "7777", Protocol = "UDP", Purpose = "Game traffic (Port, the app's default)" },
                        new GamePortInfo { Port = "27014-27015", Protocol = "UDP", Purpose = "Steam query (QueryPort 27015; the server also uses the port just below it)" },
                        new GamePortInfo { Port = "15000", Protocol = "TCP", Purpose = "Terrain data (CubePort); must be reachable or players see no terrain" },
                        new GamePortInfo { Port = "27020", Protocol = "TCP", Purpose = "RCON (RCONPort, only when RCONEnabled=True)" },
                    },
                    SetupTips = new()
                    {
                        "The app launches ShooterGame\\Binaries\\Win64\\PixARKServer.exe on CubeWorld_Light with your server name, MaxPlayers=20, Port (default 7777), QueryPort 27015 and CubePort 15000, plus -NoBattlEye, -CULTUREFORCOOKING=en and -NoHangDetection, which Snail's server guide lists as required.",
                        "Open UDP 7777, UDP 27014-27015 and TCP 15000 on the Firewall Rules page or through Connect to Tunnel. The CubePort (TCP) carries terrain data: if it is blocked, players join but see no terrain.",
                        "For a private server add ?ServerPassword=<password> inside the quoted map URL. For RCON, set RCONEnabled=True, add ?ServerAdminPassword=<password> and open TCP 27020.",
                        "Set the server type in GameUserSettings.ini under Config\\WindowsServer: ServerPVE=true for Pioneering. For Fury use ServerPVE=false, CanPVPAttack=true, ServerPVPCanAttack=false. For Chaos use ServerPVE=false, CanPVPAttack=false, ServerPVPCanAttack=true.",
                        "When running several servers, give each its own Port, QueryPort, CubePort and -cubeworld name. The official example spaces the query ports two apart (27019, 27021, 27023) because each one also uses the port below it.",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/593600/ss_af25791685ccb02a74ccdf41a3a6e4d14ea6eac9.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/593600/ss_56115b4b0002b9052e4aa905d64d336d9e838260.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/593600/ss_1bc99af9fb36e8b757ec235515154625484ef120.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/593600/ss_125547aac2af9dc0204307b0ea2fddef5026ad0b.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/593600/ss_dfe17070a7af9795fc0ba773fb47aaefbd147ff3.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/593600/ss_118403eca87e5610486e83feb05adbf410de9199.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "unturned",
                Name = "Unturned Dedicated Server",
                Description = "Official Unturned dedicated server from Smartly Dressed Games, installed through Valve's SteamCMD (anonymous). Band together or betray your friends in a blocky post-apocalyptic sandbox.",
                Genres = new() { "Survival", "Sandbox" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/304930/header.jpg",
                    Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/304930/library_hero.jpg",
                    Accent = "#4F6BFF",
                },
                Install = new InstallSpec
                {
                    Provider = "steamcmd",
                    Variants = new(),
                    AppId = 1110390,
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_root}\\Unturned.exe",
                    Arguments = "-nographics -batchmode +secureserver/{server_name}",
                    WorkingDir = "{server_root}",
                    StopCommand = "shutdown",
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
                    About = "Unturned is a free-to-play open-world zombie survival sandbox from Smartly Dressed Games. Players scavenge for food, weapons and vehicles, build bases and either cooperate or fight each other across maps such as PEI, Washington, Russia and Germany, plus many curated Workshop maps.\n\nThe Unturned Dedicated Server (U3DS) lets you run your own world with your choice of map, player cap, PvP or PvE rules, perspective lock and difficulty settings. Mods and maps from the Steam Workshop can be listed in a config file, and the server downloads and updates them on startup. Connecting players get them automatically.\n\nServers can run as LAN or Internet servers. Internet servers need a Game Server Login Token, and can be reached with either port forwarding or Steam's Fake IP relay.",
                    Developer = "Smartly Dressed Games",
                    Publisher = "Smartly Dressed Games",
                    ReleaseDate = "Jul 7, 2017",
                    Website = "https://smartlydressedgames.com/unturned/",
                    StoreUrl = "https://store.steampowered.com/app/304930/",
                    DocsUrl = "https://docs.smartlydressedgames.com/en/stable/servers/server-hosting.html",
                    MaxPlayers = "Configurable via MaxPlayers in Commands.dat (official example uses 24)",
                    Features = new()
                    {
                        "Official Unturned Dedicated Server (U3DS), installed anonymously via SteamCMD (app 1110390)",
                        "Official and curated Workshop maps (Map command)",
                        "Workshop mods auto-installed via WorkshopDownloadConfig.json",
                        "Difficulty presets and detailed gameplay settings in the server config",
                        "PvE mode, perspective locks and admin/cheat commands",
                        "Reachable without port forwarding using Server Codes or Fake IP",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "27015-27016", Protocol = "UDP", Purpose = "Server list queries (port) and game traffic (port + 1)" },
                    },
                    SetupTips = new()
                    {
                        "The app launches Unturned.exe -nographics -batchmode +secureserver/<server name>, so settings and saves live in the U3DS\\Servers\\<server name> folder.",
                        "Edit Server\\Commands.dat to set Name, Map, MaxPlayers, Password, Owner (your SteamID64) and Port.",
                        "For an Internet server, add a Game Server Login Token in the server config. This also keeps the Server Code the same between launches.",
                        "Forward UDP 27015-27016 (or the Port value and port + 1) with Firewall Rules or Connect to Tunnel, or enable Fake IP if you only need Server Code joins.",
                        "There is no autosave. Use the Save command, or stop with 'shutdown' (the app's stop command), so progress is written to disk.",
                        "List Workshop file IDs in WorkshopDownloadConfig.json to install maps and mods. When running several servers, space their ports two apart (27015, 27017, 27019).",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/304930/a2d8ffa4eaff717539c1fd9d3cd0440691c6fc81/ss_a2d8ffa4eaff717539c1fd9d3cd0440691c6fc81.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/304930/c0f8253a44a7e322ee287e309e3596d17d9aacba/ss_c0f8253a44a7e322ee287e309e3596d17d9aacba.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/304930/a4200b3c585bde5934230580162899c92a483610/ss_a4200b3c585bde5934230580162899c92a483610.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/304930/accee15f453a1f6b981114579f2a67440563d850/ss_accee15f453a1f6b981114579f2a67440563d850.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/304930/e502ef25688ec9415914ca018e8318698117248d/ss_e502ef25688ec9415914ca018e8318698117248d.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/304930/7382b0dfbc4bc44986e888c263be3535cef11a31/ss_7382b0dfbc4bc44986e888c263be3535cef11a31.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "7-days-to-die-dedicated-server",
                Name = "7 Days to Die Dedicated Server",
                Description = "7 Days to Die is an open-world game that is a unique combination of first-person shooter, survival horror, tower defense, and role-playing games. Play the definitive zombie survival sandbox RPG that came first. Navezgane awaits!",
                Genres = new() { "Action", "Adventure", "Indie", "RPG", "Simulation", "Strategy" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/251570/header.jpg?t=1784044715",
                    Hero = "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/251570/header.jpg?t=1784044715",
                    Accent = "#4F6BFF",
                },
                Install = new InstallSpec
                {
                    Provider = "steamcmd",
                    Variants = new(),
                    AppId = 294420,
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_root}\\startdedicated.bat",
                    Arguments = "",
                    WorkingDir = "{server_root}",
                    StopCommand = "quit",
                },
                Network = new NetworkSpec
                {
                    // 7 Days to Die's own ServerPort default (serverconfig.xml), not Source's 27015.
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
                    About = "7 Days to Die is a voxel-based survival game from The Fun Pimps. It mixes first-person shooting, crafting, base building and RPG-style progression. You scavenge a ruined world by day and fortify against the zombie horde that grows stronger every seventh night. The whole world can be destroyed and rebuilt, so the bases you design really matter.\n\nThe free dedicated server tool keeps your world running even when the host is not playing. Friends can drop in, keep their progress and build together on a shared map. You control the rules through serverconfig.xml: world type (Navezgane or a random world), difficulty, player count, passwords and much more. Optional telnet and web dashboard interfaces let you run admin tasks remotely.",
                    Developer = "The Fun Pimps",
                    Publisher = "The Fun Pimps Entertainment LLC",
                    ReleaseDate = "Jul 25, 2024",
                    Website = "https://7daystodie.com",
                    StoreUrl = "https://store.steampowered.com/app/251570/",
                    DocsUrl = "https://7daystodie.wiki.gg/wiki/Server:_serverconfig.xml",
                    MaxPlayers = "Configurable (default 8 via ServerMaxPlayerCount)",
                    Features = new()
                    {
                        "Open-world zombie survival with crafting, building and RPG progression",
                        "Fully destructible voxel world with base building",
                        "Horde nights every seventh day that grow in difficulty",
                        "Choose the handcrafted Navezgane map or a randomly generated world",
                        "All server rules are set in serverconfig.xml",
                        "Optional telnet console and web dashboard for remote admin",
                        "Cross-platform multiplayer",
                    },
                    ServerRequirements = new ServerRequirementsSpec
                    {
                        Notes = "No official server-only figures are published. The Steam store lists 8 GB RAM minimum and 12 GB recommended for the game client, with 15 GB of disk, and warns that running a dedicated server and the client on the same PC doubles the RAM requirement.",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "26900", Protocol = "TCP+UDP", Purpose = "Game traffic and server list (ServerPort)" },
                        new GamePortInfo { Port = "26901-26902", Protocol = "UDP", Purpose = "Additional game/networking traffic (ServerPort +1 and +2)" },
                        new GamePortInfo { Port = "8080", Protocol = "TCP", Purpose = "Web control panel (optional, disabled by default)" },
                        new GamePortInfo { Port = "8081", Protocol = "TCP", Purpose = "Telnet admin console (enabled by default)" },
                    },
                    SetupTips = new()
                    {
                        "The app starts the server with startdedicated.bat in the server folder, which reads serverconfig.xml from there. Set ServerName, ServerPassword, ServerMaxPlayerCount and GameWorld in that file before the first launch.",
                        "The game's default ServerPort is 26900. Open it on the Firewall Rules page or through Connect to Tunnel together with the next two UDP ports (26901-26902), and keep it matching ServerPort in serverconfig.xml.",
                        "If you change ServerPort, the extra UDP ports move with it. For example, a server on 26920 also uses 26921-26922.",
                        "Telnet is enabled by default on port 8081. Set TelnetPassword and do not expose 8081 or 8080 to the internet unless you need remote admin.",
                        "Keep a backup copy of your edited serverconfig.xml, because a SteamCMD update or verify can replace the stock file.",
                        "If you also play on the same PC, expect roughly double the RAM use: Steam's requirements warn that a server and client together need twice the memory.",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/251570/766ed3debfe7b894fbb5650aa89fa213fa254d5d/ss_766ed3debfe7b894fbb5650aa89fa213fa254d5d.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/251570/2d243d95de4989b37df448f29de123f0d662088c/ss_2d243d95de4989b37df448f29de123f0d662088c.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/251570/33edbeefc7f5ff8ccda4bda274b01eff84eb4eb8/ss_33edbeefc7f5ff8ccda4bda274b01eff84eb4eb8.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/251570/70c05a775f747e9d556435bfd8795536ddd4117d/ss_70c05a775f747e9d556435bfd8795536ddd4117d.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/251570/8aff547df25c2d393fbdef2977dc109806de3112/ss_8aff547df25c2d393fbdef2977dc109806de3112.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/251570/d7b2bba330c25e1eae848d8d8bb7321b9b1833d0/ss_d7b2bba330c25e1eae848d8d8bb7321b9b1833d0.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "ats",
                Name = "American Truck Simulator Server",
                Description = "Official ATS dedicated server via SteamCMD. Needs server_packages files exported from your game client (see SCS documentation).",
                Genres = new() { "Simulation" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/270880/header.jpg",
                    Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/270880/library_hero.jpg",
                    Accent = "#64748B",
                },
                Install = new InstallSpec
                {
                    Provider = "steamcmd",
                    Variants = new(),
                    AppId = 2239530,
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_root}\\bin\\win_x64\\amtrucks_server.exe",
                    Arguments = "",
                    WorkingDir = "{server_root}",
                },
                Network = new NetworkSpec
                {
                    DefaultPort = 27015,
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
                    About = "American Truck Simulator is SCS Software's US trucking sim. You drive licensed American trucks across a growing map of western and central states, deliver cargo, and build your own trucking business. Convoy mode puts friends in the same world to drive together.\n\nSCS's official dedicated server hosts a Convoy session without anyone keeping the game open as host. It reads the map, DLC and mod setup from server_packages files exported from your own game client, so the server itself needs no DLC or mod data. Lobby name, password, traffic, damage and moderator settings live in server_config.sii.",
                    Developer = "SCS Software",
                    Publisher = "SCS Software",
                    ReleaseDate = "Feb 2, 2016",
                    Website = "https://americantrucksimulator.com/",
                    StoreUrl = "https://store.steampowered.com/app/270880/",
                    DocsUrl = "https://modding.scssoft.com/wiki/Documentation/Tools/Dedicated_Server",
                    MaxPlayers = "Up to 8 players",
                    Features = new()
                    {
                        "Official SCS dedicated server for Convoy multiplayer",
                        "Map, DLC and mod setup taken from server_packages files exported from your game",
                        "Lobby name, password, welcome message and description settings",
                        "Toggles for AI traffic, player damage, speed limiter and name tags",
                        "Session moderators with chat commands such as /set_time and /set_rain_factor",
                        "Optional Steam logon token for a persistent server ID",
                    },
                    ServerRequirements = new ServerRequirementsSpec
                    {
                        Notes = "SCS's dedicated server documentation does not list hardware requirements. The server needs no DLC or mod data, only the exported server_packages.sii and server_packages.dat files.",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "27015", Protocol = "TCP+UDP", Purpose = "Game connection (connection_dedicated_port)" },
                        new GamePortInfo { Port = "27016", Protocol = "TCP+UDP", Purpose = "Steam query / session browser (query_dedicated_port)" },
                    },
                    SetupTips = new()
                    {
                        "In your ATS client, set uset g_console \"1\" in config.cfg, open the console with ~ and run export_server_packages. Copy the generated server_packages.sii and server_packages.dat into the server's home folder.",
                        "The first launch creates a default server_config.sii and reports that server packages are missing until you copy them in. Edit lobby_name, password and max_players there (8 is the maximum).",
                        "Open connection_dedicated_port 27015 (the app's default) and query_dedicated_port 27016 for both TCP and UDP so the server shows in the session browser. The virtual ports (100/101, allowed range 100-200) do not need forwarding.",
                        "Without port forwarding, players can still join through direct search using the server ID printed when the server starts.",
                        "The anonymous server gets a new ID each start. For a persistent ID, create a logon token at steamcommunity.com/dev/managegameservers using the game's app ID 270880 (not the server's) and set server_logon_token.",
                        "Add trusted players' SteamID64 values to moderator_list so they can use /set_time and /set_rain_factor in chat.",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/270880/ss_758470ddbd783e7ff0caac1a59b7fde4a74bbb67.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/270880/ss_ba330377eb0a5bffcdbe10f89d3670b83ad55b3f.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/270880/ss_f0c114cadf2c3c02f54608c6751d5936a534061c.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/270880/ss_7825c812221cb25b97cbe232272efb380e93df94.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/270880/ss_253075cab0028149aa3e449a341ba9054174b018.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/270880/e72c55ae52f0939c3dd5c76504197760296f7f42/ss_e72c55ae52f0939c3dd5c76504197760296f7f42.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "ets2",
                Name = "Euro Truck Simulator 2 Server",
                Description = "Official ETS2 dedicated server via SteamCMD. Needs server_packages files exported from your game client (see SCS documentation).",
                Genres = new() { "Simulation" },
                Artwork = new ArtworkSpec
                {
                    Cover = "https://cdn.cloudflare.steamstatic.com/steam/apps/227300/header.jpg",
                    Hero = "https://cdn.cloudflare.steamstatic.com/steam/apps/227300/library_hero.jpg",
                    Accent = "#64748B",
                },
                Install = new InstallSpec
                {
                    Provider = "steamcmd",
                    Variants = new(),
                    AppId = 1948160,
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    Executable = "{server_root}\\bin\\win_x64\\eurotrucks2_server.exe",
                    Arguments = "",
                    WorkingDir = "{server_root}",
                },
                Network = new NetworkSpec
                {
                    DefaultPort = 27015,
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
                    About = "Euro Truck Simulator 2 is SCS Software's long-running trucking sim. You drive licensed trucks across a large map of Europe, deliver cargo, and grow your own haulage company. Its Convoy mode lets friends share one world and drive together.\n\nSCS's official dedicated server lets you host a Convoy session without keeping the game open on a host PC. The server uses the map, DLC and mod setup you export from your own game client as server_packages files, so it needs no DLC or mod data of its own. Settings such as lobby name, password, traffic, player damage and moderators live in server_config.sii.",
                    Developer = "SCS Software",
                    Publisher = "SCS Software",
                    ReleaseDate = "Oct 12, 2012",
                    Website = "https://eurotrucksimulator2.com/",
                    StoreUrl = "https://store.steampowered.com/app/227300/",
                    DocsUrl = "https://modding.scssoft.com/wiki/Documentation/Tools/Dedicated_Server",
                    MaxPlayers = "Up to 8 players",
                    Features = new()
                    {
                        "Official SCS dedicated server for Convoy multiplayer",
                        "Map, DLC and mod setup taken from server_packages files exported from your game",
                        "Lobby name, password, welcome message and description settings",
                        "Toggles for AI traffic, player damage, speed limiter and name tags",
                        "Session moderators with chat commands such as /set_time and /set_rain_factor",
                        "Optional Steam logon token for a persistent server ID",
                    },
                    ServerRequirements = new ServerRequirementsSpec
                    {
                        Notes = "SCS's dedicated server documentation does not list hardware requirements. The server needs no DLC or mod data, only the exported server_packages.sii and server_packages.dat files.",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "27015", Protocol = "TCP+UDP", Purpose = "Game connection (connection_dedicated_port)" },
                        new GamePortInfo { Port = "27016", Protocol = "TCP+UDP", Purpose = "Steam query / session browser (query_dedicated_port)" },
                    },
                    SetupTips = new()
                    {
                        "In your game client, set uset g_console \"1\" in config.cfg, open the console with ~ and run export_server_packages. Copy the generated server_packages.sii and server_packages.dat into the server's home folder.",
                        "The first launch creates a default server_config.sii and reports that server packages are missing until you copy them in. Edit lobby_name, password and max_players there (8 is the maximum).",
                        "Open connection_dedicated_port 27015 (the app's default) and query_dedicated_port 27016 for both TCP and UDP so the server shows in the session browser. The virtual ports (100/101, allowed range 100-200) do not need forwarding.",
                        "Without port forwarding, players can still join through direct search using the server ID printed when the server starts.",
                        "The anonymous server gets a new ID each start. For a persistent ID, create a logon token at steamcommunity.com/dev/managegameservers using the game's app ID 227300 (not the server's) and set server_logon_token.",
                        "Add trusted players' SteamID64 values to moderator_list so they can use /set_time and /set_rain_factor in chat.",
                    },
                    Screenshots = new()
                    {
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/227300/e040f74641ac21f15e3ec7c2415fc8b4de0b6bf9/ss_e040f74641ac21f15e3ec7c2415fc8b4de0b6bf9.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/227300/edbff61a062a49d1d97269e53b8afbffde05bb24/ss_edbff61a062a49d1d97269e53b8afbffde05bb24.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/227300/c610d985eb21cc22c26507b53cd843cfa0b3a00d/ss_c610d985eb21cc22c26507b53cd843cfa0b3a00d.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/227300/a944c7f801629bfa0cfb91fa7b23b4ff95e11753/ss_a944c7f801629bfa0cfb91fa7b23b4ff95e11753.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/227300/06aa11ece5d3303cfcfeafb3c81734258a7dd614/ss_06aa11ece5d3303cfcfeafb3c81734258a7dd614.1920x1080.jpg",
                        "https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/227300/5dff1bca6580b5df288a4ad061e4f92f62f6a073/ss_5dff1bca6580b5df288a4ad061e4f92f62f6a073.1920x1080.jpg",
                    },
                },
            },
            new GameDefinition
            {
                ManifestVersion = 1,
                Id = "openrct2",
                Name = "OpenRCT2 Server",
                Description = "Host co-op theme park building with OpenRCT2, the open-source RollerCoaster Tycoon 2 engine, as a headless dedicated server.",
                Genres = new() { "Simulation", "Strategy" },
                Artwork = new ArtworkSpec
                {
                    // The old URL ("https://openrct2.org/uploads/images/logo.png")
                    // now 404s — the site's asset layout changed.
                    // A later stand-in, the 64x64 GitHub app icon, rendered blurry stretched over
                    // a wide card. The cover is now a banner bundled with the app
                    // (App/Assets/Artwork/openrct2-banner.png, 1840x860): the RCT2 helter-skelter
                    // emblem from SteamGridDB grid 94151 centered on openrct2.io's park artwork.
                    // The detail-page hero is openrct2.io's front-page background (1920x1080).
                    Cover = "asset:openrct2-banner.png",
                    Hero = "https://openrct2.io/uploads/images/background.jpg",
                    Accent = "#2563EB",
                },
                Install = new InstallSpec
                {
                    Provider = "url",
                    Variants = new(),
                    // Release assets carry the version (OpenRCT2-v0.5.7-windows-portable-x64.zip),
                    // so there is no stable releases/latest/download URL: the url provider
                    // resolves the latest release's portable Windows x64 zip via the GitHub API.
                    GithubRepo = "OpenRCT2/OpenRCT2",
                    AssetPattern = @"^OpenRCT2-v?[0-9][0-9A-Za-z.+-]*-windows-portable-x64\.zip$",
                    Archive = "zip",
                    Anonymous = true,
                },
                Runtime = new RuntimeSpec
                {
                    // openrct2.com is the console-subsystem build shipped next to openrct2.exe
                    // (stdin/stdout work). Command line as documented by `openrct2 --help`:
                    // host <park> --port <port> --headless. The user data dir (config.ini, saves)
                    // lives in the server folder so every server keeps its own settings.
                    Executable = "{server_root}\\openrct2.com",
                    Arguments = "host \"{server_root}\\park.park\" --port {server_port} --headless --user-data-path \"{server_root}\\user-data\"",
                    WorkingDir = "{server_root}",
                    // The headless console evaluates JavaScript; legacy console commands are
                    // reachable via console.executeLegacy. save_park with no name writes back to
                    // the loaded park file, then quit ends the game loop cleanly.
                    StopCommand = "console.executeLegacy(\"save_park\"); console.executeLegacy(\"quit\")",
                },
                Network = new NetworkSpec
                {
                    DefaultPort = 11753,
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
                    About = "OpenRCT2 is a free, open-source re-implementation of RollerCoaster Tycoon 2. You build and manage an amusement park, and it adds modern-platform support, a better interface, higher limits, extra editing tools and cooperative multiplayer. You need the original RollerCoaster Tycoon 2 (or RCT Classic) data files to run it.\n\nA dedicated OpenRCT2 server runs headless as a console app and hosts a saved park that several players build in together. The server is authoritative and keeps every client in sync tick by tick. You manage player permissions through groups, and you can run admin commands and scripts from the server console.",
                    Developer = "OpenRCT2 team",
                    Website = "https://openrct2.io",
                    StoreUrl = "https://store.steampowered.com/app/285330/",
                    DocsUrl = "https://github.com/OpenRCT2/OpenRCT2/wiki/Multiplayer",
                    MaxPlayers = "Configurable (default 16)",
                    Features = new()
                    {
                        "Free and open source (GPLv3)",
                        "Cooperative multiplayer in a shared park",
                        "Headless dedicated mode with no GUI",
                        "Group-based player permissions with an admin role",
                        "Server console scripting through the OpenRCT2 plugin API",
                        "Optional listing on the public server list",
                    },
                    ServerRequirements = new ServerRequirementsSpec
                    {
                        MinRamMb = 500,
                        RecommendedRamMb = 2048,
                        DiskGb = 1,
                        Notes = "These are OpenRCT2's general (client) system requirements, not server-specific numbers: about 500 MB of free RAM (2 GB of system RAM is enough) and about 250 MB of disk including the RCT2 files. 1 GB or more of free disk is recommended for saves and custom objects. The software renderer leans on the CPU.",
                    },
                    Ports = new()
                    {
                        new GamePortInfo { Port = "11753", Protocol = "TCP", Purpose = "Game traffic" },
                    },
                    SetupTips = new()
                    {
                        "Before the first start, copy the park you want to host into the server folder and name it park.park. Any OpenRCT2 save (.park) or scenario works. To host an older .sv6 or .sc6 file instead, change the path in the launch arguments.",
                        "OpenRCT2 needs the files of a legitimate RollerCoaster Tycoon 2 or RCT Classic copy (Steam or GOG). If the server says it cannot find them, set game_path in user-data\\config.ini inside the server folder to that game's install folder, or add --rct2-data-path \"<folder>\" to the launch arguments.",
                        "The app runs openrct2.com host \"<server folder>\\park.park\" --port <port> --headless --user-data-path \"<server folder>\\user-data\". Stopping the server saves the park back to park.park first.",
                        "Open TCP 11753 on the Firewall Rules page, or use Connect to Tunnel, so players outside your network can join.",
                        "To make yourself admin, join from your client, then in the server console run 'network.players' to find your index and 'network.players[1].group = 0' (use your index).",
                        "Players must run the same OpenRCT2 version as the server. Mismatched builds cannot connect, and desyncs drop clients until they reconnect.",
                        "Network settings such as server_name, maxplayers, default_password and advertise live in the [network] section of user-data\\config.ini in the server folder (written on first start). You can also add --password <password> to the launch arguments.",
                    },
                    Screenshots = new()
                    {
                        "https://docs.openrct2.io/en/latest/_images/screenshot.png",
                        "https://openrct2.io/uploads/images/background.jpg",
                    },
                },
            },
    };
}
