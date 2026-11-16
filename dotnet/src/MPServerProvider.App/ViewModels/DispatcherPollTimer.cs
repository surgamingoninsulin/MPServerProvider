using System.Windows.Threading;

namespace MPServerProvider.App.ViewModels;

public sealed class DispatcherPollTimer : IDisposable
{
    private readonly DispatcherTimer _timer;
    private readonly Action _tick;

    public DispatcherPollTimer(TimeSpan interval, Action tick)
    {
        _tick = tick;
        _timer = new DispatcherTimer { Interval = interval };
        _timer.Tick += (_, _) => _tick();
    }

    public void Start(bool runImmediately = false)
    {
        if (runImmediately)
        {
            _tick();
        }
        _timer.Start();
    }

    public void Stop() => _timer.Stop();

    public void Dispose() => _timer.Stop();
}
