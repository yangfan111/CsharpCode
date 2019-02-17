using Core.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="ThrowingFireLogic" />
    /// </summary>
    public class ThrowingFireLogic : IFireLogic
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ThrowingFireLogic));

        private IThrowingFactory _throwingFactory;

        private bool _initialized;

        private static int _switchCdTime = 300;

        private ThrowingFireLogicConfig _config;

        public ThrowingFireLogic(

            NewWeaponConfigItem newWeaponConfig,
            ThrowingFireLogicConfig config,
            IWeaponLogicComponentsFactory componentsFactory)
        {
            _config = config;
            _throwingFactory = componentsFactory.CreateThrowingFactory(newWeaponConfig, config.Throwing);
        }

        public void OnFrame(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (!_initialized)
            {
                _initialized = true;
            }

            if (cmd.IsFire && cmd.FilteredInput.IsInput(EPlayerInput.IsLeftAttack))
            {
                DoReady(controller, cmd);
            }

            if (cmd.SwitchThrowMode)
            {
                DoSwitchMode(controller, cmd);
            }

            if (cmd.IsReload && cmd.FilteredInput.IsInput(EPlayerInput.IsReload))
            {
                DoPull(controller, cmd);
            }

            if (cmd.IsThrowing && cmd.FilteredInput.IsInput(EPlayerInput.IsThrowing))
            {
                DoThrowing(controller, cmd);
            }

            //判断打断
            CheckBrokeThrow(controller, cmd);
        }

        //准备
        private void DoReady(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (!controller.RelatedThrowActionInfo.IsReady)
            {
                controller.RelatedThrowActionInfo.IsReady = true;
                controller.RelatedThrowActionInfo.ReadyTime = controller.RelatedTime.ClientTime;
                controller.RelatedThrowActionInfo.Config = _throwingFactory.ThrowingConfig;
                //准备动作
                controller.RelatedStateInterface.InterruptAction();
                controller.RelatedStateInterface.StartFarGrenadeThrow();
            }
        }

        //投掷/抛投切换
        private void DoSwitchMode(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (controller.RelatedThrowActionInfo.IsReady && !controller.RelatedThrowActionInfo.IsThrow)
            {
                if ((controller.RelatedTime.ClientTime - controller.RelatedThrowActionInfo.LastSwitchTime) < _switchCdTime)
                {
                    return;
                }
                controller.RelatedThrowActionInfo.LastSwitchTime = controller.RelatedTime.ClientTime;
                controller.RelatedThrowActionInfo.IsNearThrow = !controller.RelatedThrowActionInfo.IsNearThrow;
                SwitchThrowingMode(controller, controller.RelatedThrowActionInfo.IsNearThrow);
            }
        }

        private void SwitchThrowingMode(PlayerWeaponController controller, bool isNearThrow)
        {
            if (isNearThrow)
            {
                controller.RelatedStateInterface.ChangeThrowDistance(0);
            }
            else
            {
                controller.RelatedStateInterface.ChangeThrowDistance(1);
            }
        }

        //拉栓
        private void DoPull(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (controller.RelatedThrowActionInfo.IsReady && !controller.RelatedThrowActionInfo.IsPull)
            {
                controller.RelatedThrowActionInfo.IsPull = true;
                controller.RelatedThrowActionInfo.LastPullTime = controller.RelatedTime.ClientTime;
                controller.RelatedThrowActionInfo.ShowCountdownUI = true;
                controller.RelatedThrowActionInfo.IsInterrupt = false;
                //生成Entity
                int renderTime = cmd.RenderTime;
                var dir = BulletDirUtility.GetThrowingDir(controller);
                controller.RelatedThrowActionInfo.ThrowingEntityKey = _throwingFactory.CreateThrowing(controller, dir, renderTime, GetInitVel(controller));
                controller.HeldWeaponAgent.RunTimeComponent.LastBulletDir = dir;
                //弹片特效
                if (cmd.IsReload)
                {
                    controller.AddAuxEffect(EClientEffectType.PullBolt);

                }
                //     controller.weaponState.BagLocked = true;
            }
        }

        //投掷
        private void DoThrowing(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (controller.RelatedThrowActionInfo.IsReady && !controller.RelatedThrowActionInfo.IsThrow)
            {
                if (!controller.RelatedThrowActionInfo.IsPull)
                {
                    DoPull(controller, cmd);
                }
                controller.RelatedThrowActionInfo.IsThrow = true;
                controller.RelatedThrowActionInfo.ShowCountdownUI = false;
                //投掷时间
                controller.RelatedThrowActionInfo.LastFireTime = controller.RelatedTime.ClientTime;
                _throwingFactory.UpdateThrowing(controller.RelatedThrowActionInfo.ThrowingEntityKey, true, GetInitVel(controller));
                //投掷动作
                controller.RelatedStateInterface.FinishGrenadeThrow();
                //状态重置
                if (controller.RelatedThrowUpdate != null)
                    controller.RelatedThrowUpdate.IsStartFly = false;
                //投掷型物品使用数量
                controller.RelatedStatics.UseThrowingCount++;
            }
        }

        private void CheckBrokeThrow(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (controller.RelatedThrowActionInfo.IsReady && cmd.FilteredInput.IsInputBlocked(EPlayerInput.IsThrowing))
            {
                //收回手雷
                controller.ForceUnarmCurrWeapon();
                if (controller.RelatedThrowActionInfo.IsPull)
                {
                    //若已拉栓，销毁ThrowingEntity
                    _throwingFactory.DestroyThrowing(controller.RelatedThrowActionInfo.ThrowingEntityKey);
                }
                //拉栓未投掷，打断投掷动作
                controller.RelatedStateInterface.ForceFinishGrenadeThrow();
                controller.RelatedThrowActionInfo.ClearState();
            }
        }

        private float GetInitVel(PlayerWeaponController controller)
        {
            return _throwingFactory.ThrowingInitSpeed(controller.RelatedThrowActionInfo.IsNearThrow);
        }
    }
}
