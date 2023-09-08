using Godot;
using System;

public partial class BattleUnit : Node2D
{
    private AnimationPlayer animPlayer;
    private AnimatedSprite2D animSprite;
    
    private RandomNumberGenerator rand;
    private Timer timer;
    
    public override void _Ready()
    {
        rand = new RandomNumberGenerator();
        timer = GetNode<Timer>("Timer");
        timer.WaitTime = rand.RandfRange(0, 0.25f);
        timer.Timeout += HandleTimeout; 
        timer.Start();
        
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animSprite = GetNode<AnimatedSprite2D>("Unit");
    }

    private void HandleTimeout()
    {
        animPlayer.Play("battle_animation");
    }

    public void SetLeftSide(bool leftSide)
    {
        animSprite.FlipH = leftSide;
    }
}

