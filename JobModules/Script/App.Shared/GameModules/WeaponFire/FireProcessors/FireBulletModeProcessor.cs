using Core.Common;
using Core.Utils;
using WeaponConfigNs;

namespace App.Shared.GameModules.Weapon.Behavior

{
    /// <summary>
    /// 计算是否可以射击（子弹数，射击CD)
    /// NextAttackTimer, LoadedBulletCount, LastFireTime
    /// </summary>
    public class FireBulletModeProcessor : IFireChecker, IAfterFireProcess, IFrameProcess
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FireBulletModeProcessor));

        public FireBulletModeProcessor()
        {
        }

        public bool IsCanFire(PlayerWeaponController controller, IWeaponCmd weaponCmd)
        {
            WeaponBaseAgent weaponAgent = controller.HeldWeaponAgent;
            if (weaponCmd.RenderTime < weaponAgent.RunTimeComponent.NextAttackTimestamp)
            {
                return false;
            }

            if (weaponAgent.BaseComponent.Bullet <= 0)
            {
                controller.ShowTip(ETipType.FireWithNoBullet);  
                return false;
            }
            EFireMode currentMode = (EFireMode)weaponAgent.BaseComponent.RealFireModel;
            if (currentMode == EFireMode.Manual) 
            {
                if (weaponAgent.RunTimeComponent.IsPrevCmdFire)
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

        public void OnAfterFire(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var heldAgent = controller.HeldWeaponAgent;
            // 爆发模式的攻击间隔单独设定
            if ((EFireMode)heldAgent.BaseComponent.RealFireModel != EFireMode.Burst)
            {
                var commonFireCfg = heldAgent.CommonFireCfg;
                heldAgent.RunTimeComponent.NextAttackTimestamp = cmd.RenderTime + commonFireCfg.AttackInterval;

            }
            heldAgent.RunTimeComponent.LastAttackTimestamp = cmd.RenderTime;
        }

        public void OnFrame(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            var heldAgent = controller.HeldWeaponAgent;

            heldAgent.RunTimeComponent.IsPrevCmdFire = cmd.IsFire;
            if (cmd.IsSwitchFireMode && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsSwitchFireMode))
            {
                var config = heldAgent.DefaultFireModeLogicCfg;
                if (config == null || config == null)
                    return;
                EFireMode mode = (EFireMode)heldAgent.BaseComponent.RealFireModel;
                EFireMode nextMode = config.AvaliableModes[0];
                for (int i = 0; i < config.AvaliableModes.Length; i++)
                {
                    if (config.AvaliableModes[i] == mode)
                    {
                        nextMode = config.AvaliableModes[(i + 1) % config.AvaliableModes.Length];
                    }
                }
                if (nextMode == mode)
                {
                    controller.ShowTip(ETipType.FireModeLocked);

                }
                else
                {
                    ShowFireModeChangeTip(controller, nextMode);
                }
                heldAgent.BaseComponent.FireModel = (int)nextMode;
               // controller.AudioController.SwitchFireMode(nextMode);
                // controller.PlayWeaponSound(XmlConfig.EWeaponSoundType.SwitchFireMode);
            }
        }

        //TODO 提出到System
        //private void PlaySound(PlayerWeaponController controller)
        //{
        //    CommonFireConfig common = GetCommonFireConfig(controller);
        //    if(controller.appearanceInterface.Appearance.IsFirstPerson)
        //    {
        //        Audio.GameAudioMedium.PerformOnGunModelSwitch(common, controller.appearanceInterface.Appearance.GetWeaponP1InHand());
        //    }
        //    else
        //    {
        //        Audio.GameAudioMedium.PerformOnGunModelSwitch(common, controller.appearanceInterface.Appearance.GetWeaponP3InHand());
        //    }
        //}
        private void ShowFireModeChangeTip(PlayerWeaponController controller, EFireMode newFireMode)
        {

            switch (newFireMode)
            {
                case EFireMode.Auto:
                    controller.ShowTip(ETipType.FireModeToAuto);
                    break;
                case EFireMode.Burst:
                    controller.ShowTip(ETipType.FireModeToBurst);

                    break;
                case EFireMode.Manual:
                    controller.ShowTip(ETipType.FireModeToManual);

                    break;
            }
        }
    }
}
