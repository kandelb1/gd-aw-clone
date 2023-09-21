using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BattleshipCinematic : BaseCinematic
{

    private Sprite2D body;
    private AnimatedSprite2D guns;
    private AnimatedSprite2D hull;
    
    private AnimationPlayer animPlayer;
    private StringName animName;
    private int visualHealth;
    
    [Export] private PackedScene gunFX;
    private List<Node2D> gunPositions;
    private Node2D gunFXContainer;

    // TODO: implement easy swapping out of sprites/animations for different teams
    public override void Setup()
    {
        body = GetNode<Sprite2D>("Body");
        guns = GetNode<AnimatedSprite2D>("Guns");
        hull = GetNode<AnimatedSprite2D>("Hull");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        gunPositions = GetNode("GunPositions").GetChildren().ToList().ConvertAll(x => x as Node2D).ToList();
        gunFXContainer = GetNode<Node2D>("GunFXContainer");
    }

    public override void SetUnitVisualHealth(int health)
    {
        switch (health)
        {
            case 10 or 9:
                animName = "fire_5";
                break;
            case 8 or 7:
                animName = "fire_4";
                break;
            case 6 or 5:
                animName = "fire_3";
                break;
            case 4 or 3:
                animName = "fire_2";
                break;
            case 2 or 1:
                animName = "fire_1";
                break;
        }
    }

    public override void SetUnitTeam(UnitDefinition.Team team)
    {
        body.Texture = ResourceLoader.Load<Texture2D>($"res://Assets/Cinematics/Battleship/{team}Body.tres");
        guns.SpriteFrames = ResourceLoader.Load<SpriteFrames>($"res://Assets/Cinematics/Battleship/{team}Guns.tres"); 
        hull.SpriteFrames = ResourceLoader.Load<SpriteFrames>($"res://Assets/Cinematics/Battleship/{team}Hull.tres");
    }

    public override void SetLeftSide()
    {
        Scale = new Vector2(-1, Scale.Y);
    }

    public override void Fire()
    {
        animPlayer.Play(animName);
    }

    public void SpawnGunFX(int index)
    {
        Vector2 position = gunPositions[index].Position;
        BattleshipGunFX fx = gunFX.Instantiate<BattleshipGunFX>();
        fx.Position = position;
        gunFXContainer.AddChild(fx);
    }
}
