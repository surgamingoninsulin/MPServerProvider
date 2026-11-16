using System.Text.Json.Serialization;

namespace MPServerProvider.Core.Models;

/// <summary>
/// Remote-access tunnel configuration for a server (UPnP port forwarding or a playit.gg agent).
/// Ported from <c>MPServerProvider.Core.Models.TunnelSpec</c> (decompile token 0x02000038); the
/// original only had <see cref="Mode"/>. <see cref="AutoStart"/> and <see cref="ExtraPorts"/>
/// are additions for the "enable the tunnel when the server starts" feature. Missing fields in
/// an older <c>server.json</c> fall back to the defaults below, so existing files still load.
/// </summary>
public class TunnelSpec
{
    public const string ModeNone = "none";
    public const string ModeUpnp = "upnp";
    public const string ModePlayit = "playit";

    /// <summary><c>"none"</c>, <c>"upnp"</c> or <c>"playit"</c>.</summary>
    public string Mode { get; set; } = ModeNone;

    /// <summary>
    /// When true, starting the server also enables its tunnel (UPnP: forward its ports;
    /// playit: make sure the agent is running), and stopping it disables the tunnel again.
    /// </summary>
    public bool AutoStart { get; set; }

    /// <summary>
    /// More ports to forward besides <see cref="ServerInstance.ServerPort"/>, for example
    /// <c>"27015"</c>, <c>"27015/udp"</c> or <c>"8080/tcp"</c>. Without a suffix the game's
    /// network protocol is used.
    /// </summary>
    public List<string> ExtraPorts { get; set; } = new();

    /// <summary>The stored mode, normalized: any unknown value is treated as <see cref="ModeNone"/>.</summary>
    [JsonIgnore]
    public string NormalizedMode => (Mode ?? string.Empty).Trim().ToLowerInvariant() switch
    {
        ModeUpnp => ModeUpnp,
        ModePlayit => ModePlayit,
        _ => ModeNone,
    };
}
