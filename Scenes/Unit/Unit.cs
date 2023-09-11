using Godot;
using System;
using System.Collections.Generic;

public partial class Unit : Node
{
    
    [Signal]
    public delegate void HealthChangedEventHandler(int newHealth);

    [Signal]
    public delegate void UnitLoadedOrUnloadedEventHandler();
    
    [Signal]
    public delegate void ExhaustedChangedEventHandler();
    
    private Node2D baseUnit;
    
    private Vector2I gridPos;

    private UnitPathFollower pathFollower;
    
    [Export] private int shootDistance = 3;

    private List<BaseAction> unitActions;

    private bool enemy;

    [Export] private int health = 100;

    [Export] private MoveDefinition moveDef;

    private string unitName;

    private int loadCapacity = 0;
    private List<Unit> unitsLoaded;
    private Func<Unit, bool> loadRules;

    private bool exhausted;
    private bool moved;

    // weapons are set by the parent script, just like moveDef
    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;
    private bool directCombat;
    // if this unit is a direct combat unit, minRange and maxRange will be the same value (1)
    private int minWeaponRange;
    private int maxWeaponRange;
    
    public override void _Ready()
    {
        baseUnit = GetParent<Node2D>(); // assume anything that has a Unit component is a Node2D
        pathFollower = baseUnit.GetNode<UnitPathFollower>("UnitPathFollower");
        
        unitActions = new List<BaseAction>();
        foreach (Node child in baseUnit.GetNode("Actions").GetChildren())
        {
            BaseAction action = child as BaseAction;
            action.SetUnit(this);
            unitActions.Add(action);
        }

        gridPos = Level.Instance.GetGridPosition(baseUnit.Position);
        GD.Print($"set gridPos to {gridPos}");
        unitsLoaded = new List<Unit>();

        exhausted = false;
        moved = false;
        TurnSystem.Instance.TurnChanged += HandleTurnChanged;
        
        // alreadyMoved = false;
    }

    public Vector2I GetGridPosition() => gridPos;

    public void SetGridPosition(Vector2I gridPos)
    {
        baseUnit.Position = Level.Instance.MapToLocal(gridPos);
        this.gridPos = gridPos;
    }
    
    public Vector2 GetPosition() => baseUnit.Position;

    public void SetPosition(Vector2 pos) => baseUnit.Position = pos;

    public int GetShootDistance() => shootDistance;

    public List<BaseAction> GetActions() => unitActions;

    public bool IsEnemy() => enemy;

    public void SetEnemy(bool enemy) => this.enemy = enemy;

    public int GetHealth() => health;

    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0) health = 0;
        GD.Print($"{GetUnitName()} took {amount} damage. Health is now at {health}");
        EmitSignal(SignalName.HealthChanged, health);
    }

    public MoveDefinition GetMoveDefinition() => moveDef;

    public void SetMoveDefinition(MoveDefinition moveDef) => this.moveDef = moveDef;

    public Node2D GetBaseUnit() => baseUnit;

    public string GetUnitName() => unitName;

    public void SetUnitName(string unitName) => this.unitName = unitName;

    public int GetLoadCapacity() => loadCapacity;

    public void SetLoadCapacity(int loadCapacity) => this.loadCapacity = loadCapacity;
    
    // I don't think this shit should be in here, it should be in loadaction
    public bool IsLoadCapacityFull() => unitsLoaded.Count >= loadCapacity;

    public bool HasUnitsLoaded() => unitsLoaded.Count != 0;

    public List<Unit> GetLoadedUnits() => unitsLoaded;

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

    public void SetLoadRules(Func<Unit, bool> loadRules) => this.loadRules = loadRules;
    
    public bool CanLoadUnit(Unit unit)
    {
        UnloadAction unloadAction = GetAction<UnloadAction>();
        if (unloadAction == null) return false;
        return !IsLoadCapacityFull() && loadRules(unit);
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction action in unitActions)
        {
            if (action is T theAction) return theAction;
            // could also do it like
            // if (action is T) return (T) action;
            // or this
            // if (action is T) return action as T;

        }
        return null;
    }

    public void SetHidden(bool hidden)
    {
        baseUnit.Visible = !hidden;
    }

    public bool IsExhausted() => exhausted;

    public void SetExhausted(bool exhausted)
    {
        this.exhausted = exhausted;
        EmitSignal(SignalName.ExhaustedChanged);
    }

    public bool HasAlreadyMoved() => moved;

    public void SetMoved(bool moved) => this.moved = moved;
    
    private void HandleTurnChanged()
    {
        SetExhausted(false);
        moved = false;
        foreach (BaseAction action in GetActions())
        {
            action.SetDisabled(false);
        }
    }

    public Weapon GetPrimaryWeapon() => primaryWeapon;

    public void SetPrimaryWeapon(Weapon weapon)
    {
        weapon.SetCurrentAmmo(weapon.GetMaxAmmo()); // I have to do this here. Running into issues with the Weapon() constructor
        primaryWeapon = weapon;
    }

    public Weapon GetSecondaryWeapon() => secondaryWeapon; // don't need to set the current ammo for secondary weapons as they have infinite ammo

    public void SetSecondaryWeapon(Weapon weapon) => secondaryWeapon = weapon;

    public bool CanShootAt(Unit defendingUnit)
    {
        
        if (primaryWeapon != null)
        {
            return primaryWeapon.GetBaseDamageAgainstUnit(defendingUnit) != -1;
        }
        if (secondaryWeapon != null)
        {
            return secondaryWeapon.GetBaseDamageAgainstUnit(defendingUnit) != -1;
        }

        return false;
    }

    public bool IsDirectCombat() => directCombat;

    // defines whether a unit is direct combat or indirect combat. direct combat units have a range of 1. 
    public void SetDirectCombat(bool directCombat, int minWeaponRange = 1, int maxWeaponRange = 1)
    {
        this.directCombat = directCombat;
        this.minWeaponRange = minWeaponRange;
        this.maxWeaponRange = maxWeaponRange;
    }

    public int GetMinWeaponRange() => minWeaponRange;

    public int GetMaxWeaponRange() => maxWeaponRange;
}