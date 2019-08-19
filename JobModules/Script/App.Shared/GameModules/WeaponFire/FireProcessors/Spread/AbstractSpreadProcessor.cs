namespace App.Shared.GameModules.Weapon.Behavior
{
    public abstract class AbstractSpreadProcessor:ISpreadProcessor
    {
           
        public void OnIdle(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            Update(attackProxy, cmd);
        }
        protected abstract void Update(WeaponAttackProxy attackProxy, WeaponSideCmd cmd);
        public void OnBeforeFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            Update(attackProxy, cmd);
        }
    }
}