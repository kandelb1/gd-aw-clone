using Godot;
using System;

public partial class BattleshipGunFX : AnimatedSprite2D
{
    enum AnimState
    {
        Fire,
        Smoke,
    }
    
    private AnimState state;
    
    public override void _Ready()
    {
        state = AnimState.Fire;
        Play("fire");
        AnimationFinished += HandleAnimationFinished;
    }

    private void HandleAnimationFinished()
    {
        switch (state)
        {
            case AnimState.Fire:
                Play("smoke");
                state = AnimState.Smoke;
                break;
            case AnimState.Smoke:
                QueueFree();
                break;
        }
    }
}
