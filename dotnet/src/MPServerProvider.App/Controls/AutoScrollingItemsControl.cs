using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MPServerProvider.App.Controls;

public sealed class AutoScrollingItemsControl : ItemsControl
{
    public static readonly DependencyProperty ScrollSpeedProperty = DependencyProperty.Register(
        nameof(ScrollSpeed), typeof(double), typeof(AutoScrollingItemsControl), new PropertyMetadata(0.6));

    public static readonly DependencyProperty IsAutoScrollEnabledProperty = DependencyProperty.Register(
        nameof(IsAutoScrollEnabled), typeof(bool), typeof(AutoScrollingItemsControl), new PropertyMetadata(true));

    public double ScrollSpeed
    {
        get => (double)GetValue(ScrollSpeedProperty);
        set => SetValue(ScrollSpeedProperty, value);
    }

    public bool IsAutoScrollEnabled
    {
        get => (bool)GetValue(IsAutoScrollEnabledProperty);
        set => SetValue(IsAutoScrollEnabledProperty, value);
    }

    private ScrollViewer? _scrollViewer;
    private readonly DispatcherTimer _timer;
    private bool _paused;

    public AutoScrollingItemsControl()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += OnTick;

        Loaded += (_, _) => _timer.Start();
        Unloaded += (_, _) => _timer.Stop();
        MouseEnter += (_, _) => _paused = true;
        MouseLeave += (_, _) => _paused = false;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer
            ?? FindScrollViewer(this);
    }

    private static ScrollViewer? FindScrollViewer(DependencyObject root)
    {
        for (var i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(root, i);
            if (child is ScrollViewer sv) return sv;
            var nested = FindScrollViewer(child);
            if (nested != null) return nested;
        }
        return null;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (!IsAutoScrollEnabled || _paused || _scrollViewer is null) return;
        if (_scrollViewer.ScrollableWidth <= 0) return;

        var next = _scrollViewer.HorizontalOffset + ScrollSpeed;
        if (next >= _scrollViewer.ScrollableWidth)
        {
            next = 0;
        }
        _scrollViewer.ScrollToHorizontalOffset(next);
    }
}
