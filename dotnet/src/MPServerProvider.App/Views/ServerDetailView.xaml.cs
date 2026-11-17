using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MPServerProvider.App.ViewModels;

namespace MPServerProvider.App.Views;

public partial class ServerDetailView : UserControl
{
    private bool _suppressEditorDirty;
    private bool _gutterUpdateQueued;
    private bool _hookedWindowClosing;

    public ServerDetailView()
    {
        InitializeComponent();
        DataContextChanged += (_, e) =>
        {
            if (e.OldValue is ServerDetailViewModel oldVm)
            {
                oldVm.PropertyChanged -= ViewModel_PropertyChanged;
                oldVm.ConsoleUpdated -= ViewModel_ConsoleUpdated;
                oldVm.EditorTextLoaded -= ViewModel_EditorTextLoaded;
                oldVm.FilesTabRequested -= ViewModel_FilesTabRequested;
                oldVm.KleiTabRequested -= ViewModel_KleiTabRequested;
                oldVm.EditorTextProvider = null;
                oldVm.ModsTab.VsMods.PropertyChanged -= VsMods_PropertyChanged;
            }
            if (e.NewValue is ServerDetailViewModel newVm)
            {
                newVm.PropertyChanged += ViewModel_PropertyChanged;
                newVm.ConsoleUpdated += ViewModel_ConsoleUpdated;
                newVm.EditorTextLoaded += ViewModel_EditorTextLoaded;
                newVm.FilesTabRequested += ViewModel_FilesTabRequested;
                newVm.KleiTabRequested += ViewModel_KleiTabRequested;
                newVm.EditorTextProvider = () => EditorBox.Text;
                UpdateEditorLayout(newVm.IsEditorOpen);
                newVm.SetViewVisible(IsVisible);
                _followConsole = true;
                newVm.ResyncConsole();
                newVm.ModsTab.VsMods.PropertyChanged += VsMods_PropertyChanged;
                UpdateVsModWebBrowser(newVm.ModsTab.VsMods);
            }
        };

        // Only poll while visible (page/tab switches toggle visibility of this view).
        IsVisibleChanged += (_, e) => ViewModel?.SetViewVisible(e.NewValue is true);
        Loaded += OnLoaded;
        EditorBox.SelectionChanged += (_, _) => UpdateCaretPosition();
        FallBackToConsoleWhenHidden(TabResourcePack);
        FallBackToConsoleWhenHidden(TabLuckPerms);
        FallBackToConsoleWhenHidden(TabScenarios);
    }

    private void FallBackToConsoleWhenHidden(RadioButton tab)
    {
        DependencyPropertyDescriptor.FromProperty(VisibilityProperty, typeof(RadioButton))
            .AddValueChanged(tab, (_, _) =>
            {
                try
                {
                    if (tab.Visibility != Visibility.Visible && tab.IsChecked == true)
                    {
                        TabConsole.IsChecked = true;
                    }
                }
                catch (Exception ex)
                {
                    ReportError("Could not switch tabs", ex);
                }
            });
    }

    private ServerDetailViewModel? ViewModel => DataContext as ServerDetailViewModel;

