using Core;

using Core.Utils;
using UltimateFracturing;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior

{
    /// <summary>
    /// 计算是否可以射击（子弹数，射击CD)
    /// NextAttackTimer, LoadedBulletCount, LastFireTime
    /// </summary>
    /// 
    public class SpecialFireModeChecker: FireModeChecker
    {
        public override bool IsCanFire(WeaponAttackProxy attackProxy, WeaponSideCmd weaponCmd)
        {
            if(base.IsCanFire(attackProxy,weaponCmd))
                 return attackProxy.RuntimeComponent.PullBoltFinish;
            return false;

        }
    }
    public class FireModeChecker : IFireChecker
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FireModeChecker));


        public virtual bool IsCanFire(WeaponAttackProxy attackProxy, WeaponSideCmd cmd)
        {
            if (cmd.UserCmd.RenderTime < attackProxy.RuntimeComponent.NextAttackTimestamp)
                return false;
            if (attackProxy.BasicComponent.Bullet <= 0)
            {
                attackProxy.Owner.ShowTip(ETipType.FireWithNoBullet);
                if(cmd.FiltedInput(EPlayerInput.IsLeftAttack))
                    attackProxy.AudioController.PlayEmptyFireAudio();
                return false;
            }

            EFireMode currentMode = (EFireMode) attackProxy.BasicComponent.RealFireModel;
            switch (currentMode)
            {
                case EFireMode.Manual:
                    return !attackProxy.RuntimeComponent.IsPrevCmdFire;
                case EFireMode.Auto:
                    return true;
                case EFireMode.Burst:
                    return !attackProxy.RuntimeComponent.IsPrevCmdFire|| attackProxy.RuntimeComponent.NeedAutoBurstShoot;
                default:
                    return false;
            }
        }

      
    }
}