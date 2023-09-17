using Godot;
using System;

[GlobalClass]
public partial class WaitAction : BaseAction
{
    public override string GetActionName() => "Wait";

    public override bool WillExhaustUnit() => true;

    protected override void CalculateValidPositions() {}

    public override bool IsActionAvailable() => true;

    public override bool IsValidPosition(Vector2I pos) => true;

    protected override void HandleUnitSelected(Unit otherUnit) {}

    public override void TakeAction(Vector2I pos)
    {
        unit.SetExhausted(true);
        ActionEventBus.Instance.EmitSignal(ActionEventBus.SignalName.ActionCompleted);
    }

    public override void AddActionToUI(VBoxContainer actionList, Action actionClickedCallback)
    {
        UnitActionMenuButton button = ACTION_BUTTON.Instantiate<UnitActionMenuButton>();
        button.SetButtonText("Wait");
        button.Pressed += () =>
        {
            TakeAction(new Vector2I(-1, -1));
            actionClickedCallback();
        };
        actionList.AddChild(button);
    }
}
