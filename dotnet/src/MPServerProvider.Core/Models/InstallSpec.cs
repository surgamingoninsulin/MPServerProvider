namespace MPServerProvider.Core.Models;

public class InstallSpec
{
    public string? Provider { get; set; }

    public List<string> Variants { get; set; } = new();

    public string? Url { get; set; }

    public string? Archive { get; set; }

    public string? GithubRepo { get; set; }

    public string? AssetPattern { get; set; }

    // For a non-GitHub download whose Url contains a "{version}" placeholder: a public page
    // listing every selectable version (newest first) and a regex (with a named "version"
    // group) to pull them out of it, used for both the Version dropdown and the download itself.
    public string? VersionListUrl { get; set; }

    public string? VersionListPattern { get; set; }

    public int? AppId { get; set; }
    public string? Branch { get; set; }

    public bool Anonymous { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Product { get; set; }

    public List<long> WorkshopIds { get; set; } = new();
}
