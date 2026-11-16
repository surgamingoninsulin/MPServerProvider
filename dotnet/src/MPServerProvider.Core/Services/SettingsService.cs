using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public class SettingsService
{
    private readonly AppPaths _paths;
    private readonly object _lock = new();
    private readonly object _writeLock = new();
    private AppSettings _settings = new();

    public SettingsService(AppPaths paths)
    {
        _paths = paths;
        Load();
    }

    public AppSettings Settings
    {
        get { lock (_lock) { return _settings; } }
    }

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

    public bool ManageStartupEntry { get; set; } = true;

    public void Update(Action<AppSettings> mutate)
    {
        lock (_lock)
        {
            mutate(_settings);
        }

        Save();
    }

    public void SetInstallRoot(string installRoot)
    {
        _paths.OverrideServersDir(installRoot);

        lock (_lock)
        {
            _settings.InstallRoot = installRoot;
        }

        Save();
    }

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
            // Best-effort: registry access failures are silent.
        }
    }
}
