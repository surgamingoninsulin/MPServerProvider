using MPServerProvider.Core.Models;

namespace MPServerProvider.App.ViewModels;

public sealed class ServerRowViewModel
{
    public ServerRowViewModel(ServerInstance server, string gameName, ServerState state)
    {
        Server = server;
        Name = server.ServerName;
        Subtitle = gameName;
        State = state;
        IsOnline = state is ServerState.Running or ServerState.Starting;
    }

    public ServerInstance Server { get; }

    public string Name { get; }

    public string Subtitle { get; }

    public ServerState State { get; }

    public bool IsOnline { get; }
}
