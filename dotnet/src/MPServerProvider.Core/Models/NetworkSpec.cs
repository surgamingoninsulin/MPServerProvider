namespace MPServerProvider.Core.Models;

/// <summary>
/// Networking defaults (port, protocol, query method, console input method).
/// Ported from <c>MPServerProvider.Core.Models.NetworkSpec</c> (decompile token 0x0200002d).
/// </summary>
public class NetworkSpec
{
    public int DefaultPort { get; set; }

    public string Protocol { get; set; } = "both";

    public string Query { get; set; } = "none";

    public string ConsoleMethod { get; set; } = "stdin";
}
