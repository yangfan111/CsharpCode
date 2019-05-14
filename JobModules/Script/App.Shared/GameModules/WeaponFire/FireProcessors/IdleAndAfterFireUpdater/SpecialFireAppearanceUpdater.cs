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
        protected override void DoIdle(PlayerWeaponController controller, WeaponSideCmd cmd)
        {
            var heldAgent = controller.HeldWeaponAgent;
            //null->Fire->SpecialFireHold(1)->SpecialFireEnd->null
            if (cmd.FiltedInput(EPlayerInput.IsPullboltInterrupt))
            {//当前pullBolting被限制，不执行
                heldAgent.InterruptPullBolt();
                return;
            }
            var state = controller.RelatedCharState.GetActionState();
            var runTimeComponent = heldAgent.RunTimeComponent;

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
                runTimeComponent.PullBoltFinish = true;
            }
        }

        public override void OnAfterFire(WeaponBaseAgent weaponAgent, WeaponSideCmd cmd)
        {
            weaponAgent.RunTimeComponent.PullBoltFinish = false; 
            var weaponData = weaponAgent.BaseComponent;

            if (weaponData.Bullet > 0)
            {
                var needActionDeal = SingletonManager.Get<WeaponResourceConfigManager>()
                                                     .NeedActionDeal(weaponData.ConfigId, ActionDealEnum.Reload);
                SpecialFireAppearance(weaponAgent.Owner.WeaponController(), needActionDeal);
            }
            else
            {
                base.OnAfterFire(weaponAgent, cmd);
            }
        }

        protected void SpecialFireAppearance(PlayerWeaponController controller, bool needActionDeal)
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


        }

        private void SetPullBolt(WeaponRuntimeDataComponent runtimeDataComponent, bool isDoing)
        {
            runtimeDataComponent.IsPullingBolt     = isDoing;
            runtimeDataComponent.PullBoltInterrupt = false;
        }
    }
}