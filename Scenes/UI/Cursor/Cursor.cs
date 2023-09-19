using Godot;
using System;

public partial class Cursor : Sprite2D
{
    public override void _Ready()
    {
        Level.Instance.MouseChangedPosition += HandleMouseMoved;
    }

    private void HandleMouseMoved(Vector2I pos)
    {
        Position = Level.Instance.GetWorldPosition(pos);
    }
}
