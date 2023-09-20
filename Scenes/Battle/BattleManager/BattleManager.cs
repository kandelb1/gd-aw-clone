using Godot;

public partial class BattleManager : Node
{
    public override void _Ready()
    {
        ActionEventBus.Instance.ShootActionTaken += HandleBattleStart;
    }

    // TODO: if I decide to implement the battle cinematics, this is where they would start
    private void HandleBattleStart(ShootAction action, Unit attackingUnit, Unit defendingUnit)
    {
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
}
