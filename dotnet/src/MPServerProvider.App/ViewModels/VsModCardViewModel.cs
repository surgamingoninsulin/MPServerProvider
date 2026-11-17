using CommunityToolkit.Mvvm.ComponentModel;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.ViewModels;

/// <summary>Wraps a <see cref="VsModDbProject"/> search hit with the per-card install state the UI shows.</summary>
public sealed partial class VsModCardViewModel : ObservableObject
{
    private readonly VsModDbService _modDb;
    private readonly string? _serverVersion;
    private bool _compatLoadStarted;

    public VsModCardViewModel(VsModDbProject project, VsModDbService modDb, string? serverVersion)
    {
        Project = project;
        _modDb = modDb;
        _serverVersion = serverVersion;
    }

    public VsModDbProject Project { get; }

    public string Title => Project.Title;

    public string Summary => Project.Summary;

    public string? LogoUrl => Project.LogoUrl;

    public string DownloadsText => Project.Downloads >= 1_000_000
        ? $"{Project.Downloads / 1_000_000.0:0.#}M downloads"
        : Project.Downloads >= 1_000
            ? $"{Project.Downloads / 1_000.0:0.#}K downloads"
            : $"{Project.Downloads} downloads";

    [ObservableProperty]
    private bool _isInstalling;

    [ObservableProperty]
    private bool _isInstalled;

    [ObservableProperty]
    private string? _latestVersionText;

    [ObservableProperty]
    private string? _compatStatusText;

    [ObservableProperty]
    private bool _isCompatWarning;

    /// <summary>
    /// Fetches the mod's latest version and whether it's confirmed for this server's Vintage
    /// Story version. Called from the card's Loaded event, so with a virtualized list this only
    /// fires for cards actually scrolled into view, instead of hitting the API for every result
    /// up front.
    /// </summary>
    public async Task EnsureCompatibilityLoadedAsync()
    {
        if (_compatLoadStarted)
        {
            return;
        }
        _compatLoadStarted = true;

        try
        {
            var info = await _modDb.GetCompatibilityAsync(Project.ModIdStr, _serverVersion);
            if (info is null)
            {
                return;
            }

            LatestVersionText = string.IsNullOrWhiteSpace(info.LatestVersion) ? null : $"Latest: v{info.LatestVersion}";
            switch (info.ConfirmedForServerVersion)
            {
                case true:
                    CompatStatusText = "✓ Confirmed for your server";
                    IsCompatWarning = false;
                    break;
                case false:
                    CompatStatusText = "⚠ Not confirmed — may be broken";
                    IsCompatWarning = true;
                    break;
                default:
                    // Server's game version isn't known; nothing to compare against.
                    break;
            }
        }
        catch
        {
            // Best-effort badge; leave it blank on failure rather than disrupting the list.
            _compatLoadStarted = false;
        }
    }
}
