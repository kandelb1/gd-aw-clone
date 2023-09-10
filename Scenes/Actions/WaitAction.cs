using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class WaitAction : BaseAction
{
    public override string GetActionName() => "Wait";

    public override void TakeAction(Vector2I gridPos)
    {
        unit.SetExhausted(true);
    }
    
    public override List<Vector2I> GetValidPositions() => new List<Vector2I>() {new Vector2I(-1, -1)};

    public override bool IsActionAvailable() => true;

    // override this so we can call TakeAction() right after the button is clicked
    public override void AddActionToUI(VBoxContainer actionList, Action actionClickedCallback)
    {
        UnitActionMenuButton button = ACTION_BUTTON.Instantiate() as UnitActionMenuButton;
        button.SetAction(this);
        button.Pressed += () =>
        {
            TakeAction(new Vector2I(-1, -1)); // doesn't matter what coord we pass in
            actionClickedCallback();
            UnitSystem.Instance.DeselectUnit();
        };
        actionList.AddChild(button);
    }
}
