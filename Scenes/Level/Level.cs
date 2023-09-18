using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using static UnitDefinition;


// in the first campaign mission, the map takes up the entire screen (with no scrolling required)
// it is 15x10 tiles. each tile is 16x16 pixels
// so at normal resolution, it would take up 240x160 pixels
// scaling it up by 2x, it would take up 480x320 pixels
// 4x would be 960x640

public partial class Level : TileMap
{
    [Signal]
    public delegate void MouseChangedPositionEventHandler(Vector2I gridPos);
    
    public static Level Instance { get; private set; }

    public const int LEVEL_LAYER = 0;
    public const int BUILDINGS_LAYER = 1;
    public const int HIGHLIGHT_LAYER = 2;
    public const int ARROW_LAYER = 3;
    public const int UNIT_PLACEMENT_LAYER = 4;
    
    public static readonly Vector2I[] dirs = {new(1, 0), new(-1, 0), new(0, 1), new(0, -1)};

    [Export] private PackedScene baseUnitScene;

    private Node unitsNode;

    private Vector2I prevPos;
    
    private CustomAStarGrid astarGrid;

    private List<BuildingDefinition> buildings;

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
        
        unitsNode = GetNode("../main/Units"); // TODO: are relative paths bad?
        ConvertUnits();
        SetupBuildings();

