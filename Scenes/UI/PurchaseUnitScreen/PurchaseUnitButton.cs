using Godot;
using System;
using static UnitDefinition;

public partial class PurchaseUnitButton : Button
{
    private UnitDefinition unitDef;
    private Team team;

    public override void _Ready()
    {
        AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
        sprite.SpriteFrames = ResourceLoader.Load<SpriteFrames>($"res://Assets/Animations/{team}/{unitDef.GetUnitName()}.tres");
        sprite.Play("idle");

        Label name = GetNode<Label>("%Name");
        name.Text = unitDef.GetUnitName();

        Label cost = GetNode<Label>("%Cost");
        cost.Text = unitDef.GetCost().ToString();
    }

    public void SetUnitDefinition(UnitDefinition unitDef) => this.unitDef = unitDef;

    public void SetTeam(Team team) => this.team = team;
}
