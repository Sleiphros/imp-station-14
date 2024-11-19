using Robust.Client.UserInterface.CustomControls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._Impstation.CrewMedal.UI;

[GenerateTypedNameReferences]
public sealed partial class CrewMedalWindow : DefaultWindow
{
    public event Action<string>? OnReasonChanged;

    /// <summary>
    /// Is the user currently entering text into the control?
    /// </summary>
    private bool _focused;

    private string _reason = string.Empty;
    private bool _awarded = false;

    private int _maxCharacters = 50;

    public CrewMedalWindow()
    {
        RobustXamlLoader.Load(this);

        ReasonLineEdit.OnTextChanged += _ =>
        {
            SaveButton.Disabled = _awarded || ReasonLineEdit.Text.Length > _maxCharacters;
            CharacterLabel.Text = Loc.GetString("crew-medal-ui-character-limit", ("number", ReasonLineEdit.Text.Length), ("max", _maxCharacters));
        };
        ReasonLineEdit.OnFocusEnter += _ => _focused = true;
        ReasonLineEdit.OnFocusExit += _ => _focused = false;

        SaveButton.OnPressed += _ =>
        {
            OnReasonChanged?.Invoke(ReasonLineEdit.Text);
            SaveButton.Disabled = true;
        };

        CharacterLabel.Text = Loc.GetString("crew-medal-ui-character-limit", ("number", ReasonLineEdit.Text.Length), ("max", _maxCharacters));
    }

    public void SetCurrentReason(string reason)
    {
        if (_reason == reason)
            return;

        _reason = reason;
        if (!_focused)
            ReasonLineEdit.Text = reason;
    }

    public void SetAwarded(bool awarded)
    {
        _awarded = awarded;
        ReasonLineEdit.Editable = !awarded;
        SaveButton.Disabled = _awarded;
    }

    public void SetMaxCharacters(int number)
    {
        _maxCharacters = number;
        if (ReasonLineEdit.Text.Length > number)
            ReasonLineEdit.Text = ReasonLineEdit.Text[..number];
    }
}
