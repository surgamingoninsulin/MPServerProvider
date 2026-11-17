using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

public sealed class ManualProvider : IInstallProvider
{
    private static readonly string[] KnownServerExeNames =
    {
        "server.exe",
        "srcds.exe",
        "FXServer.exe",
        "valheim_server.exe",
        "RustDedicated.exe",
        "eurotrucks2_server.exe",
        "amtrucks_server.exe",
        "TerrariaServer.exe",
        "VintagestoryServer.exe",
    };

    public string Id => "manual";

    public Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct)
        => Task.FromResult(new List<string> { "manual" });

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

        // Flatten right after copying (handles a source folder that itself wraps everything
        // in one subfolder, e.g. a previously-extracted zip).
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
