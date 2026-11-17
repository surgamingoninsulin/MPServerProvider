using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MPServerProvider.App.Controls;

/// <summary>Turns a plain https URL into a lazily, asynchronously downloaded image for an Image control.</summary>
public sealed class UrlToBitmapConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string { Length: > 0 } url || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return null;
        }

        // No CacheOption.OnLoad and no Freeze(): both would force (or lock in) a synchronous
        // download on the UI thread. Left at defaults, WPF downloads in the background and the
        // bound Image updates itself once BitmapImage raises its own DownloadCompleted.
        var image = new BitmapImage();
        image.BeginInit();
        image.UriSource = uri;
        image.EndInit();
        return image;
    }

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
