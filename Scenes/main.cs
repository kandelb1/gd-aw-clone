using Godot;
using System;
using Godot.Collections;

public partial class main : Node2D
{


    public override void _Ready()
    {
        Level.Instance.MouseChangedPosition += HandleMouseMove;
    }

    private void HandleMouseMove(Vector2I gridPos)
    {
        // Vector2I startPos = new Vector2I(0, 0);
        // Vector2I[] path = Level.Instance.GetPath(startPos, gridPos);
        // GD.Print($"Path from {startPos} to {gridPos} is: {string.Join(", ", path)}");
        // Level.Instance.DrawArrowAlongPath(new Array<Vector2I>(path));
    }
}
