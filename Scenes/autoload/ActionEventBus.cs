using Godot;

public partial class ActionEventBus : Node
{
    
    public static ActionEventBus Instance { get; private set; }
    
    [Signal]
    public delegate void ActionCompletedEventHandler(BaseAction action);

    [Signal]
    public delegate void ActionTakenEventHandler();

    [Signal]
    public delegate void ShootActionTakenEventHandler(ShootAction action, Unit attackingUnit, Unit defendingUnit);

    // this signal should probably live somewhere else, but it'll be just fine here for the time being
    [Signal]
    public delegate void UnitDestroyedEventHandler(BaseUnit unit);

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
