using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;

namespace MPServerProvider.App.Controls;

/// <summary>
/// A clickable card <see cref="Border"/> that also works without a mouse: it takes keyboard
/// focus (Enter/Space open it) and exposes a UI Automation Invoke pattern, so screen readers
/// and automation tools can open it like a button.
/// </summary>
internal sealed class InvokableCard : Border
{
    public InvokableCard()
    {
        Focusable = true;
        FocusVisualStyle = null;
    }

    /// <summary>Raised when the card is activated from the keyboard or UI Automation.</summary>
    public event EventHandler? Invoked;

    internal void RaiseInvoked() => Invoked?.Invoke(this, EventArgs.Empty);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!e.Handled && (e.Key == Key.Enter || e.Key == Key.Space) && e.OriginalSource == this)
        {
            e.Handled = true;
            RaiseInvoked();
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new Peer(this);

    private sealed class Peer(InvokableCard owner) : FrameworkElementAutomationPeer(owner), IInvokeProvider
    {
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Button;

        protected override string GetClassNameCore() => nameof(InvokableCard);

        protected override bool IsContentElementCore() => true;

        protected override bool IsControlElementCore() => true;

        public override object? GetPattern(PatternInterface patternInterface)
            => patternInterface == PatternInterface.Invoke ? this : base.GetPattern(patternInterface);

        public void Invoke()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            owner.Dispatcher.BeginInvoke(owner.RaiseInvoked);
        }
    }
}
