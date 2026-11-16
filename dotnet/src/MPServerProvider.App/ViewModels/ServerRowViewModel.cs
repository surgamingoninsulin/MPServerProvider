using MPServerProvider.Core.Models;

namespace MPServerProvider.App.ViewModels;

/// <summary>
/// One row in <c>MyServersPage</c>'s left-hand server list. Port of the original's
/// <c>ServerListItemVm</c> (decompile token 0x02000015, see
/// <c>docs/decompile-report/report/MPServerProvider.App.dll/MPServerProvider.App.Views/
/// 0x02000015_ServerListItemVm.md</c>): wraps a <see cref="ServerInstance"/> with the
/// live-derived display strings the list's <c>DataTemplate</c> binds to (name, subtitle, status
/// dot), refreshed on every poll tick rather than only at selection time.
/// </summary>
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
