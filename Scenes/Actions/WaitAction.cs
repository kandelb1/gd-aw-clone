using Godot;
using System;

[GlobalClass]
public partial class WaitAction : BaseAction
{
    public override string GetActionName() => "Wait";

    protected override void CalculateValidPositions() {}

    public override bool IsActionAvailable() => !unit.GetAction<LoadAction>().IsActionAvailable();

    public override bool IsValidPosition(Vector2I pos) => true;

    protected override void HandleUnitSelected(Unit otherUnit) {}

    public override void TakeAction(Vector2I pos)
    {
        unit.SetExhausted(true);
        ActionEventBus.Instance.EmitSignal(ActionEventBus.SignalName.ActionCompleted, this);
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
