using Godot;
using System;

public abstract partial class BaseCinematic : Node2D
{
    public abstract void Setup();

    public abstract void SetUnitVisualHealth(int health);

    public abstract void SetLeftSide();
    
    // public abstract void RunIn();

    public abstract void Fire();
}
