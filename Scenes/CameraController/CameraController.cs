using Godot;
using System;

public partial class CameraController : Node
{
    private static readonly int TILE_SIZE = 16;
    // private static readonly Vector2I SCREEN_SIZE = new Vector2I(10, 15);
    private const int CAMERA_SPEED = 10;
    
    private Camera2D camera;
    private Vector2I levelSize;
    
    public override void _Ready()
    {
        camera = GetNode<Camera2D>("Camera2D");
        levelSize = Level.Instance.GetUsedRect().Size;
        SetCameraLimits();
    }

    private void SetCameraLimits()
    {
        // left and top will always be 0
        camera.LimitLeft = 0;
        camera.LimitTop = 0;

        camera.LimitRight = levelSize.X * TILE_SIZE;
        camera.LimitBottom = levelSize.Y * TILE_SIZE;
    }

    public override void _Process(double delta)
    {
        // TODO: change camera movement to work with the mouse instead of keyboard
        // but if we do keep the keyboard method, fix the faster camera movement speed when moving diagonally 
        Vector2 offset = new Vector2(0, 0);
        if (Input.IsActionPressed("move_left"))
        {
            offset += new Vector2(-16, 0);
        }
        if(Input.IsActionPressed("move_right"))
        {
            offset += new Vector2(16, 0);
        }
        if (Input.IsActionPressed("move_up"))
        {
            offset += new Vector2(0, -16);
        }
        if (Input.IsActionPressed("move_down"))
        {
            offset += new Vector2(0, 16);
        }
        
        Vector2 newPosition = camera.Position + (offset * CAMERA_SPEED * (float) delta);
        int clampedX = (int) Mathf.Clamp(newPosition.X, 0, levelSize.X * TILE_SIZE);
        int clampedY = (int) Mathf.Clamp(newPosition.Y, 0, levelSize.Y * TILE_SIZE);
        camera.Position = new Vector2(clampedX, clampedY);
    }
}
