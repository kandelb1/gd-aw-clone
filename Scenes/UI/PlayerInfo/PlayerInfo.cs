using Godot;
using System;

public partial class PlayerInfo : PanelContainer
{
    private Label name;
    private Label funds;
    
    public override void _Ready()
    {
        name = GetNode<Label>("%Name");
        funds = GetNode<Label>("%Funds");
        TurnSystem.Instance.TurnChanged += HandleTurnChanged;
        GameManager.Instance.PlayerFundsChanged += HandleFundsChanged; 
        UpdateText();
    }

    private void UpdateText()
    {
        PlayerDefinition currentPlayer = GameManager.Instance.GetCurrentPlayer();
        name.Text = currentPlayer.GetPlayerName();
        funds.Text = $"${currentPlayer.GetFunds()}";        
    }

    private void HandleTurnChanged()
    {
        UpdateText();
    }

    private void HandleFundsChanged(PlayerDefinition player, int newAmount)
    {
        if (GameManager.Instance.GetCurrentPlayer() != player) return;
        funds.Text = $"${newAmount}";
    }
}
