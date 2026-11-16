using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.BuiltInNews</c> (decompile token 0x02000007).
/// <para>
/// Ground truth: the decompiled <c>All()</c> body's string literals directly give
/// <c>Tag="Added"</c>, <c>Title</c>, <c>Summary</c>, <c>Image</c> and <c>GameId</c> for eight
/// entries (7 Days to Die, PixARK, Unturned, ATS, ETS2, BeamMP, FiveM, RedM) before the extracted
/// evidence is truncated. Those eight are reproduced verbatim below.
/// </para>
/// <para>
/// INFERRED: the <c>.cctor()</c>'s <c>Released</c> static field is a single shared
/// <c>DateTimeOffset</c> literal (opcodes show <c>ldc.i4</c> immediates feeding one
/// <c>DateTimeOffset..ctor</c> call, then every <c>NewsItem..ctor</c> call in <c>All()</c> reuses
/// that same static field for its <c>Date</c>), but the actual year/month/day/offset immediates
/// weren't captured in the extracted evidence. This port uses a single fixed placeholder
/// timestamp (<c>2026-07-01T00:00:00Z</c>, chosen to land inside the same July 2026 window the
/// real <c>data/settings.json</c>/<c>server.json</c> fixtures were captured in) shared by every
/// entry, preserving the original's "every built-in news item has the same date" behavior even
/// though the exact value is a guess — the same judgment call already made (and verified) by the
/// Rust port.
/// </para>
/// <para>
/// The decompile evidence is truncated before showing whether further entries exist for the
/// other built-in games (minecraft-java, minecraft-bedrock, rust, valheim, gmod, openrct2);
/// rather than fabricate "Added X Server" copy for those with zero string evidence, this port
/// only includes the eight confirmed entries.
/// </para>
/// </summary>
public static class BuiltInNews
{
    /// <summary>
    /// Port of the <c>Released</c> static field. See the type docs for why this is a single
    /// inferred placeholder rather than a decompiled exact value.
    /// </summary>
    private static readonly DateTime Released = new(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);

    private static readonly (string Title, string Summary, string Image, string GameId)[] Entries =
    {
        (
            "Added 7 Days To Die Server",
            "7 Days to Die is an open-world, voxel-based sandbox game that blends first-person shooting, survival horror, tower defense, and role-playing elements as players craft, build, and fight to survive weekly undead hordes.",
            "https://cdn.cloudflare.steamstatic.com/steam/apps/294420/header.jpg",
            "7-days-to-die-dedicated-server"
        ),
        (
            "Added PixARK Server",
            "Official PixARK dedicated server, installed through SteamCMD with one click. A voxel sandbox mashup of ARK and block worlds — tame dinosaurs, build bases and explore a procedurally generated cube world.",
            "https://cdn.cloudflare.steamstatic.com/steam/apps/593600/header.jpg",
            "pixark"
        ),
        (
            "Added Unturned Server",
            "Band together or betray your friends in a massive, blocky post-apocalyptic sandbox. Forage for food, scavenge military gear, craft impenetrable bases, and modify vehicles to survive both the undead hordes and ruthless rival players.",
            "https://cdn.cloudflare.steamstatic.com/steam/apps/1110390/header.jpg",
            "unturned"
        ),
        (
            "American Truck Simulator Server",
            "Official ATS dedicated server via SteamCMD. Needs server_packages files exported from your game client (see SCS documentation).",
            "https://cdn.cloudflare.steamstatic.com/steam/apps/270880/header.jpg",
            "ats"
        ),
        (
            "Euro Truck Simulator 2 Server",
            "Official ETS2 dedicated server via SteamCMD. Needs server_packages files exported from your game client (see SCS documentation).",
            "https://cdn.cloudflare.steamstatic.com/steam/apps/227300/header.jpg",
            "ets2"
        ),
        (
            "BeamMP Server",
            "Official BeamMP multiplayer server for BeamNG.drive. An AuthKey from keymaster.beammp.com is required in ServerConfig.toml.",
            "https://cdn.cloudflare.steamstatic.com/steam/apps/284160/header.jpg",
            "beammp"
        ),
        (
            "FiveM Server",
            "Cfx.re FXServer for GTA V multiplayer. txAdmin's setup wizard handles the licence key on first start — visit http://127.0.0.1:40120 after booting.",
            "https://cdn.cloudflare.steamstatic.com/steam/apps/271590/header.jpg",
            "fivem"
        ),
        (
            "RedM Server",
            "Cfx.re FXServer for Red Dead Redemption 2 multiplayer. Uses the same official artifacts as FiveM with the RDR3 game profile.",
            "https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/header.jpg",
            "redm"
        ),
    };

    /// <summary>Port of <c>BuiltInNews.All() -&gt; List&lt;NewsItem&gt;</c>.</summary>
    public static List<NewsItem> All() => Entries
        .Select(e => new NewsItem
        {
            Tag = "Added",
            Title = e.Title,
            Summary = e.Summary,
            Date = Released,
            Image = e.Image,
            GameId = e.GameId,
            Link = null,
        })
        .ToList();
}
