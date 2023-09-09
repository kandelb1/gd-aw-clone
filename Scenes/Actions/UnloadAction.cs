using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class UnloadAction : BaseAction
{

    // whatever unit was selected in the UI to be unloaded
    private Unit selectedUnit;
    
    public override string GetActionName() => "Unload";

    
    // TODO: make this work for multiple loaded units
    public override void TakeAction(Vector2I gridPos)
    {
        unit.UnloadUnit(selectedUnit);
        selectedUnit.SetGridPosition(gridPos);
        selectedUnit.SetExhausted(true);
        selectedUnit.SetHidden(false);
    }

    // TODO: make this work for multiple loaded units
    public override List<Vector2I> GetValidPositions()
    {
        List<Vector2I> validPositions = new List<Vector2I>();
        // if (!unit.HasUnitsLoaded()) return validPositions;
        foreach (Vector2I neighbor in Level.Instance.GetNeighbors(unit.GetGridPosition()))
        {
            if (Level.Instance.IsOccupied(neighbor)) continue;
            if (Level.Instance.IsBlocked(neighbor, selectedUnit.GetMoveDefinition())) continue;
            validPositions.Add(neighbor);
        }

        return validPositions;
    }
    
    public override bool IsActionAvailable()
    {
        // unloading is available if ANY of the loaded units can be put down in a neighboring tile
        if (!unit.HasUnitsLoaded()) return false;
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

    public void SetSelectedUnit(Unit unit) => selectedUnit = unit;

    public override void AddActionToUI(VBoxContainer actionList, Action actionClickedCallback)
    {
        foreach (Unit loadedUnit in unit.GetLoadedUnits())
        {
            UnitActionMenuButton button = ACTION_BUTTON.Instantiate() as UnitActionMenuButton;
            button.SetAction(this);
            button.Pressed += () =>
            {
                SetSelectedUnit(loadedUnit);
                actionClickedCallback();
            };
            actionList.AddChild(button);
            // manually set the button's text
            button.Text = "Unload " + loadedUnit.GetUnitName();
        }
    }
}
