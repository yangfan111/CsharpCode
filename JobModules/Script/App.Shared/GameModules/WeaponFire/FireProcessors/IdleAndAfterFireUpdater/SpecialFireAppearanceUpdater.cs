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
            //正常开火拉栓
            if (state == ActionInConfig.SpecialFireHold)
            {
                controller.AudioController.PlayPullBoltAudio(heldAgent.ConfigId);
                controller.RelatedCharState.SpecialFireEnd();
                return;

            }
            if (heldAgent.RunTimeComponent.PullBoltInterrupt && state == ActionInConfig.Null)
            {
                if (heldAgent.BaseComponent.Bullet > 0)
                {
                    var needActionDeal = SingletonManager.Get<WeaponResourceConfigManager>()
                                    .NeedActionDeal(heldAgent.ConfigId, ActionDealEnum.Reload);
                    //只拉栓逻辑
                    SpecialFireAppearance(heldAgent.Owner.WeaponController(), needActionDeal);
                }
                heldAgent.RunTimeComponent.PullBoltInterrupt = false;
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
            controller.HeldWeaponAgent.RunTimeComponent.PullBoltInterrupt = false;
            var isAiming = controller.RelatedCameraSNew.IsAiming();
            if (isAiming)
            {
                relatedCharState.SpecialSightsFire(() =>
                {
                    if (needActionDeal)
                    {
                        relatedAppearance.RemountWeaponOnRightHand();
                    }
                    controller.HeldWeaponAgent.RunTimeComponent.PullBoltFinish = true; 

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
                    controller.HeldWeaponAgent.RunTimeComponent.PullBoltFinish = true; 

                });
            }

            if (needActionDeal)
            {
                relatedAppearance.MountWeaponOnAlternativeLocator();
            }


        }

    }
}