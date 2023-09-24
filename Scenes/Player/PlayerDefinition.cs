using Godot;
using System;
using static UnitDefinition;

public partial class PlayerDefinition : Resource
{
    [Signal]
    public delegate void FundsChangedEventHandler(int newAmount);

    private string name;
    private int funds;
    private Team team;

    public PlayerDefinition(string name, int funds, Team team)
    {
        this.name = name;
        this.funds = funds;
        this.team = team;
    }

    public override string ToString() => $"{name} of {team}: {funds} funds";

    public string GetPlayerName() => name;

    public int GetFunds() => funds;

    public bool CanAfford(int amount) => funds >= amount;

    public void SubtractFunds(int amount)
    {
        funds -= amount;
        if (funds < 0) funds = 0;
        EmitSignal(SignalName.FundsChanged, funds);
    }

    public Team GetTeam() => team;
}
