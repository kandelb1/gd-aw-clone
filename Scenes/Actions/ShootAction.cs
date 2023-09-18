using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ShootAction : BaseAction
{

    // UnitDefinition has a moved field, but it is set to true even if the unit didn't move anywhere (if the player clicked on the same spot the unit was on)
    // so I'll just create another field here
    private bool unitMoved; // MoveAction will set this for us

    public void SetUnitMoved(bool moved) => unitMoved = moved;

    // I had a lot of trouble with this action so I make an easy (hacky) solution
    // MoveAction will call this function whenever the unit is done moving so we can CalculateValidPositions() from the correct grid position
    // ideally there would be no coupling between actions but I think that's unavoidable in this type of game?
    // the whole action system revolves around where the unit moves to and what actions are available at that position
    public void UpdateValidPositions() => CalculateValidPositions();

    public override string GetActionName() => "Shoot";

    protected override void HandleUnitSelected(Unit otherUnit) {}

    protected override void CalculateValidPositions()
    {
        Vector2I unitPos = unit.GetGridPosition();
        validPositions = new List<Vector2I>();
        int minRange = unit.GetMinWeaponRange();
        int maxRange = unit.GetMaxWeaponRange();
        for (int i = -maxRange; i <= maxRange; i++)
        {
            for (int j = -maxRange; j <= maxRange; j++)
            {
                Vector2I test = new Vector2I(unitPos.X + i, unitPos.Y + j);
                if (!Level.Instance.IsValid(test) || unitPos == test) continue;

                int distance = Math.Abs(i) + Math.Abs(j);
                if (distance < minRange || distance > maxRange) continue;

                if (!Level.Instance.IsOccupied(test)) continue;
                Unit otherUnit = Level.Instance.GetUnit(test);
                if(otherUnit.GetTeam() == unit.GetTeam()) continue;
                if (!unit.CanShootAt(otherUnit)) continue;
                validPositions.Add(test);
            }
        }
    }

    public override bool IsValidPosition(Vector2I pos) => validPositions.Contains(pos);

    public override bool IsActionAvailable()
    {
        if (!unit.IsDirectCombat() && unitMoved) return false;
        return validPositions.Count > 0;
    }

    public override void TakeAction(Vector2I pos)
    {
        // TODO: implement counter-attacks
        // TODO: signal to a BattleManager to initiate the battle instead of applying .Damage() here
        // the BattleManager could handle the cinematic and the whole counter-attack thing
        Unit defendingUnit = Level.Instance.GetUnit(pos);
        defendingUnit.Damage(DamageCalculator.CalculateDamage(unit, defendingUnit));
        unit.SetExhausted(true);
        ActionEventBus.Instance.EmitSignal(ActionEventBus.SignalName.ActionCompleted, this);
    }
}
