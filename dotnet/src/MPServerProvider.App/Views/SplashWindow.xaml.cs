using System.Windows;

namespace MPServerProvider.App.Views;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        InitializeComponent();
    }

    public void Report(string status, double fraction)
    {
        var percent = (int)Math.Round(Math.Clamp(fraction, 0, 1) * 100);
        StatusText.Text = status;
        PercentText.Text = $"{percent}%";
        Progress.Value = percent;
    }
}
