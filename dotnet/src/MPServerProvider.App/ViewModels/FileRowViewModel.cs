using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

public sealed class FileRowViewModel
{
    public FileRowViewModel(string name, bool isDirectory, string sizeLabel, string fullPath)
    {
        Name = name;
        IsDirectory = isDirectory;
        SizeLabel = sizeLabel;
        FullPath = fullPath;
    }

    public string Name { get; }

    public bool IsDirectory { get; }

    public string SizeLabel { get; }

    public string FullPath { get; }

    public bool IsEditableCandidate => !IsDirectory && !TextFileDetector.IsKnownBinaryExtension(Name);

    public string Icon => IsDirectory ? "" : IsEditableCandidate ? "" : "";

    public string OpenToolTip => IsDirectory
        ? "Open folder"
        : IsEditableCandidate ? "Edit in the built-in editor" : "Show in Explorer";

    public static string HumanReadableSize(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB" };
        double size = bytes;
        var unit = 0;
        while (size >= 1024.0 && unit < units.Length - 1)
        {
            size /= 1024.0;
            unit++;
        }
        return unit == 0 ? $"{bytes} {units[unit]}" : $"{size:0.#} {units[unit]}";
    }
}
