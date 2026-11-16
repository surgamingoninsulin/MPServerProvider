using System.IO;
using System.Windows;
using MPServerProvider.App.Toasts;
using MPServerProvider.Core.Json;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Views.Dialogs;

/// <summary>
/// Port of <c>AddCustomGameDialog</c>'s "Add to Library" flow (0x02000008): builds a
/// <see cref="GameDefinition"/> from the form, writes it to <c>{GamesDir}/{id}.json</c> — the
/// same on-disk shape <see cref="GameLibraryService.Reload"/> reads back — then reloads the
/// catalog. <see cref="GameLibraryService"/> has no dedicated "add custom game" API of its own
/// (confirmed against its real public surface: <c>Games</c>/<c>Find</c>/<c>AllGenres</c>/
/// <c>Reload</c> only), so this writes the manifest file directly exactly like the proven Rust
/// port's <c>dialogs/add_custom_game.rs</c> had to.
/// </summary>
public partial class AddCustomGameDialog : DialogWindow
{
    private readonly AppServices _services;
    private readonly ToastService? _toasts;

    public AddCustomGameDialog(AppServices services, ToastService? toasts)
    {
        _services = services;
        _toasts = toasts;
        InitializeComponent();
    }

    private async void FetchSteamInfoButton_Click(object sender, RoutedEventArgs e)
    {
        if (!uint.TryParse(SteamAppIdBox.Text.Trim(), out var appId))
        {
            StatusText.Text = "Enter a numeric Steam app id first.";
            return;
        }

        StatusText.Text = "Fetching Steam artwork…";
        try
        {
            var adapter = new CachePathsAdapter(_services.Paths);
            var path = await _services.Artwork.TryFillFromStoreAsync(adapter, (int)appId);
            if (path is { } p)
            {
                ImageUrlBox.Text = p;
                StatusText.Text = string.Empty;
                _toasts?.Success("Fetched Steam header artwork.");
            }
            else
            {
                StatusText.Text = "Steam has no artwork for that app id.";
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't fetch Steam info: {ex.Message}";
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        GameDefinition game;
        try
        {
            game = BuildGameDefinition();
        }
        catch (ArgumentException ex)
        {
            StatusText.Text = ex.Message;
            return;
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't build the game definition: {ex.Message}";
            _toasts?.Error(StatusText.Text);
            return;
        }

        try
        {
            var path = Path.Combine(_services.Paths.GamesDir, $"{game.Id}.json");
            if (File.Exists(path))
            {
                StatusText.Text = $"A game with id '{game.Id}' already exists.";
                return;
            }

            Directory.CreateDirectory(_services.Paths.GamesDir);
            File.WriteAllText(path, Json.Serialize(game));

            _services.Games.Reload();
            _toasts?.Success($"Added '{game.Name}' to the library.");
            if (IsLoaded)
            {
                DialogResult = true;
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Couldn't save the game definition: {ex.Message}";
            _toasts?.Error(StatusText.Text);
        }
    }

    private GameDefinition BuildGameDefinition()
    {
        var name = NameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Enter a game name.");
        }

        var rawId = IdBox.Text.Trim();
        var id = Slugify(string.IsNullOrWhiteSpace(rawId) ? name : rawId);
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Enter a game ID (letters, numbers, dashes).");
        }

        _ = int.TryParse(DefaultPortBox.Text.Trim(), out var port);

        var steamAppIdText = SteamAppIdBox.Text.Trim();
        InstallSpec install;
        if (!string.IsNullOrWhiteSpace(steamAppIdText) && int.TryParse(steamAppIdText, out var appId))
        {
            install = new InstallSpec
            {
                Provider = "steamcmd",
                AppId = appId,
                Anonymous = true,
            };
        }
        else
        {
            install = new InstallSpec { Provider = "manual" };
        }

        var genres = GenresBox.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        return new GameDefinition
        {
            ManifestVersion = 1,
            Id = id,
            Name = name,
            Description = DescriptionBox.Text.Trim(),
            Genres = genres,
            Artwork = new ArtworkSpec
            {
                Cover = NonEmpty(ImageUrlBox.Text),
            },
            Install = install,
            Runtime = new RuntimeSpec
            {
                Executable = string.IsNullOrWhiteSpace(ExecutableBox.Text) ? "{server_file}" : ExecutableBox.Text.Trim(),
                Arguments = ArgumentsBox.Text.Trim(),
                WorkingDir = "{server_root}",
            },
            Network = new NetworkSpec
            {
                DefaultPort = port,
                Protocol = "both",
            },
            Mods = new ModsSpec
            {
                Supported = false,
                Folder = "mods",
            },
            IsBuiltIn = false,
        };
    }

    private static string? NonEmpty(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static string Slugify(string s)
    {
        var lowered = s.Trim().ToLowerInvariant();
        var chars = lowered.Select(c => char.IsAsciiLetterOrDigit(c) ? c : '-').ToArray();
        var parts = new string(chars).Split('-', StringSplitOptions.RemoveEmptyEntries);
        return string.Join('-', parts);
    }

    private sealed class CachePathsAdapter(AppPaths paths) : ArtworkCacheService.ICachePaths
    {
        public string ArtworkCacheDir => paths.ArtworkCacheDir;
    }
}
