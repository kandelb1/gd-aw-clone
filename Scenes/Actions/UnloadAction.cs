using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class UnloadAction : BaseAction
{
    private Unit selectedUnit; // the unit we are unloading; chosen in the UnitActionMenuUI

    public void SetSelectedUnit(Unit selectedUnit)
    {
        this.selectedUnit = selectedUnit;
        CalculateValidPositions();
    }

    public override string GetActionName() => "Unload";

    protected override void CalculateValidPositions()
    {
        validPositions = new List<Vector2I>();
        foreach (Vector2I neighbor in Level.Instance.GetNeighbors(unit.GetGridPosition()))
        {
            if (Level.Instance.IsOccupied(neighbor)) continue;
            if (Level.Instance.IsBlocked(neighbor, selectedUnit.GetMoveDefinition())) continue;
            validPositions.Add(neighbor);
        }
    }

    public override bool IsValidPosition(Vector2I pos) => validPositions.Contains(pos);

    public override bool IsActionAvailable()
    {
        if (!unit.HasUnitsLoaded()) return false;
        string terrain = Level.Instance.GetTerrainName(unit.GetGridPosition());
        if (unit.GetUnitType() == UnitDefinition.UnitType.Copter && terrain == "sea")
        {
            GD.Print("Copter over sea cannot unload units");
            return false;
        }

        if (unit.GetName() == "Lander" && terrain is not ("beach" or "port"))
        {
            GD.Print("Lander not on beach or port cannot unload units");
            return false;
        }
        foreach (Vector2I neighbor in Level.Instance.GetNeighbors(unit.GetGridPosition()))
        {
            if (Level.Instance.IsOccupied(neighbor)) continue;
            foreach (Unit loadedUnit in unit.GetLoadedUnits())
            {
                if (!Level.Instance.IsBlocked(neighbor, loadedUnit.GetMoveDefinition()))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected override void HandleUnitSelected(Unit otherUnit) {}

    public override void TakeAction(Vector2I pos)
    {
        unit.UnloadUnit(selectedUnit);
        selectedUnit.SetGridPosition(pos);
        selectedUnit.SetExhausted(true);
        selectedUnit.SetHidden(false);
        if (!unit.HasUnitsLoaded())
        {
            unit.SetExhausted(true);
        }
        ActionEventBus.Instance.EmitSignal(ActionEventBus.SignalName.ActionCompleted, this);
    }
    
    public override void AddActionToUI(VBoxContainer actionList, Action actionClickedCallback)
    {
        foreach (Unit loadedUnit in unit.GetLoadedUnits())
        {
            UnitActionMenuButton button = ACTION_BUTTON.Instantiate() as UnitActionMenuButton;
            button.SetButtonText("Unload " + loadedUnit.GetName());
            button.Pressed += () =>
            {
                SetSelectedUnit(loadedUnit);
                actionClickedCallback();
            };
            actionList.AddChild(button);
        }
    }
}
