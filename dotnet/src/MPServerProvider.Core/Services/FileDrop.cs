namespace MPServerProvider.Core.Services;

public static class FileDrop
{
    public static void Copy(string source, string target)
    {
        if (Directory.Exists(source))
        {
            if (File.Exists(target))
            {
                File.Delete(target);
            }

            CopyDirectory(new DirectoryInfo(source), target);
        }
        else
        {
            if (Directory.Exists(target))
            {
                Directory.Delete(target, recursive: true);
            }

            File.Copy(source, target, overwrite: true);
        }
    }

    private static void CopyDirectory(DirectoryInfo source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (var entry in source.EnumerateFileSystemInfos())
        {
            if (entry.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                continue;
            }

            var destination = Path.Combine(target, entry.Name);
            if (entry is DirectoryInfo dir)
            {
                CopyDirectory(dir, destination);
            }
            else
            {
                File.Copy(entry.FullName, destination, overwrite: true);
            }
        }
    }
}
