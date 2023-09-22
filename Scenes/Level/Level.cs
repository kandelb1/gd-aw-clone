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

public partial class Level : Node2D
{
    [Signal]
    public delegate void MouseChangedPositionEventHandler(Vector2I gridPos);

    [Signal]
    public delegate void BuildingCapturedEventHandler(BuildingDefinition buildingDef);

    [Signal]
    public delegate void MapChangedEventHandler();
    
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

    private TileMap tileMap;

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.Print("There is already an instance of LevelGrid??");
            QueueFree();
            return;
        }
        Instance = this;

        unitsNode = GetNode("../main/Units"); // TODO: are relative paths bad?
        prevPos = Vector2I.Zero;
        ChangeMap("CampaignMission1");
        GD.Print("Level ready()");
    }
    
    private void ChangeMap(string mapName)
    {
        // clear stuff from the previous map
        foreach (Node child in unitsNode.GetChildren())
        {
            child.QueueFree();
        }
        buildings?.Clear();
        
        // load the new map
        string path = $"res://Scenes/Level/{mapName}/{mapName}.tscn";
        GD.Print($"Attempting to load map from {path}");
        tileMap = GD.Load<PackedScene>(path).Instantiate<TileMap>();
        
        // setup the grid
        astarGrid = new CustomAStarGrid();
        astarGrid.Size = tileMap.GetUsedRect().Size;
        astarGrid.CellSize = new Vector2I(16, 16);
        astarGrid.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
        astarGrid.Update();
        
        // convert units and buildings on the tilemap to the real thing
        ConvertUnits();
        SetupBuildings();
        AddChild(tileMap);
        
        EmitSignal(SignalName.MapChanged);
    }

    public bool IsValid(Vector2I position)
    {
        return tileMap.GetUsedCells(LEVEL_LAYER).Contains(position);
    }

    public Array<Vector2I> GetUsedCells(int layer) => tileMap.GetUsedCells(layer);

    public Rect2I GetUsedRect() => tileMap.GetUsedRect();

    public void ClearLayer(int layer) => tileMap.ClearLayer(layer);


    public Vector2I GetGridPosition(Vector2 globalPos) => tileMap.LocalToMap(ToLocal(globalPos));

    public Vector2 GetWorldPosition(Vector2I gridPos) => tileMap.MapToLocal(gridPos);

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
        tileMap.ClearLayer(ARROW_LAYER);
        tileMap.SetCellsTerrainPath(ARROW_LAYER, path, 1, 0);
        if (deleteStart)
        {
            // the autotiling system can't tell the difference between the start and end of the path, so it will put the arrow on both ends
            // we can just clear the first cell in the path to get rid of the unwanted arrow
            tileMap.EraseCell(ARROW_LAYER, path[0]);
        }
    }

    public void HighlightTiles(List<Vector2I> positions, Color color)
    {
        tileMap.ClearLayer(HIGHLIGHT_LAYER);
        foreach (Vector2I pos in positions)
        {
            tileMap.SetCell(HIGHLIGHT_LAYER, pos, 2, Vector2I.Zero);
            tileMap.GetCellTileData(HIGHLIGHT_LAYER, pos).Modulate = color;
        }
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            Vector2 test1 = GetGlobalMousePosition();
            // Vector2 globalPos = mouseMotion.GlobalPosition;
            // GD.Print($"GetGlobalMousePosition: {test1} mouseMotion.GlobalPosition: {globalPos}");
            // Vector2I test1GridPos = LocalToMap(test1);
            // Vector2I globalPosGridPos = LocalToMap(globalPos);
            // GD.Print($"test1GridPos: {test1GridPos} globalPosGridPos: {globalPosGridPos}");
            Vector2I gridPos = tileMap.LocalToMap(test1);
            if (IsValid(gridPos) && prevPos != gridPos)
            {
                prevPos = gridPos;
                // TODO: maybe some other singleton should be handling the mouse, not 'Level'
                // GD.Print($"Mouse at {test1} moved to grid position {gridPos}");
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

    public List<Unit> GetUnitsByTeam(Team team)
    {
        return GetTree().GetNodesInGroup("units").ToList()
            .ConvertAll(x => x as Unit)
            .Where(x => x.GetTeam() == team).ToList();
    }

    public string GetTerrainName(Vector2I pos)
    {
        int layer = BuildingExists(pos) ? BUILDINGS_LAYER : LEVEL_LAYER;
        TileData tileData = tileMap.GetCellTileData(layer, pos);
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
        if(!tileMap.GetUsedCells(BUILDINGS_LAYER).Contains(pos)) return false;
        TileData tileData = tileMap.GetCellTileData(BUILDINGS_LAYER, pos);
        bool isBase = (bool) tileData.GetCustomData("isBuildingBase");
        return isBase;
    }

    public bool BuildingExistsAt(Vector2I pos) => buildings.Any(x => x.GetGridPosition() == pos);

    public BuildingDefinition GetBuildingDefinition(Vector2I pos) => buildings.Find(x => x.GetGridPosition() == pos);

    public int GetDefense(Vector2I pos)
    {
        int layer = BuildingExists(pos) ? BUILDINGS_LAYER : LEVEL_LAYER;
        TileData tileData = tileMap.GetCellTileData(layer, pos);
        int defense = (int)tileData.GetCustomData("defense");
        return defense;
    }

    public AtlasTexture GetTexture(Vector2I pos)
    {
        int layer = BuildingExists(pos) ? BUILDINGS_LAYER : LEVEL_LAYER;
        int sourceId = tileMap.GetCellSourceId(layer, pos);
        if (sourceId == -1) return null;

        TileSetAtlasSource atlasSource = (TileSetAtlasSource) tileMap.TileSet.GetSource(sourceId);
        Vector2I atlasCoords = tileMap.GetCellAtlasCoords(layer, pos);
        Rect2I region = atlasSource.GetTileTextureRegion(atlasCoords);
        
        TileData tileData = tileMap.GetCellTileData(layer, pos);
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
        foreach (Vector2I pos in tileMap.GetUsedCells(UNIT_PLACEMENT_LAYER))
        {
            if (!IsValid(pos)) continue;
            
            TileData tileData = tileMap.GetCellTileData(UNIT_PLACEMENT_LAYER, pos);
            string team = (string) tileData.GetCustomData("team");
            string name = (string) tileData.GetCustomData("unit");
            tileMap.SetCell(UNIT_PLACEMENT_LAYER, pos); // clear the cell
            
            if (!Enum.TryParse(team, out Team unitTeam)) return;
            if (!Globals.Instance.DoesUnitExist(name)) return;
            
            GD.Print($"Spawning {unitTeam} {name} on position {pos}");
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
        foreach (Vector2I pos in tileMap.GetUsedCells(BUILDINGS_LAYER))
        {
            if (!IsValid(pos)) continue;
            // GD.Print($"There is something on the BUILDINGS_LAYER at {pos}");
            
            TileData tileData = tileMap.GetCellTileData(BUILDINGS_LAYER, pos);
            // bool isBuildingBase = (bool) tileData.GetCustomData("isBuildingBase");
            string name = ((string) tileData.GetCustomData("name")).Capitalize();
            string team = (string) tileData.GetCustomData("team");
            // if (!isBuildingBase) continue;
            if (!Globals.Instance.DoesBuildingExist(name)) continue;
            if (!Enum.TryParse(team, out Team buildingTeam)) return;
            GD.Print($"Spawning {buildingTeam} {name} on position {pos}");
            
            BuildingDefinition building = Globals.Instance.GetBuildingDefinition(name);
            building.SetGridPosition(pos);
            building.SetControllingTeam(buildingTeam);
            building.BuildingCaptured += () => { HandleBuildingCaptured(building); };
            buildings.Add(building);
        }
    }

    private void HandleBuildingCaptured(BuildingDefinition buildingDef)
    {
        UpdateBuildingSprite(buildingDef.GetGridPosition(), buildingDef.GetBuildingName(), buildingDef.GetControllingTeam());
        EmitSignal(SignalName.BuildingCaptured, buildingDef);
    }
    
    // TODO: fix bug with buildings above/below other buildings
    // some buildings take up 2 tiles vertically. when you capture them, we change the cell at both positions
    // but some maps in aw2 have buildings right next to each other vertically. when we capture a building, we dont want to delete the building above it
    // maybe we need a separate tilemap layer for building "tops"
    private void UpdateBuildingSprite(Vector2I pos, string buildingName, Team team)
    {
        Vector2I atlasCoords = GetAtlasCoordsForBuilding(buildingName.ToLower(), team);
        tileMap.SetCell(BUILDINGS_LAYER, pos, 3, atlasCoords);
        
        TileData tileData = tileMap.GetCellTileData(BUILDINGS_LAYER, pos);
        bool hasTop = (bool) tileData.GetCustomData("hasTop");
        if (hasTop)
        {
            tileMap.SetCell(BUILDINGS_LAYER, new Vector2I(pos.X, pos.Y - 1), 3, new Vector2I(atlasCoords.X, atlasCoords.Y - 1));
        }
    }

    // this function will break if the buildings ever get moved around in level-tileset.png
    private Vector2I GetAtlasCoordsForBuilding(string buildingName, Team team)
    {
        int x = 0;
        int xOffset = (team == Team.OrangeStar) ? 0 : 10;
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
            case "hq":
                x = 8;
                break;
        }
        return new Vector2I(x + xOffset, 31);
    }
}
