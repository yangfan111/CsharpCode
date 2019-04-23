using App.Shared.Components.Weapon;
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
    public class SpecialFireAppearanceUpdater : CommonFireAppearanceUpdater
    {
        public override void OnIdle(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (cmd.IsFire)
            {
                return;
            }

            var heldAgent = controller.HeldWeaponAgent;
            //null->Fire->SpecialFireHold(1)->SpecialFireEnd->null
            if (cmd.FilteredInput.IsInput(EPlayerInput.IsPullboltInterrupt))
            {//当前pullBolting被限制，不执行
                heldAgent.InterruptPullBolt();
                return;
            }

            var state = controller.RelatedCharState.GetActionState();
            var runTimeComponent = heldAgent.RunTimeComponent;
      //     if (!cmd.FilteredInput.IsInput(EPlayerInput.IsPullbolting))
//            {
//                InterruptPullBolt(clientUpdateComponents);
//                return;
//            }

            //开火中：重置拉栓 
            if (state == ActionInConfig.SpecialFireHold ||
                (runTimeComponent.PullBoltInterrupt && state == ActionInConfig.Null))
            {
                runTimeComponent.PullBoltInterrupt = false;
                if (!runTimeComponent.IsPullingBolt)
                {
                    controller.RelatedCharState.SpecialFireEnd();
                    if(controller.AudioController != null)
                        controller.AudioController.PlayPullBoltAudio(heldAgent.ConfigId);
                }
                SetPullBolt(runTimeComponent, true);
             
                return;
            }

            //拉栓行为结束：拉栓成功
            if (runTimeComponent.IsPullingBolt && state != ActionInConfig.SpecialFireEnd)
            {
                SetPullBolt(runTimeComponent, false);
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

        public override void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var weaponData = controller.HeldWeaponAgent.BaseComponent;

            if (weaponData.Bullet > 0)
            {
                var needActionDeal = SingletonManager.Get<WeaponResourceConfigManager>()
                                                     .NeedActionDeal(weaponData.ConfigId, ActionDealEnum.Reload);
                SpecialFire(controller, needActionDeal);
            }
            else
            {
                base.OnAfterFire(controller, cmd);
            }
        }

        protected void SpecialFire(PlayerWeaponController controller, bool needActionDeal)
        {
            var relatedCharState  = controller.RelatedCharState;
            var relatedAppearance = controller.RelatedAppearence;
            if (relatedCharState == null)
                return;
            var isAiming = controller.RelatedCameraSNew.IsAiming();
            if (isAiming)
            {
                relatedCharState.SpecialSightsFire(() =>
                {
                    if (needActionDeal)
                    {
                        relatedAppearance.RemountWeaponOnRightHand();
                    }
                });
            }
            else
            {
                relatedCharState.SpecialFire(() =>
                {
                    if (needActionDeal)
                    {
                        relatedAppearance.RemountWeaponOnRightHand();
                    }
                });
            }

            if (needActionDeal)
            {
                relatedAppearance.MountWeaponOnAlternativeLocator();
            }


            // controller.PlayWeaponSound(EWeaponSoundType.LeftFire1);
        }

        private void SetPullBolt(WeaponRuntimeDataComponent runtimeDataComponent, bool isDoing)
        {
            runtimeDataComponent.IsPullingBolt     = isDoing;
            runtimeDataComponent.PullBoltInterrupt = false;
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