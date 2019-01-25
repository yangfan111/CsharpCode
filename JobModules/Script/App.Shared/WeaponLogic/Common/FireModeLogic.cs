using App.Shared;
using App.Shared.WeaponLogic;
using Core.Common;
using Core.Utils;
using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.WeaponLogicInterface;
using WeaponConfigNs;

namespace Core.WeaponLogic
{
    /// <summary>
    /// 计算是否可以射击（子弹数，射击CD)
    /// NextAttackTimer, LoadedBulletCount, LastFireTime
    /// </summary>
    public class FireBulletModeLogic : IFireCheck, IAfterFire, IFrame
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FireBulletModeLogic));
        private Contexts _contexts;

        public FireBulletModeLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public bool IsCanFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd weaponCmd)
        {
            var weaponState = weaponEntity.weaponData;
            if (weaponCmd.RenderTime < weaponState.NextAttackTimer)
            {
                return false;
            }
            if (weaponState.Bullet <= 0)
            {
                if(playerEntity.hasTip)
                {
                    playerEntity.tip.TipType = ETipType.FireWithNoBullet;
                }
                return false;
            }
            EFireMode currentMode = (EFireMode)weaponState.FireMode; 
            if (currentMode == EFireMode.Manual)
            {
                if (weaponState.IsPrevCmdFire)
                    return false;
                else
                    return true;
            }
            else if (currentMode == EFireMode.Auto)
            {
                return true;
            }
            else if (currentMode == EFireMode.Burst)
            {
                return true; // _config.BurstCount;
            }
            else
            {
                return false;
            }
        }

        public void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var weaponState = weaponEntity.weaponData;
            // 爆发模式的攻击间隔单独设定
            if ((EFireMode)weaponState.FireMode != EFireMode.Burst)
            {
                var common = GetCommonFireConfig(playerEntity);
                if(null != common)
                {
                    weaponState.NextAttackTimer = (cmd.RenderTime + common.AttackInterval);
                }
                else
                {
                    LogError("common config is null");
                }
            }

            weaponState.LastFireTime = cmd.RenderTime;
        }

        public void OnFrame(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var weaponState = weaponEntity.weaponData;
            weaponState.IsPrevCmdFire = cmd.IsFire;
            if (cmd.IsSwitchFireMode && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsSwitchFireMode))
            {
                var config = GetConfig(playerEntity);
                if(null == config)
                {
                    return;
                }
                EFireMode mode = (EFireMode)weaponState.FireMode;
                EFireMode nextMode = config.AvaiableModes[0];
                for (int i = 0; i < config.AvaiableModes.Length; i++)
                {
                    if (config.AvaiableModes[i] == mode)
                    {
                        nextMode = config.AvaiableModes[(i + 1) % config.AvaiableModes.Length];
                    }
                }
                if(nextMode == mode)
                {
                    if(playerEntity.hasTip)
                    {
                        playerEntity.tip.TipType = ETipType.FireModeLocked;
                    }
                    else
                    {
                        LogError("playerEntity has no tip");
                    }
                }
                else
                {
                    ShowFireModeChangeTip(playerEntity, nextMode);
                }
                weaponState.FireMode = (int)nextMode;
                playerEntity.PlayWeaponSound(XmlConfig.EWeaponSoundType.SwitchFireMode);
            }
        }

        //TODO 提出到System
        //private void PlaySound(PlayerEntity playerEntity)
        //{
        //    CommonFireConfig common = GetCommonFireConfig(playerEntity);
        //    if(playerEntity.appearanceInterface.Appearance.IsFirstPerson)
        //    {
        //        Audio.GameAudioMedium.PerformOnGunModelSwitch(common, playerEntity.appearanceInterface.Appearance.GetWeaponP1InHand());
        //    }
        //    else
        //    {
        //        Audio.GameAudioMedium.PerformOnGunModelSwitch(common, playerEntity.appearanceInterface.Appearance.GetWeaponP3InHand());
        //    }
        //}

        private void ShowFireModeChangeTip(PlayerEntity playerEntity, EFireMode newFireMode)
        {
            if(!playerEntity.hasTip)
            {
                LogError("playerEntity has not tip");
                return;
            }
            switch (newFireMode)
            {
                case EFireMode.Auto:
                    playerEntity.tip.TipType = ETipType.FireModeToAuto;
                    break;
                case EFireMode.Burst:
                    playerEntity.tip.TipType = ETipType.FireModeToBurst;
                    break;
                case EFireMode.Manual:
                    playerEntity.tip.TipType = ETipType.FireModeToManual;
                    break;
            }
        }

        private DefaultFireModeLogicConfig GetConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.DefaultFireModeLogicCfg;
            }
            LogError("GetFireModeLogicConfig Failed");
            return null;
        }

        private CommonFireConfig GetCommonFireConfig(PlayerEntity playerEntity)
        {
            var config = playerEntity.GetWeaponConfig(_contexts);
            if(null != config)
            {
                return config.CommonFireCfg;
            }
            LogError("GetCommonFireConfig Failed");
            return null;
        }

        private void LogError(string msg)
        {
            Logger.Error(msg);
            System.Console.WriteLine(msg);
        }
    }
}