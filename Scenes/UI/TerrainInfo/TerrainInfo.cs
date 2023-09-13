using Godot;
using System;
using System.Reflection.Metadata;

public partial class TerrainInfo : PanelContainer
{

    private Label name;
    private TextureRect image;
    private Label defense;
    
    public override void _Ready()
    {
        name = GetNode<Label>("%Name");
        image = GetNode<TextureRect>("%Image");
        defense = GetNode<Label>("%Defense");
        Level.Instance.MouseChangedPosition += HandleMouseMoved;
    }
    
    private void HandleMouseMoved(Vector2I pos)
    {
        name.Text = Level.Instance.GetTerrainName(pos).Capitalize();
        image.Texture = Level.Instance.GetTexture(pos);
        defense.Text = Level.Instance.GetDefense(pos).ToString();
    }
}
