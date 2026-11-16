using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Services.ModLibraryService</c> (decompile token
/// 0x0200000f). Manages imported/downloaded mods under <c>{DataRoot}/mods/&lt;gameId&gt;/...</c>
/// and assigns/removes them into/from a server's own mods folder.
/// </summary>
public class ModLibraryService
{
    /// <summary>
    /// Game ids whose mod folder lives under a fixed <c>txData\resources</c> subpath regardless
    /// of the manifest's own <c>mods.folder</c> (Cfx.re's txAdmin layout).
    /// </summary>
    private static readonly string[] TxAdminGames = { "fivem", "redm" };

    private readonly AppPaths _paths;
    private readonly ServerRepository _servers;
    private readonly GameLibraryService _games;

    /// <summary>Parsed jar metadata keyed by full path, valid while size + last-write time match.</summary>
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, (long Size, DateTime Modified, MinecraftModInfo Info)> _metadataCache
        = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Recursive folder sizes keyed by full path, valid while the folder's last-write time matches.</summary>
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, (DateTime Modified, long Size)> _dirSizeCache
        = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Port of <c>.ctor(AppPaths, ServerRepository, GameLibraryService)</c>.</summary>
    public ModLibraryService(AppPaths paths, ServerRepository servers, GameLibraryService games)
    {
        _paths = paths;
        _servers = servers;
        _games = games;
    }

    /// <summary>
    /// Port of <c>Scan(string? gameId) -&gt; List&lt;ModItem&gt;</c>: every file/folder under
    /// <c>{ModsDir}/&lt;gameId&gt;/</c>, for every game-id subfolder when <paramref name="workshop"/>
    /// is present, plus (when the folder's game uses <c>steamcmd</c> with <c>workshop_ids</c>) any
    /// cached Steam Workshop items for that game's <c>AppId</c> not already represented on disk.
    /// </summary>
    /// <remarks>
    /// Performance: this used to stat every entry several times, recursively size every folder
    /// on every call, and probe every server's mods folder once per (mod × server) pair — about
    /// 650–880 ms for ~460 entries / 6 servers, and the Home page runs it on the UI thread. Now
    /// entries come from one directory enumeration (size and times arrive with the listing),
    /// each server's mods folder is listed once per scan, folder sizes and jar metadata are
    /// cached by path + size/last-write time, and jar metadata is only read when
    /// <paramref name="readMetadata"/> is set (the Mods pages, off the UI thread).
    /// </remarks>
    public List<ModItem> Scan(WorkshopService? workshop = null, bool readMetadata = false)
    {
        var modsDir = _paths.ModsDir;
        if (!Directory.Exists(modsDir))
        {
            return new List<ModItem>();
        }

        var result = new List<ModItem>();
        IEnumerable<string> gameDirs;
        try
        {
            gameDirs = Directory.GetDirectories(modsDir);
        }
        catch
        {
            // Best-effort: the mods directory can be transiently unreadable (locked, permission
            // denied, removable-drive hiccup). An empty scan is preferable to crashing the whole
            // application, which this previously did unconditionally.
            return result;
        }

        var servers = _servers.Servers;
        foreach (var dir in gameDirs)
        {
            var gameId = Path.GetFileName(dir);

            List<FileSystemInfo> entries;
            try
            {
                entries = new DirectoryInfo(dir).EnumerateFileSystemInfos().ToList();
            }
            catch
            {
                continue;
            }

            // One listing per server of this game instead of File/Directory.Exists per mod.
            var serverEntries = ServerEntrySets(servers.Where(s => s.ServerGame == gameId));

            foreach (var entry in entries)
            {
                try
                {
                    var isDir = entry is DirectoryInfo;
                    var fileName = entry.Name;
                    result.Add(new ModItem
                    {
                        FileName = fileName,
                        GameId = gameId,
                        SizeBytes = isDir ? CachedDirSize((DirectoryInfo)entry) : ((FileInfo)entry).Length,
                        Imported = entry.CreationTimeUtc < entry.LastWriteTimeUtc ? entry.CreationTimeUtc : entry.LastWriteTimeUtc,
                        ModifiedUtc = entry.LastWriteTimeUtc,
                        LibraryPath = entry.FullName,
                        IsDirectory = isDir,
                        Minecraft = readMetadata && !isDir && ModCompatibility.IsJarFile(fileName)
                            ? GetMinecraftInfo((FileInfo)entry)
                            : null,
                        AssignedServers = serverEntries
                            .Where(kv => kv.Value.Contains(fileName))
                            .Select(kv => kv.Key)
                            .ToList(),
                    });
                }
                catch
                {
                    // Best-effort: one bad entry (broken symlink, permission-denied file, path too
                    // long) shouldn't blank out every other mod in the scan.
                }
            }

            try
            {
                if (workshop is not null && _games.Find(gameId) is { } game
                    && game.Install.Provider == "steamcmd"
                    && game.Install.WorkshopIds.Count > 0
                    && game.Install.AppId is { } appId)
                {
                    var existing = new HashSet<string>(result.Select(m => m.FileName));
                    foreach (var item in workshop.ScanItemsForApp(appId, gameId))
                    {
                        if (!existing.Contains(item.FileName))
                        {
                            item.IsDirectory = true;
                            result.Add(item);
                        }
                    }
                }
            }
            catch
            {
                // Best-effort: a Workshop cache read failure shouldn't blank out the on-disk mods
                // already collected above.
            }
        }

        return result;
    }

    /// <summary>
    /// Port of <c>ImportFromFolder(string folder, string gameId) -&gt; int</c>: copies every
    /// file/subfolder under <paramref name="folder"/> matching the game's <c>mods.extensions</c>
    /// (a folder is imported wholesale, regardless of its contents' extensions) into
    /// <c>{ModsDir}/&lt;gameId&gt;/</c>, returning the count imported.
    /// </summary>
    /// <remarks>
    /// Minecraft Java only loads jar files, so for it sub-folders are skipped (they used to be
    /// copied wholesale, which is how other games' mod folders ended up in the Minecraft library
    /// and then on Minecraft servers).
    /// </remarks>
    public int ImportFromFolder(string folder, string gameId)
    {
        var game = _games.Find(gameId);
        if (game is null)
        {
            return 0;
        }

        var extensions = game.Mods.Extensions;
        var destDir = Path.Combine(_paths.ModsDir, gameId);
        Directory.CreateDirectory(destDir);
        var jarsOnly = ModCompatibility.IsMinecraftJava(gameId);

        var count = 0;
        foreach (var entry in Directory.EnumerateFileSystemEntries(folder))
        {
            var name = Path.GetFileName(entry);
            if (Directory.Exists(entry))
            {
                if (jarsOnly)
                {
                    continue;
                }

                CopyDirectory(entry, Path.Combine(destDir, name));
                count++;
                continue;
            }

            if (jarsOnly && !ModCompatibility.IsJarFile(name))
            {
                continue;
            }

            var ext = Path.GetExtension(entry);
            var matches = extensions.Count == 0
                || extensions.Any(e => string.Equals(e.TrimStart('.'), ext.TrimStart('.'), StringComparison.OrdinalIgnoreCase));
            if (matches)
            {
                File.Copy(entry, Path.Combine(destDir, name), overwrite: true);
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Port of <c>AssignToServer(ModItem, ServerInstance)</c>, plus a compatibility gate: an
    /// item <see cref="CheckCompatibility"/> rejects (wrong game, non-jar on Minecraft, wrong
    /// loader/variant, clear Minecraft version mismatch) is refused here with
    /// <see cref="ModIncompatibleException"/>, so no UI path can install it.
    /// </summary>
    public void AssignToServer(ModItem item, ServerInstance server)
    {
        var destDir = ServerModsDir(server)
            ?? throw new InvalidOperationException("This server does not support mods.");

        if (string.IsNullOrEmpty(item.LibraryPath))
        {
            throw new InvalidOperationException($"Mod '{item.FileName}' has no library path to copy from.");
        }

        var compatibility = CheckCompatibility(item, server);
        if (!compatibility.IsCompatible)
        {
            throw new ModIncompatibleException(
                $"{item.FileName} can't be added to {server.ServerName}: {compatibility.Reason}.");
        }

        Directory.CreateDirectory(destDir);

        var dest = Path.Combine(destDir, item.FileName);
        if (Directory.Exists(item.LibraryPath))
        {
            CopyDirectory(item.LibraryPath, dest);
        }
        else
        {
            File.Copy(item.LibraryPath, dest, overwrite: true);
        }

        if (TxAdminGames.Contains(server.ServerGame))
        {
            AddEnsureLine(destDir, item.FileName);
        }
    }

    /// <summary>
    /// Installs files dropped onto a server's Mods tab straight into its mods/plugins folder,
    /// through the same compatibility gate as <see cref="AssignToServer"/>. A dropped folder is
    /// expanded to the jars directly inside it on Minecraft (which only loads jars) and copied as
    /// one item for other games (FiveM resources, BepInEx plugin folders, …).
    /// </summary>
    public ModDropResult InstallFilesToServer(IEnumerable<string> paths, ServerInstance server)
    {
        var destDir = ServerModsDir(server)
            ?? throw new InvalidOperationException("This server does not support mods.");
        var result = new ModDropResult();
        foreach (var path in ExpandDropped(paths, server.ServerGame, result))
        {
            var name = Path.GetFileName(path);
            var dest = Path.Combine(destDir, name);
            if (SamePath(path, dest))
            {
                result.Skipped.Add((name, "already in this server's folder"));
                continue;
            }

            var item = new ModItem
            {
                FileName = name,
                GameId = server.ServerGame,
                LibraryPath = path,
                IsDirectory = Directory.Exists(path),
            };
            var verdict = CheckCompatibility(item, server);
            if (!verdict.IsCompatible)
            {
                result.Skipped.Add((name, verdict.Reason ?? "not compatible with this server"));
                continue;
            }

            try
            {
                var existed = File.Exists(dest) || Directory.Exists(dest);
                AssignToServer(item, server);
                (existed ? result.Replaced : result.Added).Add(name);
                if (!item.IsDirectory && ModCompatibility.IsJarFile(name)
                    && FindOtherCopy(destDir, name, GetMinecraftInfo(dest)) is { } other)
                {
                    result.Duplicates.Add((name, other));
                }
            }
            catch (Exception ex)
            {
                result.Skipped.Add((name, ex.Message));
            }
        }

        return result;
    }

    /// <summary>
    /// Copies files dropped onto the Mod Library page into the library for
    /// <paramref name="gameId"/>, applying the same filters as <see cref="ImportFromFolder"/>
    /// (Minecraft: jars only; others: the game's mod extensions, folders copied whole).
    /// </summary>
    public ModDropResult ImportFilesToLibrary(IEnumerable<string> paths, string gameId)
    {
        var result = new ModDropResult();
        var game = _games.Find(gameId);
        if (game is null)
        {
            return result;
        }

        var destDir = Path.Combine(_paths.ModsDir, gameId);
        Directory.CreateDirectory(destDir);
        var jarsOnly = ModCompatibility.IsMinecraftJava(gameId);
        var extensions = game.Mods.Extensions;
        foreach (var path in ExpandDropped(paths, gameId, result))
        {
            var name = Path.GetFileName(path);
            var dest = Path.Combine(destDir, name);
            if (SamePath(path, dest))
            {
                result.Skipped.Add((name, "already in the library"));
                continue;
            }

            try
            {
                var existed = File.Exists(dest) || Directory.Exists(dest);
                if (Directory.Exists(path))
                {
                    CopyDirectory(path, dest);
                }
                else if (jarsOnly && !ModCompatibility.IsJarFile(name))
                {
                    result.Skipped.Add((name, "not a .jar — Minecraft only loads .jar files"));
                    continue;
                }
                else if (extensions.Count > 0 && !extensions.Any(e =>
                             string.Equals(e.TrimStart('.'), Path.GetExtension(name).TrimStart('.'), StringComparison.OrdinalIgnoreCase)))
                {
                    result.Skipped.Add((name, $"not a {string.Join("/", extensions)} file"));
                    continue;
                }
                else
                {
                    File.Copy(path, dest, overwrite: true);
                }

                (existed ? result.Replaced : result.Added).Add(name);
            }
            catch (Exception ex)
            {
                result.Skipped.Add((name, ex.Message));
            }
        }

        return result;
    }

    /// <summary>Dropped paths → the items to install: Minecraft folders become the jars directly
    /// inside them; anything that no longer exists is reported as skipped.</summary>
    private static List<string> ExpandDropped(IEnumerable<string> paths, string gameId, ModDropResult result)
    {
        var items = new List<string>();
        foreach (var raw in paths)
        {
            var path = Path.TrimEndingDirectorySeparator(raw);
            var name = Path.GetFileName(path);
            if (Directory.Exists(path))
            {
                if (!ModCompatibility.IsMinecraftJava(gameId))
                {
                    items.Add(path);
                    continue;
                }

                var jars = Directory.EnumerateFiles(path, "*.jar").ToList();
                if (jars.Count == 0)
                {
                    result.Skipped.Add((name, "folder has no .jar files"));
                }

                items.AddRange(jars);
            }
            else if (File.Exists(path))
            {
                items.Add(path);
            }
            else
            {
                result.Skipped.Add((name, "not found"));
            }
        }

        return items;
    }

    /// <summary>Another jar in <paramref name="dir"/> declaring the same mod/plugin id or name as
    /// the just-added <paramref name="fileName"/> (typically an older version left behind).</summary>
    private string? FindOtherCopy(string dir, string fileName, MinecraftModInfo added)
    {
        var key = added.Id ?? added.Name;
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        foreach (var file in new DirectoryInfo(dir).EnumerateFiles("*.jar"))
        {
            if (string.Equals(file.Name, fileName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var info = GetMinecraftInfo(file);
            if (string.Equals(info.Id ?? info.Name, key, StringComparison.OrdinalIgnoreCase))
            {
                return file.Name;
            }
        }

        return null;
    }

    private static bool SamePath(string a, string b) => string.Equals(
        Path.TrimEndingDirectorySeparator(Path.GetFullPath(a)),
        Path.TrimEndingDirectorySeparator(Path.GetFullPath(b)),
        StringComparison.OrdinalIgnoreCase);

    /// <summary>Port of <c>RemoveFromServer(ModItem, ServerInstance)</c>.</summary>
    public void RemoveFromServer(ModItem item, ServerInstance server)
    {
        var destDir = ServerModsDir(server);
        if (destDir is null)
        {
            return;
        }

        var target = Path.Combine(destDir, item.FileName);
        if (Directory.Exists(target))
        {
            Directory.Delete(target, recursive: true);
        }
        else if (File.Exists(target))
        {
            File.Delete(target);
        }

        if (TxAdminGames.Contains(server.ServerGame))
        {
            RemoveEnsureLine(destDir, item.FileName);
        }
    }

    /// <summary>Port of <c>DeleteFromLibrary(ModItem)</c>.</summary>
    public void DeleteFromLibrary(ModItem item)
    {
        if (string.IsNullOrEmpty(item.LibraryPath))
        {
            return;
        }

        if (!AppPaths.IsStrictlyInside(item.LibraryPath, _paths.ModsDir)
            && !AppPaths.IsStrictlyInside(item.LibraryPath, _paths.WorkshopDir))
        {
            throw new InvalidOperationException($"Refusing to delete '{item.LibraryPath}': it is not inside the mod library.");
        }

        if (Directory.Exists(item.LibraryPath))
        {
            Directory.Delete(item.LibraryPath, recursive: true);
        }
        else if (File.Exists(item.LibraryPath))
        {
            File.Delete(item.LibraryPath);
        }
    }

    /// <summary>Port of <c>ServerModsDir(ServerInstance) -&gt; string?</c>.</summary>
    public string? ServerModsDir(ServerInstance server)
    {
        if (TxAdminGames.Contains(server.ServerGame))
        {
            var root = _servers.RootFor(server);
            // Port of the `txData`/`resources` lookup: txAdmin stores resources under
            // `{root}\txData\<profile>\resources`; the decompiled call graph enumerates
            // `txData`'s subdirectories to find the active profile rather than hardcoding its
            // name.
            var txData = Path.Combine(root, "txData");
            if (Directory.Exists(txData))
            {
                var profile = Directory.GetDirectories(txData).FirstOrDefault();
                if (profile is not null)
                {
                    return Path.Combine(profile, "resources");
                }
            }

            return Path.Combine(txData, "default", "resources");
        }

        var folder = server.ModsFolder;
        if (string.IsNullOrWhiteSpace(folder) || !IsSafeRelativeFolder(folder))
        {
            // A rooted path or one with ".." segments (e.g. from a tampered/imported server.json)
            // would let assign/remove write or delete outside the server's own folder.
            return null;
        }

        return Path.Combine(_servers.RootFor(server), folder);
    }

    /// <summary>
    /// <c>true</c> for a plain relative folder path (e.g. <c>mods</c>, <c>BepInEx/plugins</c>):
    /// not rooted, no drive/UNC prefix, no <c>.</c>/<c>..</c> segments, no invalid characters.
    /// </summary>
    internal static bool IsSafeRelativeFolder(string folder)
    {
        if (Path.IsPathRooted(folder) || folder.Contains(':') || folder.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            return false;
        }

        var segments = folder.Split('/', '\\');
        return segments.All(s => s.Length > 0 && s != "." && s != "..");
    }

    /// <summary>
    /// Port of <c>FindAssignments(string gameId, string fileName) -&gt; List&lt;string&gt;</c>:
    /// every server (of the matching game) whose mods dir already contains <paramref name="fileName"/>.
    /// </summary>
    internal List<string> FindAssignments(string gameId, string fileName)
        => _servers.Servers
            .Where(s => s.ServerGame == gameId)
            .Where(s => ServerModsDir(s) is { } dir
                && (File.Exists(Path.Combine(dir, fileName)) || Directory.Exists(Path.Combine(dir, fileName))))
            .Select(s => s.ServerName)
            .ToList();

    /// <summary>
    /// Compatibility of a library item with a server (see <see cref="ModCompatibility.Check"/>).
    /// Reads the item's jar metadata from disk (cached) when the item doesn't carry it, so the
    /// verdict never depends on how the caller built the <see cref="ModItem"/>.
    /// </summary>
    public ModCompatibilityResult CheckCompatibility(ModItem item, ServerInstance server)
    {
        var probe = item;
        if (!string.IsNullOrEmpty(item.LibraryPath))
        {
            var isDir = Directory.Exists(item.LibraryPath);
            probe = new ModItem
            {
                FileName = item.FileName,
                GameId = item.GameId,
                LibraryPath = item.LibraryPath,
                IsDirectory = isDir,
                Minecraft = !isDir && ModCompatibility.IsJarFile(item.FileName)
                    ? item.Minecraft ?? GetMinecraftInfo(item.LibraryPath)
                    : null,
            };
        }

        return ModCompatibility.Check(probe, server, _games.Find(server.ServerGame));
    }

    /// <summary>Jar metadata for <paramref name="path"/>, cached by path + size + last-write time.</summary>
    public MinecraftModInfo GetMinecraftInfo(string path)
    {
        try
        {
            var info = new FileInfo(path);
            return info.Exists ? GetMinecraftInfo(info) : new MinecraftModInfo { IsReadable = false, Error = "File not found" };
        }
        catch (Exception ex)
        {
            return new MinecraftModInfo { IsReadable = false, Error = ex.Message };
        }
    }

    private MinecraftModInfo GetMinecraftInfo(FileInfo file)
    {
        var key = file.FullName;
        var size = file.Length;
        var mtime = file.LastWriteTimeUtc;
        if (_metadataCache.TryGetValue(key, out var cached) && cached.Size == size && cached.Modified == mtime)
        {
            return cached.Info;
        }

        var parsed = MinecraftJarReader.Read(key);
        _metadataCache[key] = (size, mtime, parsed);
        return parsed;
    }

    /// <summary>
    /// What is currently installed in <paramref name="server"/>'s mods/plugins folder, with jar
    /// metadata (for the per-server Mods tab). Empty when the server has no mods folder yet.
    /// </summary>
    public List<ModItem> ListServerMods(ServerInstance server)
    {
        var result = new List<ModItem>();
        var dir = ServerModsDir(server);
        if (dir is null || !Directory.Exists(dir))
        {
            return result;
        }

        foreach (var entry in new DirectoryInfo(dir).EnumerateFileSystemInfos())
        {
            try
            {
                var isDir = entry is DirectoryInfo;
                result.Add(new ModItem
                {
                    FileName = entry.Name,
                    GameId = server.ServerGame,
                    SizeBytes = isDir ? CachedDirSize((DirectoryInfo)entry) : ((FileInfo)entry).Length,
                    Imported = entry.LastWriteTimeUtc,
                    ModifiedUtc = entry.LastWriteTimeUtc,
                    LibraryPath = entry.FullName,
                    IsDirectory = isDir,
                    Minecraft = !isDir && ModCompatibility.IsJarFile(entry.Name) ? GetMinecraftInfo((FileInfo)entry) : null,
                    AssignedServers = new List<string> { server.ServerName },
                });
            }
            catch
            {
                // One unreadable entry shouldn't hide the rest.
            }
        }

        return result;
    }

    /// <summary>Server name → names present in its mods folder (one listing per server).</summary>
    private Dictionary<string, HashSet<string>> ServerEntrySets(IEnumerable<ServerInstance> servers)
    {
        var result = new Dictionary<string, HashSet<string>>();
        foreach (var server in servers)
        {
            try
            {
                if (ServerModsDir(server) is { } dir && Directory.Exists(dir))
                {
                    result[server.ServerName] = new HashSet<string>(
                        Directory.EnumerateFileSystemEntries(dir).Select(Path.GetFileName)!,
                        StringComparer.OrdinalIgnoreCase);
                }
            }
            catch
            {
                // Unreadable server folder: treat as "nothing assigned".
            }
        }

        return result;
    }

    /// <summary>
    /// Recursive folder size, cached by path + the folder's own last-write time. A folder's
    /// timestamp only moves when its direct children change, so an in-place edit deep inside a
    /// library folder can show a stale size until the folder itself changes — acceptable for a
    /// display-only figure, and it keeps big folder mods/Workshop items from being re-walked on
    /// every refresh.
    /// </summary>
    private long CachedDirSize(DirectoryInfo dir)
    {
        var key = dir.FullName;
        var mtime = dir.LastWriteTimeUtc;
        if (_dirSizeCache.TryGetValue(key, out var cached) && cached.Modified == mtime)
        {
            return cached.Size;
        }

        long size = 0;
        try
        {
            foreach (var file in dir.EnumerateFiles("*", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true }))
            {
                size += file.Length;
            }
        }
        catch
        {
            // Best-effort.
        }

        _dirSizeCache[key] = (mtime, size);
        return size;
    }

    /// <summary>
    /// Port of the <c>ensure &lt;resource&gt;</c> line management in
    /// <c>AssignToServer</c>/<c>RemoveFromServer</c> for <c>fivem</c>/<c>redm</c>'s <c>server.cfg</c>.
    /// </summary>
    private static void AddEnsureLine(string resourcesDir, string resourceName)
    {
        var serverCfg = FindServerCfg(resourcesDir);
        if (serverCfg is null)
        {
            return;
        }

        var lines = File.Exists(serverCfg) ? File.ReadAllLines(serverCfg).ToList() : new List<string>();
        var ensureLine = $"ensure {resourceName}";
        if (!lines.Any(l => l.Trim() == ensureLine))
        {
            lines.Add(ensureLine);
            File.WriteAllLines(serverCfg, lines);
        }
    }

    private static void RemoveEnsureLine(string resourcesDir, string resourceName)
    {
        var serverCfg = FindServerCfg(resourcesDir);
        if (serverCfg is null)
        {
            return;
        }

        var ensureLine = $"ensure {resourceName}";
        var lines = (File.Exists(serverCfg) ? File.ReadAllLines(serverCfg) : Array.Empty<string>())
            .Where(l => l.Trim() != ensureLine)
            .ToList();
        File.WriteAllLines(serverCfg, lines);
    }

    /// <summary>
    /// <c>server.cfg</c> lives one level up from <c>resources/</c> in a txAdmin profile
    /// (<c>txData/&lt;profile&gt;/server.cfg</c>); search upward for it.
    /// </summary>
    private static string? FindServerCfg(string resourcesDir)
    {
        var profileDir = Path.GetDirectoryName(resourcesDir);
        if (profileDir is null)
        {
            return null;
        }

        var direct = Path.Combine(profileDir, "server.cfg");
        return File.Exists(direct) ? direct : null;
    }

    /// <summary>Port of <c>CopyDirectory(string, string)</c>.</summary>
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

/// <summary>Outcome of a drag-and-drop install/import (see
/// <see cref="ModLibraryService.InstallFilesToServer"/>).</summary>
public sealed class ModDropResult
{
    public List<string> Added { get; } = new();

    /// <summary>Items that overwrote one with the same name.</summary>
    public List<string> Replaced { get; } = new();

    public List<(string Name, string Reason)> Skipped { get; } = new();

    /// <summary>A jar just added → another jar of the same mod/plugin already in the folder.</summary>
    public List<(string Added, string Other)> Duplicates { get; } = new();

    public int Count => Added.Count + Replaced.Count;
}

/// <summary>
/// Thrown by <see cref="ModLibraryService.AssignToServer"/> when the item isn't compatible with
/// the target server; the message carries the human-readable reason.
/// </summary>
public sealed class ModIncompatibleException : InvalidOperationException
{
    public ModIncompatibleException(string message) : base(message)
    {
    }
}
