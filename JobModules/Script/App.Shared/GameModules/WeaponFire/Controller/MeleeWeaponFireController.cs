using Core.Attack;
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
    public class MeleeWeaponFireController : IWeaponFireController
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeWeaponFireController));

        private const int _maxCD = 5000;

        private MeleeFireLogicConfig _config;

        public MeleeWeaponFireController(MeleeFireLogicConfig config)
        {
            _config = config;
        }

        public void OnUpdate(PlayerWeaponController controller, IWeaponCmd cmd, Contexts contexts)
        {

            var weaponId = controller.HeldWeaponAgent.ConfigId;
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            var nowTime = controller.RelatedTime;
            //var delta = weaponState.NextAttackPeriodStamp - nowTime;
            //delta = weaponState.ContinueAttackEndStamp - nowTime;
            bool isProne = (controller.RelatedCharState.GetCurrentPostureState() == XmlConfig.PostureInConfig.Prone);

            if (cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsLeftAttack) && controller.RelatedThrowAction.ThrowingEntityKey == EntityKey.Default
                && (controller.LastFireWeaponId == controller.HeldWeaponAgent.WeaponKey.EntityId || controller.LastFireWeaponId == 0) && !isProne)
            {
                if (nowTime > weaponState.NextAttackPeriodStamp)
                {
                    // 轻击1
                    weaponState.NextAttackPeriodStamp = nowTime + _config.AttackTotalInterval; //目前表里配的间隔时间是结束后到开始时间
                    weaponState.ContinueAttackStartStamp = nowTime + _config.AttackOneCD;
                    weaponState.ContinueAttackEndStamp = nowTime + _config.ContinousInterval;
                    controller.RelatedCharState.LightMeleeAttackOne(OnAttackAniFinish);
                    AfterAttack(controller, cmd);
                }
                else if (CompareUtility.IsBetween(nowTime, weaponState.ContinueAttackStartStamp, weaponState.ContinueAttackEndStamp))
                {
                    // 轻击2
                    weaponState.ContinueAttackStartStamp = 0;
                    weaponState.ContinueAttackEndStamp = 0;
                    weaponState.NextAttackPeriodStamp = Math.Max(nowTime + _config.AttackOneCD, weaponState.ContinueAttackEndStamp);
                    controller.RelatedCharState.LightMeleeAttackTwo(OnAttackAniFinish);
                    AfterAttack(controller, cmd);
                }
                controller.LastFireWeaponId = controller.HeldWeaponAgent.WeaponKey.EntityId;
            }
            else if (cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsRightAttack) && nowTime >= weaponState.NextAttackPeriodStamp && !isProne)
            {
                controller.RelatedCharState.MeleeSpecialAttack(OnAttackAniFinish);
                weaponState.NextAttackPeriodStamp = nowTime + _config.SpecialDamageInterval;
                AfterAttack(controller, cmd);
            }

            if (!cmd.IsFire)
            {
                controller.RelatedThrowAction.ThrowingEntityKey = EntityKey.Default;
                controller.LastFireWeaponId = 0;
            }
        }

        private void OnAttackAniFinish()
        {
        }

        public void AfterAttack(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            //TODO 声音和特效添加 
            if (cmd.IsFire)
            {
                //  DebugUtil.MyLog("DamageInterval:"+_config.DamageInterval);
                StartMeleeAttack(controller, cmd.RenderTime + _config.DamageInterval,
                    new MeleeAttackInfo { AttackType = MeleeAttckType.LeftMeleeAttack },
                    _config);
            }
            else
            {
                StartMeleeAttack(controller, cmd.RenderTime + _config.SpecialDamageInterval,
                   new MeleeAttackInfo { AttackType = MeleeAttckType.RightMeleeAttack },
                   _config);
            }
            controller.AfterAttack();
        }

        private void StartMeleeAttack(PlayerWeaponController controller, int attackTime, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            controller.CreateSetMeleeAttackInfo(attackInfo, config);
            controller.CreateSetMeleeAttackInfoSync(attackTime);
        }
    }
}
