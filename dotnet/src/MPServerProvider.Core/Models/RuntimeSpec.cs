namespace MPServerProvider.Core.Models;

public class RuntimeSpec
{
    public string Executable { get; set; } = "{server_file}";

    public string Arguments { get; set; } = string.Empty;

    public string WorkingDir { get; set; } = "{server_root}";

    public string? StopCommand { get; set; }

    public RequiresSpec? Requires { get; set; }
}
