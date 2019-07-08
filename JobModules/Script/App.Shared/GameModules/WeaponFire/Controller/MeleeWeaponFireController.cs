using Core;
using Core.EntityComponent;
using Core.Utils;
using System;
using Utils.Compare;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="MeleeWeaponFireController" />
    /// </summary>
    public class MeleeWeaponFireController : AbstractFireController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeWeaponFireController));

        private const int _maxCD = 5000;

        private MeleeFireLogicConfig _config;
        private bool _inSpecialAttack = false;

        public MeleeWeaponFireController(MeleeFireLogicConfig config)
        {
            _config = config;
        }

        protected override void UpdateFire(PlayerWeaponController controller, WeaponSideCmd cmd, Contexts contexts)
        {
            var nowTime          = controller.RelatedTime;
            var runTimeComponent = controller.HeldWeaponAgent.RunTimeComponent;
            if (!cmd.FiltedInput(XmlConfig.EPlayerInput.MeleeAttack) || _inSpecialAttack)
                return;
            if (cmd.FiltedInput(XmlConfig.EPlayerInput.IsLeftAttack) &&
                controller.RelatedThrowAction.ThrowingEntityKey == EntityKey.Default
             && (controller.RelatedThrowAction.LastFireWeaponKey == controller.HeldWeaponAgent.WeaponKey.EntityId ||
                 controller.RelatedThrowAction.LastFireWeaponKey == 0))
            {
                if (nowTime > runTimeComponent.NextAttackPeriodStamp)
                {
                    // 轻击1
                    runTimeComponent.NextAttackPeriodStamp =
                        nowTime + _config.AttackTotalInterval; //目前表里配的间隔时间是结束后到开始时间
                    runTimeComponent.ContinueAttackStartStamp = nowTime + _config.AttackOneCD;
                    runTimeComponent.ContinueAttackEndStamp   = nowTime + _config.ContinousInterval;
                    controller.RelatedCharState.LightMeleeAttackOne(OnAttackAniFinish);
                    AfterAttack(controller, cmd, EMeleeAttackType.Soft);
                }
                else if (CompareUtility.IsBetween(nowTime, runTimeComponent.ContinueAttackStartStamp,
                    runTimeComponent.ContinueAttackEndStamp))
                {
                    // 轻击2
                    runTimeComponent.ContinueAttackStartStamp = 0;
                    runTimeComponent.ContinueAttackEndStamp   = 0;
                    runTimeComponent.NextAttackPeriodStamp = Math.Max(nowTime + _config.AttackOneCD,
                        runTimeComponent.ContinueAttackEndStamp);
                    controller.RelatedCharState.LightMeleeAttackTwo(OnAttackAniFinish);
                    AfterAttack(controller, cmd, EMeleeAttackType.Soft);
                }

                controller.RelatedThrowAction.LastFireWeaponKey = controller.HeldWeaponAgent.WeaponKey.EntityId;
            }
            else if (cmd.FiltedInput(XmlConfig.EPlayerInput.IsRightAttack) &&
                     nowTime >= runTimeComponent.NextAttackPeriodStamp)
            {
                _inSpecialAttack = true;
                controller.RelatedCharState.MeleeSpecialAttack(OnSpecialAttackFinished);
                runTimeComponent.NextAttackPeriodStamp = nowTime + _config.SpecialAttackTotalInterval;
                AfterAttack(controller, cmd, EMeleeAttackType.Hard);
            }
        }

        private void OnSpecialAttackFinished()
        {
            _inSpecialAttack = false;
        }

        private void OnAttackAniFinish()
        {
        }

        public void AfterAttack(PlayerWeaponController controller, WeaponSideCmd cmd, EMeleeAttackType attckType)
        {
            if (cmd.IsFire)
            {
                 DebugUtil.MyLog("DamageInterval:"+_config.DamageInterval);
                StartMeleeAttack(controller, controller.RelatedTime + _config.DamageInterval,
                    new MeleeAttackInfo {AttackType = attckType},
                    _config);
            }
            else
            {
                StartMeleeAttack(controller, controller.RelatedTime  + _config.SpecialDamageInterval,
                    new MeleeAttackInfo {AttackType = attckType},
                    _config);
            }

            controller.AfterAttack();
        }

        private void StartMeleeAttack(PlayerWeaponController controller, int attackTime, MeleeAttackInfo attackInfo,
                                      MeleeFireLogicConfig   config)
        {
            controller.CreateSetMeleeAttackInfo(attackInfo, config);
            controller.CreateSetMeleeAttackInfoSync(attackTime);

            controller.AudioController.PlayMeleeAttackAudio(controller.HeldConfigId, (int) attackInfo.AttackType);
        }
    }
}
