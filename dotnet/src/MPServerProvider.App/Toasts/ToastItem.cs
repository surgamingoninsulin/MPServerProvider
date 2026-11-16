namespace MPServerProvider.App.Toasts;

/// <summary>Port of <c>MPServerProvider.App.ToastItem</c> (decompile token 0x02000006).</summary>
/// <remarks>
/// The decompiled type only carries <c>Message</c>/<c>Icon</c> string fields (no severity enum,
/// no duration — those live in the proven Rust port's <c>toast.rs</c>, which went further than
/// the decompile evidence alone supports). This port keeps the decompiled shape (Message, Icon)
/// as the source of truth but adds <see cref="Severity"/> and <see cref="Duration"/> as
/// additive, non-conflicting extensions — the same blend the task brief asks for — so the UI can
/// style/auto-dismiss toasts without losing fidelity to the real type.
/// </remarks>
public enum ToastSeverity
{
    Info,
    Success,
    Warning,
    Error,
}

public sealed class ToastItem
{
    private static int _nextId = 1;

    public int Id { get; } = System.Threading.Interlocked.Increment(ref _nextId);

    /// <summary>Port of <c>ToastItem.Message</c>.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>Port of <c>ToastItem.Icon</c> (glyph shown next to the message).</summary>
    public string Icon { get; init; } = string.Empty;

    public ToastSeverity Severity { get; init; } = ToastSeverity.Info;

    public TimeSpan Duration { get; init; } = TimeSpan.FromSeconds(4);
}
