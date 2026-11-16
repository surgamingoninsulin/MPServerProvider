using System.IO;
using System.Windows;

namespace MPServerProvider.App.Views.Dialogs;

public partial class LicenseKeyDialog : DialogWindow
{
    private const string PlaceholderKey = "changeme";
    private const int MinKeyLength = 8;

    private readonly string _serverCfgPath;
    private readonly Action<string>? _onSave;

    public LicenseKeyDialog(string serverCfgPath, Action<string>? onSave = null)
    {
        InitializeComponent();
        _serverCfgPath = serverCfgPath;
        _onSave = onSave;
        try
        {
            KeyBox.Text = ReadKey(serverCfgPath);
        }
        catch (Exception ex)
        {
            ErrorText.Text = $"Could not read server.cfg: {ex.Message}";
            ErrorText.Visibility = Visibility.Visible;
        }
        Loaded += (_, _) => KeyBox.Focus();
    }

    public static bool Show(string serverCfgPath, Action<string>? onSave = null, Window? owner = null)
    {
        var dialog = new LicenseKeyDialog(serverCfgPath, onSave);
        if (owner is not null && owner.IsLoaded)
        {
            dialog.Owner = owner;
        }
        return dialog.ShowDialog() == true;
    }

    public static string ReadKey(string serverCfgPath)
    {
        if (!File.Exists(serverCfgPath))
        {
            return string.Empty;
        }

        foreach (var line in File.ReadAllLines(serverCfgPath))
        {
            var trimmed = line.TrimStart();
            if (!trimmed.StartsWith("sv_licenseKey", StringComparison.Ordinal))
            {
                continue;
            }

            var value = trimmed["sv_licenseKey".Length..].Trim().Trim('"').Trim();
            if (value.Length == 0 || value.Equals(PlaceholderKey, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return value;
        }

        return string.Empty;
    }

    public static void WriteKey(string serverCfgPath, string key)
    {
        var existingLines = File.Exists(serverCfgPath) ? File.ReadAllLines(serverCfgPath).ToList() : new List<string>();
        var newLine = $"sv_licenseKey \"{key}\"";

        var found = false;
        for (var i = 0; i < existingLines.Count; i++)
        {
            if (existingLines[i].TrimStart().StartsWith("sv_licenseKey", StringComparison.Ordinal))
            {
                existingLines[i] = newLine;
                found = true;
            }
        }

        if (!found)
        {
            existingLines.Add(newLine);
        }

        var parent = Path.GetDirectoryName(serverCfgPath);
        if (!string.IsNullOrEmpty(parent))
        {
            Directory.CreateDirectory(parent);
        }

        File.WriteAllLines(serverCfgPath, existingLines);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var key = KeyBox.Text.Trim();
        if (key.Length < MinKeyLength)
        {
            ErrorText.Text = "That doesn't look like a valid licence key";
            ErrorText.Visibility = Visibility.Visible;
            return;
        }

        try
        {
            WriteKey(_serverCfgPath, key);
            _onSave?.Invoke(key);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            ErrorText.Text = $"Could not save the key: {ex.Message}";
            ErrorText.Visibility = Visibility.Visible;
            ToastError($"Could not save the licence key: {ex.Message}");
        }
    }
}
