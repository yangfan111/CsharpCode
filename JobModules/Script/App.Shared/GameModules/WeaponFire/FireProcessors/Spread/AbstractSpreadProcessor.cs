namespace App.Shared.GameModules.Weapon.Behavior
{
    public abstract class AbstractSpreadProcessor:ISpreadProcessor
    {
        public void OnBeforeFire(WeaponBaseAgent agent, WeaponSideCmd cmd)
        {
            Update(agent, cmd);
        }            
           


        public void OnIdle(WeaponBaseAgent agent, WeaponSideCmd cmd)
        {
            Update(agent, cmd);
        }
        protected abstract void Update(WeaponBaseAgent agent, WeaponSideCmd cmd);
    }
}