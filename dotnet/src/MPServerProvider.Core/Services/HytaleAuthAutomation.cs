using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Services;

/// <summary>
/// A Hytale dedicated server prints "No server tokens configured. Use /auth login to
/// authenticate." on every boot unless it already has a persisted token. This watches each
/// Hytale server's console, starts the device-code login for it, surfaces the sign-in link/code
/// for the UI to show, and - once signed in - saves the token (`/auth persistence Encrypted`) so
/// the server doesn't ask again on its next restart.
/// </summary>
public sealed class HytaleAuthAutomation
{
    private const int MaxLinesPerServer = 60;

    private readonly ServerProcessService _processes;
    private readonly object _lock = new();
    private readonly Dictionary<string, State> _state = new(StringComparer.OrdinalIgnoreCase);

    public HytaleAuthAutomation(ServerProcessService processes)
    {
        _processes = processes;
        _processes.OutputReceived += OnOutputReceived;
        _processes.StateChanged += OnStateChanged;
    }

    /// <summary>Raised whenever a sign-in link and/or device code appear (or change) in a Hytale server's console.</summary>
    public event Action<ServerInstance, string?, string?>? LoginPromptUpdated;

    /// <summary>Raised once the server reports it signed in; the token has been saved to persist across restarts.</summary>
    public event Action<ServerInstance>? LoginSucceeded;

    private void OnStateChanged(ServerInstance server, ServerState state)
    {
        if (state != ServerState.Starting || !IsHytale(server))
        {
            return;
        }
        lock (_lock)
        {
            _state[server.ServerName] = new State();
        }
    }

    private void OnOutputReceived(ServerInstance server, string line)
    {
        if (!IsHytale(server))
        {
            return;
        }

        string? link = null;
        string? code = null;
        var promptChanged = false;
        var sendLogin = false;
        var sendPersist = false;
        var justSignedIn = false;

        lock (_lock)
        {
            if (!_state.TryGetValue(server.ServerName, out var state))
            {
                state = new State();
                _state[server.ServerName] = state;
            }

            state.Lines.Add(line);
            if (state.Lines.Count > MaxLinesPerServer)
            {
                state.Lines.RemoveAt(0);
            }

            if (!state.LoginRequested && line.Contains("No server tokens configured", StringComparison.OrdinalIgnoreCase))
            {
                state.LoginRequested = true;
                sendLogin = true;
            }

            var scan = HytaleAuth.Scan(state.Lines);
            if (scan.Link != state.LastLink || scan.Code != state.LastCode)
            {
                state.LastLink = scan.Link;
                state.LastCode = scan.Code;
                if (scan.Link is not null || scan.Code is not null)
                {
                    link = scan.Link;
                    code = scan.Code;
                    promptChanged = true;
                }
            }

            if (scan.SignedIn && !state.WasSignedIn)
            {
                state.WasSignedIn = true;
                justSignedIn = true;
                if (!state.PersistRequested)
                {
                    state.PersistRequested = true;
                    sendPersist = true;
                }
            }
            else if (!scan.SignedIn)
            {
                state.WasSignedIn = false;
            }
        }

        if (sendLogin)
        {
            _processes.SendCommand(server, HytaleAuth.LoginCommand);
        }
        if (promptChanged)
        {
            LoginPromptUpdated?.Invoke(server, link, code);
        }
        if (sendPersist)
        {
            _processes.SendCommand(server, HytaleAuth.PersistCommand);
        }
        if (justSignedIn)
        {
            LoginSucceeded?.Invoke(server);
        }
    }

    private static bool IsHytale(ServerInstance server) =>
        string.Equals(server.ServerGame, HytaleAuth.GameId, StringComparison.OrdinalIgnoreCase);

    private sealed class State
    {
        public List<string> Lines { get; } = new();
        public bool LoginRequested { get; set; }
        public bool PersistRequested { get; set; }
        public bool WasSignedIn { get; set; }
        public string? LastLink { get; set; }
        public string? LastCode { get; set; }
    }
}
