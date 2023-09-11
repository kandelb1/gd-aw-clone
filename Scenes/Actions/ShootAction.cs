using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ShootAction : BaseAction
{

    public override string GetActionName() => "Shoot";

    public override void TakeAction(Vector2I gridPos)
    {
        Unit enemyUnit = Level.Instance.GetUnit(gridPos);
        enemyUnit.Damage(DamageCalculator.CalculateDamage(unit, enemyUnit));
        // enemyUnit.Damage(10);
        unit.SetExhausted(true);
    }

    public override List<Vector2I> GetValidPositions()
    {
        Vector2I unitPos = unit.GetGridPosition();
        List<Vector2I> validPositions = new List<Vector2I>();
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
                // if (unit.IsEnemy() == otherUnit.IsEnemy()) continue;
                if (!unit.CanShootAt(otherUnit)) continue;
                validPositions.Add(test);
            }
        }

        return validPositions;
    }

    public override bool IsActionAvailable()
    {
        // an indirect combat unit cannot move and shoot on the same turn
        if (!unit.IsDirectCombat() && unit.HasAlreadyMoved()) return false;
        return base.IsActionAvailable();
    }
}
