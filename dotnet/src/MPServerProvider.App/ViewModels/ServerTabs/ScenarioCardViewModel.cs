using CommunityToolkit.Mvvm.ComponentModel;
using MPServerProvider.Core.Models;

namespace MPServerProvider.App.ViewModels;

/// <summary>Wraps a <see cref="ScenarioListing"/> with the per-card download state the UI shows.</summary>
public sealed partial class ScenarioCardViewModel : ObservableObject
{
    public ScenarioCardViewModel(ScenarioListing listing)
    {
        Listing = listing;
    }

    public ScenarioListing Listing { get; }

    public string Title => Listing.Title;

    public string Author => Listing.Author;

    public string? ThumbnailUrl => Listing.ThumbnailUrl;

    [ObservableProperty]
    private bool _isDownloading;

    [ObservableProperty]
    private bool _isDownloaded;
}
