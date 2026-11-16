using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MPServerProvider.App.Controls;

/// <summary>
/// Attached behavior (applied to every <see cref="ScrollViewer"/> by the implicit style in
/// <c>Themes/Theme.xaml</c>) that makes mouse-wheel scrolling work everywhere despite nesting.
/// <para>
/// WPF's <see cref="ScrollViewer"/> marks <c>MouseWheel</c> as handled even when it has nothing
/// left to scroll, so an inner scroller (a read-only console <see cref="TextBox"/>, a
/// <see cref="ListBox"/>, the horizontal news carousel, a ComboBox dropdown...) swallowed the
/// wheel and the page around it never scrolled. With scrollbars hidden app-wide that left whole
/// pages un-scrollable under the cursor. This forwards the wheel to the parent whenever the
/// innermost scroller under the mouse can't move further in that direction.
/// </para>
/// </summary>
public static class WheelScroll
{
    public static readonly DependencyProperty ForwardToParentProperty =
        DependencyProperty.RegisterAttached(
            "ForwardToParent",
            typeof(bool),
            typeof(WheelScroll),
            new PropertyMetadata(false, OnForwardToParentChanged));

    public static bool GetForwardToParent(DependencyObject obj) => (bool)obj.GetValue(ForwardToParentProperty);

    public static void SetForwardToParent(DependencyObject obj, bool value) => obj.SetValue(ForwardToParentProperty, value);

    private static void OnForwardToParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ScrollViewer viewer)
        {
            return;
        }
        viewer.PreviewMouseWheel -= OnPreviewMouseWheel;
        if (e.NewValue is true)
        {
            viewer.PreviewMouseWheel += OnPreviewMouseWheel;
        }
    }

    private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Handled || sender is not ScrollViewer viewer)
        {
            return;
        }

        // Only the innermost ScrollViewer under the mouse decides; outer ones see the preview
        // first (tunneling) and must leave it alone.
        if (!ReferenceEquals(NearestScrollViewer(e.OriginalSource as DependencyObject), viewer))
        {
            return;
        }

        var canScrollVertically = viewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                                  viewer.ScrollableHeight > 0;
        var canMove = canScrollVertically &&
                      (e.Delta > 0 ? viewer.VerticalOffset > 0 : viewer.VerticalOffset < viewer.ScrollableHeight);
        if (canMove)
        {
            return;
        }

        // Never scroll the page behind an open dropdown/popup.
        if (PresentationSource.FromVisual(viewer)?.RootVisual?.GetType().Name == "PopupRoot")
        {
            return;
        }

        if (VisualTreeHelper.GetParent(viewer) is not UIElement parent)
        {
            return;
        }

        e.Handled = true;
        parent.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent,
            Source = viewer,
        });
    }

    private static ScrollViewer? NearestScrollViewer(DependencyObject? element)
    {
        while (element is not null)
        {
            if (element is ScrollViewer sv)
            {
                return sv;
            }
            element = element is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(element)
                : LogicalTreeHelper.GetParent(element);
        }
        return null;
    }
}
