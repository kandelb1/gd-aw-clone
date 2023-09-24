using Godot;
using System;

public partial class SideSwitchComponent : Node
{
    [Signal]
    public delegate void SideSwitchedEventHandler(bool leftSide);
    
    private Vector2 viewportSize;
    private bool leftSide;

    public override void _Ready()
    {
        viewportSize = GetViewport().GetVisibleRect().Size;
        Level.Instance.MouseChangedPosition += HandleMouseMove;
    }
    
    private void HandleMouseMove(Vector2I pos)
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        float halfwayPoint = viewportSize.X / 2;
        if (mousePos.X < halfwayPoint) // left side
        {
            if (leftSide) return;
            leftSide = true;
            EmitSignal(SignalName.SideSwitched, true);
        }
        else // right side
        {
            if (!leftSide) return;
            leftSide = false;
            EmitSignal(SignalName.SideSwitched, false);
        }
    }
}
