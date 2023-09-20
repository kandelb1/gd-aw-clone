using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Globals : Node
{
    
    public static Globals Instance { get; private set; }
    
    private const string PATH_TO_UNITDEFS = "res://Scenes/Unit/UnitDefinitions";
    private const string PATH_TO_BUILDINGDEFS = "res://Scenes/Buildings/";

    private List<UnitDefinition> allUnits;
    private List<BuildingDefinition> allBuildings;

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.Print("There is already an instance of Globals??");
            QueueFree();
            return;
        }
        Instance = this;
        LoadUnits();
        LoadBuildings();
        GD.Print("Globals ready.");
    }

    public List<UnitDefinition> GetAllUnits() => allUnits;

    public bool DoesUnitExist(string unitName)
    {
        return allUnits.Any(x => x.GetUnitName() == unitName);
    }

    public UnitDefinition GetUnitDefinition(string unitName) =>
        (UnitDefinition) allUnits.Find(x => x.GetUnitName() == unitName).Duplicate(true);

    public bool DoesBuildingExist(string buildingName)
    {
        return allBuildings.Any(x => x.GetBuildingName() == buildingName);
    }

    public BuildingDefinition GetBuildingDefinition(string buildingName)
    {
        return (BuildingDefinition) allBuildings.Find(x => x.GetBuildingName() == buildingName).Duplicate();
    }
    
    
    private void LoadUnits()
    {
        allUnits = new List<UnitDefinition>();
        DirAccess dir = DirAccess.Open(PATH_TO_UNITDEFS);
        if (dir == null) return;
        dir.ListDirBegin();
        string fileName = dir.GetNext();
        while (fileName != "")
        {
            if (fileName.EndsWith(".tres"))
            {
                string fullPath = Path.Join(PATH_TO_UNITDEFS, fileName);
                try
                {
                    UnitDefinition unit = ResourceLoader.Load<UnitDefinition>(fullPath);
                    // unit.SetPrimaryWeapon((Weapon) unit.GetPrimaryWeapon().Duplicate());
                    allUnits.Add(unit);
                    GD.Print($"Loaded UnitDefinition with name {unit.GetUnitName()}");
                }
                catch (InvalidCastException e)
                {
                    GD.PrintErr($"Invalid UnitDefinition at {fullPath} - skipping over.");
                }
            }
            fileName = dir.GetNext();
        }
    }
    
    private void LoadBuildings()
    {
        allBuildings = new List<BuildingDefinition>();
        DirAccess dir = DirAccess.Open(PATH_TO_BUILDINGDEFS);
        if (dir == null) return;
        dir.ListDirBegin();
        string fileName = dir.GetNext();
        while (fileName != "")
        {
            if (fileName.EndsWith(".tres"))
            {
                string fullPath = Path.Join(PATH_TO_BUILDINGDEFS, fileName);
                try
                {
                    BuildingDefinition building = ResourceLoader.Load<BuildingDefinition>(fullPath);
                    allBuildings.Add(building);
                    GD.Print($"Loaded BuildingDefinition with name {building.GetBuildingName()}");
                }
                catch (InvalidCastException e)
                {
                    GD.PrintErr($"Invalid BuildingDefinition at {fullPath} - skipping over.");
                }
            }
            fileName = dir.GetNext();
        }
    }
}
