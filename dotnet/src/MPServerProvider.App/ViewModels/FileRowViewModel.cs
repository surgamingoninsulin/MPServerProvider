using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

/// <summary>
/// One row in the Server Detail "File Manager" tab's listing. Port of the original's
/// <c>FileEntryVm</c> (decompile token 0x02000017, see
/// <c>docs/decompile-report/report/MPServerProvider.App.dll/MPServerProvider.App.Views/
/// 0x02000017_FileEntryVm.md</c>), simplified to a flat per-directory listing like the proven
/// Rust reference (<c>crates/mpsp-app/src/pages/server_detail.rs</c>'s <c>FileRowData</c>/
/// <c>human_readable_size</c>).
/// </summary>
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

    /// <summary>Not a directory and not on the known-binary list (content is still sniffed
    /// before the built-in editor actually opens it).</summary>
    public bool IsEditableCandidate => !IsDirectory && !TextFileDetector.IsKnownBinaryExtension(Name);

    /// <summary>Icon glyph (codepoints present in both Segoe Fluent Icons and Segoe MDL2 Assets):
    /// folder, text document, or generic/binary file.</summary>
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
