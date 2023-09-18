using Godot;

public partial class ActionEventBus : Node
{
    
    public static ActionEventBus Instance { get; private set; }
    
    [Signal]
    public delegate void ActionCompletedEventHandler(BaseAction action);

    [Signal]
    public delegate void ActionTakenEventHandler();

    private bool actionActive;

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.Print("There is already an instance of ActionEventBus??");
            QueueFree();
            return;
        }
        Instance = this;
    }

    public bool IsActionActive() => actionActive;

    public void SetActionActive(bool active) => actionActive = active;

}
