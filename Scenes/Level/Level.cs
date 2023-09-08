using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Array = System.Array;

public partial class Level : TileMap
{
    [Signal]
    public delegate void MouseChangedPositionEventHandler(Vector2I gridPos);
    
    public static Level Instance { get; private set; }

    public static readonly int HIGHLIGHT_LAYER = 1;
    public static readonly int ARROW_LAYER = 2;
    public static readonly Vector2I[] dirs = {new(1, 0), new(-1, 0), new(0, 1), new(0, -1)};

    private Vector2I prevPos;
    
    private CustomAStarGrid astarGrid;

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.Print("There is already an instance of LevelGrid??");
            QueueFree();
            return;
        }
        Instance = this;
        
        astarGrid = new CustomAStarGrid();
        astarGrid.Size = GetUsedRect().Size;
        astarGrid.CellSize = new Vector2I(16, 16);
        astarGrid.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
        astarGrid.Update();
        
        prevPos = Vector2I.Zero;
        GD.Print("Level ready()");
    }

    public bool IsValid(Vector2I position)
    {
        return GetUsedCells(0).Contains(position);
    }

    public Vector2I GetGridPosition(Vector2 globalPos) => LocalToMap(globalPos);

    public Vector2 GetWorldPosition(Vector2I gridPos) => MapToLocal(gridPos);

    public Vector2I[] GetPath(Vector2I start, Vector2I end, MoveDefinition moveDef, bool ignoreUnits = false)
    {
        // TODO: change ignoreUnits to ignoreEnemies.
        // units can walk through friendly units, but not enemies.
        astarGrid.SetMoveDefinition(moveDef, ignoreUnits);
        Array<Vector2I> path = astarGrid.GetIdPath(start, end);
        return path.ToArray();
    }

    // TODO: remember that the tilemap's arrow layer has a z-index of 1, so it will automatically be drawn on top of everything else
    public void DrawArrowAlongPath(Array<Vector2I> path, bool deleteStart = false)
    {
        ClearLayer(2);
        SetCellsTerrainPath(2, path, 1, 0);
        if (deleteStart)
        {
            // the autotiling system can't tell the difference between the start and end of the path, so it will put the arrow on both ends
            // we can just clear the first cell in the path to get rid of the unwanted arrow
            EraseCell(ARROW_LAYER, path[0]);
        }
    }

    public void HighlightTiles(List<Vector2I> positions, Color color)
    {
        ClearLayer(1);
        foreach (Vector2I pos in positions)
        {
            SetCell(1, pos, 2, Vector2I.Zero);
            GetCellTileData(1, pos).Modulate = color;
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            Vector2I gridPos = LocalToMap(mouseMotion.Position);
            if (IsValid(gridPos) && prevPos != gridPos)
            {
                prevPos = gridPos;
                // TODO: maybe some other singleton should be handling the mouse, not 'Level'
                EmitSignal(SignalName.MouseChangedPosition, gridPos);
            }
        }
    }

    public bool IsOccupied(Vector2I gridPos)
    {
        return GetTree().GetNodesInGroup("units").ToList()
            .ConvertAll(x => x as Unit)
            .Any(x => x.GetGridPosition() == gridPos);
    }

    public Unit GetUnit(Vector2I gridPos)
    {
        foreach (Node n in GetTree().GetNodesInGroup("units"))
        {
            if (n is Unit unit && unit.GetGridPosition() == gridPos)
            {
                return unit;
            }
        }

        return null;
    }

    public List<Unit> GetUnits()
    {
        return GetTree().GetNodesInGroup("units").ToList()
            .ConvertAll(x => x as Unit);
    }

    public string GetTerrainName(Vector2I pos)
    {
        TileData tileData = GetCellTileData(0, pos);
        string terrainName = (string)tileData.GetCustomData("name");
        return terrainName;
    }

    public bool IsReachable(Vector2I start, Vector2I end, MoveDefinition moveDef)
    {
        string terrain = GetTerrainName(end);
        if (moveDef.GetMoveCostForTerrain(terrain) == 0) return false;
        Vector2I[] path = GetPath(start, end, moveDef);
        return path.Length - 1 <= moveDef.GetMoveDistance();
    }
    
    public List<Vector2I> GetNeighbors(Vector2I pos)
    {
        List<Vector2I> answer = new List<Vector2I>();
        foreach (Vector2I dir in dirs)
        {
            Vector2I test = pos + dir;
            if (!IsValid(test)) continue;
            answer.Add(test);
        }
        return answer;
    }

    public int GetMovementCost(Vector2I pos, MoveDefinition moveDef)
    {
        return moveDef.GetMoveCostForTerrain(GetTerrainName(pos));
    }

    public bool IsBlocked(Vector2I pos, MoveDefinition moveDef)
    {
        return GetMovementCost(pos, moveDef) == 0;
    }
    
}
