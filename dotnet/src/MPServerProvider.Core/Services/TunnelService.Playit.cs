using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using MPServerProvider.Core.Native;

namespace MPServerProvider.Core.Services;

public enum PlayitState
{
    NotInstalled,
    Stopped,
    Installing,
    Starting,
    WaitingForClaim,
    Connected,
    Error,
}

public sealed record PlayitTunnel(string Address, string Destination)
{
    public int? DestinationPort
    {
        get
        {
            var match = System.Text.RegularExpressions.Regex.Match(Destination ?? string.Empty, @":(\d{1,5})\b");
            return match.Success && int.TryParse(match.Groups[1].Value, out var port) && port is > 0 and <= 65535 ? port : null;
        }
    }
}

public sealed record PlayitPortCheck(IReadOnlyList<PlayitTunnel> Matching, IReadOnlyList<PlayitTunnel> OtherPorts)
{
    public int? SuggestedPort =>
        Matching.Count == 0 && OtherPorts.Select(t => t.DestinationPort).Distinct().ToList() is [int only]
            ? only
            : null;

    public static PlayitPortCheck For(IEnumerable<PlayitTunnel> tunnels, int serverPort)
    {
        var withPort = tunnels.Where(t => t.DestinationPort is not null).ToList();
        return new PlayitPortCheck(
            withPort.Where(t => t.DestinationPort == serverPort).ToList(),
            withPort.Where(t => t.DestinationPort != serverPort).ToList());
    }
}

public sealed record PlayitStatus(
    PlayitState State,
    string Message,
    string? AgentPath,
    string? ClaimUrl,
    IReadOnlyList<PlayitTunnel> Tunnels,
    bool UsingInstalledService,
    string DashboardUrl);

public sealed record GitHubReleaseAsset(string Name, string DownloadUrl, long Size, string? Digest);

public enum PlayitLineKind
{
    None,
    ClaimUrl,
    LoginUrl,
    WaitingForApproval,
    Approved,
    SetupComplete,
    Connected,
    Tunnel,
    PortableMode,
    Error,
    Fatal,
}

public sealed record PlayitLineInfo(PlayitLineKind Kind, string? Url = null, PlayitTunnel? Tunnel = null, string? Message = null);

public sealed record PlayitLifecycle(
    string State,
    string? ErrorMessage,
    IReadOnlyList<PlayitTunnel> Tunnels,
    int PendingTunnels,
    string? LoginLink,
    IReadOnlyList<string> Notices);

public static class PlayitOutputParser
{
    private static readonly Regex Ansi = new(@"\x1B\[[0-9;?]*[ -/]*[@-~]", RegexOptions.Compiled);
    private static readonly Regex ClaimUrl = new(@"https://playit\.gg/claim/[0-9A-Za-z]+", RegexOptions.Compiled);
    private static readonly Regex GuestLogin = new(@"https://playit\.gg/login/guest-account/[^\s""']+", RegexOptions.Compiled);
    private static readonly Regex TunnelLine = new(
        @"^\s*(?:[●◐○•*\-]\s*)?(?<addr>(?:[A-Za-z0-9-]+\.)+[A-Za-z]{2,}(?::\d{1,5})?|\d{1,3}(?:\.\d{1,3}){3}:\d{1,5})\s*=>\s*(?<dest>[^\s(]+)",
        RegexOptions.Compiled);
    private static readonly Regex LogLevel = new(@"^\S+\s+(?<level>TRACE|DEBUG|INFO|WARN|ERROR)\s+(?<target>[\w:]+):\s*(?<msg>.*)$", RegexOptions.Compiled);
    private static readonly Regex LongSecret = new(@"\b[0-9a-fA-F]{48,}\b", RegexOptions.Compiled);

    public static string StripAnsi(string line) => Ansi.Replace(line ?? string.Empty, string.Empty);

    public static string Redact(string line)
    {
        var text = GuestLogin.Replace(line ?? string.Empty, "https://playit.gg/login/guest-account/<hidden>");
        text = Regex.Replace(text, @"(secret(?:_key)?\s*[=:]\s*)\S+", "$1<hidden>", RegexOptions.IgnoreCase);
        return LongSecret.Replace(text, "<hidden>");
    }

    public static bool IsTrustedPlayitUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && uri.Scheme == Uri.UriSchemeHttps
        && uri.IsDefaultPort
        && string.IsNullOrEmpty(uri.UserInfo)
        && (uri.Host.Equals("playit.gg", StringComparison.OrdinalIgnoreCase)
            || uri.Host.EndsWith(".playit.gg", StringComparison.OrdinalIgnoreCase));

