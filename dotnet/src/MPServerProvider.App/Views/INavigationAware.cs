namespace MPServerProvider.App.Views;

/// <summary>
/// Port of <c>MPServerProvider.App.Views.INavigationAware</c> (decompile token 0x02000011): a
/// page lifecycle hook. <see cref="MainWindow"/> calls <see cref="OnNavigatedTo"/> on a page's
/// <c>UserControl</c> (when it implements this interface) every time the sidebar navigates into
/// it, mirroring the original's real navigation-aware pages (e.g. Mods/Settings re-pulling live
/// data on every visit instead of only once at startup — see the proven Rust reference's
/// <c>main.rs</c> <c>wire_navigation</c> comment for the same behavior).
/// </summary>
public interface INavigationAware
{
    void OnNavigatedTo();

    /// <summary>
    /// Called on the page being left when the shell navigates elsewhere, so pages can stop
    /// their polling timers (which otherwise kept hitting the disk/WMI for the app's lifetime,
    /// even while minimized). Default: nothing to do.
    /// </summary>
    void OnNavigatedFrom()
    {
    }
}
