using Godot;
using System;

public partial class CustomAStarGrid : AStarGrid2D
{

    private MoveDefinition moveDef;
    
    // talk to the tile map and figure out the cost of going from one tile to the next
    // _ComputeCost is always called with two points that are next to each other
    public override float _ComputeCost(Vector2I fromId, Vector2I toId)
    {
        string terrainName = Level.Instance.GetTerrainName(toId);
        int moveCost = moveDef.GetMoveCostForTerrain(terrainName);
        if (moveCost == 0) return float.PositiveInfinity;
        return moveCost;
    }
    
    // public void SetMoveDefinition(MoveDefinition moveDef, bool ignoreUnits)
    // {
    //     this.moveDef = moveDef;
    //     // block out all tiles that are not reachable with this move def
    //     foreach (Vector2I pos in Level.Instance.GetUsedCells(0))
    //     {
    //         int moveCost = moveDef.GetMoveCostForTerrain(Level.Instance.GetTerrainName(pos));
    //         // bool hasUnit = Level.Instance.IsOccupied(pos);
    //         // if (ignoreUnits) hasUnit = false;
    //         // SetPointSolid(pos, moveCost == 0 || hasUnit);
    //         SetPointSolid(pos, moveCost == 0);
    //     }
    // }
    
    // block any tiles that are not reachable with the given move def, or have a unit of a different team occupying them
    public void ConfigureGrid(UnitDefinition.Team team, MoveDefinition moveDef)
    {
        this.moveDef = moveDef;
        foreach (Vector2I pos in Level.Instance.GetUsedCells(Level.LEVEL_LAYER))
        {
            int moveCost = moveDef.GetMoveCostForTerrain(Level.Instance.GetTerrainName(pos));
            bool occupied = false;
            if (Level.Instance.IsOccupied(pos))
            {
                Unit otherUnit = Level.Instance.GetUnit(pos);
                occupied = otherUnit.GetTeam() != team;
            }
            SetPointSolid(pos, moveCost == 0 || occupied);
        }
    }
}
