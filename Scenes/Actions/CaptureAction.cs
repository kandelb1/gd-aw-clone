using Godot;
using System;

[GlobalClass]
public partial class CaptureAction : BaseAction
{
    private BuildingDefinition buildingToCapture;

    public void SetBuilding(BuildingDefinition buildingDef) => buildingToCapture = buildingDef;
    
    public override string GetActionName() => "Capture";

    protected override void CalculateValidPositions() {}

    public override bool IsValidPosition(Vector2I pos) => true;

    public override bool IsActionAvailable()
    {
        BuildingDefinition buildingDef = Level.Instance.GetBuildingDefinition(unit.GetGridPosition());
        if (buildingDef != null && buildingDef.CanUnitCapture(unit.GetUnitDefinition()))
        {
            buildingToCapture = buildingDef;
            return true;
        }
        return false;
    }

    protected override void HandleUnitSelected(Unit otherUnit) {}

    public override void TakeAction(Vector2I pos)
    {
        buildingToCapture.Capture(unit);
        unit.SetExhausted(true);
        ActionEventBus.Instance.EmitSignal(ActionEventBus.SignalName.ActionCompleted, this);
    }
    
    public override void AddActionToUI(VBoxContainer actionList, Action actionClickedCallback)
    {
        UnitActionMenuButton button = ACTION_BUTTON.Instantiate<UnitActionMenuButton>();
        button.SetButtonText("Capture");
        button.Pressed += () =>
        {
            TakeAction(new Vector2I(-1, -1));
            actionClickedCallback();
        };
        actionList.AddChild(button);
    }
}
