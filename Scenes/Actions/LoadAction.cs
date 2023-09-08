using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class LoadAction : BaseAction
{
    
    private UnitPathFollower pathFollower;

    private Unit loaderUnit;
    
    public override void _Ready()
    {
        pathFollower = unit.GetNode<UnitPathFollower>("../UnitPathFollower");
        pathFollower.UnitStopped += HandleUnitStopped;
    }

    public override string GetActionName() => "Load";

    private void HandleUnitStopped()
    {
        if (!IsActive()) return;
        // the unit stopped moving. we can hide the sprite and actually load the unit into whatever they are getting into
        unit.SetHidden(true);
        unit.SetGridPosition(new Vector2I(-1, -1));
        loaderUnit.LoadUnit(unit);
        CompleteAction();
    }

    public override void TakeAction(Vector2I gridPos)
    {
        StartAction();
        loaderUnit = Level.Instance.GetUnit(gridPos);
        // UnloadAction unload = loaderUnit.GetAction<UnloadAction>();
        // unload.SetSelectedUnit(unit); // TODO: remove
        // GD.Print($"{unit.GetUnitName()} is loading into {loaderUnit.GetUnitName()}. Setting the selectedUnit for UnloadAction");
        Vector2I[] gridPath = Level.Instance.GetPath(unit.GetGridPosition(), gridPos, unit.GetMoveDefinition(), true);
        pathFollower.MoveAlongPath(gridPath[1..]); // GetPath returns a path including the position you start on, so make sure to remove that
    }

    public override List<Vector2I> GetValidPositions()
    {
        MoveDefinition moveDef = unit.GetMoveDefinition();
        int moveDistance = moveDef.GetMoveDistance();
        Vector2I pos = unit.GetGridPosition();

        List<Vector2I> validPositions = new List<Vector2I>();
        foreach (Unit otherUnit in Level.Instance.GetUnits())
        {
            if (otherUnit == unit) continue;
            if (unit.IsEnemy() != otherUnit.IsEnemy()) continue; // can't load into units on the opposite team
            int distance = Mathf.Abs(pos.X - otherUnit.GetGridPosition().X) +
                           Mathf.Abs(pos.Y - otherUnit.GetGridPosition().Y);
            if (distance > moveDistance) continue; // don't bother running Level.IsReachable until we know otherUnit is within our move distance
            if (!Level.Instance.IsReachable(pos, otherUnit.GetGridPosition(), moveDef)) continue;
            if (!otherUnit.CanLoadUnit(unit)) continue;

            validPositions.Add(otherUnit.GetGridPosition());
        }

        return validPositions;
    }
}
