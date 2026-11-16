using System.Text.RegularExpressions;
using System.Windows;
using MPServerProvider.Core.Services;

namespace MPServerProvider.App.Views.Dialogs;

public partial class AddFirewallRuleDialog : DialogWindow
{
    private static readonly Regex RuleNameRegex = new(@"^[\w .()\-]{1,64}$", RegexOptions.CultureInvariant);

    private readonly FirewallService _firewall;
    private readonly SettingsService _settings;

    public AddFirewallRuleDialog(FirewallService firewall, SettingsService settings)
    {
        InitializeComponent();
        _firewall = firewall;
        _settings = settings;
        Loaded += (_, _) => NameBox.Focus();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var name = NameBox.Text.Trim();
            if (name.Length == 0)
            {
                ShowError("Enter a rule name");
                return;
            }

            // The name ends up inside name="..." of an elevated netsh call: only allow a safe
            // character set (no quotes) so it can't inject extra netsh parameters.
            if (!RuleNameRegex.IsMatch(name))
            {
                ShowError("Rule names may only use letters, numbers, spaces, . ( ) - and _ (max 64 characters)");
                return;
            }

            if (!int.TryParse(PortBox.Text.Trim(), out var port) || port < 1 || port > 65535)
            {
                ShowError("Enter a valid port (1–65535)");
                return;
            }

            // Combo box index -> protocol string (ComboBoxItem order in
            // AddFirewallRuleDialog.xaml: Both, TCP, UDP).
            var protocol = ProtocolCombo.SelectedIndex switch
            {
                1 => "tcp",
                2 => "udp",
                _ => "both",
            };

            AddButton.IsEnabled = false;
            try
            {
                var rule = await _firewall.AddRuleAsync(name, port, protocol);
                _settings.Update(s => s.FirewallRules.Add(rule));
                DialogResult = true;
                Close();
            }
            finally
            {
                AddButton.IsEnabled = true;
            }
        }
        catch (Exception ex)
        {
            ShowError($"Couldn't add firewall rule: {ex.Message}");
            ToastError($"Couldn't add firewall rule: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }
}
