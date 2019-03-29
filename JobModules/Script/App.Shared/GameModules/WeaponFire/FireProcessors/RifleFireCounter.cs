namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="RifleFireProcessCounter" />
    /// </summary>
    public class RifleFireProcessCounter : IFireProcessCounter
    {
        public RifleFireProcessCounter()
        {
        }

        public void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd)
        {

            var config = controller.HeldWeaponAgent.RifleFireCounterCfg;
            if (config == null)
                return;
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            if (weaponState.ContinuesShootDecreaseNeeded)
            {
                weaponState.ContinuesShootDecreaseNeeded = false;
                if (weaponState.ContinuesShootCount > config.MaxCount)
                {
                    weaponState.ContinuesShootCount = config.MaxCount;
                }
                weaponState.ContinuesShootDecreaseTimer = controller.RelatedTime + config.DecreaseInitInterval;
            }

            if (weaponState.ContinuesShootCount > 0 && weaponState.ContinuesShootDecreaseTimer <= controller.RelatedTime)
            {
                weaponState.ContinuesShootDecreaseTimer = controller.RelatedTime + config.DecreaseStepInterval;
                weaponState.ContinuesShootCount--;
            }
            else if (weaponState.ContinuesShootCount == 0)
            {
            }
        }

        public void OnBeforeFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            weaponState.ContinuesShootDecreaseNeeded = true;
            weaponState.ContinuesShootCount++;
        }
    }
}
