using Godot;
using System;

public partial class DamageIndicator : Control
{
    [Export] private TileHighlighter tileHighlighter;
    private Label damageLabel;
    private SideSwitchComponent sideSwitch;
    
    public override void _Ready()
    {
        Visible = false;
        damageLabel = GetNode<Label>("%Damage");
        tileHighlighter.ShowDamageAgainstUnit += ShowDamage;
        tileHighlighter.HideDamageAgainstUnit += HideDamage;
        sideSwitch = GetNode<SideSwitchComponent>("SideSwitchComponent");
        sideSwitch.SideSwitched += HandleSideSwitch;
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

    private void HandleSideSwitch(bool leftSide)
    {
        LayoutDirection = leftSide ? LayoutDirectionEnum.Ltr : LayoutDirectionEnum.Rtl;
    }
}
