using Godot;
using System;

public partial class CombinedInfoPanel : HBoxContainer
{
    private SideSwitchComponent sideSwitch;
    private bool leftSide;
    
    public override void _Ready()
    {
        sideSwitch = GetNode<SideSwitchComponent>("SideSwitchComponent");
        sideSwitch.SideSwitched += HandleSideSwitch;
    }

    private void HandleSideSwitch(bool leftSide)
    {
        // if (this.leftSide == leftSide) return;
        // this.leftSide = leftSide;
        LayoutDirection = leftSide ? LayoutDirectionEnum.Ltr : LayoutDirectionEnum.Rtl;
    }
}
