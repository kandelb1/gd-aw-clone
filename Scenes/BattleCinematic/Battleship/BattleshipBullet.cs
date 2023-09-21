using Godot;
using System;

public partial class BattleshipBullet : AnimatedSprite2D
{
    private Timer timer;
    public override void _Ready()
    {
        // RandomNumberGenerator rand;
        // timer.WaitTime 
        // timer.Timeout += HandleTimeout;
        Play("explode");
        AnimationFinished += HandleAnimationFinished;
    }

    private void HandleTimeout()
    {
        // Play("explode");
    }

    private void HandleAnimationFinished()
    {
        QueueFree();
    }
}
