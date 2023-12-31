using Godot;
using System;

public partial class UnitInfo : PanelContainer
{
    private Label name;
    private TextureRect image;
    private Label health;
    private Label ammo;
    private TextureRect ammoIcon;
    
    public override void _Ready()
    {
        name = GetNode<Label>("%Name");
        image = GetNode<TextureRect>("%Image");
        health = GetNode<Label>("%Health");
        ammo = GetNode<Label>("%Ammo");
        ammoIcon = GetNode<TextureRect>("%AmmoIcon");
        Level.Instance.MouseChangedPosition += HandleMouseMoved;
    }
    
    private void HandleMouseMoved(Vector2I pos)
    {
        if (!Level.Instance.IsOccupied(pos))
        {
            Visible = false;
        }
        else
        {
            Unit unit = Level.Instance.GetUnit(pos);
            name.Text = unit.GetName();
            image.Texture = unit.GetUnitDefinition().GetSpriteFrames().GetFrameTexture("idle", 0);
            health.Text = unit.GetVisualHealth().ToString();
            if (unit.GetPrimaryWeapon() != null)
            {
                ammo.Text = unit.GetPrimaryWeapon().GetCurrentAmmo().ToString();
                ammo.Visible = true;
                ammoIcon.Visible = true;
            }
            else
            {
                ammo.Visible = false;
                ammoIcon.Visible = false;
            }
            Visible = true;
        }
    }
}
