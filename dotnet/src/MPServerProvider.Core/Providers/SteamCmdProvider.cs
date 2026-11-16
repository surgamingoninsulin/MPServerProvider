using System.Diagnostics;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Native;

namespace MPServerProvider.Core.Providers;

public sealed partial class SteamCmdProvider : IInstallProvider
{
    private const string SteamCmdZipUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";

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

    internal static readonly TimeSpan InstallTimeout = TimeSpan.FromHours(3);

    internal static readonly TimeSpan AppInfoTimeout = TimeSpan.FromMinutes(10);

    internal static string LoginArguments(InstallSpec spec)
    {
        if (spec.Anonymous || string.IsNullOrWhiteSpace(spec.Username))
        {
            return "+login anonymous";
        }

        return $"+login \"{RequireSafeToken(spec.Username!, "Steam username")}\"";
    }

    internal static string? RunscriptLoginLine(InstallSpec spec)
    {
        if (spec.Anonymous || string.IsNullOrWhiteSpace(spec.Username) || string.IsNullOrEmpty(spec.Password))
        {
            return null;
        }

        string password;
        try
        {
            // `SecretProtector.Unprotect` applied to the stored password: the stored
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

    public static bool NeedsSteamLogin(GameDefinition game)
        => game.Install.Provider == "steamcmd"
           && !game.Install.Anonymous
           && string.IsNullOrWhiteSpace(game.Install.Username);

    internal static string TypedLoginLine(SteamLogin login)
    {
        var line = $"login \"{RequireSafeToken(login.Username.Trim(), "Steam username")}\" \"{RequireSafeToken(login.Password!, "Steam password")}\"";
        return string.IsNullOrWhiteSpace(login.GuardCode)
            ? line
            : $"{line} \"{RequireSafeToken(login.GuardCode.Trim(), "Steam Guard code")}\"";
    }

    internal static string? LoginProblem(string output, InstallRequest request)
    {
        bool Has(string text) => output.Contains(text, StringComparison.OrdinalIgnoreCase);
        var game = request.Game.Name;

        if (Has("Two-factor code mismatch") || Has("Invalid Login Auth Code"))
        {
            return "Steam didn't accept the Steam Guard code. Enter the newest code from the Steam app or email and try again.";
        }

        if (Has("Steam Guard") || Has("two-factor") || Has("Account Logon Denied") || Has("AccountLogonDenied"))
        {
            return "Steam wants a Steam Guard code for this login. Open the Steam app on your phone (or check your email), then enter the code and try again.";
        }

        if (Has("Invalid Password") || Has("InvalidPassword"))
        {
            return "Steam says the username or password is wrong.";
        }

        if (Has("Rate Limit") || Has("RateLimitExceeded"))
        {
            return "Steam blocked the login for a while after too many attempts. Wait 30 minutes or so and try again.";
        }

        if (Has("Cached credentials not found") || (request.SteamLogin is { Password: null or "" } && Has("password")))
        {
            return "SteamCMD has no saved login for this account any more. Enter your password again.";
        }

        if (Has("No subscription") || Has("Invalid platform"))
        {
            return request.SteamLogin is null && request.Game.Install.Anonymous
                ? $"Steam won't let anonymous accounts download the {game} server."
                : $"This Steam account doesn't own {game}. Log in with an account that owns the game.";
        }

        return null;
    }

    internal static string? BranchName(InstallSpec spec)
    {
        var branch = spec.Branch?.Trim();
        if (string.IsNullOrEmpty(branch) || branch.Equals("public", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (!branch.All(c => char.IsAsciiLetterOrDigit(c) || c is '_' or '-' or '.'))
        {
            throw new InvalidOperationException($"The Steam beta branch '{branch}' contains characters SteamCMD can't accept.");
        }

        return branch;
    }

    private static string BranchSuffix(InstallSpec spec)
        => BranchName(spec) is { } branch ? $" -beta {branch}" : string.Empty;

    private static string RequireSafeToken(string value, string what)
    {
        if (value.Any(c => c == '"' || char.IsControl(c)))
        {
            throw new InvalidOperationException($"The {what} contains a quote or control character, which SteamCMD can't accept.");
        }

        return value;
    }

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

    private static string? LastErrorLine(string output)
        => output.Split('\n')
            .Select(l => l.Trim())
            .LastOrDefault(l => l.Contains("ERROR!", StringComparison.Ordinal) || l.Contains("FAILED", StringComparison.Ordinal));

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

    internal static bool DefinitionNamesExecutable(string executable)
        => !string.IsNullOrWhiteSpace(executable) && executable.Contains("{server_file}");

    private static string? ExeFromDefinition(InstallRequest request)
    {
        var executable = request.Game.Runtime.Executable;
        if (DefinitionNamesExecutable(executable))
        {
            return null;
        }

        // A runtime placeholder ({dotnet}/{java}) launches a .dll/.jar named in the arguments.
        if (executable.Trim() is "{dotnet}" or "{java}")
        {
            var payload = System.Text.RegularExpressions.Regex.Match(
                request.Game.Runtime.Arguments ?? string.Empty, @"\{server_root\}[^""\s]*\.(dll|jar)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (!payload.Success)
            {
                return null;
            }

            var payloadPath = payload.Value.Replace("{server_root}", request.TargetDir);
            return File.Exists(payloadPath) ? payloadPath : null;
        }

        var resolved = executable.Replace("{server_root}", request.TargetDir).Trim();
        if (resolved.Length == 0)
        {
            return null;
        }

        var path = Path.IsPathRooted(resolved) ? resolved : Path.Combine(request.TargetDir, resolved);
        return File.Exists(path) ? resolved : null;
    }

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

    internal static string Megabytes(long bytes) => Math.Round(bytes / 1_048_576.0).ToString("F0", System.Globalization.CultureInfo.InvariantCulture);

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
            var loginLine = request.SteamLogin is { Password.Length: > 0 } typed
                ? TypedLoginLine(typed)
                : RunscriptLoginLine(request.Game.Install);
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
                    $"app_update {appId}{BranchSuffix(request.Game.Install)} validate",
                    "quit",
                });
                scriptDir = Path.GetDirectoryName(script);
                args = new[] { "+runscript", script };
            }
            else
            {
                // A login without a password reuses the session SteamCMD cached last time.
                var login = request.SteamLogin is { } cached
                    ? RequireSafeToken(cached.Username.Trim(), "Steam username")
                    : request.Game.Install.Anonymous || string.IsNullOrWhiteSpace(request.Game.Install.Username)
                        ? "anonymous"
                        : RequireSafeToken(request.Game.Install.Username!, "Steam username");
                var argList = new List<string>
                {
                    "+@ShutdownOnFailedCommand", "1",
                    "+@NoPromptForPassword", "1",
                    "+force_install_dir", request.TargetDir,
                    "+login", login,
                    "+app_update", appId.ToString(),
                };
                if (BranchName(request.Game.Install) is { } branch)
                {
                    argList.Add("-beta");
                    argList.Add(branch);
                }

                argList.Add("validate");
                argList.Add("+quit");
                args = argList.ToArray();
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
            if (LoginProblem(run.Output, request) is { } loginProblem)
            {
                throw new InvalidOperationException(loginProblem);
            }

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

public sealed class SteamProgress
{
    public string State { get; set; } = string.Empty;

    public double Percent { get; set; }

    public long BytesDone { get; set; }

    public long BytesTotal { get; set; }
}
