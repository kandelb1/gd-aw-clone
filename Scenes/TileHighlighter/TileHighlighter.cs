using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;

public partial class TileHighlighter : Node
{

    [Signal]
    public delegate void ShowDamageAgainstUnitEventHandler(Unit attackingUnit, Unit defendingUnit);

    [Signal]
    public delegate void HideDamageAgainstUnitEventHandler();

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
        
        ActionEventBus.Instance.ActionTaken += ClearEverything;
        
        Level.Instance.MouseChangedPosition += HandleMouseMove;
        GD.Print("TileHighlighter._Ready()");
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
            case UnloadAction:
                Level.Instance.HighlightTiles(validPositions, genericColor);
                break;
            case ShootAction:
                Level.Instance.HighlightTiles(validPositions, shootColor);
                break;
        }
    }
    
    private void HandleMouseMove(Vector2I pos)
    {
        if (ActionEventBus.Instance.IsActionActive()) return;
        BaseAction selectedAction = UnitSystem.Instance.GetSelectedAction();
        List<Vector2I> validPositions;
        switch (selectedAction)
        {
            case MoveAction moveAction:
                validPositions = selectedAction.GetValidPositions();
                if (!validPositions.Contains(pos))
                {
                    Level.Instance.ClearLayer(Level.ARROW_LAYER);
                    return;
                }
                Array<Vector2I> path = Level.Instance.GetPath(selectedAction.GetUnit().GetGridPosition(), pos);
                Level.Instance.DrawArrowAlongPath(path, true);
                break;
            case ShootAction shootAction:
                validPositions = selectedAction.GetValidPositions();
                if (!validPositions.Contains(pos))
                {
                    EmitSignal(SignalName.HideDamageAgainstUnit);
                    return;
                }
                EmitSignal(SignalName.ShowDamageAgainstUnit, selectedAction.GetUnit(), Level.Instance.GetUnit(pos));
                break;
        }
    }

    private void HandleActionDeselected()
    {
        EmitSignal(SignalName.HideDamageAgainstUnit);
        ClearEverything();
    }

    private void ClearEverything()
    {
        Level.Instance.ClearLayer(Level.HIGHLIGHT_LAYER);
        Level.Instance.ClearLayer(Level.ARROW_LAYER);
        attackCursor.HideCursor();
    }

}
