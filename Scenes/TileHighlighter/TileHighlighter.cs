using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class TileHighlighter : Node
{
    [Export] private Color moveColor;
    [Export] private Color shootColor;
    // [Export] private Color shootRangeColor;
    [Export] private Color genericColor;

    private AttackCursor attackCursor;

    public override void _Ready()
    {

        attackCursor = GetNode<AttackCursor>("AttackCursor");

        Level.Instance.MouseChangedPosition += HandleMouseMove;
        UnitSystem.Instance.ActionSelected += HandleActionSelected;
        UnitSystem.Instance.ActionTaken += HandleActionTaken;
        UnitSystem.Instance.UnitDeselected += HandleUnitDeselected;
    }
    
    private void HandleUnitDeselected()
    {
        ClearEverything();
    }
    
    private void HandleMouseMove(Vector2I gridPos)
    {
        Unit unit = UnitSystem.Instance.GetSelectedUnit();
        if (unit == null) return;

        Vector2I[] path;
        switch (UnitSystem.Instance.GetSelectedAction())
        {
            // Level.GetPath() is an expensive call
            case MoveAction moveAction:
                if (!ValidActionPositionsCache.Instance.GetCachedPositions().Contains(gridPos))
                {
                    Level.Instance.ClearLayer(Level.ARROW_LAYER);
                    return;
                }
                path = Level.Instance.GetPath(unit.GetGridPosition(), gridPos, unit.GetMoveDefinition());
                Level.Instance.DrawArrowAlongPath(new Array<Vector2I>(path), true);
                break;
            case ShootAction shootAction:
                if (!shootAction.GetValidPositions().Contains(gridPos)) return; // TODO: shouldnt this use ValidActionPositionsCache?
                attackCursor.ShowCursorAt(gridPos);
                break;
            case LoadAction loadAction:
                if (!ValidActionPositionsCache.Instance.GetCachedPositions().Contains(gridPos))
                {
                    Level.Instance.ClearLayer(Level.ARROW_LAYER);
                    return;
                }
                path = Level.Instance.GetPath(unit.GetGridPosition(), gridPos, unit.GetMoveDefinition(), true);
                Level.Instance.DrawArrowAlongPath(new Array<Vector2I>(path), true);
                break;
        }
    }

    private void HandleActionSelected(BaseAction action)
    {
        GD.Print("TileHighlighter HandleActionSelected()");
        switch (action)
        {
            case ShootAction shootAction:
                Level.Instance.HighlightTiles(ValidActionPositionsCache.Instance.GetCachedPositions(), shootColor);
                break;
            case MoveAction moveAction:
                Level.Instance.HighlightTiles(ValidActionPositionsCache.Instance.GetCachedPositions(), moveColor);
                break;
            default:
                Level.Instance.HighlightTiles(ValidActionPositionsCache.Instance.GetCachedPositions(), genericColor);
                break;
        }
    }

    private void HandleActionTaken()
    {
        ClearEverything();
    }

    private void ClearEverything()
    {
        Level.Instance.ClearLayer(Level.HIGHLIGHT_LAYER);
        Level.Instance.ClearLayer(Level.ARROW_LAYER);
        attackCursor.HideCursor();
    }

}
