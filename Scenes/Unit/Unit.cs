using Godot;
using System;
using System.Collections.Generic;
using static UnitDefinition;

// Unit is basically a wrapper around UnitDefinition that extends from Node so it can exist in the nodetree
public partial class Unit : Node
{

    [Signal]
    public delegate void HealthChangedEventHandler(int newHealth);

    [Signal]
    public delegate void UnitLoadedOrUnloadedEventHandler();

    [Signal]
    public delegate void ExhaustedChangedEventHandler();

    private BaseUnit baseUnit;
    private UnitDefinition unitDef;

    private UnitPathFollower pathFollower;
    
    private List<BaseAction> unitActions; // TODO: refactor the Action system

    public override void _Ready()
    {
        baseUnit = GetParent<BaseUnit>();
        // propagate signals from UnitDefinition up
        unitDef.HealthChanged += (newHealth) => EmitSignal(SignalName.HealthChanged, newHealth);
        unitDef.UnitLoadedOrUnloaded += () => EmitSignal(SignalName.UnitLoadedOrUnloaded);
        unitDef.ExhaustedChanged += () => EmitSignal(SignalName.ExhaustedChanged);
        
        pathFollower = baseUnit.GetNode<UnitPathFollower>("UnitPathFollower"); // TODO: move this to BaseUnit
        
        unitActions = new List<BaseAction>();
        foreach (Node child in baseUnit.GetNode("Actions").GetChildren())
        {
            BaseAction action = child as BaseAction;
            action.SetUnit(this);
            unitActions.Add(action);
        }
        
        
        TurnSystem.Instance.TurnChanged += HandleTurnChanged;
    }

    public Vector2I GetGridPosition() => unitDef.GetGridPosition();

    public void SetGridPosition(Vector2I gridPos)
    {
        baseUnit.Position = Level.Instance.MapToLocal(gridPos);
        unitDef.SetGridPosition(gridPos);
    }
    
    public Vector2 GetPosition() => baseUnit.Position;

    public void SetPosition(Vector2 pos) => baseUnit.Position = pos;

    public List<BaseAction> GetActions() => unitActions;

    public bool IsEnemy() => unitDef.IsEnemy();

    // public void SetEnemy(bool enemy) => unitDef.SetEnemy(enemy);

    public int GetHealth() => unitDef.GetHealth();

    public void Damage(int amount) => unitDef.Damage(amount);

    public MoveDefinition GetMoveDefinition() => unitDef.GetMoveDefinition();

    // public void SetMoveDefinition(MoveDefinition moveDef) => unitDef.SetMoveDefinition(moveDef);

    // public Node2D GetBaseUnit() => baseUnit;

    public string GetName() => unitDef.GetUnitName();

    // public void SetUnitName(string unitName) => this.unitName = unitName;
    
    // calling it GetType() would hide another function
    public UnitType GetUnitType() => unitDef.GetUnitType();

    public Team GetTeam() => unitDef.GetTeam();

    // public void SetTeam(UnitDefinition.Team team) => this.team = team;

    public int GetLoadCapacity() => unitDef.GetLoadCapacity();

    public bool IsLoadCapacityFull() => unitDef.IsLoadCapacityFull();

    public bool HasUnitsLoaded() => unitDef.HasUnitsLoaded();

    public List<Unit> GetLoadedUnits() => unitDef.GetLoadedUnits();

    public void LoadUnit(Unit unit) => unitDef.LoadUnit(unit);

    public void UnloadUnit(Unit unit) => unitDef.UnloadUnit(unit);

    public bool CanLoadUnit(Unit unit) => unitDef.CanLoadUnit(unit);

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

    public bool IsExhausted() => unitDef.IsExhausted();

    public void SetExhausted(bool exhausted) => unitDef.SetExhausted(exhausted);

    public bool HasAlreadyMoved() => unitDef.HasAlreadyMoved();

    public void SetMoved(bool moved) => unitDef.SetMoved(moved);
    
    private void HandleTurnChanged()
    {
        SetExhausted(false);
        SetMoved(false);
        foreach (BaseAction action in GetActions())
        {
            action.SetDisabled(false);
        }
    }

    public Weapon GetPrimaryWeapon() => unitDef.GetPrimaryWeapon();

    // public void SetPrimaryWeapon(Weapon weapon)
    // {
    //     weapon.SetCurrentAmmo(weapon.GetMaxAmmo()); // I have to do this here. Running into issues with the Weapon() constructor
    //     primaryWeapon = weapon;
    // }

    public Weapon GetSecondaryWeapon() => unitDef.GetSecondaryWeapon(); // don't need to set the current ammo for secondary weapons as they have infinite ammo

    // public void SetSecondaryWeapon(Weapon weapon) => secondaryWeapon = weapon;

    public bool CanShootAt(Unit defendingUnit) => unitDef.CanShootAt(defendingUnit);

    public bool IsDirectCombat() => unitDef.IsDirectCombat();

    // defines whether a unit is direct combat or indirect combat. direct combat units have a range of 1. 
    // public void SetDirectCombat(bool directCombat, int minWeaponRange = 1, int maxWeaponRange = 1)
    // {
    //     this.directCombat = directCombat;
    //     this.minWeaponRange = minWeaponRange;
    //     this.maxWeaponRange = maxWeaponRange;
    // }

    public int GetMinWeaponRange() => unitDef.GetMinWeaponRange();

    public int GetMaxWeaponRange() => unitDef.GetMaxWeaponRange();

    public void SetUnitDefinition(UnitDefinition unitDef) => this.unitDef = unitDef;

    public UnitDefinition GetUnitDefinition() => unitDef;

    public int GetVisualHealth() => unitDef.GetVisualHealth();
}