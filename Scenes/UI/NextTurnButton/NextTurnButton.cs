using Godot;
using System;

public partial class NextTurnButton : Panel
{

    private Button button;
    
    public override void _Ready()
    {
        button = GetNode<Button>("Button");
        button.Pressed += HandleNextTurnPressed;
    }

    private void HandleNextTurnPressed()
    {
        TurnSystem.Instance.NextTurn();
    }
}
