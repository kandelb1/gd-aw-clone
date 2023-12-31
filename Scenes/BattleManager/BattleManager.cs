using Godot;

public partial class BattleManager : Node
{

    [Export] private CanvasLayer screenUI;

    [Export] private PackedScene explosion;
    // [Export] private BattleCinematic battleCinematic;

    public override void _Ready()
    {
        ActionEventBus.Instance.ShootActionTaken += HandleBattleStart;
        ActionEventBus.Instance.UnitDestroyed += HandleUnitDestroyed;
        // battleCinematic.BattleFinished += HandleBattleCinematicFinished;
    }

    private void HandleBattleStart(ShootAction action, Unit attackingUnit, Unit defendingUnit)
    {
        // battleCinematic.SetupBattle(attackingUnit, defendingUnit);
        // battleCinematic.StartBattle(counterAttack);
        
        DamageUnit(attackingUnit, defendingUnit);
        
        bool counterAttack = true;
        if (!attackingUnit.IsDirectCombat()) counterAttack = false; // nothing can shoot back at indirect units
        if (!defendingUnit.IsDirectCombat()) counterAttack = false; // indirect units can't shoot back
        if (defendingUnit.GetHealth() == 0) counterAttack = false; // dead units can't shoot back
        if (DamageCalculator.GetWeaponToUse(defendingUnit, attackingUnit) == null) counterAttack = false; // units that can't shoot back...can't shoot back
        if (counterAttack)
        {
            DamageUnit(defendingUnit, attackingUnit);            
        }
        // signal that we're done (I don't think it's necessary but lets be consistent)
        ActionEventBus.Instance.EmitSignal(ActionEventBus.SignalName.ActionCompleted, action);
    }
    
    // private void HandleBattleCinematicFinished()
    // {
    //     
    // }

    private void DamageUnit(Unit attackingUnit, Unit defendingUnit)
    {
        Weapon weaponToUse = DamageCalculator.GetWeaponToUse(attackingUnit, defendingUnit);
        if (weaponToUse == null)
        {
            GD.PrintErr($"BattleManager: battle between {attackingUnit.GetName()} and {defendingUnit.GetName()} cannot occur - no viable weapon");
            return;
        }
        int damage = DamageCalculator.CalculateDamageWithWeapon(attackingUnit, weaponToUse, defendingUnit);
        defendingUnit.Damage(damage);
        weaponToUse.SetCurrentAmmo(weaponToUse.GetCurrentAmmo() - 1);
    }

    private void HandleUnitDestroyed(BaseUnit unit)
    {
        unit.QueueFree();
        Explosion x = explosion.Instantiate<Explosion>();
        x.SetUnitDefinition(unit.GetUnitDefinition());
        AddChild(x);
    }
}
