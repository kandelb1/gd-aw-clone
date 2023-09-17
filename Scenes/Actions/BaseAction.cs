using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class BaseAction : Node
{
    protected static PackedScene ACTION_BUTTON = ResourceLoader.Load<PackedScene>("res://Scenes/UI/UnitActionMenu/UnitActionMenuButton.tscn");
    
    protected Unit unit;
    protected List<Vector2I> validPositions;

    public override void _Ready()
    {
        UnitSystem.Instance.UnitSelected += HandleUnitSelected;
    }

    public void SetUnit(Unit unit) => this.unit = unit;

    public Unit GetUnit() => unit;
    
    public abstract string GetActionName();

    protected abstract void CalculateValidPositions();

    public virtual List<Vector2I> GetValidPositions() => validPositions;
    
    public abstract bool IsValidPosition(Vector2I pos);

    public abstract bool IsActionAvailable();

    protected abstract void HandleUnitSelected(Unit otherUnit);

    public abstract void TakeAction(Vector2I pos);

    public abstract bool WillExhaustUnit(); // will this action exhaust the unit when it's done?
    
    public virtual void AddActionToUI(VBoxContainer actionList, Action actionClickedCallback)
    {
        UnitActionMenuButton button = ACTION_BUTTON.Instantiate() as UnitActionMenuButton;
        button.SetButtonText(GetActionName());
        button.Pressed += actionClickedCallback;
        actionList.AddChild(button);
    }
}
