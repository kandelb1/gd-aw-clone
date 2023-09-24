using Godot;
using System;

public partial class LoadedUnitsInfo : PanelContainer
{
    [Export] private PackedScene loadedUnitScene;
    private HBoxContainer container;

    public override void _Ready()
    {
        container = GetNode<HBoxContainer>("HBoxContainer");
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
            if (!unit.HasUnitsLoaded())
            {
                Visible = false;
                return;
            }
            ClearChildren();
            foreach (Unit loadedUnit in unit.GetLoadedUnits())
            {
                LoadedUnit instance = loadedUnitScene.Instantiate<LoadedUnit>();
                container.AddChild(instance);
                instance.SetUnit(loadedUnit);
            }
            Visible = true;
        }
    }

    private void ClearChildren()
    {
        foreach (Node child in container.GetChildren())
        {
            child.QueueFree();
        }
    }
}