    public static PlayitLineInfo Parse(string rawLine)
    {
        var line = StripAnsi(rawLine).Trim();
        if (line.Length == 0)
        {
            return new PlayitLineInfo(PlayitLineKind.None);
        }

        if (GuestLogin.Match(line) is { Success: true } login)
        {
            return new PlayitLineInfo(PlayitLineKind.LoginUrl, Url: login.Value);
        }
        if (ClaimUrl.Match(line) is { Success: true } claim)
        {
            return new PlayitLineInfo(PlayitLineKind.ClaimUrl, Url: claim.Value);
        }

        var message = line;
        string? level = null;
        if (LogLevel.Match(line) is { Success: true } log)
        {
            level = log.Groups["level"].Value;
            message = log.Groups["msg"].Value;
        }

        bool Has(string text) => message.Contains(text, StringComparison.OrdinalIgnoreCase);

        if (Has("Setup was not approved"))
        {
            return Fatal("playit.gg setup was not approved in the browser. Start the agent again to get a new link.");
        }
        if (Has("already running in another window"))
        {
            return Fatal("playit is already running in another window. Close it (or use it), then start the agent here again.");
        }
        if (Has("over the agent limit"))
        {
            return Fatal("Your playit.gg account is over its agent limit. Delete unused agents at https://playit.gg/account/agents.");
        }
        if (Has("invalid secret") || Has("secret is no longer valid"))
        {
            return Fatal("The playit agent's saved secret is invalid. Run \"playit reset\" in a terminal, then start the agent again to set it up.");
        }
        if (Has("Setup timed out"))
        {
            return Fatal("playit.gg setup timed out before it was approved.");
        }
        if (Has("Could not connect to the playit service") || Has("Could not start playit"))
        {
            return Fatal(message);
        }
        if (Has("Approve this program in your browser"))
        {
            return new PlayitLineInfo(PlayitLineKind.WaitingForApproval, Message: message);
        }
        if (Has("Program approved"))
        {
            return new PlayitLineInfo(PlayitLineKind.Approved, Message: message);
        }
        if (Has("playit setup is complete"))
        {
            return new PlayitLineInfo(PlayitLineKind.SetupComplete, Message: message);
        }
        if (Has("playit connected; tunnels loaded") || Has("playit tunnel state updated"))
        {
            return new PlayitLineInfo(PlayitLineKind.Connected, Message: message);
        }
        if (Has("background service is not installed, so playit is running in this window"))
        {
            return new PlayitLineInfo(PlayitLineKind.PortableMode, Message: message);
        }
        if (TunnelLine.Match(message) is { Success: true } tunnel)
        {
            return new PlayitLineInfo(PlayitLineKind.Tunnel,
                Tunnel: new PlayitTunnel(tunnel.Groups["addr"].Value, tunnel.Groups["dest"].Value));
        }
        if (level == "ERROR" || Has("stopped unexpectedly"))
        {
            return new PlayitLineInfo(PlayitLineKind.Error, Message: message);
        }
        return new PlayitLineInfo(PlayitLineKind.None, Message: message);

        static PlayitLineInfo Fatal(string text) => new(PlayitLineKind.Fatal, Message: text);
    }

    public static PlayitLifecycle? ParseIpcStateResponse(string line)
    {
        try
        {
            using var doc = JsonDocument.Parse(line);
            var root = doc.RootElement;
            if (!Str(root, "message_kind").Equals("response", StringComparison.Ordinal)
                || !root.TryGetProperty("data", out var data)
                || !data.TryGetProperty("response", out var response))
            {
                return null;
            }

            var type = Str(response, "type");
            if (type == "error" && response.TryGetProperty("data", out var err))
            {
                return new PlayitLifecycle("error", Str(err, "message"), Array.Empty<PlayitTunnel>(), 0, null, Array.Empty<string>());
            }
            if (type != "state" || !response.TryGetProperty("data", out var lifecycle))
            {
                return null;
            }

            var state = Str(lifecycle, "state");
            lifecycle.TryGetProperty("data", out var payload);
            if (state != "running")
            {
                var error = payload.ValueKind == JsonValueKind.Object ? Str(payload, "message") : null;
                return new PlayitLifecycle(state, string.IsNullOrEmpty(error) ? null : error,
                    Array.Empty<PlayitTunnel>(), 0, null, Array.Empty<string>());
            }

            var tunnels = new List<PlayitTunnel>();
            if (payload.ValueKind == JsonValueKind.Object && payload.TryGetProperty("tunnels", out var list) && list.ValueKind == JsonValueKind.Array)
            {
                foreach (var t in list.EnumerateArray())
                {
                    var address = Str(t, "display_address");
                    if (address.Length == 0)
                    {
                        continue;
                    }
                    var destination = Str(t, "destination");
                    if (t.TryGetProperty("is_disabled", out var disabled) && disabled.ValueKind == JsonValueKind.True)
                    {
                        var reason = Str(t, "disabled_reason");
                        destination += reason.Length > 0 ? $" (disabled: {reason})" : " (disabled)";
                    }
                    tunnels.Add(new PlayitTunnel(address, destination));
                }
            }

            var pending = payload.ValueKind == JsonValueKind.Object && payload.TryGetProperty("pending_tunnels", out var p) && p.ValueKind == JsonValueKind.Array
                ? p.GetArrayLength()
                : 0;
            var notices = new List<string>();
            if (payload.ValueKind == JsonValueKind.Object && payload.TryGetProperty("notices", out var n) && n.ValueKind == JsonValueKind.Array)
            {
                notices.AddRange(n.EnumerateArray().Select(x => Str(x, "message")).Where(m => m.Length > 0));
            }
            var loginLink = payload.ValueKind == JsonValueKind.Object ? Str(payload, "login_link") : string.Empty;

            return new PlayitLifecycle("running", null, tunnels, pending, loginLink.Length > 0 ? loginLink : null, notices);
        }
        catch (JsonException)
        {
            return null;
        }

        static string Str(JsonElement e, string name) =>
            e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String
                ? v.GetString() ?? string.Empty
                : string.Empty;
    }

    public static int? ParseIpcHelloVersion(string? line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return null;
        }
        try
        {
            using var doc = JsonDocument.Parse(line);
            return doc.RootElement.TryGetProperty("data", out var data)
                   && data.TryGetProperty("protocol", out var protocol)
                   && protocol.TryGetProperty("ipc_version", out var version)
                   && version.TryGetInt32(out var v)
                ? v
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}

public sealed partial class TunnelService
{
    public const string PlayitServicePipe = "playitd-system";

    public const string PlayitPortablePipePrefix = "playit-portable-";

    public const string PlayitDefaultDashboardUrl = "https://playit.gg/account/agents";

    private static readonly int[] PlayitRestartDelaysSeconds = { 2, 5, 10, 20, 40, 60 };

    private readonly HttpClient _githubHttp;
    private readonly object _playitLock = new();
    private readonly LinkedList<string> _playitOutput = new();
    private IPlayitRunner? _playitRunner;
    private CancellationTokenSource? _playitCts;
    private int _playitGeneration;
    private bool _playitWanted;
    private int _playitQuickFailures;
    private DateTimeOffset _playitStartedAt;
    private string? _playitFatal;

