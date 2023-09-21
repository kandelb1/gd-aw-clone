using Godot;
using System;
using System.Collections.Generic;
using static UnitDefinition;

public partial class Explosion : AnimatedSprite2D
{
    private UnitDefinition unitDef;

    public override void _Ready()
    {
        StringName name = "land";
        AnimationFinished += HandleAnimationFinished;
        switch(unitDef.GetUnitType())
        {
            case UnitType.Infantry or UnitType.Vehicle:
                name = "land";
                break;
            case UnitType.Copter or UnitType.Plane:
                name = "air";
                break;
            case UnitType.Ship or UnitType.Submarine:
                name = "sea";
                break;
        }

        Position = Level.Instance.GetWorldPosition(unitDef.GetGridPosition());
        Play(name);
        AudioStreamPlayer audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        audio.Play();
        audio.Finished += HandleAudioFinished;
    }

    public void SetUnitDefinition(UnitDefinition unitDef) => this.unitDef = unitDef;

    private void HandleAnimationFinished()
    {
        Visible = false;
    }

    private void HandleAudioFinished()
    {
        QueueFree();
    }
}