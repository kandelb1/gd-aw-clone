using Godot;
using System;

public partial class InfantryUnit : Node2D
{

    [Export] private bool isEnemy;
    private Unit unit;

    public override void _Ready()
    {
        unit = GetNode<Unit>("Unit");
        // configure things specific to this unit, like its movement and weapons
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/InfantryMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(3);
        unit.SetMoveDefinition(moveDef);
        unit.SetUnitName("Infantry");

        Weapon machineGun = (Weapon) GD.Load("res://Scenes/Weapons/Infantry/MachineGun.tres").Duplicate();
        unit.SetSecondaryWeapon(machineGun);
        unit.SetDirectCombat(true);

        // AnimatedSprite2D sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        // sprite.SpriteFrames = (SpriteFrames) GD.Load("res://Assets/Animations/OrangeStarInfantry.tres");
    }
}
