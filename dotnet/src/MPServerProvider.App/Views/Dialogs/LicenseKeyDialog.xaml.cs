using System.IO;
using System.Windows;

namespace MPServerProvider.App.Views.Dialogs;

/// <summary>
/// Port of <c>MPServerProvider.App.Views.LicenseKeyDialog</c> (decompile token 0x0200000f).
/// Despite the name, this has nothing to do with <c>SecretProtector</c>/DPAPI — the decompiled
/// <c>ReadKey</c>/<c>WriteKey</c> just plain-string-scan a Cfx.re server's <c>server.cfg</c> for
/// its <c>sv_licenseKey "..."</c> line (confirmed by the decompiled regex literal
/// <c>sv_licenseKey\s+"?([^"\r\n]*)"?</c> for reading and <c>sv_licenseKey "{key}"</c> for
/// writing — cross-checked against the already-proven Rust port's
/// <c>crates/mpsp-app/src/dialogs/license_key.rs</c>, which reached the same "plain string
/// scanning, not a regex/DPAPI dependency" conclusion). This port keeps that same plain-text
/// read/write shape and exposes it as a ready-to-call <see cref="Show"/> API; no caller exists
/// yet in this C# port (a concurrent wave owns <c>ServerDetailView</c>, which would call this
/// when a Cfx.re server's key is missing/placeholder before start).
/// </summary>
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

    /// <summary>
    /// Shows the dialog pre-filled from <paramref name="serverCfgPath"/>'s existing
    /// <c>sv_licenseKey</c> (if any), ready for a future caller (e.g. ServerDetailView) to invoke
    /// before starting a Cfx.re server whose key is missing/placeholder. <paramref name="onSave"/>
    /// is invoked with the validated, trimmed key once the user clicks "Save &amp; Start" and the
    /// key has been written to <paramref name="serverCfgPath"/>. Returns <c>true</c> if the key
    /// was saved, <c>false</c> if the dialog was cancelled.
    /// </summary>
    public static bool Show(string serverCfgPath, Action<string>? onSave = null, Window? owner = null)
    {
        var dialog = new LicenseKeyDialog(serverCfgPath, onSave);
        if (owner is not null && owner.IsLoaded)
        {
            dialog.Owner = owner;
        }
        return dialog.ShowDialog() == true;
    }

    /// <summary>Port of <c>LicenseKeyDialog.ReadKey(string serverCfgPath)</c>.</summary>
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

    /// <summary>Port of <c>LicenseKeyDialog.WriteKey(string serverCfgPath, string key)</c>.</summary>
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

    /// <summary>Port of <c>LicenseKeyDialog.OnSave</c>'s length validation.</summary>
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
