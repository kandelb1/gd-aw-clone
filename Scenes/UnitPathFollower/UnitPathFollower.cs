using Godot;
using System;

// TODO: rename this to 'MoveAction' and follow what we did in the udemy course?
public partial class UnitPathFollower : Node
{

    [Signal]
    public delegate void UnitMovingEventHandler(Vector2I dir);

    [Signal]
    public delegate void UnitStoppedEventHandler();

    private const int MOVE_SPEED = 100;
    private const float STOP_DIST = 0.01f;

    [Export] private Unit unit;

    private bool moving;
    private Vector2I[] path;
    private Vector2 nextPoint;
    private int pathIndex;
    // private Vector2I prevMoveDir;

    public override void _Ready()
    {
        moving = false;
        // prevMoveDir = Vector2I.Zero;
    }

    public override void _Process(double delta)
    {
        if (!moving) return;
        // GD.Print($"Moving toward {nextPoint}...");
        Vector2 position = unit.GetPosition();
        float distanceBefore = position.DistanceTo(nextPoint);
        Vector2 dir = (nextPoint - position).Normalized();
        unit.SetPosition(position + dir * (float) (MOVE_SPEED * delta));
        float distanceAfter = unit.GetPosition().DistanceTo(nextPoint);
        if (distanceBefore < distanceAfter) // we were closer to the point before we moved, so we are basically there
        {
            // go to the next point
            pathIndex++;
            if (pathIndex >= path.Length)
            {
                FinishMove();
                return;
            }
            nextPoint = Level.Instance.GetWorldPosition(path[pathIndex]);
            Vector2I moveDir = path[pathIndex] - path[pathIndex - 1];
            EmitSignal(SignalName.UnitMoving, moveDir);
            // I don't think this is necessary
            // if (moveDir != prevMoveDir)
            // {
            //     EmitSignal(SignalName.UnitMoving, moveDir);
            //     prevMoveDir = moveDir;
            // }
        }
    }

    public void MoveAlongPath(Vector2I[] path)
    {
        this.path = path;
        nextPoint = Level.Instance.GetWorldPosition(path[0]);
        pathIndex = 0;
        Vector2I moveDir = path[0] - unit.GetGridPosition();
        EmitSignal(SignalName.UnitMoving, moveDir);
        moving = true;
    }

    private void FinishMove()
    {
        moving = false;
        unit.SetGridPosition(Level.Instance.GetGridPosition(unit.GetPosition()));
        EmitSignal(SignalName.UnitStopped);
    }

    public bool IsMoving() => moving;
}
