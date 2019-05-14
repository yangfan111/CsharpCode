using App.Server.GameModules.GamePlay;
using Core;
using Core.EntityComponent;
using Core.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="ThrowingWeaponFireController" />
    /// </summary>
    public class ThrowingWeaponFireController : AbstractFireController
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ThrowingWeaponFireController));

        private IThrowingFactory _throwingFactory;

        private static int _switchCdTime = 300;

        private ThrowingFireLogicConfig _config;

        public ThrowingWeaponFireController(ThrowingFireLogicConfig config, IThrowingFactory factory)
        {
            CleanFireInspector = (WeaponSideCmd cmd) => cmd.UserCmd.IsThrowing;
            _config = config;
            _throwingFactory = factory;//componentsFactory.CreateThrowingFactory(newWeaponConfig, config.Throwing);
        }

       protected override void UpdateFire(PlayerWeaponController controller, WeaponSideCmd cmd, Contexts contexts)
       {
            if (cmd.FiltedInput(EPlayerInput.IsLeftAttack))
            {
                DoReady(controller, cmd);
            }

            if (cmd.SwitchThrowMode)
            {
                DoSwitchMode(controller, cmd);
            }

            if (cmd.FiltedInput(EPlayerInput.IsReload))
            {
                DoPull(controller, cmd);
            }

            if (cmd.FiltedInput(EPlayerInput.IsThrowing))
            {
                DoThrowing(controller, cmd, contexts);
            }         
        }

        //准备
        private void DoReady(PlayerWeaponController controller, WeaponSideCmd cmd)
        {
            var throwAction = controller.RelatedThrowAction;
            if (!throwAction.IsReady && 
                throwAction.ThrowingEntityKey == EntityKey.Default
                && controller.RelatedThrowAction.LastFireWeaponKey == 0)
            {
                if (GlobalConst.EnableWeaponLog)
                    DebugUtil.MyLog("Do Ready");
                throwAction.IsReady = true;
                throwAction.ReadyTime = controller.RelatedTime;
                throwAction.Config = _throwingFactory.ThrowingConfig;
                //准备动作
                controller.RelatedCharState.InterruptAction();
                controller.RelatedCharState.StartFarGrenadeThrow(() =>
                {
                    controller.AutoStuffGrenade();
                    DebugUtil.MyLog("ThrowGrenadeFinished");
                });
                controller.RelatedThrowAction.LastFireWeaponKey = controller.HeldWeaponAgent.WeaponKey.EntityId;
            }
        }

        //投掷/抛投切换
        private void DoSwitchMode(PlayerWeaponController controller, WeaponSideCmd cmd)
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
        private void DoPull(PlayerWeaponController controller, WeaponSideCmd cmd)
        {
            if (controller.RelatedThrowAction.IsReady && !controller.RelatedThrowAction.IsPull)
            {
                if (GlobalConst.EnableWeaponLog)
                    DebugUtil.MyLog("Do Pull");
             
                controller.RelatedThrowAction.IsPull = true;
                controller.RelatedThrowAction.LastPullTime = controller.RelatedTime;
                controller.RelatedThrowAction.ShowCountdownUI = true;
                controller.RelatedThrowAction.IsInterrupt = false;
                //生成Entity
                int renderTime = cmd.UserCmd.RenderTime;
                var dir = BulletDirUtility.GetThrowingDir(controller);
                controller.RelatedThrowAction.ThrowingEntityKey = _throwingFactory.CreateThrowing(controller, dir, renderTime, GetInitVel(controller));
                controller.HeldWeaponAgent.RunTimeComponent.LastBulletDir = dir;
                //弹片特效
                if (cmd.UserCmd.IsReload)
                {
                    controller.AddAuxEffect(EClientEffectType.PullBolt);
                }
                if (controller.AudioController != null)
                    controller.AudioController.PlaySimpleAudio(EAudioUniqueId.GrenadeTrigger, true);
            }
        }

        //投掷
        private void DoThrowing(PlayerWeaponController controller, WeaponSideCmd cmd, Contexts contexts)
        {
            if (controller.RelatedThrowAction.IsReady && !controller.RelatedThrowAction.IsThrow)
            {
                if (GlobalConst.EnableWeaponLog)
                    DebugUtil.MyLog("Do Throwing");
               
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
                else
                {
                    if(controller.AudioController != null)
                        controller.AudioController.PlaySimpleAudio(EAudioUniqueId.GrenadeThrow, true);
                }

            }
            //controller.RelatedThrowAction.ThrowingEntityKey = EntityKey.Default;
        }

        protected override bool CheckInterrupt(PlayerWeaponController controller, WeaponSideCmd cmd)
        {
            if (controller.RelatedThrowAction.IsReady && 
                cmd.FiltedInput(EPlayerInput.IsThrowingInterrupt))
            {
                //收回手雷
                controller.RelatedThrowAction.IsReady = false;
                if (controller.RelatedThrowAction.IsPull)
                {
                    //若已拉栓，销毁ThrowingEntity
                    _throwingFactory.DestroyThrowing(controller.RelatedThrowAction.ThrowingEntityKey);
                }
                controller.UnArmWeapon(false);
                //拉栓未投掷，打断投掷动作
                controller.RelatedCharState.ForceFinishGrenadeThrow();
                controller.RelatedThrowAction.ClearState();
                return true;
            }
            return false;
        }

        private float GetInitVel(PlayerWeaponController controller)
        {
            return _throwingFactory.ThrowingInitSpeed(controller.RelatedThrowAction.IsNearThrow);
        }
    }
}
