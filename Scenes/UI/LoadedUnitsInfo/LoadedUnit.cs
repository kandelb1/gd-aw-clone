using Godot;
using System;

public partial class LoadedUnit : VBoxContainer
{
    private TextureRect image;
    private Label health;
    
    public override void _Ready()
    {
        image = GetNode<TextureRect>("%Image");
        health = GetNode<Label>("%Health");
    }
    
    public void SetUnit(Unit unit)
    {
        image.Texture = unit.GetUnitDefinition().GetSpriteFrames().GetFrameTexture("idle", 0);
        health.Text = unit.GetVisualHealth().ToString();
    }
}
