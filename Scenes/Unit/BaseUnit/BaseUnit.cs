using Godot;
using System;
using System.Linq;

public partial class BaseUnit : Node2D
{
    private UnitDefinition unitDef;
    private Unit unit; // Unit component
    private UnitAnimationPlayer animPlayer; // UnitAnimationPlayer component

    private Vector2I startPos;

    public override void _Ready()
    {
        // set up the unit component
        unit = GetNode<Unit>("Unit");
        unit.SetUnitDefinition(unitDef);
        unit.SetUnitName(unitDef.GetName());
        unit.SetEnemy(unitDef.IsEnemy());
        unit.SetTeam(unitDef.GetTeam());
        
        unit.SetLoadCapacity(unitDef.GetLoadCapacity());
        unit.SetLoadRules(u => unitDef.GetLoadRules().Contains(u.GetUnitName()));
        
        unit.SetMoveDefinition(unitDef.GetMoveDefinition());
        if (unitDef.GetPrimaryWeapon() != null) // im sure theres a cool way to use the c# ?? operator but im not looking into it rn
        {
            unit.SetPrimaryWeapon(unitDef.GetPrimaryWeapon());    
        }
        if (unitDef.GetSecondaryWeapon() != null)
        {
            unit.SetSecondaryWeapon(unitDef.GetSecondaryWeapon());    
        }
        unit.SetDirectCombat(unitDef.IsDirectCombat(), unitDef.GetMinWeaponRange(), unitDef.GetMaxWeaponRange());
        
        unit.SetGridPosition(startPos);
        
        // set up the animation player component
        animPlayer = GetNode<UnitAnimationPlayer>("UnitAnimationPlayer");
        animPlayer.SetSpriteFrames(unitDef.GetSpriteFrames());
        // animPlayer.PlayAnimation("idle");
    }

    public void SetUnitDefinition(UnitDefinition unitDef) => this.unitDef = unitDef;

    public void SetStartPosition(Vector2I startPos) => this.startPos = startPos;
}
