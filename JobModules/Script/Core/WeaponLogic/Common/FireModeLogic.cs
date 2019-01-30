using Core.Common;
using Core.GameModule.System;
using WeaponConfigNs;

namespace Core.WeaponLogic
{
    public class FireBulletModeLogic : IFireBulletModeLogic
    {
        private DefaultFireModeLogicConfig _config;
        private CommonFireConfig _common;
        public FireBulletModeLogic(DefaultFireModeLogicConfig config, CommonFireConfig common)
        {
            _common = common;
            _config = config;
        }

        public int GetFireTimes(IPlayerWeaponState playerWeapon)
        {
            if (playerWeapon.ClientTime < playerWeapon.NextAttackTimer)
            {
                return 0;
            }
            if (playerWeapon.LoadedBulletCount <= 0)
            {
                playerWeapon.ShowNoBulletTip();
                return 0;
            }
            EFireMode currentMode = playerWeapon.FireMode;
            if (currentMode == EFireMode.Manual)
            {
                if (playerWeapon.IsPrevCmdFire)
                    return 0;
                else
                    return 1;
            }
            else if (currentMode == EFireMode.Auto)
            {
                return 1;
            }
            else if (currentMode == EFireMode.Burst)
            {
                return 1; // _config.BurstCount;
            }
            else
                return 0;
        }

        public void AfterFireCmd(IPlayerWeaponState playerWeapon, IWeaponCmd cmd, int bulletCount)
        {
            if (bulletCount > 0)
            {
                // 爆发模式的攻击间隔单独设定
                if (playerWeapon.FireMode != EFireMode.Burst)
                {
                    playerWeapon.NextAttackTimer = (playerWeapon.ClientTime + _common.AttackInterval);
                }

                playerWeapon.LastFireTime = playerWeapon.ClientTime;
            }
        }

        public void OnFrame(IPlayerWeaponState playerWeapon, IWeaponCmd cmd)
        {
            playerWeapon.IsPrevCmdFire = cmd.IsFire;
            if (cmd.IsSwitchFireMode && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsSwitchFireMode))
            {
                EFireMode mode = playerWeapon.FireMode;
                EFireMode nextMode = _config.AvaiableModes[0];
                for (int i = 0; i < _config.AvaiableModes.Length; i++)
                {
                    if (_config.AvaiableModes[i] == mode)
                    {
                        nextMode = _config.AvaiableModes[(i + 1) % _config.AvaiableModes.Length];
                    }
                }
                if(nextMode == mode)
                {
                    playerWeapon.ShowFireModeUnchangeTip();
                }
                else
                {
                    playerWeapon.ShowFireModeChangeTip(nextMode);
                }
                playerWeapon.FireMode = nextMode;
                playerWeapon.OnSwitchMode(_common);
             
            }
        }
    }
}