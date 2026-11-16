using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace MPServerProvider.App.Toasts;

public sealed class ToastService
{
    public ObservableCollection<ToastItem> Toasts { get; } = new();

    private Dispatcher Dispatcher => Application.Current?.Dispatcher
        ?? Dispatcher.CurrentDispatcher;

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
