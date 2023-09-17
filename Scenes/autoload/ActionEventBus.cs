using Godot;
using System;

public partial class ActionEventBus : Node
{
    
    public static ActionEventBus Instance { get; private set; }
    
    [Signal]
    public delegate void ActionCompletedEventHandler();

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.Print("There is already an instance of ActionEventBus??");
            QueueFree();
            return;
        }
        Instance = this;
        GD.Print("ActionEventBus._Ready()");
    }
}
