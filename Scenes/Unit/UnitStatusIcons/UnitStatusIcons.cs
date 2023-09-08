using Godot;
using System;
using System.Collections.Generic;

public partial class UnitStatusIcons : Node2D
{
    private Unit unit;
    
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
        unit = GetNode<Unit>("../Unit");
        healthSprite = GetNode<Sprite2D>("HealthSprite");
        unit.HealthChanged += UpdateHealth;
        UpdateHealth(unit.GetHealth());

        loadSprite = GetNode<Sprite2D>("LoadSprite");
        unit.UnitLoadedOrUnloaded += UpdateLoad;
        UpdateLoad();
    }
    
    private void UpdateHealth(int newHealth)
    {
        if (newHealth is 10 or 0)
        {
            healthSprite.Hide();
        }
        else
        {
            healthSprite.RegionRect = new Rect2(HEALTH_SPRITE_POSITIONS[newHealth], HEALTH_SPRITE_SIZE);
            healthSprite.Show();
        }
    }

    private void UpdateLoad()
    {
        loadSprite.Visible = unit.GetLoadCapacity() != 0 && unit.IsLoadCapacityFull();
    }
}
