using Godot;
using System;
using System.Collections.Generic;

public partial class UnitStatusIcons : Node2D
{
    
    [Export] private Unit unit;
    
    private Sprite2D healthSprite;
    private static readonly Vector2 HEALTH_SPRITE_SIZE = new Vector2(8, 7);
    private static readonly Dictionary<int, Vector2> HEALTH_SPRITE_POSITIONS = new Dictionary<int, Vector2>()
    {
        {1, new Vector2(556, 1233)},
        {2, new Vector2(565, 1233)},
        {3, new Vector2(574, 1233)},
        {4, new Vector2(583, 1233)},
        {5, new Vector2(592, 1233)},
        {6, new Vector2(601, 1233)},
        {7, new Vector2(610, 1233)},
        {8, new Vector2(619, 1233)},
        {9, new Vector2(628, 1233)},
    };
    
    private Sprite2D loadSprite;

    public override void _Ready()
    {
        healthSprite = GetNode<Sprite2D>("HealthSprite");
        unit.HealthChanged += UpdateHealth;
        UpdateHealth(unit.GetHealth());

        loadSprite = GetNode<Sprite2D>("LoadSprite");
        unit.UnitLoadedOrUnloaded += UpdateLoad;
        UpdateLoad();
    }
    
    private void UpdateHealth(int newHealth)
    {
        if (newHealth is <= 100 and >= 91 or 0)
        {
            healthSprite.Hide();
        }
        else
        {
            healthSprite.RegionRect = new Rect2(HEALTH_SPRITE_POSITIONS[GetHealthSpriteIndex(newHealth)], HEALTH_SPRITE_SIZE);
            healthSprite.Show();
        }
    }

    private int GetHealthSpriteIndex(int health)
    {
        switch (health)
        {
            case <= 90 and >= 81:
                return 9;
            case <= 80 and >= 71:
                return 8;
            case <= 70 and >= 61:
                return 7;
            case <= 60 and >= 51:
                return 6;
            case <= 50 and >= 41:
                return 5;
            case <= 40 and >= 31:
                return 4;
            case <= 30 and >= 21:
                return 3;
            case <= 20 and >= 11:
                return 2;
            case <= 10 and >= 1:
                return 1;
        }
        GD.PrintErr($"Invalid health given to UnitStatusIcons.GetHealthSpriteIndex(): {health}");
        return 1;
    }

    private void UpdateLoad()
    {
        loadSprite.Visible = unit.GetLoadCapacity() != 0 && unit.IsLoadCapacityFull();
    }
}
