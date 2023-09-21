using Godot;
using System;

public abstract partial class BaseCinematic : Node2D
{
    // [Signal]
    // public delegate void ShakeScreenEventHandler(double strength);

    [Signal]
    public delegate void DoneFiringEventHandler();

    public abstract PackedScene GetBullet();

    public abstract int GetBulletsFired(); 
    
    public abstract void Setup();

    public abstract void SetUnitVisualHealth(int health);

    public abstract void SetUnitTeam(UnitDefinition.Team team);

    public abstract void SetLeftSide();
    
    // public abstract void RunIn();

    public abstract void Fire();

    public abstract void TakeFire(PackedScene bullet, int amount, bool flipped);
}
