using Godot;
using System;

public partial class AttackCursor : Node2D
{
    private Vector2 prevPosition;
    private Vector2 nextPosition;
    private AnimationPlayer anim;
    
    public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        prevPosition = Vector2.Zero;
        nextPosition = Vector2.Zero;
        HideCursor();
    }

    public void ShowCursorAt(Vector2I gridPos)
    {
        Position = Level.Instance.GetWorldPosition(gridPos);
        nextPosition = Level.Instance.GetWorldPosition(gridPos);
        anim.Play("bob_and_weave");
        Show();
    }

    public void HideCursor()
    {
        Hide();
    }

    public override void _Process(double delta)
    {
        
    }
}
