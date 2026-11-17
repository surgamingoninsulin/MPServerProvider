using CommunityToolkit.Mvvm.ComponentModel;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.ViewModels;

/// <summary>Wraps a <see cref="VsModDbProject"/> search hit with the per-card install state the UI shows.</summary>
public sealed partial class VsModCardViewModel : ObservableObject
{
    public VsModCardViewModel(VsModDbProject project)
    {
        Project = project;
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
}
