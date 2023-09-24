using Godot;
using System;

public partial class UIEventBus : Node
{
    
    public static UIEventBus Instance { get; private set; }

    [Signal]
    public delegate void PurchaseScreenOpenedEventHandler();

    [Signal]
    public delegate void PurchaseScreenClosedEventHandler();
    
    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.Print("There is already an instance of UIEventBus??");
            QueueFree();
            return;
        }
        Instance = this;
    }
}
