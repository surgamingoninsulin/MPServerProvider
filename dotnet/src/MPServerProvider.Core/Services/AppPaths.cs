namespace MPServerProvider.Core.Services;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.AppPaths</c> (decompile token 0x02000004).
/// Central path resolution: everything else in <c>Services/</c> resolves its on-disk locations
/// through this type rather than hardcoding paths.
/// <para>
/// <see cref="Resolve"/> (port of <c>Resolve()</c>, token 0x06000014) walks up from the
/// executable's directory looking for a <c>*.sln</c>/<c>*.slnx</c> file (i.e. "am I running from
/// inside a dev checkout?"); if found, <see cref="DataRoot"/> is <c>&lt;that directory&gt;/data</c>,
/// otherwise it falls back to <c>&lt;exe dir&gt;/data</c>. An <c>MPSM_DATA</c> environment
/// variable, if set and non-blank, always wins over both (confirmed by the decompiled
/// <c>Resolve()</c> body's <c>"MPSM_DATA"</c>/<c>"*.sln"</c>/<c>"*.slnx"</c>/<c>"data"</c> string
/// literals and its <c>Environment.GetEnvironmentVariable</c>/<c>DirectoryInfo.EnumerateFiles</c>
/// call chain).
/// </para>
/// <para>
/// The <c>.ctor(string)</c> (token 0x06000012) takes an optional servers-dir override (used by
/// <see cref="SettingsService"/>'s <c>InstallRoot</c> setting via
/// <see cref="SettingsService.SetInstallRoot"/>) and eagerly creates every subdirectory it
/// exposes, matching the decompiled constructor's <c>Directory.CreateDirectory</c> call chain
/// over <see cref="GamesDir"/>, <see cref="GameArtOverrideDir"/>, <see cref="ServersDir"/>,
/// <see cref="ModsDir"/>, <see cref="ArtworkCacheDir"/>, <see cref="DownloadCacheDir"/>,
/// <see cref="ToolsDir"/> and <see cref="WorkshopDir"/>.
/// </para>
/// </summary>
public class AppPaths
{
    private readonly object _lock = new();
    private readonly List<string> _knownServersDirs = new();
    private string _serversDir;

    /// <summary>Port of <c>get_DataRoot()</c>.</summary>
    public string DataRoot { get; }

    /// <summary>
    /// Test/internal helper: builds an <see cref="AppPaths"/> rooted at an explicit
    /// <paramref name="dataRoot"/>, bypassing <see cref="Resolve"/> (and so the <c>MPSM_DATA</c>
    /// environment variable) entirely. Mirrors the already-verified Rust port's
    /// <c>AppPaths::for_root</c> test helper, used by this project's other service tests so they
    /// don't need to mutate process-global environment state.
    /// </summary>
    public static AppPaths ForRoot(string dataRoot, string? serversDirOverride = null)
        => new(dataRoot, serversDirOverride);

    /// <summary>Port of <c>.ctor(string serversDirOverride)</c>.</summary>
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

    /// <summary>Port of <c>get_ProvidersDir()</c>: <c>{DataRoot}/providers</c>.</summary>
    public string ProvidersDir => Path.Combine(DataRoot, "providers");

    /// <summary>Port of <c>get_GamesDir()</c>: <c>{ProvidersDir}/json</c>.</summary>
    public string GamesDir => Path.Combine(ProvidersDir, "json");

    /// <summary>Port of <c>get_GameArtOverrideDir()</c>: <c>{ProvidersDir}/art</c>.</summary>
    public string GameArtOverrideDir => Path.Combine(ProvidersDir, "art");

    /// <summary>
    /// Port of <c>get_LegacyGamesDir()</c>: <c>{DataRoot}/games</c> (pre-manifest-v1 layout).
    /// </summary>
    public string LegacyGamesDir => Path.Combine(DataRoot, "games");

    /// <summary>Port of <c>get_ServersDir()</c>.</summary>
    public string ServersDir
    {
        get { lock (_lock) { return _serversDir; } }
    }

    /// <summary>
    /// Port of <c>OverrideServersDir(string)</c>: repoints <see cref="ServersDir"/> and ensures it
    /// exists.
    /// </summary>
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

    /// <summary>
    /// Every folder that may contain installed servers: the current <see cref="ServersDir"/>,
    /// the default <c>{DataRoot}/servers</c>, and every previous install location remembered by
    /// <see cref="OverrideServersDir"/> (persisted in <see cref="KnownServersDirsFile"/>).
    /// Distinct, current location first.
    /// </summary>
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

    /// <summary><c>{DataRoot}/known_server_dirs.json</c>: index of every servers folder ever used.</summary>
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

    /// <summary>Case-insensitive, trailing-separator-insensitive full-path comparison.</summary>
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

    /// <summary>
    /// <c>true</c> when <paramref name="path"/> is strictly inside (never equal to)
    /// <paramref name="baseDir"/> after full-path normalization, so <c>..</c> segments, a
    /// relative <c>"."</c> or a UNC path can never pass.
    /// </summary>
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

    /// <summary>Port of <c>get_ModsDir()</c>: <c>{DataRoot}/mods</c>.</summary>
    public string ModsDir => Path.Combine(DataRoot, "mods");

    /// <summary>Port of <c>get_CacheDir()</c>: <c>{DataRoot}/cache</c>.</summary>
    public string CacheDir => Path.Combine(DataRoot, "cache");

    /// <summary>Port of <c>get_ArtworkCacheDir()</c>: <c>{CacheDir}/artwork</c>.</summary>
    public string ArtworkCacheDir => Path.Combine(CacheDir, "artwork");

    /// <summary>Port of <c>get_DownloadCacheDir()</c>: <c>{CacheDir}/downloads</c>.</summary>
    public string DownloadCacheDir => Path.Combine(CacheDir, "downloads");

    /// <summary>Port of <c>get_ToolsDir()</c>: <c>{DataRoot}/tools</c>.</summary>
    public string ToolsDir => Path.Combine(DataRoot, "tools");

    /// <summary>Port of <c>get_WorkshopDir()</c>: <c>{CacheDir}/workshop</c>.</summary>
    public string WorkshopDir => Path.Combine(CacheDir, "workshop");

    /// <summary>Port of <c>get_SettingsFile()</c>: <c>{DataRoot}/settings.json</c>.</summary>
    public string SettingsFile => Path.Combine(DataRoot, "settings.json");

    private static readonly Lazy<string> ResolvedDataRoot = new(Resolve);

    /// <summary>
    /// The data root a default-constructed <see cref="AppPaths"/> uses (memoized). Exposed so
    /// code that isn't handed an <see cref="AppPaths"/> instance (the provider-local
    /// <c>DownloadHelper.ToolsDir()</c>) resolves exactly the same folders instead of a
    /// divergent copy of the rules (which used to download SteamCMD/Java twice).
    /// </summary>
    internal static string DefaultDataRoot => ResolvedDataRoot.Value;

    /// <summary>
    /// Port of <c>Resolve()</c> (token 0x06000014).
    /// <para>
    /// SECURITY: the "walk up looking for a <c>*.sln</c>" dev-checkout heuristic is only honoured
    /// in DEBUG builds. In a release install any writable ancestor folder containing a stray
    /// <c>.sln</c> would otherwise redirect <see cref="DataRoot"/> — and with it the game
    /// definitions whose executables the (often elevated) app launches — to a folder a
    /// non-admin user controls.
    /// </para>
    /// </summary>
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
