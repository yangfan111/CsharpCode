namespace App.Shared.GameModules.Weapon.Behavior
{
    public abstract class AbstractSpreadProcessor:ISpreadProcessor
    {
        public void OnBeforeFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            Update(controller, cmd);
        }            
           


        public void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            Update(controller, cmd);
        }
        protected abstract void Update(PlayerWeaponController controller, IWeaponCmd cmd);
    }
}