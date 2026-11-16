using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

public sealed partial class LuckPermsTabViewModel : ObservableObject
{
    private static readonly TimeSpan EditorLinkTimeout = TimeSpan.FromSeconds(30);

    private static readonly TimeSpan DetectInterval = TimeSpan.FromSeconds(5);

    private readonly AppServices _services;
    private readonly ToastService _toasts;
    private readonly Dispatcher _dispatcher;
    private readonly DispatcherTimer _linkTimeout;
    private DateTime _lastDetect = DateTime.MinValue;
    private string? _detectedFor;

    public LuckPermsTabViewModel(AppServices services, ToastService toasts)
    {
        _services = services;
        _toasts = toasts;
        _dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        _linkTimeout = new DispatcherTimer(DispatcherPriority.Normal, _dispatcher) { Interval = EditorLinkTimeout };
        _linkTimeout.Tick += (_, _) => OnLinkTimeout();

        // Lives as long as the (cached) server detail view model, i.e. the app: never unsubscribed.
        _services.Processes.OutputReceived += OnOutputReceived;
    }

    public ServerInstance? Current { get; private set; }

    [ObservableProperty]
    private bool _isAvailable;

    [ObservableProperty]
    private string _jarName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenEditorCommand))]
    private bool _isServerRunning;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasEditorUrl))]
    private string _editorUrl = string.Empty;

    public bool HasEditorUrl => EditorUrl.Length > 0;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenEditorCommand))]
    private bool _isWaitingForLink;

    [ObservableProperty]
    private string _statusText = "Start the server, then press Open editor.";

    [ObservableProperty]
    private bool _isLive;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasApplyCode))]
    private string _applyCode = string.Empty;

    public bool HasApplyCode => ApplyCode.Length > 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasTrustNonce))]
    private string _trustNonce = string.Empty;

    public bool HasTrustNonce => TrustNonce.Length > 0;

    private string _applyAlias = "lp";
    private string _trustAlias = "lp";

    public void SetServer(ServerInstance? server)
    {
        var isSame = server is not null && Current?.ServerName == server.ServerName;
        Current = server;
        if (!isSame)
        {
            ResetSession("Start the server, then press Open editor.");
            _detectedFor = null;
        }

        RefreshLive();
    }

    public void RefreshLive()
    {
        if (Current is not { } server)
        {
            IsAvailable = false;
            IsServerRunning = false;
            return;
        }

        var running = _services.Processes.StateOf(server) == ServerState.Running;
        if (IsServerRunning && !running && (HasEditorUrl || IsWaitingForLink))
        {
            ResetSession("The server stopped, so the editor session ended. Start it again to open a new one.");
        }

        IsServerRunning = running;
        if (!HasEditorUrl && !IsWaitingForLink && !HasApplyCode)
        {
            StatusText = running
                ? "Press Open editor to edit groups, users and permissions in the LuckPerms web editor."
                : "Start the server to open the LuckPerms web editor (it needs the server running).";
        }

        if (_detectedFor != server.ServerName || DateTime.UtcNow - _lastDetect >= DetectInterval)
        {
            _detectedFor = server.ServerName;
            _lastDetect = DateTime.UtcNow;
            try
            {
                var jar = LuckPermsSupport.FindJar(server, _services.Games.Find(server.ServerGame));
                IsAvailable = jar is not null;
                JarName = jar is null ? string.Empty : System.IO.Path.GetFileName(jar);
            }
            catch
            {
                // Best-effort; keep the last answer.
            }
        }
    }

    private void ResetSession(string status)
    {
        _linkTimeout.Stop();
        IsWaitingForLink = false;
        EditorUrl = string.Empty;
        IsLive = false;
        ApplyCode = string.Empty;
        TrustNonce = string.Empty;
        StatusText = status;
    }

    private bool CanOpenEditor() => !IsWaitingForLink;

    [RelayCommand(CanExecute = nameof(CanOpenEditor))]
    private void OpenEditor()
    {
        if (Current is not { } server)
        {
            return;
        }

        try
        {
            var state = _services.Processes.StateOf(server);
            if (state != ServerState.Running)
            {
                var message = state == ServerState.Starting
                    ? "The server is still starting. Open the editor once it has finished loading."
                    : "The server isn't running. Start it first: LuckPerms creates editor sessions from the running server.";
                StatusText = message;
                _toasts.Warning(message);
                return;
            }

            EditorUrl = string.Empty;
            ApplyCode = string.Empty;
            TrustNonce = string.Empty;
            IsLive = false;
            IsWaitingForLink = true;
            StatusText = "Asked LuckPerms for a new editor session, waiting for the link…";
            _linkTimeout.Stop();
            _linkTimeout.Start();
            _services.Processes.SendCommand(server, "lp editor");
        }
        catch (Exception ex)
        {
            IsWaitingForLink = false;
            _toasts.Error($"Could not open the LuckPerms editor: {ex.Message}");
        }
    }

    private void OnLinkTimeout()
    {
        _linkTimeout.Stop();
        if (!IsWaitingForLink)
        {
            return;
        }

        IsWaitingForLink = false;
        StatusText = "LuckPerms didn't print an editor link within 30 seconds. Check the Console tab: the server needs internet " +
                     "access to reach luckperms.net / bytebin, and LuckPerms must have finished loading.";
        _toasts.Warning("No LuckPerms editor link received. See the Console tab for details.");
    }

    [RelayCommand]
    private void ApplyChanges()
    {
        if (Current is not { } server || !HasApplyCode)
        {
            return;
        }

        try
        {
            if (_services.Processes.StateOf(server) != ServerState.Running)
            {
                _toasts.Warning("Start the server to apply the editor changes (the code stays valid for a while).");
                return;
            }

            if (LuckPermsSupport.BuildCommand(_applyAlias, "applyedits", ApplyCode) is not { } command)
            {
                _toasts.Error("The editor's apply code doesn't look valid. Copy the command from the editor into the Console tab instead.");
                return;
            }

            _services.Processes.SendCommand(server, command);
            ApplyCode = string.Empty;
            StatusText = "Sent the changes to LuckPerms. To keep editing, press Open editor for a fresh session " +
                         "(a session shouldn't be saved twice).";
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not apply the changes: {ex.Message}");
        }
    }

    [RelayCommand]
    private void TrustEditor()
    {
        if (Current is not { } server || !HasTrustNonce)
        {
            return;
        }

        try
        {
            var confirmed = Views.Dialogs.ConfirmDialog.Show(
                Application.Current.MainWindow,
                "Trust Editor",
                $"An editor window (session id {TrustNonce}) wants to apply changes directly to this server. Trust it only if it's " +
                "the editor you just opened here. LuckPerms remembers this browser for future sessions.",
                confirmText: "Trust",
                destructive: false);
            if (!confirmed)
            {
                return;
            }

            if (LuckPermsSupport.BuildCommand(_trustAlias, "trusteditor", TrustNonce) is not { } command)
            {
                _toasts.Error("The session id doesn't look valid.");
                return;
            }

            _services.Processes.SendCommand(server, command);
            TrustNonce = string.Empty;
            StatusText = "Trust sent. The editor connects live in a moment.";
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not trust the editor: {ex.Message}");
        }
    }

    [RelayCommand]
    private void CopyLink()
    {
        try
        {
            if (HasEditorUrl)
            {
                Clipboard.SetText(EditorUrl);
                _toasts.Info("Editor link copied.");
            }
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not copy the link: {ex.Message}");
        }
    }

    [RelayCommand]
    private void OpenInBrowser()
    {
        try
        {
            if (HasEditorUrl)
            {
                Views.ServerTabs.EmbeddedBrowser.OpenExternal(EditorUrl);
            }
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not open the browser: {ex.Message}");
        }
    }

    public void OnEditorPageCommand(string kind, string alias, string code)
    {
        if (LuckPermsSupport.BuildCommand(alias, kind, code) is null)
        {
            return;
        }

        if (kind == "applyedits" && code != ApplyCode)
        {
            _applyAlias = alias.ToLowerInvariant();
            ApplyCode = code;
            StatusText = IsLive
                ? "The editor couldn't send the changes live. Press Apply changes to send them from the console."
                : "Changes saved in the editor. Press Apply changes to send them to the server.";
        }
        else if (kind == "trusteditor" && code != TrustNonce && !IsLive)
        {
            _trustAlias = alias.ToLowerInvariant();
            TrustNonce = code;
            StatusText = "LuckPerms doesn't know this editor yet. Press Trust editor to let it apply changes live.";
        }
    }

    private void OnOutputReceived(ServerInstance server, string line)
    {
        // Pump thread: cheap filters first, UI work on the dispatcher.
        if (Current is not { } current || server.ServerName != current.ServerName || !IsAvailable)
        {
            return;
        }

        var parsed = LuckPermsSupport.Parse(line);
        if (parsed.Kind == LuckPermsEventKind.None)
        {
            return;
        }

        _dispatcher.BeginInvoke(() =>
        {
            try
            {
                if (Current?.ServerName == server.ServerName)
                {
                    Handle(parsed);
                }
            }
            catch
            {
                // Console parsing is best-effort and must never surface as an error.
            }
        });
    }

    private void Handle(LuckPermsEvent e)
    {
        switch (e.Kind)
        {
            case LuckPermsEventKind.Preparing:
                if (IsWaitingForLink)
                {
                    StatusText = "LuckPerms is preparing the editor session…";
                }

                break;

            case LuckPermsEventKind.EditorUrl:
                // Also picks up an "lp editor" typed in the Console tab.
                _linkTimeout.Stop();
                IsWaitingForLink = false;
                IsLive = false;
                ApplyCode = string.Empty;
                TrustNonce = string.Empty;
                EditorUrl = e.Value;
                StatusText = "Editor loaded. Make your changes, then Save in the editor.";
                break;

            case LuckPermsEventKind.SocketConnected:
            case LuckPermsEventKind.Trusted when IsLive:
                IsLive = true;
                TrustNonce = string.Empty;
                StatusText = "Live mode: the editor is connected to the server, so Save applies the changes immediately.";
                break;

            case LuckPermsEventKind.Trusted:
                TrustNonce = string.Empty;
                StatusText = "Editor trusted. It connects live in a moment.";
                break;

            case LuckPermsEventKind.TrustRequired:
                if (e.Value.Length > 0)
                {
                    OnEditorPageCommand("trusteditor", e.Alias, e.Value);
                }

                break;

            case LuckPermsEventKind.ChangesApplied:
                ApplyCode = string.Empty;
                StatusText = IsLive
                    ? "Changes applied live. Keep editing, or Save again for more changes."
                    : "Changes applied. Press Open editor for a fresh session to continue editing.";
                _toasts.Success("LuckPerms applied the editor changes.");
                break;

            case LuckPermsEventKind.NoChanges:
                StatusText = "LuckPerms says the editor data had no changes to apply.";
                break;

            case LuckPermsEventKind.Error:
                if (IsWaitingForLink)
                {
                    _linkTimeout.Stop();
                    IsWaitingForLink = false;
                }

                StatusText = $"LuckPerms: {e.Value}";
                _toasts.Warning("LuckPerms reported a problem with the web editor. See the Console tab.");
                break;
        }
    }
}
