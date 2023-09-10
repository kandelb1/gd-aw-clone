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
        enemyUnit.Damage(2);
        unit.SetExhausted(true);
    }

    public override List<Vector2I> GetValidPositions()
    {
        Vector2I unitPos = unit.GetGridPosition();
        int shootDist = unit.GetShootDistance();
        
        List<Vector2I> positions = new List<Vector2I>();
        for (int i = -shootDist; i <= shootDist; i++)
        {
            for (int j = -shootDist; j <= shootDist; j++)
            {
                Vector2I test = new Vector2I(unitPos.X + i, unitPos.Y + j);
                if (!Level.Instance.IsValid(test) || test == unitPos) continue;
                int distance = Math.Abs(i) + Math.Abs(j);
                if (distance > shootDist) continue;
                if (!Level.Instance.IsOccupied(test)) continue;
                if (unit.IsEnemy() == Level.Instance.GetUnit(test).IsEnemy()) continue;
                positions.Add(test);
            }
        }

        return positions;
    }

    // public override bool IsActionAvailable()
    // {
    //     return GetValidPositions().Count != 0;
    // }
}
