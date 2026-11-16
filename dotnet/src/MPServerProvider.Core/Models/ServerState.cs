namespace MPServerProvider.Core.Models;

public enum ServerState
{
    Stopped,
    Starting,
    Running,
    Stopping,
    Crashed,
    Installing,
}
