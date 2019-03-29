using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="SpecialFireActionLogic" />
    /// </summary>
    public class SpecialFireActionLogic : AbstractFireActionLogic<CommonFireConfig>
    {
        public override void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (cmd.IsFire)
            {
                return;
            }
            var weaponAgent = controller.HeldWeaponAgent;
            var weaponState = weaponAgent.RunTimeComponent;
            if (weaponState.PullBolting && !IsFireEnd(controller) && !IsFireHold(controller))
            {
                SetPullBolting(controller, false);
            }
            if (IsFireHold(controller))
            {
                EndSpecialFire(controller);
                SetPullBolting(controller, true);
            }
        }

        protected override void OnAfterFire(PlayerWeaponController controller, bool needActionDeal)
        {
            var weaponData = controller.HeldWeaponAgent.BaseComponent;

            if (weaponData.Bullet > 0)
            {
                SpecialFire(controller, needActionDeal);
            }
            else
            {
                DefaultFire(controller);
            }
        }

        private void SetPullBolting(PlayerWeaponController controller, bool value)
        {
            var weaponData = controller.HeldWeaponAgent.RunTimeComponent;
            if (value)
            {
                var gunSight = controller.RelatedCameraSNew.ViewNowMode == (int)ECameraViewMode.GunSight;
                weaponData.GunSightBeforePullBolting = gunSight;
                weaponData.ForceChangeGunSight = gunSight;
            }
            else
            {
                if (weaponData.GunSightBeforePullBolting)
                {
                    weaponData.ForceChangeGunSight = true;
                    weaponData.GunSightBeforePullBolting = false;
                }
            }
            weaponData.PullBolting = value;
        }

        private bool IsFireEnd(PlayerWeaponController controller)
        {
            var state = controller.RelatedStateInterface;
            return state.GetActionState() == ActionInConfig.SpecialFireEnd;
        }

        private bool IsFireHold(PlayerWeaponController controller)
        {
            return controller.RelatedStateInterface.GetActionState() == ActionInConfig.SpecialFireHold;
        }

        private void EndSpecialFire(PlayerWeaponController controller)
        {
            controller.RelatedStateInterface.SpecialFireEnd();
        }
    }
}
