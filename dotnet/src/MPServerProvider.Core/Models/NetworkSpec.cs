namespace MPServerProvider.Core.Models;

public class NetworkSpec
{
    public int DefaultPort { get; set; }

    public string Protocol { get; set; } = "both";

    public string Query { get; set; } = "none";

    public string ConsoleMethod { get; set; } = "stdin";
}