    private PlayitState _playitState = PlayitState.NotInstalled;
    private string _playitMessage = string.Empty;
    private string? _playitAgentPath;
    private string? _playitClaimUrl;
    private string? _playitLoginUrl;
    private string? _playitIpcLoginLink;
    private IReadOnlyList<PlayitTunnel> _playitTunnels = Array.Empty<PlayitTunnel>();
    private bool _playitUsesService;

    public event Action<PlayitStatus>? PlayitStatusChanged;

    public event Action<string>? PlayitOutputReceived;

    public string PlayitExePath(IToolPaths paths) => Path.Combine(paths.ToolsDir, "playit", "playit.exe");

    public bool PlayitInstalled(IToolPaths paths) => File.Exists(PlayitExePath(paths));

    private string? ManagedPlayitPath => _toolsDir is null ? null : Path.Combine(_toolsDir, "playit", "playit.exe");

    public bool IsPlayitRunning
    {
        get
        {
            lock (_playitLock)
            {
                return _playitRunner is { HasExited: false };
            }
        }
    }

    public PlayitStatus PlayitCurrentStatus
    {
        get
        {
            lock (_playitLock)
            {
                return SnapshotLocked();
            }
        }
    }

    public IReadOnlyList<string> PlayitOutputSnapshot()
    {
        lock (_playitOutput)
        {
            return _playitOutput.ToList();
        }
    }

    public string? LocatePlayitAgent()
    {
        try
        {
            var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            foreach (var entry in path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var dir = entry.Trim('"');
                if (!Path.IsPathFullyQualified(dir))
                {
                    continue;
                }
                var candidate = Path.Combine(dir, "playit.exe");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            foreach (var root in new[] { Environment.SpecialFolder.ProgramFiles, Environment.SpecialFolder.ProgramFilesX86 })
            {
                var dir = Environment.GetFolderPath(root);
                if (string.IsNullOrEmpty(dir))
                {
                    continue;
                }
                var candidate = Path.Combine(dir, "playit_gg", "bin", "playit.exe");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }
        catch (Exception ex) when (ex is ArgumentException or IOException or UnauthorizedAccessException)
        {
            Log($"Searching for playit.exe failed: {ex.Message}");
        }

        return ManagedPlayitPath is { } managed && File.Exists(managed) ? managed : null;
    }

    private void RefreshPlayitInstallState()
    {
        var path = LocatePlayitAgent();
        lock (_playitLock)
        {
            _playitAgentPath = path;
            if (_playitState is PlayitState.NotInstalled or PlayitState.Stopped)
            {
                _playitState = path is null ? PlayitState.NotInstalled : PlayitState.Stopped;
                _playitMessage = path is null
                    ? "The playit.gg agent is not installed. Install downloads the official agent from playit-cloud's GitHub releases."
                    : "The agent is installed but not running.";
            }
        }
    }

    // -- Install ------------------------------------------------------------------------------

    public async Task<string> InstallPlayitAsync(IProgress<double>? progress = null, CancellationToken ct = default)
    {
        var target = ManagedPlayitPath ?? throw new InvalidOperationException("No tools folder is configured to install playit into.");
        SetPlayitState(PlayitState.Installing, "Looking up the latest playit.gg agent release…");
        try
        {
            var release = await GetLatestReleaseJsonAsync(ct).ConfigureAwait(false);
            var arch = RuntimeInformation.OSArchitecture;
            var asset = SelectPlayitAsset(ParseReleaseAssets(release), arch)
                        ?? throw new InvalidOperationException("The latest playit.gg release has no Windows agent download.");
            if (!Uri.TryCreate(asset.DownloadUrl, UriKind.Absolute, out var url) || url.Scheme != Uri.UriSchemeHttps
                || !url.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The playit.gg release download is not a github.com HTTPS link; refusing to download it.");
            }

            SetPlayitState(PlayitState.Installing, $"Downloading {asset.Name}…");
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            var temp = target + $".{Guid.NewGuid():N}.part";
            try
            {
                await DownloadToFileAsync(url, temp, asset.Size, progress, ct).ConfigureAwait(false);
                VerifyDownloadedAgent(temp, asset, arch == Architecture.X86 ? (ushort)0x014C : (ushort)0x8664);
                File.Move(temp, target, overwrite: true);
            }
            finally
            {
                TryDelete(temp);
            }

            Log($"Installed playit agent {asset.Name} to {target}.");
            lock (_playitLock)
            {
                _playitAgentPath = LocatePlayitAgent() ?? target;
            }
            SetPlayitState(PlayitState.Stopped, "The agent is installed. Press Start to run it.");
            return target;
        }
        catch (Exception ex)
        {
            Log($"Installing playit failed: {ex}");
            var installed = LocatePlayitAgent() is not null;
            SetPlayitState(installed ? PlayitState.Stopped : PlayitState.NotInstalled,
                ex is OperationCanceledException ? "Download cancelled." : $"Download failed: {ex.Message}");
            throw;
        }
    }

    private async Task<string> GetLatestReleaseJsonAsync(CancellationToken ct)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(20));
        using var request = new HttpRequestMessage(HttpMethod.Get, PlayitReleasesApi);
        request.Headers.Accept.ParseAdd("application/vnd.github+json");
        try
        {
            using var response = await _githubHttp.SendAsync(request, timeout.Token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(timeout.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException("GitHub did not answer in time.");
        }
    }

    private async Task DownloadToFileAsync(Uri url, string path, long expectedSize, IProgress<double>? progress, CancellationToken ct)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromMinutes(5));
        try
        {
            using var response = await _githubHttp.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, timeout.Token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var total = response.Content.Headers.ContentLength ?? expectedSize;
            if (total > 200_000_000)
            {
                throw new InvalidOperationException("The download is unexpectedly large; refusing it.");
            }
            await using var source = await response.Content.ReadAsStreamAsync(timeout.Token).ConfigureAwait(false);
            await using var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            var buffer = new byte[81920];
            long done = 0;
            int read;
            while ((read = await source.ReadAsync(buffer, timeout.Token).ConfigureAwait(false)) > 0)
            {
                await file.WriteAsync(buffer.AsMemory(0, read), timeout.Token).ConfigureAwait(false);
                done += read;
                if (done > 200_000_000)
                {
                    throw new InvalidOperationException("The download is unexpectedly large; refusing it.");
                }
                if (total > 0)
                {
                    progress?.Report(Math.Min(1.0, (double)done / total));
                }
            }
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException("The playit.gg download took too long.");
        }
    }

