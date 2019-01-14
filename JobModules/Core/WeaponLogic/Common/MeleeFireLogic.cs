using System;
using Core.WeaponLogic.Attachment;
using WeaponConfigNs;
using Core.Attack;
using Core.Utils;

namespace Core.WeaponLogic.Common
{
    public class MeleeFireLogic : AbstractFireLogic<MeleeFireLogicConfig, object>, IFireLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeFireLogic));
        MeleeFireLogicConfig _config;
        IWeaponSoundLogic _soundLogic;
        IWeaponEffectLogic _effectLogic;
        private const int _maxCD = 5000;

        public MeleeFireLogic(MeleeFireLogicConfig config, IWeaponSoundLogic sound, IWeaponEffectLogic effect):base(config)
        {
            _config = config;
            _soundLogic = sound;
            _effectLogic = effect;
        }

        public int GetBulletLimit()
        {
            return 0;
        }

        public int GetSpecialReloadCount()
        {
            return 0;
        }
        public override void Apply(MeleeFireLogicConfig baseConfig, MeleeFireLogicConfig output, object arg)
        {
            
        }
        public void OnFrame(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            if((null != cmd.FilteredInput && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.MeleeAttack)) 
                && playerWeapon.CanMeleeFire())
            {
                if(playerWeapon.MeleeAttacking)
                {
                    if(playerWeapon.ClientTime > playerWeapon.NextAttackingTimeLimit)
                    {
                        playerWeapon.MeleeAttacking = false;
                    }
                }
                if(playerWeapon.MeleeAttacking)
                {
                    return;
                }
                if(cmd.IsFire)
                {
                    // 轻击1
                    if(playerWeapon.ClientTime > playerWeapon.ContinuousAttackTime)
                    {
                        playerWeapon.OnLightMeleeFire(() =>
                        {
                            playerWeapon.MeleeAttacking = false;
                            playerWeapon.ContinuousAttackTime = playerWeapon.ClientTime + _config.AttackInterval;
                        });
                    }
                    // 轻击2
                    else
                    {
                        playerWeapon.OnSecondLightMeleeFire(() =>
                        {
                            playerWeapon.MeleeAttacking = false;
                            playerWeapon.ContinuousAttackTime = playerWeapon.ClientTime;
                        });
                    }
                    AfterAttack(playerWeapon, cmd);
                }
                else if( cmd.IsSpecialFire)
                {
                    playerWeapon.OnMeleeSpecialFire(()=> 
                    {
                        playerWeapon.MeleeAttacking = false;
                    });
                    AfterAttack(playerWeapon, cmd);
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

        public void AfterAttack(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            playerWeapon.MeleeAttacking = true;
            playerWeapon.NextAttackingTimeLimit = playerWeapon.ClientTime + _maxCD;
            if(null != _soundLogic)
            {
                //_soundLogic.PlaySound(playerWeapon.Key, XmlConfig.EWeaponSoundType.LeftFire1);
            }
            if(null != _effectLogic)
            {

            }
            if(cmd.IsFire)
            {
                playerWeapon.StartMeleeAttack(cmd.RenderTime + _config.AttackInterval / 2,
                    new MeleeAttackInfo { AttackType = MeleeAttckType.LeftMeleeAttack },
                    _config);
            }
            else
            {
                playerWeapon.StartMeleeAttack(cmd.RenderTime + _config.SpecialAttackInterval / 2,
                   new MeleeAttackInfo { AttackType = MeleeAttckType.RightMeleeAttack },
                   _config);
            }
            playerWeapon.OnWeaponCost();
        }

        public void SetAttachment(WeaponPartsStruct attachments)
        {
            //没有近战配件
        }

        public void Reset()
        {

        }

        public void SetVisualConfig(ref VisualConfigGroup config)
        {

        }
    }
}
