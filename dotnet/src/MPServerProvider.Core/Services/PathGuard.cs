namespace MPServerProvider.Core.Services;

public static class PathGuard
{
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
