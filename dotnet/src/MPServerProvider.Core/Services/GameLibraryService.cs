using MPServerProvider.Core.Json;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.GameLibraryService</c> (decompile token
/// 0x0200000d). Merges the embedded <see cref="BuiltInGames"/> catalog with every manifest under
/// <c>{DataRoot}/providers/json/*.json</c> (built-ins are seeded there on first run so they're
/// always editable/overridable on disk, exactly like the decompiled
/// <c>SeedBuiltIns</c>/<c>Reload</c> pair).
/// </summary>
public class GameLibraryService
{
    private readonly AppPaths _paths;
    private readonly object _lock = new();
    private List<GameDefinition> _games = new();

    /// <summary>
    /// Artwork URLs from older app releases that should be overwritten with the current built-in
    /// artwork on reload, even for a manifest that already exists on disk. Port of the
    /// <c>StaleArtworkUrls</c> static <c>HashSet&lt;string&gt;</c> (<c>StringComparer.OrdinalIgnoreCase</c>),
    /// seeded from the <c>.cctor()</c>'s three string literals (the cfx/FiveM and OpenRCT2 logo
    /// URLs that got replaced).
    /// </summary>
    private static readonly HashSet<string> StaleArtworkUrls = new(StringComparer.OrdinalIgnoreCase)
    {
        "https://forum.cfx.re/uploads/default/original/4X/7/5/e/75ef9fcabc1abea8fce0ebd0236a4132710fcb2e.png",
        "https://openrct2.org/uploads/images/logo.png",
        "https://rsg-fivem-forum-assets.s3.dualstack.us-west-2.amazonaws.com/original/5X/e/d/8/1/ed819c68cedac6e7bb496bb7b8423f7949e51c79.png",
        // OpenRCT2's 64x64 app icon, a later stand-in cover that rendered blurry on the wide card.
        "https://raw.githubusercontent.com/OpenRCT2/OpenRCT2/develop/resources/logo/icon_x64.png",
        // Interim OpenRCT2 cover (a plain screenshot), replaced by the bundled banner.
        "https://docs.openrct2.io/en/latest/_images/screenshot.png",
    };

    /// <summary>
    /// Launch definitions (executable + arguments) that earlier releases seeded for a built-in
    /// and that are now known to be broken. An on-disk copy whose runtime still matches one of
    /// these EXACTLY (i.e. the user never edited it) gets the current built-in runtime, network
    /// defaults and Game Detail content (which describes that launch), so the corrected
    /// definition actually reaches users who already have a seeded copy.
    /// </summary>
    internal static readonly IReadOnlyDictionary<string, (string Executable, string Arguments)[]> StaleLaunchDefinitions =
        new Dictionary<string, (string, string)[]>
        {
            // `host {server_file}` substituted the server's own executable path as the park, and
            // openrct2.exe is the GUI-subsystem build (no usable console).
            ["openrct2"] = new[] { ("{server_root}\\openrct2.exe", "host {server_file} --port {server_port} --headless") },
            // Missing the CubePort (terrain; players saw no terrain), -NoBattlEye and
            // -CULTUREFORCOOKING=en that Snail's server guide lists as required.
            ["pixark"] = new[]
            {
                ("{server_root}\\ShooterGame\\Binaries\\Win64\\PixARKServer.exe",
                 "CubeWorld_Light?listen?SessionName={server_name}?Port={server_port}?QueryPort=27015 -server -log"),
            },
        };

    /// <summary>
    /// Install URLs earlier releases seeded for a built-in that no longer resolve (e.g. a
    /// GitHub <c>releases/latest/download/&lt;name&gt;</c> URL whose asset name now carries the
    /// version). A copy still pointing at one gets the current built-in install spec.
    /// </summary>
    internal static readonly IReadOnlyDictionary<string, string[]> StaleInstallUrls = new Dictionary<string, string[]>
    {
        ["openrct2"] = new[] { "https://github.com/OpenRCT2/OpenRCT2/releases/latest/download/OpenRCT2-windows-x64.zip" },
    };

    /// <summary>Port of <c>.ctor(AppPaths)</c>.</summary>
    public GameLibraryService(AppPaths paths)
    {
        _paths = paths;
    }

    /// <summary>Port of <c>get_Games()</c>.</summary>
    public List<GameDefinition> Games
    {
        get { lock (_lock) { return new List<GameDefinition>(_games); } }
    }

    /// <summary>Port of <c>Find(string id) -&gt; GameDefinition?</c>.</summary>
    public GameDefinition? Find(string id)
    {
        lock (_lock)
        {
            return _games.FirstOrDefault(g => g.Id == id);
        }
    }

    /// <summary>
    /// Port of <c>AllGenres() -&gt; List&lt;string&gt;</c>: every distinct genre across the loaded
    /// catalog, alphabetically sorted.
    /// </summary>
    public List<string> AllGenres()
    {
        List<GameDefinition> snapshot;
        lock (_lock)
        {
            snapshot = _games;
        }

        var genres = snapshot.SelectMany(g => g.Genres).Distinct().ToList();
        genres.Sort(StringComparer.Ordinal);
        return genres;
    }

    /// <summary>
    /// Port of <c>Reload()</c>: migrates any legacy on-disk layout, seeds built-ins, then
    /// (re)loads every manifest under <see cref="AppPaths.GamesDir"/>.
    /// </summary>
    public void Reload()
    {
        MigrateLegacyLayout();
        SeedBuiltIns();

        var builtInIds = new HashSet<string>(BuiltInGames.All().Select(g => g.Id));

        var gamesDir = _paths.GamesDir;
        var loaded = new List<GameDefinition>();
        if (Directory.Exists(gamesDir))
        {
            var files = Directory.GetFiles(gamesDir, "*.json").OrderBy(f => Path.GetFileName(f), StringComparer.Ordinal);
            foreach (var path in files)
            {
                GameDefinition? game;
                try
                {
                    var text = File.ReadAllText(path);
                    game = Json.Json.Deserialize<GameDefinition>(text);
                }
                catch
                {
                    continue;
                }

                if (game is null || string.IsNullOrWhiteSpace(game.Id))
                {
                    continue;
                }

                game.SourceFile = path;
                game.IsBuiltIn = builtInIds.Contains(game.Id);
                loaded.Add(game);
            }
        }

        lock (_lock)
        {
            _games = loaded;
        }
    }

    /// <summary>
    /// Port of <c>MigrateLegacyLayout()</c>: moves any <c>{DataRoot}/games/*.json</c> (and
    /// <c>{DataRoot}/games/art</c>) from the pre-manifest-v1 layout into
    /// <see cref="AppPaths.GamesDir"/>/<see cref="AppPaths.GameArtOverrideDir"/>, then removes the
    /// now-empty legacy directory.
    /// </summary>
    private void MigrateLegacyLayout()
    {
        var legacyDir = _paths.LegacyGamesDir;
        if (!Directory.Exists(legacyDir))
        {
            return;
        }

        var gamesDir = _paths.GamesDir;
        Directory.CreateDirectory(gamesDir);
        foreach (var path in Directory.GetFiles(legacyDir, "*.json"))
        {
            var dest = Path.Combine(gamesDir, Path.GetFileName(path));
            try
            {
                if (File.Exists(dest))
                {
                    File.Delete(path);
                }
                else
                {
                    File.Move(path, dest);
                }
            }
            catch
            {
                // Best-effort migration.
            }
        }

        var legacyArt = Path.Combine(legacyDir, "art");
        if (Directory.Exists(legacyArt))
        {
            var artDir = _paths.GameArtOverrideDir;
            Directory.CreateDirectory(artDir);
            foreach (var path in Directory.GetFiles(legacyArt))
            {
                var dest = Path.Combine(artDir, Path.GetFileName(path));
                try
                {
                    if (!File.Exists(dest))
                    {
                        File.Move(path, dest);
                    }
                }
                catch
                {
                    // Best-effort migration.
                }
            }

            try
            {
                Directory.Delete(legacyArt);
            }
            catch
            {
                // Non-empty or in use; leave it.
            }
        }

        try
        {
            if (!Directory.EnumerateFileSystemEntries(legacyDir).Any())
            {
                Directory.Delete(legacyDir);
            }
        }
        catch
        {
            // Best-effort cleanup.
        }
    }

    /// <summary>
    /// Port of <c>SeedBuiltIns()</c>: writes a <c>{id}.json</c> for every built-in game that
    /// doesn't already exist on disk; for ones that do, re-applies a few known one-way fixups
    /// (stale artwork URL replacement plus <see cref="ApplyBuiltInFixups"/>) without otherwise
    /// touching user edits.
    /// <para>
    /// The original also appended <c>+exec server.cfg</c> to every <c>cfx</c> definition's
    /// arguments here, while <c>ServerRepository.MigrateCfxToTxAdmin</c> strips that same flag
    /// from installed servers (txAdmin mode manages server.cfg itself). The two contradicted each
    /// other, so the seeding side now strips it too — one source of truth.
    /// </para>
    /// </summary>
    private void SeedBuiltIns()
    {
        var gamesDir = _paths.GamesDir;
        Directory.CreateDirectory(gamesDir);

        foreach (var builtIn in BuiltInGames.All())
        {
            var path = Path.Combine(gamesDir, $"{builtIn.Id}.json");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, Json.Json.Serialize(builtIn));
                continue;
            }

            GameDefinition existing;
            try
            {
                existing = Json.Json.Deserialize<GameDefinition>(File.ReadAllText(path))
                    ?? throw new InvalidOperationException();
            }
            catch
            {
                continue;
            }

            var changed = false;

            if (existing.Artwork.Cover is { } cover && StaleArtworkUrls.Contains(cover))
            {
                existing.Artwork = builtIn.Artwork;
                changed = true;
            }

            changed |= ApplyBuiltInFixups(existing, builtIn);

            if (changed)
            {
                File.WriteAllText(path, Json.Json.Serialize(existing));
            }
        }
    }

    /// <summary>
    /// Targeted, idempotent repairs applied to an existing on-disk copy of a built-in manifest.
    /// Each one only fills in data that is missing or provably stale, so any other user edit is
    /// left alone. Returns <c>true</c> if <paramref name="existing"/> was modified.
    /// </summary>
    internal static bool ApplyBuiltInFixups(GameDefinition existing, GameDefinition builtIn)
    {
        var changed = false;

        // Game Detail page content was added after many users already had their built-in
        // copies on disk; without this they'd never see it.
        if (existing.Details is null && builtIn.Details is not null)
        {
            existing.Details = builtIn.Details;
            changed = true;
        }

        // An empty description, or the reconstruction note that used to ship as OpenRCT2's
        // card text, gets the current built-in description.
        if ((string.IsNullOrWhiteSpace(existing.Description) || existing.Description.StartsWith("INFERRED:", StringComparison.Ordinal))
            && !string.IsNullOrWhiteSpace(builtIn.Description))
        {
            existing.Description = builtIn.Description;
            changed = true;
        }

        if (existing.Install.Provider == "cfx" && existing.Runtime.Arguments.Contains("+exec server.cfg"))
        {
            existing.Runtime.Arguments = existing.Runtime.Arguments.Replace("+exec server.cfg", string.Empty).Trim();
            changed = true;
        }

        // RedM shares FiveM's FXServer artifact; without the rdr3 profile it boots as GTA V.
        if (existing.Id == "redm" && !existing.Runtime.Arguments.Contains("gamename"))
        {
            existing.Runtime.Arguments = string.IsNullOrEmpty(existing.Runtime.Arguments)
                ? builtIn.Runtime.Arguments
                : $"{builtIn.Runtime.Arguments} {existing.Runtime.Arguments}";
            changed = true;
        }

        // 7 Days to Die's real default ServerPort is 26900; older copies shipped Source's 27015.
        if (existing.Id == "7-days-to-die-dedicated-server" && existing.Network.DefaultPort == 27015
            && builtIn.Network.DefaultPort != 27015)
        {
            existing.Network.DefaultPort = builtIn.Network.DefaultPort;
            changed = true;
        }

        if (StaleInstallUrls.TryGetValue(existing.Id, out var staleUrls)
            && existing.Install.Url is { } url && staleUrls.Contains(url, StringComparer.OrdinalIgnoreCase)
            && string.IsNullOrWhiteSpace(existing.Install.GithubRepo))
        {
            existing.Install = Json.Json.Deserialize<InstallSpec>(Json.Json.Serialize(builtIn.Install))!;
            changed = true;
        }

        if (StaleLaunchDefinitions.TryGetValue(existing.Id, out var staleLaunches)
            && staleLaunches.Any(s => existing.Runtime.Executable == s.Executable && existing.Runtime.Arguments == s.Arguments))
        {
            existing.Runtime = Json.Json.Deserialize<RuntimeSpec>(Json.Json.Serialize(builtIn.Runtime))!;
            existing.Network = Json.Json.Deserialize<NetworkSpec>(Json.Json.Serialize(builtIn.Network))!;
            existing.Details = builtIn.Details;
            changed = true;
        }

        // Minecraft Java's generic default used to be Java 21; 26.1+ needs Java 25. It's only the
        // fallback for servers whose install didn't record their own JavaMajor, but an old 21
        // there would still launch new versions on the wrong runtime.
        if (existing.Id == "minecraft-java" && existing.Runtime.Requires?.Java == 21
            && builtIn.Runtime.Requires?.Java is { } builtInJava && builtInJava > 21)
        {
            existing.Runtime.Requires.Java = builtInJava;
            changed = true;
        }

        return changed;
    }
}
