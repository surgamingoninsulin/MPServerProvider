using System.Diagnostics;

using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

public class WorkshopService
{
    private const string SteamCmdZipUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";

    internal static readonly TimeSpan WorkshopItemTimeout = TimeSpan.FromMinutes(60);

    private readonly AppPaths _paths;

    public WorkshopService(AppPaths paths)
    {
        _paths = paths;
    }

    public string CacheDirFor(long appId) => Path.Combine(_paths.WorkshopDir, appId.ToString());

    public string ItemDir(long appId, long itemId) => Path.Combine(CacheDirFor(appId), itemId.ToString());

    public bool IsCached(long appId, long itemId) => Directory.Exists(ItemDir(appId, itemId));

    public List<long> CachedItems(long appId)
    {
        var dir = CacheDirFor(appId);
        if (!Directory.Exists(dir))
        {
            return new List<long>();
        }

        var items = Directory.GetDirectories(dir)
            .Select(d => Path.GetFileName(d))
            .Where(name => long.TryParse(name, out _))
            .Select(long.Parse)
            .ToList();
        items.Sort();
        return items;
    }

    public async Task DownloadItemsAsync(GameDefinition game, IProgress<InstallProgress> progress, CancellationToken ct = default)
    {
        var appId = game.Install.AppId
            ?? throw new InvalidOperationException($"Game '{game.Id}' has no steamcmd app_id.");
        if (game.Install.WorkshopIds.Count == 0)
        {
            return;
        }

        var steamcmd = await EnsureSteamCmdAsync(progress, ct).ConfigureAwait(false);
        foreach (var itemId in game.Install.WorkshopIds)
        {
            progress.Report(InstallProgress.Indeterminate($"Downloading Workshop item {itemId} for app {appId}…"));
            var args = new[]
            {
                "+@NoPromptForPassword", "1",
                "+login", "anonymous",
                "+workshop_download_item", appId.ToString(), itemId.ToString(),
                "+quit",
            };

            // ExternalProcess drains stdout/stderr continuously (the old code redirected both but
            // never read them, so steamcmd blocked forever once a pipe buffer filled), kills the
            // process on cancel and enforces a timeout.
            var run = await Providers.ExternalProcess.RunAsync(
                steamcmd, args, workingDir: null, onLine: null, WorkshopItemTimeout, ct).ConfigureAwait(false);
            var succeeded = run.Output.Contains("Success. Downloaded item", StringComparison.OrdinalIgnoreCase)
                || (run.ExitCode == 0 && !run.Output.Contains("ERROR!", StringComparison.Ordinal));
            if (!succeeded)
            {
                throw new InvalidOperationException($"steamcmd exited with {run.ExitCode} downloading workshop item {itemId}");
            }
        }
    }

    public void DeployToServer(GameDefinition game, string serverRoot)
    {
        if (game.Install.AppId is not { } appId)
        {
            return;
        }

        foreach (var itemId in game.Install.WorkshopIds)
        {
            var source = ItemDir(appId, itemId);
            if (!Directory.Exists(source))
            {
                continue;
            }

            var dest = Path.Combine(serverRoot, "steamapps", "workshop", "content", appId.ToString(), itemId.ToString());
            CopyDirectory(source, dest);
        }
    }

    public List<ModItem> ScanItemsForApp(long appId, string gameId)
    {
        var result = new List<ModItem>();
        foreach (var itemId in CachedItems(appId))
        {
            var dir = ItemDir(appId, itemId);
            var sizeBytes = DirSize(dir);
            var imported = GetCreatedOrModified(dir);
            result.Add(new ModItem
            {
                FileName = $"workshop_{itemId}",
                GameId = gameId,
                SizeBytes = sizeBytes,
                Imported = imported,
                LibraryPath = dir,
                AssignedServers = new List<string>(),
            });
        }

        return result;
    }

    private async Task<string> EnsureSteamCmdAsync(IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var dir = Path.Combine(_paths.ToolsDir, "steamcmd");
        var exe = Path.Combine(dir, "steamcmd.exe");
        if (File.Exists(exe))
        {
            return exe;
        }

        var pathVar = Environment.GetEnvironmentVariable("PATH");
        if (!string.IsNullOrEmpty(pathVar))
        {
            foreach (var candidateDir in pathVar.Split(Path.PathSeparator))
            {
                if (candidateDir.Length == 0)
                {
                    continue;
                }

                var candidate = Path.Combine(candidateDir, "steamcmd.exe");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        progress.Report(InstallProgress.Indeterminate("Downloading SteamCMD (official Valve tool)"));
        var zipPath = Path.Combine(dir, "steamcmd.zip");
        await DownloadService.DownloadFileAsync(SteamCmdZipUrl, zipPath, "Downloading SteamCMD…", progress, ct).ConfigureAwait(false);
        DownloadService.ExtractZip(zipPath, dir, "Extracting SteamCMD…", progress);
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

    private static void CopyDirectory(string src, string dest)
    {
        Directory.CreateDirectory(dest);
        foreach (var file in WalkFiles(src))
        {
            var relative = Path.GetRelativePath(src, file);
            var target = Path.Combine(dest, relative);
            var parent = Path.GetDirectoryName(target);
            if (!string.IsNullOrEmpty(parent))
            {
                Directory.CreateDirectory(parent);
            }

            File.Copy(file, target, overwrite: true);
        }
    }

    private static long DirSize(string dir)
        => WalkFiles(dir).Sum(f => new FileInfo(f).Length);

    private static DateTime GetCreatedOrModified(string path)
    {
        try
        {
            var created = File.GetCreationTimeUtc(path);
            var modified = File.GetLastWriteTimeUtc(path);
            return created < modified ? created : modified;
        }
        catch
        {
            return DateTime.UtcNow;
        }
    }

    private static List<string> WalkFiles(string dir)
    {
        var result = new List<string>();
        if (!Directory.Exists(dir))
        {
            return result;
        }

        foreach (var entry in Directory.EnumerateFileSystemEntries(dir))
        {
            if (Directory.Exists(entry))
            {
                result.AddRange(WalkFiles(entry));
            }
            else
            {
                result.Add(entry);
            }
        }

        return result;
    }
}
