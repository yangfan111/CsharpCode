using Assets.Utils.Configuration;
using Core.Utils;
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
            var state = controller.RelatedCharState.GetActionState();
            //开火中：重置拉栓
            if (state == ActionInConfig.SpecialFireHold)
            {
                controller.RelatedCharState.SpecialFireEnd();
                SetPullBolting(controller, true);
                controller.AudioController.PlayPullBoltAudio( controller.HeldWeaponAgent.ConfigId);
                return;
            }
            //拉栓行为结束：拉栓成功
            if (state != ActionInConfig.SpecialFireEnd)
            {
                SetPullBolting(controller, false);
            }
         
//            if (IsFireHold(controller))
//            {
//                EndSpecialFire(controller);
//                SetPullBolting(controller, true);
//                controller.AudioController.PlayPullBoltAudio(weaponAgent.ConfigId);
//                return;
//            }
//            if (!IsFireEnd(controller) && !IsFireHold(controller))
//            {
//                SetPullBolting(controller, false);
//            }
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
                if (controller.RelatedCharState != null)
                {
                    controller.RelatedCharState.SpecialSightsFire(() =>
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
                if (controller.RelatedCharState != null)

                {
                    controller.RelatedCharState.SpecialFire(() =>
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
            var weaponData = controller.HeldWeaponAgent.ClientSyncComponent;
        
            weaponData.IsPullingBolt = value;
            weaponData.PullBoltEnd = !value;
//            if (value)
//               controller.AddPullboltInterrupt();
        }

//        private bool IsFireEnd(PlayerWeaponController controller)
//        {
//            var state = controller.RelatedCharState;
//            return state.GetActionState() == ActionInConfig.SpecialFireEnd;
//        }
//
//        private bool IsFireHold(PlayerWeaponController controller)
//        {
//            return controller.RelatedCharState.GetActionState() == ActionInConfig.SpecialFireHold;
//        }

//        private void EndSpecialFire(PlayerWeaponController controller)
//        {
//            controller.RelatedCharState.SpecialFireEnd();
//        }
    }
}
