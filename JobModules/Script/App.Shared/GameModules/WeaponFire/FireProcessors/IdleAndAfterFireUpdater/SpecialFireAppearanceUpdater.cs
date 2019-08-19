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
        LoggerAdapter logger = new LoggerAdapter("SpecialFireAppearanceUpdater");
        protected override void DoIdle(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            //null->Fire->SpecialFireHold(1)->SpecialFireEnd->null
            if (cmd.FiltedInput(EPlayerInput.IsPullboltInterrupt))
            {//当前pullBolting被限制，不执行
                attackProxy.InterruptPullBolt();
                return;
            }
            var state = attackProxy.CharacterState.GetActionState();
            //正常开火拉栓
            if (state == ActionInConfig.SpecialFireHold)
            {
                logger.Info("Normal pullbolt");
                attackProxy.AudioController.PlayPullBoltAudio(attackProxy.WeaponConfigAssy.S_Id);
                attackProxy.CharacterState.SpecialFireEnd();
                attackProxy.RuntimeComponent.FinishPullBolt();
                return;

            }
            if (state == ActionInConfig.Null && attackProxy.BasicComponent.Bullet > 0)
            {
                //只拉栓逻辑
                if (attackProxy.RuntimeComponent.IsPullboltInterrupt)
                {
                    logger.Info("Interrupt pullbolt");
                    attackProxy.RuntimeComponent.FinishPullBolt();
                    attackProxy.AudioController.PlayPullBoltAudio(attackProxy.WeaponConfigAssy.S_Id);
                    attackProxy.CharacterState.SpecialFireEnd();
                }
                    // var needActionDeal = SingletonManager.Get<WeaponResourceConfigManager>()
                    //                 .NeedActionDeal(attackProxy.WeaponConfigAssy.S_Id, ActionDealEnum.Reload);
                 
            }
        }

        public override void OnAfterFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            attackProxy.RuntimeComponent.StartPullBolt();
            if (attackProxy.BasicComponent.Bullet > 0)
            {
                var needActionDeal = SingletonManager.Get<WeaponResourceConfigManager>()
                                                     .NeedActionDeal(attackProxy.WeaponConfigAssy.S_Id, ActionDealEnum.Reload);
                SpecialFireAppearance(attackProxy, needActionDeal);
            }
            else
            {
                base.OnAfterFire(attackProxy, cmd);
            }
        }

        protected void SpecialFireAppearance(WeaponAttackProxy attackProxy, bool needActionDeal)
        {
            var relatedCharState  = attackProxy.CharacterState;
            var relatedAppearance = attackProxy.Appearence;
            if (relatedCharState == null)
                return;
            if (attackProxy.CanFire)
            {
                relatedCharState.SpecialSightsFire(() =>
                {
                    if (needActionDeal)
                    {
                        relatedAppearance.RemountWeaponOnRightHand();
                    }
                    attackProxy.RuntimeComponent.FinishPullBolt(); 

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

                    attackProxy.RuntimeComponent.FinishPullBolt();

                });
            }

            if (needActionDeal)
            {
                relatedAppearance.MountWeaponOnAlternativeLocator();
            }


        }

    }
}