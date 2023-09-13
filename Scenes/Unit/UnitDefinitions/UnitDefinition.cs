using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class UnitDefinition : Resource
{

    public enum UnitType
    {
        Infantry,
        Vehicle,
        Plane,
        Copter,
        Ship,
        Submarine,
    }
    
    [Export] private string name;
    [Export] private int cost;
    [Export] private UnitType type;
    private int health = 100; // should health be defined here or in the Unit class?
    [Export] private Unit.Team team; // used for animations
    [Export] private bool enemy;
    
    [Export] private int loadCapacity;
    [Export] private string[] loadRules; // list of unit names that are allowed to load onto this unit
    
    [Export] private MoveDefinition moveDef;
    [Export] private Weapon primaryWeap;
    [Export] private Weapon secondaryWeap;
    [Export] private bool directCombat = true;
    [Export] private int minRange = 1;
    [Export] private int maxRange = 1;

    public string GetName() => name;

    public int GetCost() => cost;

    public UnitType GetType() => type;

    public Unit.Team GetTeam() => team;

    public bool IsEnemy() => enemy;

    public MoveDefinition GetMoveDefinition() => moveDef;

    public Weapon GetPrimaryWeapon() => primaryWeap;

    public Weapon GetSecondaryWeapon() => secondaryWeap;
}
