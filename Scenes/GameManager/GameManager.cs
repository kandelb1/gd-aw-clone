using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    [Signal]
    public delegate void GameOverEventHandler();
    
    [Signal]
    public delegate void PlayerFundsChangedEventHandler(PlayerDefinition player, int newAmount);
    
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
        enemy.FundsChanged += (int newAmount) => EmitSignal(SignalName.PlayerFundsChanged, enemy, newAmount);
        
        player = new PlayerDefinition("Andy", 1000, UnitDefinition.Team.OrangeStar);
        player.FundsChanged += (int newAmount) => EmitSignal(SignalName.PlayerFundsChanged, player, newAmount);
        
        
        Level.Instance.BuildingCaptured += HandleBuildingCaptured;
        TurnSystem.Instance.TurnChanged += HandleTurnChanged;
    }

    public PlayerDefinition GetCurrentPlayer()
    {
        return TurnSystem.Instance.IsPlayerTurn() ? player : enemy;
    }

    private void HandleTurnChanged()
    {
        PlayerDefinition currentPlayer = GetCurrentPlayer();
        foreach (BuildingDefinition building in Level.Instance.GetBuildingsControlledBy(currentPlayer.GetTeam()))
        {
            currentPlayer.AddFunds(building.GetIncome());
        }
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
