namespace MPServerProvider.Core.Services;

public class AppPaths
{
    private readonly object _lock = new();
    private readonly List<string> _knownServersDirs = new();
    private string _serversDir;

    public string DataRoot { get; }

    public static AppPaths ForRoot(string dataRoot, string? serversDirOverride = null)
        => new(dataRoot, serversDirOverride);

    public AppPaths(string? serversDirOverride = null)
        : this(Resolve(), serversDirOverride)
    {
    }

    private AppPaths(string dataRoot, string? serversDirOverride)
    {
        DataRoot = dataRoot;
        _serversDir = string.IsNullOrWhiteSpace(serversDirOverride)
            ? Path.Combine(DataRoot, "servers")
            : serversDirOverride!;

        // Best-effort: a read-only data folder (e.g. installed under Program Files and run as a
        // standard user) must not stop the app from starting; the individual writers report
        // their own failures later with a clear message instead.
        foreach (var dir in new[] { GamesDir, GameArtOverrideDir, ServersDir, ModsDir, ArtworkCacheDir, DownloadCacheDir, ToolsDir, WorkshopDir })
        {
            TryCreateDirectory(dir);
        }

        LoadKnownServersDirs();
    }

    private static void TryCreateDirectory(string dir)
    {
        try
        {
            Directory.CreateDirectory(dir);
        }
        catch
        {
            // See the constructor comment.
        }
    }

    public string ProvidersDir => Path.Combine(DataRoot, "providers");

    public string GamesDir => Path.Combine(ProvidersDir, "json");

    public string GameArtOverrideDir => Path.Combine(ProvidersDir, "art");

    public string LegacyGamesDir => Path.Combine(DataRoot, "games");

    public string ServersDir
    {
        get { lock (_lock) { return _serversDir; } }
    }

    public void OverrideServersDir(string dir)
    {
        Directory.CreateDirectory(dir);
        lock (_lock)
        {
            // Changing the install location only affects where NEW servers go: remember the
            // previous location (and the new one) so servers already installed there stay
            // visible/manageable instead of silently disappearing from the list.
            RememberServersDirLocked(_serversDir);
            RememberServersDirLocked(dir);
            _serversDir = dir;
        }

        SaveKnownServersDirs();
    }

    public IReadOnlyList<string> AllServersDirs
    {
        get
        {
            lock (_lock)
            {
                var result = new List<string> { _serversDir };
                foreach (var dir in _knownServersDirs.Prepend(Path.Combine(DataRoot, "servers")))
                {
                    if (!result.Any(r => SamePath(r, dir)))
                    {
                        result.Add(dir);
                    }
                }

                return result;
            }
        }
    }

    public string KnownServersDirsFile => Path.Combine(DataRoot, "known_server_dirs.json");

    private void RememberServersDirLocked(string dir)
    {
        if (string.IsNullOrWhiteSpace(dir) || _knownServersDirs.Any(d => SamePath(d, dir)))
        {
            return;
        }

        _knownServersDirs.Add(dir);
    }

    private void LoadKnownServersDirs()
    {
        try
        {
            var loaded = Json.Json.ReadFile<List<string>>(KnownServersDirsFile);
            if (loaded is null)
            {
                return;
            }

            lock (_lock)
            {
                foreach (var dir in loaded)
                {
                    RememberServersDirLocked(dir);
                }
            }
        }
        catch
        {
            // A corrupt index only costs the extra scan locations; never fatal.
        }
    }

    private void SaveKnownServersDirs()
    {
        try
        {
            List<string> snapshot;
            lock (_lock)
            {
                snapshot = new List<string>(_knownServersDirs);
            }

            Json.Json.WriteFile(KnownServersDirsFile, snapshot);
        }
        catch
        {
            // Best-effort persistence; the in-memory list still applies for this session.
        }
    }

    public static bool SamePath(string a, string b)
    {
        try
        {
            return string.Equals(
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(a)),
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(b)),
                StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static bool IsStrictlyInside(string path, string baseDir)
    {
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(baseDir)
            || !Path.IsPathFullyQualified(path) || !Path.IsPathFullyQualified(baseDir))
        {
            return false;
        }

        try
        {
            var full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path)) + Path.DirectorySeparatorChar;
            var root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(baseDir)) + Path.DirectorySeparatorChar;
            return full.StartsWith(root, StringComparison.OrdinalIgnoreCase)
                && !full.Equals(root, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public string ModsDir => Path.Combine(DataRoot, "mods");

    public string CacheDir => Path.Combine(DataRoot, "cache");

    public string ArtworkCacheDir => Path.Combine(CacheDir, "artwork");

    public string DownloadCacheDir => Path.Combine(CacheDir, "downloads");

    public string ToolsDir => Path.Combine(DataRoot, "tools");

    public string WorkshopDir => Path.Combine(CacheDir, "workshop");

    public string SettingsFile => Path.Combine(DataRoot, "settings.json");

    private static readonly Lazy<string> ResolvedDataRoot = new(Resolve);

    internal static string DefaultDataRoot => ResolvedDataRoot.Value;

    private static string Resolve()
    {
        var envValue = Environment.GetEnvironmentVariable("MPSM_DATA");
        if (!string.IsNullOrWhiteSpace(envValue))
        {
            try
            {
                return Path.GetFullPath(envValue);
            }
            catch
            {
                return envValue;
            }
        }

        var baseDir = AppContext.BaseDirectory;
#if DEBUG
        var dir = new DirectoryInfo(baseDir);
        while (dir is not null)
        {
            if (HasSolutionFile(dir))
            {
                return Path.Combine(dir.FullName, "data");
            }

            dir = dir.Parent;
        }
#endif

        return Path.Combine(baseDir, "data");
    }

#if DEBUG
    private static bool HasSolutionFile(DirectoryInfo dir)
    {
        try
        {
            return dir.EnumerateFiles().Any(f =>
                f.Name.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
                || f.Name.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }
#endif
}
