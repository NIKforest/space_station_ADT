using Content.Client.UserInterface.Controls;
using Content.Shared.Access.Systems;
using Content.Shared.ADT.Shuttles;
using Content.Shared.ADT.Shuttles.Components;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Timing;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Client.ADT.Shuttles.UI;

[GenerateTypedNameReferences]
public sealed partial class DockingConsoleWindow : FancyWindow
{
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    private readonly AccessReaderSystem _access;

    public event Action<int>? OnFTL;

    private EntityUid _owner;
    private int? _selected;
    private FTLState _state;
    private StartEndTime _ftlTime;
    private StyleBoxFlat _ftlStyle;

    public DockingConsoleWindow(EntityUid owner)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        _access = _entMan.System<AccessReaderSystem>();

        _owner = owner;

        _ftlStyle = new StyleBoxFlat(Color.LimeGreen);
        FTLBar.ForegroundStyleBoxOverride = _ftlStyle;

        if (!_entMan.TryGetComponent<DockingConsoleComponent>(owner, out var comp))
            return;

        Title = Loc.GetString(comp.WindowTitle);

        if (!comp.HasShuttle)
        {
            MapFTLState.Text = Loc.GetString("docking-console-no-shuttle");
            _ftlStyle.BackgroundColor = Color.FromHex("#B02E26");
            return;
        }

        Destinations.OnItemSelected += args => _selected = args.ItemIndex;
        Destinations.OnItemDeselected += _ => _selected = null;

        FTLButton.OnPressed += _ =>
        {
            if (_selected is {} index)
                OnFTL?.Invoke(index);
        };
    }

    public void UpdateState(DockingConsoleState state)
    {
        _state = state.FTLState;
        _ftlTime = state.FTLTime;

        MapFTLState.Text = Loc.GetString($"shuttle-console-ftl-state-{_state.ToString()}");
        _ftlStyle.BackgroundColor = Color.FromHex(_state switch
        {
            FTLState.Available => "#80C71F",
            FTLState.Starting => "#169C9C",
            FTLState.Travelling => "#8932B8",
            FTLState.Arriving => "#F9801D",
            _ => "#B02E26" // cooldown and fallback
        });

        UpdateButton();

        if (Destinations.Count == state.Destinations.Count)
            return;

        Destinations.Clear();
        foreach (var dest in state.Destinations)
        {
            Destinations.AddItem(dest.Name);
        }
    }

    private void UpdateButton()
    {
        FTLButton.Disabled = _selected == null || _state != FTLState.Available || !HasAccess();
    }

    private bool HasAccess()
    {
        return _player.LocalSession?.AttachedEntity is {} player && _access.IsAllowed(player, _owner);
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        UpdateButton();

        var progress = _ftlTime.ProgressAt(_timing.CurTime);
        FTLBar.Value = float.IsFinite(progress) ? progress : 1;
    }
}
