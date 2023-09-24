using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class PurchaseUnitScreen : PanelContainer
{
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
        List<UnitDefinition> purchasableUnits = Globals.Instance.GetAllUnits()
            .Where(x => buildingDef.GetAllowedTypes().Contains(x.GetUnitType()))
            .OrderBy(x => x.GetCost())
            .ToList();
        
        PlayerDefinition currentPlayer = GameManager.Instance.GetCurrentPlayer();
        foreach (UnitDefinition unitDef in purchasableUnits)
        {
            PurchaseUnitButton entry = listEntry.Instantiate<PurchaseUnitButton>();
            entry.SetUnitDefinition(unitDef);
            entry.SetTeam(buildingDef.GetControllingTeam());
            entry.Disabled = !currentPlayer.CanAfford(unitDef.GetCost());
            entry.Pressed += () => HandleButtonPressed(unitDef, buildingDef);
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
    
    private void HandleButtonPressed(UnitDefinition unitDef, BuildingDefinition buildingDef)
    {
        PlayerDefinition currentPlayer = GameManager.Instance.GetCurrentPlayer();
        currentPlayer.SubtractFunds(unitDef.GetCost());
        
        BaseUnit baseUnit = Level.Instance.SpawnUnit(unitDef, buildingDef.GetControllingTeam(), buildingDef.GetGridPosition());
        baseUnit.GetUnitDefinition().SetExhausted(true);
        Hide();
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("left click"))
        {
            InputEventMouseButton e = @event as InputEventMouseButton;
            if (!GetRect().HasPoint(e.Position))
            {
                Hide();
            }
        }

        if (@event.IsActionPressed("right click"))
        {
            Hide();
        }
    }

}
