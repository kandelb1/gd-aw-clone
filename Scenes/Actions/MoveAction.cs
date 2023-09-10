using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class MoveAction : BaseAction
{

    private UnitPathFollower pathFollower;

    public override void _Ready()
    {
        pathFollower = unit.GetNode<UnitPathFollower>("../UnitPathFollower");
        pathFollower.UnitStopped += HandleUnitStopped;
    }

    public override void _Process(double delta)
    {
        // GD.Print("move action process");
    }

    private void HandleUnitStopped()
    {
        if (!IsActive()) return;
        // TODO: this is duplicate code from UnloadAction, maybe we can move it to BaseAction.CompleteAction()?
        if (unit.HasUnitsLoaded())
        {
            GD.Print($"{unit.GetUnitName()} has more units loaded, forcing them to unload or wait");
            // disable every action except Unload and Wait
            foreach (BaseAction action in unit.GetActions())
            {
                switch (action)
                {
                    case UnloadAction:
                        continue;
                    case WaitAction:
                        continue;
                    default:
                        action.SetDisabled(true);
                        break;
                }
            }
        }
        else
        {
            unit.SetExhausted(true);
        }
        
        CompleteAction();
    }

    public override string GetActionName() => "Move";

    public override void TakeAction(Vector2I gridPos)
    {
        StartAction();
        Vector2I[] gridPath = Level.Instance.GetPath(unit.GetGridPosition(), gridPos, unit.GetMoveDefinition());
        pathFollower.MoveAlongPath(gridPath[1..]); // GetPath returns a path including the position you start on, so make sure to remove that
    }
    
    public override List<Vector2I> GetValidPositions()
    {
        List<Vector2I> reachableTiles = GetReachableTiles(unit.GetGridPosition(), unit.GetMoveDefinition());
        // the first tile is always the unit's position, so remove that
        return reachableTiles.GetRange(1, reachableTiles.Count - 1);
        // GD.Print("GetValidPositions()");
        // Vector2I unitPos = unit.GetGridPosition();
        // int moveDist = unit.GetMoveDefinition().GetMoveDistance();
        //
        // List<Vector2I> positions = new List<Vector2I>();
        // for (int i = -moveDist; i <= moveDist; i++)
        // {
        //     for (int j = -moveDist; j <= moveDist; j++)
        //     {
        //         Vector2I test = new Vector2I(unitPos.X + i, unitPos.Y + j);
        //         int distance = Math.Abs(i) + Math.Abs(j);
        //         if (distance > moveDist) continue;
        //         if (!Level.Instance.IsValid(test) || test == unitPos) continue;
        //         if (Level.Instance.IsOccupied(test)) continue;
        //         if (!Level.Instance.IsReachable(unitPos, test, unit.GetMoveDefinition())) continue;
        //         positions.Add(test);
        //     }
        // }
        //
        // return positions;
    }

    // TODO: test whether building up a path using this BFS algorithm gives the same path as the one from astar...
    // im pretty sure it should, since this correctly gets whether or not we can move to a tile or not,
    // and so the path to get there must have been the most efficient one, right?
    // I guess for tiles that are close that might not be true
    private List<Vector2I> GetReachableTiles(Vector2I start, MoveDefinition moveDef)
    {
        List<Vector2I> reachableTiles = new List<Vector2I>();
        Queue<TileInfo> queue = new Queue<TileInfo>();
        HashSet<Vector2I> visited = new HashSet<Vector2I>();
        int moveDistance = moveDef.GetMoveDistance();

        queue.Enqueue(new TileInfo(start, moveDistance));

        while (queue.Count > 0)
        {
            TileInfo current = queue.Dequeue();
            reachableTiles.Add(current.position);
            visited.Add(current.position);
            foreach (Vector2I neighbor in Level.Instance.GetNeighbors(current.position))
            {
                if (visited.Contains(neighbor)) continue;
                if (Level.Instance.IsBlocked(neighbor, moveDef)) continue;
                if (Level.Instance.IsOccupied(neighbor)) continue;

                int moveCost = Level.Instance.GetMovementCost(neighbor, moveDef);
                int remainingDistance = current.remainingMoveDistance - moveCost;
                if (remainingDistance < 0) continue;
                int distanceAway = Mathf.Abs(start.X - current.position.X) + Mathf.Abs(start.Y - current.position.Y);
                // GD.Print($"{start} to {neighbor} is {distanceAway} tiles away");
                queue.Enqueue(new TileInfo(neighbor, remainingDistance));
            }
        }

        return reachableTiles;
    }

    // public override bool IsActionAvailable() => GetValidPositions()

    public struct TileInfo
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
