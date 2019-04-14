using App.Server.GameModules.GamePlay;
using Core.EntityComponent;
using Core.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="ThrowingWeaponFireController" />
    /// </summary>
    public class ThrowingWeaponFireController : IWeaponFireController
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ThrowingWeaponFireController));

        private IThrowingFactory _throwingFactory;

        private bool _initialized;

        private static int _switchCdTime = 300;

        private ThrowingFireLogicConfig _config;

        public ThrowingWeaponFireController(ThrowingFireLogicConfig config,  IThrowingFactory factory)
        {
            _config = config;
            _throwingFactory = factory;//componentsFactory.CreateThrowingFactory(newWeaponConfig, config.Throwing);
        }

        public void OnUpdate(PlayerWeaponController controller, IWeaponCmd cmd, Contexts contexts)
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
                DoThrowing(controller, cmd, contexts);
            }

            if (!cmd.IsFire && !cmd.IsThrowing)
            {
                controller.RelatedThrowAction.ThrowingEntityKey = EntityKey.Default;
                controller.LastFireWeaponId = 0;
            }
            //判断打断
            CheckBrokeThrow(controller, cmd);
        }

        //准备
        private void DoReady(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (!controller.RelatedThrowAction.IsReady && controller.RelatedThrowAction.ThrowingEntityKey == EntityKey.Default
                && controller.LastFireWeaponId == 0)
            {
                controller.RelatedThrowAction.IsReady = true;
                controller.RelatedThrowAction.ReadyTime = controller.RelatedTime;
                controller.RelatedThrowAction.Config = _throwingFactory.ThrowingConfig;
                //准备动作
                controller.RelatedCharState.InterruptAction();
                controller.RelatedCharState.StartFarGrenadeThrow();
                controller.LastFireWeaponId = controller.HeldWeaponAgent.WeaponKey.EntityId;
            }
        }

        //投掷/抛投切换
        private void DoSwitchMode(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (controller.RelatedThrowAction.IsReady && !controller.RelatedThrowAction.IsThrow)
            {
                if ((controller.RelatedTime - controller.RelatedThrowAction.LastSwitchTime) < _switchCdTime)
                {
                    return;
                }
                controller.RelatedThrowAction.LastSwitchTime = controller.RelatedTime;
                controller.RelatedThrowAction.IsNearThrow = !controller.RelatedThrowAction.IsNearThrow;
                SwitchThrowingMode(controller, controller.RelatedThrowAction.IsNearThrow);
            }
        }

        private void SwitchThrowingMode(PlayerWeaponController controller, bool isNearThrow)
        {
            if (isNearThrow)
            {
                controller.RelatedCharState.ChangeThrowDistance(0);
            }
            else
            {
                controller.RelatedCharState.ChangeThrowDistance(1);
            }
        }

        //拉栓
        private void DoPull(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (controller.RelatedThrowAction.IsReady && !controller.RelatedThrowAction.IsPull)
            {
                controller.RelatedThrowAction.IsPull = true;
                controller.RelatedThrowAction.LastPullTime = controller.RelatedTime;
                controller.RelatedThrowAction.ShowCountdownUI = true;
                controller.RelatedThrowAction.IsInterrupt = false;
                //生成Entity
                int renderTime = cmd.RenderTime;
                var dir = BulletDirUtility.GetThrowingDir(controller);
                controller.RelatedThrowAction.ThrowingEntityKey = _throwingFactory.CreateThrowing(controller, dir, renderTime, GetInitVel(controller));
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
        private void DoThrowing(PlayerWeaponController controller, IWeaponCmd cmd, Contexts contexts)
        {
            if (controller.RelatedThrowAction.IsReady && !controller.RelatedThrowAction.IsThrow)
            {
                if (!controller.RelatedThrowAction.IsPull)
                {
                    DoPull(controller, cmd);
                }
                controller.RelatedThrowAction.IsThrow = true;
                controller.RelatedThrowAction.ShowCountdownUI = false;
                //投掷时间
                controller.RelatedThrowAction.LastFireTime = controller.RelatedTime;
                _throwingFactory.UpdateThrowing(controller.RelatedThrowAction.ThrowingEntityKey, true, GetInitVel(controller));
                //投掷动作
                controller.RelatedCharState.FinishGrenadeThrow();
                //状态重置
                if (controller.RelatedThrowUpdate != null)
                    controller.RelatedThrowUpdate.ReadyFly = false;
                //投掷型物品使用数量
                controller.RelatedStatisticsData.UseThrowingCount++;
                if (SharedConfig.IsServer)
                {
                    FreeRuleEventArgs args = contexts.session.commonSession.FreeArgs as FreeRuleEventArgs;
                    (args.Rule as IGameRule).HandleWeaponFire(contexts, contexts.player.GetEntityWithEntityKey(controller.Owner), controller.GetWeaponAgent().ResConfig);
                }

                controller.RelatedThrowAction.ThrowingEntityKey = EntityKey.Default;
            }
        }

        private void CheckBrokeThrow(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (controller.RelatedThrowAction.IsReady && !cmd.FilteredInput.IsInput(EPlayerInput.IsThrowing))
            {
                //收回手雷
                controller.UnArmWeapon(false);
                if (controller.RelatedThrowAction.IsPull)
                {
                    //若已拉栓，销毁ThrowingEntity
                    _throwingFactory.DestroyThrowing(controller.RelatedThrowAction.ThrowingEntityKey);
                }
                //拉栓未投掷，打断投掷动作
                controller.RelatedCharState.ForceFinishGrenadeThrow();
                controller.RelatedThrowAction.ClearState();
            }
        }

        private float GetInitVel(PlayerWeaponController controller)
        {
            return _throwingFactory.ThrowingInitSpeed(controller.RelatedThrowAction.IsNearThrow);
        }
    }
}
