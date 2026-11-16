namespace MPServerProvider.Core.Models;

public class InstallSpec
{
    public string? Provider { get; set; }

    public List<string> Variants { get; set; } = new();

    public string? Url { get; set; }

    public string? Archive { get; set; }

    public string? GithubRepo { get; set; }

    public string? AssetPattern { get; set; }

    public int? AppId { get; set; }
    public string? Branch { get; set; }

    public bool Anonymous { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Product { get; set; }

    public List<long> WorkshopIds { get; set; } = new();
}
