using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Battle : Node2D
{
    private AnimatedSprite2D leftSide;
    private AnimatedSprite2D rightSide;

    [Export] private PackedScene battleUnit;

    private Node leftSideUnits;
    
    public override void _Ready()
    {
        leftSide = GetNode<AnimatedSprite2D>("LeftSide");
        leftSideUnits = GetNode("LeftSide/Units");
        rightSide = GetNode<AnimatedSprite2D>("RightSide");
        
        SetupBattle(10);
    }

    public void SetupBattle(int leftHealth)
    {
        List<Node2D> battlePositions = GetBattlePositions();
        for (int i = 0; i < GetNumberOfUnitsForHealth(leftHealth); i++)
        {
            BattleUnit unit = battleUnit.Instantiate() as BattleUnit;
            unit.Position = battlePositions[i].Position;
            leftSideUnits.AddChild(unit);
            unit.SetLeftSide(true);
        }
    }

    private List<Node2D> GetBattlePositions()
    {
        return leftSide.GetNode("UnitPositions").GetChildren().ToList().ConvertAll(x => x as Node2D);
    }

    // TODO: this function will be different for every unit
    // ie. there's only ever 1 battleship, it doesn't matter the health
    // whereas for infantry/mech infantry it can vary
    private int GetNumberOfUnitsForHealth(int health)
    {
        if(health is <= 10 and >= 9)
        {
            return 5;
        }
        if (health is <= 8 and >= 7)
        {
            return 4;
        }
        if (health is <= 6 and >= 5)
        {
            return 3;
        }
        if (health is <= 4 and >= 3)
        {
            return 2;
        }
        if (health is <= 2 and >= 1)
        {
            return 1;
        }
        return 0;
    }
}
