using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class BaseAction : Node
{

    protected Unit unit;
    protected bool targeting;
    protected bool active;
    private bool disabled = false;

    public static PackedScene ACTION_BUTTON = ResourceLoader.Load<PackedScene>("res://Scenes/UI/UnitActionMenu/UnitActionMenuButton.tscn");

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
    }

    public Unit GetUnit() => unit;

    public bool IsTargeting() => targeting;

    public bool IsActive() => active;

    public abstract string GetActionName();

    public virtual void Update(double delta) {}

    protected void StartAction()
    {
        active = true;
    }

    protected void CompleteAction()
    {
        active = false;
    }

    public abstract void TakeAction(Vector2I gridPos);

    public abstract List<Vector2I> GetValidPositions();

    public virtual bool IsActionAvailable() => !IsActionDisabled() && GetValidPositions().Count > 0;

    public bool IsValidPosition(Vector2I gridPos)
    {
        return GetValidPositions().Contains(gridPos);
    }

    public virtual void AddActionToUI(VBoxContainer actionList, Action actionClickedCallback)
    {
        UnitActionMenuButton button = ACTION_BUTTON.Instantiate() as UnitActionMenuButton;
        button.SetAction(this);
        button.Pressed += actionClickedCallback;
        actionList.AddChild(button);
    }

    public bool IsActionDisabled() => disabled;

    public void SetDisabled(bool disabled) => this.disabled = disabled;

}
