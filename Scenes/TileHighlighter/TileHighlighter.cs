using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class TileHighlighter : Node
{
    [Export] private Color moveColor;
    [Export] private Color shootColor;
    // [Export] private Color shootRangeColor;
    [Export] private Color genericColor;

    private AttackCursor attackCursor;

    private List<Vector2I> movementRange;

    public override void _Ready()
    {

        attackCursor = GetNode<AttackCursor>("AttackCursor");
        
        UnitSystem.Instance.ActionSelected += HandleActionSelected;
        UnitSystem.Instance.ActionDeselected += HandleActionDeselected;
        UnitSystem.Instance.UnitDeselected += ClearEverything;
        // UnitSystem.Instance.ActionCompleted += ClearEverything;
        
        Level.Instance.MouseChangedPosition += HandleMouseMove;
    }

    private void HandleActionSelected(BaseAction action)
    {
        ClearEverything();
        List<Vector2I> validPositions = action.GetValidPositions();
        switch (action)
        {
            case MoveAction:
                Level.Instance.HighlightTiles(validPositions, moveColor);
                break;
        }
    }

    private void HandleActionDeselected()
    {
        ClearEverything();
    }

    private void HandleMouseMove(Vector2I pos)
    {
        BaseAction selectedAction = UnitSystem.Instance.GetSelectedAction();
        switch (selectedAction)
        {
            case MoveAction moveAction:
                List<Vector2I> validPositions = selectedAction.GetValidPositions();
                if (!validPositions.Contains(pos))
                {
                    Level.Instance.ClearLayer(Level.ARROW_LAYER);
                    return;
                }
                Array<Vector2I> path = Level.Instance.GetPath(selectedAction.GetUnit().GetGridPosition(), pos);
                Level.Instance.DrawArrowAlongPath(path, true);
                break;
        }
    }

    private void ClearEverything()
    {
        Level.Instance.ClearLayer(Level.HIGHLIGHT_LAYER);
        Level.Instance.ClearLayer(Level.ARROW_LAYER);
        attackCursor.HideCursor();
    }

}
