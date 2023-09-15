using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
    
    public enum Team
    {
        OrangeStar,
        BlackHole,
    }

    [Signal]
    public delegate void HealthChangedEventHandler(int newHealth);
    
    [Signal]
    public delegate void UnitLoadedOrUnloadedEventHandler();

    [Signal]
    public delegate void ExhaustedChangedEventHandler();
    
    [Export] private string name;
    [Export] private int cost;
    [Export] private UnitType type;
    private int health = 100; // should health be defined here or in the Unit class?
    [Export] private Team team; // used for animations
    [Export] private bool enemy;

    private List<Unit> unitsLoaded = new List<Unit>(); // a list of the Node Unit (that exists in the node tree) instead of UnitDefinitions
    [Export] private int loadCapacity;
    [Export] private string[] loadRules; // list of unit names that are allowed to load onto this unit

    private Vector2I gridPos;
    [Export] private MoveDefinition moveDef;
    private bool exhausted;
    private bool moved;
    
    [Export] private Weapon primaryWeap;
    [Export] private Weapon secondaryWeap;
    [Export] private bool directCombat = true;
    [Export] private int minRange = 1;
    [Export] private int maxRange = 1;

    public string GetName() => name;

    public int GetCost() => cost;

    public UnitType GetType() => type;

    public int GetHealth() => health;
    
    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0) health = 0;
        GD.Print($"{GetName()} took {amount} damage. Health is now at {health}");
        EmitSignal(SignalName.HealthChanged, health);
    }

    public Team GetTeam() => team;

    public void SetTeam(Team team) => this.team = team; 

    public bool IsEnemy() => enemy;

    public void SetEnemy(bool enemy) => this.enemy = enemy;

    public int GetLoadCapacity() => loadCapacity;

    public string[] GetLoadRules() => loadRules;
    
    public bool IsLoadCapacityFull() => unitsLoaded.Count >= loadCapacity;

    public bool HasUnitsLoaded() => unitsLoaded.Count != 0;

    public List<Unit> GetLoadedUnits() => unitsLoaded;
    
    public bool CanLoadUnit(Unit unit)
    {
        return !IsLoadCapacityFull() && loadRules.Contains(unit.GetName());
    }

    public void LoadUnit(Unit unit)
    {
        unitsLoaded.Add(unit);
        EmitSignal(SignalName.UnitLoadedOrUnloaded);
    }

    public void UnloadUnit(Unit unit)
    {
        unitsLoaded.Remove(unit);
        EmitSignal(SignalName.UnitLoadedOrUnloaded);
    }

    public Vector2I GetGridPosition() => gridPos;

    public void SetGridPosition(Vector2I gridPosition) => gridPos = gridPosition;

    public MoveDefinition GetMoveDefinition() => moveDef;

    public void SetMoveDefinition(MoveDefinition moveDef) => this.moveDef = moveDef;
    
    public bool IsExhausted() => exhausted;

    public void SetExhausted(bool exhausted)
    {
        this.exhausted = exhausted;
        EmitSignal(SignalName.ExhaustedChanged);
    }

    public bool HasAlreadyMoved() => moved;

    public void SetMoved(bool moved) => this.moved = moved;

    public Weapon GetPrimaryWeapon() => primaryWeap;

    public Weapon GetSecondaryWeapon() => secondaryWeap;

    public bool IsDirectCombat() => directCombat;

    public int GetMinWeaponRange() => minRange;

    public int GetMaxWeaponRange() => maxRange;
    
    public bool CanShootAt(Unit defendingUnit)
    {
        
        if (primaryWeap != null)
        {
            return primaryWeap.GetBaseDamageAgainstUnit(defendingUnit) != -1;
        }
        if (secondaryWeap != null)
        {
            return secondaryWeap.GetBaseDamageAgainstUnit(defendingUnit) != -1;
        }

        return false;
    }
    
    public SpriteFrames GetSpriteFrames()
    {
        string path = $"res://Assets/Animations/{GetTeam().ToString()}/{GetName()}.tres";
        // GD.Print($"fetching sprite frames at {path}");
        return (SpriteFrames) GD.Load(path);
    }
    
}
