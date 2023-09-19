using Godot;
using System;

public partial class NextTurnButton : Panel
{

    private Button button;
    
    public override void _Ready()
    {
        button = GetNode<Button>("Button");
        button.Pressed += HandleNextTurnPressed;

        ActionEventBus.Instance.ActionTaken += HandleActionTaken;
        ActionEventBus.Instance.ActionCompleted += HandleActionCompleted;
    }

    private void HandleNextTurnPressed()
    {
        TurnSystem.Instance.NextTurn();
    }

    private void HandleActionTaken()
    {
        SetDisabled(true);
    }

    private void HandleActionCompleted(BaseAction action)
    {
        SetDisabled(false);
    }

    private void SetDisabled(bool disabled)
    {
        button.Disabled = disabled;
    }
}
