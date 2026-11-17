using CommunityToolkit.Mvvm.ComponentModel;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.ViewModels;

/// <summary>Wraps a <see cref="ModrinthProject"/> search hit with the per-card install state the UI shows.</summary>
public sealed partial class ModrinthResultViewModel : ObservableObject
{
    public ModrinthResultViewModel(ModrinthProject project)
    {
        Project = project;
    }

    public ModrinthProject Project { get; }

    public string Title => Project.Title;

    public string Description => Project.Description;

    public string? IconUrl => Project.IconUrl;

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