        prevPos = Vector2I.Zero;
        GD.Print("Level ready()");
    }

    public bool IsValid(Vector2I position)
    {
        return GetUsedCells(LEVEL_LAYER).Contains(position);
    }

    public Vector2I GetGridPosition(Vector2 globalPos) => LocalToMap(globalPos);

    public Vector2 GetWorldPosition(Vector2I gridPos) => MapToLocal(gridPos);

    public void ConfigureAstarGrid(UnitDefinition unitDef)
    {
        astarGrid.ConfigureGrid(unitDef.GetTeam(), unitDef.GetMoveDefinition());
    }

    public Array<Vector2I> GetPath(Vector2I startPos, Vector2I endPos)
    {
        Array<Vector2I> path = astarGrid.GetIdPath(startPos, endPos);
        return path;
    }

    // TODO: remember that the tilemap's arrow layer has a z-index of 1, so it will automatically be drawn on top of everything else
    public void DrawArrowAlongPath(Array<Vector2I> path, bool deleteStart = false)
    {
        ClearLayer(ARROW_LAYER);
        SetCellsTerrainPath(ARROW_LAYER, path, 1, 0);
        if (deleteStart)
        {
            // the autotiling system can't tell the difference between the start and end of the path, so it will put the arrow on both ends
            // we can just clear the first cell in the path to get rid of the unwanted arrow
            EraseCell(ARROW_LAYER, path[0]);
        }
    }

    public void HighlightTiles(List<Vector2I> positions, Color color)
    {
        ClearLayer(HIGHLIGHT_LAYER);
        foreach (Vector2I pos in positions)
        {
            SetCell(HIGHLIGHT_LAYER, pos, 2, Vector2I.Zero);
            GetCellTileData(HIGHLIGHT_LAYER, pos).Modulate = color;
        }
    }
    
    public override void _UnhandledInput(InputEvent @event)
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

    public bool IsOccupiedByOtherTeam(Team team, Vector2I gridPos)
    {
        return GetTree().GetNodesInGroup("units").ToList()
            .ConvertAll(x => x as Unit)
            .Any(x => x.GetGridPosition() == gridPos && x.GetTeam() != team);
    }

    public bool IsOccupiedBySameTeam(Team team, Vector2I gridPos)
    {
        return GetTree().GetNodesInGroup("units").ToList()
            .ConvertAll(x => x as Unit)
            .Any(x => x.GetGridPosition() == gridPos && x.GetTeam() == team);
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
        int layer = BuildingExists(pos) ? BUILDINGS_LAYER : LEVEL_LAYER;
        TileData tileData = GetCellTileData(layer, pos);
        string terrainName = (string)tileData.GetCustomData("name");
        return terrainName;
    }

    // public bool IsReachable(Vector2I start, Vector2I end, MoveDefinition moveDef)
    // {
    //     string terrain = GetTerrainName(end);
    //     if (moveDef.GetMoveCostForTerrain(terrain) == 0) return false;
    //     Vector2I[] path = GetPath(start, end, moveDef);
    //     return path.Length - 1 <= moveDef.GetMoveDistance();
    // }
    
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

    private bool BuildingExists(Vector2I pos)
    {
        if(!GetUsedCells(BUILDINGS_LAYER).Contains(pos)) return false;
        TileData tileData = GetCellTileData(BUILDINGS_LAYER, pos);
        bool isBase = (bool) tileData.GetCustomData("isBuildingBase");
        return isBase;
    }

    public bool BuildingExistsAt(Vector2I pos) => buildings.Any(x => x.GetGridPosition() == pos);

    public BuildingDefinition GetBuildingDefinition(Vector2I pos) => buildings.Find(x => x.GetGridPosition() == pos);

    public int GetDefense(Vector2I pos)
    {
        int layer = BuildingExists(pos) ? BUILDINGS_LAYER : LEVEL_LAYER;
        TileData tileData = GetCellTileData(layer, pos);
        int defense = (int)tileData.GetCustomData("defense");
        return defense;
    }

    public AtlasTexture GetTexture(Vector2I pos)
    {
        int layer = BuildingExists(pos) ? BUILDINGS_LAYER : LEVEL_LAYER;
        int sourceId = GetCellSourceId(layer, pos);
        if (sourceId == -1) return null;

        TileSetAtlasSource atlasSource = (TileSetAtlasSource) TileSet.GetSource(sourceId);
        Vector2I atlasCoords = GetCellAtlasCoords(layer, pos);
        Rect2I region = atlasSource.GetTileTextureRegion(atlasCoords);
        
        TileData tileData = GetCellTileData(layer, pos);
        bool hasTop = (bool) tileData.GetCustomData("hasTop");
        if (hasTop)
        {
            region.Position += new Vector2I(0, -16);
            region.Size += new Vector2I(0, 16);    
        }
        
        AtlasTexture texture = new AtlasTexture();
        texture.Atlas = atlasSource.Texture;
        texture.Region = region;
        return texture;
    }

    // converts tiles on the unit_placement layer to actual units in the scene tree
    private void ConvertUnits()
    {
        foreach (Vector2I pos in GetUsedCells(UNIT_PLACEMENT_LAYER))
        {
            if (!IsValid(pos)) continue;
            
            TileData tileData = GetCellTileData(UNIT_PLACEMENT_LAYER, pos);
            string team = (string) tileData.GetCustomData("team");
            string name = (string) tileData.GetCustomData("unit");
            SetCell(UNIT_PLACEMENT_LAYER, pos); // clear the cell
            
            if (!Enum.TryParse(team, out Team unitTeam)) return;
            if (!Globals.Instance.DoesUnitExist(name)) return;
            
            GD.Print($"Spawning {unitTeam} {name} on position {pos}!");
            SpawnUnit(name, unitTeam, pos);
        }
    }

    public BaseUnit SpawnUnit(string unitName, Team team, Vector2I pos)
    {
        UnitDefinition unitDef = (UnitDefinition) Globals.Instance.GetUnitDefinition(unitName).Duplicate();
        unitDef.SetTeam(team);
        BaseUnit baseUnit = baseUnitScene.Instantiate<BaseUnit>();
        baseUnit.SetUnitDefinition(unitDef);
        baseUnit.SetStartPosition(pos);
        unitsNode.AddChild(baseUnit);
        return baseUnit;
    }

    public BaseUnit SpawnUnit(UnitDefinition unitDef, Team team, Vector2I pos)
    {
        UnitDefinition copy = (UnitDefinition) unitDef.Duplicate();
        copy.SetTeam(team);
        BaseUnit baseUnit = baseUnitScene.Instantiate<BaseUnit>();
        baseUnit.SetUnitDefinition(copy);
        baseUnit.SetStartPosition(pos);
        unitsNode.AddChild(baseUnit);
        return baseUnit;
    }

    // \creates BuildingDefinitions for every building on the tilemap
    private void SetupBuildings()
    {
        buildings = new List<BuildingDefinition>();
        foreach (Vector2I pos in GetUsedCells(BUILDINGS_LAYER))
        {
            if (!IsValid(pos)) continue;
            // GD.Print($"There is something on the BUILDINGS_LAYER at {pos}");
            
            TileData tileData = GetCellTileData(BUILDINGS_LAYER, pos);
            // bool isBuildingBase = (bool) tileData.GetCustomData("isBuildingBase");
            string name = ((string) tileData.GetCustomData("name")).Capitalize();
            string team = (string) tileData.GetCustomData("team");
            // if (!isBuildingBase) continue;
            if (!Globals.Instance.DoesBuildingExist(name)) continue;
            if (!Enum.TryParse(team, out Team buildingTeam)) return;
            GD.Print($"{buildingTeam} {name} exists at {pos}");
            
            BuildingDefinition building = Globals.Instance.GetBuildingDefinition(name);
            building.SetGridPosition(pos);
            building.SetControllingTeam(buildingTeam);
            building.BuildingCaptured += () => { HandleBuildingCaptured(building); };
            buildings.Add(building);
        }
        GD.Print("----PRINTING BUILDINGS LIST----");
        foreach (BuildingDefinition def in buildings)
        {
            GD.Print($"{def.GetControllingTeam().ToString()} {def.GetBuildingName()} at position {def.GetGridPosition()}");
        }
    }

    private void HandleBuildingCaptured(BuildingDefinition buildingDef)
    {
        UpdateBuildingSprite(buildingDef.GetGridPosition(), buildingDef.GetBuildingName(), buildingDef.GetControllingTeam());
    }

    private void UpdateBuildingSprite(Vector2I pos, string buildingName, Team team)
    {
        Vector2I atlasCoords = GetAtlasCoordsForBuilding(buildingName.ToLower(), team);
        SetCell(BUILDINGS_LAYER, pos, 3, atlasCoords);
        
        TileData tileData = GetCellTileData(BUILDINGS_LAYER, pos);
        bool hasTop = (bool) tileData.GetCustomData("hasTop");
        if (hasTop)
        {
            SetCell(BUILDINGS_LAYER, new Vector2I(pos.X, pos.Y - 1), 3, new Vector2I(atlasCoords.X, atlasCoords.Y - 1));
        }
    }

    // this function will break if the buildings ever get moved around in level-tileset.png
    private Vector2I GetAtlasCoordsForBuilding(string buildingName, Team team)
    {
        int x = 0;
        int xOffset = (team == Team.OrangeStar) ? 0 : 8;
        switch (buildingName)
        {
            case "city":
                x = 0;
                break;
            case "factory":
                x = 2;
                break;
            case "airport":
                x = 4;
                break;
            case "port":
                x = 6;
                break;
        }
        return new Vector2I(x + xOffset, 31);
    }
}
