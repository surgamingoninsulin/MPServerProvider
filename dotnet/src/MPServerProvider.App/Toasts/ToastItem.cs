namespace MPServerProvider.App.Toasts;

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

    public string Message { get; init; } = string.Empty;

    public string Icon { get; init; } = string.Empty;

    public ToastSeverity Severity { get; init; } = ToastSeverity.Info;

    public TimeSpan Duration { get; init; } = TimeSpan.FromSeconds(4);
}
