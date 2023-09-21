using Godot;
using System;
using System.Collections.Generic;
using static UnitDefinition;

public partial class BattleCinematic : Node2D
{

    [Signal]
    public delegate void BattleFinishedEventHandler();
    
    private Timer timer;
    private Sprite2D leftBackground;
    private Sprite2D rightBackground;

    private const int BACKGROUND_WIDTH = 128;
    private const int BACKGROUND_HEIGHT = 160;
    private static readonly Vector2 BACKGROUND_SIZE = new Vector2(BACKGROUND_WIDTH, BACKGROUND_HEIGHT);
    // maps a terrain name (like 'plains') to the top-left coordinate of the corresponding background in battle-backgrounds.png
    private static readonly Dictionary<string, Vector2I> backgroundCoordsMap = new()
    {
        {"plains", new Vector2I(1, 1)},
        {"road", new Vector2I(130, 1)},
        {"beach", new Vector2I(259, 484)},
        {"river", new Vector2I(388, 484)},
        {"mountains", new Vector2I(517, 484)},
        {"woods", new Vector2I(775, 484)},
        {"sea", new Vector2I(1033, 162)},
        {"sky", new Vector2I(1033, 1)}, // special, for air units.
        {"city", new Vector2I(1, 323)},
        {"factory", new Vector2I(646, 162)},
        {"airport", new Vector2I(775, 162)},
        {"port", new Vector2I(904, 162)},
        {"hq", new Vector2I(1, 162)}, // TODO: this is the orange star HQ, add the other ones
    };

    private BaseCinematic leftCinematic;
    private BaseCinematic rightCinematic;

    private RandomNumberGenerator rng;

    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer.Timeout += HandleTimeout;
        leftBackground = GetNode<Sprite2D>("LeftBackground");
        rightBackground = GetNode<Sprite2D>("RightBackground");
        rng = new RandomNumberGenerator();
    }

    public void SetupBattle(Unit attackingUnit, Unit defendingUnit)
    {
        // for now, lets always put the attacking unit on the left and the defending on the right TODO: implement side-switching
        SetupBackground(leftBackground, attackingUnit);
        SetupBackground(rightBackground, defendingUnit);
        leftCinematic = SetupCinematic(true, attackingUnit);
        rightCinematic = SetupCinematic(false, defendingUnit);
    }
    
    public async void StartBattle(bool counterAttack)
    {
        Show();
        timer.Start();
        leftCinematic.Fire();
        await ToSignal(leftCinematic, "DoneFiring");
        GD.Print("Spawning bullets...");
        rightCinematic.TakeFire(leftCinematic.GetBullet(), leftCinematic.GetBulletsFired(), true);
        
        // await on the fire to finish
        // spawn bullets on the other side?... idk
        if (counterAttack)
        {
            rightCinematic?.Fire();    
        }
    }

    // temporary 
    private void HandleTimeout()
    {
        Hide();
        EmitSignal(SignalName.BattleFinished);
    }

    private void SetupBackground(Sprite2D background, Unit unit)
    {
        Vector2I coords;
        if (unit.GetUnitType() is UnitType.Copter or UnitType.Plane)
        {
            coords = backgroundCoordsMap["sky"];
        }
        else
        {
            coords = backgroundCoordsMap[Level.Instance.GetTerrainName(unit.GetGridPosition())];    
        }
        background.RegionRect = new Rect2(coords, BACKGROUND_SIZE);
    }

    private BaseCinematic SetupCinematic(bool leftSide, Unit unit)
    {
        string unitName = unit.GetName();
        string path = $"res://Scenes/BattleCinematic/{unitName}/{unitName}Cinematic.tscn";
        GD.Print($"Trying to load packed scene from path {path}");
        PackedScene scene = GD.Load<PackedScene>(path);
        BaseCinematic cinematic = scene.Instantiate<BaseCinematic>();
        cinematic.Setup();
        cinematic.SetUnitVisualHealth(unit.GetVisualHealth());
        cinematic.SetUnitTeam(unit.GetTeam());
        if (leftSide)
        {
            cinematic.SetLeftSide();
            cinematic.Position = new Vector2(40, -48); // TODO: what's the best way to place them?
            leftBackground.AddChild(cinematic);
        }
        else
        {
            cinematic.Position = new Vector2(-40, -48);
            rightBackground.AddChild(cinematic);
        }
        return cinematic;
    }

    // TODO: the plains background changes depending on what tiles are surrounding it. implement these later
    private bool IsPlainsNextToRoad(Vector2I pos) => true;
    
    private bool IsPlainsNextToMountain(Vector2I pos) => true;
    
    private bool IsPlainsNextToCity(Vector2I pos) => true;
}
