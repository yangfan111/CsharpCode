using Core.Attack;
using Core.Utils;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="MeleeFireLogic" />
    /// </summary>
    public class MeleeFireLogic : IFireLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeFireLogic));

        private const int _maxCD = 5000;

        private MeleeFireLogicConfig _config;

        public MeleeFireLogic(MeleeFireLogicConfig config)
        {
            _config = config;
        }

        public void OnFrame(PlayerWeaponController controller, IWeaponCmd cmd)
        {

            if (null != cmd.FilteredInput && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.MeleeAttack))
            {
                var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
                if (weaponState.MeleeAttacking)
                {
                    if (controller.RelatedTime.ClientTime > weaponState.NextAttackingTimeLimit)
                    {
                        weaponState.MeleeAttacking = false;
                    }
                }
                if (weaponState.MeleeAttacking)
                {
                    return;
                }
                if (cmd.IsFire)
                {
                    // 轻击1
                    if (controller.RelatedTime.ClientTime > weaponState.ContinuousAttackTime)
                    {
                        controller.RelatedStateInterface.LightMeleeAttackOne(() =>
                        {
                            weaponState.MeleeAttacking = false;
                            weaponState.ContinuousAttackTime = controller.RelatedTime.ClientTime + _config.ContinousInterval;
                        });
                    }
                    // 轻击2
                    else
                    {
                        controller.RelatedStateInterface.LightMeleeAttackTwo(() =>
                        {
                            weaponState.MeleeAttacking = false;
                            weaponState.ContinuousAttackTime = controller.RelatedTime.ClientTime;
                        });
                    }
                    AfterAttack(controller, cmd);
                }
                else if (cmd.IsSpecialFire)
                {
                    controller.RelatedStateInterface.MeleeSpecialAttack(() =>
                    {
                        weaponState.MeleeAttacking = false;
                    });
                    AfterAttack(controller, cmd);
                }
            }
            else
            {
                if (null == cmd.FilteredInput)
                {
                    Logger.Error("FilteredInput in cmd should never be null !");
                }
            }
        }

        public void AfterAttack(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var weaponState = controller.HeldWeaponAgent.RunTimeComponent;
            weaponState.MeleeAttacking = true;
            weaponState.NextAttackingTimeLimit = controller.RelatedTime.ClientTime + _maxCD;
            //TODO 声音和特效添加 
            if (cmd.IsFire)
            {
                StartMeleeAttack(controller, cmd.RenderTime + _config.AttackInterval,
                    new MeleeAttackInfo { AttackType = MeleeAttckType.LeftMeleeAttack },
                    _config);
            }
            else
            {
                StartMeleeAttack(controller, cmd.RenderTime + _config.SpecialAttackInterval,
                   new MeleeAttackInfo { AttackType = MeleeAttckType.RightMeleeAttack },
                   _config);
            }
            controller.ExpendAfterAttack();
        }

        private void StartMeleeAttack(PlayerWeaponController controller, int attackTime, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            controller.CreateSetMeleeAttackInfoSync(attackTime);
            controller.CreateSetMeleeAttackInfo(attackInfo, config);
        }
    }
}
