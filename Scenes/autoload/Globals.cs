using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class Globals : Node
{
    
    public static Globals Instance { get; private set; }
    
    private const string PATH_TO_UNITDEFS = "res://Scenes/Unit/UnitDefinitions";

    private List<UnitDefinition> allUnits;

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
        GD.Print("Globals ready.");
    }

    public List<UnitDefinition> GetAllUnits() => allUnits;

    public bool DoesUnitExist(string unitName)
    {
        return allUnits.Any(x => x.GetName() == unitName);
    }

    public UnitDefinition GetUnitDefinition(string unitName) => allUnits.Find(x => x.GetName() == unitName);
    
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
                    allUnits.Add(unit);
                }
                catch (InvalidCastException e)
                {
                    GD.PrintErr($"Invalid UnitDefinition at {fullPath} - skipping over.");
                }
            }
            fileName = dir.GetNext();
        }
    }
}
