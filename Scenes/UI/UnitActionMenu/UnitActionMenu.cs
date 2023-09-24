using Godot;
using System;
using System.Collections.Generic;

public partial class UnitActionMenu : Node2D
{
    [Signal]
    public delegate void ActionSelectedEventHandler(BaseAction action);
    
    [Signal]
    public delegate void MenuClosedEventHandler();
    
    [Export] private PackedScene actionMenuButton;
    
    private VBoxContainer buttonList;
    private List<BaseAction> actions;
    
    public override void _Ready()
    {
        buttonList = GetNode<VBoxContainer>("PanelContainer/List");
        foreach (BaseAction action in actions)
        {
            action.AddActionToUI(buttonList, () => { ActionClicked(action); });
        }
    }
    
    public void SetActions(List<BaseAction> actions) => this.actions = actions;
    
    private void ActionClicked(BaseAction action)
    {
        EmitSignal(SignalName.ActionSelected, action);
        QueueFree();
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("left click") || @event.IsActionPressed("right click"))
        {
            PanelContainer c = GetNode<PanelContainer>("PanelContainer");
            InputEventMouseButton e = c.MakeInputLocal(@event) as InputEventMouseButton;
            if (!c.GetRect().HasPoint(e.Position))
            {
                EmitSignal(SignalName.MenuClosed);
                QueueFree();
            }
        }
    }
}
