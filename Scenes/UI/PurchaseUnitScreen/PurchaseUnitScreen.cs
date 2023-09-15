using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class PurchaseUnitScreen : PanelContainer
{
    // private UnitDefinition.UnitType[] typesAllowed; // list of unit types we are allowed to purchase
    [Export] private PackedScene listEntry;

    private VBoxContainer list;

    public override void _Ready()
    {
        list = GetNode<VBoxContainer>("VBoxContainer");
        UnitSystem.Instance.BuildingSelected += HandleBuildingSelected;
        
        Hide();
    }

    private void HandleBuildingSelected(BuildingDefinition buildingDef)
    {
        if (!buildingDef.CanBuild()) return;
        ClearList();
        GD.Print("PurchaseUnitScreen.HandleBuildingSelected()");
        List<UnitDefinition> purchasableUnits = Globals.Instance.GetAllUnits()
            .Where(x => buildingDef.GetAllowedTypes().Contains(x.GetUnitType()))
            .OrderBy(x => x.GetCost())
            .ToList();

        GD.Print($"BUILDING {buildingDef.GetBuildingName()} IS ALLOWED TO PRODUCE: ");
        foreach (UnitDefinition unitDef in purchasableUnits)
        {
            GD.Print(unitDef.GetUnitName());
            PurchaseUnitButton entry = listEntry.Instantiate<PurchaseUnitButton>();
            entry.SetUnitDefinition(unitDef);
            entry.PurchasePressed += HandleButtonPressed;
            list.AddChild(entry);
        }
        
        Show();
    }

    private void ClearList()
    {
        foreach (Node child in list.GetChildren())
        {
            child.QueueFree();
        }
    }
    
    private void HandleButtonPressed(UnitDefinition unitDef)
    {
        GD.Print($"Purchasing {unitDef.GetUnitName()}"); // TODO: spawn a unit on the factory/port/airport position
        Hide();
    }

}
