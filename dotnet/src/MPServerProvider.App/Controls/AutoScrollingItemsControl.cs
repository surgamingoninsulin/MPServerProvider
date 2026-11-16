using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MPServerProvider.App.Controls;

/// <summary>
/// Reusable horizontally auto-scrolling card row — built now so the Home page's news carousel
/// (explicitly called out by the user as broken in the Rust/Slint port: "no sliding news
/// carousel", just a static row) has a genuinely reusable, genuinely-auto-scrolling home from
/// day one. A later wave drops real <c>NewsItem</c> cards into its <see cref="ItemsControl.ItemsSource"/>
/// and styles them via <c>ItemTemplate</c> — the scrolling mechanism itself does not change.
/// <para>
/// Mechanism: hosts an <see cref="ItemsControl"/> (so arbitrary card templates work) inside a
/// <see cref="ScrollViewer"/> with horizontal scrolling enabled but its scrollbar hidden, and
/// nudges <see cref="ScrollViewer.ScrollToHorizontalOffset"/> forward on a
/// <see cref="DispatcherTimer"/> tick. When the offset reaches the scrollable extent it wraps
/// back to 0, giving a continuous, auto-sliding loop — the behavior the real original app has
/// (per <c>docs/reference-screenshots/1.png</c>'s horizontal scrollbar on the news row) and the
/// Rust/Slint port lacked. Pausing on pointer-over is included so a user can actually read a
/// card before it slides away.
/// </para>
/// </summary>
public sealed class AutoScrollingItemsControl : ItemsControl
{
    public static readonly DependencyProperty ScrollSpeedProperty = DependencyProperty.Register(
        nameof(ScrollSpeed), typeof(double), typeof(AutoScrollingItemsControl), new PropertyMetadata(0.6));

    public static readonly DependencyProperty IsAutoScrollEnabledProperty = DependencyProperty.Register(
        nameof(IsAutoScrollEnabled), typeof(bool), typeof(AutoScrollingItemsControl), new PropertyMetadata(true));

    /// <summary>Pixels nudged per timer tick.</summary>
    public double ScrollSpeed
    {
        get => (double)GetValue(ScrollSpeedProperty);
        set => SetValue(ScrollSpeedProperty, value);
    }

    /// <summary>Port of the Settings page's "Slide the Home news cards automatically" toggle
    /// (<c>AppSettings.news_auto_scroll</c> per the reference-screenshots index) — bind this to
    /// that setting so users can turn the slide off.</summary>
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
