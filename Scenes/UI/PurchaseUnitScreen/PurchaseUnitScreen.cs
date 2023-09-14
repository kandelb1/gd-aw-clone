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
            UnitDefinition.UnitType.Plane,
            UnitDefinition.UnitType.Copter,
        };
        GetPurchasableUnits();
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
        GD.Print($"Purchasing {unitDef.GetName()}"); // TODO: spawn a unit on the factory/port/airport position
    }

    private void GetPurchasableUnits()
    {
        purchasableUnits = Globals.Instance.GetAllUnits()
            .Where(x => typesAllowed.Contains(x.GetType()))
            .OrderBy(x => x.GetCost())
            .ToList();
    }
}