    private static void VerifyDownloadedAgent(string path, GitHubReleaseAsset asset, ushort expectedMachine)
    {
        var info = new FileInfo(path);
        if (asset.Size > 0 && info.Length != asset.Size)
        {
            throw new InvalidOperationException($"The download is incomplete ({info.Length} of {asset.Size} bytes).");
        }

        if (asset.Digest is { } digest && digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
        {
            using var stream = File.OpenRead(path);
            var actual = Convert.ToHexString(SHA256.HashData(stream));
            if (!actual.Equals(digest["sha256:".Length..], StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The download's SHA-256 doesn't match the release; it was discarded.");
            }
        }

        var head = new byte[4096];
        int read;
        using (var stream = File.OpenRead(path))
        {
            read = stream.Read(head, 0, head.Length);
        }
        if (!IsValidPeExecutable(head.AsSpan(0, read), expectedMachine))
        {
            throw new InvalidOperationException("The download is not a Windows program for this PC; it was discarded.");
        }
    }

    public static bool IsValidPeExecutable(ReadOnlySpan<byte> head, ushort expectedMachine)
    {
        if (head.Length < 0x40 || head[0] != (byte)'M' || head[1] != (byte)'Z')
        {
            return false;
        }
        var peOffset = BitConverter.ToInt32(head.Slice(0x3C, 4));
        if (peOffset < 0x40 || peOffset > head.Length - 6)
        {
            return false;
        }
        if (head[peOffset] != (byte)'P' || head[peOffset + 1] != (byte)'E' || head[peOffset + 2] != 0 || head[peOffset + 3] != 0)
        {
            return false;
        }
        return BitConverter.ToUInt16(head.Slice(peOffset + 4, 2)) == expectedMachine;
    }

    public static IReadOnlyList<GitHubReleaseAsset> ParseReleaseAssets(string releaseJson)
    {
        var list = new List<GitHubReleaseAsset>();
        using var doc = JsonDocument.Parse(releaseJson);
        if (!doc.RootElement.TryGetProperty("assets", out var assets) || assets.ValueKind != JsonValueKind.Array)
        {
            return list;
        }
        foreach (var a in assets.EnumerateArray())
        {
            var name = a.TryGetProperty("name", out var n) ? n.GetString() : null;
            var url = a.TryGetProperty("browser_download_url", out var u) ? u.GetString() : null;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url))
            {
                continue;
            }
            var size = a.TryGetProperty("size", out var s) && s.TryGetInt64(out var sz) ? sz : 0;
            var digest = a.TryGetProperty("digest", out var d) && d.ValueKind == JsonValueKind.String ? d.GetString() : null;
            list.Add(new GitHubReleaseAsset(name, url, size, digest));
        }
        return list;
    }

    public static GitHubReleaseAsset? SelectPlayitAsset(IEnumerable<GitHubReleaseAsset> assets, Architecture arch)
    {
        var archToken = arch == Architecture.X86 ? "x86" : "x86_64";
        var pattern = new Regex($@"^playit-windows-{Regex.Escape(archToken)}(?<signed>-signed)?\.exe$", RegexOptions.IgnoreCase);
        return assets
            .Select(a => (Asset: a, Match: pattern.Match(a.Name)))
            .Where(x => x.Match.Success)
            .OrderByDescending(x => x.Match.Groups["signed"].Success)
            .Select(x => x.Asset)
            .FirstOrDefault();
    }

    // -- Start / stop -------------------------------------------------------------------------

    public async Task StartPlayitAgentAsync(bool installIfMissing, CancellationToken ct = default)
    {
        if (IsPlayitRunning)
        {
            return;
        }

        var path = LocatePlayitAgent();
        if (path is null)
        {
            if (!installIfMissing)
            {
                SetPlayitState(PlayitState.NotInstalled, "The playit.gg agent is not installed yet. Press Install first.");
                throw new InvalidOperationException("The playit.gg agent is not installed.");
            }
            path = await InstallPlayitAsync(null, ct).ConfigureAwait(false);
        }

        lock (_playitLock)
        {
            _playitWanted = true;
            _playitQuickFailures = 0;
            _playitFatal = null;
            _playitClaimUrl = null;
            _playitLoginUrl = null;
            _playitIpcLoginLink = null;
            _playitTunnels = Array.Empty<PlayitTunnel>();
            _playitAgentPath = path;
        }
        lock (_playitOutput)
        {
            _playitOutput.Clear();
        }
        LaunchPlayit(path);
    }

    private void LaunchPlayit(string path)
    {
        CancellationTokenSource cts;
        int generation;
        lock (_playitLock)
        {
            _playitCts?.Cancel();
            _playitCts?.Dispose();
            _playitCts = cts = new CancellationTokenSource();
            generation = ++_playitGeneration;
            _playitStartedAt = DateTimeOffset.UtcNow;
            _playitUsesService = false;
        }

        SetPlayitState(PlayitState.Starting, "Starting the playit.gg agent…");
        AppendPlayitOutput($"[MP Server Provider] Starting {path} --stdout");

        IPlayitRunner runner;
        try
        {
            runner = StartRunner(path, line => HandlePlayitLine(generation, line));
        }
        catch (Exception ex)
        {
            Log($"Starting playit failed: {ex}");
            lock (_playitLock)
            {
                _playitWanted = false;
            }
            SetPlayitState(PlayitState.Error, $"Could not start the agent: {ex.Message}");
            throw;
        }

        lock (_playitLock)
        {
            _playitRunner = runner;
        }

        _ = Task.Run(() => WatchPlayitExitAsync(runner, generation, cts.Token));
        _ = Task.Run(() => PollPlayitIpcAsync(runner, generation, cts.Token));
    }

