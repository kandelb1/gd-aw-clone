using Godot;
using System;

public partial class ReconUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Recon");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/TiresMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(8);
        unit.SetMoveDefinition(moveDef);
        
        Weapon machineGun = (Weapon) GD.Load("res://Scenes/Weapons/Recon/MachineGun.tres");
        unit.SetSecondaryWeapon(machineGun);
        unit.SetDirectCombat(true);
    }
}
