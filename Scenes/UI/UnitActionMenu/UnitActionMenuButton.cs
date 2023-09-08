using Godot;
using System;

public partial class UnitActionMenuButton : Button
{

    private BaseAction action;
    
    public override void _Ready()
    {
        Text = action.GetActionName();
    }

    public void SetAction(BaseAction action) => this.action = action;
}
