using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    [Signal]
    public delegate void GameOverEventHandler();
    
    private PlayerDefinition player;
    private PlayerDefinition enemy;
    
    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.Print("There is already an instance of GameManager??");
            QueueFree();
            return;
        }
        Instance = this;
        
        enemy = new PlayerDefinition("Hawke", 1000, UnitDefinition.Team.BlackHole);
        player = new PlayerDefinition("Andy", 1000, UnitDefinition.Team.OrangeStar);
        
        
        Level.Instance.BuildingCaptured += HandleBuildingCaptured;
    }

    public PlayerDefinition GetCurrentPlayer()
    {
        return TurnSystem.Instance.IsPlayerTurn() ? player : enemy;
    }

    private void HandleBuildingCaptured(BuildingDefinition buildingDef)
    {
        if (buildingDef.GetBuildingName() == "Hq")
        {
            // game over
            GD.Print($"Game over, {buildingDef.GetControllingTeam()} captured headquarters.");
            GetTree().Quit();
        }
    }
}
