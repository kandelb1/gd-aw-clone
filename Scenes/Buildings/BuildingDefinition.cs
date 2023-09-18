using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using static UnitDefinition;

[GlobalClass]
public partial class BuildingDefinition : Resource
{

    [Signal]
    public delegate void BuildingCapturedEventHandler();
    
    [Export] private string name;
    [Export] private int incomePerTurn;
    
    [Export] private int capturePoints = 20;
    [Export] private Team controllingTeam;
    
    [Export] private bool canBuild;
    [Export] private Array<UnitType> allowedTypes;
    
    private Vector2I gridPos;

    public string GetBuildingName() => name;

    public int GetIncome() => incomePerTurn;

    public int GetCapturePointsRemaining() => capturePoints;

    public Team GetControllingTeam() => controllingTeam;

    public void SetControllingTeam(Team team) => controllingTeam = team;

    public bool CanBuild() => canBuild;

    public List<UnitType> GetAllowedTypes() => allowedTypes.ToList(); // convert between godot array and .net list

    public Vector2I GetGridPosition() => gridPos;

    public void SetGridPosition(Vector2I gridPos) => this.gridPos = gridPos;

    public bool CanUnitCapture(UnitDefinition unitDef) => unitDef.GetUnitType() == UnitType.Infantry && unitDef.GetTeam() != GetControllingTeam();

    public void Capture(Unit unit)
    {
        int amount = unit.GetVisualHealth();
        capturePoints -= amount;
        if (capturePoints <= 0) // captured!
        {
            controllingTeam = unit.GetTeam();
            EmitSignal(SignalName.BuildingCaptured);
        }
    }
}
