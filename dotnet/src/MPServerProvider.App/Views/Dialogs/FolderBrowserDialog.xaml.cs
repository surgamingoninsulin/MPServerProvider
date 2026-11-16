using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MPServerProvider.App.Views.Dialogs;

public sealed record FolderEntry(string Name, string Path, string Icon, string Detail);

public partial class FolderBrowserDialog : DialogWindow
{
    private const string FolderIcon = "";
    private const string DriveIcon = "";

    private string? _current;
    private int _loadVersion;
    private bool _suppressPlaces;

    private FolderBrowserDialog(string title, string? startFolder)
    {
        InitializeComponent();
        TitleText.Text = title;
        Loaded += async (_, _) =>
        {
            try
            {
                await LoadPlacesAsync();
                var start = startFolder is not null && Directory.Exists(startFolder)
                    ? startFolder
                    : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                await NavigateAsync(start);
            }
            catch (Exception ex)
            {
                ListStatusText.Text = $"Couldn't list folders: {ex.Message}";
            }
        };
    }

    public string? SelectedFolder { get; private set; }

    public static string? Pick(Window? owner, string title, string? startFolder = null)
    {
        var dialog = new FolderBrowserDialog(title, startFolder);
        if (owner is not null)
        {
            dialog.Owner = owner;
        }

        dialog.ShowDialog();
        return dialog.SelectedFolder;
    }

    private async Task LoadPlacesAsync()
    {
        var places = await Task.Run(() =>
        {
            var list = new List<FolderEntry>();
            foreach (var (name, folder) in new[]
                     {
                         ("Desktop", Environment.SpecialFolder.DesktopDirectory),
                         ("Documents", Environment.SpecialFolder.MyDocuments),
                         ("Home", Environment.SpecialFolder.UserProfile),
                     })
            {
                var path = Environment.GetFolderPath(folder);
                if (path.Length > 0 && Directory.Exists(path))
                {
                    list.Add(new FolderEntry(name, path, name == "Home" ? "" : FolderIcon, string.Empty));
                }
            }

            var downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            if (Directory.Exists(downloads))
            {
                list.Insert(Math.Min(2, list.Count), new FolderEntry("Downloads", downloads, "", string.Empty));
            }

            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (!drive.IsReady)
                    {
                        continue;
                    }

                    var letter = drive.Name.TrimEnd('\\');
                    var label = string.IsNullOrWhiteSpace(drive.VolumeLabel)
                        ? drive.DriveType == DriveType.Removable ? "USB drive" : "Local Disk"
                        : drive.VolumeLabel;
                    list.Add(new FolderEntry($"{label} ({letter})", drive.RootDirectory.FullName, DriveIcon,
                        $"{FormatSize(drive.AvailableFreeSpace)} free"));
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    // A drive that went away or is locked: leave it out.
                }
            }

            return list;
        });

        _suppressPlaces = true;
        PlacesList.ItemsSource = places;
        _suppressPlaces = false;
    }

    private static string FormatSize(long bytes) => bytes >= 1L << 40
        ? $"{bytes / (double)(1L << 40):0.0} TB"
        : $"{bytes / (double)(1L << 30):0} GB";

    private async Task NavigateAsync(string path)
    {
        var version = ++_loadVersion;
        string full;
        try
        {
            full = Path.GetFullPath(path);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            ListStatusText.Text = "That isn't a valid folder path.";
            return;
        }

        ListStatusText.Text = "Loading…";
        var (entries, error) = await Task.Run(() => ListFolders(full));
        if (version != _loadVersion)
        {
            return; // a newer navigation won
        }

        if (error is not null && entries.Count == 0 && !Directory.Exists(full))
        {
            ListStatusText.Text = error;
            return;
        }

        _current = full;
        PathBox.Text = full;
        FoldersList.ItemsSource = entries;
        ListStatusText.Text = error ?? (entries.Count == 0 ? "No folders here." : string.Empty);
        UpButton.IsEnabled = Directory.GetParent(full) is not null;
        UpdateSelection();
    }

    private static (List<FolderEntry> Entries, string? Error) ListFolders(string path)
    {
        var result = new List<FolderEntry>();
        try
        {
            var options = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                AttributesToSkip = FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReparsePoint,
            };
            foreach (var dir in new DirectoryInfo(path).EnumerateDirectories("*", options)
                         .OrderBy(d => d.Name, StringComparer.CurrentCultureIgnoreCase))
            {
                result.Add(new FolderEntry(dir.Name, dir.FullName, FolderIcon, string.Empty));
            }

            return (result, null);
        }
        catch (UnauthorizedAccessException)
        {
            return (result, "Windows doesn't allow opening this folder.");
        }
        catch (DirectoryNotFoundException)
        {
            return (result, "That folder doesn't exist.");
        }
        catch (IOException ex)
        {
            return (result, $"This folder can't be read: {ex.Message}");
        }
    }

    private string? Target => (FoldersList.SelectedItem as FolderEntry)?.Path ?? _current;

    private void UpdateSelection()
    {
        SelectedText.Text = Target ?? string.Empty;
        SelectButton.Content = FoldersList.SelectedItem is FolderEntry entry ? $"Select “{entry.Name}”" : "Select this folder";
        SelectButton.IsEnabled = Target is not null;
    }

    private async void PlacesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_suppressPlaces && PlacesList.SelectedItem is FolderEntry place)
        {
            await NavigateAsync(place.Path);
        }
    }

    private async void FoldersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is DependencyObject source
            && ItemsControl.ContainerFromElement(FoldersList, source) is ListBoxItem { DataContext: FolderEntry entry })
        {
            await NavigateAsync(entry.Path);
        }
    }

    private async void FoldersList_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && FoldersList.SelectedItem is FolderEntry entry)
        {
            e.Handled = true;
            await NavigateAsync(entry.Path);
        }
        else if (e.Key == Key.Back)
        {
            e.Handled = true;
            await GoUpAsync();
        }
    }

    private void FoldersList_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateSelection();

    private async void PathBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            var text = Environment.ExpandEnvironmentVariables(PathBox.Text.Trim().Trim('"'));
            if (Directory.Exists(text))
            {
                await NavigateAsync(text);
            }
            else
            {
                ListStatusText.Text = "That folder doesn't exist.";
            }
        }
    }

    private async void UpButton_Click(object sender, RoutedEventArgs e) => await GoUpAsync();

    private async Task GoUpAsync()
    {
        if (_current is not null && Directory.GetParent(_current) is { } parent)
        {
            await NavigateAsync(parent.FullName);
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        if (_current is not null)
        {
            await NavigateAsync(_current);
        }
    }

    private void SelectButton_Click(object sender, RoutedEventArgs e)
    {
        if (Target is { } target && Directory.Exists(target))
        {
            SelectedFolder = target;
            Close();
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}
