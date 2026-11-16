using System.Windows;

namespace MPServerProvider.App.Views;

/// <summary>
/// Port of <c>MPServerProvider.App.Views.SplashWindow</c> (decompile token 0x0200001c).
/// Startup progress is driven from <see cref="App"/> via <see cref="Report"/>, called through
/// <see cref="System.Windows.Threading.Dispatcher.Invoke(Action)"/> while the composition root
/// builds off the UI thread — WPF's native equivalent of the proven Rust port's manual
/// thread+channel dance.
/// </summary>
public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
    }

    /// <summary>Updates the status line, percent text, and progress bar in one call. Safe to
    /// call only from the UI thread — callers marshal via Dispatcher first.</summary>
    public void Report(string status, double fraction)
    {
        var percent = (int)Math.Round(Math.Clamp(fraction, 0, 1) * 100);
        StatusText.Text = status;
        PercentText.Text = $"{percent}%";
        Progress.Value = percent;
    }
}
