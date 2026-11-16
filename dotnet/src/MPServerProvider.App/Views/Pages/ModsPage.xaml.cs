using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MPServerProvider.App.ViewModels;

namespace MPServerProvider.App.Views.Pages;

/// <summary>
/// Port of <c>MPServerProvider.App.Views.ModsPage</c> (decompile token 0x02000014): game filter
/// dropdown + per-game checkbox sidebar, keyword search, "Import from Folder" (WPF's built-in
/// <see cref="Microsoft.Win32.OpenFolderDialog"/>), and per-mod "Add to Server ▾"/assignment-chip
/// removal context menus, wired against the real <see cref="AppServices.Mods"/> API. State lives
/// in <see cref="ModsPageViewModel"/>; this code-behind only builds the dynamic context menus the
/// decompiled page's own code-behind built (no clean declarative XAML equivalent for a
/// per-row popup whose item count varies per mod).
/// </summary>
public partial class ModsPage : UserControl, INavigationAware
{
    private ModsPageViewModel? _viewModel;

    // Never cast DataContext: MainWindow sets DataContext = itself and WPF inherits it into every
    // hosted page, so until our own view model is assigned DataContext is the MainWindow, not
    // null — casting it threw InvalidCastException every time the Mods page opened.
    private ModsPageViewModel? ViewModel => _viewModel;

    public ModsPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Lazily builds the view model on first navigation: <see cref="MainWindow"/> constructs every
    /// page from its own constructor, before the window (and its <see cref="MainWindow.Toasts"/>)
    /// is reachable, so the view model can only be built once the shell has navigated here.
    /// </summary>
    public void OnNavigatedTo()
    {
        if (_viewModel is null && Application.Current is App { Services: { } services } &&
            Window.GetWindow(this) is MainWindow mainWindow)
        {
            _viewModel = new ModsPageViewModel(services, mainWindow.Toasts);
            DataContext = _viewModel;
        }

        _viewModel?.Initialize();
    }

    /// <summary>
    /// Builds the "Add to Server ▾" menu: compatible servers first, then incompatible ones
    /// disabled with the reason in the header (e.g. "Survival — Needs MC 1.20.x, server is 26.3").
    /// </summary>
    private void AddToServerButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not FrameworkElement { DataContext: ModCardViewModel card } button)
            {
                return;
            }

            var menu = new ContextMenu();
            if (card.ServerTargets.Count == 0)
            {
                menu.Items.Add(new MenuItem { Header = "No servers of this game without it", IsEnabled = false });
            }
            else
            {
                foreach (var target in card.ServerTargets)
                {
                    var item = new MenuItem { Header = target.Header, IsEnabled = target.IsCompatible };
                    if (target.IsCompatible)
                    {
                        var serverName = target.ServerName;
                        item.Click += async (_, _) =>
                        {
                            if (ViewModel is { } vm)
                            {
                                await vm.AssignToServerAsync(card, serverName);
                            }
                        };
                    }

                    menu.Items.Add(item);
                }
            }

            button.ContextMenu = menu;
            menu.PlacementTarget = button;
            menu.IsOpen = true;
        }
        catch (Exception ex)
        {
            ReportError("Couldn't open the server menu", ex);
        }
    }

    private async void RemoveAssignmentButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is not FrameworkElement { DataContext: string serverName } element)
            {
                return;
            }

            var card = FindAncestorDataContext<ModCardViewModel>(element);
            if (card is not null && ViewModel is { } vm)
            {
                await vm.RemoveAssignmentAsync(card, serverName);
            }
        }
        catch (Exception ex)
        {
            ReportError("Couldn't remove the mod", ex);
        }
    }

    // -- Drag and drop from Explorer → mod library -----------------------------------------

    private static string[]? DroppedPaths(DragEventArgs e)
        => e.Data.GetDataPresent(DataFormats.FileDrop) ? e.Data.GetData(DataFormats.FileDrop) as string[] : null;

    private void Drop_DragOver(object sender, DragEventArgs e)
    {
        e.Handled = true;
        if (ViewModel is not { IsImportingDrop: false } vm || DroppedPaths(e) is not { Length: > 0 } paths)
        {
            e.Effects = DragDropEffects.None;
            DropOverlay.Visibility = Visibility.Collapsed;
            return;
        }

        e.Effects = DragDropEffects.Copy;
        var gameId = vm.DropTargetGameId(paths);
        var gameName = gameId is null ? null : (Application.Current as App)?.Services?.Games.Find(gameId)?.Name;
        DropOverlayTitle.Text = gameName is null
            ? "Pick the game in “Show” first"
            : $"Drop to import for {gameName}";
        DropOverlay.Visibility = Visibility.Visible;
    }

    private void Drop_DragLeave(object sender, DragEventArgs e)
    {
        var p = e.GetPosition(DropArea);
        if (p.X <= 0 || p.Y <= 0 || p.X >= DropArea.ActualWidth || p.Y >= DropArea.ActualHeight)
        {
            DropOverlay.Visibility = Visibility.Collapsed;
        }
    }

    private async void Drop_Drop(object sender, DragEventArgs e)
    {
        DropOverlay.Visibility = Visibility.Collapsed;
        try
        {
            if (DroppedPaths(e) is { Length: > 0 } paths && ViewModel is { } vm)
            {
                e.Handled = true;
                await vm.ImportDroppedAsync(paths);
            }
        }
        catch (Exception ex)
        {
            ReportError("Couldn't import the dropped files", ex);
        }
    }

    private void ReportError(string what, Exception ex)
    {
        if (Window.GetWindow(this) is MainWindow { Toasts: { } toasts })
        {
            toasts.Error($"{what}: {ex.Message}");
        }
    }

    private static T? FindAncestorDataContext<T>(DependencyObject? start) where T : class
    {
        var current = start;
        while (current is not null)
        {
            if (current is FrameworkElement { DataContext: T match })
            {
                return match;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}
