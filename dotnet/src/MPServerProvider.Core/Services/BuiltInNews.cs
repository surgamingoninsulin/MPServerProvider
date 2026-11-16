using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public static class BuiltInNews
{
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

    private static readonly DateTime AddedGamesReleased = new(2026, 11, 16, 0, 0, 0, DateTimeKind.Utc);

    public static List<NewsItem> All()
    {
        var news = Entries
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

        // Same date for the whole batch; a slightly earlier time per card keeps the Library order.
        var added = BuiltInGames.AddedGames();
        for (var i = 0; i < added.Count; i++)
        {
            var game = added[i];
            news.Add(new NewsItem
            {
                Tag = "Added",
                Title = $"Added {game.Name}",
                Summary = Summarize(game),
                Date = AddedGamesReleased.AddMinutes(-i),
                Image = game.Artwork.Cover,
                GameId = game.Id,
                Link = null,
            });
        }

        return news;
    }

    internal static string Summarize(GameDefinition game)
    {
        var about = game.Details?.About?.Split("\n\n", 2)[0].Trim();
        if (string.IsNullOrWhiteSpace(about))
        {
            return game.Description;
        }

        var sentences = about.Split(". ");
        var summary = sentences[0].TrimEnd('.') + ".";
        for (var i = 1; i < sentences.Length && summary.Length + sentences[i].Length < 260; i++)
        {
            summary += " " + sentences[i].TrimEnd('.') + ".";
        }

        return summary;
    }
}
