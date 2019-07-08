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
        public override bool IsCanFire(PlayerWeaponController controller, WeaponSideCmd weaponCmd)
        {
            if(base.IsCanFire(controller,weaponCmd))
                 return controller.HeldWeaponAgent.RunTimeComponent.PullBoltFinish;
            return false;

        }
    }
    public class FireModeChecker : IFireChecker
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FireModeChecker));


        public virtual bool IsCanFire(PlayerWeaponController controller, WeaponSideCmd cmd)
        {
            WeaponBaseAgent weaponAgent = controller.HeldWeaponAgent;
            if (cmd.UserCmd.RenderTime < weaponAgent.RunTimeComponent.NextAttackTimestamp)
                return false;
            if (weaponAgent.BaseComponent.Bullet <= 0)
            {
                controller.ShowTip(ETipType.FireWithNoBullet);
                if(cmd.FiltedInput(EPlayerInput.IsLeftAttack))
                    controller.AudioController.PlayEmptyFireAudio();
                return false;
            }

            EFireMode currentMode = (EFireMode) weaponAgent.BaseComponent.RealFireModel;
            switch (currentMode)
            {
                case EFireMode.Manual:
                    return !weaponAgent.RunTimeComponent.IsPrevCmdFire;
                case EFireMode.Auto:
                    return true;
                case EFireMode.Burst:
                    return !weaponAgent.RunTimeComponent.IsPrevCmdFire|| weaponAgent.RunTimeComponent.NeedAutoBurstShoot;
                default:
                    return false;
            }
        }

      
    }
}