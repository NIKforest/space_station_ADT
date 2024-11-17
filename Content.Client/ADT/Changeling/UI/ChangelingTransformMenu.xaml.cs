using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Speech;
using Content.Shared.Whitelist;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Content.Shared.ADT.Phantom;
using Robust.Shared.Utility;
using Content.Shared.Humanoid.Prototypes;
using Content.Client.Humanoid;
using Content.Shared.Preferences;
using Robust.Shared.Map;
using Content.Shared.Humanoid;
using System.Linq;
using Content.Shared.Changeling;

namespace Content.Client.ADT.Changeling.UI;

[GenerateTypedNameReferences]
public sealed partial class ChangelingTransformMenu : RadialMenu
{
    [Dependency] private readonly EntityManager _entManager = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    private readonly HumanoidAppearanceSystem _appearanceSystem;
    private readonly SpriteSystem _spriteSystem;
    public List<NetEntity> Forms = new();
    public ChangelingMenuType Type = ChangelingMenuType.Transform;
    public NetEntity Target = NetEntity.Invalid;

    public event Action<NetEntity>? OnSelectForm;

    public ChangelingTransformMenu()
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);

        _spriteSystem = _entManager.System<SpriteSystem>();
        _appearanceSystem = _entManager.System<HumanoidAppearanceSystem>();
    }

    public void Populate(RequestChangelingFormsMenuEvent args)
    {
        var parent = FindControl<RadialContainer>("Main");

        foreach (var item in args.HumanoidData)
        {
            if (Forms.Contains(item.NetEntity))
                return;

            var dummy = _entManager.SpawnEntity(_proto.Index(item.Profile.Species).DollPrototype, MapCoordinates.Nullspace);
            _appearanceSystem.LoadProfile(dummy, item.Profile);
            var face = new SpriteView();
            face.SetEntity(dummy);

            var button = new ChangelingTransformMenuButton
            {
                StyleClasses = { "RadialMenuButton" },
                SetSize = new Vector2(64f, 64f),
                ToolTip = Loc.GetString(item.Name ?? String.Empty),
                Entity = item.NetEntity,
                Profile = item.Profile,
                Name = item.Name,
            };

            face.Scale *= 1f;
            button.AddChild(face);
            parent.AddChild(button);
            Forms.Add(item.NetEntity);
        }
        foreach (var child in Children)
        {
            if (child is not RadialContainer container)
                continue;
            AddLingClickAction(container);
        }
    }
    private void AddLingClickAction(RadialContainer container)
    {
        foreach (var child in container.Children)
        {
            if (child is not ChangelingTransformMenuButton castChild)
                continue;

            castChild.OnButtonUp += _ =>
            {
                OnSelectForm?.Invoke(castChild.Entity);
            };
        }
    }
}


public sealed class ChangelingTransformMenuButton : RadialMenuTextureButton
{
    public NetEntity Entity;
    public HumanoidCharacterProfile? Profile;
    public string? Name;
}
