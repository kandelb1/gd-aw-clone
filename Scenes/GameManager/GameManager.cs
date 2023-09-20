using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    [Signal]
    public delegate void GameOverEventHandler();
    
    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.Print("There is already an instance of GameManager??");
            QueueFree();
            return;
        }
        Instance = this;

        Level.Instance.BuildingCaptured += HandleBuildingCaptured;
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