    private IPlayitRunner StartRunner(string path, Action<string> onLine)
    {
        if (OperatingSystem.IsWindows() && DeElevatedLauncher.IsCurrentProcessElevated())
        {
            try
            {
                return DeElevatedPlayitRunner.Start(path, onLine);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "MP Server Provider is running as administrator and could not start playit as your normal user, so it was not started. " +
                    "Restart MP Server Provider without \"Run as administrator\" to use playit.gg. (" + ex.Message + ")", ex);
            }
        }
        return DirectPlayitRunner.Start(path, onLine);
    }

    private async Task WatchPlayitExitAsync(IPlayitRunner runner, int generation, CancellationToken ct)
    {
        try
        {
            await runner.WaitForExitAsync(ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            Log($"Waiting for playit to exit failed: {ex.Message}");
        }

        int delaySeconds;
        string? fatal;
        lock (_playitLock)
        {
            if (generation != _playitGeneration || !_playitWanted)
            {
                return;
            }
            _playitRunner = null;
            fatal = _playitFatal;
            if (DateTimeOffset.UtcNow - _playitStartedAt > TimeSpan.FromMinutes(2))
            {
                _playitQuickFailures = 0;
            }
            _playitQuickFailures++;
            delaySeconds = _playitQuickFailures <= PlayitRestartDelaysSeconds.Length
                ? PlayitRestartDelaysSeconds[_playitQuickFailures - 1]
                : -1;
            if (fatal is not null || delaySeconds < 0)
            {
                _playitWanted = false;
            }
        }
        runner.Dispose();

        if (fatal is not null)
        {
            SetPlayitState(PlayitState.Error, fatal);
            Raise(TunnelNoticeLevel.Error, $"playit.gg: {fatal}");
            return;
        }
        if (delaySeconds < 0)
        {
            const string gaveUp = "The playit.gg agent keeps exiting, so it won't be restarted automatically. See the output below, then press Start to try again.";
            SetPlayitState(PlayitState.Error, gaveUp);
            Raise(TunnelNoticeLevel.Error, gaveUp);
            return;
        }

        AppendPlayitOutput($"[MP Server Provider] The agent exited; restarting in {delaySeconds} s.");
        SetPlayitState(PlayitState.Starting, $"The agent exited unexpectedly; restarting in {delaySeconds} s…");
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds), ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        string? path;
        lock (_playitLock)
        {
            if (generation != _playitGeneration || !_playitWanted)
            {
                return;
            }
            path = _playitAgentPath;
        }
        if (path is null || !File.Exists(path))
        {
            SetPlayitState(PlayitState.NotInstalled, "The playit.gg agent is no longer installed.");
            return;
        }
        try
        {
            LaunchPlayit(path);
        }
        catch (Exception ex)
        {
            Log($"Restarting playit failed: {ex.Message}");
        }
    }

    public async Task StopPlayitAgentAsync()
    {
        IPlayitRunner? runner;
        bool usesService;
        lock (_playitLock)
        {
            _playitWanted = false;
            _playitGeneration++;
            _playitCts?.Cancel();
            _playitCts?.Dispose();
            _playitCts = null;
            runner = _playitRunner;
            _playitRunner = null;
            usesService = _playitUsesService;
            _playitTunnels = Array.Empty<PlayitTunnel>();
            _playitClaimUrl = null;
        }
        lock (_playitAutoServers)
        {
            _playitStartedByAutomation = false;
        }

        if (runner is not null)
        {
            if (!usesService)
            {
                try
                {
                    // Only the pipe of the agent this app started: never stop someone else's playit window.
                    var ownPipes = runner.ProcessId > 0 ? new[] { PlayitPortablePipePrefix + runner.ProcessId } : Array.Empty<string>();
                    foreach (var pipe in ownPipes)
                    {
                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                        if (await PlayitIpcRequestAsync(pipe, "stop", TimeSpan.FromSeconds(2), cts.Token).ConfigureAwait(false) is not null)
                        {
                            break;
                        }
                    }
                    using var wait = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                    await runner.WaitForExitAsync(wait.Token).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // Fall through to a hard kill.
                }
            }
            runner.Kill();
            runner.Dispose();
        }

        AppendPlayitOutput(usesService
            ? "[MP Server Provider] Detached. The installed playit.gg service keeps your tunnels online in the background; stop it from its tray icon."
            : "[MP Server Provider] Agent stopped.");
        RefreshAfterStop(usesService);
    }

    public void StopPlayit()
    {
        IPlayitRunner? runner;
        lock (_playitLock)
        {
            _playitWanted = false;
            _playitGeneration++;
            _playitCts?.Cancel();
            _playitCts?.Dispose();
            _playitCts = null;
            runner = _playitRunner;
            _playitRunner = null;
        }
        if (runner is null)
        {
            return;
        }
        try
        {
            runner.Kill();
        }
        catch (Exception ex)
        {
            Log($"Killing playit failed: {ex.Message}");
        }
        runner.Dispose();
        RefreshAfterStop(false);
    }

    private void RefreshAfterStop(bool usedService)
    {
        lock (_playitLock)
        {
            _playitAgentPath = LocatePlayitAgent();
            _playitState = _playitAgentPath is null ? PlayitState.NotInstalled : PlayitState.Stopped;
            _playitMessage = usedService
                ? "Detached from the installed playit.gg service (it keeps running in the background)."
                : "The agent is stopped.";
            _playitTunnels = Array.Empty<PlayitTunnel>();
        }
        RaisePlayitStatus();
    }

    // -- Output and IPC -----------------------------------------------------------------------

