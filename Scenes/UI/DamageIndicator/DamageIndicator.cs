using Godot;
using System;

public partial class DamageIndicator : Control
{
    [Export] private TileHighlighter tileHighlighter;
    private Label damageLabel;
    
    public override void _Ready()
    {
        Visible = false;
        damageLabel = GetNode<Label>("%Damage");
        tileHighlighter.ShowDamageAgainstUnit += ShowDamage;
        tileHighlighter.HideDamageAgainstUnit += HideDamage;
    }

    private void ShowDamage(Unit attackingUnit, Unit defendingUnit)
    {
        int number = DamageCalculator.CalculateDamage(attackingUnit, defendingUnit);
        damageLabel.Text = number.ToString();
        Visible = true;
    }

    private void HideDamage()
    {
        Visible = false;
    }
}
