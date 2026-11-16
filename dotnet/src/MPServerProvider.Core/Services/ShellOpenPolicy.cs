namespace MPServerProvider.Core.Services;

public static class ShellOpenPolicy
{
    private static readonly HashSet<string> ExecutableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".com", ".scr", ".pif", ".cpl", ".msc", ".msi", ".msp", ".mst", ".appx", ".msix", ".appref-ms", ".application", ".gadget",
        ".bat", ".cmd", ".ps1", ".psm1", ".psd1", ".ps1xml", ".vbs", ".vbe", ".js", ".jse", ".wsf", ".wsh", ".wsc", ".hta", ".sct",
        ".jar", ".jnlp", ".lnk", ".url", ".website", ".scf", ".reg", ".inf", ".chm", ".library-ms", ".settingcontent-ms", ".diagcab",
        ".sh", ".py", ".pyw", ".pyz", ".rb", ".pl", ".php", ".dll", ".sys", ".drv", ".ocx",
    };

    public static bool IsUnsafeToShellOpen(string path)
    {
        var ext = Path.GetExtension(path);
        return string.IsNullOrEmpty(ext) || ExecutableExtensions.Contains(ext);
    }
}
