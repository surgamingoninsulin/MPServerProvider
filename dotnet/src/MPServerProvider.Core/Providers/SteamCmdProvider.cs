using System.Diagnostics;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Native;

namespace MPServerProvider.Core.Providers;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Providers.SteamCmdProvider</c> (decompile token
/// 0x02000026).
/// <para>
/// Note: the original's <c>.ctor</c> also builds a <c>WorkshopService</c> (<c>_workshop</c>
/// field) for Steam Workshop item downloads. <c>WorkshopService</c> lives under
/// <c>MPServerProvider.Core.Services</c>, which is out of scope for this task (a separate task
/// owns <c>Services/</c>), so workshop support is intentionally not ported here — matching the
/// proven Rust port's same omission for the same reason.
/// </para>
/// </summary>
public sealed partial class SteamCmdProvider : IInstallProvider
{
    /// <summary><c>SteamCmdZip</c> field.</summary>
    private const string SteamCmdZipUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";

    /// <summary><c>ExeBlocklist</c> static field: installer/support binaries to skip when
    /// auto-detecting a Steam app's dedicated-server executable.</summary>
    private static readonly string[] ExeBlocklist =
    {
        "steamerrorreporter",
        "steamservice",
        "crashhandler",
        "unitycrashhandler",
        "vcredist",
        "dxsetup",
        "dxwebsetup",
        "uninstall",
        "steamcmd",
        "installscript",
    };

    public string Id => "steamcmd";

    /// <summary>Port of <c>SteamCmdProvider.FindOnPath(string exeName)</c>.</summary>
    private static string? FindOnPath(string exeName)
    {
        var pathVar = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(pathVar))
        {
            return null;
        }

        foreach (var dir in pathVar.Split(Path.PathSeparator))
        {
            if (dir.Length == 0)
            {
                continue;
            }

            var candidate = Path.Combine(dir, exeName);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    /// <summary>
    /// Port of <c>SteamCmdProvider.EnsureSteamCmdAsync(IProgress&lt;InstallProgress&gt;, CancellationToken)</c>:
    /// finds an existing <c>steamcmd.exe</c> (under <c>{ToolsDir}/steamcmd/</c> or on
    /// <c>PATH</c>), or downloads+extracts the official Valve zip.
    /// </summary>
    private static async Task<string> EnsureSteamCmdAsync(IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var dir = Path.Combine(DownloadHelper.ToolsDir(), "steamcmd");
        var exe = Path.Combine(dir, "steamcmd.exe");
        if (File.Exists(exe))
        {
            return exe;
        }

        var onPath = FindOnPath("steamcmd.exe");
        if (onPath is not null)
        {
            return onPath;
        }

        progress.Report(InstallProgress.Indeterminate("Downloading SteamCMD (official Valve tool)"));
        var zipPath = Path.Combine(dir, "steamcmd.zip");
        await DownloadHelper.DownloadFileAsync(SteamCmdZipUrl, zipPath, ct).ConfigureAwait(false);

        progress.Report(InstallProgress.Indeterminate("Extracting SteamCMD…"));
        DownloadHelper.ExtractZip(zipPath, dir);
        try
        {
            File.Delete(zipPath);
        }
        catch
        {
            // Best-effort cleanup.
        }

        return exe;
    }

    /// <summary>Max wall-clock time for one <c>app_update</c> run (multi-GB games on slow links).</summary>
    internal static readonly TimeSpan InstallTimeout = TimeSpan.FromHours(3);

    /// <summary>Max wall-clock time for a metadata-only SteamCMD run.</summary>
    internal static readonly TimeSpan AppInfoTimeout = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Port of <c>SteamCmdProvider.LoginArguments(InstallSpec spec)</c>, display/command-line
    /// form. SECURITY: this never contains the password any more — anything on a process command
    /// line is readable by every local user (<c>Win32_Process.CommandLine</c>). Authenticated
    /// logins with a password go through <see cref="RunscriptLoginLine"/> in a private runscript.
    /// </summary>
    internal static string LoginArguments(InstallSpec spec)
    {
        if (spec.Anonymous || string.IsNullOrWhiteSpace(spec.Username))
        {
            return "+login anonymous";
        }

        return $"+login \"{RequireSafeToken(spec.Username!, "Steam username")}\"";
    }

    /// <summary>
    /// The <c>login</c> line for a SteamCMD runscript when the spec has a stored password, or
    /// <c>null</c> for anonymous / username-only logins.
    /// </summary>
    internal static string? RunscriptLoginLine(InstallSpec spec)
    {
        if (spec.Anonymous || string.IsNullOrWhiteSpace(spec.Username) || string.IsNullOrEmpty(spec.Password))
        {
            return null;
        }

        string password;
        try
        {
            // Port of `SecretProtector.Unprotect` applied to the stored password: the stored
            // value is a base64-encoded DPAPI blob.
            // DPAPI only exists on Windows (also silences CA1416 for this net8.0 library).
            if (!OperatingSystem.IsWindows())
            {
                return null;
            }

            password = SecretProtector.Unprotect(spec.Password);
        }
        catch
        {
            return null;
        }

        if (string.IsNullOrEmpty(password))
        {
            return null;
        }

        return $"login \"{RequireSafeToken(spec.Username!, "Steam username")}\" \"{RequireSafeToken(password, "Steam password")}\"";
    }

    /// <summary>SteamCMD has no escape syntax: refuse values that would break out of quotes.</summary>
    private static string RequireSafeToken(string value, string what)
    {
        if (value.Any(c => c == '"' || char.IsControl(c)))
        {
            throw new InvalidOperationException($"The {what} contains a quote or control character, which SteamCMD can't accept.");
        }

        return value;
    }

    /// <summary>
    /// <c>true</c> when SteamCMD's output shows the app install succeeded. SteamCMD's exit code
    /// alone is unreliable (it can exit non-zero after self-updating even on success, and exit 0
    /// after printing an <c>ERROR!</c>), so the success banner is the primary signal.
    /// </summary>
    internal static bool InstallSucceeded(int exitCode, string output, long appId)
    {
        if (output.Contains($"Success! App '{appId}' fully installed", StringComparison.OrdinalIgnoreCase)
            || output.Contains($"Success! App '{appId}' already up to date", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return exitCode == 0 && !output.Contains("ERROR!", StringComparison.Ordinal)
            && !output.Contains("FAILED", StringComparison.Ordinal);
    }

    /// <summary>Last <c>ERROR!</c>/<c>FAILED</c> line from SteamCMD output, for a useful error message.</summary>
    private static string? LastErrorLine(string output)
        => output.Split('\n')
            .Select(l => l.Trim())
            .LastOrDefault(l => l.Contains("ERROR!", StringComparison.Ordinal) || l.Contains("FAILED", StringComparison.Ordinal));

    /// <summary>
    /// Writes a SteamCMD runscript (holding credentials) into a fresh private temp folder that
    /// only the current user can read, returning the script path. Callers delete the folder.
    /// </summary>
    private static string WritePrivateRunscript(IEnumerable<string> lines)
    {
        var dir = Path.Combine(Path.GetTempPath(), "mpsp-steamcmd-" + Guid.NewGuid().ToString("N"));
        var created = Directory.CreateDirectory(dir);
        if (OperatingSystem.IsWindows())
        {
            var user = System.Security.Principal.WindowsIdentity.GetCurrent().User;
            if (user is not null)
            {
                var security = new System.Security.AccessControl.DirectorySecurity();
                security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
                security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(
                    user,
                    System.Security.AccessControl.FileSystemRights.FullControl,
                    System.Security.AccessControl.InheritanceFlags.ContainerInherit | System.Security.AccessControl.InheritanceFlags.ObjectInherit,
                    System.Security.AccessControl.PropagationFlags.None,
                    System.Security.AccessControl.AccessControlType.Allow));
                created.SetAccessControl(security);
            }
        }

        var script = Path.Combine(dir, "install.txt");
        File.WriteAllLines(script, lines);
        return script;
    }

    private static void TryDeleteDirectory(string dir)
    {
        try
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, recursive: true);
            }
        }
        catch
        {
            // Best-effort; it lives in the user's own temp folder.
        }
    }

    /// <summary>Port of <c>SteamCmdProvider.DefinitionNamesExecutable(string executable)</c>.</summary>
    internal static bool DefinitionNamesExecutable(string executable)
        => !string.IsNullOrWhiteSpace(executable) && executable.Contains("{server_file}");

    /// <summary>Port of <c>SteamCmdProvider.ExeFromDefinition(InstallRequest request)</c>.</summary>
    private static string? ExeFromDefinition(InstallRequest request)
    {
        var executable = request.Game.Runtime.Executable;
        if (DefinitionNamesExecutable(executable))
        {
            return null;
        }

        var resolved = executable.Replace("{server_root}", request.TargetDir).Trim();
        if (resolved.Length == 0)
        {
            return null;
        }

        var path = Path.IsPathRooted(resolved) ? resolved : Path.Combine(request.TargetDir, resolved);
        return File.Exists(path) ? resolved : null;
    }

    /// <summary>
    /// Port of <c>SteamCmdProvider.AutoDetectServerExe(string rootDir)</c>: scans for
    /// <c>*.exe</c> under <paramref name="rootDir"/>, skipping <see cref="ExeBlocklist"/> names
    /// and anything under a <c>_commonredist</c>/<c>redist</c> folder, preferring names
    /// containing "server"/"srcds"/"dedicated", then falling back to the largest <c>.exe</c> file.
    /// </summary>
    internal static string? AutoDetectServerExe(string rootDir)
    {
        var candidates = new List<string>();
        CollectExeFiles(rootDir, candidates);

        bool IsBlocked(string p)
        {
            var pathLower = p.ToLowerInvariant();
            if (pathLower.Contains("_commonredist") || pathLower.Contains("redist"))
            {
                return true;
            }

            var stem = Path.GetFileNameWithoutExtension(p).ToLowerInvariant();
            return ExeBlocklist.Any(b => stem.Contains(b));
        }

        var usable = candidates.Where(p => !IsBlocked(p)).ToList();
        if (usable.Count == 0)
        {
            return null;
        }

        var preferred = usable.FirstOrDefault(p =>
        {
            var stem = Path.GetFileNameWithoutExtension(p).ToLowerInvariant();
            return stem.Contains("server") || stem.Contains("srcds") || stem.Contains("dedicated");
        });

        var chosen = preferred ?? usable.MaxBy(p =>
        {
            try
            {
                return new FileInfo(p).Length;
            }
            catch
            {
                return 0L;
            }
        });

        if (chosen is null)
        {
            return null;
        }

        return Path.GetRelativePath(rootDir, chosen);
    }

    private static void CollectExeFiles(string dir, List<string> output)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }

        foreach (var entry in Directory.EnumerateFileSystemEntries(dir))
        {
            if (Directory.Exists(entry))
            {
                CollectExeFiles(entry, output);
            }
            else if (string.Equals(Path.GetExtension(entry), ".exe", StringComparison.OrdinalIgnoreCase))
            {
                output.Add(entry);
            }
        }
    }

    /// <summary>
    /// Port of <c>SteamCmdProvider.ParseProgress(string line) -&gt; SteamProgress?</c>. Matches
    /// the precompiled regex from the decompiled <c>.cctor</c>:
    /// <c>Update state \(0x[0-9a-fA-F]+\)\s*(?&lt;state&gt;[\w ]+?),\s*progress:\s*(?&lt;pct&gt;[0-9.]+)\s*\(\s*(?&lt;done&gt;\d+)\s*/\s*(?&lt;total&gt;\d+)\s*\)</c>.
    /// </summary>
    internal static SteamProgress? ParseProgress(string line)
    {
        var match = SteamProgressRegex().Match(line);
        if (!match.Success)
        {
            return null;
        }

        if (!double.TryParse(match.Groups["pct"].Value, System.Globalization.CultureInfo.InvariantCulture, out var pct)
            || !long.TryParse(match.Groups["done"].Value, System.Globalization.CultureInfo.InvariantCulture, out var done)
            || !long.TryParse(match.Groups["total"].Value, System.Globalization.CultureInfo.InvariantCulture, out var total))
        {
            return null;
        }

        return new SteamProgress
        {
            State = match.Groups["state"].Value.Trim(),
            Percent = Math.Clamp(pct, 0.0, 100.0),
            BytesDone = done,
            BytesTotal = total,
        };
    }

    [GeneratedRegex(@"Update state \(0x[0-9a-fA-F]+\)\s*(?<state>[\w ]+?),\s*progress:\s*(?<pct>[0-9.]+)\s*\(\s*(?<done>\d+)\s*/\s*(?<total>\d+)\s*\)")]
    private static partial Regex SteamProgressRegex();

    /// <summary>
    /// Port of <c>SteamCmdProvider.Megabytes(long bytes) -&gt; string</c>. Used by the original to
    /// format <c>SteamProgress.BytesDone</c>/<c>BytesTotal</c> for display; kept here (tested
    /// below) even though this port's <c>InstallAsync</c> reports percentage-only progress rather
    /// than a formatted "X / Y MB" string.
    /// </summary>
    internal static string Megabytes(long bytes) => Math.Round(bytes / 1_048_576.0).ToString("F0", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>
    /// Port of <c>SteamCmdProvider.GetAppInfoAsync(int appId, IProgress&lt;InstallProgress&gt;, CancellationToken)</c>:
    /// runs <c>steamcmd +login anonymous +app_info_update 1 +app_info_print &lt;id&gt; +quit</c>
    /// and parses its VDF-formatted stdout with <see cref="Vdf"/>. Not part of
    /// <see cref="IInstallProvider"/> itself (the decompiled type exposes it as an extra public
    /// method, used elsewhere by <c>InstallService</c> per the decompile report's call graph),
    /// but ported here since it's the one real use of <see cref="Vdf.ToAppInfo"/> outside of
    /// tests.
    /// </summary>
    public static async Task<SteamAppInfo> GetAppInfoAsync(int appId, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var steamcmd = await EnsureSteamCmdAsync(progress, ct).ConfigureAwait(false);
        progress.Report(InstallProgress.Indeterminate($"SteamCMD: reading app info for {appId}…"));

        var args = new[] { "+@NoPromptForPassword", "1", "+login", "anonymous", "+app_info_update", "1", "+app_info_print", appId.ToString(), "+quit" };
        var stdout = (await ExternalProcess.RunAsync(steamcmd, args, workingDir: null, onLine: null, AppInfoTimeout, ct).ConfigureAwait(false)).Output;

        var root = Vdf.Parse(stdout);
        var info = Vdf.ToAppInfo(root);
        if (info is null || (string.IsNullOrEmpty(info.Name) && info.Executable is null))
        {
            throw new InvalidOperationException($"SteamCMD produced no parsable app info for {appId}.");
        }

        return info;
    }

    public Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct)
        => Task.FromResult(new List<string> { "latest" });

    /// <summary>
    /// Port of <c>InstallAsync</c>: runs
    /// <c>steamcmd +force_install_dir "&lt;target&gt;" &lt;login&gt; +app_update &lt;id&gt; validate +quit</c>,
    /// streaming stdout for progress, then auto-detects the server executable.
    /// </summary>
    public async Task<InstallResult> InstallAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var appId = request.Game.Install.AppId
            ?? throw new InvalidOperationException($"Game '{request.Game.Id}' has no steamcmd app_id.");

        var steamcmd = await EnsureSteamCmdAsync(progress, ct).ConfigureAwait(false);
        progress.Report(InstallProgress.Indeterminate($"SteamCMD: downloading app {appId} (this can take a while)…"));

        if (request.TargetDir.Contains('"'))
        {
            throw new InvalidOperationException("The install folder path contains a quote character, which SteamCMD can't accept.");
        }

        void OnLine(string line)
        {
            // Streamed live (not parsed after exit) so the progress bar actually moves.
            var parsed = ParseProgress(line);
            if (parsed is not null)
            {
                progress.Report(InstallProgress.At($"SteamCMD: {parsed.State}", parsed.Percent));
            }
        }

        string? scriptDir = null;
        ExternalProcessResult run;
        try
        {
            var loginLine = RunscriptLoginLine(request.Game.Install);
            string[] args;
            if (loginLine is not null)
            {
                // Credentials never touch the command line: they go in a runscript that only
                // the current user can read, deleted right after the run.
                var script = WritePrivateRunscript(new[]
                {
                    "@ShutdownOnFailedCommand 1",
                    "@NoPromptForPassword 1",
                    $"force_install_dir \"{request.TargetDir}\"",
                    loginLine,
                    $"app_update {appId} validate",
                    "quit",
                });
                scriptDir = Path.GetDirectoryName(script);
                args = new[] { "+runscript", script };
            }
            else
            {
                var login = request.Game.Install.Anonymous || string.IsNullOrWhiteSpace(request.Game.Install.Username)
                    ? "anonymous"
                    : RequireSafeToken(request.Game.Install.Username!, "Steam username");
                args = new[]
                {
                    "+@ShutdownOnFailedCommand", "1",
                    "+@NoPromptForPassword", "1",
                    "+force_install_dir", request.TargetDir,
                    "+login", login,
                    "+app_update", appId.ToString(), "validate",
                    "+quit",
                };
            }

            run = await ExternalProcess.RunAsync(steamcmd, args, request.TargetDir, OnLine, InstallTimeout, ct).ConfigureAwait(false);
        }
        finally
        {
            if (scriptDir is not null)
            {
                TryDeleteDirectory(scriptDir);
            }
        }

        if (!InstallSucceeded(run.ExitCode, run.Output, appId))
        {
            var detail = LastErrorLine(run.Output);
            throw new InvalidOperationException(
                $"SteamCMD could not install app {appId} (exit code {run.ExitCode})" + (detail is null ? "." : $": {detail}"));
        }

        var definitionExe = ExeFromDefinition(request);
        var serverExe = definitionExe ?? AutoDetectServerExe(request.TargetDir)
            ?? throw new InvalidOperationException(
                $"SteamCMD exited and no server executable was found in {request.TargetDir}.");

        return new InstallResult
        {
            ServerFile = serverExe,
            Version = $"latest (app {appId})",
            StartCommand = null,
            JavaMajor = 0,
        };
    }
}

/// <summary>
/// Port of <c>SteamProgress</c> (a value type used by <c>ParseProgress</c>'s
/// <c>Nullable&lt;SteamProgress&gt;</c> return): a single parsed
/// <c>"Update state (...) ..., progress: N (a / b)"</c> line.
/// </summary>
public sealed class SteamProgress
{
    public string State { get; set; } = string.Empty;

    public double Percent { get; set; }

    public long BytesDone { get; set; }

    public long BytesTotal { get; set; }
}
