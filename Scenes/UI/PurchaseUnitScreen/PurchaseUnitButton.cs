using Godot;
using System;

public partial class PurchaseUnitButton : Button
{
    private UnitDefinition unitDef;

    public override void _Ready()
    {
        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        sprite.SpriteFrames = unitDef.GetSpriteFrames();
        sprite.Play("idle");

        Label name = GetNode<Label>("%Name");
        name.Text = unitDef.GetUnitName();

        Label cost = GetNode<Label>("%Cost");
        cost.Text = unitDef.GetCost().ToString();
    }

    public void SetUnitDefinition(UnitDefinition unitDef) => this.unitDef = unitDef;
}
