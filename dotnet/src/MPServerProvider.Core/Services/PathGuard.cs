namespace MPServerProvider.Core.Services;

/// <summary>
/// Path-containment checks for anything that reads, writes or deletes files on a user's behalf
/// inside a server's root folder (the Server Detail File Manager + built-in editor). Every
/// operation must resolve to a path strictly inside the server root: no <c>..</c> traversal, no
/// sibling-prefix tricks (<c>C:\servers\abc</c> vs <c>C:\servers\abcdef</c>), and no hops through
/// a symlink/junction (reparse point) below the root that could point somewhere else on disk.
/// </summary>
public static class PathGuard
{
    /// <summary>
    /// Pure string check: does <paramref name="candidate"/> resolve (via
    /// <see cref="Path.GetFullPath(string)"/>) to <paramref name="root"/> itself or a path below
    /// it? Comparison is case-insensitive (Windows file systems) and always uses a trailing
    /// separator on the root so <c>root2</c> is never treated as being inside <c>root</c>.
    /// Does not touch the disk.
    /// </summary>
    public static bool IsInsideRoot(string root, string candidate, bool allowRootItself = false)
    {
        if (string.IsNullOrWhiteSpace(root) || string.IsNullOrWhiteSpace(candidate))
        {
            return false;
        }

        string fullRoot;
        string fullCandidate;
        try
        {
            fullRoot = Path.GetFullPath(root);
            // Relative candidates are resolved against the root, not the process's current dir.
            fullCandidate = Path.GetFullPath(candidate, fullRoot);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException or System.Security.SecurityException)
        {
            return false;
        }

        var rootNoSep = Path.TrimEndingDirectorySeparator(fullRoot);
        var candidateNoSep = Path.TrimEndingDirectorySeparator(fullCandidate);

        if (string.Equals(rootNoSep, candidateNoSep, StringComparison.OrdinalIgnoreCase))
        {
            return allowRootItself;
        }

        var rootWithSep = rootNoSep + Path.DirectorySeparatorChar;
        return candidateNoSep.StartsWith(rootWithSep, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// <see cref="IsInsideRoot"/> plus an on-disk check that no existing component between the
    /// root (exclusive) and <paramref name="candidate"/> (inclusive) is a reparse point
    /// (symlink/junction/mount point), which could otherwise redirect a "contained" path to an
    /// arbitrary location. The root itself may be a reparse point (e.g. a server folder that
    /// lives on a junctioned drive) — that's the user's own choice of location, not an escape.
    /// Returns the normalized full path on success.
    /// </summary>
    public static bool TryResolveInsideRoot(string root, string candidate, out string fullPath, bool allowRootItself = false)
    {
        fullPath = string.Empty;
        if (!IsInsideRoot(root, candidate, allowRootItself))
        {
            return false;
        }

        var fullRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
        var resolved = Path.TrimEndingDirectorySeparator(Path.GetFullPath(candidate, fullRoot));

        try
        {
            var current = resolved;
            while (current.Length > fullRoot.Length)
            {
                if (IsReparsePoint(current))
                {
                    return false;
                }
                var parent = Path.GetDirectoryName(current);
                if (string.IsNullOrEmpty(parent) || parent.Length >= current.Length)
                {
                    break;
                }
                current = parent;
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }

        fullPath = resolved;
        return true;
    }

    /// <summary>
    /// Throwing variant of <see cref="TryResolveInsideRoot"/> for call sites that already report
    /// exceptions to the user (e.g. as a toast).
    /// </summary>
    public static string EnsureInsideRoot(string root, string candidate, bool allowRootItself = false)
    {
        if (!TryResolveInsideRoot(root, candidate, out var fullPath, allowRootItself))
        {
            throw new UnauthorizedAccessException("That path is outside this server's folder.");
        }
        return fullPath;
    }

    private static bool IsReparsePoint(string path)
    {
        FileSystemInfo info = Directory.Exists(path) ? new DirectoryInfo(path) : new FileInfo(path);
        if (!info.Exists)
        {
            return false;
        }
        return info.Attributes.HasFlag(FileAttributes.ReparsePoint) || info.LinkTarget is not null;
    }
}
