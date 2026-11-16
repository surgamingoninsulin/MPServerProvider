using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Providers.ManualProvider</c> (decompile token
/// 0x02000022). "Installs" by copying an already-downloaded/extracted server folder
/// (<see cref="InstallRequest.Version"/>, repurposed to carry the source path) into
/// <see cref="InstallRequest.TargetDir"/>.
/// </summary>
public sealed class ManualProvider : IInstallProvider
{
    /// <summary>
    /// Well-known dedicated-server executable names the decompiled <c>InstallAsync</c> body's
    /// string-literal array searches for, in priority order, after copying a user-selected
    /// source folder.
    /// </summary>
    private static readonly string[] KnownServerExeNames =
    {
        "server.exe",
        "srcds.exe",
        "FXServer.exe",
        "valheim_server.exe",
        "RustDedicated.exe",
        "eurotrucks2_server.exe",
        "amtrucks_server.exe",
    };

    public string Id => "manual";

    public Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct)
        => Task.FromResult(new List<string> { "manual" });

    /// <remarks>
    /// The (potentially multi-GB) copy runs on a thread-pool thread — it used to run
    /// synchronously on the caller's (UI) thread and freeze the window — and honours
    /// <paramref name="ct"/> between files.
    /// </remarks>
    public Task<InstallResult> InstallAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
        => Task.Run(() => Install(request, progress, ct), ct);

    private static InstallResult Install(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var source = request.Version ?? string.Empty;
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new InvalidOperationException("Manual install requires a source folder path.");
        }

        if (!Directory.Exists(source))
        {
            throw new InvalidOperationException($"Source folder not found: {source}");
        }

        var sourceFull = Path.TrimEndingDirectorySeparator(Path.GetFullPath(source)) + Path.DirectorySeparatorChar;
        var targetFull = Path.TrimEndingDirectorySeparator(Path.GetFullPath(request.TargetDir)) + Path.DirectorySeparatorChar;
        if (targetFull.StartsWith(sourceFull, StringComparison.OrdinalIgnoreCase))
        {
            // Copying a folder into itself would recurse forever.
            throw new InvalidOperationException("The source folder can't contain the new server's folder. Pick the server files folder itself.");
        }

        progress.Report(InstallProgress.Indeterminate("Copying server files…"));

        CopyDirRecursive(source, source, request.TargetDir, ct);

        // Matches the original's call into `DownloadService.FlattenSingleRootFolder` right
        // after copying (handles a source folder that itself wraps everything in one
        // subfolder, e.g. a previously-extracted zip).
        DownloadHelper.FlattenSingleRootFolder(request.TargetDir);

        var serverFile = FindKnownServerExe(request.TargetDir) ?? string.Empty;

        return new InstallResult
        {
            ServerFile = serverFile,
            Version = "manual",
            StartCommand = null,
            JavaMajor = 0,
        };
    }

    /// <summary>
    /// Recursively copies the contents of <paramref name="sourceRoot"/> into
    /// <paramref name="destRoot"/>, mirroring the decompiled body's
    /// <c>EnumerateDirectories</c>/<c>EnumerateFiles</c> walk plus
    /// <c>Path.GetRelativePath</c>/<c>File.Copy</c>, checking cancellation before each file copy
    /// (<c>ct.ThrowIfCancellationRequested()</c>).
    /// </summary>
    private static void CopyDirRecursive(string sourceRoot, string dir, string destRoot, CancellationToken ct)
    {
        Directory.CreateDirectory(destRoot);

        foreach (var path in Directory.EnumerateFileSystemEntries(dir))
        {
            var rel = Path.GetRelativePath(sourceRoot, path);
            var destPath = Path.Combine(destRoot, rel);

            if (Directory.Exists(path))
            {
                Directory.CreateDirectory(destPath);
                CopyDirRecursive(sourceRoot, path, destRoot, ct);
            }
            else
            {
                ct.ThrowIfCancellationRequested();
                var parent = Path.GetDirectoryName(destPath);
                if (!string.IsNullOrEmpty(parent))
                {
                    Directory.CreateDirectory(parent);
                }

                File.Copy(path, destPath, overwrite: true);
            }
        }
    }

    /// <summary>
    /// Finds the first file under <paramref name="dir"/> (recursively) whose name matches one of
    /// <see cref="KnownServerExeNames"/>, case-insensitively, in priority order, and returns its
    /// path relative to <paramref name="dir"/>.
    /// </summary>
    internal static string? FindKnownServerExe(string dir)
    {
        var allFiles = new List<string>();
        CollectFiles(dir, allFiles);

        foreach (var candidate in KnownServerExeNames)
        {
            var found = allFiles.FirstOrDefault(p =>
                string.Equals(Path.GetFileName(p), candidate, StringComparison.OrdinalIgnoreCase));
            if (found is not null)
            {
                return Path.GetRelativePath(dir, found);
            }
        }

        return null;
    }

    private static void CollectFiles(string dir, List<string> output)
    {
        if (!Directory.Exists(dir))
        {
            return;
        }

        foreach (var entry in Directory.EnumerateFileSystemEntries(dir))
        {
            if (Directory.Exists(entry))
            {
                CollectFiles(entry, output);
            }
            else
            {
                output.Add(entry);
            }
        }
    }
}
