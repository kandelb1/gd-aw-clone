using Godot;
using System;
using System.Linq;

public partial class BaseUnit : Node2D
{
    private UnitDefinition unitDef;
    private Unit unit; // Unit component
    private UnitAnimationPlayer animPlayer; // UnitAnimationPlayer component

    private Vector2I startPos;
    
    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitDefinition(unitDef); // need to do this right away
        animPlayer = GetNode<UnitAnimationPlayer>("UnitAnimationPlayer");
    }

    public override void _Ready()
    {
        // set up the unit component
        unit.SetGridPosition(startPos);
        
        // set up the animation player component
        animPlayer.SetSpriteFrames(unitDef.GetSpriteFrames());
        animPlayer.PlayAnimation("idle");
    }

    public void SetUnitDefinition(UnitDefinition unitDef) => this.unitDef = unitDef;

    public void SetStartPosition(Vector2I startPos) => this.startPos = startPos;
}
