using System.Globalization;
using System.Windows.Data;

namespace MPServerProvider.App.Converters;

/// <summary>Small shared XAML converter: <c>!bool</c>, used to disable a button while a bound
/// "busy"/"running" flag is true (e.g. <c>TunnelDialog</c>'s Map/Start buttons).</summary>
public sealed class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is bool b && !b;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is bool b && !b;
}
