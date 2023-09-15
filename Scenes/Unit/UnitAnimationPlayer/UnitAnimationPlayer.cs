using Godot;
using System;

public partial class UnitAnimationPlayer : Node
{
    [Export] private Unit unit;
    [Export] private UnitPathFollower pathFollower;
    
    private SpriteFrames spriteFrames;
    private AnimatedSprite2D animSprite;

    public override void _Ready()
    {
        pathFollower.UnitMoving += HandleUnitMoving;
        pathFollower.UnitStopped += HandleUnitStopped;

        // UnitSystem.Instance.ActionSelected += HandleActionSelected;
        // UnitSystem.Instance.UnitDeselected += HandleUnitDeselected;
        
        animSprite = GetNode<AnimatedSprite2D>("../AnimatedSprite2D");
        animSprite.SpriteFrames = spriteFrames;

        unit.ExhaustedChanged += HandleExhaustedChanged;
    }

    private void HandleExhaustedChanged()
    {
        animSprite.Play(unit.IsExhausted() ? "exhausted" : "idle");
    }

    public void SetSpriteFrames(SpriteFrames spriteFrames)
    {
        this.spriteFrames = spriteFrames;
        animSprite.SpriteFrames = spriteFrames;
        animSprite.Play("idle");
    }

    // private void HandleActionSelected(BaseAction action)
    // {
    //     if (action is MoveAction moveAction && moveAction.GetUnit() == unit)
    //     {
    //         animSprite.Play("move_left");
    //     }
    // }
    //
    // private void HandleUnitDeselected()
    // {
    //     animSprite.Play("idle");
    // }
    
    private void HandleUnitMoving(Vector2I moveDir)
    {
        // GD.Print($"unit moving in dir: {moveDir}");
        // if (prevMoveDir != Vector2I.Zero && prevMoveDir == moveDir) return;
        // GD.Print($"unit changed directions to {moveDir}");
        switch (moveDir.X)
        {
            case -1:
                PlayAnimation("move_left");
                break;
            case 1:
                PlayAnimation("move_right");
                break;
        }
        switch (moveDir.Y)
        {
            case 1:
                PlayAnimation("move_down");
                break;
            case -1:
                PlayAnimation("move_up");
                break;
        }
    }

    private void HandleUnitStopped()
    {
        PlayAnimation("idle");
    }

    public void PlayAnimation(string name)
    {
        switch (name)
        {
            case "idle":
                animSprite.Offset = Vector2.Zero;
                animSprite.Play("idle");
                break;
            case "move_left":
                animSprite.Offset = new Vector2(-1, -4);
                animSprite.FlipH = false;
                animSprite.Play("move_left");
                break;
            case "move_right":
                animSprite.Offset = new Vector2(-1, -4);
                animSprite.FlipH = true;
                animSprite.Play("move_left");
                break;
            case "move_up":
                animSprite.Offset = new Vector2(0, -3);
                animSprite.Play("move_up");
                break;
            case "move_down":
                animSprite.Offset = new Vector2(0, -3);
                animSprite.Play("move_down");
                break;
        }
    }
}
