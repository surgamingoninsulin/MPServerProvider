namespace MPServerProvider.Core.Services;

/// <summary>
/// Decides whether a file from a server folder may be handed to the Windows shell's default
/// "open" verb. Server folders contain untrusted content (downloaded mods, plugins, workshop
/// items) and the app often runs elevated, so anything that would *execute* (programs, scripts,
/// shortcuts, installers, registry files, Java archives) must never be shell-opened — the File
/// Manager reveals those in Explorer instead.
/// </summary>
public static class ShellOpenPolicy
{
    private static readonly HashSet<string> ExecutableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".com", ".scr", ".pif", ".cpl", ".msc", ".msi", ".msp", ".mst", ".appx", ".msix", ".appref-ms", ".application", ".gadget",
        ".bat", ".cmd", ".ps1", ".psm1", ".psd1", ".ps1xml", ".vbs", ".vbe", ".js", ".jse", ".wsf", ".wsh", ".wsc", ".hta", ".sct",
        ".jar", ".jnlp", ".lnk", ".url", ".website", ".scf", ".reg", ".inf", ".chm", ".library-ms", ".settingcontent-ms", ".diagcab",
        ".sh", ".py", ".pyw", ".pyz", ".rb", ".pl", ".php", ".dll", ".sys", ".drv", ".ocx",
    };

    /// <summary>True when shell-opening <paramref name="path"/> could run code (or the file has no
    /// extension, so its handler is unknown).</summary>
    public static bool IsUnsafeToShellOpen(string path)
    {
        var ext = Path.GetExtension(path);
        return string.IsNullOrEmpty(ext) || ExecutableExtensions.Contains(ext);
    }
}
