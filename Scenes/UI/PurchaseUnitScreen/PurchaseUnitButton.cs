using Godot;
using System;

public partial class PurchaseUnitButton : Button
{

    [Signal]
    public delegate void PurchasePressedEventHandler(UnitDefinition unitDef);

    private UnitDefinition unitDef;

    public override void _Ready()
    {
        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        sprite.SpriteFrames = unitDef.GetSpriteFrames();
        sprite.Play("idle");

        Label name = GetNode<Label>("%Name");
        name.Text = unitDef.GetName();

        Label cost = GetNode<Label>("%Cost");
        cost.Text = unitDef.GetCost().ToString();

        Pressed += () => EmitSignal(SignalName.PurchasePressed, unitDef);
    }

    public void SetUnitDefinition(UnitDefinition unitDef) => this.unitDef = unitDef;
}
