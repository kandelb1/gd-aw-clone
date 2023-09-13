using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class PurchaseUnitScreen : PanelContainer
{
    private UnitDefinition.UnitType[] typesAllowed; // list of unit types we are allowed to purchase
    [Export] private PackedScene listEntry;
    
    private List<UnitDefinition> purchasableUnits;
    
    public override void _Ready()
    {
        // set this manually for now
        typesAllowed = new[]
        {
            UnitDefinition.UnitType.Infantry,
            UnitDefinition.UnitType.Vehicle,
        };
        LoadUnits();
        VBoxContainer list = GetNode<VBoxContainer>("VBoxContainer");
        foreach (UnitDefinition unitDef in purchasableUnits)
        {
            PurchaseUnitButton entry = listEntry.Instantiate<PurchaseUnitButton>();
            entry.SetUnitDefinition(unitDef);
            entry.PurchasePressed += HandleButtonPressed;
            list.AddChild(entry);
        }
    }

    public void SetTypesAllowed(UnitDefinition.UnitType[] typesAllowed) => this.typesAllowed = typesAllowed;

    private void HandleButtonPressed(UnitDefinition unitDef)
    {
        GD.Print($"Purchasing {unitDef.GetName()}");
    }

    private void LoadUnits()
    {
        purchasableUnits = new List<UnitDefinition>();
        const string path = "res://Scenes/Unit/UnitDefinitions";
        DirAccess dir = DirAccess.Open(path);
        if (dir == null) return;
        dir.ListDirBegin();
        string fileName = dir.GetNext();
        while (fileName != "")
        {
            if (fileName.EndsWith(".tres"))
            {
                UnitDefinition unit = ResourceLoader.Load<UnitDefinition>(Path.Join(path, fileName));
                if (typesAllowed.Contains(unit.GetType()))
                {
                    purchasableUnits.Add(unit);    
                }
            }
            fileName = dir.GetNext();
        }
        purchasableUnits.Sort((a, b) =>
        {
            int aCost = a.GetCost();
            int bCost = b.GetCost();
            if (aCost < bCost) return -1;
            if (aCost > bCost) return 1;
            return 0;
        });
    }
}
