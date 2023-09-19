using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnitDefinition;

public partial class AIPlayer : Node
{
    enum AIState
    {
        Inactive,
        Active,
        Waiting,
    }

    private Team team = Team.BlackHole;
    private AIState state;
    // private List<Unit> unitsToMove;
    private Queue<Unit> unitsToMove;
    private Random rng;

    public override void _Ready()
    {
        rng = new Random();
        TurnSystem.Instance.TurnChanged += HandleTurnChanged;
        ActionEventBus.Instance.ActionTaken += HandleActionTaken;
        ActionEventBus.Instance.ActionCompleted += HandleActionCompleted;
    }

    private void HandleTurnChanged()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            state = AIState.Inactive;
            return;
        }
        unitsToMove = new Queue<Unit>(Level.Instance.GetUnitsByTeam(team));
        GD.Print($"TURN START: The AI has {unitsToMove.Count} units to move");
        state = AIState.Active;
    }

    private void HandleActionTaken()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) return;
        state = AIState.Waiting;
    }

    private void HandleActionCompleted(BaseAction action)
    {
        if (TurnSystem.Instance.IsPlayerTurn()) return;
        if (action is MoveAction)
        {
            GD.Print("AI HandleActionCompleted, the unit finished moving so making them wait");
            action.GetUnit().GetAction<WaitAction>().TakeAction(new Vector2I(-1, -1));
        }
        state = AIState.Active;
    }

    public override void _Process(double delta)
    {
        if (state == AIState.Inactive) return;
        if (state == AIState.Waiting)
        {
            GD.Print("AI waiting for action to complete");
            return;
        }
        if (unitsToMove.Count > 0)
        {
            GD.Print($"AI has {unitsToMove.Count} units left to move");
            Unit unit = unitsToMove.Dequeue();
            GD.Print($"AI attempting to move {unit.GetName()}");
            MoveAction moveAction = unit.GetAction<MoveAction>();
            List<Vector2I> validMoves = moveAction.GetValidPositions();
            if (validMoves.Count == 0)
            {
                GD.Print($"AI has no valid moves for {unit.GetName()}, taking WaitAction");
                unit.GetAction<WaitAction>().TakeAction(new Vector2I(-1, -1));
                return;
            }
            int randomMove = rng.Next(validMoves.Count);
            while (!moveAction.IsValidPosition(validMoves[randomMove]))
            {
                GD.Print($"{validMoves[randomMove]} was not a valid move, picking another one");
                randomMove = rng.Next(validMoves.Count);
            }
            GD.Print($"AI {unit.GetName()} is moving to {validMoves[randomMove]}");
            Level.Instance.ConfigureAstarGrid(unit.GetUnitDefinition());
            moveAction.TakeAction(validMoves[randomMove]);
            return;
        }
        GD.Print($"AI finished with turn");
        TurnSystem.Instance.NextTurn();
    }
}