    private void HandlePlayitLine(int generation, string rawLine)
    {
        try
        {
            var clean = PlayitOutputParser.StripAnsi(rawLine);
            AppendPlayitOutput(PlayitOutputParser.Redact(clean));

            var info = PlayitOutputParser.Parse(clean);
            lock (_playitLock)
            {
                if (generation != _playitGeneration)
                {
                    return;
                }
            }

            switch (info.Kind)
            {
                case PlayitLineKind.ClaimUrl when PlayitOutputParser.IsTrustedPlayitUrl(info.Url):
                    lock (_playitLock)
                    {
                        _playitClaimUrl = info.Url;
                    }
                    SetPlayitState(PlayitState.WaitingForClaim,
                        "Link this agent to playit.gg: open the claim link, sign in or continue as guest, and approve it. Then add a tunnel for your server's port on playit.gg.");
                    Raise(TunnelNoticeLevel.Info, "playit.gg needs to be linked once: open Connect to Tunnel → playit.gg Agent and press \"Open claim link\".");
                    break;
                case PlayitLineKind.LoginUrl when PlayitOutputParser.IsTrustedPlayitUrl(info.Url):
                    lock (_playitLock)
                    {
                        _playitLoginUrl = info.Url; // memory only: it carries a session key
                    }
                    RaisePlayitStatus();
                    break;
                case PlayitLineKind.WaitingForApproval:
                    SetPlayitState(PlayitState.WaitingForClaim, "Approve the agent in your browser (the claim link) to finish setup.");
                    break;
                case PlayitLineKind.Approved:
                case PlayitLineKind.SetupComplete:
                    lock (_playitLock)
                    {
                        _playitClaimUrl = null;
                    }
                    SetPlayitState(PlayitState.Starting, "Setup approved. Connecting to playit.gg…");
                    break;
                case PlayitLineKind.Connected:
                    SetPlayitState(PlayitState.Connected, ConnectedMessage());
                    break;
                case PlayitLineKind.Tunnel when info.Tunnel is { } tunnel:
                    lock (_playitLock)
                    {
                        if (!_playitTunnels.Contains(tunnel))
                        {
                            _playitTunnels = _playitTunnels.Append(tunnel).ToList();
                        }
                    }
                    SetPlayitState(PlayitState.Connected, ConnectedMessage());
                    break;
                case PlayitLineKind.Fatal:
                    lock (_playitLock)
                    {
                        _playitFatal = info.Message;
                    }
                    SetPlayitState(PlayitState.Error, info.Message ?? "The playit.gg agent reported an error.");
                    break;
                case PlayitLineKind.Error:
                    lock (_playitLock)
                    {
                        if (_playitState == PlayitState.Connected)
                        {
                            break;
                        }
                    }
                    SetPlayitState(PlayitState.Starting, $"The agent reported a problem and is retrying: {info.Message}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Log($"Handling playit output failed: {ex.Message}");
        }
    }

    private string ConnectedMessage()
    {
        lock (_playitLock)
        {
            return _playitTunnels.Count == 0
                ? "Connected to playit.gg, but this agent has no tunnels yet. Open the playit.gg dashboard and add a tunnel for your server's port."
                : "Connected to playit.gg. Players join using the address below.";
        }
    }

    private void AppendPlayitOutput(string line)
    {
        lock (_playitOutput)
        {
            _playitOutput.AddLast(line);
            while (_playitOutput.Count > 400)
            {
                _playitOutput.RemoveFirst();
            }
        }
        try
        {
            PlayitOutputReceived?.Invoke(line);
        }
        catch (Exception ex)
        {
            Log($"playit output handler failed: {ex.Message}");
        }
    }

    private async Task PollPlayitIpcAsync(IPlayitRunner runner, int generation, CancellationToken ct)
    {
        await Task.Delay(1500, ct).ContinueWith(_ => { }, TaskScheduler.Default).ConfigureAwait(false);
        while (!ct.IsCancellationRequested)
        {
            var connected = false;
            try
            {
                var pipes = PortablePipeCandidates(runner.ProcessId).Append(PlayitServicePipe);
                foreach (var pipe in pipes)
                {
                    var line = await PlayitIpcRequestAsync(pipe, "get_state", TimeSpan.FromSeconds(2), ct).ConfigureAwait(false);
                    if (line is null || PlayitOutputParser.ParseIpcStateResponse(line) is not { } lifecycle)
                    {
                        continue;
                    }
                    lock (_playitLock)
                    {
                        if (generation != _playitGeneration)
                        {
                            return;
                        }
                        _playitUsesService = pipe == PlayitServicePipe;
                    }
                    ApplyLifecycle(lifecycle);
                    connected = lifecycle.State == "running";
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                Log($"playit IPC poll failed: {ex.Message}");
            }

            try
            {
                await Task.Delay(connected ? 5000 : 2000, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    private void ApplyLifecycle(PlayitLifecycle lifecycle)
    {
        switch (lifecycle.State)
        {
            case "running":
                lock (_playitLock)
                {
                    _playitTunnels = lifecycle.Tunnels;
                    _playitClaimUrl = null;
                    _playitIpcLoginLink = PlayitOutputParser.IsTrustedPlayitUrl(lifecycle.LoginLink) ? lifecycle.LoginLink : null;
                }
                var message = ConnectedMessage();
                if (lifecycle.PendingTunnels > 0)
                {
                    message += $" ({lifecycle.PendingTunnels} tunnel(s) still being set up.)";
                }
                if (lifecycle.Notices.Count > 0)
                {
                    message += " Notice: " + lifecycle.Notices[0];
                }
                SetPlayitState(PlayitState.Connected, message);
                break;
            case "waiting_for_secret":
                bool hasClaim;
                lock (_playitLock)
                {
                    hasClaim = _playitClaimUrl is not null;
                }
                if (!hasClaim)
                {
                    SetPlayitState(PlayitState.Starting, "The agent is waiting to be set up; the claim link will appear shortly…");
                }
                break;
            case "has_invalid_secret":
            case "disabled_over_limit":
            case "error":
                SetPlayitState(PlayitState.Error, lifecycle.ErrorMessage ?? "The playit.gg agent reported an error.");
                break;
            case "starting":
            case "stopping":
                SetPlayitState(PlayitState.Starting, "Connecting to playit.gg…");
                break;
        }
    }

    private static IEnumerable<string> PortablePipeCandidates(int knownPid)
    {
        var names = new List<string>();
        if (knownPid > 0)
        {
            names.Add(PlayitPortablePipePrefix + knownPid);
        }
        try
        {
            foreach (var full in Directory.GetFiles(@"\\.\pipe\"))
            {
                var name = Path.GetFileName(full);
                if (name.StartsWith(PlayitPortablePipePrefix, StringComparison.OrdinalIgnoreCase) && !names.Contains(name))
                {
                    names.Add(name);
                }
            }
        }
        catch (Exception ex) when (ex is IOException or ArgumentException or UnauthorizedAccessException)
        {
            // Pipe enumeration is a convenience; the known PID is tried regardless.
        }
        return names;
    }

    private static async Task<string?> PlayitIpcRequestAsync(string pipeName, string requestType, TimeSpan timeout, CancellationToken ct)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);
        try
        {
            await using var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            await pipe.ConnectAsync((int)timeout.TotalMilliseconds, cts.Token).ConfigureAwait(false);
            using var reader = new StreamReader(pipe, new UTF8Encoding(false), false, 8192, leaveOpen: true);

            var hello = await reader.ReadLineAsync(cts.Token).ConfigureAwait(false);
            var version = PlayitOutputParser.ParseIpcHelloVersion(hello) ?? 3;
            var request = JsonSerializer.Serialize(new Dictionary<string, object>
            {
                ["ipc_version"] = version,
                ["request_id"] = 1,
                ["request"] = new Dictionary<string, string> { ["type"] = requestType },
            }) + "\n";
            var bytes = Encoding.UTF8.GetBytes(request);
            await pipe.WriteAsync(bytes, cts.Token).ConfigureAwait(false);
            await pipe.FlushAsync(cts.Token).ConfigureAwait(false);

            for (var i = 0; i < 50; i++)
            {
                var line = await reader.ReadLineAsync(cts.Token).ConfigureAwait(false);
                if (line is null)
                {
                    return null;
                }
                if (line.Contains("\"response\"", StringComparison.Ordinal))
                {
                    return line;
                }
            }
            return null;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return null;
        }
        catch (Exception ex) when (ex is IOException or TimeoutException or UnauthorizedAccessException or InvalidOperationException)
        {
            return null;
        }
    }

    // -- State --------------------------------------------------------------------------------

    private void SetPlayitState(PlayitState state, string message)
    {
        lock (_playitLock)
        {
            _playitState = state;
            _playitMessage = message;
        }
        RaisePlayitStatus();
    }

    private void RaisePlayitStatus()
    {
        PlayitStatus snapshot;
        lock (_playitLock)
        {
            snapshot = SnapshotLocked();
        }
        try
        {
            PlayitStatusChanged?.Invoke(snapshot);
        }
        catch (Exception ex)
        {
            Log($"playit status handler failed: {ex.Message}");
        }
    }

    private PlayitStatus SnapshotLocked() => new(
        _playitState,
        _playitMessage,
        _playitAgentPath,
        _playitClaimUrl,
        _playitTunnels,
        _playitUsesService,
        _playitIpcLoginLink ?? _playitLoginUrl ?? PlayitDefaultDashboardUrl);

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Best-effort temp cleanup.
        }
    }

    // -- Runners ------------------------------------------------------------------------------

    private interface IPlayitRunner : IDisposable
    {
        int ProcessId { get; }

        bool HasExited { get; }

        Task WaitForExitAsync(CancellationToken ct);

        void Kill();
    }

    private sealed class DirectPlayitRunner : IPlayitRunner
    {
        private readonly Process _process;
        private readonly KillOnCloseJob? _job;

        private DirectPlayitRunner(Process process, KillOnCloseJob? job)
        {
            _process = process;
            _job = job;
        }

        public static DirectPlayitRunner Start(string path, Action<string> onLine)
        {
            var psi = new ProcessStartInfo(path)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                WorkingDirectory = Path.GetDirectoryName(path) ?? Environment.CurrentDirectory,
            };
            psi.ArgumentList.Add("--stdout");
            psi.Environment["NO_COLOR"] = "1";

            var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            process.OutputDataReceived += (_, e) => { if (e.Data is not null) onLine(e.Data); };
            process.ErrorDataReceived += (_, e) => { if (e.Data is not null) onLine(e.Data); };
            if (!process.Start())
            {
                process.Dispose();
                throw new InvalidOperationException("playit.exe did not start.");
            }

            var job = KillOnCloseJob.TryCreateFor(process.Id);
            // No interactive answers: an empty stdin makes any y/n prompt take its default and
            // lets a "press Enter to close" pause end immediately.
            try
            {
                process.StandardInput.Close();
            }
            catch (IOException)
            {
            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return new DirectPlayitRunner(process, job);
        }

        public int ProcessId
        {
            get
            {
                try
                {
                    return _process.Id;
                }
                catch (InvalidOperationException)
                {
                    return 0;
                }
            }
        }

        public bool HasExited
        {
            get
            {
                try
                {
                    return _process.HasExited;
                }
                catch (InvalidOperationException)
                {
                    return true;
                }
            }
        }

        public Task WaitForExitAsync(CancellationToken ct) => _process.WaitForExitAsync(ct);

        public void Kill()
        {
            try
            {
                if (!_process.HasExited)
                {
                    _process.Kill(entireProcessTree: true);
                }
                _process.WaitForExit(2000);
            }
            catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception or NotSupportedException)
            {
                // Already gone.
            }
        }

        public void Dispose()
        {
            _process.Dispose();
            _job?.Dispose();
        }
    }

    private sealed class DeElevatedPlayitRunner : IPlayitRunner
    {
        private readonly Process _shell;
        private readonly string _logPath;
        private readonly CancellationTokenSource _tailCts = new();
        private readonly KillOnCloseJob? _job;

        private DeElevatedPlayitRunner(Process shell, string logPath, Action<string> onLine, KillOnCloseJob? job)
        {
            _shell = shell;
            _logPath = logPath;
            _job = job;
            _ = Task.Run(() => TailAsync(onLine, _tailCts.Token));
        }

        public static DeElevatedPlayitRunner Start(string path, Action<string> onLine)
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException();
            }
            if (path.Contains('"'))
            {
                throw new ArgumentException("Invalid agent path.");
            }
            var logPath = Path.Combine(Path.GetTempPath(), $"mpsp-playit-{Guid.NewGuid():N}.log");
            var cmd = Path.Combine(Environment.SystemDirectory, "cmd.exe");
            var commandLine = $"\"{cmd}\" /d /s /c \"\"{path}\" --stdout <nul >\"{logPath}\" 2>&1\"";
            var pid = DeElevatedLauncher.Launch(commandLine, Path.GetDirectoryName(path));
            var shell = Process.GetProcessById(pid);
            return new DeElevatedPlayitRunner(shell, logPath, onLine, KillOnCloseJob.TryCreateFor(pid));
        }

        public int ProcessId => 0; // the agent is cmd's child; its pipe is found by enumeration

        public bool HasExited
        {
            get
            {
                try
                {
                    return _shell.HasExited;
                }
                catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception)
                {
                    return true;
                }
            }
        }

        public Task WaitForExitAsync(CancellationToken ct) => _shell.WaitForExitAsync(ct);

        private async Task TailAsync(Action<string> onLine, CancellationToken ct)
        {
            var pending = new StringBuilder();
            var buffer = new char[4096];
            try
            {
                FileStream? stream = null;
                while (stream is null)
                {
                    try
                    {
                        stream = new FileStream(_logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    }
                    catch (FileNotFoundException)
                    {
                        await Task.Delay(250, ct).ConfigureAwait(false);
                    }
                }
                using var reader = new StreamReader(stream, Encoding.UTF8);
                while (true)
                {
                    var read = await reader.ReadAsync(buffer.AsMemory(), ct).ConfigureAwait(false);
                    if (read == 0)
                    {
                        if (HasExited)
                        {
                            break;
                        }
                        await Task.Delay(300, ct).ConfigureAwait(false);
                        continue;
                    }
                    pending.Append(buffer, 0, read);
                    var text = pending.ToString();
                    var lastBreak = text.LastIndexOf('\n');
                    if (lastBreak < 0)
                    {
                        continue;
                    }
                    foreach (var line in text[..lastBreak].Split('\n'))
                    {
                        onLine(line.TrimEnd('\r'));
                    }
                    pending.Clear().Append(text[(lastBreak + 1)..]);
                }
                if (pending.Length > 0)
                {
                    onLine(pending.ToString().TrimEnd('\r'));
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                // Tailing is best-effort; the IPC poll still reports state.
            }
        }

        public void Kill()
        {
            try
            {
                if (!_shell.HasExited)
                {
                    _shell.Kill(entireProcessTree: true);
                }
                _shell.WaitForExit(2000);
            }
            catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception or NotSupportedException)
            {
                // Already gone or not ours to kill.
            }
        }

        public void Dispose()
        {
            _tailCts.Cancel();
            _shell.Dispose();
            _job?.Dispose();
            TryDelete(_logPath);
        }
    }

    private sealed class KillOnCloseJob : IDisposable
    {
        private const int JobObjectExtendedLimitInformation = 9;
        private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000;
        private const uint PROCESS_SET_QUOTA = 0x0100;
        private const uint PROCESS_TERMINATE = 0x0001;

        private IntPtr _handle;

        private KillOnCloseJob(IntPtr handle) => _handle = handle;

        public static KillOnCloseJob? TryCreateFor(int pid)
        {
            if (!OperatingSystem.IsWindows())
            {
                return null;
            }
            var job = IntPtr.Zero;
            var process = IntPtr.Zero;
            try
            {
                job = CreateJobObject(IntPtr.Zero, null);
                if (job == IntPtr.Zero)
                {
                    return null;
                }
                var info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();
                info.BasicLimitInformation.LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;
                var size = Marshal.SizeOf<JOBOBJECT_EXTENDED_LIMIT_INFORMATION>();
                var ptr = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.StructureToPtr(info, ptr, false);
                    if (!SetInformationJobObject(job, JobObjectExtendedLimitInformation, ptr, (uint)size))
                    {
                        CloseHandle(job);
                        return null;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }

                process = OpenProcess(PROCESS_SET_QUOTA | PROCESS_TERMINATE, false, (uint)pid);
                if (process == IntPtr.Zero || !AssignProcessToJobObject(job, process))
                {
                    CloseHandle(job);
                    return null;
                }
                return new KillOnCloseJob(job);
            }
            catch (Exception)
            {
                if (job != IntPtr.Zero)
                {
                    CloseHandle(job);
                }
                return null;
            }
            finally
            {
                if (process != IntPtr.Zero)
                {
                    CloseHandle(process);
                }
            }
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                CloseHandle(_handle);
                _handle = IntPtr.Zero;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public uint LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public uint ActiveProcessLimit;
            public UIntPtr Affinity;
            public uint PriorityClass;
            public uint SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public UIntPtr ProcessMemoryLimit;
            public UIntPtr JobMemoryLimit;
            public UIntPtr PeakProcessMemoryUsed;
            public UIntPtr PeakJobMemoryUsed;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string? lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetInformationJobObject(IntPtr hJob, int infoClass, IntPtr lpInfo, uint cbInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint access, bool inherit, uint pid);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);
    }
}
