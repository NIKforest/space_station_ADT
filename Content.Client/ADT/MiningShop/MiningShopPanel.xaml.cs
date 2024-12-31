﻿using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.ADT.MiningShop;

[GenerateTypedNameReferences]
public sealed partial class MiningShopPanel : Control
{
    public static readonly Color DefaultColor = Color.FromHex("#141F2F");
    public static readonly Color DefaultBorderColor = Color.FromHex("#4972A1");
    public static readonly Color DefaultHoveredColor = Color.FromHex("#4972A1");
    private static readonly Color DisabledColor = Color.FromHex("#999999");

    public Color Color = DefaultColor;
    public Color BorderColor = DefaultBorderColor;
    public Color HoveredColor = DefaultHoveredColor;

    public MiningShopPanel()
    {
        RobustXamlLoader.Load(this);

        Button.OnDrawModeChanged += UpdateColor;
        UpdateColor();
    }

    private void UpdateColor()
    {
        var panel = (StyleBoxFlat) Panel.PanelOverride!;
        if (Button.Disabled)
            panel.BackgroundColor = DisabledColor;
        else
            panel.BackgroundColor = Button.IsHovered ? HoveredColor : Color;

        panel.BorderColor = BorderColor;
    }

    protected override void EnteredTree()
    {
        base.EnteredTree();
        UpdateColor();
    }
}
