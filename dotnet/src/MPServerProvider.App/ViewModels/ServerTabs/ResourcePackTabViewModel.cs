using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

public sealed partial class ResourcePackTabViewModel : ObservableObject
{
    public const string SiteUrl = "https://mcpacks.dev/";

    private readonly AppServices _services;
    private readonly ToastService _toasts;

    private string _loadedPromptJson = string.Empty;
    private string _loadedPromptText = string.Empty;

    public ResourcePackTabViewModel(AppServices services, ToastService toasts)
    {
        _services = services;
        _toasts = toasts;
    }

    public ServerInstance? Current { get; private set; }

    [ObservableProperty]
    private bool _isAvailable;

    [ObservableProperty]
    private bool _isServerRunning;

    // -- Form ---------------------------------------------------------------------------------

    [ObservableProperty]
    private string _packUrl = string.Empty;

    [ObservableProperty]
    private string _packSha1 = string.Empty;

    [ObservableProperty]
    private bool _requirePack;

    [ObservableProperty]
    private string _promptText = string.Empty;

    [ObservableProperty]
    private string _captureText = "Upload your pack on the left: its download link and SHA-1 are filled in here automatically.";

    [ObservableProperty]
    private bool _hasCapture;

    [ObservableProperty]
    private string _versionWarning = string.Empty;

    [ObservableProperty]
    private string _styledPromptNote = string.Empty;

    // -- Current server.properties --------------------------------------------------------------

    [ObservableProperty]
    private string _currentUrlText = "—";

    [ObservableProperty]
    private string _currentSha1Text = "—";

    [ObservableProperty]
    private string _currentRequiredText = "—";

