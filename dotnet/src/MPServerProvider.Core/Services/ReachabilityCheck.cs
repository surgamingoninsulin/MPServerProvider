using System.Text.Json;

namespace MPServerProvider.Core.Services;

public sealed record ReachabilityResult(bool Online, string Text);

public static class ReachabilityCheck
{
    public static bool IsSupported(string? gameId)
        => ModCompatibility.IsMinecraftJava(gameId) || string.Equals(gameId, "minecraft-bedrock", StringComparison.OrdinalIgnoreCase);

    public static async Task<ReachabilityResult> CheckAsync(string gameId, string address, CancellationToken ct = default)
    {
        var edition = ModCompatibility.IsMinecraftJava(gameId) ? "java" : "bedrock";
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(20));
        var json = await DownloadService.GetStringAsync(
            $"https://api.mcstatus.io/v2/status/{edition}/{Uri.EscapeDataString(address.Trim())}", timeout.Token).ConfigureAwait(false);
        return Parse(json, address.Trim());
    }

    public static ReachabilityResult Parse(string json, string address)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (!root.TryGetProperty("online", out var online) || online.ValueKind != JsonValueKind.True)
        {
            return new ReachabilityResult(false,
                $"{address} is not reachable from the internet. Check that the server is running, the tunnel/port forward is active, and the firewall allows the port.");
        }

        var players = root.TryGetProperty("players", out var p)
                      && p.TryGetProperty("online", out var on) && on.TryGetInt32(out var count)
                      && p.TryGetProperty("max", out var mx) && mx.TryGetInt32(out var max)
            ? $" · {count}/{max} players"
            : string.Empty;
        var version = root.TryGetProperty("version", out var v) && v.ValueKind == JsonValueKind.Object
                      && v.TryGetProperty("name_clean", out var name) && name.GetString() is { Length: > 0 } n
            ? $" · {n}"
            : string.Empty;
        return new ReachabilityResult(true, $"Online from the internet: {address}{version}{players}");
    }
}
