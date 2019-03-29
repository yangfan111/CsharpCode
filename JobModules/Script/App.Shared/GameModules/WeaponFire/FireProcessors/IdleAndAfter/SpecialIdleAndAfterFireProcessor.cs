using Assets.Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="SpecialIdleAndAfterFireProcess" />
    /// </summary>
    public class SpecialIdleAndAfterFireProcessor : CommonIdleAndAfterFireProcessor
    {
        public override void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (cmd.IsFire)
            {
                return;
            }
            var weaponAgent = controller.HeldWeaponAgent;
            if (weaponAgent.RunTimeComponent.PullBolting && !IsFireEnd(controller) && !IsFireHold(controller))
            {
                SetPullBolting(controller, false);
            }
            if (IsFireHold(controller))
            {
                EndSpecialFire(controller);
                SetPullBolting(controller, true);
                controller.AudioController.PlayPullBoltAudio(weaponAgent.ConfigId);
            }
        }

        public override  void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
           
            var weaponData = controller.HeldWeaponAgent.BaseComponent;

            if (weaponData.Bullet > 0)
            {
                var needActionDeal =  SingletonManager.Get<WeaponResourceConfigManager>().NeedActionDeal(weaponData.ConfigId, ActionDealEnum.Reload);
                SpecialFire(controller, needActionDeal);
            }
            else
            {
                base.OnAfterFire(controller,cmd); 
            }
        }
        protected void SpecialFire(PlayerWeaponController controller, bool needActionDeal)
        {
            if (controller.RelatedCameraSNew.ViewNowMode == (int)ECameraViewMode.GunSight)
            {
                if (controller.RelatedStateInterface != null)
                {
                    controller.RelatedStateInterface.SpecialSightsFire(() =>
                    {
                        if (needActionDeal)
                        {
                            controller.RelatedAppearence.RemountWeaponOnRightHand();
                        }
                    });
                }
            }
            else
            {
                if (controller.RelatedStateInterface != null)

                {
                    controller.RelatedStateInterface.SpecialFire(() =>
                    {
                        if (needActionDeal)
                        {
                            controller.RelatedAppearence.RemountWeaponOnRightHand();
                        }
                    });
                }
            }
            // controller.PlayWeaponSound(EWeaponSoundType.LeftFire1);
            if (needActionDeal)
            {
                controller.RelatedAppearence.MountWeaponOnAlternativeLocator();
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