    [ObservableProperty]
    private string _currentPromptText = "—";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowRestartButton))]
    private bool _isRestartNeeded;

    public bool ShowRestartButton => IsRestartNeeded && IsServerRunning;

    partial void OnIsServerRunningChanged(bool value) => OnPropertyChanged(nameof(ShowRestartButton));

    public void SetServer(ServerInstance? server)
    {
        var isSame = server is not null && Current?.ServerName == server.ServerName;
        Current = server;
        IsAvailable = server?.ServerGame == "minecraft-java";
        if (!isSame)
        {
            IsRestartNeeded = false;
            HasCapture = false;
            CaptureText = "Upload your pack on the left: its download link and SHA-1 are filled in here automatically.";
            if (server is not null && IsAvailable)
            {
                LoadFromDisk(fillForm: true);
            }
        }

        RefreshLive();
    }

    public void RefreshLive()
    {
        if (Current is not { } server)
        {
            IsServerRunning = false;
            return;
        }

        IsServerRunning = _services.Processes.StateOf(server) is ServerState.Running or ServerState.Starting;
        VersionWarning = ResourcePackProperties.SupportsRequiredPrompt(server.ServerVersion) == false
            ? $"This server runs {server.ServerVersion}: \"require\" and the prompt message need Minecraft 1.17 or newer and are ignored by older versions."
            : string.Empty;
    }

    public void OnPackCaptured(HostedPackInfo pack)
    {
        if (string.Equals(PackUrl.Trim(), pack.DownloadUrl, StringComparison.Ordinal) &&
            string.Equals(PackSha1.Trim(), pack.Sha1, StringComparison.OrdinalIgnoreCase) && HasCapture)
        {
            return; // same pack reported again by the page poll
        }

        PackUrl = pack.DownloadUrl;
        PackSha1 = pack.Sha1;
        HasCapture = true;
        CaptureText = pack.FileName.Length > 0
            ? $"Captured \"{pack.FileName}\" from mcpacks.dev. Review the options below, then press Apply."
            : "Captured the pack link and SHA-1 from mcpacks.dev. Review the options below, then press Apply.";
        _toasts.Success("Resource pack link and SHA-1 captured from mcpacks.dev.");
    }

    private void LoadFromDisk(bool fillForm)
    {
        if (Current is not { } server)
        {
            return;
        }

        var settings = ResourcePackProperties.Read(server.ServerRoot);
        var promptText = ResourcePackProperties.PromptTextFromJson(settings.PromptJson);
        CurrentUrlText = settings.Url.Length > 0 ? settings.Url : "(none)";
        CurrentSha1Text = settings.Sha1.Length > 0 ? settings.Sha1 : "(none)";
        CurrentRequiredText = settings.Required ? "yes" : "no";
        CurrentPromptText = settings.PromptJson.Length > 0 ? promptText : "(default)";

        _loadedPromptJson = settings.PromptJson;
        _loadedPromptText = promptText;
        StyledPromptNote = ResourcePackProperties.IsStyledPrompt(settings.PromptJson)
            ? "The current prompt uses formatting (colors/styles). It is kept as-is unless you change the text here."
            : string.Empty;

        if (fillForm)
        {
            PackUrl = settings.Url;
            PackSha1 = settings.Sha1;
            RequirePack = settings.Required;
            PromptText = promptText;
        }
    }

    private string PromptJsonForSave()
        => PromptText.Trim() == _loadedPromptText.Trim() ? _loadedPromptJson : ResourcePackProperties.PromptJsonFromText(PromptText);

    [RelayCommand]
    private void Reload()
    {
        try
        {
            LoadFromDisk(fillForm: true);
            HasCapture = false;
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not read server.properties: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Apply()
    {
        if (Current is not { } server)
        {
            return;
        }

        try
        {
            var settings = new ResourcePackSettings(PackUrl, PackSha1, RequirePack, PromptJsonForSave());
            if (ResourcePackProperties.Validate(settings) is { } error)
            {
                _toasts.Warning(error);
                return;
            }

            if (settings.Url.Trim().Length > 0 && settings.Sha1.Trim().Length == 0)
            {
                // Allowed by Minecraft, but players then re-download the pack on every join.
                _toasts.Info("No SHA-1 set: players will re-download the pack every time they join.");
            }

            ResourcePackProperties.Apply(server.ServerRoot, settings);
            LoadFromDisk(fillForm: true);
            IsRestartNeeded = true;
            _toasts.Success(IsServerRunning
                ? "Resource pack saved to server.properties. Restart the server to use it."
                : "Resource pack saved to server.properties. It is used from the next start.");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not update server.properties: {ex.Message}");
        }
    }

    [RelayCommand]
    private void RemovePack()
    {
        if (Current is not { } server)
        {
            return;
        }

        try
        {
            var confirmed = Views.Dialogs.ConfirmDialog.Show(
                Application.Current.MainWindow,
                "Remove Resource Pack",
                "Remove the server resource pack? This clears resource-pack, resource-pack-sha1, require-resource-pack and " +
                "resource-pack-prompt in server.properties (the file on mcpacks.dev is not deleted).",
                confirmText: "Remove");
            if (!confirmed)
            {
                return;
            }

            ResourcePackProperties.Remove(server.ServerRoot);
            LoadFromDisk(fillForm: true);
            HasCapture = false;
            IsRestartNeeded = true;
            _toasts.Success(IsServerRunning
                ? "Resource pack removed. Restart the server for players to stop getting it."
                : "Resource pack removed from server.properties.");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not update server.properties: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ComputeSha1()
    {
        try
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Choose the resource pack .zip",
                Filter = "Resource pack (*.zip)|*.zip|All files (*.*)|*.*",
                CheckFileExists = true,
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            var path = dialog.FileName;
            var hash = await Task.Run(() => ResourcePackProperties.ComputeSha1(path));
            PackSha1 = hash;
            _toasts.Success($"SHA-1 of {Path.GetFileName(path)} computed. Make sure it's the same file as the one at the pack URL.");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not compute the SHA-1: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Restart()
    {
        if (Current is not { } server)
        {
            return;
        }

        try
        {
            await _services.Processes.RestartAsync(server);
            IsRestartNeeded = false;
            _toasts.Success($"Restarting {server.ServerName}…");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not restart {server.ServerName}: {ex.Message}");
        }

        RefreshLive();
    }

    [RelayCommand]
    private void CopyServerProperties()
    {
        try
        {
            var file = JavaPropertiesFile.Parse(string.Empty);
            ResourcePackProperties.Apply(file, new ResourcePackSettings(PackUrl, PackSha1, RequirePack, PromptJsonForSave()));
            Clipboard.SetText(file.ToString());
            _toasts.Info("server.properties lines copied.");
        }
        catch (Exception ex)
        {
            _toasts.Error($"Could not copy: {ex.Message}");
        }
    }

    public void ReportError(string what, Exception ex)
    {
        try
        {
            _toasts.Error($"{what}: {ex.Message}");
        }
        catch
        {
            // Reporting must never throw out of an event handler.
        }
    }
}
