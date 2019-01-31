using WeaponConfigNs;
using Core.Attack;
using Core.Utils;
using App.Shared.WeaponLogic;
using App.Shared;
using App.Shared.GameModules.Weapon;

namespace Core.WeaponLogic.Common
{
    public class MeleeFireLogic : IFireLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeFireLogic));
        private const int _maxCD = 5000;
        private MeleeFireLogicConfig _config;
        private Contexts _contexts;

        public MeleeFireLogic(Contexts contexts, MeleeFireLogicConfig config)
        {
            _config = config;
            _contexts = contexts;
        }

        public void OnFrame(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            if(null != cmd.FilteredInput && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.MeleeAttack)) 
            {
                var weaponState = weaponEntity.weaponRuntimeInfo;
                if(weaponState.MeleeAttacking)
                {
                    if(playerEntity.time.ClientTime > weaponState.NextAttackingTimeLimit)
                    {
                        weaponState.MeleeAttacking = false;
                    }
                }
                if(weaponState.MeleeAttacking)
                {
                    return;
                }
                if(cmd.IsFire)
                {
                    // 轻击1
                    if(playerEntity.time.ClientTime > weaponState.ContinuousAttackTime)
                    {
                        playerEntity.stateInterface.State.LightMeleeAttackOne(() =>
                        {
                            weaponState.MeleeAttacking = false;
                            weaponState.ContinuousAttackTime = playerEntity.time.ClientTime + _config.ContinousInterval;
                        });
                    }
                    // 轻击2
                    else
                    {
                        playerEntity.stateInterface.State.LightMeleeAttackTwo(() =>
                        {
                            weaponState.MeleeAttacking = false;
                            weaponState.ContinuousAttackTime = playerEntity.time.ClientTime;
                        });
                    }
                    AfterAttack(playerEntity, cmd);
                }
                else if( cmd.IsSpecialFire)
                {
                    playerEntity.stateInterface.State.MeleeSpecialAttack(()=> 
                    {
                        weaponState.MeleeAttacking = false;
                    });
                    AfterAttack(playerEntity, cmd);
                }
            }
            else
            {
                if(null == cmd.FilteredInput)
                {
                    Logger.Error("FilteredInput in cmd should never be null !");
                }
            }
        }

        public void AfterAttack(PlayerEntity playerEntity, IWeaponCmd cmd)
        {
            var weaponState = playerEntity.GetWeaponRunTimeInfo(_contexts);
            weaponState.MeleeAttacking = true;
            weaponState.NextAttackingTimeLimit = playerEntity.time.ClientTime + _maxCD;
            //TODO 声音和特效添加 
            if(cmd.IsFire)
            {
                StartMeleeAttack(playerEntity, cmd.RenderTime + _config.AttackInterval,
                    new MeleeAttackInfo { AttackType = MeleeAttckType.LeftMeleeAttack },
                    _config);
            }
            else
            {
                StartMeleeAttack(playerEntity, cmd.RenderTime + _config.SpecialAttackInterval,
                   new MeleeAttackInfo { AttackType = MeleeAttckType.RightMeleeAttack },
                   _config);
            }
            playerEntity.GetController<PlayerWeaponController>().OnExpend(_contexts, 
                (EWeaponSlotType)playerEntity.bagState.CurSlot);
        }

        private void StartMeleeAttack(PlayerEntity playerEntity, int attackTime, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            if (playerEntity.hasMeleeAttackInfoSync)
            {
                playerEntity.meleeAttackInfoSync.AttackTime = attackTime;
            }
            else
            {
                playerEntity.AddMeleeAttackInfoSync(attackTime);
            }
            if (playerEntity.hasMeleeAttackInfo)
            {
                playerEntity.meleeAttackInfo.AttackInfo = attackInfo;
                playerEntity.meleeAttackInfo.AttackConfig = config;
            }
            else
            {
                playerEntity.AddMeleeAttackInfo();
                playerEntity.meleeAttackInfo.AttackInfo = attackInfo;
                playerEntity.meleeAttackInfo.AttackConfig = config;
            }
        }
    }
}
