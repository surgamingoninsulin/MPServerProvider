using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

public interface IInstallProvider
{
    string Id { get; }

    Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct);

    Task<InstallResult> InstallAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct);
}
