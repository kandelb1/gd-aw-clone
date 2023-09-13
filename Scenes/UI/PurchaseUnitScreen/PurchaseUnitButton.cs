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
        sprite.SpriteFrames = GetSpriteFrames();
        sprite.Play("idle");

        Label name = GetNode<Label>("%Name");
        name.Text = unitDef.GetName();

        Label cost = GetNode<Label>("%Cost");
        cost.Text = unitDef.GetCost().ToString();

        Pressed += () => EmitSignal(SignalName.PurchasePressed, unitDef);
    }
    
    // TODO: duplicate code from UnitAnimationPlayer
    private SpriteFrames GetSpriteFrames()
    {
        string path = $"res://Assets/Animations/{unitDef.GetTeam().ToString()}/{unitDef.GetName()}.tres";
        return (SpriteFrames) GD.Load(path);
    }

    public void SetUnitDefinition(UnitDefinition unitDef) => this.unitDef = unitDef;
}
