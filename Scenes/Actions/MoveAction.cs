using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class MoveAction : BaseAction
{
    public override string GetActionName() => "Move";

    public override bool WillExhaustUnit() => false;

    protected override void HandleUnitSelected(Unit otherUnit)
    {
        if (otherUnit != unit) return;
        CalculateValidPositions();
        Level.Instance.ConfigureAstarGrid(unit.GetUnitDefinition());
    }

    public override void TakeAction(Vector2I pos)
    {
        unit.SetGridPosition(pos);
        unit.SetMoved(true);
        ActionEventBus.Instance.EmitSignal(ActionEventBus.SignalName.ActionCompleted); // I will admit this is quite ugly
    }

    public override bool IsActionAvailable() => !unit.HasAlreadyMoved();

    public override bool IsValidPosition(Vector2I pos)
    {
        // a move is a valid if the tile is reachable
        // if pos is the same position as this unit's current position, that's fine
        // however, if we are moving on top of another unit, it's only valid if we can load or join with the unit there
        if (!validPositions.Contains(pos)) return false;
        if (pos == unit.GetGridPosition()) return true;
        if (Level.Instance.IsOccupiedBySameTeam(unit.GetTeam(), pos))
        {
            // only valid if we can join or load with the unit here
            Unit friendlyUnit = Level.Instance.GetUnit(pos);
            bool canLoad = friendlyUnit.CanLoadUnit(unit);
            bool canJoin = false; // bool canJoin = friendlyUnit.CanJoin(unit);
            return canLoad || canJoin;
        }
        return true;
    }

    protected override void CalculateValidPositions()
    {
        Vector2I start = unit.GetGridPosition();
        MoveDefinition moveDef = unit.GetMoveDefinition();

        validPositions = new List<Vector2I>();
        Queue<TileInfo> queue = new Queue<TileInfo>();
        HashSet<Vector2I> visited = new HashSet<Vector2I>();
        int moveDistance = moveDef.GetMoveDistance();
        queue.Enqueue(new TileInfo(start, moveDistance));

        while (queue.Count > 0)
        {
            TileInfo current = queue.Dequeue();
            validPositions.Add(current.position);
            visited.Add(current.position);
            foreach (Vector2I neighbor in Level.Instance.GetNeighbors(current.position))
            {
                if (visited.Contains(neighbor)) continue;
                if (Level.Instance.IsBlocked(neighbor, moveDef)) continue;
                if (Level.Instance.IsOccupiedByOtherTeam(unit.GetTeam(), neighbor)) continue;

                int moveCost = Level.Instance.GetMovementCost(neighbor, moveDef);
                int remainingDistance = current.remainingMoveDistance - moveCost;
                if (remainingDistance < 0) continue;
                queue.Enqueue(new TileInfo(neighbor, remainingDistance));
            }
        }
    }

    private struct TileInfo
    {
        public Vector2I position;
        public int remainingMoveDistance;
        
        public TileInfo(Vector2I pos, int distance)
        {
            position = pos;
            remainingMoveDistance = distance;
        }
    }
}
