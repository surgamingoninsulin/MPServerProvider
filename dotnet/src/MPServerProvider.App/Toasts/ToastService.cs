using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace MPServerProvider.App.Toasts;

/// <summary>
/// Port of <c>MPServerProvider.App.ToastService</c> (decompile token 0x02000007): a static-ish
/// notification queue backed by an <see cref="ObservableCollection{T}"/>
/// (<c>Toasts</c> in the original, same name here) that a toast-overlay region in
/// <c>MainWindow.xaml</c> binds to.
/// <para>
/// The decompiled <c>Show(string, string)</c> marshals onto the UI dispatcher via
/// <c>Dispatcher.CheckAccess()</c>/<c>Dispatcher.Invoke</c> when called off-thread — this port
/// does the same with <see cref="Dispatcher.Invoke(Action)"/>, WPF's native equivalent of the
/// Rust port's <c>slint::invoke_from_event_loop</c> dance, without needing a manual channel.
/// Auto-dismiss after <see cref="ToastItem.Duration"/> is ported from the proven Rust reference
/// (<c>toast.rs</c>'s <c>Timer::single_shot</c>) using a <see cref="DispatcherTimer"/>.
/// </para>
/// </summary>
public sealed class ToastService
{
    /// <summary>Port of <c>ToastService.Toasts</c> — the live, UI-bindable queue.</summary>
    public ObservableCollection<ToastItem> Toasts { get; } = new();

    private Dispatcher Dispatcher => Application.Current?.Dispatcher
        ?? Dispatcher.CurrentDispatcher;

    /// <summary>Port of <c>ToastService.Show(string message, string icon)</c>, extended with
    /// severity/duration (see <see cref="ToastItem"/>'s remarks).</summary>
    public void Show(string message, string icon = "", ToastSeverity severity = ToastSeverity.Info, TimeSpan? duration = null)
    {
        void Add()
        {
            var item = new ToastItem
            {
                Message = message,
                Icon = icon,
                Severity = severity,
                Duration = duration ?? DefaultDuration(severity),
            };
            Toasts.Add(item);

            var timer = new DispatcherTimer { Interval = item.Duration };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                Dismiss(item.Id);
            };
            timer.Start();
        }

        if (Dispatcher.CheckAccess())
        {
            Add();
        }
        else
        {
            Dispatcher.Invoke(Add);
        }
    }

    public void Info(string message) => Show(message, "", ToastSeverity.Info);
    public void Success(string message) => Show(message, "", ToastSeverity.Success);
    public void Warning(string message) => Show(message, "", ToastSeverity.Warning);

    /// <summary>Port of <c>ToastService.Error(string)</c>.</summary>
    public void Error(string message) => Show(message, "", ToastSeverity.Error);

    public void Dismiss(int id)
    {
        void Remove()
        {
            for (var i = Toasts.Count - 1; i >= 0; i--)
            {
                if (Toasts[i].Id == id)
                {
                    Toasts.RemoveAt(i);
                }
            }
        }

        if (Dispatcher.CheckAccess())
        {
            Remove();
        }
        else
        {
            Dispatcher.Invoke(Remove);
        }
    }

    private static TimeSpan DefaultDuration(ToastSeverity severity) => severity switch
    {
        ToastSeverity.Warning => TimeSpan.FromSeconds(5),
        ToastSeverity.Error => TimeSpan.FromSeconds(6),
        _ => TimeSpan.FromSeconds(4),
    };
}
