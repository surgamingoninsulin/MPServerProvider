# MPServerProvider.Core.Services.BuiltInNews

Token: `0x02000007`  Flags: `1048961`  Extends: `0x01000015`

## Fields

- `SteamCdn` : `string`  (flags=32849, token=0x0400000b)
- `MinecraftBedrockArt` : `string`  (flags=32849, token=0x0400000c)
- `MinecraftJavaArt` : `string`  (flags=32849, token=0x0400000d)
- `Released` : `valuetype:157`  (flags=49, token=0x0400000e)

## Methods

### `All()` -> `class:153<class:196>`
- token `0x0600001a`  flags=`150`  rva=`12844`
- IL size 1039 bytes, 289 instructions, max_stack=11
- opcodes: callvirtx91, dupx91, ldstrx65, newobjx16, ldsfldx13, ldc.i4.0x6, ldc.i4x2, ldc.i4.7x2, ldc.i4.sx2, retx1
- string literals: "Added", "Added 7 Days To Die Server", "7 Days to Die is an open-world, voxel-based sandbox game that blends first-person shooting, survival horror, tower defense, and role-playing elements as players craft, build, and fight to survive weekly undead hordes.", "https://cdn.cloudflare.steamstatic.com/steam/apps//294420/header.jpg", "7-days-to-die-dedicated-server", "Added", "Added PixARK Server", "Official PixARK dedicated server, installed through SteamCMD with one click. A voxel sandbox mashup of ARK and block worlds — tame dinosaurs, build bases and explore a procedurally generated cube world.", "https://cdn.cloudflare.steamstatic.com/steam/apps//593600/header.jpg", "pixark", "Added", "Added Unturned Server", "Band together or betray your friends in a massive, blocky post-apocalyptic sandbox. Forage for food, scavenge military gear, craft impenetrable bases, and modify vehicles to survive both the undead hordes and ruthless rival players.", "https://cdn.cloudflare.steamstatic.com/steam/apps//1110390/header.jpg", "unturned", "Added", "American Truck Simulator Server", "Official ATS dedicated server via SteamCMD. Needs server_packages files exported from your game client (see SCS documentation).", "https://cdn.cloudflare.steamstatic.com/steam/apps//270880/header.jpg", "ats", "Added", "Euro Truck Simulator 2 Server", "Official ETS2 dedicated server via SteamCMD. Needs server_packages files exported from your game client (see SCS documentation).", "https://cdn.cloudflare.steamstatic.com/steam/apps//227300/header.jpg", "ets2", "Added", "BeamMP Server", "Official BeamMP multiplayer server for BeamNG.drive. An AuthKey from keymaster.beammp.com is required in ServerConfig.toml.", "https://cdn.cloudflare.steamstatic.com/steam/apps//284160/header.jpg", "beammp", "Added", "FiveM Server", "Cfx.re FXServer for GTA V multiplayer. txAdmin's setup wizard handles the licence key on first start — visit http://127.0.0.1:40120 after booting.", "https://cdn.cloudflare.steamstatic.com/steam/apps//271590/header.jpg", "fivem", "Added", "RedM Server", "Cfx.re FXServer for Red Dead Redemption 2 multiplayer. Uses the same official artifacts as FiveM with the RDR3 game profile.", "https://cdn.cloudflare.steamstatic.com/steam/apps//1174180/header.jpg", "redm"...
- calls: System.Collections.Generic.List`1..ctor, MPServerProvider.Core.Models.NewsItem..ctor, MPServerProvider.Core.Models.NewsItem.set_Tag, MPServerProvider.Core.Models.NewsItem.set_Title, MPServerProvider.Core.Models.NewsItem.set_Summary, System.DateTimeOffset..ctor, MPServerProvider.Core.Models.NewsItem.set_Date, MPServerProvider.Core.Models.NewsItem.set_Image, MPServerProvider.Core.Models.NewsItem.set_GameId, System.Collections.Generic.List`1.Add

### `.cctor()` -> `void`
- token `0x0600001b`  flags=`6289`  rva=`13895`
- IL size 27 bytes, 10 instructions, max_stack=8
- opcodes: ldc.i4.0x3, ldc.i4x1, ldc.i4.7x1, ldc.i4.sx1, ldsfldx1, newobjx1, retx1, stsfldx1
- calls: System.DateTimeOffset..ctor

