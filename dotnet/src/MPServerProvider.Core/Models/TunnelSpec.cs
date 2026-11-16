using System.Text.Json.Serialization;

namespace MPServerProvider.Core.Models;

public class TunnelSpec
{
    public const string ModeNone = "none";
    public const string ModeUpnp = "upnp";
    public const string ModePlayit = "playit";

    public string Mode { get; set; } = ModeNone;

    public bool AutoStart { get; set; }

    public List<string> ExtraPorts { get; set; } = new();

    [JsonIgnore]
    public string NormalizedMode => (Mode ?? string.Empty).Trim().ToLowerInvariant() switch
    {
        ModeUpnp => ModeUpnp,
        ModePlayit => ModePlayit,
        _ => ModeNone,
    };
}
