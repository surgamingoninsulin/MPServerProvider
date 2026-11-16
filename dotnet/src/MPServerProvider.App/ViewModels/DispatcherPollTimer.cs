using System.Windows.Threading;

namespace MPServerProvider.App.ViewModels;

/// <summary>
/// Small UI-thread poll-timer wrapper shared by this wave's view models. Mirrors the already-
/// proven Rust reference's <c>slint::Timer::start(Repeated, ...)</c> pattern
/// (<c>crates/mpsp-app/src/pages/{my_servers,server_detail}.rs</c>) using WPF's native
/// <see cref="DispatcherTimer"/> instead of a manual channel/poll dance.
/// </summary>
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
