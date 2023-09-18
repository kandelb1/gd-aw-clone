using Godot;
using System;

[GlobalClass]
public partial class LoadAction : BaseAction
{
    private Unit loaderUnit; // the unit we are loading onto; set by MoveAction

    public void SetLoaderUnit(Unit loaderUnit) => this.loaderUnit = loaderUnit;

    public override string GetActionName() => "Load";

    protected override void CalculateValidPositions() {}

    public override bool IsActionAvailable() => loaderUnit != null;

    public override bool IsValidPosition(Vector2I pos) => true;

    protected override void HandleUnitSelected(Unit otherUnit) {}

    public override void TakeAction(Vector2I pos)
    {
        unit.SetHidden(true);
        unit.SetGridPosition(new Vector2I(-1, -1));
        loaderUnit.LoadUnit(unit);
        unit.SetExhausted(true);
        ActionEventBus.Instance.EmitSignal(ActionEventBus.SignalName.ActionCompleted);
    }

    public override void AddActionToUI(VBoxContainer actionList, Action actionClickedCallback)
    {
        UnitActionMenuButton button = ACTION_BUTTON.Instantiate<UnitActionMenuButton>();
        button.SetButtonText("Load");
        button.Pressed += () =>
        {
            TakeAction(new Vector2I(-1, -1));
            actionClickedCallback();
        };
        actionList.AddChild(button);
    }
}
