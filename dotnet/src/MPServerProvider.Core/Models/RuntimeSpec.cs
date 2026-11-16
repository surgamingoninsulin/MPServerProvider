namespace MPServerProvider.Core.Models;

/// <summary>
/// How to launch and stop the installed server.
/// Ported from <c>MPServerProvider.Core.Models.RuntimeSpec</c> (decompile token 0x0200002b).
/// </summary>
public class RuntimeSpec
{
    public string Executable { get; set; } = "{server_file}";

    public string Arguments { get; set; } = string.Empty;

    public string WorkingDir { get; set; } = "{server_root}";

    public string? StopCommand { get; set; }

    public RequiresSpec? Requires { get; set; }
}
