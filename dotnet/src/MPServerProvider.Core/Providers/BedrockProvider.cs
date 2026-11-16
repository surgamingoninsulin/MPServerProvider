using System.Text.Json;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

/// <summary>
/// Ported from <c>MPServerProvider.Core.Providers.BedrockProvider</c> (decompile token
/// 0x0200001d). Installs the official Minecraft Bedrock dedicated server by resolving the
/// current download URL from Mojang's links API and extracting the zip.
/// </summary>
public sealed class BedrockProvider : IInstallProvider
{
    /// <summary><c>LinksApi</c> field: Mojang's "latest Bedrock dedicated server download" endpoint.</summary>
    private const string LinksApi = "https://net-secondary.web.minecraft-services.net/api/v1.0/download/links";

    public string Id => "minecraft-bedrock";

    /// <summary><c>GetVersionsAsync</c> just does <c>Task.FromResult(new List&lt;string&gt; { "latest" })</c>.</summary>
    public Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct)
        => Task.FromResult(new List<string> { "latest" });

    public async Task<InstallResult> InstallAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        progress.Report(InstallProgress.Indeterminate("Resolving Bedrock server from Mojang…"));

        var body = await DownloadHelper.GetStringAsync(LinksApi, ct).ConfigureAwait(false);
        using var doc = JsonDocument.Parse(body);
        var links = doc.RootElement.TryGetProperty("result", out var result)
            && result.TryGetProperty("links", out var linksEl)
            && linksEl.ValueKind == JsonValueKind.Array
            ? linksEl
            : default;

        string? downloadUrl = null;
        if (links.ValueKind == JsonValueKind.Array)
        {
            // INFERRED: the decompiled `<>c.<InstallAsync>b__6_0` predicate body wasn't
            // captured (only its call edge via `Enumerable.FirstOrDefault`), so the exact
            // field/value it filters on is reconstructed from Mojang's publicly documented
            // links-API shape: each entry has a `downloadType` discriminator, and the
            // Windows dedicated server's is `"serverBedrockWindows"`.
            foreach (var link in links.EnumerateArray())
            {
                if (link.TryGetProperty("downloadType", out var dt) && dt.GetString() == "serverBedrockWindows")
                {
                    downloadUrl = link.TryGetProperty("downloadUrl", out var url) ? url.GetString() : null;
                    break;
                }
            }
        }

        if (downloadUrl is null)
        {
            throw new InvalidOperationException("Mojang's API returned no Windows Bedrock server download.");
        }

        var fileStem = Path.GetFileNameWithoutExtension(DownloadHelper.SafeFileName(
            Uri.TryCreate(downloadUrl, UriKind.Absolute, out var parsedUrl) ? parsedUrl.AbsolutePath : downloadUrl,
            "bedrock-server-latest.zip"));
        var version = fileStem.StartsWith("bedrock-server-", StringComparison.Ordinal)
            ? fileStem["bedrock-server-".Length..]
            : fileStem;

        var zipPath = Path.Combine(request.TargetDir, "bedrock-server.zip");

        progress.Report(InstallProgress.Indeterminate($"Downloading Bedrock server {version}"));
        await DownloadHelper.DownloadFileAsync(downloadUrl, zipPath, ct).ConfigureAwait(false);

        progress.Report(InstallProgress.Indeterminate("Extracting Bedrock server…"));
        try
        {
            DownloadHelper.ExtractZip(zipPath, request.TargetDir);
        }
        finally
        {
            TryDelete(zipPath);
        }

        var propsPath = Path.Combine(request.TargetDir, "server.properties");
        if (File.Exists(propsPath))
        {
            try
            {
                var contents = await File.ReadAllTextAsync(propsPath, ct).ConfigureAwait(false);
                var updated = contents
                    .Replace("server-port=19132", $"server-port={request.Port}")
                    .Replace("server-name=Dedicated Server", $"server-name={ConfigText.SingleLine(request.ServerName)}");
                await File.WriteAllTextAsync(propsPath, updated, ct).ConfigureAwait(false);
            }
            catch
            {
                // Matches the original's best-effort rewrite: a failure here isn't fatal.
            }
        }

        return new InstallResult
        {
            ServerFile = "bedrock_server.exe",
            Version = version,
            StartCommand = null,
            JavaMajor = 0,
        };
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch
        {
            // Best-effort cleanup, matching the original's fire-and-forget delete.
        }
    }
}
