using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.SettingsService</c> (decompile token
/// 0x02000016). Loads/saves <see cref="AppSettings"/> to/from <c>{DataRoot}/settings.json</c>.
/// </summary>
public class SettingsService
{
    private readonly AppPaths _paths;
    private readonly object _lock = new();
    private readonly object _writeLock = new();
    private AppSettings _settings = new();

    /// <summary>Port of <c>.ctor(AppPaths)</c>: constructs with defaults, then immediately <see cref="Load"/>s.</summary>
    public SettingsService(AppPaths paths)
    {
        _paths = paths;
        Load();
    }

    /// <summary>Port of <c>get_Settings()</c>.</summary>
    public AppSettings Settings
    {
        get { lock (_lock) { return _settings; } }
    }

    /// <summary>
    /// Port of <c>Load()</c>: reads <c>settings.json</c> if present (falling back to
    /// <c>new AppSettings()</c> otherwise), then applies <c>InstallRoot</c> (if set) to
    /// <see cref="AppPaths.OverrideServersDir"/>.
    /// </summary>
    public void Load()
    {
        var path = _paths.SettingsFile;
        AppSettings loaded;
        try
        {
            loaded = Json.Json.ReadFile<AppSettings>(path) ?? new AppSettings();
        }
        catch (Exception ex) when (ex is System.Text.Json.JsonException or NotSupportedException or ArgumentException)
        {
            // A truncated/corrupt settings.json (crash or power loss mid-write on an older build,
            // a bad hand edit) used to throw out of this constructor and stop the app from ever
            // starting again. Keep the broken file for inspection and continue with defaults.
            LoadError = $"settings.json was unreadable and has been reset to defaults (a copy was kept as settings.json.corrupt): {ex.Message}";
            BackUpCorruptFile(path);
            loaded = new AppSettings();
        }
        catch (IOException ex)
        {
            LoadError = $"settings.json could not be read; using defaults: {ex.Message}";
            loaded = new AppSettings();
        }
        catch (UnauthorizedAccessException ex)
        {
            LoadError = $"settings.json could not be read; using defaults: {ex.Message}";
            loaded = new AppSettings();
        }

        if (!string.IsNullOrWhiteSpace(loaded.InstallRoot))
        {
            try
            {
                _paths.OverrideServersDir(loaded.InstallRoot!);
            }
            catch (Exception ex)
            {
                // e.g. the install drive was removed: fall back to the default servers folder
                // rather than failing startup.
                LoadError ??= $"Install location '{loaded.InstallRoot}' is unavailable ({ex.Message}); using the default servers folder.";
            }
        }

        lock (_lock)
        {
            _settings = loaded;
        }
    }

    /// <summary>
    /// Set when the last <see cref="Load"/> had to fall back to defaults (corrupt/unreadable
    /// <c>settings.json</c> or an unavailable install location); <c>null</c> otherwise. The UI
    /// can surface it once instead of the app failing to start.
    /// </summary>
    public string? LoadError { get; private set; }

    private static void BackUpCorruptFile(string path)
    {
        try
        {
            File.Copy(path, path + ".corrupt", overwrite: true);
        }
        catch
        {
            // Best-effort: the defaults still apply even if the backup copy fails.
        }
    }

    /// <summary>Port of <c>Save()</c>: writes <c>settings.json</c> (atomically, see
    /// <see cref="Json.Json.WriteAllTextAtomic"/>), then <see cref="ApplyStartOnBoot"/>.</summary>
    public void Save()
    {
        string json;
        lock (_lock)
        {
            // Serialize under the lock so a concurrent Update() can't mutate the object mid-way.
            json = Json.Json.Serialize(_settings);
        }

        lock (_writeLock)
        {
            Json.Json.WriteAllTextAtomic(_paths.SettingsFile, json);
        }

        if (ManageStartupEntry)
        {
            ApplyStartOnBoot();
        }
    }

    /// <summary>
    /// Whether <see cref="Save"/> also writes the Windows "run at sign-in" registry entry. Tests
    /// turn this off so they never touch the developer's real <c>HKCU\…\Run</c> key.
    /// </summary>
    public bool ManageStartupEntry { get; set; } = true;

    /// <summary>
    /// Additive helper (not a direct decompile port of a single method, but needed by every
    /// settings-mutating caller — <c>OnStartOnBootChanged</c>, <c>OnBehaviourChanged</c>,
    /// <c>OnAddRule</c>/<c>OnDeleteRule</c> in the decompiled <c>SettingsPage</c> all
    /// read-modify-write <c>AppSettings</c> then call <c>Save()</c>, exactly what this does in
    /// one step): mutates the in-memory settings via <paramref name="mutate"/>, then
    /// <see cref="Save"/>s.
    /// </summary>
    public void Update(Action<AppSettings> mutate)
    {
        lock (_lock)
        {
            mutate(_settings);
        }

        Save();
    }

    /// <summary>Port of <c>SetInstallRoot(string)</c>.</summary>
    /// <remarks>
    /// The install root only decides where NEW servers go: <see cref="AppPaths.OverrideServersDir"/>
    /// remembers the previous location so existing servers stay listed. The folder is created
    /// (and validated) before the setting is changed, so a failure leaves everything as it was.
    /// </remarks>
    public void SetInstallRoot(string installRoot)
    {
        _paths.OverrideServersDir(installRoot);

        lock (_lock)
        {
            _settings.InstallRoot = installRoot;
        }

        Save();
    }

    /// <summary>
    /// Port of <c>ApplyStartOnBoot()</c>: writes/removes a
    /// <c>HKCU\Software\Microsoft\Windows\CurrentVersion\Run\MPServerProvider</c> value so the
    /// app does/doesn't launch at Windows sign-in, matching <c>AppSettings.StartOnBoot</c>/
    /// <c>StartMinimizedToTray</c>.
    /// </summary>
    public void ApplyStartOnBoot()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        bool startOnBoot;
        bool minimized;
        lock (_lock)
        {
            startOnBoot = _settings.StartOnBoot;
            minimized = _settings.StartMinimizedToTray;
        }

        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true);
            if (key is null)
            {
                return;
            }

            if (startOnBoot)
            {
                var exe = Environment.ProcessPath ?? string.Empty;
                var value = $"\"{exe}\"";
                if (minimized)
                {
                    value += " --minimized";
                }

                key.SetValue("MPServerProvider", value);
            }
            else
            {
                key.DeleteValue("MPServerProvider", throwOnMissingValue: false);
            }
        }
        catch
        {
            // Best-effort; matches the original's silent-failure registry access.
        }
    }
}
