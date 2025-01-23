using Content.Shared.ADT.Language;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.ADT.Language.UI;

[GenerateTypedNameReferences]
public sealed partial class LanguageEntry : Control
{
    public Action<string>? OnLanguageSelected;
    public string Language;

    public LanguageEntry(LanguagePrototype proto, bool translator)
    {
        RobustXamlLoader.Load(this);
        Name.Text = proto.LocalizedName;
        if (proto.Color.HasValue)
            Name.FontColorOverride = proto.Color.Value;
        SelectButton.ToolTip = translator ?
            Loc.GetString("language-choose-button-tooltip-translator") :
            Loc.GetString("language-choose-button-tooltip-known");
        Description.SetMessage(proto.LocalizedDescription);
        Language = proto.ID;

        SelectButton.OnPressed += _ => OnLanguageSelected?.Invoke(proto.ID);
    }

}
