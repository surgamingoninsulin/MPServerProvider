using MPServerProvider.Core.Json;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public class GameLibraryService
{
    private readonly AppPaths _paths;
    private readonly object _lock = new();
    private List<GameDefinition> _games = new();

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

    public GameLibraryService(AppPaths paths)
    {
        _paths = paths;
    }

    public List<GameDefinition> Games
    {
        get { lock (_lock) { return new List<GameDefinition>(_games); } }
    }

    public GameDefinition? Find(string id)
    {
        lock (_lock)
        {
            return _games.FirstOrDefault(g => g.Id == id);
        }
    }

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

    public string BuiltInBackupDir => Path.Combine(_paths.ProvidersDir, "json-builtin-backup");

    public void Reload()
    {
        MigrateLegacyLayout();

        var loaded = new List<GameDefinition>();
        var builtInIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var builtIn in BuiltInGames.All())
        {
            builtIn.IsBuiltIn = true;
            builtIn.SourceFile = null;
            builtInIds.Add(builtIn.Id);
            loaded.Add(builtIn);
        }

        var gamesDir = _paths.GamesDir;
        if (Directory.Exists(gamesDir))
        {
            var files = Directory.GetFiles(gamesDir, "*.json").OrderBy(f => Path.GetFileName(f), StringComparer.Ordinal);
            var customIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var path in files)
            {
                GameDefinition? game;
                try
                {
                    game = Json.Json.Deserialize<GameDefinition>(File.ReadAllText(path));
                }
                catch
                {
                    continue;
                }

                if (game is null || string.IsNullOrWhiteSpace(game.Id))
                {
                    continue;
                }

                if (builtInIds.Contains(game.Id))
                {
                    RetireBuiltInCopy(path);
                    continue;
                }

                if (!customIds.Add(game.Id))
                {
                    continue;
                }

                game.SourceFile = path;
                game.IsBuiltIn = false;
                loaded.Add(game);
            }
        }

        lock (_lock)
        {
            _games = loaded;
        }
    }

    private void RetireBuiltInCopy(string path)
    {
        try
        {
            var backupDir = BuiltInBackupDir;
            Directory.CreateDirectory(backupDir);
            var name = Path.GetFileNameWithoutExtension(path);
            var dest = Path.Combine(backupDir, Path.GetFileName(path));
            for (var i = 2; File.Exists(dest); i++)
            {
                dest = Path.Combine(backupDir, $"{name} ({i}).json");
            }

            File.Move(path, dest);
        }
        catch
        {
            // Best effort: if it can't move, it is still skipped for this load.
        }
    }

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
}