    private void ReportError(string what, Exception ex)
    {
        if (ViewModel is { } vm)
        {
            vm.ReportError(what, ex);
        }
        else if (Window.GetWindow(this) is MainWindow { Toasts: { } toasts })
        {
            toasts.Error($"{what}: {ex.Message}");
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Offer to save unsaved editor changes when the app is closed. Hooked once: this view
        // lives as long as the (cached) My Servers page, i.e. as long as the main window.
        if (_hookedWindowClosing || Window.GetWindow(this) is not { } window)
        {
            return;
        }
        _hookedWindowClosing = true;
        window.Closing += (_, _) =>
        {
            try
            {
                ViewModel?.PromptSaveEditorBeforeLeaving("exiting");
            }
            catch (Exception ex)
            {
                ReportError("Could not save the open file", ex);
            }
        };
    }

    private void VsMods_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is VsModBrowserViewModel vm)
        {
            UpdateVsModWebBrowser(vm);
        }
    }

    private void UpdateVsModWebBrowser(VsModBrowserViewModel vsMods)
        => VsModWebBrowser.SetContext(vsMods.Services, vsMods.Toasts, vsMods.Current, vsMods.NotifyInstalled);

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName is nameof(ServerDetailViewModel.IsOnline) or nameof(ServerDetailViewModel.IsFiveMOrRedM))
            {
                if (TabTxAdmin.IsChecked == true)
                {
                    _ = TryShowTxAdminAsync();
                }
            }
            else if (e.PropertyName == nameof(ServerDetailViewModel.IsEditorOpen) && ViewModel is { } vm)
            {
                UpdateEditorLayout(vm.IsEditorOpen);
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not update the view", ex);
        }
    }

    // -- Console ------------------------------------------------------------------------------

    private bool _followConsole = true;

    private void ConsoleBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        // Content growth (extent change) isn't the user scrolling: ignore it.
        if (e.ExtentHeightChange == 0 && e.ViewportHeightChange == 0)
        {
            _followConsole = e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - 4;
        }
    }

    private void ViewModel_ConsoleUpdated(object? sender, ConsoleUpdate update)
    {
        try
        {
            var hasSelection = ConsoleBox.SelectionLength > 0;

            if (update.Reset)
            {
                ConsoleBox.Text = update.Text;
            }
            else if (update.Text.Length > 0)
            {
                ConsoleBox.AppendText(update.Text);
            }

            if (_followConsole && !hasSelection)
            {
                ScrollConsoleToEnd();
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not update the console", ex);
        }
    }

    private void ScrollConsoleToEnd()
    {
        ConsoleBox.ScrollToEnd();
        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, () =>
        {
            if (_followConsole)
            {
                ConsoleBox.ScrollToEnd();
            }
        });
    }

    // -- Tabs ---------------------------------------------------------------------------------

    private void Tab_Checked(object sender, RoutedEventArgs e)
    {
        // Loaded before InitializeComponent finishes wiring every named panel the first time
        // RadioButton.IsChecked=True fires during XAML parsing; guard against that.
        if (ConsolePanel is null)
        {
            return;
        }

        try
        {
            ConsolePanel.Visibility = Visibility.Collapsed;
            FilesPanel.Visibility = Visibility.Collapsed;
            ModsPanel.Visibility = Visibility.Collapsed;
            ResourcesPanel.Visibility = Visibility.Collapsed;
            TxAdminPanel.Visibility = Visibility.Collapsed;
            KleiPanel.Visibility = Visibility.Collapsed;
            SettingsPanel.Visibility = Visibility.Collapsed;
            ResourcePackPanel.Visibility = Visibility.Collapsed;
            LuckPermsPanel.Visibility = Visibility.Collapsed;
            ScenariosPanel.Visibility = Visibility.Collapsed;

            if (ReferenceEquals(sender, TabConsole))
            {
                ConsolePanel.Visibility = Visibility.Visible;
                _followConsole = true;
                ScrollConsoleToEnd();
            }
            else if (ReferenceEquals(sender, TabFiles))
            {
                FilesPanel.Visibility = Visibility.Visible;
                ViewModel?.RefreshFilesCommand.Execute(null);
                QueueGutterUpdate();
            }
            else if (ReferenceEquals(sender, TabMods))
            {
                ModsPanel.Visibility = Visibility.Visible;
            }
            else if (ReferenceEquals(sender, TabResources))
            {
                ResourcesPanel.Visibility = Visibility.Visible;
            }
            else if (ReferenceEquals(sender, TabTxAdmin))
            {
                TxAdminPanel.Visibility = Visibility.Visible;
                _ = TryShowTxAdminAsync();
            }
            else if (ReferenceEquals(sender, TabKlei))
            {
                KleiPanel.Visibility = Visibility.Visible;
                _ = TryShowKleiAsync();
            }
            else if (ReferenceEquals(sender, TabSettings))
            {
                SettingsPanel.Visibility = Visibility.Visible;
            }
            else if (ReferenceEquals(sender, TabResourcePack))
            {
                ResourcePackPanel.Visibility = Visibility.Visible;
            }
            else if (ReferenceEquals(sender, TabLuckPerms))
            {
                LuckPermsPanel.Visibility = Visibility.Visible;
            }
            else if (ReferenceEquals(sender, TabScenarios))
            {
                ScenariosPanel.Visibility = Visibility.Visible;
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not switch tabs", ex);
        }
    }

    private async Task TryShowKleiAsync()
    {
        try
        {
            if (KleiWeb.Visibility == Visibility.Visible || ViewModel is not { IsDontStarve: true } vm)
            {
                return;
            }

            await KleiWeb.EnsureCoreWebView2Async();
            KleiWeb.Source = new Uri(vm.KleiLoginUrl);
            KleiWeb.Visibility = Visibility.Visible;
            KleiWebStatus.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            KleiWeb.Visibility = Visibility.Collapsed;
            KleiWebStatus.Visibility = Visibility.Visible;
            KleiWebStatus.Text = $"Couldn't show the Klei page here ({ex.Message}). Use Open in Browser instead.";
        }
    }

    private void ViewModel_KleiTabRequested(object? sender, EventArgs e)
    {
        if (TabKlei.Visibility == Visibility.Visible)
        {
            TabKlei.IsChecked = true;
        }
    }

    private async Task TryShowTxAdminAsync()
    {
        try
        {
            if (ViewModel is not { IsOnline: true, IsFiveMOrRedM: true } vm)
            {
                TxAdminWeb.Visibility = Visibility.Collapsed;
                TxAdminPlaceholder.Visibility = Visibility.Visible;
                TxAdminStatusText.Text = ViewModel?.IsOnline == true
                    ? "txAdmin is only available for FiveM/RedM servers."
                    : "Start the server to open the txAdmin console";
                return;
            }

            await TxAdminWeb.EnsureCoreWebView2Async();
            TxAdminWeb.Source = new Uri(vm.TxAdminUrl);
            TxAdminWeb.Visibility = Visibility.Visible;
            TxAdminPlaceholder.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            TxAdminWeb.Visibility = Visibility.Collapsed;
            TxAdminPlaceholder.Visibility = Visibility.Visible;
            TxAdminStatusText.Text = $"Couldn't embed the txAdmin console ({ex.Message}). Use the button below instead.";
        }
    }

    private void CommandBox_KeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            if (e.Key == Key.Enter && ViewModel is { } vm && vm.SendCommandCommand.CanExecute(null))
            {
                vm.SendCommandCommand.Execute(null);
                e.Handled = true;
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not send the command", ex);
        }
    }

    // -- File list ----------------------------------------------------------------------------

    private void FilesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        try
        {
            // Ignore double-clicks on the row's own buttons / empty space.
            if (e.OriginalSource is DependencyObject source &&
                ItemsControl.ContainerFromElement(FilesList, source) is ListBoxItem { DataContext: FileRowViewModel entry })
            {
                ViewModel?.OpenEntry(entry);
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not open the file", ex);
        }
    }

    private void FilesList_KeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            if (FilesList.SelectedItem is not FileRowViewModel entry)
            {
                if (e.Key == Key.Back)
                {
                    ViewModel?.UpFolderCommand.Execute(null);
                    e.Handled = true;
                }
                return;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    ViewModel?.OpenEntry(entry);
                    e.Handled = true;
                    break;
                case Key.Delete:
                    ViewModel?.DeleteEntry(entry);
                    e.Handled = true;
                    break;
                case Key.Back:
                    ViewModel?.UpFolderCommand.Execute(null);
                    e.Handled = true;
                    break;
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not complete that action", ex);
        }
    }

    private void EditFile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is FrameworkElement { DataContext: FileRowViewModel entry })
            {
                ViewModel?.OpenEntry(entry);
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not open the file", ex);
        }
    }

    private void DeleteFile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is FrameworkElement { DataContext: FileRowViewModel entry })
            {
                ViewModel?.DeleteEntry(entry);
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not delete the file", ex);
        }
    }

    private void ModsLink_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (Window.GetWindow(this) is { } window &&
                window.FindName("NavMods") is RadioButton modsNav)
            {
                modsNav.IsChecked = true;
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not open the Mods page", ex);
        }
    }

    private void ModsPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        try
        {
            ViewModel?.ModsTab.SetActive(e.NewValue is true);
        }
        catch (Exception ex)
        {
            ReportError("Could not load this server's mods", ex);
        }
    }

    private void ViewModel_FilesTabRequested(object? sender, EventArgs e) => TabFiles.IsChecked = true;

    // -- Drag and drop from Explorer --------------------------------------------------------
    // Note: Windows blocks dragging from a normal Explorer window into an app running as
    // administrator, so these only work when the app isn't elevated.

    private static string[]? DroppedPaths(DragEventArgs e)
        => e.Data.GetDataPresent(DataFormats.FileDrop) ? e.Data.GetData(DataFormats.FileDrop) as string[] : null;

    private static void ShowDropOverlay(DragEventArgs e, Border overlay, bool enabled)
    {
        var accept = enabled && e.Data.GetDataPresent(DataFormats.FileDrop);
        e.Effects = accept ? DragDropEffects.Copy : DragDropEffects.None;
        overlay.Visibility = accept ? Visibility.Visible : Visibility.Collapsed;
        e.Handled = true;
    }

    private static void HideDropOverlayIfLeft(DragEventArgs e, FrameworkElement area, Border overlay)
    {
        var p = e.GetPosition(area);
        if (p.X <= 0 || p.Y <= 0 || p.X >= area.ActualWidth || p.Y >= area.ActualHeight)
        {
            overlay.Visibility = Visibility.Collapsed;
        }
    }

    private void FilesDrop_DragOver(object sender, DragEventArgs e)
        => ShowDropOverlay(e, FilesDropOverlay, ViewModel is { Current: not null, IsCopyingFiles: false });

    private void FilesDrop_DragLeave(object sender, DragEventArgs e)
        => HideDropOverlayIfLeft(e, (FrameworkElement)sender, FilesDropOverlay);

    private async void FilesDrop_Drop(object sender, DragEventArgs e)
    {
        FilesDropOverlay.Visibility = Visibility.Collapsed;
        try
        {
            if (DroppedPaths(e) is { Length: > 0 } paths && ViewModel is { } vm)
            {
                e.Handled = true;
                await vm.CopyDroppedFilesAsync(paths);
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not copy the dropped files", ex);
        }
    }

    private void ModsDrop_DragOver(object sender, DragEventArgs e)
        => ShowDropOverlay(e, ModsDropOverlay, ViewModel is { SupportsMods: true, ModsTab.HasNotice: false, ModsTab.IsInstallingDrop: false });

    private void ModsDrop_DragLeave(object sender, DragEventArgs e)
        => HideDropOverlayIfLeft(e, (FrameworkElement)sender, ModsDropOverlay);

    private async void ModsDrop_Drop(object sender, DragEventArgs e)
    {
        ModsDropOverlay.Visibility = Visibility.Collapsed;
        try
        {
            if (DroppedPaths(e) is { Length: > 0 } paths && ViewModel is { SupportsMods: true, ModsTab.HasNotice: false } vm)
            {
                e.Handled = true;
                await vm.ModsTab.InstallDroppedAsync(paths);
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not install the dropped files", ex);
        }
    }

    // -- Built-in editor ----------------------------------------------------------------------

    private void UpdateEditorLayout(bool open)
    {
        EditorPanel.Visibility = open ? Visibility.Visible : Visibility.Collapsed;
        FileListColumn.Width = open ? new GridLength(320) : new GridLength(1, GridUnitType.Star);
        EditorColumn.Width = open ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        if (open)
        {
            QueueGutterUpdate();
        }
    }

    private void ViewModel_EditorTextLoaded(object? sender, string text)
    {
        try
        {
            _suppressEditorDirty = true;
            EditorBox.Text = text;
            EditorBox.CaretIndex = 0;
            EditorBox.ScrollToHome();
            EditorBox.UndoLimit = 0;   // clear the undo stack of the previous document...
            EditorBox.UndoLimit = 200; // ...then re-enable undo for this one
        }
        catch (Exception ex)
        {
            ReportError("Could not load the file into the editor", ex);
        }
        finally
        {
            _suppressEditorDirty = false;
        }
        if (!string.IsNullOrEmpty(text))
        {
            EditorBox.Focus();
        }
        QueueGutterUpdate();
    }

    private void EditorBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_suppressEditorDirty)
        {
            ViewModel?.MarkEditorDirty();
        }
        QueueGutterUpdate();
    }

    private void EditorBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        try
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                SaveEditor();
            }
        }
        catch (Exception ex)
        {
            ReportError("Could not save the file", ex);
        }
    }

    private void SaveEditor_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SaveEditor();
        }
        catch (Exception ex)
        {
            ReportError("Could not save the file", ex);
        }
    }

    private void SaveEditor()
    {
        if (ViewModel is { IsEditorOpen: true } vm && vm.SaveEditorCommand.CanExecute(null))
        {
            vm.SaveEditorCommand.Execute(null);
        }
    }

    private void EditorBox_ScrollChanged(object sender, ScrollChangedEventArgs e) => QueueGutterUpdate();

    private void EditorBox_SizeChanged(object sender, SizeChangedEventArgs e) => QueueGutterUpdate();

    private void QueueGutterUpdate()
    {
        if (_gutterUpdateQueued)
        {
            return;
        }
        _gutterUpdateQueued = true;
        Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        {
            _gutterUpdateQueued = false;
            try
            {
                UpdateGutter();
                UpdateCaretPosition();
            }
            catch
            {
                // Cosmetic only; never surface layout hiccups.
            }
        }));
    }

    private void UpdateGutter()
    {
        if (EditorPanel.Visibility != Visibility.Visible)
        {
            return;
        }

        var first = EditorBox.GetFirstVisibleLineIndex();
        var last = EditorBox.GetLastVisibleLineIndex();
        if (first < 0 || last < first)
        {
            GutterText.Text = "1";
            Canvas.SetTop(GutterText, EditorBox.Padding.Top);
            return;
        }

        var firstRect = EditorBox.GetRectFromCharacterIndex(EditorBox.GetCharacterIndexFromLineIndex(first));
        double lineHeight;
        if (last > first)
        {
            var secondRect = EditorBox.GetRectFromCharacterIndex(EditorBox.GetCharacterIndexFromLineIndex(first + 1));
            lineHeight = secondRect.Top - firstRect.Top;
        }
        else
        {
            lineHeight = firstRect.Height;
        }
        if (double.IsNaN(lineHeight) || double.IsInfinity(lineHeight) || lineHeight <= 0)
        {
            lineHeight = EditorBox.FontSize * EditorBox.FontFamily.LineSpacing;
        }

        var sb = new StringBuilder();
        for (var i = first; i <= last; i++)
        {
            if (i > first)
            {
                sb.Append('\n');
            }
            sb.Append(i + 1);
        }
        GutterText.LineHeight = lineHeight;
        GutterText.Text = sb.ToString();
        Canvas.SetTop(GutterText, firstRect.IsEmpty ? 0 : firstRect.Top);
    }

    private void UpdateCaretPosition()
    {
        if (EditorPanel.Visibility != Visibility.Visible)
        {
            return;
        }
        var caret = EditorBox.CaretIndex;
        var line = EditorBox.GetLineIndexFromCharacterIndex(caret);
        if (line < 0)
        {
            CaretPositionText.Text = string.Empty;
            return;
        }
        var column = caret - EditorBox.GetCharacterIndexFromLineIndex(line);
        CaretPositionText.Text = $"Ln {line + 1}, Col {column + 1}   ·   Ctrl+S to save";
    }

    private void ModrinthSearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && ViewModel?.ModsTab.Modrinth.SearchCommand is { } command && command.CanExecute(null))
        {
            e.Handled = true;
            command.Execute(null);
        }
    }

}
