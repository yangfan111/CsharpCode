using App.Server.GameModules.GamePlay;
using App.Shared.EntityFactory;
using Core;
using Core.CharacterState;
using Core.Utils;
using Utils.Appearance;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    ///     Defines the <see cref="ThrowingWeaponFireController" />
    /// </summary>
    public class ThrowingWeaponFireController : AbstractFireController
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ThrowingWeaponFireController));

        private ICharacterState characterState;
        private int currentTime;
        private WeaponResConfigItem resCfg;
        private ThrowingFireLogicConfig throwCfg;

        public ThrowingWeaponFireController(ThrowingFireLogicConfig throwCfg, WeaponResConfigItem resCfg)
        {
            CleanFireInspector        = cmd => cmd.UserCmd.IsThrowing;
            this.throwCfg             = throwCfg;
            this.resCfg               = resCfg;
        }

        protected override void SyncData(PlayerWeaponController weaponController)
        {
            base.SyncData(weaponController);
            characterState = weaponController.RelatedCharState;
            currentTime    = weaponController.RelatedTime;
        }

        protected override void UpdateFire(PlayerWeaponController controller, WeaponSideCmd cmd, Contexts contexts)
        {
            if (cmd.FiltedInput(EPlayerInput.IsLeftAttack))
            {
                DoReady(controller, cmd);
            }

            if (cmd.SwitchThrowMode)
            {
                DoSwitchMode();
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
            if (!throwingActionData.CanReady())
                return;
            throwingActionData.SetReady(holdAgent.WeaponKey.EntityId, throwCfg.Throwing);
            //准备动作
            Logger.Info("[Tmp]Throw Interrupt");
            characterState.InterruptAction();
            characterState.StartFarGrenadeThrow(() =>
            {
                throwingActionData.IsThrowing = false;
                controller.AutoStuffGrenade();
            });
            controller.AudioController.PlaySimpleAudio(EAudioUniqueId.GrenadeReady, true);
        }

        //投掷/抛投切换
        private void DoSwitchMode()
        {
            if (throwingActionData.TrySetSwitch(currentTime))
            {
                if (throwingActionData.IsNearThrow)
                {
                    characterState.ChangeThrowDistance(0);
                }

                else
                {
                    characterState.ChangeThrowDistance(1);
                }
            }
        }

        //拉栓/不按R进行一次自动拉栓
        private void DoPull(PlayerWeaponController controller, WeaponSideCmd cmd, bool playPullAudio = true)
        {
            if (!throwingActionData.CanPull())
                return;

            //生成Entity
            int renderTime = cmd.UserCmd.RenderTime;
            var throwAmmunitionCalculator = SingletonManager.Get<ThrowAmmunitionCalculator>();
            var dir        = throwAmmunitionCalculator.GetFireDir(0, controller, 0);
            var viewPosition    = throwAmmunitionCalculator.GetFireViewPosition(controller);
            var throwingEntity = ThrowingEntityFactory.CreateThrowingEntity(controller, renderTime, dir, viewPosition,
                throwCfg.Throwing.GetThrowingInitSpeed(throwingActionData.IsNearThrow), resCfg, throwCfg.Throwing);
            throwingActionData.SetPull(throwingEntity.entityKey.Value);
            //弹片特效
            if (cmd.UserCmd.IsReload)
            {
                controller.AddAuxEffect(EClientEffectType.PullBolt);
            }

            if (playPullAudio)
                controller.AudioController.PlaySimpleAudio(EAudioUniqueId.GrenadeTrigger, true);
        }

        //投掷
        private void DoThrowing(PlayerWeaponController controller, WeaponSideCmd cmd, Contexts contexts)
        {
            if (!throwingActionData.CanThrow())
                return;

            if (!throwingActionData.IsPull)
            {
                DoPull(controller, cmd, false);
            }

            var handPos = controller.HandPos;
            DebugUtil.MyLog("Throw");
            throwingActionData.SetThrow(handPos);
            ThrowingEntityFactory.StartThrowingEntityFly(throwingActionData.ThrowingEntityKey, true,
                throwCfg.Throwing.GetThrowingInitSpeed(throwingActionData.IsNearThrow));
            //投掷动作
            characterState.FinishGrenadeThrow();
            //状态重置
            if (controller.RelatedThrowUpdate != null)
                controller.RelatedThrowUpdate.ReadyFly = false;
            //投掷型物品使用数量
            controller.RelatedStatisticsData.UseThrowingCount++;
            controller.RelatedAppearence.UnmountWeaponFromHand();
            controller.RelatedAppearence.UnmountWeaponInPackage(WeaponInPackage.ThrownWeapon);

            if (SharedConfig.IsServer)
            {
                FreeRuleEventArgs args = contexts.session.commonSession.FreeArgs as FreeRuleEventArgs;
                (args.Rule as IGameRule).HandleWeaponFire(contexts,
                    contexts.player.GetEntityWithEntityKey(controller.Owner), resCfg);
                (args.Rule as IGameRule).HandleWeaponState(contexts,
                    contexts.player.GetEntityWithEntityKey(controller.Owner), resCfg.Id);
            }

            controller.AudioController.PlaySimpleAudio(EAudioUniqueId.GrenadeThrow, true);
        }

        protected override bool CheckInterrupt(PlayerWeaponController controller, WeaponSideCmd cmd)
        {
            if (throwingActionData.IsReady && cmd.FiltedInput(EPlayerInput.IsThrowingInterrupt))
            {
                DebugUtil.MyLog("Throw interrupt");
                //拉栓未投掷，打断投掷动作
                characterState.ForceFinishGrenadeThrow();
                controller.UnArmWeapon(false);
                return true;
            }

            return false;
        }
    }
}