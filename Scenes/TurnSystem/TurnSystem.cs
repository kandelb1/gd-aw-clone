using Godot;
using System;

public partial class TurnSystem : Node
{

    [Signal]
    public delegate void TurnChangedEventHandler();

    public static TurnSystem Instance { get; private set; }

    private bool playerTurn;

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.Print("There is already an instance of TurnSystem??");
            QueueFree();
            return;
        }
        Instance = this;

        playerTurn = true;
    }

    public bool IsPlayerTurn() => playerTurn;

    public bool IsEnemyTurn() => !playerTurn;

    public void NextTurn()
    {
        playerTurn = !playerTurn;
        EmitSignal(SignalName.TurnChanged);
    }
}
