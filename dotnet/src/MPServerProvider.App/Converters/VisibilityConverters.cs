using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MPServerProvider.App.Converters;

/// <summary>Bool -&gt; Visibility (true =&gt; Visible, false =&gt; Collapsed).</summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is Visibility.Visible;
}

/// <summary>Bool -&gt; Visibility, inverted (true =&gt; Collapsed, false =&gt; Visible).</summary>
public sealed class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is Visibility.Collapsed;
}

/// <summary>String -&gt; Visibility (null/empty =&gt; Visible, otherwise Collapsed); used for
/// text-box placeholder hints that should hide once the user types something.</summary>
public sealed class StringEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Int length -&gt; Visibility (0 =&gt; Visible, otherwise Collapsed); used for a
/// <c>TextBox.Text.Length</c>-bound placeholder hint overlay.</summary>
public sealed class IntZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i && i == 0 ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Object -&gt; Visibility (non-null =&gt; Visible, null =&gt; Collapsed); used to hide an
/// <c>Image</c> until its asynchronously-resolved source actually loads (e.g. the news carousel's
/// cover art), instead of showing a blank/broken image box while the fetch is in flight.</summary>
public sealed class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is null ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
