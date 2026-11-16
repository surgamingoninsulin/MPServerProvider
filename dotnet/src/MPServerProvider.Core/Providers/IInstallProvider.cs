using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

/// <summary>
/// Shared contract implemented by every concrete install provider plugin
/// (<see cref="SteamCmdProvider"/>, <see cref="CfxProvider"/>, <see cref="BeamMpProvider"/>,
/// <see cref="BedrockProvider"/>, <see cref="ManualProvider"/>, <see cref="MinecraftJavaProvider"/>,
/// <see cref="UrlProvider"/>).
/// Ported from <c>MPServerProvider.Core.Providers.IInstallProvider</c> (decompile token
/// 0x02000020). The decompiled interface only carries opaque <c>class:NNN</c>/<c>valuetype:NNN</c>
/// tokens for every parameter/return type:
/// <code>
/// get_Id() -&gt; string
/// GetVersionsAsync(class:160, string, valuetype:121) -&gt; class:117&lt;class:269&lt;string&gt;&gt;
/// InstallAsync(class:208, class:233&lt;class:200&gt;, valuetype:121) -&gt; class:117&lt;class:212&gt;
/// </code>
/// These were pinned down by cross-referencing every *concrete* overriding provider's IL (which
/// carries real signatures), already done once by the proven Rust/Slint port
/// (<c>crates/mpsp-core/src/providers/mod.rs</c>) — this restores that exact resolution rather
/// than re-deriving it:
/// <list type="bullet">
/// <item><c>class:117&lt;T&gt;</c> is <see cref="Task{T}"/>.</item>
/// <item><c>class:269&lt;string&gt;</c> is <see cref="List{T}"/> of <see cref="string"/> (every
/// provider's <c>GetVersionsAsync</c> body builds a <c>List&lt;string&gt;</c>).</item>
/// <item><c>valuetype:121</c> is <see cref="CancellationToken"/>.</item>
/// <item><c>class:160</c> is <see cref="GameDefinition"/> (the first parameter, read only by the
/// providers that switch on <c>variant</c>, e.g. <see cref="MinecraftJavaProvider"/>).</item>
/// <item><c>class:208</c> is <see cref="InstallRequest"/> (confirmed directly: every
/// <c>InstallAsync</c> body reads <c>InstallRequest.TargetDir</c>/<c>Version</c>/<c>ServerName</c>
/// etc.).</item>
/// <item><c>class:200</c> is <see cref="InstallProgress"/>, carried as
/// <c>class:233&lt;class:200&gt;</c> i.e. <see cref="IProgress{T}"/> of <see cref="InstallProgress"/>.</item>
/// <item><c>class:212</c> is <see cref="InstallResult"/> (every <c>InstallAsync</c> body
/// constructs one as its return value).</item>
/// </list>
/// So the real, restored signature is exactly:
/// <code>
/// string Id { get; }
/// Task&lt;List&lt;string&gt;&gt; GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct);
/// Task&lt;InstallResult&gt; InstallAsync(InstallRequest request, IProgress&lt;InstallProgress&gt; progress, CancellationToken ct);
/// </code>
/// </summary>
public interface IInstallProvider
{
    /// <summary>Port of <c>get_Id()</c>. Stable identifier used as the <c>install.provider</c>
    /// value in <see cref="GameDefinition"/> manifests (e.g. <c>"steamcmd"</c>, <c>"cfx"</c>).</summary>
    string Id { get; }

    /// <summary>Port of <c>GetVersionsAsync(GameDefinition, string?, CancellationToken)</c>.</summary>
    Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct);

    /// <summary>Port of <c>InstallAsync(InstallRequest, IProgress&lt;InstallProgress&gt;, CancellationToken)</c>.</summary>
    Task<InstallResult> InstallAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct);
}
